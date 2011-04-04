#define PocketPC
using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CharMapTool
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
        private int iOffsetLeft = 4;

        private int iXstep = 30, iYstep = 30;
		private int uniFontSize=16;
		private int uniFontSizeMap=32;
		
		private Font mapFont = new Font("Arial Unicode MS", 30, FontStyle.Regular);
		public Font _mapFont{
			get {return mapFont;}
			set {mapFont=value;
				_form.Refresh();}
		}
		
		public DrawUniMap (Form f)
		{
			_form=f;
			
			this._form.Paint+=new PaintEventHandler(this.PaintMap);
			this._form.Resize+=new EventHandler(this.ResizeMap);
		}
		public DrawUniMap (Form f, int iOffsetY)
		{
			_form=f;
            iOffsetTop = iOffsetY;
			this._form.Paint+=new PaintEventHandler(this.PaintMap);
			this._form.Resize+=new EventHandler(this.ResizeMap);
            
		}
		
		public void ResizeMap(object sender, EventArgs e){
            //calculate cells, we want 16 columns and 16 rows
            //plus one column for the indexing
            //iXstep = _form.Width / 18;
            //iYstep = iXstep;

			//iXstep
			int iXCell = (_form.ClientRectangle.Width - iOffsetLeft) / 18;
            //iYstep 
			int iYCell = (_form.ClientRectangle.Height - iOffsetTop) / 18;
			iXstep = Math.Min(iXCell, iYCell);
			iYstep = iXstep;
			
			//start with large font size
			uniFontSizeMap=32;
			uniFontSize=16;
			
			_form.Refresh();
		}
		//public delegate void PaintEventHandler();
		
        public void PaintMap(object sender, PaintEventArgs e)
        {
			
            int iX = 0, iY = 0;
            byte bHigh = bCurrCodepage;// 0x04;
            byte bLow = 0x00;


			//calc maximum font cell size, for headers
			uniFontSize = 
				this.getMaxFonzSize(e.Graphics, 
				     new Font("Arial Unicode MS", 10, FontStyle.Regular),
				                     iXstep,
				                     1);

			//calc maximum font size for map chars
			uniFontSizeMap = this.getMaxFonzSize(e.Graphics, 
				     _mapFont,
				                     iXstep,
				                     1);


			//headers font
            Font drawFont = new Font("Arial Unicode MS", uniFontSize, FontStyle.Regular);

			SolidBrush drawBrush = new SolidBrush(Color.Black);
			//draw header row
            for (iX = 0; iX < 16; iX++)
            {
#if PocketPC
                e.Graphics.DrawString(iX.ToString("x2"), drawFont, drawBrush,
                    new RectangleF(iX * iXstep  + iXstep + iOffsetLeft, 
				                   /*iYstep +*/ iOffsetTop, 
				                   iXstep, 
				                   iYstep));
#else
                e.Graphics.DrawString(iX.ToString("x2"),
                        drawFont, drawBrush,
                        new Point(iX * iXstep + iXstep, iYstep));
#endif
            }
            for (iY = 0; iY < 16; iY++)
            {
                int xx = (iY * 0x10);
				//draw header column
#if PocketPC
                e.Graphics.DrawString(xx.ToString("x2"),
                        drawFont, drawBrush,
                        new RectangleF(
				              0 + iOffsetLeft, 								//always left
				              iY * iYstep + iOffsetTop + iYstep, 
				              iXstep, 
				              iYstep));
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
                    Point p = new Point(iX * iXstep + iXstep + iOffsetLeft, 
					                    iY * iYstep + iOffsetTop + iYstep);
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

            //Font drawFont = new Font("Arial Unicode MS", uniFontSize, FontStyle.Regular);
			mapFont = new Font(mapFont.Name, uniFontSizeMap, FontStyle.Regular);
			
            SolidBrush drawBrush = new SolidBrush(Color.Black);

            // Draw string to screen.
#if PocketPC
            e.Graphics.DrawString(uStr,
                        mapFont, 
			            drawBrush,
                        new RectangleF(drawPoint.X, drawPoint.Y, iXstep, iYstep));
#else

            e.Graphics.DrawString(uStr, drawFont, drawBrush, drawPoint);
#endif
        }

		private int getMaxFonzSize(Graphics g, Font font, int iCellWidth, int iNumChars){
			int iRet = 10;
			Font testFont; // = font.Clone();
			
			int iFSize = (int)font.Size;
			testFont = new Font( font.Name, iFSize, FontStyle.Regular);
			String sChars="";
			for(int x=0; x<iNumChars; x++){
				sChars+="W";
			}

			while(Math.Max( g.MeasureString(sChars, testFont).Width, 
			               g.MeasureString(sChars, testFont).Height)>iCellWidth){
				iFSize--;
				testFont = new Font( font.Name, iFSize, FontStyle.Regular);
			}
			//SizeF sizeF = g.MeasureString("00", font);
			iRet = iFSize;
			System.Diagnostics.Debug.WriteLine("Cell max="+iRet.ToString());
			return iRet;
		}
	}
}

