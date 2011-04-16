using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
//using System.Windows.Forms;

namespace tools
{
    public class tools
    {
		public static string ByteAr2HexStr(byte[] b){
			string s="";
			foreach (byte b1 in b){
				s+=b1.ToString("x");
			}
			return s;
		}
		static int iByteGroup=3;
		static int iByteCount=1;
		public static string ByteAr2BinStr(byte[] b){
			string s="";
			
			foreach(byte b1 in b){
				iByteCount++;
				s += Byte2DotStr(b1);
				//s += Byte2BinStr(b1);
				if(iByteCount>iByteGroup){
					s += "\r\n";
					iByteCount=1;
				}
			}
			
			return s;
		}
        public static string Byte2BinStr(byte b)
        {
            int maxbits = 8;
            byte b8 = b;
            int i;
            string s="";
            //s[0] = bit[0]
            //start at highest value and test
            for (i = maxbits; i > 0; i--)
            {
                if (b8 >= Math.Pow(2, i-1))
                {
                    s = "1" + s;
                    b8 -= (byte)Math.Pow(2, i-1);
                }
                else
                    s = "0" + s;
            }
            s.PadRight(8, '0');
            return s;
        }
        public static string Byte2DotStr(byte b)
        {
            int maxbits = 8;
            byte b8 = b;
            int i;
            string s="";
            //s[0] = bit[0]
            //start at highest value and test
            for (i = maxbits; i > 0; i--)
            {
                if (b8 >= Math.Pow(2, i-1))
                {
                    s = "." + s;
                    b8 -= (byte)Math.Pow(2, i-1);
                }
                else
                    s = " " + s;
            }
            s.PadRight(8, ' ');
            return s;
        }
        public static string ReverseString(string s)
        {
            int i = 0;
            string str="";
            for (i = s.Length-1; i >= 0; i--)
            {
                str = str + s.Substring(i, 1);
            }
            return str;
        }

        /// <summary>
        /// Copy a string until a \r appears
        /// </summary>
        /// <param name="s">input string</param>
        /// <returns>string cutdown to \r</returns>
        public static string CopyToR(string s)
        {
            int i = 0;
            string t = "";
            while (s.Substring(i, 1) != "\r")
            {
                t += s.Substring(i, 1);
                i++;
            }
            return t;
        }
        /// <summary>
        /// C# to convert a string to a byte array.
        /// </summary>
        /// <param name="str"></param>
        /// <returns>sbyte[]</returns>
        public static sbyte[] StrToSByteArray(string str)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] src = new byte[str.Length];
            sbyte[] dest = new sbyte[src.Length];
            src = encoding.GetBytes(str);
            System.Buffer.BlockCopy(src, 0, dest, 0, src.Length);
            return dest;
        }


#region NewStuff
        //===============================================================================================
        // New stuff
        //===============================================================================================

        /// <summary>
        /// takes a string of 0s and 1s and returns a uint
        /// </summary>
        /// <param name="s">a 'binary' string of 0s and 1s</param>
        /// <returns></returns>
        public static UInt32 BinStr2Uint(string s)
        {
            int maxbits = s.Length;
            int i;
            UInt32 u=0;
            //s[s.lenghth] = Bit[0]
            //start at end of string, highest bit
            for (i = 0; i < maxbits ; i++)
            {
                if (s.Substring( i, 1).Equals("1"))
                {
                    u += (uint)Math.Pow(2, i);
                }
            }
            return u;
        }

        /// <summary>
        /// convert an uint to a BinStr
        /// </summary>
        /// <param name="u"></param>
        /// <returns>a BinStr of 0s and 1s, lowest bit left</returns>
        public static string Uint2BinStr(UInt32 u)
        {
            int maxbits = 32;
            UInt32 u32 = u;
            int i;
            string s="";
            //s[0] = bit[0]
            //start at highest value and test
            for (i = maxbits; i > 0; i--)
            {
                if (u32 >= Math.Pow(2, i-1))
                {
                    s = "1" + s;
                    u32 -= (UInt32)Math.Pow(2, i-1);
                }
                else
                    s = "0" + s;
            }
            s.PadRight(32, '0');
            return s;
        }
        
        /// <summary>
        /// get binary string in reverse order
        /// </summary>
        /// <param name="u"></param>
        /// <param name="bitcount"></param>
        /// <returns></returns>
        public static string Uint2BinStr(UInt32 u, int bitcount)
        {
            //49977473 -> //10111110101001100010000001
            //123456789 -> 00000111010110111100110100010101
            //             "00000111 01011011 11001101 00010101
            //             "
            string s = Uint2BinStr(u);
            if (s.Length > bitcount)
            {
                s = s.Substring(0, bitcount);
            }
            else if (s.Length < bitcount)
                s = s.PadRight(bitcount, '0');
            return s;
        }

        /// <summary>
        /// Convert a BinStr ie '01010011' to a hex string '53'hex
        /// </summary>
        /// <param name="BinStr"></param>
        /// <returns>a hex str</returns>
        public static string BinStr2HexStr(string BinStr)
        {
            //convert byte by byte
            string sHex2="";
            string sBin8="";
            string sHex="";
            string sBin = BinStr;
            byte b = 0;
            //ensure a binstr dividable by 8
            if (sBin.Length % 8 > 0) //is there any rest?
            {
                int l = sBin.Length;
                int c = l / 8;
                c++;
                c = c * 8;
                l = c - l;
                for (int i = 0; i < l; i++)
                    sBin = sBin + "0";
            }
            //start at lowest bit
            while (sBin.Length > 0)
            {
                sBin8 = sBin.Substring(0, 8); //01010011
                b = (byte)BinStr2Uint(sBin);
                sHex2 = Convert.ToString(b, 16); //0x53
                sHex = sHex2.PadLeft(2,'0') + sHex ;
                sBin = sBin.Substring(8);
            }
            return sHex;
        }
        /// <summary>
        /// convert a BinStr to a Hex string
        /// </summary>
        /// <param name="BinStr"></param>
        /// <param name="width">the Byte width, ie 12 means you get a string with 24 letters
        /// each byte is translated to a two-letter hex str</param>
        /// <returns></returns>
        public static string BinStr2HexStr(string BinStr, int ByteWidth)
        {
            string HexStr = BinStr2HexStr(BinStr);
            if (HexStr.Length < ByteWidth) //pad left with '0'
                HexStr = HexStr.PadLeft(ByteWidth*2, '0');
            else if (HexStr.Length > ByteWidth) //return right part, cut left
                HexStr = HexStr.Substring(HexStr.Length - (ByteWidth * 2), ByteWidth * 2);
            return HexStr;
        }

        public static char[] HexStr2CharAr(string sHex)
        {
            char[] c;
            string sBin = "";
            sBin = HexStr2BinStr(sHex);
            c=BinStr2CharAr(sBin);
            return c;
        }

        public static string CharAr2HexStr(char[] c)
        {
            string s = "";
            string sBin = "";
            sBin = CharAr2BinStr(c);
            s = BinStr2HexStr(sBin);
            return s;
        }

        public static string HexStr2BinStr(string hex)
        {
            byte b;
            string BinStr = "";
            string sBin8 = "";
            string hex2 = "";
            if (hex.Length == 24)
            {
                //from left to right //34 .......
                for (int i = 0; i < 12; i++)
                {                                                //0-1-2-3-4- 5- 6- 7- 8- 9-10-11
                    hex2 = hex.Substring(i*2, 2);//start at right, 0-2-4-6-8-10-12-14-16-18-20-22
                    b = Convert.ToByte(hex2, 16);
                    sBin8 = Uint2BinStr(b).Substring(0,8);
                    BinStr = sBin8 + BinStr ;
                }
            }
			else{
				//is this a even number of chars?
				if(hex.Length % 2 > 0)
					hex="0"+hex; //add a zero in front
                //from left to right //34 .......
                for (int i = 0; i < hex.Length/2; i++)
                {                                                //0-1-2-3-4- 5- 6- 7- 8- 9-10-11
                    hex2 = hex.Substring(i*2, 2);//start at right, 0-2-4-6-8-10-12-14-16-18-20-22
                    b = Convert.ToByte(hex2, 16);
                    sBin8 = Uint2BinStr(b).Substring(0,8);
                    BinStr = sBin8 + BinStr ;
                }
				
			}
            if (!BinStr2HexStr(BinStr).Equals(hex.ToLower()))
                System.Diagnostics.Debugger.Break();
            return BinStr;
        }

        /// <summary>
        /// convert a BinStr to a char[] array
        /// </summary>
        /// <param name="BinStr"></param>
        /// <returns></returns>
        public static char[] BinStr2CharAr(string BinStr)
        {
            int bitcount = BinStr.Length;
            if (BinStr.Length > 96)
                BinStr = BinStr.Substring(0,96);//limit bitcount to 96
            else if (BinStr.Length < 96)
                BinStr = BinStr.PadRight(96, '0');

            char[] c = new char[96];
            for (int i = 0; i < 96; i++)
            {
                if (BinStr.Substring(i, 1).Equals("1"))
                    c[i] = '1';
                else
                    c[i] = '0';
            }
            return c;
        }
        /// <summary>
        /// convert a char[] to BinStr (substr(0,1)=char[0]
        /// BinStr highest Bit at RIGHT!
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string CharAr2BinStr(char[] c)
        {
            int bitcount = c.Length;
            string s = "";
            for (int i = 0; i < bitcount; i++)
            {
                if (c[i] == '1')
                    s = s + "1";
                else
                    s = s + "0";
            }
            if (s.Length < 96)
                s.PadRight(96, '0');
            else if (s.Length > 96)
                s = s.Substring(0, 96);
            return s;
        }
#endregion
    }
}