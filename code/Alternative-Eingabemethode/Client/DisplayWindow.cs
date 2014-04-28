using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Common;
using Client.ResourceHandler;

namespace Client
{
    /// <summary>
    /// Represents a Window on the Display.
    /// </summary>
    class DisplayWindow
    {
        private ClientState state;
        private IResourceHandler resource;

        public DisplayWindow()
        {
        }

        public void Update(ClientState state) {
            this.state = state;
        }

        /// <summary>
        /// Paint Window on Display
        /// </summary>
        /// <param name="g"></param>
        public void OnPaint(Graphics g)
        {
            resource.OnPaint(g, state);
        }

        /// <summary>
        /// Fired when Resource is Loaded
        /// </summary>
        /// <param name="resource"></param>
        private void ResourceLoadedListener(IResourceHandler resource)
        {
            lock(this.resource)
                this.resource = resource;
        }

    }
}
