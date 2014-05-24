using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Common;
using System.Net.Sockets;
using System.Drawing;

namespace Server
{
    /// <summary>
    /// Needs to be threadsafe
    /// </summary>
    public class Client
    {
        //possible performance increase: save writeWindowData() byte array and update modified

        public readonly Socket UpdateSocket;
        public readonly EndPoint ResourceEndPoint;

        #region constants
        public readonly int Id;
        public readonly double XFrictionFactor;
        public readonly double YFrictionFactor;
        public readonly int PixelWidth;
        public readonly int PixelHeight;
        public readonly int CmWidth;
        public readonly int CmHeight;
        #endregion

        private readonly Object wLock = new Object();
        private readonly Dictionary<int, AnimationWindow> windows;
        private readonly Dictionary<int, AnimationWindow> removedWindows;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="updateSocket"></param>
        /// <param name="resourceEndPoint"></param>
        /// <param name="pixelWidth"></param>
        /// <param name="pixelHeight"></param>
        /// <param name="cmWidth"></param>
        /// <param name="cmHeight"></param>
        public Client(int Id,Socket updateSocket, EndPoint resourceEndPoint,int pixelWidth, int pixelHeight,int cmWidth,int cmHeight)
        {
            this.Id = Id;
            this.UpdateSocket = updateSocket;
            this.ResourceEndPoint = resourceEndPoint;
            this.PixelHeight = pixelHeight;
            this.PixelWidth = pixelWidth;
            this.CmWidth = cmWidth;
            this.CmHeight = cmHeight;
            this.XFrictionFactor = 0.1;
            this.YFrictionFactor = 0.1;
            windows = new Dictionary<int, AnimationWindow>();
            removedWindows = new Dictionary<int, AnimationWindow>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
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
            answer.Windows = new List<WindowState>(windows.Count+removedWindows.Count);
            answer.Cursors = null;
            
            lock (wLock)
            {
                foreach (AnimationWindow w in windows.Values)
                    answer.Windows.Add(w.GetWindowState());

                foreach (AnimationWindow w in removedWindows.Values)
                {
                    System.Console.WriteLine("Sending removed flag");
                    WindowState state = w.GetWindowState();
                    state.RemovedFlag = true;
                    answer.Windows.Add(state);
                }

                removedWindows.Clear();
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
            return answer;
        }

        public void AddWindow(AnimationWindow w)
        {
            lock (wLock)
            {
                windows.Add(w.WindowId, w);
                removedWindows.Remove(w.WindowId);
            }
        }

        public void RemoveWindow(AnimationWindow w)
        {
            lock (wLock)
            {
                windows.Remove(w.WindowId);
                if(!removedWindows.ContainsKey(w.WindowId))
                    removedWindows.Add(w.WindowId, w);
            }
        }
    }
}
