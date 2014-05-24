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
    /// Represents the actual 
    /// which is responsible for drawing the windows and cursors on the screen.
    /// Threadsafe
    /// </summary>
    internal partial class Display : Form
    {
        private Object updateLock = new Object();
        private Dictionary<int, DisplayWindow> windows;
        private Dictionary<int, DisplayCursor> cursors;
        private Image background;

        public Display(string backgroundImage)
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer |
              ControlStyles.AllPaintingInWmPaint | ControlStyles.SupportsTransparentBackColor, true);
            
            //PictureResource res = new PictureResource(new Bitmap(@"C:\Users\Jon\Desktop\resource.jpg"));
            /*WaitResource res = new WaitResource(new Bitmap("loading.png"));
            w = new DisplayWindow(res);
            w.Update(new Common.WindowState() { X = 1000, Angle = 0, Y = 500, Height = 450, Width = 500, ResourceId = 0});*/
            
            Rectangle dimensions = Screen.FromControl(this).Bounds;
            this.background = new Bitmap(dimensions.Width, dimensions.Height);
            using (Graphics g = Graphics.FromImage(this.background))
                g.DrawImage(new Bitmap(backgroundImage),dimensions);
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
            g.DrawImage(this.background, new Point(0, 0));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            this.DrawBackground(g);

            lock (updateLock)
            {
                if (windows != null && cursors != null)
                {
                    System.Console.WriteLine("inDraw: "+windows.Count + " " + cursors.Count);
                    
                    foreach (DisplayWindow entry in windows.Values)
                        entry.Draw(g);

                    foreach (DisplayCursor entry in cursors.Values)
                        entry.Draw(g);
                }
            }
        }
    }
}
