using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiimoteLib;

namespace Server.Input
{
    class InputPoint
    {
        public double X { get; private set; }
        public double Y { get; private set; }

        public InputPoint(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Calcluate Manhattan distance between these two points
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public double GetDistance(InputPoint p)
        {
            return Math.Abs(p.X - X) + Math.Abs(p.Y - Y);
        }

        public bool IsHorizontal(InputPoint p)
        {
            double dx = Math.Abs(p.X - X);
            double dy = Math.Abs(p.Y - Y);

            return dx > dy ? true : false; 
        }

        public bool IsRightOf(InputPoint p)
        {
            double dx = p.X - X;
            return dx < 0 ? true : false;
        }

        public bool IsTopOf(InputPoint p)
        {
            double dy = p.Y - Y;
            return dy < 0 ? true : false;
        }
    }
}
