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
            /*
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Display());*/

            
            EndPoint point = new IPEndPoint(IPAddress.Parse("127.0.0.1"),6556);
            ResourceManager manager = new ResourceManager(point, new System.IO.DirectoryInfo(@"C:\Users\Jon\Desktop\testClient"), new ResourceHandlerFactory());

            System.Threading.Thread.Sleep(15 * 1000);

        }
    }
}
