using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    /// <summary>
    /// represents a rotation matrix
    /// </summary>
    class Matrix
    {
        private double[,] m;

        public Matrix()
        {
            this.m = new double[4, 4];
            m[0, 0] = 1;
            m[1, 1] = 1;
            m[2, 2] = 1;
            m[3, 3] = 1;
        }

        public Matrix(double[,] m)
        {
            this.m = m;
        }

        public void multiply(Matrix rot)
        {
            double[,] tmp = new double[4,4];

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    for (int k = 0; k < 4; k++)
                        tmp[i, j] = m[i, k] * rot.m[k, j];
                }
            }

            this.m = tmp;
        }
        public static Matrix rotation(double phi, double a1, double a2, double a3)
        {
            double c = Math.cos(Math.toRadians(phi));
            double s = Math.sin(Math.toRadians(phi));
            double[,] rot = { { (1 - c) * a1 * a1 + c, (1 - c) * a1 * a2 - s * a3, (1 - c) * a1 * a3 + s * a2, 0 },
				{ (1 - c) * a2 * a1 + s * a3, (1 - c) * a2 * a2 + c, (1 - c) * a2 * a3 - s * a1, 0 },
				{ (1 - c) * a3 * a1 - s * a2, (1 - c) * a3 * a2 + s * a1, (1 - c) * a3 * a3 + c, 0 }, { 0, 0, 0, 1 } };
            return new Matrix(rot);
        }

        public void setX(double phi)
        {

        }

        public void setY(double phi)
        {
        }

        public void setZ(double phi)
        {
        }
    }
}
