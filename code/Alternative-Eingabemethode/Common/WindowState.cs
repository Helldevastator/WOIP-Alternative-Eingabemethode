using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    //needs to be threadsave?
    /// <summary>
    /// Represents a Window somewhere on a client.
    /// </summary>
    [Serializable]
    public struct WindowState
    {
        private static Object nextIdLock = new Object();
        private static int nextId = 0;

        public int WindowId { get; private set; }
        public int X { get; set; }
        public int Y { get; set; }
        
        public int Width2 { get; set; }
        public int Height2 { get; set; }
        public double Angle { get; set; }
        public int ResourceId { get; set; }

        public WindowState(int resourceId) : this()
        {
            lock (nextIdLock)
            {
                this.WindowId = nextId;
                nextId++;
            }

            this.ResourceId = resourceId;
        }
    }
}
