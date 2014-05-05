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
using System.Runtime.Serialization.Formatters.Binary;

namespace Server
{
    class ResourceServer
    {
        private delegate void SendResourceDelegate(Client client, int resourceId);

        private static const int bufferLength = 1048576 << 3; // 8 MiBytes;

        private readonly BinaryFormatter bf = new BinaryFormatter();
        private readonly Dictionary<int, Resource> resources;
        private readonly DirectoryInfo resourceFolder;
        private readonly SendResourceDelegate sender;

        public ResourceServer(string resourceFolder, EndPoint serverAdress)
        {
            sender = new SendResourceDelegate(SendAsync);
        }

        private void SendAsync(Client client, int resourceId) 
        {
            Resource r = this.resources[resourceId];

            using (Socket toClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                FileInfo f = new FileInfo(Path.Combine(resourceFolder.FullName, resourceId.ToString()));
                toClient.Connect(client.ResourceEndPoint);
                byte[] buffer = new byte[this.bufferLength];
                using (MemoryStream ms = new MemoryStream(buffer)) 
                {
                    bf.Serialize(ms,r.ResourceId);
                    bf.Serialize(ms, r.ResourceType);
                    bf.Serialize(ms, f.Length);

                    toClient.Send(ms.ToArray(),sizeof(int) * 2,SocketFlags.None);
                }

                
                NetworkFileIO.SendFile(toClient, f, buffer);
            }
        }

        

        public void SendResource(Client client, int resourceId)
        {
            this.sender.BeginInvoke(client, resourceId, null, null);
        }

    }
}
