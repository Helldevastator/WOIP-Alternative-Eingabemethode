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
        double x;
        double y;

        public InputPoint(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Calcluate the distance between these two points
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public double GetDistance(InputPoint p)
        {
            double dx = p.x - x;
            double dy = p.y - y;

            return Math.Sqrt(dx*dx+dy*dy);
        }

        public bool IsHorizontal(InputPoint p)
        {
            double dx = Math.Abs(p.x - x);
            double dy = Math.Abs(p.y - y);

            return dx < dy ? true : false; 
        }

        public bool IsRightOf(InputPoint p)
        {
            double dx = p.x - x;
            return dx < 0 ? true : false;
        }

        public bool IsTopOf(InputPoint p)
        {
            double dy = p.y - y;
            return dy > 0 ? true : false;
        }
    }
}
