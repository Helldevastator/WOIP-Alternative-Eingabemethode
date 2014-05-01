using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Client
{
    /// <summary>
    /// Represents a cursor on the display
    /// </summary>
    class DisplayCursor
    {
        CursorState state;

        public DisplayCursor(CursorState state)
        {
            this.state = state;
        }

        public void Update(CursorState state)
        {
            this.state = state;
        }

        public void Draw(Graphics g)
        {
        }
    }
}
