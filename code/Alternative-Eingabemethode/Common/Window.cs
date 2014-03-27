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
    public sealed struct Window
    {
        private static Object nextIdLock = new Object();
        private static int nextId = 0;

        public int WindowId { public get; private set; }
        public int TileId { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public int ResourceId { public get; private set; }
        //fullscreen?
        //stateID??

        public Window(int resourceId)
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
