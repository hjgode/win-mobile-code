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

        private int iWidth = 240;
        public int _iWidth
        {
            get { return iWidth; }
            set
            {
                iWidth = value;
                this._form.Refresh();
            }
        }
        private int iHeight = 320;
        public int _iHeight
        {
            get { return iHeight; }
            set
            {
                iHeight = value;
                this._form.Refresh();
            }
        }
        private int iXstep = 30, iYstep = 30;
		private int uniFontSize=16;
		private int uniFontSizeMap=32;

        private Label lblVersion;
        //private Panel panel;

		private Font mapFont = new Font("Arial Unicode MS", 30, FontStyle.Regular);
		public Font _mapFont{
			get {return mapFont;}
			set {mapFont=value;
				_form.Refresh();}
		}

        private void initDrawUni()
        {
            iWidth = _form.ClientRectangle.Width-iOffsetLeft;
            iHeight = _form.ClientRectangle.Height-iOffsetTop;

            this._form.Paint += new PaintEventHandler(this.PaintMap);
            this._form.Resize += new EventHandler(this.ResizeMap);

            //could assign click handler here, is done outside
            //this._form.MouseUp += new MouseEventHandler(_form_MouseUp);

            //add a dummy control to enable scrolling
            
            lblVersion = new Label();
            lblVersion.Size = new Size(20, 20);
            lblVersion.Location = new Point(iWidth, iHeight);
            _form.Controls.Add(lblVersion);
            _form.AutoScroll = true;

            _form.Refresh();
        }
        public DrawUniMap(Form f, int iOffsetY)
        {
            _form = f;
            iOffsetTop = iOffsetY;
            initDrawUni();
        }
		
		public DrawUniMap (Form f)
		{
			_form=f;

            initDrawUni();

		}

        public class MessageEventArgs : EventArgs
        {
            private string _message;
            public MessageEventArgs(string msg)
            {
                this._message = msg;
            }
            public string NewMessage
            {
                get
                {
                    return _message;
                }
            }
        }
        public delegate void KlickedEventHandler(object sender, MessageEventArgs e);
        public event KlickedEventHandler NewMessageHandler;
        protected virtual void OnMessage(MessageEventArgs e)
        {
            if (NewMessageHandler != null)
            {
                NewMessageHandler(this, e);//Raise the event
            }
        }

        public void _form_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                return;
            int x = e.X;
            int y = e.Y;
            if (x < iXstep)
                return;
            if (y < iYstep + iOffsetTop)
                return;
            int locX = (int)((x - iXstep) / iXstep);
            int locY = (int)(((y - iYstep - iOffsetTop) / iYstep)) * 0x10;
            int uCodePoint = locX + locY;
            
            //MessageBox.Show("You clicked: 0x" + locX.ToString("x02") + "/0x" + locY.ToString("x02") + " = " + uCodePoint.ToString("x02"));
            
            byte[] bytes = new byte[2];
            bytes[1] = bCurrCodepage;
            bytes[0] = (byte)uCodePoint;
            string s = Encoding.Unicode.GetString(bytes, 0, 2);
            this.OnMessage(new MessageEventArgs(s));
        }

		public void ResizeMap(object sender, EventArgs e){
            //calculate cells, we want 16 columns and 16 rows
            //plus one column for the indexing
            //iXstep = _form.Width / 18;
            //iYstep = iXstep;

			//iXstep
			int iXCell = (iWidth - iOffsetLeft) / 18;
            //iYstep 
			int iYCell = (iHeight - iOffsetTop) / 18;
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

