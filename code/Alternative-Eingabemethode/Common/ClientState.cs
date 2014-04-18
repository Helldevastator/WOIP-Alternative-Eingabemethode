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
        List<Window> Windows;
        List<Cursor> Cursors;
    }
}
