using System;
using System.Collections.Generic;
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
            
            

            System.Console.WriteLine("bla");
            EndPoint resourceListenerPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"),6556);
            EndPoint updatePoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5665);

            ResourceManager manager = new ResourceManager(resourceListenerPoint, new System.IO.DirectoryInfo(@"C:\Users\Jon\Desktop\testClient"), new ResourceHandlerFactory());
            UpdateListener upListener = new UpdateListener(updatePoint);
            Display display = new Display();

            DisplayController controller = new DisplayController(display, manager);
            upListener.UpdateEvent += controller.UpdateClient;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(display);
        }
    }
}
