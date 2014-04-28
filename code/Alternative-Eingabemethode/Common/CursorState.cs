using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [Serializable]
    public sealed class CursorState
    {
        public int cursorId;
        public int x;
        public int y;
        public bool activated;
    }
}
