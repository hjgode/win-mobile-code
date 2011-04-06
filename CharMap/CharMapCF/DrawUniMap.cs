#define PocketPC
using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

//Test with a copy of C:\Windows\Fonts\ARIALUNI.TTF on device!

namespace CharMapTool
{
    /// <summary>
    /// draw a panel with all 256 glyphs of a font using one of the unicode Basic Multilanguage Panel unicode codepages
    /// </summary>
	public class DrawUniMap:Panel
	{
        /// <summary>
        /// a var for debug usage
        /// </summary>
        private static int msgCount=0;
        /// <summary>
        /// the 'codepage' used for display
        /// </summary>
		public byte _bCurrCodepage{
			get{return bCurrCodepage;}
			set{
                bCurrCodepage=value;
                this.Refresh();
            }
		}
        private byte bCurrCodepage = 0x04;
        
        /// <summary>
        /// the top margin for drawing the table
        /// </summary>
		private int iOffsetTop = 4;
        /// <summary>
        /// the top margin for drawing the table
        /// </summary>
        private int iOffsetLeft = 4;
        /// <summary>
        /// the initial width of the table (panel)
        /// ignored during init!
        /// </summary>
        private int iWidth = 240;
        /// <summary>
        /// the width of the panel (table)
        /// </summary>
        public int _iWidth
        {
            get { return iWidth; }
            set
            {
                iWidth = value;
                //this.Refresh();
            }
        }

        /// <summary>
        /// the initial height of the table (panel)
        /// ignored during init!
        /// </summary>
        private int iHeight = 320;
        /// <summary>
        /// the height of the panel (table)
        /// </summary>
        public int _iHeight
        {
            get { return iHeight; }
            set
            {
                iHeight = value;
                //this.Refresh();
            }
        }

        /// <summary>
        /// the initial rectangle for each char
        /// </summary>
        private int iXstep = 30, iYstep = 30;
        /// <summary>
        /// initial fontsize of the font used for header row and column
        /// </summary>
		private int uniFontSize=16;
        /// <summary>
        /// initial fontsize of the font used for the unicode glyphs table
        /// </summary>
		private int uniFontSizeMap=32;
        /// <summary>
        /// initial font used for the glyphs table chars
        /// </summary>
		private Font mapFont = new Font("Arial Unicode MS", 30, FontStyle.Regular);
		public Font _mapFont{
			get {return mapFont;}
			set {mapFont=value;
				this.Refresh();}
		}
        /// <summary>
        /// initialize the control
        /// </summary>
        private void initDrawUni()
        {            
            iWidth = this.Width-iOffsetLeft;
            iHeight = this.Height - iOffsetTop;

            this.Paint += new PaintEventHandler(this.PaintMap);
            this.Resize += new EventHandler(this.ResizeMap);

            this.MouseUp += new MouseEventHandler(_panel_MouseUp);

            this.Refresh();
        }
        /// <summary>
        /// constructor for new unicode glyphs panel
        /// </summary>
		public DrawUniMap ()
		{
            initDrawUni();
        }
        #region MessageEventArgs
        public class MessageEventArgs : EventArgs
        {
            private string _message;
            private byte[] _bytes;
            public MessageEventArgs(string msg)
            {
                this._message = msg;
                _bytes = Encoding.Unicode.GetBytes(msg);
            }
            public string NewMessage
            {
                get
                {
                    return _message;
                }
            }
            public byte[] bytes
            {
                get { return _bytes; }
            }
            public string getBytesString
            {
                get
                {
                    string sHex = "";
                    for (int i = _bytes.Length-1; i >=0 ; i--)
                    {
                        sHex += _bytes[i].ToString("x02");
                    }
                    sHex = "0x" + sHex;
                    return sHex;
                }
            }
        }
        #endregion
        #region delegates_and_events
        /// <summary>
        /// this will inform the subscriber about the char clicked
        /// </summary>
        public delegate void KlickedEventHandler(object sender, MessageEventArgs e);
        public event KlickedEventHandler NewMessageHandler;
        protected virtual void OnMessage(MessageEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("OnMessage");
            if (NewMessageHandler != null)
            {
                NewMessageHandler(this, e);//Raise the event
            }
        }
        #endregion

        /// <summary>
        /// a click will fire the OnMessage event handlers and informs about the glyh clicked
        /// </summary>
        public void _panel_MouseUp(object sender, MouseEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(msgCount++.ToString() + " _form_MouseUp");
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
        /// <summary>
        /// the resize event handler will perform some updates for the drawing
        /// </summary>
		public void ResizeMap(object sender, EventArgs e){
            System.Diagnostics.Debug.WriteLine(msgCount++.ToString() + " ResizeMap");
            iWidth = this.Width;
            iHeight = this.Height;
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
			
			this.Refresh();
		}
		/// <summary>
		/// this is the paint event handler which does all the drawing
		/// </summary>
        public void PaintMap(object sender, PaintEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(msgCount++.ToString() + " PaintMap");
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
                e.Graphics.DrawString(iX.ToString("x2"), drawFont, drawBrush,
                    new RectangleF(iX * iXstep  + iXstep + iOffsetLeft, 
				                   /*iYstep +*/ iOffsetTop, 
				                   iXstep, 
				                   iYstep));
            }
            for (iY = 0; iY < 16; iY++)
            {
                int xx = (iY * 0x10);
				//draw header column
                e.Graphics.DrawString(xx.ToString("x2"),
                        drawFont, drawBrush,
                        new RectangleF(
				              0 + iOffsetLeft, 								//always left
				              iY * iYstep + iOffsetTop + iYstep, 
				              iXstep, 
				              iYstep));
            }

            //draw all glyphs at there rectangles
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
        /// <summary>
        /// draw one glyph
        /// </summary>
        /// <param name="e"></param>
        /// <param name="bUni"></param>
        /// <param name="drawPoint"></param>
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
            e.Graphics.DrawString(uStr,
                        mapFont, 
			            drawBrush,
                        new RectangleF(drawPoint.X, drawPoint.Y, iXstep, iYstep));
        }
        /// <summary>
        /// enlarge or shrink the panel size
        /// no shrink lower than 120 and no enlarge to 2*parent.width
        /// </summary>
        /// <param name="fZoom">the maginfication factor</param>
        /// <returns>magnification factor or 0 for error</returns>
        public double zoomInOut(double fZoom)
        {
            CharMapCF.winhelper.enableRedraw(this, false);
            if (fZoom < 1)
            {
                if ((this.Width * fZoom) < 120)
                    fZoom = 0;
                else
                {
                    this.Width = (int)(this.Width * 0.8);
                    this.Height = (int)(this.Height * 0.8);
                }
            }
            else if (fZoom > 1)
            {
                if ((this.Width * fZoom) > 2 * Screen.PrimaryScreen.Bounds.Width)
                    fZoom = 0;
                else
                {
                    this.Width = (int)(this.Width * fZoom);
                    this.Height = (int)(this.Height * fZoom);
                }
            }

            CharMapCF.winhelper.enableRedraw(this, true);
            return fZoom;
        }
        /// <summary>
        /// return the max font size fitting in iWidth
        /// </summary>
        /// <param name="g"></param>
        /// <param name="font">font to use for measuring</param>
        /// <param name="iCellWidth">width of the cell to fit</param>
        /// <param name="iNumChars">how many chars to measure, have to fit</param>
        /// <returns></returns>
		private int getMaxFonzSize(Graphics g, Font font, int iCellWidth, int iNumChars){
			int iRet = 10;
			Font testFont; // = font.Clone();

            //start with a large font size
            int iFSize = 72; // (int)font.Size;
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

