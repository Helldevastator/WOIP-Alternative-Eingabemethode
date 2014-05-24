using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Client.ResourceHandler
{
    class WaitResource : IResourceHandler
    {
        
        private Brush bgBrush = new SolidBrush(Color.FromArgb(82, 85, 91));

        public void Draw(System.Drawing.Graphics g, int width, int height, System.Drawing.Imaging.ImageAttributes attributes)
        {
            g.FillRectangle(bgBrush, new Rectangle(0, 0, width, height));
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            Matrix transformationState = g.Transform;
            g.InterpolationMode = InterpolationMode.HighQualityBilinear;

            g.DrawString("Loading", new Font("Impact", 25), Brushes.White, new Point(width / 2, height / 2), stringFormat);
            g.Transform = transformationState;
        }
    }
}
