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
        }

        public double GetDistance(InputPoint p)
        {
            return 0;
        }

        public bool IsHorizontal(InputPoint p)
        {
        }

        public bool IsRightOf(InputPoint p)
        {
        }

        public bool IsTopOf(InputPoint p)
        {
        }
    }
}
