using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Client.ResourceHandler;

namespace Client
{
    /// <summary>
    /// Represents the actual display which is responsible for drawing the windows and cursors on the screen.
    /// Threadsafe
    /// </summary>
    public partial class Display : Form
    {
        private Object updateLock = new Object();
        Dictionary<int, DisplayWindow> windows;
        Dictionary<int, DisplayCursor> cursors;
        
        public Display()
        {
            this.windows = windows;
            this.cursors = cursors;

            InitializeComponent();
            /*PictureResource res = new PictureResource(new Bitmap(@"C:\Users\Jon\Documents\GitHub\WOIP-Alternative-Eingabemethode\code\Alternative-Eingabemethode\example.bmp"));
            w = new DisplayWindow(res);
            w.Update(new Common.WindowState() { X = 300, Angle = 45, Y = 200, Height = 400, Width = 600, ResourceId = 0});*/
        }

        public void UpdateDisplay(Dictionary<int, DisplayWindow> windows,Dictionary<int, DisplayCursor> cursors)
        {
            lock (updateLock)
            {
                this.windows = windows;
                this.cursors = cursors;
            }
            this.Invalidate();
        }

        private void DrawBackground(Graphics g)
        {
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            this.DrawBackground(g);
            //w.Draw(g);

            lock (updateLock)
            {
                foreach (DisplayWindow w in windows)
                    w.Draw(g);

                foreach (DisplayCursor c in cursors)
                    c.Draw(g);
            }
        }
    }
}
