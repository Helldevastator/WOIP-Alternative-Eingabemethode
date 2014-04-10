using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using WiimoteLib;

namespace Server
{
    public struct CursorInfo {
        public double yaw;
        public double roll;
        public double pitch;

        public double yawRaw;
        public double rollRaw;
        public double pitchRaw;

        public int xPos;
        public int yPos;
    }

    public delegate void CursorEvent(InputCursor sender, CursorInfo i);

    public class InputCursor : IDisposable
    {
        #region id
        private static int nextId = 0;
        private static Object nextLock = new Object();

        public event CursorEvent CursorUpdated;
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
        private Matrix rot;

        public InputCursor(Wiimote mote)
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

            rot = new Matrix();
        }

        private void wm_WiimoteChanged(object sender, WiimoteChangedEventArgs args)
        {
            WiimoteState ws = args.WiimoteState;
            this.eventCounter++;
            if (!this.isCalibrating)
            {
                CursorInfo i = new CursorInfo();

                double yaw, roll, pitch;
                this.CalcToDegrees(ws, out yaw,out roll,out pitch);
                i.yaw = yaw;
                i.roll = roll;
                i.pitch = pitch;

                //calc rotation matrix, x is left, y is backwards and z is up.
                Matrix dRotation = Matrix.rotation(yaw,0,1,0).multiply(Matrix.rotation(roll,0,0,1).multiply(Matrix.rotation(pitch,1,0,0)));
                
                //not sure if needed, but wiimoteChanged gets called asynchronously I think
                lock (rot)
                {
                    this.rot = dRotation.multiply(rot);
                }

                i.yawRaw = ws.MotionPlusState.RawValues.X;
                i.rollRaw = ws.MotionPlusState.RawValues.Y;
                i.pitchRaw = ws.MotionPlusState.RawValues.Z;

                if (this.CursorUpdated != null)
                    this.CursorUpdated.Invoke(this, i);
            }
            else
            {
                this.calibrateMote(ws);
            }
        }

        private void CalcToDegrees(WiimoteState ws, out double yaw, out double roll, out double pitch)
        {
            yaw = ws.MotionPlusState.RawValues.X - this.zeroX;
            yaw = ws.MotionPlusState.YawFast ? yaw * toDegFast : yaw * toDegSlow;
            yaw *= dt;

            roll = ws.MotionPlusState.RawValues.Y - this.zeroY;
            roll = ws.MotionPlusState.RollFast ? roll * toDegFast : roll * toDegSlow;
            roll *= dt;

            pitch = ws.MotionPlusState.RawValues.Z - this.zeroZ;
            pitch = ws.MotionPlusState.YawFast ? pitch * toDegFast : pitch * toDegSlow;
            pitch *= dt;
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
                        System.Console.WriteLine("");
                        this.zeroX = this.calibrationX / cCount;
                        this.zeroY = this.calibrationY / cCount;
                        this.zeroZ = this.calibrationZ / cCount;

                        this.isCalibrating = false;
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
