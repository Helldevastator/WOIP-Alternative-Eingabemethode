using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            Matrix rotA = Matrix.rotation(0,1, 0, 0); //yaw
	        Matrix rotB = Matrix.rotation(45, 0, 0, 1); //pitch
            Matrix rotC = Matrix.rotation(90, 0, 1, 0); //roll
            Matrix rot = rotA.multiply(rotB).multiply(rotC);
            
            System.Console.WriteLine(rot.CalculatePitch() * 180d / Math.PI); //pitch
            System.Console.WriteLine(rot.CalculateRoll() * 180d / Math.PI); //roll
            System.Console.WriteLine(rot.CalculateYaw() * 180d / Math.PI); //yaw
            System.Console.WriteLine(); 

            Matrix rotD = Matrix.rotation(90, 0, 1, 0); //roll
            rot = rot.multiply(rotD);

            System.Console.WriteLine(rot.CalculatePitch() * 180d / Math.PI); //pitch
            System.Console.WriteLine(rot.CalculateRoll() * 180d / Math.PI); //roll
            System.Console.WriteLine(rot.CalculateYaw() * 180d / Math.PI); //yaw


           /* Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());*/

            EndPoint point = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6556);
            ResourceServer server = new ResourceServer(new System.IO.DirectoryInfo(@"C:\Users\Jon\Desktop\testServer"));
            Client c = new Client(point);
            server.AddResource(new Common.Resource(1, 0), new System.IO.FileInfo("bla.txt"));
            server.SendResource(c, 1);

            System.Threading.Thread.Sleep(15 * 1000);
            System.Console.WriteLine("main out");
        }
    }
}
