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


        public Form1()
        {
            InitializeComponent();
            this.rec = new Rectangle(this.picBox.Size.Width / 2, this.picBox.Size.Height / 2, 20, 20);
            im = new Bitmap(picBox.Width, picBox.Height);
            cursor = new InputCursor(new Wiimote());
            cursor.CursorUpdated += new CursorEvent(CursorListener);

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

            this.lblRelX.Text = c.yaw.ToString("0.0000")+"yaw deg/s";
            this.lblRelY.Text = c.pitch.ToString("0.0000") + "pitch deg/s";
            this.lblRelZ.Text = c.roll.ToString("0.0000") + "roll deg/s";
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

    }
}
