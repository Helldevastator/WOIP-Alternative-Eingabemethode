using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Common;
using System.Net.Sockets;

namespace Server
{
    public class Client
    {
        //possible performance increase: save writeWindowData() byte array and update modified

        public long Id { get; private set; }
        public NetworkStream UpdateStream { get; private set; }
        public EndPoint ResourceEndPoint { get; private set; } 

        //threadsave?
        public int PixelWidth { get; private set; }
        public int PixelHeight { get; private set; }
        public Dictionary<int, CursorState> Cursors { get; private set; }
        public List<AnimationWindow> Windows { get; private set; }

        private ClientState state;

        public Client(EndPoint resourceEndPoint)
        {
            this.ResourceEndPoint = resourceEndPoint;
            //this.WindowEndPoint = new IPEndPoint(IPAddress.Parse("10.0.0.1"), 3000);
        }
        
        /// <summary>
        /// Returns the client InputState for serialization
        /// </summary>
        /// <returns></returns>
        public ClientState GetClientState() 
        {
           /* InputState.Cursors = new List<CursorState>((IEnumerable<CursorState>)Cursors);
            InputState.Windows = new List<WindowState>(Windows.Count);
            for (int i = 0; i < InputState.Windows.Count; i++)
                InputState.Windows[i] = Windows[i].GetWindowState();*/

            return null;
        }
    }
}
