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

        private readonly Thread listenerThread;
        private readonly Socket listenerSocket;
        private readonly DirectoryInfo resourceFolder;
        private readonly IResourceHandlerFactory factory;
        private readonly Object resourcesLock = new Object();
        private readonly Dictionary<int, IResourceHandler> resources;
        private readonly Object waitSetLock = new Object();
        private readonly Dictionary<int, List<ResourceLoadedCallback>> waitSet;
       

        public ResourceManager(EndPoint adress, DirectoryInfo resourceFolder,IResourceHandlerFactory factory)
        {
            this.factory = factory;
            this.resourceFolder = resourceFolder;
            listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenerSocket.Bind(adress);
            listenerThread = new Thread(new ThreadStart(ListenerMethod));
            listenerThread.IsBackground = true;
            listenerThread.Start();
        }

        /// <summary>
        /// Calls the ResourceLoadedCallback if this resource is known, else put it in a waiting queue until the Manager has received the resource.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="resourceId">resource defined by this resourceTypeId</param>
        public void SetOrUpdateResource(ResourceLoadedCallback callback,int resourceId)
        {
            lock (resourcesLock)
            {
                lock (waitSetLock)
                {
                    if (resources.Keys.Contains(resourceId))
                    {
                        callback.Invoke(resources[resourceId]);
                    }
                    else
                    {
                        if (waitSet.Keys.Contains(resourceId))
                        {
                            waitSet[resourceId].Add(callback);
                        }
                        else
                        {
                            var list = new List<ResourceLoadedCallback>(3); //heuristic, don't expect more than three windows waiting on the same resource
                            list.Add(callback);
                            waitSet.Add(resourceId,list);
                        }
                    }
                }
            }
        }

        #region listener implementation
        private void ListenerMethod()
        {
            listenerSocket.Listen(100);

            while (true)
            {
                Socket handler = listenerSocket.Accept();

                //read
                FileInfo file;
                int resourceId;
                int typeId;

                IResourceHandler res = this.factory.CreateResourceHandler(typeId, file);

                lock (resourcesLock)
                    resources.Add(resourceId, res);

                this.UpdateWaitSet(resourceId);
            }
        }

        /// <summary>
        /// Calls objects waiting for this resource
        /// </summary>
        /// <param name="newId"></param>
        private void UpdateWaitSet(int newId)
        {
            lock (resourcesLock)
            {
                lock (waitSetLock)
                {
                    if (waitSet.Keys.Contains(newId))
                    {
                        var list = this.waitSet[newId];
                        var resource = this.resources[newId];
                        foreach (ResourceLoadedCallback c in list)
                            c.Invoke(resource);
                    }
                }
            }
        }
        #endregion

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
