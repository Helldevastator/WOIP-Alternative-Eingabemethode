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
    /// <summary>
    /// Represents a server responsible for sending resources to the client asynchronously
    /// </summary>
    class ResourceServer
    {
        private delegate void SendResourceDelegate(Client client, int resourceId);

        private const int bufferLength = 1048576 << 3; // 8 MiBytes;

        private readonly BinaryFormatter bf = new BinaryFormatter();
        private readonly Object resourcesLock = new Object();
        private readonly Dictionary<int, Resource> resources;
        private readonly DirectoryInfo resourceFolder;
        private readonly SendResourceDelegate sender;

        public ResourceServer(DirectoryInfo resourceFolder, EndPoint serverAdress)
        {
            resources = new Dictionary<int, Resource>();
            sender = new SendResourceDelegate(SendAsync);
            
            this.resourceFolder = resourceFolder;
        }

        public void AddResource(Resource resource,FileInfo f) {
            f.CopyTo(Path.Combine(resourceFolder.FullName, resource.ResourceId.ToString()));
            lock(resourcesLock)
                resources.Add(resource.ResourceId, resource);
        }
        

        private void SendAsync(Client client, int resourceId) 
        {
            Resource r;

            lock (resourcesLock)
            {
                //check if there is a resource, if not don't do anything
                if (!this.resources.ContainsKey(resourceId))
                    return;
                
                r = this.resources[resourceId];
            }

            using (Socket toClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                FileInfo f = new FileInfo(Path.Combine(resourceFolder.FullName, resourceId.ToString()));
                toClient.Connect(client.ResourceEndPoint);
                byte[] buffer = new byte[bufferLength];
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

        
        /// <summary>
        /// Send a resource to the client, if resourceId does not exist don't do anything
        /// </summary>
        /// <param name="client"></param>
        /// <param name="resourceId"></param>
        public void SendResource(Client client, int resourceId)
        {
            this.sender.BeginInvoke(client, resourceId, null, null);
        }

    }
}
