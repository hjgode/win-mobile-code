using System;
using System.Collections.Generic;
using System.Text;

using System.Collections;
using System.IO;
using System.Drawing;
/*
USAGE:
			if(System.IO.File.Exists("RM-Logo.bmp")){
				BitImage.BitimageClass.dumpBands("RM-Logo.bmp");
				bitmap = BitImage.BitimageClass.getRawBitmap("RM-Logo.bmp");
			}
			else
				MessageBox.Show("Missing file: " + "RM-Logo.bmp");
...
		private System.Drawing.Bitmap bitmap = null;
		private void Form1_Paint(object sender, PaintEventArgs e){
			//e.Graphics.Dr
			if(bitmap!=null){
				e.Graphics.DrawImage(bitmap,new System.Drawing.Point(0,100));
			}
		}
*/
/* Different bitimage graphics mode
 * this is actually using 24 or 8 byte bands
 * but some ESCP manuals say ESC V n1 n2 is for line image mode:
 * PB50 pixelwidth=832pixels, 104 bytes.
 * ESC V n1 n2 - number of lines with graphics is n1*256+n2 (just as it 
 * says in the ESC/P manual).
 * 
 * so the code has to be reordered to provide line image mode
 * where one line of pixels has to be printed after the other
*/


namespace BitImage
{
    public class BitmapDotData
    {
        public BitArray Dots
        {
            get;
            set;
        }

        public int Height
        {
            get;
            set;
        }

        public int Width
        {
            get;
            set;
        }
    }

    class BitimageClass
    {
		private static int bytesPerRow=3;
		public static int _bytesPerRow{
			get{ return bytesPerRow;}
			set{ bytesPerRow=value;}
		}
		private static int numPixelRows=0;
		public static int _numPixelRows{
			get{ return numPixelRows;}
		}
		/// <summary>
		/// get the B&W 1BPP bitmap for a bmp file 
		/// </summary>
		/// <param name="bmpFileName">
		/// </param>
		/// <returns>
		/// </returns>
        private static BitmapDotData GetBitmapData(string bmpFileName)
        {
            Image img = new Bitmap(bmpFileName); 
            using (var bitmap = new Bitmap(img))// (Bitmap)Bitmap.FromFile(bmpFileName))
            {
                var threshold = 127;
                var index = 0;
                var dimensions = bitmap.Width * bitmap.Height;
                var dots = new BitArray(dimensions);

                for (var y = 0; y < bitmap.Height; y++)
                {
                    for (var x = 0; x < bitmap.Width; x++)
                    {
                        var color = bitmap.GetPixel(x, y);
                        var luminance = (int)(color.R * 0.3 + color.G * 0.59 + color.B * 0.11);
                        dots[index] = (luminance < threshold);
                        index++;
                    }
                }

                return new BitmapDotData()
                    {
                        Dots = dots,
                        Height = bitmap.Height,
                        Width = bitmap.Width
                    };
            }
        }
        public static byte[] GetDocument(string sBitmapFile)
        {
            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                // Reset the printer bws (NV images are not cleared)
/*                bw.Write(ASCIIhelper.AsciiControlChars.Escape);
                bw.Write('@');
*/
                // Render the logo
                RenderBands(sBitmapFile, bw);

                // Feed 3 vertical motion units and cut the paper with a 1 point cut
/*                bw.Write(ASCIIhelper.AsciiControlChars.GroupSeparator);
                bw.Write('V');
                bw.Write((byte)66);
                bw.Write((byte)3);
*/
                bw.Flush();

                return ms.ToArray();
            }
        }

        private static void RenderBands(string bitmapFile, BinaryWriter bw)
        {
            var data = GetBitmapData(bitmapFile);
            //var data = GetBitmapData("RM-Logo.bmp");
            var dots = data.Dots;
            var width = BitConverter.GetBytes(data.Width);

            //write header
/*            bw.Write(ASCIIhelper.AsciiControlChars.Escape);
            bw.Write('*');         // bit-image mode
            bw.Write((byte)0);     // 8-dot single-density
            bw.Write((byte)5);     // width low byte
            bw.Write((byte)0);     // width high byte
            bw.Write((byte)1);
            bw.Write((byte)2);
            bw.Write((byte)4);
            bw.Write((byte)8);
            bw.Write((byte)16);
*/
            // So we have our bitmap data sitting in a bit array called "dots."
            // This is one long array of 1s (black) and 0s (white) pixels arranged
            // as if we had scanned the bitmap from top to bottom, left to right.
            // The printer wants to see these arranged in bytes stacked three high.
            // So, essentially, we need to read 24 bits for x = 0, generate those
            // bytes, and send them to the printer, then keep increasing x. If our
            // image is more than 24 dots high, we have to send a second bit image
            // command.

            // Set the line spacing to 24 dots, the height of each "stripe" of the
            // image that we're drawing.
/*            bw.Write(ASCIIhelper.AsciiControlChars.Escape);
            bw.Write('3');
            bw.Write((byte)24);
*/
            // OK. So, starting from x = 0, read 24 bits down and send that data
            // to the printer.
            int offset = 0;
			
            while (offset < data.Height)
            {
/*                bw.Write(ASCIIhelper.AsciiControlChars.Escape);
                bw.Write('*');         // bit-image mode
                bw.Write((byte)33);    // 24-dot double-density
                bw.Write(width[0]);  // width low byte
                bw.Write(width[1]);  // width high byte
*/
                for (int x = 0; x < data.Width; ++x)
                {
                    for (int k = 0; k < bytesPerRow; ++k)
                    {
                        byte slice = 0;

                        /*
                        We have our working copy of a byte that we’re munging with in a variable
                         called ‘slice’. I have a Boolean named ‘v’ that tells me if I want to 
                         shove a 1 or a zero into this byte. So if v is true, I take the
                         integer 1 and shift it (7 – b) units to the left. (I think in Delphi this
                          is ‘Shl’?) If it’s a zero, well, I don’t have to do anything actually 
                          but there is not harm in “shifting zero” in a one-liner. Then I 
                          bitwise OR my shifted integer with the ‘slice’.

                        So if ‘slice’ looked like ’01000000′ and I’m working on b = 3, 
                        then I take ’00000001′, shift it to the left (7 – 3) == 4 places 
                        to get ’00010000′, and then I OR that number (’00010000′) with 
                        the slice (’01000000′) to get ’01010000′. And then I keep on 
                        trucking. (If it was zero, I would have shifted zero and then OR it 
                        with the slice, effectively doing nothing.)
                        */
                        for (int b = 0; b < 8; ++b)
                        {
                            int y = (((offset / 8) + k) * 8) + b;

                            // Calculate the location of the pixel we want in the bit array.
                            // It'll be at (y * width) + x.
                            int i = (y * data.Width) + x;

                            // If the image is shorter than 24 dots, pad with zero.
                            bool v = false;
                            if (i < dots.Length)
                            {
                                v = dots[i];
                            }

                            slice |= (byte)((v ? 1 : 0) << (7 - b));
                        }

                        bw.Write(slice);
                    }
                }

                offset += bytesPerRow * 8;// 24;
/*                bw.Write(ASCIIhelper.AsciiControlChars.Newline);
*/            }

            // Restore the line spacing to the default of 30 dots.
/*            bw.Write(ASCIIhelper.AsciiControlChars.Escape);
            bw.Write('3');
            bw.Write((byte)30);
*/        }
		public static Image resizeImage(Image imgToResize, Size size)
		{
		   int sourceWidth = imgToResize.Width;
		   int sourceHeight = imgToResize.Height;
		
		   float nPercent = 0;
		   float nPercentW = 0;
		   float nPercentH = 0;
		
		   nPercentW = ((float)size.Width / (float)sourceWidth);
		   nPercentH = ((float)size.Height / (float)sourceHeight);
		
		   if (nPercentH < nPercentW)
		      nPercent = nPercentH;
		   else
		      nPercent = nPercentW;
		
		   int destWidth = (int)(sourceWidth * nPercent);
		   int destHeight = (int)(sourceHeight * nPercent);
		
		   Bitmap b = new Bitmap(destWidth, destHeight);
		   Graphics g = Graphics.FromImage((Image)b);
			g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
		   //g.InterpolationMode = InterpolationMode.HighQualityBicubic;
		
		   g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
		   g.Dispose();
		
		   return (Image)b;
		}		
	        public static void dumpBands(string sBitmapFile, byte[] bytes)
        {
            byte[] bs = GetDocument("RM-Logo.bmp");
            Console.WriteLine(tools.tools.ByteAr2HexStr(bs));
            Console.WriteLine(tools.tools.ByteAr2BinStr(bs));
        }
        public static void dumpBands(string sBitmapFile)
        {
            byte[] bs = GetDocument(sBitmapFile);//"RM-Logo.bmp");
//            Console.WriteLine(tools.tools.ByteAr2HexStr(bs));
            Console.WriteLine(tools.tools.ByteAr2BinStr(bs));
        }
		public static Bitmap getRawBitmap(string sBitmapFile){
			//byte[] bs = GetDocument(sBitmapFile);//"RM-Logo.bmp");
			BitmapDotData data = GetBitmapData(sBitmapFile);
			Bitmap bmp = new Bitmap(data.Width,data.Height,System.Drawing.Imaging.PixelFormat.Format1bppIndexed);
			int width = bmp.Width;
			int height = bmp.Height;
			System.Drawing.Imaging.BitmapData bits = 
				bmp.LockBits(new Rectangle(0,0,bmp.Width,bmp.Height),
			    System.Drawing.Imaging.ImageLockMode.ReadWrite, 
			    System.Drawing.Imaging.PixelFormat.Format1bppIndexed);
			numPixelRows=0;
			
	        unsafe
	        {
/*
 http://www.bobpowell.net/lockingbits.htm
 Format1BppIndexed Given the X and Y coordinates, the byte containing the pixel is calculated
 by Scan0+(y*Stride)+(x/8). The byte contains 8 bits, each bit is one pixel with the leftmost
 pixel in bit 8 and the rightmost pixel in bit 0. The bits select from the two entry colour
 palette.
*/
				for (int y = 0; y < height; y++)
	            {
					numPixelRows++;
	                //int* row = (int*)((byte*)bits.Scan0 + (y * bits.Stride));
					for (int x = 0; x < width; x++)
	                {
						//int* row = (int*)((byte*)bits.Scan0 + (y * bits.Stride)+(x/8));
						byte* p=(byte*)bits.Scan0.ToPointer();
						int index=y*bits.Stride+(x>>3);
						byte mask=(byte)(0x80>>(x&0x7));
						bool pixel = !(data.Dots[y * width + x]);
						if(pixel)
							p[index]|=mask;
						else
							p[index]&=(byte)(mask^0xff); 
					
	                    //row[x] = data.Dots[y * width + x];
	                }
	            }
	        }
	        bmp.UnlockBits(bits);
		return bmp;
		}
    }
}
