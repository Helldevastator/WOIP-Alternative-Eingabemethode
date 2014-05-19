using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Common;
using System.Drawing.Imaging;

namespace Client.ResourceHandler
{
    interface IResourceHandler
    {
        /// <summary>
        /// Draw function which displays a resource on the resourceListenerPoint (0,0) of the Graphics object.
        /// Idempotent function. The call should always draw the same image in g if the attributes are the same.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="width">width of the area in pixels</param>
        /// <param name="height">Height of the area in pixels</param>
        /// <param name="attributes">Used to alter the image drawn to g. For example, making it transparent</param>
       void Draw(Graphics g, int width, int height,ImageAttributes attributes);
        
    }
}
