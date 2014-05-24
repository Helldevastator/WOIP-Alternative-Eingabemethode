using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
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
            
            System.Console.WriteLine("Starting Client");
            EndPoint resourceListenerPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"),6556);
            EndPoint updatePoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5665);

            
            ResourceManager manager = new ResourceManager(resourceListenerPoint, new System.IO.DirectoryInfo(@"C:\Users\Jon\Desktop\testClient"), new ResourceHandlerFactory(), new WaitResource());
            UpdateListener upListener = new UpdateListener(updatePoint);
            Display display = new Display("background.png");

            DisplayController controller = new DisplayController(display, manager);
            upListener.UpdateEvent += controller.UpdateClient;
          
            Application.Run(display);
        }
    }
}
