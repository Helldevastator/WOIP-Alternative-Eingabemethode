using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// Represents a Window somewhere on a client.
    /// </summary>
    [Serializable]
    public class Window
    {
        public int WindowId { public get; private set; }
        public int TileId { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public int ResourceId { public get; private set; }
        //fullscreen?
        //stateID??

        public Window(long resourceId)
        {
            this.ResourceId = resourceId;
        }
    }
}
