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
        private WindowState state;
        private IResourceHandler resource;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loadingResource">Resource to display while it waits for the actual one</param>
        public DisplayWindow(IResourceHandler loadingResource)
        {
            lock (state)
            {
                this.resource = resource;
                this.state = null;
            }
        }

        public void Update(WindowState state) {
            this.state = state;
        }

        /// <summary>
        /// Paint Window on Display
        /// </summary>
        /// <param name="g"></param>
        public void OnPaint(Graphics g)
        {
            lock (state)
            {
                if (state != null)
                {
                    Bitmap bmp = new Bitmap(state.Width, state.Height);
                    double opacity = 1;

                    using (Graphics window = Graphics.FromImage(bmp))
                    {
                        resource.OnPaint(window, state.Width, state.Height);
                    }

                    //rotate image
                    Matrix translationState = g.Transform;
                    g.TranslateTransform(state.X, state, Y);
                    g.RotateTransform(state.Angle);
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
            lock (state)
            {
                this.resource = resource;
            }
        }

    }
}
