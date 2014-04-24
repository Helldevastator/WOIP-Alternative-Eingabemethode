using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Matrix rotA = Matrix.rotation(45,1, 0, 0); //yaw
            Matrix rotB = Matrix.rotation(45, 0, 0, 1); //pitch
            Matrix rotC = Matrix.rotation(10, 0, 1, 0); //roll
            Matrix rot = rotA.multiply(rotB).multiply(rotC);

            System.Console.WriteLine(rot.getAlpha() * 180d / Math.PI); //roll
            System.Console.WriteLine(rot.getBeta() * 180d / Math.PI); //pitch
            System.Console.WriteLine(rot.getGamma() * 180d / Math.PI); //yaw

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

        }
    }
}
