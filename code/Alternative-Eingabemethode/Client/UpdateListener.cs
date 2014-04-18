using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;

namespace Client
{
    class UpdateListener : IDisposable
    {
        private Socket clientSocket;
        private NetworkStream toServer;
        private Display display;
        private Thread listenerThread;

        public UpdateListener(EndPoint adress)
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Bind(adress);

        }

        private void ListenerMethod()
        {
            Socket handler = clientSocket.Accept();
            handler.Listen(100);

            BinaryFormatter bf = new BinaryFormatter();
            toServer = new NetworkStream(handler);

            while (true)
            {
                if (toServer.CanRead)
                {
                   
                }
            }
        }


        #region IDisposable implementation
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disp)
        {
            if (disp)
            {
            }
        }
        #endregion
    }
}
