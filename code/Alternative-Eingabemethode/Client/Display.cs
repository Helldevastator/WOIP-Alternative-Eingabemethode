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
        Dictionary<int, DisplayWindow> windows;
        Dictionary<int, DisplayCursor> cursors;

        public Display()
        {
            InitializeComponent();
            //this.WindowState = FormWindowState.Maximized;
            //PictureResource res = new PictureResource(new Bitmap(@"C:\Users\Jon\Documents\GitHub\WOIP-Alternative-Eingabemethode\code\Alternative-Eingabemethode\example.bmp"));
            /*PictureResource res = new PictureResource(new Bitmap(@"C:\Users\Jon\Desktop\testClient\0"));
            w = new DisplayWindow(res);
            w.Update(new Common.WindowState() { X = 300, Angle = 45, Y = 200, Height = 400, Width = 300, ResourceId = 0});*/
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

            lock (updateLock)
            {
                if (windows != null && cursors != null)
                {
                    System.Console.WriteLine("inDraw: "+windows.Count + " " + cursors.Count);
                    
                    foreach (DisplayWindow entry in windows.Values)
                        entry.Draw(g);

                    /*foreach (DisplayCursor entry in cursors.Values)
                        entry.Draw(g);*/
                }
            }
        }
    }
}
