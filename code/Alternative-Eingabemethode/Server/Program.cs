using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Server.Input;

namespace Server
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Matrix rotA = Matrix.rotation(0,1, 0, 0); //yaw
	        Matrix rotB = Matrix.rotation(45, 0, 0, 1); //pitch
            Matrix rotC = Matrix.rotation(90, 0, 1, 0); //rollInterpolated
            Matrix rot = rotA.multiply(rotB).multiply(rotC);
            
            System.Console.WriteLine(rot.CalculatePitch() * 180d / Math.PI); //pitch
            System.Console.WriteLine(rot.CalculateRoll() * 180d / Math.PI); //rollInterpolated
            System.Console.WriteLine(rot.CalculateYaw() * 180d / Math.PI); //yaw
            System.Console.WriteLine(); 

            Matrix rotD = Matrix.rotation(90, 0, 1, 0); //rollInterpolated
            rot = rot.multiply(rotD);

            System.Console.WriteLine(rot.CalculatePitch() * 180d / Math.PI); //pitch
            System.Console.WriteLine(rot.CalculateRoll() * 180d / Math.PI); //rollInterpolated
            System.Console.WriteLine(rot.CalculateYaw() * 180d / Math.PI); //yaw

            EndPoint resourceEnd = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6556);
            EndPoint updateEnd = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5665);
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            s.Connect(updateEnd);
            Client c = new Client(0,s, resourceEnd, 1920, 1080, 30, 20);
            Client[] clients = new Client[4];
            clients[0] = c;
            
            List<MoteController> controllers = new List<MoteController>();
            controllers.Add(new MoteController(new WiimoteLib.Wiimote()));

            ResourceServer resServer = new ResourceServer(new System.IO.DirectoryInfo(@"C:\Users\Jon\Desktop\testServer"));
            AnimationServer anmServer = AnimationServer.AnimationServerFactory(controllers, clients, resServer);
     
            /*Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());*/
        }
    }
}
