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
        private static readonly Image img = Image.FromFile("cursor.png");
        private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

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
            float angle = (float)(timeMillis() / 1000.0 % 360.0);
            System.Console.WriteLine(angle.ToString());
            Matrix translationState = g.Transform;
            g.TranslateTransform(state.X, state.Y);
            g.RotateTransform(angle);
            g.TranslateTransform(-img.Width / 2, -img.Height / 2);
            Rectangle rec = new Rectangle(0,0,20,20);
            g.DrawImage(img, new Point(0, 0));
            g.Transform = translationState;
        }

        private static long timeMillis()
        {
            return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }
    }
}
