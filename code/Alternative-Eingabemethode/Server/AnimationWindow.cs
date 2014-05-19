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

        public int ResourceId { get; private set; }
        public int WindowId { get; private set; }

        private readonly Object moveLock = new Object();
        private WindowState lastState;
        private Client lastClient;
        public Client Client { get; private set; }
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
            int maxX = 0;
            int minX = 0;
            int maxY = 0;
            int minY = 0;
            lock (moveLock)
            {
                maxX = currentState.X + currentState.Width / 2;
                minX = currentState.X - currentState.Width / 2;
                maxY = currentState.Y + currentState.Height / 2;
                minY = currentState.Y - currentState.Height / 2;
            }
            bool answer = maxX >= p.X && minY <= p.X && maxY >= p.Y && minY <= p.Y;
            return answer;
        }

        #region moving window
        public void startMove()
        {
            lock (moveLock)
            {
                lastState = currentState;
                lastClient = Client;
                currentState = new WindowState(lastState.WindowId, lastState.ResourceId);
                currentState.Angle = lastState.Angle;
                currentState.Height = lastState.Height;
                currentState.Width = lastState.Width;
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
                lastState = currentState;
                lastClient = Client;
            }
        }

        public void Animate(double dt)
        {
            lastPoint.X = currentState.X;
            lastPoint.Y = currentState.Y;
            
            currentState.X = (int) (dx * dt);
            currentState.Y = (int) (dy * dt);
            dx *= Client.XFrictionFactor * dt;
            dy *= Client.YFrictionFactor * dt;
            if (currentState.X < 0)
            {
                currentState.X = 0;
                dx = -dx;
            }
            if (currentState.Y < 0)
            {
                currentState.Y = 0;
                dy = -dy;
            }
            if (currentState.X >= Client.PixelWidth)
            {
                currentState.X = Client.PixelWidth - 1;
                dx = -dx;
            }
            if (currentState.Y >= Client.PixelHeight)
            {
                currentState.Y = Client.PixelHeight - 1; ;
                dy = -dy;
            }
        }

        public WindowState GetWindowState()
        {
            return currentState;
        }
        #endregion
    }
}
