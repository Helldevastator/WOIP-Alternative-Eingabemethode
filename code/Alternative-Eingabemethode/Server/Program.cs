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
            double toDegFactorSlow = 8192d / 595d;
            double fastMultiplier =  2000d / 440d;
            double raw = 8083;
            double zero = 8063;
            bool fast = true;
            double dt = 1 / 200d;

            double yaw = raw - zero;
            yaw = yaw / toDegFactorSlow;
            yaw = fast ? yaw * fastMultiplier : yaw;
            //yaw *= dt;

            System.Console.WriteLine(yaw);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

        }
    }
}
