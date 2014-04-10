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
        private float zeroX;
        private float zeroY;
        private float zeroZ;

        private double calibrationX = 8063d;
        private double calibrationY = 8063d;
        private double calibrationZ = 8063d;
        private int cCount = -1;
        private bool isCalibrating = false;
        private static int calibrationCount = 100;
        #endregion

        #region static constants
        private const double toDegSlow = 1/(8192 / 595);
        private const double toDegFast = toDegSlow * 2000 / 440;
        #endregion

        private Wiimote mote;

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
        }

        private void wm_WiimoteChanged(object sender, WiimoteChangedEventArgs args)
        {
            WiimoteState ws = args.WiimoteState;
            if (!this.isCalibrating)
            {
                CursorInfo i = new CursorInfo();

                double yaw, roll, pitch;
                this.CalcToDegreesPerSec(ws, out yaw,out roll,out pitch);

                i.yaw = yaw;
                i.roll = roll;
                i.pitch = pitch;

                if (this.CursorUpdated != null)
                    this.CursorUpdated.Invoke(this, i);
            }
            else
            {
                this.calibrateMote(ws);
            }

        }

        private void CalcToDegreesPerSec(WiimoteState ws, out double yaw, out double roll, out double pitch)
        {
            yaw = ws.MotionPlusState.RawValues.X - this.calibrationX;
            yaw = ws.MotionPlusState.YawFast ? yaw * toDegFast : yaw * toDegSlow;
            roll = ws.MotionPlusState.RawValues.X - this.calibrationY;
            roll = ws.MotionPlusState.RollFast ? roll * toDegFast : roll * toDegSlow;
            pitch = ws.MotionPlusState.RawValues.Z - this.calibrationZ;
            pitch = ws.MotionPlusState.YawFast ? pitch * toDegFast : pitch * toDegSlow;
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
            
            this.calibrationX = ws.MotionPlusState.RawValues.X;
            this.calibrationY = ws.MotionPlusState.RawValues.Y;
            this.calibrationZ = ws.MotionPlusState.RawValues.Z;
            this.cCount++;

            if (this.cCount >= calibrationCount)
            {
                this.calibrationX /= cCount;
                this.calibrationY /= cCount;
                this.calibrationZ /= cCount;

                this.isCalibrating = false;
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
