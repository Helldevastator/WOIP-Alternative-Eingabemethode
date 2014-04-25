using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [Serializable]
    public sealed class ClientState
    {
        public List<WindowState> Windows;
        public List<CursorState> Cursors;
    }
}
