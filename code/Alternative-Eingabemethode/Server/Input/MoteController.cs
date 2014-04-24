using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using WiimoteLib;

namespace Server
{
    public struct MotionInfo {
        public bool buttonA;
        public bool buttonB;

        public bool yawFast;
        public bool rollFast;
        public bool pitchFast;
        
        public IRBarConfiguration configuration;

        public double yaw;
        public double pitch;
        public double roll;
    }

    public delegate void StateListener(MoteController sender, MotionInfo i);

    public enum IRBarConfiguration
    {
        LEFT_BOTTOM = 0,
        RIGHT_BOTTOM = 1,
        LEFT_TOP = 2,
        RIGHT_TOP = 3
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
        private const double toDegSlow = 1/(8192d / 595d);
        private const double toDegFast = toDegSlow * 2000d / 440d;
        private const double dt = 1 / 200d;
        #endregion

        private Wiimote mote;
        private Object rotLock = new Object();
        private Matrix rot = new Matrix();

        public MoteController(Wiimote mote)
        {
            lock (nextLock)
            {
                this.Id = nextId;
                nextId++;
            }

            this.mote = mote;
            mote.WiimoteChanged += wm_WiimoteChanged;
            mote.WiimoteExtensionChanged += wm_WiimoteExtensionChanged;
            mote.Connect();
            mote.SetReportType(InputReport.IRAccel, true);
            mote.SetLEDs(false, true, true, false);
            mote.InitializeMotionPlus();
        }

        private void wm_WiimoteChanged(object sender, WiimoteChangedEventArgs args)
        {
            WiimoteState ws = args.WiimoteState;

            if (!this.isCalibrating)
            {
                MotionInfo i = new MotionInfo();

                double yaw, roll, pitch;
                this.CalcToDegrees(ws, out yaw,out roll,out pitch);

                Matrix dRotation = Matrix.rotation(yaw,1,0,0).multiply(Matrix.rotation(pitch,0,1,0)).multiply(Matrix.rotation(roll,0,0,1));

                this.yaw += yaw;
                this.roll += roll;
                this.pitch += pitch;

                i.yaw = this.yaw;
                i.pitch = this.pitch;
                i.roll = this.roll;
                i.yawFast = ws.MotionPlusState.YawFast;
                i.rollFast = ws.MotionPlusState.RollFast;
                i.pitchFast = ws.MotionPlusState.PitchFast;
                
                if (this.MoteUpdated != null)
                    this.MoteUpdated.Invoke(this, i);
            }
            else
            {
                this.calibrateMote(ws);
            }
        }

        private void CalcToDegrees(WiimoteState ws, out double yaw, out double roll, out double pitch)
        {
            yaw = ws.MotionPlusState.RawValues.X - this.zeroX;
            yaw = yaw * toDegSlow;
            //yaw = ws.MotionPlusState.YawFast ? yaw * toDegFast : yaw * toDegSlow;
            yaw *= dt;

            roll = ws.MotionPlusState.RawValues.Y - this.zeroY;
            roll = roll * toDegSlow;
            //roll = ws.MotionPlusState.RollFast ? roll * toDegFast : roll * toDegSlow;
            roll *= dt; 

            pitch = ws.MotionPlusState.RawValues.Z - this.zeroZ;
            pitch = pitch * toDegSlow;
            //pitch = ws.MotionPlusState.YawFast ? pitch * toDegFast : pitch * toDegSlow;
            pitch *= -dt;   //because otherwise delta angle is negative when turning upwards
        }

        private void wm_WiimoteExtensionChanged(object sender, WiimoteExtensionChangedEventArgs args)
        {
            if (args.Inserted)
                mote.SetReportType(InputReport.IRExtensionAccel, true);
            else
                mote.SetReportType(InputReport.IRAccel, true);
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
            }
        }
        #endregion
    }
}
