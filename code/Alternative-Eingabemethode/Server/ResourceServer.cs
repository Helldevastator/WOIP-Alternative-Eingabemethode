using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Common;

namespace Server
{
    class ResourceServer : IDisposable
    {
        private readonly int bufferSize;
        private readonly Thread server;
        private readonly Socket serverSocket;
        private readonly Dictionary<int, Resource> resources;
        private readonly DirectoryInfo resourceFolder;

        public ResourceServer(string resourceFolder, EndPoint serverAdress)
        {
            bufferSize = 1048576 << 3; // 8 MiBytes;
         
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            serverSocket.Bind(serverAdress);
        }

        private void ServerLoop()
        {
            serverSocket.Listen(100);
            byte[] buffer = new byte[bufferSize];

            while (true)
            {
                using (Socket client = serverSocket.Accept())
                {
                    NetworkFileIO.ReadExact(client, 4, buffer, 0);
                    int type = BitConverter.ToInt32(buffer,0);
                    switch (type)
                    {
                        
                    }
                }
                
            }
        }

        private void handleNewResource(Socket client, byte[] buffer)
        {
            NetworkFileIO.ReadExact(client, 4, buffer, 0);
            int resourceType = BitConverter.ToInt32(buffer, 0);
            try
            {
                Resource r = new Resource(resourceType);
                FileInfo file = new FileInfo(Path.Combine(this.resourceFolder.FullName, r.ResourceId.ToString()));
                NetworkFileIO.Receive(file, client, buffer);
                this.resources.Add(r.ResourceId, r);

                client.Send(BitConverter.GetBytes(r.ResourceId));
            }
            catch (SocketException e)
            {
                //programming error
                System.Console.Write(e);
                Thread current = Thread.CurrentThread;
                current.Abort();
            }
            catch (Exception e)
            {
                //what to do?
                System.Console.Write(e);
            }

            //send back new resourceId
        }

        private void handleResourceRequest(Socket client, byte[] buffer)
        {
            NetworkFileIO.ReadExact(client, 4, buffer, 0);
            int resourceId = BitConverter.ToInt32(buffer, 0);

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
