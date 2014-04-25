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
        private WindowState w;
        private Point lastPoint;
        private double dx;
        private double dy;

        public void move(Point toPoint)
        {
            lastPoint = toPoint;
        }

        public void finishMove(Point toPoint)
        {
            dx = toPoint.X - lastPoint.X;
            dy = toPoint.Y - lastPoint.Y;
        }

        public WindowState GetWindowState(double dt)
        {

            return w;
        }
    }
}
