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
                lblH2.Text = "Horizontal Point2: y:" + c.horizontal.p2.X.ToString("0.0") + " y:" + c.horizontal.p2.Y.ToString("0.0");
            }

/*  
            this.rec.X += c.xPos;
            this.rec.Y += c.yPos;
            this.relXPos.Text = this.rec.X.ToString();
            this.relYPos.Text = this.rec.Y.ToString();
            
            Graphics g = Graphics.FromImage(this.im);
            g.Clear(Color.White);

            g.FillRectangle(Brushes.Red, rec);
            
            g.Dispose();
            this.picBox.Image = this.im;*/
        }

        private void button1_Click(object sender, EventArgs e)
        {
            cursor.Calibrate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            cursor.ToZero();
        }

    }
}
