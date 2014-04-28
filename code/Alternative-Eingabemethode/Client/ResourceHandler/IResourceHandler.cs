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
       void OnPaint(Graphics g, int width, int height,ImageAttributes attributes);
        
    }
}
