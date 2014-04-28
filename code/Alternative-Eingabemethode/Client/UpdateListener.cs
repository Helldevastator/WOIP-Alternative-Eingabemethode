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
    /// <summary>
    /// This class listens for Update messages over the TCP/IP network stack.
    /// </summary>
    sealed class UpdateListener : IDisposable
    {
        public delegate void UpdateClientListener(ClientState state);

        public event UpdateClientListener UpdateEvent;
        private Socket listenerSocket;
        private NetworkStream toServer;
        private Display display;
        private Thread listenerThread;

        public UpdateListener(EndPoint adress)
        {
            listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenerSocket.Bind(adress);
            listenerThread = new Thread(new ThreadStart(ListenerMethod));
            listenerThread.IsBackground = true;
            listenerThread.Start();
        }

        /// <summary>
        /// Start Thread for 
        /// </summary>
        private void ListenerMethod()
        {
            listenerSocket.Listen(1);
            Socket handler = listenerSocket.Accept();


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
                if (this.toServer != null) toServer.Dispose();
                if (this.listenerSocket != null) listenerSocket.Dispose();
            }
        }
        #endregion
    }
}
