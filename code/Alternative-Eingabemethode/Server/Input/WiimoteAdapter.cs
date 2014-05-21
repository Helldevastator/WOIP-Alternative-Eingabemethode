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

    public class BarPoints
    {
        public InputPoint p1;
        public InputPoint p2;
    }

    public struct MoteState {
        public bool buttonA;
        public bool buttonB;

        public bool yawFast;
        public bool rollFast;
        public bool pitchFast;
        
        public IRBarConfiguration configuration;

        public BarPoints vertical;
        public BarPoints horizontal;

        public bool point1;
        public bool point2;
        public bool point3;
        public bool point4;

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

    public delegate void StateListener(WiimoteAdapter sender, MoteState i);

    /// <summary>
    /// Responsible for accessing the Wiimote and correctly reading out the yaw, pitch and rollInterpolated values in relation to the screen pointed to.
    /// </summary>
    public class WiimoteAdapter : IDisposable
    {
        public event StateListener MoteUpdatedEvent;
        private double yaw = 0; //REMOVE
        private double pitch = 0; //REMOVE
        private double rollInterpolated = 0;
        private volatile IRBarConfiguration lastConfiguration = IRBarConfiguration.NONE;

        private Wiimote mote;

        #region resId
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
        private const double correctionFactor = 1;
        public const int IR_PIXEL_WIDTH = 1024;
        public const int IR_PIXEL_HEIGHT = 728;
        #endregion

        public WiimoteAdapter(Wiimote mote)
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
            System.Threading.Thread.Sleep(100); //give wiimote time to react
            mote.SetLEDs(false, true, true, false);
            System.Threading.Thread.Sleep(100); //give wiimote time to react
            mote.InitializeMotionPlus();
        }

        public void ToZero()
        {
            this.yaw = 0;
            this.rollInterpolated = 0;
            this.pitch = 0;
        }

        #region wiimote listener
        private void wm_WiimoteChanged(object sender, WiimoteChangedEventArgs args)
        {
            WiimoteState ws = args.WiimoteState;

            if (!this.isCalibrating)
            {
                MoteState state = new MoteState();

                double yaw, roll, pitch;
                this.CalculateToDegrees(ws, out yaw,out roll,out pitch);

                this.yaw += yaw;
                this.rollInterpolated += roll;
                this.pitch += pitch;

                int irCount = 0;
                for (int i = 0; i < ws.IRState.IRSensors.Length; i++)
                    if (ws.IRState.IRSensors[i].Found)
                        irCount++;

                BarPoints horizontal;
                BarPoints vertical;
                this.GetIRPosition(ws, irCount, out horizontal, out vertical);
                state.vertical = vertical;
                state.horizontal = horizontal;
                IRBarConfiguration configuration = GetIRBarConfiguration(ws,irCount,vertical,horizontal);
                state.configuration = configuration;

               
                state.point1 = ws.IRState.IRSensors[0].Found;
                state.point2 = ws.IRState.IRSensors[1].Found;
                state.point3 = ws.IRState.IRSensors[2].Found;
                state.point4 = ws.IRState.IRSensors[3].Found;

                
                //delete
                state.distance = CalculateDistance(ws, configuration);//fill into update event
                state.yaw = this.yaw;
                state.pitch = this.pitch;
                state.roll = this.rollInterpolated;
                state.yawFast = ws.MotionPlusState.YawFast;
                state.rollFast = ws.MotionPlusState.RollFast;
                state.pitchFast = ws.MotionPlusState.PitchFast;
                state.yawRaw = ws.MotionPlusState.RawValues.X;
                state.pitchRaw = ws.MotionPlusState.RawValues.Z;
                state.rollRaw = ws.MotionPlusState.RawValues.Y;



                if (this.MoteUpdatedEvent != null)
                    this.MoteUpdatedEvent.Invoke(this, state);
            }
            else
            {
                this.calibrateMote(ws);
            }
        }

        /// <summary>
        /// Listener for extensions changed, not sure if needed. Sometimes the IR and Acceleration sensors only startMove recording when nunchuck is Activated+deactivated again
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
            yaw /= correctionFactor;
            yaw *= dt;

            roll = ws.MotionPlusState.RawValues.Y - this.zeroY;
            roll = roll / toDegFactorSlow;
            roll = ws.MotionPlusState.RollFast ? roll * fastMultiplier : roll;
            roll /= correctionFactor;
            roll *= dt; 

            pitch = ws.MotionPlusState.RawValues.Z - this.zeroZ;
            pitch = pitch / toDegFactorSlow;
            pitch = ws.MotionPlusState.YawFast ? pitch * fastMultiplier : pitch;
            pitch /= correctionFactor;
            pitch *= -dt;   //because otherwise delta angle is negative when turning upwards
        }

        /// <summary>
        /// Splits the 4 IR Points into 2 Points for the horizontal Bar and 2 for the vertical Bar.
        /// If there less than 3, it only creates the bar which is fully visible.
        /// if there are less than 2 IR Points available, it doesn't do anything.
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="irCount"></param>
        /// <param name="horizontal"></param>
        /// <param name="vertical"></param>
        private void GetIRPosition(WiimoteState ws, int irCount, out BarPoints horizontal, out BarPoints vertical)
        {
            int index = 0;
            InputPoint[] points = new InputPoint[irCount];
            horizontal = null;
            vertical = null;

            //copy
            for (int i = 0; i < 4; i++)
            {
                if (ws.IRState.IRSensors[i].Found)
                {
                    IRSensor s = ws.IRState.IRSensors[i];

                    points[index] = new InputPoint(s.RawPosition.X / (double)IR_PIXEL_WIDTH, s.RawPosition.Y/(double)IR_PIXEL_HEIGHT);
                    index++;
                }
            }

            if (irCount == 2)
            {
                if (points[0].IsHorizontal(points[1]))
                {
                    horizontal = new BarPoints();
                    horizontal.p1 = points[0];
                    horizontal.p2 = points[1];
                }
                else
                {
                    vertical = new BarPoints();
                    vertical.p1 = points[0];
                    vertical.p2 = points[1];
                }
            }
            else if (irCount > 2)
            {
                InputPoint px1;
                InputPoint px2;
                double xDistance = InputPoint.MinimumXDistance(points, out px1, out px2);

                InputPoint py1;
                InputPoint py2;
                double yDistance = InputPoint.MinimumYDistance(points, out py1, out py2);

                if (irCount == 4)
                {
                    horizontal = new BarPoints();
                    horizontal.p1 = py1;
                    horizontal.p2 = py2;
                    vertical = new BarPoints();
                    vertical.p1 = px1;
                    vertical.p2 = px2;
                }
                else
                {
                    if (xDistance < yDistance)
                    {
                        vertical = new BarPoints();
                        vertical.p1 = px1;
                        vertical.p2 = px2;
                    }
                    else
                    {
                        horizontal = new BarPoints();
                        horizontal.p1 = py1;
                        horizontal.p2 = py2;
                    }
                }
            }
        }

        /// <summary>
        /// Finds out what IR-Bar Configuration the Mote points to.
        /// 
        /// only calculate configuration, if all IR Points are available. 
        /// 
        /// If none are available, return IRBarConfiguration.NONE. 
        /// 
        /// If some are available, return the last calculated configuration
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="irCount">Number of IR sensors with a signal</param>
        /// <returns></returns>
        private IRBarConfiguration GetIRBarConfiguration(WiimoteState ws,int irCount,BarPoints vertical,BarPoints horizontal)
        {

            //nothing was found, reset last configuration
            if (irCount == 0)
                lastConfiguration = IRBarConfiguration.NONE;

            //at least one is missing, can't calculate configuration so return last known.
            if (irCount < 4)
                return lastConfiguration;


            //rotate and copy
            InputPoint[] points = new InputPoint[4];
            for (int i = 0; i < 4; i++)
            {
                IRSensor s = ws.IRState.IRSensors[i];
                points[i] = new InputPoint(s.Position.X, s.Position.Y);
            }

            /* 
             * Example Configuration of IR Bars:
             *  D1
             *  |
             *  |
             *  X   X --- D2
             *  
             * 1. Algorithm: find the diagonal points (D1,D2), they have the biggest distance between each other.
             * 2. Take one of the other points and compare its location to the diagonal.
             *     
             */
            /*
            int indexD1 = 0;
            int indexD2 = 0;
            double distance = 1000000000;
            for (int i = 0; i < 3; i++)
            {
                
                for (int j = i+1; j < 4; j++)
                {
                    double curDistance = points[i].GetDistance(points[j]);
                    if (curDistance < distance)
                    {
                        indexD1 = i;
                        indexD2 = j;
                    }
                }
            }

            InputPoint D1 = points[indexD1];
            InputPoint D2 = points[indexD2];
            int indexOther = indexD1 == 0 ? indexD2 == 1 ? 2 : 1 : 0;
            InputPoint other = points[indexOther];

            //make sure that D1 is the upper diagonal point;
            if(!D1.IsTopOf(D2)) {
                D1 = points[indexD2];
                D2 = points[indexD1];
            }*/

            //now compare the points and figure out the figure ;)
            bool isRight = vertical.p1.IsRightOf(horizontal.p1) || vertical.p2.IsRightOf(horizontal.p2);   //is one IR Bar on the top?
            bool isTop = horizontal.p1.IsTopOf(vertical.p1) || horizontal.p2.IsTopOf(vertical.p2);     //is one IR Bar on the right?

            /*if (D1.IsRightOf(D2))
            {
                if (D1.IsHorizontal(other))
                {
                    isTop = true;
                }
                else
                {
                    isRight = true;
                }
            }
            else
            {
                if (D1.IsHorizontal(other))
                {
                    isRight = true;
                    isTop = true;
                }
            }*/

            //put in result integer and cast it to enum
            int result = Convert.ToInt32(isTop) << 1;
            result += Convert.ToInt32(isRight);

            this.lastConfiguration = (IRBarConfiguration)result;
            return this.lastConfiguration;
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
            }
        }
        #endregion
    }
}
