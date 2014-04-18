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
        public delegate void UpdateClientListener(ClientState state);

        public event UpdateClientListener UpdateEvent;
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
                    ClientState state = (ClientState)bf.Deserialize(toServer);
                    if (UpdateEvent != null)
                        UpdateEvent.BeginInvoke(state, null, null);
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
