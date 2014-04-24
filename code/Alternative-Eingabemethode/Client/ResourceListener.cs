using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Client.ResourceHandler;
using Common;

namespace Client
{
    sealed class ResourceListener : IDisposable
    {
        public delegate void ResourceLoadedListener(IResourceHandler handler);

        private Thread listenerThread;
        private Socket listenerSocket;

        public ResourceListener(EndPoint adress)
        {
            listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenerSocket.Bind(adress);
            listenerThread = new Thread(new ThreadStart(ListenerMethod));
            listenerThread.IsBackground = true;
            listenerThread.Start(); 
        }

        private void ListenerMethod()
        {
            listenerSocket.Listen(100);

            while (true)
            {
                Socket handler = listenerSocket.Accept();
                
                //
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
                if (this.listenerSocket != null) listenerSocket.Dispose();
            }
        }
        #endregion
    }
}
