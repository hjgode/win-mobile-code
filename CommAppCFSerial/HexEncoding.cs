using System;
using System.Text;
using System.Globalization;

namespace Utility
{
	/// <summary>
	/// Summary description for HexEncoding.
	/// </summary>
	public static class HexEncoding
	{
        //public HexEncoding()
        //{
        //}
        /// <summary>
        /// helper to get count of hex chars in a string
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns>only hex chars are counted</returns>
		public static int GetByteCount(string hexString)
		{
			int numHexChars = 0;
			char c;
			// remove all none A-F, 0-9, characters
			for (int i=0; i<hexString.Length; i++)
			{
				c = hexString[i];
				if (IsHexDigit(c))
					numHexChars++;
			}
			// if odd number of characters, discard last character
			if (numHexChars % 2 != 0)
			{
				numHexChars--;
			}
			return numHexChars / 2; // 2 characters per byte
		}
		/// <summary>
		/// Creates a byte array from the hexadecimal string. Each two characters are combined
		/// to create one byte. First two hexadecimal characters become first byte in returned array.
		/// Non-hexadecimal characters are ignored. 
		/// </summary>
		/// <param name="hexString">string to convert to byte array</param>
		/// <param name="discarded">number of characters in string ignored</param>
		/// <returns>byte array, in the same left-to-right order as the hexString</returns>
		public static byte[] GetBytes(string hexString, out int discarded)
		{
			discarded = 0;
			string newString = "";
			char c;
			// remove all none A-F, 0-9, characters
			for (int i=0; i<hexString.Length; i++)
			{
				c = hexString[i];
				if (IsHexDigit(c))
					newString += c;
				else
					discarded++;
			}
			// if odd number of characters, discard last character
			if (newString.Length % 2 != 0)
			{
				discarded++;
				newString = newString.Substring(0, newString.Length-1);
			}

			int byteLength = newString.Length / 2;
			byte[] bytes = new byte[byteLength];
			string hex;
			int j = 0;
			for (int i=0; i<bytes.Length; i++)
			{
				hex = new String(new Char[] {newString[j], newString[j+1]});
				bytes[i] = HexToByte(hex);
				j = j+2;
			}
			return bytes;
		}
        /// <summary>
        /// convert a byte array to a sequence of hex chars
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
		public static string ToString(byte[] bytes)
		{
			string hexString = "";
			for (int i=0; i<bytes.Length; i++)
			{
				hexString += bytes[i].ToString("X2");
                if (i <= bytes.Length-1 )
                    hexString+="-";
			}
			return hexString;
		}
        /// <summary>
        /// convert a mixed string with \xAB encoded hex chars back to a byte array
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static byte[] FromHexedString(string val){ 
            //replace \xHH with char of before using getbytes
            int iPos = val.IndexOf("\\x");
            while(iPos>=0){
                string s = val.Substring(iPos, 4);
                byte b = byte.Parse(val.Substring(iPos+2,2), NumberStyles.HexNumber);
                //string c = Convert.ToString(b,16);
                char ch = (char)b;
                //test
                //byte[] bTest = Encoding.ASCII.GetBytes(ch.ToString());
                //System.Diagnostics.Debug.WriteLine(string.Format("Encoded {0:x} as '{1}'", bTest,ch));
                string o = val.Substring(0, iPos) + ch + val.Substring(iPos + 4);
                val = o;
                iPos = val.IndexOf("\\x");
            }
            byte[] valAsByteArray = Encoding.ASCII.GetBytes(val);
            return valAsByteArray;
        }
        /// <summary>
        /// convert a string with chars to a string that uses \xAB encoding for non-printable chars
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToMixedString(string s)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(s);
            string sReturn = ToMixedString(bytes);
            return sReturn;
        }
        /// <summary>
        /// convert a byte array to a string that uses \xAB encoding for non-printable chars
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToMixedString(byte[] bytes)
        {
            string hexString = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] >= 0x20)
                {
                    //hexString += bytes[i].ToString();
                    hexString += ASCIIEncoding.ASCII.GetString(bytes, i, 1);
                }
                else
                {
                    hexString += "\\x"; // "<"
                    hexString += bytes[i].ToString("X2");
                    //if (i <= bytes.Length - 1) hexString += "-";
                    //hexString += ">";
                }
            }
            return hexString;
        }

        /// <summary>
		/// Determines if given string is in proper hexadecimal string format
		/// </summary>
		/// <param name="hexString"></param>
		/// <returns></returns>
		public static bool InHexFormat(string hexString)
		{
			bool hexFormat = true;

			foreach (char digit in hexString)
			{
				if (!IsHexDigit(digit))
				{
					hexFormat = false;
					break;
				}
			}
			return hexFormat;
		}

		/// <summary>
		/// Returns true is c is a hexadecimal digit (A-F, a-f, 0-9)
		/// </summary>
		/// <param name="c">Character to test</param>
		/// <returns>true if hex digit, false if not</returns>
		public static bool IsHexDigit(Char c)
		{
			int numChar;
			int numA = Convert.ToInt32('A');
			int num1 = Convert.ToInt32('0');
			c = Char.ToUpper(c);
			numChar = Convert.ToInt32(c);
			if (numChar >= numA && numChar < (numA + 6))
				return true;
			if (numChar >= num1 && numChar < (num1 + 10))
				return true;
			return false;
		}
		/// <summary>
		/// Converts 1 or 2 character string into equivalant byte value
		/// </summary>
		/// <param name="hex">1 or 2 character string</param>
		/// <returns>byte</returns>
		private static byte HexToByte(string hex)
		{
			if (hex.Length > 2 || hex.Length <= 0)
				throw new ArgumentException("hex must be 1 or 2 characters in length");
			byte newByte = byte.Parse(hex, System.Globalization.NumberStyles.HexNumber);
			return newByte;
		}
	}
}
