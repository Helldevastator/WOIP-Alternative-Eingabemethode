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
        public float XAbs;
        public float YAbs;
        public float ZAbs;

        public float XRel;
        public float YRel;
        public float ZRel;

        public int xPos;
        public int yPos;
    }

    public delegate void CursorEvent(InputCursor sender, CursorInfo i);

    public class InputCursor : IDisposable
    {
        private static float xThreshold = 0.05F;
        private static float yThreshold = 0.05F;
        private static float zThreshold = 0.05F;

        private static int nextId = 0;
        private static Object nextLock = new Object();

        public event CursorEvent CursorUpdated;
        public int Id { get; private set; }

        private float lastX = float.MaxValue;
        private float lastY = 0;
        private float lastZ = 0;
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

        }


        private void wm_WiimoteChanged(object sender, WiimoteChangedEventArgs args)
        {
            WiimoteState ws = args.WiimoteState;
            if (this.lastX != float.MaxValue)
            {
                CursorInfo i = new CursorInfo();

                i.XAbs = ws.AccelState.Values.X;
                i.YAbs = ws.AccelState.Values.Y;
                i.ZAbs = ws.AccelState.Values.Z;

                i.XRel = i.XAbs - lastX;
                i.YRel = i.YAbs - lastY;
                i.ZRel = i.ZAbs - lastZ;

                
                if (!IsNoise(i.XRel, i.YRel, i.ZRel))
                {
                    i.xPos = (int)(i.XRel * 50);
                    i.yPos = (int)(i.ZRel * 50);
                }

                if (this.CursorUpdated != null)
                    this.CursorUpdated.Invoke(this, i);

                this.lastX = ws.AccelState.Values.X;
                this.lastY = ws.AccelState.Values.Y;
                this.lastZ = ws.AccelState.Values.Z;
            }
            else
            {
                //init this object, 
                this.lastX = ws.AccelState.Values.X;
                this.lastY = ws.AccelState.Values.Y;
                this.lastZ = ws.AccelState.Values.Z;
            }
            

            
        }

        private bool IsNoise(float relX, float relY, float relZ)
        {
            bool answer = true;
            answer &= Math.Abs(relX) < xThreshold;
            answer &= Math.Abs(relY) < yThreshold;
            answer &= Math.Abs(relZ) < zThreshold;

            return answer;
        }

        private void wm_WiimoteExtensionChanged(object sender, WiimoteExtensionChangedEventArgs args)
        {
            if (args.Inserted)
                mote.SetReportType(InputReport.IRExtensionAccel, true);
            else
                mote.SetReportType(InputReport.IRAccel, true);
        }

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
