using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Client.ResourceHandler;

namespace Client
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string localIP = args.Length > 0 ? args[0]: LocalIPAddress();
            localIP = "127.0.0.1";
            System.Console.WriteLine("Starting Client, listening at:"+localIP);
            EndPoint resourceListenerPoint = new IPEndPoint(IPAddress.Parse(localIP),6556);
            EndPoint updatePoint = new IPEndPoint(IPAddress.Parse(localIP), 5665);
            
            ResourceManager manager = new ResourceManager(resourceListenerPoint, new System.IO.DirectoryInfo(@"C:\Users\Jon\Desktop\testClient"), new ResourceHandlerFactory(), new WaitResource());
            UpdateListener upListener = new UpdateListener(updatePoint);
            Display display = new Display("background.png");

            DisplayController controller = new DisplayController(display, manager);
            upListener.UpdateEvent += controller.UpdateClient;
          
            Application.Run(display);
        }

        private static string LocalIPAddress()
        {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }
    }
}
