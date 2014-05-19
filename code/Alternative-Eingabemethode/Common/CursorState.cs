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
        public int CursorId;
        public int X;
        public int Y;
        public bool Activated;
    }
}
