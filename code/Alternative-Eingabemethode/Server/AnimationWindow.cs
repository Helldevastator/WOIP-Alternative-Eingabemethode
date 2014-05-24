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

        private const double friction = 0.2;
        private const double crashFriction = 0.7;

        public int ResourceId { get; private set; }
        public int WindowId { get; private set; }

        private readonly Object moveLock = new Object();
        private WindowState lastState;
        private Client lastClient;
        private readonly Object clientLock = new Object();
        private Client client;
        private WindowState currentState;

        //for fancy moving
        private Point lastPoint;
        private double x;
        private double y;

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
                currentState.Angle = 0.0F;
                currentState.Height = windowDimensions.Height;
                currentState.Width = windowDimensions.Width;
                currentState.X = windowDimensions.X;
                currentState.Y = windowDimensions.Y;
                this.x = windowDimensions.X;
                this.y = windowDimensions.Y;
                this.dx = 0;
                this.dy = 0;
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
                x = 0;
                y = 0;
                lastState = currentState;
                lastClient = Client;
                currentState = Clone(lastState);
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
                this.x = currentState.X;
                this.y = currentState.Y;
                dx = 0;
                dy = 0;
            }
        }

        public void move(Point toPoint,double dt)
        {
            lock (moveLock)
            {
                this.x = toPoint.X;
                this.y = toPoint.Y;
                this.dx = (toPoint.X - lastPoint.X)/dt;
                this.dy = (toPoint.Y - lastPoint.Y)/dt;
                lastPoint = toPoint;
            }
        }

        public void finishMove()
        {
            lock (moveLock)
            {
                lastState = currentState;
                lastClient = Client;
                currentState.MovingFlag = false;
            }
        }

        public void Animate(double dt)
        {
            lock (moveLock)
            {
                if (!currentState.MovingFlag)
                {
                    lastPoint.X = (int)this.x;
                    lastPoint.Y = (int)this.y;
                    int height2 = currentState.Height / 2;
                    int width2 = currentState.Width / 2;
                    this.x += (dx * dt);
                    this.y += (dy * dt);
                    dx -= dx * friction * dt;
                    dy -= dy * friction * dt;
                    if (this.x - width2 < 0)
                    {
                        this.x = 0 + width2;
                        dx = -dx;
                        dx -= dx * crashFriction * dt;
                    }
                    if (this.y - height2 < 0)
                    {
                        this.y = 0 + height2;
                        dy = -dy;
                        dy -= dy * crashFriction * dt;
                    }
                    if (this.x + width2 >= Client.PixelWidth)
                    {
                        this.x = Client.PixelWidth - 1 - width2;
                        dx = -dx;
                        dx -= dx * crashFriction * dt;
                    }
                    if (this.y + height2 >= Client.PixelHeight)
                    {
                        this.y = Client.PixelHeight - 1 - height2;
                        dy = -dy;
                        dy -= dy * crashFriction * dt;
                    }
                }
                System.Console.WriteLine("windowPos: " + this.dx.ToString() + " " + this.dy.ToString());

            }
        }

        public Client Client
        {
            get { lock (clientLock) { return this.client; } }
            set { lock (clientLock) { this.client = value; } }
        }

        public WindowState GetWindowState()
        {
            this.currentState.X = (int)x;
            this.currentState.Y = (int)y;
            return  Clone(currentState);
        }

        private WindowState Clone(WindowState s)
        {
            WindowState answer = new WindowState(s.WindowId, s.ResourceId);
            answer.Angle = s.Angle;
            answer.Height = s.Height;
            answer.Width = s.Width;
            answer.X = s.X;
            answer.Y = s.Y;
            answer.MovingFlag = s.MovingFlag;
            return answer;
        }
        #endregion
    }
}
