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

        /// <summary>
        /// Gets the points with the minimum distance in the X Axis.
        /// </summary>
        /// <param name="points">Points to search for</param>
        /// <param name="p1">Minimum Point 1</param>
        /// <param name="p2">Minimum Point 2</param>
        /// <returns>Distance</returns>
        public static double MinimumXDistance(InputPoint[] points, out InputPoint p1, out InputPoint p2)
        {
            double minimum = 10000;

            for (int i = 0; i < points.Length; i++)
            {
                for (int j = i + 1; j < points.Length; j++)
                {
                    double curDistance = Math.Abs(points[i].X - points[j].X);
                    if (curDistance < minimum)
                    {
                        minimum = curDistance;
                        p1 = points[i];
                        p2 = poitns[j];
                    }
                }
            }

            return minimum;
        }

        /// <summary>
        /// Gets the points with the minimum distance in the Y Axis.
        /// </summary>
        /// <param name="points">Points to search for</param>
        /// <param name="p1">Minimum Point 1</param>
        /// <param name="p2">Minimum Point 2</param>
        /// <returns>Distance</returns>
        public static double MinimumYDistance(InputPoint[] points, out InputPoint p1, out InputPoint p2)
        {
            double minimum = 10000;

            for (int i = 0; i < points.Length; i++)
            {
                for (int j = i + 1; j < points.Length; j++)
                {
                    double curDistance = Math.Abs(points[i].Y - points[j].Y);
                    if (curDistance < minimum)
                    {
                        minimum = curDistance;
                        p1 = points[i];
                        p2 = poitns[j];
                    }
                }
            }

            return minimum;
        }
    }
}
