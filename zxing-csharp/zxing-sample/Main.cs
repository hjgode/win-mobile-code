using System;
using System.Windows.Forms;

using com.google.zxing;
using com.google.zxing.common;

namespace zxingsample
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

		Button btn_File;
		TextBox tb;
		private void InitializeComponent ()
		{
			this.Width = 400;
			this.Height = 300;
			this.Text = "My Dialog";
			Button btnOK = new Button ();
			btnOK.Text = "OK";
			btnOK.Location = new System.Drawing.Point (10, 10);
			btnOK.Size = new System.Drawing.Size (80, 24);
			this.Controls.Add (btnOK);
			btnOK.Click += new EventHandler (btnOK_Click);
			
			btn_File=new Button();
			btn_File.Text="File...";
			btn_File.Location=new System.Drawing.Point(100,10);
			btn_File.Size=btnOK.Size;
			this.Controls.Add(btn_File);
			btn_File.Click+=new EventHandler(btn_File_Click);
			
			tb=new TextBox();
			tb.Location=new System.Drawing.Point(10,50);
			tb.Size=new System.Drawing.Size(200, 24);
			tb.Text="";
			this.Controls.Add(tb);
		}
		private void btn_File_Click (object sender, System.EventArgs e)
		{
			string sRes="";
			OpenFileDialog ofn=new OpenFileDialog();
			if(ofn.ShowDialog()==DialogResult.OK){
				try {
							Reader barcodeReader= 
								new com.google.zxing.MultiFormatReader();
							System.Drawing.Bitmap srcbitmap = 
								new System.Drawing.Bitmap(ofn.FileName.ToString()); // Make a copy of the image in the Bitmap variable 
							//make a grey bitmap of it
							bmp_util util=new bmp_util();
							srcbitmap = util.ConvertToGrayscale(srcbitmap);
					
							RGBLuminanceSource source = 
								new RGBLuminanceSource(srcbitmap, srcbitmap.Width, srcbitmap.Height);
							com.google.zxing.BinaryBitmap bitmap= 
								new com.google.zxing.BinaryBitmap(new HybridBinarizer(source));
							com.google.zxing.Result result = barcodeReader.decode(bitmap);
							System.Console.WriteLine(result.Text);
					sRes=result.Text;
				} 
				catch (com.google.zxing.ReaderException zex) {
					sRes=zex.Message;
					System.Console.WriteLine(zex.Message);	
				}
				catch (Exception ex) {
					sRes=ex.Message;
					System.Console.WriteLine(ex.Message);	
				}
				tb.Text=sRes;
			}
		}
	private void btnOK_Click (object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			this.Close ();
		}
		
	}
}

