using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class SerializedDisplayState
    {
        public int WindowId { get; private set; }
        public byte[] State { get; private set; } 
    }
}
