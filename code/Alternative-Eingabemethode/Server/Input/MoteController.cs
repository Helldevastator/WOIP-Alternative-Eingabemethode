using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using WiimoteLib;
using System.IO;

namespace Server.Input
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
        public double rollInterpolated;
        public double distance;

        public double yawRaw;
        public double pitchRaw;
        public double rollRaw;
    }
    
    public enum IRBarConfiguration
    {
        LEFT_BOTTOM = 0,
        RIGHT_BOTTOM = 1,
        LEFT_TOP = 2,
        RIGHT_TOP = 3,
        NONE = 4
    }

    public delegate void StateListener(MoteController sender, MoteState i);

    /// <summary>
    /// Responsible for accessing the Wiimote and correctly reading out the yaw, pitch and rollInterpolated values in relation to the screen pointed to.
    /// </summary>
    public class MoteController : IDisposable
    {
        public event StateListener MoteUpdated;
        private double yaw = 0;
        private double pitch = 0;
        private double rollInterpolated = 0;

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
        private Object calibrationLock = new Object();
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
        private const double distanceFactor = 1;
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
                this.CalculateToDegrees(ws, out yaw,out roll,out pitch);

                this.yaw += yaw;
                this.rollInterpolated += roll;
                this.pitch += pitch;

                IRBarConfiguration configuration = GetIRBarConfiguration(ws, this.rollInterpolated);
                
                //fill into update event
                state.distance = CalculateDistance(ws, configuration);
                state.configuration = configuration;
                state.yaw = this.yaw;
                state.pitch = this.pitch;
                state.roll = this.rollInterpolated;
                state.yawFast = ws.MotionPlusState.YawFast;
                state.rollFast = ws.MotionPlusState.RollFast;
                state.pitchFast = ws.MotionPlusState.PitchFast;
                state.yawRaw = ws.MotionPlusState.RawValues.X;
                state.pitchRaw = ws.MotionPlusState.RawValues.Z;
                state.rollRaw = ws.MotionPlusState.RawValues.Y;

                state.configuration = this.GetIRBarConfiguration(ws, this.rollInterpolated);
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
        /// <param name="receiver"></param>
        /// <param name="args"></param>
        private void wm_WiimoteExtensionChanged(object sender, WiimoteExtensionChangedEventArgs args)
        {
            if (args.Inserted)
                mote.SetReportType(InputReport.IRExtensionAccel, true);
            else
                mote.SetReportType(InputReport.IRAccel, true);
        }
        #endregion

        #region helper methods for listener
        private void CalculateToDegrees(WiimoteState ws, out double yaw, out double roll, out double pitch)
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
        private IRBarConfiguration GetIRBarConfiguration(WiimoteState ws, double rollInterpolated)
        {
            //simplified rotation matrix
            double sine = Math.Sin(rollInterpolated * Math.PI / 180);
            double cosine = Math.Cos(rollInterpolated * Math.PI / 180);
            double rotX = cosine * -sine;
            double rotY = sine * cosine;

            if (ws.IRState.IRSensors.Length < 4)
                return IRBarConfiguration.NONE;

            //rotate
            InputPoint[] points = new InputPoint[4];
            for (int i = 0; i < 4; i++)
            {
                IRSensor s = ws.IRState.IRSensors[i];
                points[i] = new InputPoint(s.Position.X * rotX, s.Position.Y * rotY);
            }

            InputPoint point1 = points[0];
            InputPoint point2 = points[1];  //point which is closest to point1
            InputPoint point3 = null;       //third point which isn't point 2 or 1
            int index = 1;
            double distance = point1.GetDistance(point2);

            //get point with shortest distance in point2
            for (int i = 2; i < 4; i++)
            {
                double d = point1.GetDistance(points[i]);
                if (d < distance)
                {
                    distance = d;
                    point2 = points[i];
                    index = i;
                }
            }

            point3 = index == 1 ? points[2] : points[1];

            //now compare the points and figure out the figure ;)
            bool isRight = false;
            bool isTop = false;
            if (point1.IsHorizontal(point2))
            {
                isTop = point1.IsTopOf(point3);
                isRight = point3.IsRightOf(point1);
            }
            else
            {
                isTop = point3.IsTopOf(point1);
                isRight = point1.IsRightOf(point3);
            }

            //put in result integer and cast it to enum
            int result = isTop << 1;
            result += isRight;
  
            return (IRBarConfiguration)result;
        }

        /// <summary>
        /// Calculates the distance from the wiimote to the screen using the distance between the sensor bar points.
        /// If no points are available, it returns positive infinity
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        private double CalculateDistance(WiimoteState ws,IRBarConfiguration configuration)
        {
            if (configuration == IRBarConfiguration.NONE)
                return Double.PositiveInfinity;

            IRSensor point1 = ws.IRState.IRSensors[0];
            IRSensor point2 = ws.IRState.IRSensors[1];
            float x = point1.Position.X - point2.Position.X;
            float y = point1.Position.Y - point2.Position.Y;
            double distance = Math.Sqrt(x * x + y * y);

            return distance*distanceFactor;
        }

        /// <summary>
        /// Resets the integrated Roll, Pitch and Yaw to counter the gyro'InputState drifting.
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="conf"></param>
        private void ResetGyro(WiimoteState ws, IRBarConfiguration conf)
        {
            
        }
        #endregion

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
            lock (this.calibrationLock)
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
                        this.rollInterpolated = 0;
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
