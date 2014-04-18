using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Common;

namespace Server
{
    public class Client
    {
        //possible performance increase: save writeWindowData() byte array and update modified

        public long Id { get; private set; }
        public EndPoint WindowEndPoint { get; private set; }
        public EndPoint ResourceEndPoint { get; private set; } 

        /// <summary>
        /// Represents the number of tiles in a row on a client
        /// </summary>
        public int TileCountX { get; private set; }
        public int TileCountY { get; private set; }

        //threadsave?
        public List<Window> Windows  { get; private set; }
        public int pixelXSize { get; private set; }
        public int pixelYSize { get; private set; }
        //what ir-bar configuration this user has.

        public Client()
        {
            //this.WindowEndPoint = new IPEndPoint(IPAddress.Parse("10.0.0.1"), 3000);
        }

    }
}
