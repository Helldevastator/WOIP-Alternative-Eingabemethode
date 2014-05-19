using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
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
            Matrix translationState = g.Transform;
            g.TranslateTransform(state.X, state.Y);
            Rectangle rec = new Rectangle(0,0,20,20);
            g.FillRectangle(Brushes.Red, rec);
            g.Transform = translationState;
        }
    }
}
