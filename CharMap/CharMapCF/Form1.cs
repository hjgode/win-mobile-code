using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CharMapCF
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.SuspendLayout();
            Button btnOK = new Button();
            btnOK.Text = "OK";
            btnOK.Location = new System.Drawing.Point(4, 4);
            btnOK.Size = new System.Drawing.Size(80, 24);
            this.Controls.Add(btnOK);
            btnOK.Click += new EventHandler(btnOK_Click);

            lb = new ComboBox();
            lb.DropDownStyle = ComboBoxStyle.DropDownList;
            lb.Items.Clear();
            for (int i = 0; i < 0xff; i++)
            {
                lb.Items.Insert(i, i.ToString("x02"));
            }
            lb.SelectedIndex = 0x04;
            lb.Location = new Point(100, 4);
            lb.Size = new System.Drawing.Size(80, 60);
            lb.SelectedIndexChanged += new EventHandler(lb_SelectedIndexChanged);
            this.Controls.Add(lb);

            this.Paint += new PaintEventHandler(Form_Paint);

            //calculate cells, we want 16 columns and 16 rows
            //plus one column for the indexing
            iXstep = Screen.PrimaryScreen.WorkingArea.Width / 18;
            iYstep = iXstep;

            this.ResumeLayout();
        }

        ComboBox lb;
        byte bCurrCodepage = 0x04;
        int iOffsetTop = 30;
        int iXstep = 30, iYstep = 30;

        private void lb_SelectedIndexChanged(object sender, EventArgs e)
        {
            bCurrCodepage = (byte)lb.SelectedIndex;
            this.Refresh();
        }

        private void Form_Paint(object sender, PaintEventArgs e)
        {
            /*
            // Create string to draw.
            // 0x0416 -> Ж
            String drawString = "Sample Text: Ж / " +uStr ;
            // Create font and brush.
            Font drawFont = new Font("Arial Unicode MS", 16);
            SolidBrush drawBrush = new SolidBrush(Color.Black);
		             
            // Create point for upper-left corner of drawing.
            PointF drawPoint = new PointF(150.0F, 150.0F);
		             
            // Draw string to screen.
            e.Graphics.DrawString(drawString, drawFont, drawBrush, drawPoint);
            */
            int iX = 0, iY = 0;
            byte bHigh = bCurrCodepage;// 0x04;
            byte bLow = 0x00;


            Font drawFont = new Font("Arial Unicode MS", 10, FontStyle.Regular);
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            for (iX = 0; iX < 16; iX++)
            {
#if PocketPC
                e.Graphics.DrawString(iX.ToString(), drawFont, drawBrush,
                    new RectangleF(iX * iXstep + iXstep, iYstep, iXstep, iYstep));
#else
                e.Graphics.DrawString(iX.ToString("x2"),
                        drawFont, drawBrush,
                        new Point(iX * iXstep + iXstep, iYstep));
#endif
            }
            for (iY = 0; iY < 16; iY++)
            {
                int xx = (iY * 0x10);
#if PocketPC
                e.Graphics.DrawString(xx.ToString("x2"),
                        drawFont, drawBrush,
                        new RectangleF(0, iY * iYstep + iOffsetTop + iYstep, iXstep, iYstep));
#else
                e.Graphics.DrawString(xx.ToString("x2"),
                        drawFont, drawBrush,
                        new Point(0, iY * iYstep + iOffsetTop + iYstep));
#endif
            }

            for (iY = 0; iY < 16; iY++)
            {
                for (iX = 0; iX < 16; iX++)
                {
                    Point p = new Point(iX * iXstep + iXstep, iY * iYstep + iOffsetTop + iYstep);
                    byte[] b = new byte[2];
                    b[0] = bLow++; b[1] = bHigh;

                    DrawUniChar(e, b, p);
                }
            }
        }

        public void DrawUniChar(PaintEventArgs e, byte[] bUni, Point drawPoint)
        {
            byte[] bCode = bUni;
            //bCode[1]=0x04; bCode[0]=0x16;
            char[] uChr = Encoding.Unicode.GetChars(bCode);
            String uStr = new string(uChr);

            Font drawFont = new Font("Arial Unicode MS", 10, FontStyle.Regular);
            SolidBrush drawBrush = new SolidBrush(Color.Black);

            // Draw string to screen.
#if PocketPC
            e.Graphics.DrawString(uStr,
                        drawFont, drawBrush,
                        new RectangleF(drawPoint.X, drawPoint.Y, iXstep, iYstep));
#else

            e.Graphics.DrawString(uStr, drawFont, drawBrush, drawPoint);
#endif
        }

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

    }
}