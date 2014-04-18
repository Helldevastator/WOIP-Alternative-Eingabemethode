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
    sealed class InputServer :IDisposable
    {
        private Socket sender;
        private List<Client> clients;
        private Timer t;
        private BinaryFormatter bf;
        private long windowStamp = 0;
        private long cursorStamp = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clients"></param>
        /// <param name="serverAdress">Adress to send from</param>
        /// <param name="updateInterval">interval in milliseconds</param>
        public InputServer(List<Client> clients,EndPoint serverAdress,int updateInterval = 100)
        {
            this.clients = clients;
            sender = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,ProtocolType.Udp);
            sender.Bind(serverAdress);

            t = new Timer(SendWindowStates,null,0,updateInterval);
            bf = new BinaryFormatter();
        }

        /// <summary>
        /// Send cursor event to client
        /// </summary>
        /// <param name="i"></param>
        /// <param name="cursorEvent"></param>
        public void SendInputEvent(Client c, CursorEvent cursorEvent)
        {
            using (MemoryStream ms = new MemoryStream())
            {
               
            }  
        }

        /// <summary>
        /// Sends the current State of a Window (Position, size...) to each client via udp.
        /// This Method is automatically called at a specified interval
        /// </summary>
        /// <param name="o">Ignored, Always Null</param>
        public void SendWindowStates(Object o)
        {
            foreach (Client c in clients)
            {
                MemoryStream ms = new MemoryStream();
                bf.Serialize(ms, windowStamp);

                foreach (Window w in c.Windows)
                    bf.Serialize(ms, w);

                sender.SendTo(ms.ToArray(), c.WindowEndPoint);
            }

            windowStamp++;   
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
                if (t != null) t.Dispose();
                if (sender != null) sender.Dispose();
            }
        }
        #endregion
    }
}
