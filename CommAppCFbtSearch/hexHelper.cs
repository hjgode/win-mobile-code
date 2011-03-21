using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;


public static class hexHelper
{
    public static byte[] reverseBytes(byte[] inB)
    {
        byte[] bOut = new byte[inB.Length];
        int iLast = inB.Length - 1;
        for (int i = 0; i < inB.Length; i++)
        {
            bOut[i] = inB[iLast - i];
        }
        return bOut;
    }
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
    public static byte[] GetBytes(string hexString, out int discarded)
    {
        discarded = 0;
        string newString = "";
        char c;
        // remove all none A-F, 0-9, characters
        for (int i = 0; i < hexString.Length; i++)
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
            newString = newString.Substring(0, newString.Length - 1);
        }

        int byteLength = newString.Length / 2;
        byte[] bytes = new byte[byteLength];
        string hex;
        int j = 0;
        for (int i = 0; i < bytes.Length; i++)
        {
            hex = new String(new Char[] { newString[j], newString[j + 1] });
            bytes[i] = HexToByte(hex);
            j = j + 2;
        }
        return bytes;
    }
    private static byte HexToByte(string hex)
    {
        if (hex.Length > 2 || hex.Length <= 0)
            throw new ArgumentException("hex must be 1 or 2 characters in length");
        byte newByte = byte.Parse(hex, System.Globalization.NumberStyles.HexNumber);
        return newByte;
    }
    public static string ToString(byte[] bytes)
    {
        string hexString = "";
        for (int i = 0; i < bytes.Length; i++)
        {
            hexString += bytes[i].ToString("X2");
            //if (i <= bytes.Length - 1)
            //    hexString += "-";
        }
        return hexString;
    }

}

