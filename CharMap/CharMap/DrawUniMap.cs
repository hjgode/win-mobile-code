#define PocketPC
using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CharMap
{
	public class DrawUniMap
	{
		private Form _form;
		public byte _bCurrCodepage{
			get{return bCurrCodepage;}
			set{bCurrCodepage=value;}
		}
        private byte bCurrCodepage = 0x04;
        private int iOffsetTop = 30;
        private int iXstep = 30, iYstep = 30;
		private int uniFontSize=16;
		
		public DrawUniMap (Form f)
		{
			_form=f;
            //calculate cells, we want 16 columns and 16 rows
            //plus one column for the indexing
            iXstep = _form.Width / 18;
            iYstep = iXstep;

		}
		//public delegate void PaintEventHandler();
		
        public void PaintMap(object sender, PaintEventArgs e)
        {
			
            int iX = 0, iY = 0;
            byte bHigh = bCurrCodepage;// 0x04;
            byte bLow = 0x00;


			//calc maximum font cell size
			uniFontSize = this.getFontCellRect(e.Graphics, new Font("Arial Unicode MS", 10, FontStyle.Regular));
			
            Font drawFont = new Font("Arial Unicode MS", uniFontSize, FontStyle.Regular);

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

            Font drawFont = new Font("Arial Unicode MS", uniFontSize, FontStyle.Regular);
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

		private int getFontCellRect(Graphics g, Font font){
			int iRet = 10;
			SizeF sizeF = g.MeasureString("00", font);
			iRet = (int)sizeF.Width;
			Console.WriteLine("Cell max="+iRet.ToString());
			return iRet;
		}
	}
}

