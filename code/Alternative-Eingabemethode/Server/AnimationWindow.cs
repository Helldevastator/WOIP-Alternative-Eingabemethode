using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using System.Drawing;

namespace Server
{
    public class AnimationWindow
    {
        #region idGeneration
        private static Object nextIdLock = new Object();
        private static int nextId = 0;
        #endregion

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
            Client = lastClient = c;
            lastState = currentState = new WindowState(id, resourceId);
            
        }

        public void startMove()
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
        }


        public void resetMove()
        {
            Client = lastClient;
            currentState = lastState;
            dx = 0;
            dy = 0;
            moving = false;
        }

        public void move(Point toPoint)
        {
            currentState.X = toPoint.X;
            currentState.Y = toPoint.Y;
            this.dx = toPoint.X - lastPoint.X;
            this.dy = toPoint.Y - lastPoint.Y;
            lastPoint = toPoint;
        }

        public void finishMove()
        {
            lastState = currentState;
            lastClient = Client;
        }

        public WindowState GetWindowState(double dt)
        {

            return w;
        }
    }
}
