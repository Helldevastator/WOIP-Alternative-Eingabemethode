using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ResourceHandler
{
    class ResourceHandlerFactory :IResourceHandlerFactory
    {
        public ResourceHandlerFactory()
        {
        }

        public IResourceHandler CreateResourceHandler(int resourceTypeId, System.IO.FileInfo file)
        {
            return new PictureResource(new System.Drawing.Bitmap(file.FullName));
        }
    }
}
