using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class SerializedDisplayState
    {
        public readonly int WindowId { public get; private set; }
        public readonly byte[] State { public get; private set; } 
    }
}
