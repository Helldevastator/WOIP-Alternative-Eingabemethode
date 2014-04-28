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
                this.resource = resource;
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
                    Bitmap bmp = new Bitmap(state.Width, state.Height);
                    float opacity = 1;

                    using (Graphics window = Graphics.FromImage(bmp))
                    {
                        resource.OnPaint(window, state.Width, state.Height);
                    }

                    //rotate image
                    Matrix translationState = g.Transform;
                    g.TranslateTransform(state.X, state.Y);
                    g.RotateTransform(state.Angle,MatrixOrder.Append);
                    g.TranslateTransform(-state.Width, state.Height);

                    //draw opaque
                    ColorMatrix matrix = new ColorMatrix();
                    matrix.Matrix33 = opacity;
                    ImageAttributes attributes = new ImageAttributes();
                    attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                    g.DrawImage(bmp, new Rectangle(0, 0, state.Width, state.Height), 0, 0, state.Width, state.Height, GraphicsUnit.Pixel, attributes);
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
