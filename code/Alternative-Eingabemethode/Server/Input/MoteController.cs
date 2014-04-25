using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using WiimoteLib;
using System.IO;

namespace Server
{
    public struct MoteState {
        public bool buttonA;
        public bool buttonB;

        public bool yawFast;
        public bool rollFast;
        public bool pitchFast;
        
        public IRBarConfiguration configuration;

        public double yaw;
        public double pitch;
        public double roll;

        public double yawRaw;
        public double pitchRaw;
        public double rollRaw;
    }

    public delegate void StateListener(MoteController sender, MoteState i);

    public enum IRBarConfiguration
    {
        LEFT_BOTTOM = 0,
        RIGHT_BOTTOM = 1,
        LEFT_TOP = 2,
        RIGHT_TOP = 3,
        NONE = 4
    }

    /// <summary>
    /// Responsible for accessing the Wiimote and correctly reading out the yaw, pitch and roll values in relation to the screen pointed to.
    /// </summary>
    public class MoteController : IDisposable
    {
        public event StateListener MoteUpdated;
        private double yaw = 0;
        private double pitch = 0;
        private double roll = 0;

        private double[] delta;
        private int deltaIndex;
        private Wiimote mote;
        private StreamWriter writer;

        #region id
        private static int nextId = 0;
        private static Object nextLock = new Object();
        public int Id { get; private set; }
        #endregion

        #region calibration fields
        private Object calLock = new Object();
        private double zeroX = 8063d;
        private double zeroY= 8063d;
        private double zeroZ= 8063d;
        

        private double calibrationX ;
        private double calibrationY;
        private double calibrationZ;
        private int cCount = -1;
        private volatile bool isCalibrating = false;
        private static int calibrationCount = 100;
        #endregion

        #region static constants
        private const double toDegFactorSlow = 8192d / 595d;
        private const double fastMultiplier =  2000d / 440d;
        private const double dt = 1 / 200d;
        #endregion

        public MoteController(Wiimote mote)
        {
            lock (nextLock)
            {
                this.Id = nextId;
                nextId++;
            }
            //writer = new StreamWriter("moveBla.csv");

            this.mote = mote;
            mote.WiimoteChanged += wm_WiimoteChanged;
            mote.WiimoteExtensionChanged += wm_WiimoteExtensionChanged;
            mote.Connect();
            mote.SetReportType(InputReport.IRAccel, true);
            mote.SetLEDs(false, true, true, false);
            mote.InitializeMotionPlus();
        }

        #region wiimote listener
        private void wm_WiimoteChanged(object sender, WiimoteChangedEventArgs args)
        {
            WiimoteState ws = args.WiimoteState;

            if (!this.isCalibrating)
            {
                MoteState state = new MoteState();

                /*lock (writer)
                {
                    writer.WriteLine("{0};{1};{2};{3};{4};{5};", ws.MotionPlusState.RawValues.X, ws.MotionPlusState.RawValues.Z, ws.MotionPlusState.RawValues.Y, ws.MotionPlusState.YawFast, ws.MotionPlusState.PitchFast, ws.MotionPlusState.RollFast);
                }*/

                double yaw, roll, pitch;
                this.CalcToDegrees(ws, out yaw,out roll,out pitch);

                this.yaw += yaw;
                this.roll += roll;
                this.pitch += pitch;

                state.yaw = this.yaw;
                state.pitch = this.pitch;
                state.roll = this.roll;
                state.yawFast = ws.MotionPlusState.YawFast;
                state.rollFast = ws.MotionPlusState.RollFast;
                state.pitchFast = ws.MotionPlusState.PitchFast;
                state.yawRaw = ws.MotionPlusState.RawValues.X;
                state.pitchRaw = ws.MotionPlusState.RawValues.Z;
                state.rollRaw = ws.MotionPlusState.RawValues.Y;

                state.configuration = this.GetIRBarConfiguration(ws, this.yaw, this.pitch, this.roll);
                this.ResetGyro(ws, state.configuration);

                if (this.MoteUpdated != null)
                    this.MoteUpdated.Invoke(this, state);
            }
            else
            {
                this.calibrateMote(ws);
            }
        }

        /// <summary>
        /// Listener for extensions changed, not sure if needed. Sometimes the IR and Acceleration sensors only start recording when nunchuck is activated+deactivated again
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void wm_WiimoteExtensionChanged(object sender, WiimoteExtensionChangedEventArgs args)
        {
            if (args.Inserted)
                mote.SetReportType(InputReport.IRExtensionAccel, true);
            else
                mote.SetReportType(InputReport.IRAccel, true);
        }
        #endregion

        private void CalcToDegrees(WiimoteState ws, out double yaw, out double roll, out double pitch)
        {
            yaw = ws.MotionPlusState.RawValues.X - this.zeroX;
            yaw = yaw / toDegFactorSlow;
            yaw = ws.MotionPlusState.YawFast ? yaw * fastMultiplier : yaw;
            yaw *= dt;

            roll = ws.MotionPlusState.RawValues.Y - this.zeroY;
            roll = roll / toDegFactorSlow;
            roll = ws.MotionPlusState.RollFast ? roll * fastMultiplier : roll;
            roll *= dt; 

            pitch = ws.MotionPlusState.RawValues.Z - this.zeroZ;
            pitch = pitch / toDegFactorSlow;
            pitch = ws.MotionPlusState.YawFast ? pitch * fastMultiplier : pitch;
            pitch *= -dt;   //because otherwise delta angle is negative when turning upwards
        }

        /// <summary>
        /// Finds out what IR-Bar Configuration the Mote points to.
        /// </summary>
        /// <param name="ws"></param>
        /// <returns></returns>
        private IRBarConfiguration GetIRBarConfiguration(WiimoteState ws,double yaw, double pitch, double roll)
        {
            
            return IRBarConfiguration.NONE;
        }

        /// <summary>
        /// Resets the integrated Roll, Pitch and Yaw to counter the gyro's drifting.
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="conf"></param>
        private void ResetGyro(WiimoteState ws, IRBarConfiguration conf)
        {
            
        }

        #region calibration method
        public void Calibrate()
        {
            this.isCalibrating = true;
            this.calibrationX = 0;
            this.calibrationY = 0;
            this.calibrationZ = 0;
            this.cCount = 0;
        }

        private void calibrateMote(WiimoteState ws)
        {
            lock (this.calLock)
            {
                if (this.cCount < calibrationCount)
                {
                    this.calibrationX += ws.MotionPlusState.RawValues.X;
                    this.calibrationY += ws.MotionPlusState.RawValues.Y;
                    this.calibrationZ += ws.MotionPlusState.RawValues.Z;
                    this.cCount++;

                    if (this.cCount >= calibrationCount)
                    {
                        this.zeroX = this.calibrationX / cCount;
                        this.zeroY = this.calibrationY / cCount;
                        this.zeroZ = this.calibrationZ / cCount;

                        this.isCalibrating = false;
                        this.yaw = 0;
                        this.pitch = 0;
                        this.roll = 0;
                    }
                }
            }
        }
        #endregion

        #region IDisposable implementation
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disp)
        {
            if (disp)
            {
                if (mote != null) mote.Dispose();
                lock (writer)
                {
                    if (writer != null)
                        writer.Dispose();
                }
            }
        }
        #endregion
    }
}
