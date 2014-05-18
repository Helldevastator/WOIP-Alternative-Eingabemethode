using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using System.Drawing;

namespace Server
{
    /// <summary>
    /// Needs to  be threadsafe
    /// </summary>
    public class AnimationWindow
    {
        #region idGeneration
        private static Object nextIdLock = new Object();
        private static int nextId = 0;
        #endregion

        public int ResourceId { public get; private set; }
        public int WindowId { public get; private set; }

        private readonly Object moveLock = new Object();
        private bool moving = false;
        private WindowState lastState;
        private Client lastClient;
        public Client Client { public get; private set; }
        private WindowState currentState;

        //for fancy moving
        private Point lastPoint;
        private double dx;
        private double dy;

        public AnimationWindow(Client c, Rectangle windowDimensions,int resourceId)
        {
            int id = 0;
            lock(nextIdLock) {
                id = nextId;
                nextId++;
            }

            lock (moveLock)
            {
                Client = lastClient = c;
                lastState = currentState = new WindowState(id, resourceId);
                this.ResourceId = resourceId;
                this.WindowId = WindowId;
            }
            
        }

        public bool ContainsPoint(Point p)
        {

            return false;
        }

        #region moving window
        public void startMove()
        {
            lock (moveLock)
            {
                lastState = currentState;
                lastClient = Client;
                moving = true;
                currentState = new WindowState(lastState.WindowId, lastState.ResourceId);
                currentState.Angle = lastState.Angle;
                currentState.Height = lastState.Height;
                currentState.Width = lastState.With;
                currentState.X = lastState.X;
                currentState.Y = lastState.Y;
                currentState.MovingFlag = true;
            }
        }

        public void resetSlide()
        {
            lock (moveLock)
            {
                this.dx = 0;
                this.dy = 0;
            }
        }
        public void resetMove()
        {
            lock (moveLock)
            {
                Client = lastClient;
                currentState = lastState;
                dx = 0;
                dy = 0;
                moving = false;
            }
        }

        public void move(Point toPoint)
        {
            lock (moveLock)
            {
                currentState.X = toPoint.X;
                currentState.Y = toPoint.Y;
                this.dx = toPoint.X - lastPoint.X;
                this.dy = toPoint.Y - lastPoint.Y;
                lastPoint = toPoint;
            }
        }

        public void finishMove()
        {
            lock (moveLock)
            {
                this.moving = false;
                lastState = currentState;
                lastClient = Client;
            }
        }

        public WindowState GetWindowState(double dt)
        {

            return w;
        }
        #endregion
    }
}
