using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [Serializable]
    public sealed class CursorEvent
    {
        public int CursorId { public get; private set; }
        public int EventType {public get; private set;}

    }
}
