using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [Serializable]
    public struct CursorEvent
    {
        public int cursorId;
        public int eventType;
    }
}
