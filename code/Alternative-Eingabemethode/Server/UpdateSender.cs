using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Common;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Server
{
    /// <summary>
    /// Class which is responsible for sending MoteState updates to the client
    /// </summary>
    sealed class UpdateSender
    {
        private BinaryFormatter bf;

        public UpdateSender(EndPoint serverAdress,List<Client> clients)
        {
            /*sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream,ProtocolType.Tcp);
            sender.Bind(serverAdress);*/

            bf = new BinaryFormatter();
        }

        /// <summary>
        /// Asynchronously send an update event to the client
        /// </summary>
        public void UpdateClient(Client client)
        {
            NetworkStream ns = client.UpdateStream;
            bf.Serialize(ns, client.GetClientState()); 
        }

    }
}
