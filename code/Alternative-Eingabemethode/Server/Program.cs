using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Server.Input;
using System.IO;
using System.Drawing;

namespace Server
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            System.Console.WriteLine("Starting server");

            string[] configuration = System.IO.File.ReadAllLines("client_conf.txt");
            Client[] clients = new Client[4];

            //once client
            System.Console.WriteLine("Establishing connection to clients");
            EndPoint resourceEnd = new IPEndPoint(IPAddress.Parse(configuration[1]), 6556);
            EndPoint updateEnd = new IPEndPoint(IPAddress.Parse(configuration[1]), 5665);
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            s.Connect(updateEnd);
            clients[0] = new Client(0, s, resourceEnd, configuration[2], configuration[3], configuration[4], configuration[5]);

            resourceEnd = new IPEndPoint(IPAddress.Parse(configuration[7]), 6556);
            updateEnd = new IPEndPoint(IPAddress.Parse(configuration[7]), 5665);
            s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            s.Connect(updateEnd);
            clients[3] = new Client(3, s, resourceEnd, configuration[8], configuration[9], configuration[10], configuration[11]);


            System.Console.WriteLine("Establishing connection to wiimote");
            List<WiimoteAdapter> controllers = new List<WiimoteAdapter>();
            controllers.Add(new WiimoteAdapter(new WiimoteLib.Wiimote()));

            System.Console.WriteLine("Starting sub components");
            ResourceServer resServer = new ResourceServer(new System.IO.DirectoryInfo(@"C:\Users\Jon\Desktop\testServer"));
            AnimationServer anmServer = AnimationServer.AnimationServerFactory(controllers, clients, resServer);
            System.Console.WriteLine("Started");


            System.Console.WriteLine("Add Windows");
            DirectoryInfo dir = new DirectoryInfo(@"C:\Users\Jon\Desktop\testdata");
            Random rand = new Random();
            foreach (FileInfo f in dir.GetFiles())
            {
                int resId = resServer.AddResource(f, 0);
                Image im = new Bitmap(f.FullName);
                int x = rand.Next(1920);
                int y = rand.Next(1080);
                AnimationWindow w = new AnimationWindow(c, new System.Drawing.Rectangle(x, y, im.Width, im.Height), resId);
                System.Console.WriteLine("another one "+w.WindowId+" name:"+f.Name);
                anmServer.AddWindowToClient(c, w, new System.Drawing.Point(x, y));
            }

            System.Console.Read();
            
            /*Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());*/
            System.Console.WriteLine("Server out");
        }


    }
}
