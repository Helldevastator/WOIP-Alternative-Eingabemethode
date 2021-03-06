﻿using System;
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

            System.Console.WriteLine("Starting server");
            System.Console.WriteLine("Establishing connection to clients");
            EndPoint resourceEnd = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6556);
            EndPoint updateEnd = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5665);
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            s.Connect(updateEnd);
            Client c = new Client(0,s, resourceEnd, 1920, 1080, 30, 20);
            Client[] clients = new Client[4];
            clients[0] = c;

            System.Console.WriteLine("Establishing connection to wiimote");
            List<WiimoteAdapter> controllers = new List<WiimoteAdapter>();
            controllers.Add(new WiimoteAdapter(new WiimoteLib.Wiimote()));

            System.Console.WriteLine("Starting sub components");
            ResourceServer resServer = new ResourceServer(new System.IO.DirectoryInfo(@"C:\Users\Jon\Desktop\testServer"));
            AnimationServer anmServer = AnimationServer.AnimationServerFactory(controllers, clients, resServer);
            System.Console.WriteLine("Started");

            System.Console.WriteLine("Add Window");
            int resId = resServer.AddResource(new System.IO.FileInfo(@"C:\Users\Jon\Desktop\resource.jpg"),0);
            AnimationWindow w = new AnimationWindow(c, new System.Drawing.Rectangle(1000, 500, 600, 450), resId);
            anmServer.AddWindowToClient(c, w, new System.Drawing.Point(1000, 500));

            System.Console.Read();
            System.Console.WriteLine("Server out");
            /*Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());*/
        }
    }
}
