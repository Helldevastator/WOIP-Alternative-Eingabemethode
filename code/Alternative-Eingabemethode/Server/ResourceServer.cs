using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;

namespace Server
{
    class ResourceServer : IDisposable
    {
        private readonly int bufferSize;
        private readonly Thread server;
        private readonly Socket serverSocket;
        private readonly Dictionary<int, Resource> resources;
        private readonly string resourceFolder;

        public ResourceServer(string resourceFolder)
        {
            bufferSize = 1048576 << 3; // 8 MiBytes;
            /*string IpAddressString = "192.168.1.102";
            ipEnd_server = new IPEndPoint(IPAddress.Parse(IpAddressString), 5656);
            sock_server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            sock_server.Bind(ipEnd_server);*/
        }

        private void ServerLoop()
        {
            serverSocket.Listen(100);
            byte[] buffer = new byte[bufferSize];

            while (true)
            {
                using (Socket client = serverSocket.Accept())
                {
                    int received = serverSocket.Receive(buffer,8,SocketFlags.None);
                    int type = BitConverter.Toint32(buffer,0);
                    int second = BitConverter.Toint32(buffer, 4);
                    switch (type)
                    {
                        
                    }
                }
                
            }
        }

        private void handleNewResource(Socket client, byte[] buffer, int resourceType)
        {
            
            //send back new resourceId
        }

        private void handleResourceRequest(Socket client, byte[] buffer, int resourceId)
        {
            if (this.resources.ContainsKey(resourceId))
            {
                Resource r = resources[resourceId];

            }
            else
            {
                client.Close();
            }
        }

        #region IDisposable implementation
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.server != null) server.Abort();
                if (this.serverSocket != null) serverSocket.Dispose();
            }
        }
        #endregion
    }
}
