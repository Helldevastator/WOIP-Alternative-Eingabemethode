using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ResourceHandler
{
    /// <summary>
    /// Represents a ResourceHandlerFactory which is responsible for creating a new ResourceHanlder implementation for the given resourceTypeId
    /// </summary>
    interface IResourceHandlerFactory
    {
        IResourceHandler CreateResourceHandler(int resourceTypeId, System.IO.FileInfo file);
    }
}
