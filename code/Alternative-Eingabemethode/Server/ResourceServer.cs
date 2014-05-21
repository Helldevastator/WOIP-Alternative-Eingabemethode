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
    public class ResourceServer
    {
        private static Object nextIdLock = new Object();
        private static int nextId = 0;

        private delegate void SendResourceDelegate(Client client, int resourceId);

        private readonly Object resourcesLock = new Object();
        private readonly Dictionary<int, Resource> resources;
        private readonly DirectoryInfo resourceFolder;
        private readonly SendResourceDelegate sender;

        public ResourceServer(DirectoryInfo resourceFolder)
        {
            resources = new Dictionary<int, Resource>();
            sender = new SendResourceDelegate(SendAsync);
            
            this.resourceFolder = resourceFolder;
        }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="f"></param>
       /// <param name="typeId"></param>
        /// <returns>resource resId</returns>
        public int AddResource(FileInfo f,int typeId) {
            int id = 0;
            lock (nextIdLock)
            {
                id = nextId;
                nextId++;
            }

            f.CopyTo(Path.Combine(resourceFolder.FullName, resource.ResourceId.ToString()));
            lock(resourcesLock)
                resources.Add(new Resource(id,typeId), resource);

            return id;
        }
        

        private void SendAsync(Client client, int resourceId) 
        {
            System.Console.WriteLine("sending");
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
                NetworkIO.SendObject(toClient, r);
                NetworkIO.SendFile(toClient, f);
            }
        }

        
        /// <summary>
        /// Send a resource to the client, if resourceId does not exist don't do anything
        /// </summary>
        /// <param name="client"></param>
        /// <param name="resourceId"></param>
        public void SendResource(Client client, int resourceId)
        {
            System.Console.WriteLine("start sending");
            this.sender.BeginInvoke(client, resourceId, null, null);
        }

    }
}
