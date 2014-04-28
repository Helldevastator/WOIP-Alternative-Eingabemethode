using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Common;

namespace Client.ResourceHandler
{
    interface IResourceHandler
    {
        public void OnPaint(Graphics g, WindowState state);
        
    }
}
