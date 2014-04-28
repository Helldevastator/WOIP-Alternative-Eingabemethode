using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Client.ResourceHandler;
using Common;

namespace Client
{
    /// <summary>
    /// 
    /// </summary>
    sealed class ResourceManager : IDisposable
    {
        public delegate void ResourceLoadedCallback(IResourceHandler handler);

        private Thread listenerThread;
        private Socket listenerSocket;
        private DirectoryInfo resourceFolder;

        public ResourceManager(EndPoint adress, DirectoryInfo resourceFolder)
        {
            this.resourceFolder = resourceFolder;
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

        public void SetOrUpdateResource(ResourceLoadedCallback callback,int resourceId)
        {
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
