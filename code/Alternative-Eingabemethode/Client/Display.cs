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
    public partial class Display : Form
    {
        DisplayWindow w;
        //cursors
        public Display()
        {
            InitializeComponent();
            PictureResource res = new PictureResource(new Bitmap(@"C:\Users\Jon\Documents\GitHub\WOIP-Alternative-Eingabemethode\code\Alternative-Eingabemethode\example.bmp"));
            w = new DisplayWindow(res);
            w.Update(new Common.WindowState() { X = 300, Angle = 45, Y = 200, Height = 400, Width = 600, ResourceId = 0});
        }

        public void AddWindow()
        {
        }
        public void RemoveWindow()
        {
        }
        public void Contains()
        {
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            w.OnPaint(g);

        }
    }
}
