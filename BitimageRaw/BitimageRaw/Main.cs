using System;
using System.Windows.Forms;

namespace BitimageRaw
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
		public WinForm ()
		{
			InitializeComponent ();
		}

		private void InitializeComponent ()
		{
			this.Width = 400;
			this.Height = 300;
			this.Text = "My Dialog";
			
			this.Paint+=new PaintEventHandler(Form1_Paint);
			Button btnOK = new Button ();
			btnOK.Text = "OK";
			btnOK.Location = new System.Drawing.Point (10, 10);
			btnOK.Size = new System.Drawing.Size (80, 24);
			this.Controls.Add (btnOK);
			btnOK.Click += new EventHandler (btnOK_Click);
			
			if(System.IO.File.Exists("RM-Logo.bmp")){
				
				BitImage.BitimageClass._bytesPerRow=1;
				BitImage.BitimageClass.dumpBands("RM-Logo.bmp");
				bitmap = BitImage.BitimageClass.getRawBitmap("RM-Logo.bmp");
				
				//resize for paint
				//calc aspect ratio
				decimal dRatio = bitmap.Width / bitmap.Height;
				//new width is width of printhead, ie 203
				int iWidth = 203; //use thermal printer print width
				int iHeight=(int) (iWidth * dRatio / bitmap.Height);
				System.Drawing.Bitmap bitmap2 = 
					(System.Drawing.Bitmap) BitImage.BitimageClass.resizeImage(bitmap, 
						new System.Drawing.Size(iWidth, iHeight));
				bitmap = bitmap2;
				
				Console.WriteLine("bytesPerRow: " +BitImage.BitimageClass._bytesPerRow);
				Console.WriteLine("numBands: " +
				    BitImage.BitimageClass._numPixelRows/BitImage.BitimageClass._bytesPerRow);
			}
			else
				MessageBox.Show("Missing file: " + "RM-Logo.bmp");
		}
		private System.Drawing.Bitmap bitmap = null;
		private void Form1_Paint(object sender, PaintEventArgs e){
			//e.Graphics.Dr
			if(bitmap!=null){
				e.Graphics.DrawImage(bitmap,new System.Drawing.Point(0,100));
			}
		}
		
		private void btnOK_Click (object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			this.Close ();
		}
		
	}
}

