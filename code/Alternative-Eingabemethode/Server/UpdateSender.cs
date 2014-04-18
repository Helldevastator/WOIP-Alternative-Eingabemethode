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
    /// Class which is responsible for sending state updates to the client
    /// </summary>
    sealed class UpdateSender :IDisposable
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
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, client.Windows.Count);
                foreach (Window w in client.Windows)
                    bf.Serialize(ms, w);

                bf.Serialize(ms, client.Cursors.Count);
                foreach (Cursor c in client.Cursors)
                    bf.Serialize(ms, c);

                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                byte[] buffer = ms.ToArray();
                args.SetBuffer(buffer,0,buffer.Length);

                client.UpdateSocket.SendAsync(args);
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
                //if (sender != null) sender.Dispose();
            }
        }
        #endregion
    }
}
