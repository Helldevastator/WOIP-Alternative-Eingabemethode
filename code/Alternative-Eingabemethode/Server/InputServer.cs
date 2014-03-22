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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clients"></param>
        /// <param name="updateInterval">interval in milliseconds</param>
        public InputServer(List<Client> clients,int updateInterval = 100)
        {
            this.clients = clients;
            sender = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,ProtocolType.Udp);
            t = new Timer(SendWindowStates,null,0,updateInterval);
            bf = new BinaryFormatter();
        }

        /// <summary>
        /// Send cursor event to client
        /// </summary>
        /// <param name="c"></param>
        /// <param name="cursorEvent"></param>
        public void SendInputEvent(Client c, CursorEvent cursorEvent)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, cursorEvent);
                sender.SendTo(ms.GetBuffer(), c.CursorEndPoint);
            }  
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o">Ignored, Always Null</param>
        public void SendWindowStates(Object o)
        {
            foreach (Client c in clients)
            {
                byte[] send = new byte[sizeof(Window) * c.Windows.Count + sizeof(long)];
                bf.Serialize(send, windowStamp);

                foreach (Window w in c.Windows)
                    bf.Serialize(send, w);

                sender.SendTo(send, c.WindowEndPoint);
            }
                
        }

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
    }
}
