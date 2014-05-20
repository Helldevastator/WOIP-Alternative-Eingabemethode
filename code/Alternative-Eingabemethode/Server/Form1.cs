using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Server.Input;
using WiimoteLib;

namespace Server
{
    public partial class Form1 : Form
    {
        private Rectangle rec;
        private Image im;
        private MoteController cursor;
        private double barSizeCM = 20;

        public Form1()
        {
            InitializeComponent();
            this.rec = new Rectangle(this.picBox.Size.Width / 2, this.picBox.Size.Height / 2, 20, 20);
            im = new Bitmap(picBox.Width, picBox.Height);
            cursor = new MoteController(new Wiimote());
            cursor.MoteUpdatedEvent += new StateListener(CursorListener);
            clb1.Items.AddRange(new object[] {
            "IR 1",
            "IR 2",
            "IR 3",
            "IR 4"});
        }

        private void CursorListener(MoteController sender, MoteState c)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.Update(sender, c);
            });
            
        }

        private void Update(MoteController sender, MoteState c)
        {
            this.lblRelX.Text = c.yaw.ToString("0.0")+" yaw d deg "+c.yawFast.ToString();
            this.lblRelY.Text = c.pitch.ToString("0.0") + " pitch d deg " + c.pitchFast.ToString();
            this.lblRelZ.Text = c.roll.ToString("0.0") + " roll deg " + c.rollFast.ToString();
            this.lblIRBar.Text = c.configuration.ToString();
            this.clb1.SetItemChecked(0, c.point1);
            this.clb1.SetItemChecked(1, c.point2);
            this.clb1.SetItemChecked(2, c.point3);
            this.clb1.SetItemChecked(3, c.point4);
            if (c.vertical == null)
            {
                lblV1.Text = "Vertical Point1: None";
                lblV2.Text = "Vertical Point2: None";
            }
            else
            {
                lblV1.Text = "Vertical Point1: x:" + c.vertical.p1.X.ToString("0.0") + " y:" + c.vertical.p1.Y.ToString("0.0");
                lblV2.Text = "Vertical Point2: x:" + c.vertical.p2.X.ToString("0.0") + " y:" + c.vertical.p2.Y.ToString("0.0");
            }

            if (c.horizontal == null)
            {
                lblH1.Text = "Horizontal Point1: None";
                lblH2.Text = "Horizontal Point2: None";
            }
            else
            {
                lblH1.Text = "Horizontal Point1: x:" + c.horizontal.p1.X.ToString("0.0") + " y:" + c.horizontal.p1.Y.ToString("0.0");
                lblH2.Text = "Horizontal Point2: x:" + c.horizontal.p2.X.ToString("0.0") + " y:" + c.horizontal.p2.Y.ToString("0.0");
            }


                System.Drawing.Point p = this.calcPoint(c);

                this.rec.X = p.X;
                this.rec.Y = p.Y;
            
            
            Graphics g = Graphics.FromImage(this.im);
            g.Clear(Color.White);

            g.FillRectangle(Brushes.Red, rec);
            
            g.Dispose();
            this.picBox.Image = this.im;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            cursor.Calibrate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            cursor.ToZero();
        }

        private System.Drawing.Point calcPoint(MoteState state)
        {
            if ((state.horizontal != null || state.vertical != null))
            {
                int moteWidth = MoteController.IR_PIXEL_WIDTH;
                int moteHeight = MoteController.IR_PIXEL_HEIGHT;
                double xPointAt = 0;    // centimeters of where the wiimote is pointing. (0,0) is in the upper left corner
                double yPointAt = 0;

                //convert to screen coordinates, coordinate center is the upper left screen corner;
                if (state.horizontal != null)
                {
                    //calculate distance in pixels between the ir points;
                    double dx = (state.horizontal.p2.X - state.horizontal.p1.X) * moteWidth;
                    double dy = (state.horizontal.p2.Y - state.horizontal.p1.Y) * moteHeight;
                    double distance = Math.Sqrt(dx * dx + dy * dy);

                    InputPoint centerDistance = CalcDistanceToCenter(state.horizontal);
                    xPointAt = centerDistance.X * moteWidth / distance * barSizeCM + 30 / (double)2;
                    if (state.configuration == IRBarConfiguration.LEFT_TOP || state.configuration == IRBarConfiguration.RIGHT_TOP)
                        yPointAt = (centerDistance.Y * moteHeight) / distance * barSizeCM;

                    else
                        yPointAt = 20 - (centerDistance.Y * moteHeight / distance * barSizeCM);

                }
                else
                {
                    //calculate distance between the ir points;
                    double dx = (state.vertical.p2.X - state.vertical.p1.X) * moteWidth;
                    double dy = (state.vertical.p2.Y - state.vertical.p1.Y) * moteHeight;
                    double distance = Math.Sqrt(dx * dx + dy * dy);

                    InputPoint centerDistance = CalcDistanceToCenter(state.vertical);
                    yPointAt = -centerDistance.Y * moteHeight / distance * barSizeCM + 20 / (double)2;
                    if (state.configuration == IRBarConfiguration.RIGHT_BOTTOM || state.configuration == IRBarConfiguration.RIGHT_TOP)
                        xPointAt =  30- (centerDistance.X * moteWidth / distance * barSizeCM);
                    else
                        xPointAt = (centerDistance.X * moteWidth / distance * barSizeCM);
                }

                int xPixel = (int)(xPointAt / 30 * im.Width);
                int yPixel = (int)(yPointAt / 20 * im.Height);
                System.Console.WriteLine(xPointAt + " " + yPointAt);
                System.Console.WriteLine(xPixel + " " + yPixel);
                return new System.Drawing.Point(xPixel, yPixel);
            }
            else
                return new System.Drawing.Point(-1, -1);
        }

        private InputPoint CalcDistanceToCenter(BarPoints bar)
        {
            double x = (bar.p2.X - bar.p1.X) / 2;
            double y = (bar.p2.Y - bar.p1.Y) / 2;

            double centerX = bar.p1.X + x;
            double centerY = bar.p1.Y + y;

            return new InputPoint(0.5 - centerX, 0.5 - centerY);
        }
    }
}
