using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Common;
using Client.ResourceHandler;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Client
{
    /// <summary>
    /// Represents a Window on the Display.
    /// 
    /// Threadsafe
    /// </summary>
    class DisplayWindow
    {
        private Object stateLock = new Object();
        private WindowState state;
        private IResourceHandler resource;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loadingResource">Resource to display while it waits for the actual one</param>
        public DisplayWindow(IResourceHandler loadingResource)
        {
            lock (stateLock)
            {
                this.resource = loadingResource;
                this.state = null;
            }
        }

        public void Update(WindowState state) {
            lock(stateLock) 
                this.state = state;
        }

        /// <summary>
        /// Paint Window on Display
        /// </summary>
        /// <param name="g"></param>
        public void OnPaint(Graphics g)
        {
            lock (stateLock)
            {
                if (state != null)
                {
                    float opacity = 0.50f;

                    //rotate image
                    Matrix translationState = g.Transform;
                    
                    g.TranslateTransform(state.X, state.Y);
                    g.RotateTransform(state.Angle, MatrixOrder.Prepend);
                    g.TranslateTransform(-state.Width/2, -state.Height/2);

                    
                    //draw opaque
                    ColorMatrix matrix = new ColorMatrix();
                    matrix.Matrix33 = opacity;
                    ImageAttributes attributes = new ImageAttributes();
                    attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                    this.resource.OnPaint(g, state.Width, state.Height,attributes);
                    g.Transform = translationState;
                }
            }
        }

        /// <summary>
        /// Callback for when the resource is loaded.
        /// </summary>
        /// <param name="resource"></param>
        public void ResourceLoadedCallback(IResourceHandler resource)
        {
            //doesn't need to be locked but needed for happens-before relation
            lock (stateLock)
            {
                this.resource = resource;
            }
        }

    }
}
