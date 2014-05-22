using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Client.ResourceHandler
{
    class PictureResource : IResourceHandler
    {
        private Image im;

        public PictureResource(Image im)
        {
            this.im = im;
        }

        public void Draw(Graphics g, int width, int height,ImageAttributes attributes)
        {
            Matrix transformationState = g.Transform;

            g.InterpolationMode = InterpolationMode.HighQualityBilinear;
            GraphicsUnit unit = GraphicsUnit.Pixel;
            RectangleF bounds = im.GetBounds(ref unit);
            //g.ScaleTransform(width / bounds.Width, height / bounds.Height);
            g.DrawImage(im, new Rectangle(0, 0, width, height), 0, 0, width, height, GraphicsUnit.Pixel, attributes);

            g.Transform = transformationState;
        }
    }
}
