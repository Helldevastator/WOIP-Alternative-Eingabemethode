using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Client.ResourceHandler
{
    class WaitResource : IResourceHandler
    {
        public void Draw(System.Drawing.Graphics g, int width, int height, System.Drawing.Imaging.ImageAttributes attributes)
        {
            Pen p = new Pen(Brushes.Purple);
            g.DrawRectangle(p,new Rectangle(0,0,width,height));
        }
    }
}
