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
    /// <summary>
    /// Needs to be threadsafe
    /// </summary>
    public class Client
    {
        //possible performance increase: save writeWindowData() byte array and update modified

        public long Id { get; private set; }
        public NetworkStream UpdateStream { get; private set; }
        public EndPoint ResourceEndPoint { get; private set; } 

        //constants
        public double XFrictionFactor { get; private set; }
        public double YFrictionFactor { get; private set; }
        public int PixelWidth { get; private set; }
        public int PixelHeight { get; private set; }
        public int CmWidth { get; private set; }
        public int CmHeight { get; private set; }

        private readonly Object wLock = new Object();
        private readonly Dictionary<int, AnimationWindow> windows;

        //threadsave?

        public Client(EndPoint resourceEndPoint,int pixelWidth, int pixelHeight,int CmWidth,int cmHeight)
        {
            this.ResourceEndPoint = resourceEndPoint;
            //this.WindowEndPoint = new IPEndPoint(IPAddress.Parse("10.0.0.1"), 3000);

        }

        public void Animate(double dt)
        {
            lock (wLock)
            {
                foreach (AnimationWindow w in windows.Values)
                    w.Animate(dt);
            }
        }
        /// <summary>
        ///  Returns a ClientState class representing the state of each window on this client.
        /// 
        /// it does not contain information about any cursors, this is done in the AnimationServer Class
        /// </summary>
        /// <param name="dt"></param>
        /// <returns>ClientState object for serialization over the network</returns>
        public ClientState GetAnimatedClientState() 
        {
            ClientState answer = new ClientState();
            answer.Windows = new List<WindowState>(windows.Count);
            answer.Cursors = null;
            
            lock (wLock)
            {
                foreach (AnimationWindow w in windows.Values)
                    answer.Windows.Add(w.GetWindowState());
            }

            return answer;
        }

        public AnimationWindow GetWindowAt(Point p)
        {
            AnimationWindow answer = null;
            lock (wLock)
            {
                foreach (AnimationWindow w in windows.Values)
                {
                    if (w.ContainsPoint(p))
                    {
                        answer = w;
                        break;
                    }
                }
            }
            return anwer;
        }

        public void AddWindow(AnimationWindow w)
        {
            lock (wLock)
                windows.Add(w.WindowId, w);
        }

        public void RemoveWindow(AnimationWindow w)
        {
            lock(wLock)
             windows.Remove(w.WindowId);
        }
    }
}
