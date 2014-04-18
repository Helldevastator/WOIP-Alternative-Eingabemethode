using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Common;
using System.Net.Sockets;

namespace Server
{
    public class Client
    {
        //possible performance increase: save writeWindowData() byte array and update modified

        public long Id { get; private set; }
        public NetworkStream UpdateStream { get; private set; }
        public EndPoint ResourceEndPoint { get; private set; } 

        //threadsave?
        public ClientState State { get; private set; }
        public int PixelWidth { get; private set; }
        public int PixelHeight { get; private set; }
        //what ir-bar configuration this user has.

        public Client()
        {
            //this.WindowEndPoint = new IPEndPoint(IPAddress.Parse("10.0.0.1"), 3000);
        }

    }
}
