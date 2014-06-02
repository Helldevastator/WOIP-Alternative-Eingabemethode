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
        private static readonly Object nextIdLock = new Object();
        private static int nextId = 0;
        #endregion

        private const double friction = 0.2;
        private const double crashFriction = 0.2;

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
            lock (nextIdLock)
            {
                id = nextId;
                System.Console.WriteLine("bla: " + nextId.ToString());
                nextId++;
            }

            lock (moveLock)
            {
                
                Client = lastClient = c;
                lastState = currentState = new WindowState(id, resourceId);
                this.ResourceId = resourceId;
                this.WindowId = id;
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
            bool answer = maxX >= p.X && minX <= p.X && maxY >= p.Y && minY <= p.Y;
            
            return answer;
        }

        #region moving window
        public void startMove()
        {
            lock (moveLock)
            {
                x = lastState.X;
                y = lastState.Y;
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

        //scale by factor. The factor is always multiplied by size before scaling has begun
        public void scale(double factor)
        {
            lock (moveLock)
            {
                int width = (int)(factor * this.lastState.Width);
                int height = (int)(factor * this.lastState.Height);
                if (width >= 300 && width <= client.PixelWidth && height >= 200 && height <= client.PixelHeight)
                {

                    this.currentState.Width = width;
                    System.Console.WriteLine(this.currentState.Width.ToString());
                    this.currentState.Height = height;
                }
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
                        dx -= dx * crashFriction;
                        dy -= dy * crashFriction;
                    }
                    if (this.y - height2 < 0)
                    {
                        this.y = 0 + height2;
                        dy = -dy;
                        dx -= dx * crashFriction;
                        dy -= dy * crashFriction;
                    }
                    if (this.x + width2 >= Client.PixelWidth)
                    {
                        this.x = Client.PixelWidth - 1 - width2;
                        dx = -dx;
                        dx -= dx * crashFriction;
                        dy -= dy * crashFriction;
                    }
                    if (this.y + height2 >= Client.PixelHeight)
                    {
                        this.y = Client.PixelHeight - 1 - height2;
                        dy = -dy;
                        dx -= dx * crashFriction;
                        dy -= dy * crashFriction;
                    }

                }
               

            }
        }

        public Client Client
        {
            get { lock (clientLock) { return this.client; } }
            set { lock (clientLock) { this.client = value; } }
        }

        public WindowState GetWindowState()
        {
            this.currentState.X = Math.Max((int)x,0);
            this.currentState.Y = Math.Max((int)y,0);
            if (client != null)
            {
                this.currentState.X = Math.Min((int)x, client.PixelWidth);
                this.currentState.Y = Math.Min((int)y, client.PixelHeight);
            }
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
