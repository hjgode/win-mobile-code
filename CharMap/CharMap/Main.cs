using System;
using System.Windows.Forms;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

using CharMapTool;

namespace CharMap
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			WinForm form = new WinForm ();
			Application.Run (form);
			//Console.WriteLine("Hello World!");
		}
	}
	public class WinForm : Form
	{
        DrawUniMap drawUniPanel;
		public WinForm ()
		{
			InitializeComponent ();
		}
		
		ComboBox cboUnicodePlane;
		
		ComboBox lbFont;
		Label lblFontName;
		TextBox txtUniChar;
		private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem mnuZoomIn;
        private System.Windows.Forms.MenuItem mnuZoomOut;
        private System.Windows.Forms.MenuItem mnuExit;
		
		byte bCurrCodepage=0x04;
		int iOffsetTop = 30;
				
		private void InitializeComponent ()
		{
			this.Width = 480;
			this.Height = 640;
			this.Text = "My Dialog";

			
			this.SuspendLayout();
			
			drawUniPanel = new DrawUniMap();
			drawUniPanel.Location = new Point(0, 50);
            drawUniPanel.Size = new Size(this.Width, this.Height - 50);
            this.Controls.Add(drawUniPanel);

			Button btnOK = new Button ();
			btnOK.Text = "OK";
			btnOK.Location = new System.Drawing.Point (4, 4);
			btnOK.Size = new System.Drawing.Size (80, 24);
			this.Controls.Add (btnOK);
			btnOK.Click += new EventHandler (btnOK_Click);
		
			//combo to select 'codepage'
			cboUnicodePlane = new ComboBox();
			cboUnicodePlane.DropDownStyle=ComboBoxStyle.DropDownList;
			cboUnicodePlane.Items.Clear();
			for (int i=0; i<0xff; i++){
				cboUnicodePlane.Items.Insert(i, i.ToString("x02"));	
			}
			cboUnicodePlane.SelectedIndex=0x04;
			cboUnicodePlane.Location = new Point(100, 4);
			cboUnicodePlane.Size = new System.Drawing.Size (80, 60);
			cboUnicodePlane.SelectedIndexChanged+=new EventHandler(lb_SelectedIndexChanged);
			this.Controls.Add(cboUnicodePlane);

			//combo to select font
			lbFont = new ComboBox();
			lbFont.DropDownStyle=ComboBoxStyle.DropDownList;
			lbFont.Items.Clear();
			lbFont.Items.Insert(0, "Arial");
			lbFont.Items.Insert(1, "Arial Unicode MS");
			lbFont.Items.Insert(2, "Times New Roman");
			lbFont.Items.Insert(3, "Fanatasie");
			lbFont.SelectedIndex=1;
			lbFont.Location=new Point(190,4);
			lbFont.Size=new Size(120,60);
			lbFont.SelectedIndexChanged+=new EventHandler(lbFont_SelectedIndexChanged);
			this.Controls.Add(lbFont);
		
			lblFontName = new Label();
			lblFontName.Text=lbFont.SelectedItem.ToString();
			lblFontName.Size = new Size(120, 24);
			lblFontName.Location= new Point(330,4);
			this.Controls.Add(lblFontName);

			txtUniChar = new System.Windows.Forms.TextBox();
            // 
            // txtUniChar
            // 
            this.txtUniChar.Location = new System.Drawing.Point(240, 32);
            this.txtUniChar.Name = "txtUniChar";
            this.txtUniChar.ReadOnly = true;
            this.txtUniChar.Size = new System.Drawing.Size(23, 21);
			this.Controls.Add(txtUniChar);
			
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.mnuExit = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.mnuZoomIn = new System.Windows.Forms.MenuItem();
            this.mnuZoomOut = new System.Windows.Forms.MenuItem();
            // 
            // mnuExit
            // 
            this.mnuExit.Text = "Exit";
            this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.MenuItems.Add(this.mnuZoomIn);
            this.menuItem1.MenuItems.Add(this.mnuZoomOut);
            this.menuItem1.Text = "Options";
            // 
            // mnuZoomIn
            // 
            this.mnuZoomIn.Text = "Zoom In";
            this.mnuZoomIn.Click += new System.EventHandler(this.mnuZoomIn_Click);
            // 
            // mnuZoomOut
            // 
            this.mnuZoomOut.Text = "Zoom Out";
            this.mnuZoomOut.Click += new System.EventHandler(this.mnuZoomOut_Click);
            this.Menu = this.mainMenu1;
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.mnuExit);
            this.mainMenu1.MenuItems.Add(this.menuItem1);

			this.Load += new System.EventHandler(this.Form1_Load);
			
            drawUniPanel._bCurrCodepage = (byte)cboUnicodePlane.SelectedIndex;
			
            this.FormBorderStyle = FormBorderStyle.Sizable;
            drawUniPanel.NewMessageHandler += new DrawUniMap.KlickedEventHandler(drawUni_NewMessageHandler);

            this.ResumeLayout();
        }

        void drawUni_NewMessageHandler(object sender, DrawUniMap.MessageEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Message: " + e.NewMessage);
            txtUniChar.Text = e.NewMessage;
        }
		
		private void lb_SelectedIndexChanged(object sender, EventArgs e){
			bCurrCodepage=(byte)cboUnicodePlane.SelectedIndex;
			drawUniPanel._bCurrCodepage=bCurrCodepage;
			this.Refresh();
		}
		private void lbFont_SelectedIndexChanged(object sender, EventArgs e){
			drawUniPanel._mapFont=new Font(lbFont.SelectedItem.ToString(), 30, FontStyle.Regular);
			lblFontName.Text=drawUniPanel._mapFont.Name;
		}
		private void Form_Paint (object sender, PaintEventArgs e)
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
			int iX=0, iY=0;
			int iXstep=30, iYstep=30;
			byte bHigh=bCurrCodepage;// 0x04;
			byte bLow=0x00;
			
			
			Font drawFont = new Font("Arial Unicode MS", 10);
		    SolidBrush drawBrush = new SolidBrush(Color.Black);
			for(iX=0; iX<16; iX++){
				e.Graphics.DrawString(iX.ToString("x2"), 
				        drawFont, drawBrush, 
				        new Point(iX*iXstep+iXstep, iYstep));
			}			
			for(iY=0; iY<16; iY++){
				int xx = (iY*0x10);
				e.Graphics.DrawString(xx.ToString("x2"), 
				        drawFont, drawBrush, 
				        new Point(0, iY* iYstep + iOffsetTop+iYstep));
			}
			
			for(iY=0; iY<16; iY++){
				for (iX=0; iX<16; iX++){
					Point p = new Point(iX*iXstep+iXstep, iY*iYstep + iOffsetTop + iYstep);
					byte[] b=new byte[2];
					b[0]=bLow++; b[1]=bHigh;
					
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
		    
			Font drawFont = new Font("Arial Unicode MS", 16);
		    SolidBrush drawBrush = new SolidBrush(Color.Black);

			// Draw string to screen.
		    e.Graphics.DrawString(uStr, drawFont, drawBrush, drawPoint);
		}			
		
		private void btnOK_Click (object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			this.Close ();
		}
        private void mnuExit_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        private void mnuZoomIn_Click(object sender, EventArgs e)
        {
            if (drawUniPanel.Width > 2 * Screen.PrimaryScreen.Bounds.Width)
                return;
            drawUniPanel.Width = (int)(drawUniPanel.Width * 1.2);
            drawUniPanel.Height = (int)(drawUniPanel.Height * 1.2);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txtUniChar.Font = new Font("Arial Unicode MS", 9, FontStyle.Bold);
        }

        private void mnuZoomOut_Click(object sender, EventArgs e)
        {
            if (drawUniPanel.Width < 120)
                return;
            drawUniPanel.Width = (int)(drawUniPanel.Width * 0.8);
            drawUniPanel.Height = (int)(drawUniPanel.Height * 0.8);

        }
		
	}
}

