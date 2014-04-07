using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WiimoteLib;

namespace Server
{
    public partial class Form1 : Form
    {
        private Rectangle rec;
        private Image im;
        private InputCursor cursor;

        private int count = 0;

        //variance
        private float[] sampleX = new float[100];
        private float[] sampleY = new float[100];
        private float[] sampleZ = new float[100];
        private bool firstTime = true;
        private double sumX;
        private double sumY;
        private double sumZ;

        public Form1()
        {
            InitializeComponent();
            this.rec = new Rectangle(this.picBox.Size.Width / 2, this.picBox.Size.Height / 2, 20, 20);
            im = new Bitmap(picBox.Width, picBox.Height);
            cursor = new InputCursor(new Wiimote());
            cursor.CursorUpdated += new CursorEvent(CursorListener);

            btnVariance.Enabled = false;
        }

        private void CursorListener(InputCursor sender, CursorInfo c)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.Update(sender, c);
            });
        }

        private void Update(InputCursor sender, CursorInfo c)
        {
            this.barXAbsolute.Value = (int)((c.XAbs + 3) / 6 * 100) % 100;
            this.barYAbsolute.Value = (int)((c.YAbs + 3) / 6 * 100) % 100;
            this.barZAbsolute.Value = (int)((c.ZAbs + 3) / 6 * 100) % 100;

            this.barRelativeX.Value = (int)((c.XRel + 3) / 6 * 100) % 100;
            this.barRelativeY.Value = (int)((c.YRel + 3) / 6 * 100) % 100;
            this.barRelativeZ.Value = (int)((c.ZRel + 3) / 6 * 100) % 100;
            this.lblRelX.Text = c.XRel.ToString("0.0000");
            this.lblRelY.Text = c.YRel.ToString("0.0000");
            this.lblRelZ.Text = c.ZRel.ToString("0.0000");

            this.rec.X += c.xPos;
            this.rec.Y += c.yPos;
            this.relXPos.Text = this.rec.X.ToString();
            this.relYPos.Text = this.rec.Y.ToString();

            Graphics g = Graphics.FromImage(this.im);
            g.Clear(Color.White);

            g.FillRectangle(Brushes.Red, rec);
            
            g.Dispose();
            this.picBox.Image = this.im;

            this.calcVariance(c);
        }

        private void calcVariance(CursorInfo c)
        {
            this.sumX -= this.sampleX[count];
            this.sumY -= this.sampleY[count];
            this.sumZ -= this.sampleZ[count];
            this.sampleX[count] = c.XRel;
            this.sampleY[count] = c.YRel;
            this.sampleZ[count] = c.ZRel;
            this.sumX += this.sampleX[count];
            this.sumY += this.sampleY[count];
            this.sumZ += this.sampleZ[count];

            count++;

            if (firstTime)
            {
                firstTime = count < this.sampleX.Length;
                btnVariance.Enabled = !firstTime;
            }

            count = count % this.sampleX.Length;
        }

        private void btnVariance_Click(object sender, EventArgs e)
        {
            double avX = sumX / this.sampleX.Length;
            double avY = sumY / this.sampleX.Length;
            double avZ = sumZ / this.sampleX.Length;

            double varX = 0;
            double varY = 0;
            double varZ = 0;

            for (int i = 0; i < this.sampleX.Length; i++)
            {
                double var = sampleX[i] - avX;
                varX += var * var;
                var = sampleY[i] - avY;
                varY += var * var;
                var = sampleZ[i] - avZ;
                varZ += var * var;
            }

            //varX /= this.sampleX.Length;
            //varY /= this.sampleX.Length;
            //varZ /= this.sampleX.Length;
            this.varX.Text = varX.ToString("0.0000");
            this.varY.Text = varY.ToString("0.0000");
            this.varZ.Text = varZ.ToString("0.0000");
            this.averageX.Text = avX.ToString("0.0000");
            this.averageY.Text = avY.ToString("0.0000");
            this.averageZ.Text = avZ.ToString("0.0000");
        }
    }
}
