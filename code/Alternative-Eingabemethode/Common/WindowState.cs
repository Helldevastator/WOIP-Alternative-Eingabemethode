﻿using System;
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
    public sealed class WindowState
    {
        private static Object nextIdLock = new Object();
        private static int nextId = 0;

        public int WindowId { get; private set; }
        public int X { get; set; }
        public int Y { get; set; }
        
        public int Width { get; set; }
        public int Height { get; set; }
        public float Angle { get; set; }
        public int ResourceId { get; set; }
        public bool RemovedFlag { get; set; }

        public WindowState(int resourceId)
        {
            lock (nextIdLock)
            {
                this.WindowId = nextId;
                nextId++;
            }

            this.ResourceId = resourceId;
        }

        public WindowState()
        {
        }
    }
}