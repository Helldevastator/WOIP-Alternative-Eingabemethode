using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Client.ResourceHandler
{
    class PictureResource : IResourceHandler
    {
        private Image im;


        public void OnPaint(System.Drawing.Graphics g, int width, int height)
        {
            
            g.DrawImage(im, 0, 0);
        }
    }
}
