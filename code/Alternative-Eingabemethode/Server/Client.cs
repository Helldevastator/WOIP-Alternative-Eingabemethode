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

        public readonly long Id { public get; private set; }
        public readonly EndPoint WindowEndPoint { public get; private set; }
        public readonly EndPoint CursorEndPoint { public get; private set; }
        public readonly EndPoint ResourceEndPoint { public get; private set; } 

        /// <summary>
        /// Represents the number of tiles in a row on a client
        /// </summary>
        public readonly int TileCountX { public get; private set; }
        public readonly int TileCountY { public get; private set; }
        private readonly List<Window> Windows;
        public readonly int pixelXSize { public get; private set; }
        public readonly int pixelYSize { public get; private set; }
        //what ir-bar configuration this user has.

        public Client()
        {
            this.WindowEndPoint = new IPEndPoint(IPAddress.Parse("10.0.0.1"), 3000);
        }

        public void writeWindowData(byte[] arr,int offset)
        {
            foreach (Window w in Windows)
            {
                w.EncodeToByteArray(arr, offset);
                offset += Window.CHUNK_SIZE;
            }
        }
    }
}
