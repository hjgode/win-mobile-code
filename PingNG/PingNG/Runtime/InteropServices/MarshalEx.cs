//==========================================================================================
//
//		OpenNETCF.Runtime.InteropServices.MarshalEx
//		Copyright (c) 2003-2005, OpenNETCF.org
//
//		This library is free software; you can redistribute it and/or modify it under 
//		the terms of the OpenNETCF.org Shared Source License.
//
//		This library is distributed in the hope that it will be useful, but 
//		WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or 
//		FITNESS FOR A PARTICULAR PURPOSE. See the OpenNETCF.org Shared Source License 
//		for more details.
//
//		You should have received a copy of the OpenNETCF.org Shared Source License 
//		along with this library; if not, email licensing@opennetcf.org to request a copy.
//
//		If you wish to contact the OpenNETCF Advisory Board to discuss licensing, please 
//		email licensing@opennetcf.org.
//
//		For general enquiries, email enquiries@opennetcf.org or visit our website at:
//		http://www.opennetcf.org
//
//==========================================================================================
using System;
using System.Text;
using System.Runtime.InteropServices;

namespace OpenNETCF.Runtime.InteropServices 
{
	/// <summary>
	/// Provides a collection of methods for allocating unmanaged memory, copying unmanaged memory blocks, and converting managed to unmanaged types, as well as other miscellaneous methods used when interacting with unmanaged code. 
	/// </summary>
	public sealed class MarshalEx 
	{	
		//don't allow constructor to be called - all static methods
		private MarshalEx(){}

		#region Fields

		private const int GMEM_FIXED = 0x0000;
		private const int LMEM_MOVEABLE = 2;
		private const int LMEM_ZEROINIT  = 0x0040;
		private const int LPTR = (GMEM_FIXED | LMEM_ZEROINIT);
		private static readonly int HIWORDMASK = -65536;//new IntPtr((long)-65536); 

		/// <summary>
		/// Represents the default character size on the system; the default is 2 for Unicode systems and 1 for ANSI systems. This field is read-only.
		/// </summary>
		public static readonly int SystemDefaultCharSize = Marshal.SystemDefaultCharSize;

		#endregion

		#region Read functions

		#region IntPtr
		/// <summary>
		/// Reads an IntPtr from an unmanaged pointer.   
		/// </summary>
		/// <param name="ptr">The address in unmanaged memory from which to read. </param>
		/// <param name="ofs">The offset from the ptr where the IntPtr is located.</param>
		/// <returns>The IntPtr read from the ptr parameter. </returns>
		public static IntPtr ReadIntPtr(IntPtr ptr, int ofs)
		{
			int i = Marshal.ReadInt32(ptr,ofs);
			return new IntPtr(i);
		}
		#endregion

		#region Int32
		/// <summary>
		/// Reads an Int32 object from an unmanaged pointer.   
		/// </summary>
		/// <param name="ptr">The address in unmanaged memory from which to read. </param>
		/// <param name="ofs">The offset from the ptr where the Int32 is located.</param>
		/// <returns>The Int32 read from the ptr parameter. </returns>
		public static Int32 ReadInt32(IntPtr ptr, int ofs)
		{
			return Marshal.ReadInt32(ptr, ofs);
			/*byte[] data = new byte[4];
			Marshal.Copy(new IntPtr(ptr.ToInt32() + ofs), data, 0, 4);

			return BitConverter.ToInt32(data,0);*/
		}

		/// <summary>
		/// Reads a 32-bit unsigned integer from unmanaged memory.
		/// </summary>
		/// <param name="ptr">The base address in unmanaged memory from which to read.</param>
		/// <param name="ofs">An additional byte offset, added to the ptr parameter before reading.</param>
		/// <returns>The 32-bit unsigned integer read from the ptr parameter.</returns>
		[CLSCompliant(false)]
		public static UInt32 ReadUInt32(IntPtr ptr, int ofs)
		{
			byte[] data = new byte[4];
			Marshal.Copy(new IntPtr(ptr.ToInt32() + ofs), data, 0, 4);

			return BitConverter.ToUInt32(data,0);
		}
		#endregion

		#region Int16
		/// <summary>
		/// Reads a Int16 from an unmanaged pointer.   
		/// </summary>
		/// <param name="ptr">The address in unmanaged memory from which to read. </param>
		/// <param name="ofs">The offset from the ptr where the Int16 is located.</param>
		/// <returns>The Int16 read from the ptr parameter. </returns>
		public static Int16 ReadInt16(IntPtr ptr, int ofs)
		{
			return Marshal.ReadInt16(ptr, ofs);

			/*byte[] data = new byte[2];
			Marshal.Copy(new IntPtr(ptr.ToInt32() + ofs), data, 0, 2);

			return BitConverter.ToInt16(data,0);*/
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ptr"></param>
		/// <param name="ofs"></param>
		/// <returns></returns>
		[CLSCompliant(false)]
		public static UInt16 ReadUInt16(IntPtr ptr, int ofs)
		{
			byte[] data = new byte[2];
			Marshal.Copy(new IntPtr(ptr.ToInt32() + ofs), data, 0, 2);

			return BitConverter.ToUInt16(data,0);
		}
		#endregion

		#region String
		/// <summary>
		/// Reads a string from an unmanaged pointer.   
		/// </summary>
		/// <param name="ptr">The address in unmanaged memory from which to read. </param>
		/// <param name="ofs">The offset from the ptr where the string is located.</param>
		/// <param name="len">Length in characters.</param>
		/// <returns>The string read from the ptr parameter. </returns>
		public static string PtrToStringUni(IntPtr ptr, int ofs, int len)
		{
			int cb = len * Marshal.SystemDefaultCharSize;
			byte[] data = new byte[cb];
			Marshal.Copy(new IntPtr(ptr.ToInt32() + ofs), data, 0, cb);

			string s = Encoding.Unicode.GetString(data, 0, len);
			
			int nullpos = s.IndexOf('\0');
			if(nullpos > -1)
				s = s.Substring(0,nullpos);

			return s;
		}

		/// <summary>
		/// Allocates a managed System.String, copies a specified number of characters from an unmanaged ANSI string into it, and widens each ANSI character to Unicode.
		/// </summary>
		/// <param name="ptr">The address of the first character of the unmanaged string.</param>
		/// <param name="ofs"></param>
		/// <param name="len">The byte count of the input string to copy.</param>
		/// <returns>A managed System.String that holds a copy of the native ANSI string.</returns>
		public static string PtrToStringAnsi(IntPtr ptr, int ofs, int len)
		{
			int cb = len;
			byte[] data = new byte[cb];
			Marshal.Copy(new IntPtr(ptr.ToInt32() + ofs), data, 0, cb);

			string s = Encoding.ASCII.GetString(data, 0, len);
			
			int nullpos = s.IndexOf('\0');
			if(nullpos > -1)
				s = s.Substring(0,nullpos);

			return s;
		}

		/// <summary>
		/// Copies all characters up to the first null from an unmanaged ANSI string to a managed System.String. Widens each ANSI character to Unicode.
		/// <para><b>New in v1.1</b></para>
		/// </summary>
		/// <param name="ptr">The address of the first character of the unmanaged string.</param>
		/// <returns>A managed <see cref="T:System.String"/> object that holds a copy of the unmanaged ANSI string.</returns>
		public static string PtrToStringAnsi(IntPtr ptr)
		{
			string returnval = "";
			byte thisbyte = 0;
			int offset = 0;

			//while more chars to read
			while(true)
			{
				//read current byte
				thisbyte = Marshal.ReadByte(ptr, offset);

				//if not null
				if(thisbyte == 0)
				{
					break;
				}

				//add the character
				returnval += ((char)thisbyte).ToString();
				
				//move to next position
				offset++;
			}

			return returnval;
		}

		/// <summary>
		/// Allocates a managed System.String and copies all characters up to the first null character from an unmanaged Unicode string into it.
		/// </summary>
		/// <param name="ptr">The address of the first character of the unmanaged string.</param>
		/// <returns>A managed string holding a copy of the native string.</returns>
		public static string PtrToStringUni(IntPtr ptr)
		{
			return Marshal.PtrToStringUni(ptr);
		}

		/// <summary>
		/// Allocates a managed <see cref="T:System.String"/> and copies all characters up to the first null character from a string stored in unmanaged memory into it.
		/// <para><b>New in v1.1</b></para>
		/// </summary>
		/// <param name="ptr">The address of the first character.</param>
		/// <returns>A managed string that holds a copy of the unmanaged string.</returns>
		public static string PtrToStringAuto(IntPtr ptr)
		{
			if(ptr==IntPtr.Zero)
			{
				return null;
			}
			else
			{
				//final string value
				string returnval = "";

				//read first byte
				int firstbyte = Marshal.ReadByte(ptr, 0);
				//read second byte
				int secondbyte = Marshal.ReadByte(ptr, 1);

				//if first byte is non-zero continue
				if(firstbyte!=0)
				{
				
					//if second byte is zero we may have unicode or one byte string
					if(secondbyte==0)
					{
						//read third byte
						int thirdbyte = Marshal.ReadByte(ptr, 2);

						//if third byte is null this is a single byte string
						if(thirdbyte==0)
						{
							//single ascii char
							returnval = ((char)firstbyte).ToString();
						}
						else
						{
							//read unicode
							return Marshal.PtrToStringUni(ptr);
						}
					} 
					else
					{
						//else appears to be ASCII
						return PtrToStringAnsi(ptr);
					}
				}

				return returnval;
			}
		}
		#endregion

		#region Char
		/// <summary>
		/// Reads a single char from an unmanaged pointer.   
		/// </summary>
		/// <param name="ptr">The address in unmanaged memory from which to read. </param>
		/// <param name="ofs">The offset from the ptr where the char is located.</param>
		/// <returns>The char read from the ptr parameter. </returns>
		public static char ReadChar(IntPtr ptr, int ofs)
		{
			byte[] data = new byte[Marshal.SystemDefaultCharSize];
			Marshal.Copy(new IntPtr(ptr.ToInt32() + ofs),data, 0, data.Length);

			return BitConverter.ToChar(data,0);
		}
		#endregion

		#region Byte[]
		/// <summary>
		/// Reads a byte array from an unmanaged pointer.   
		/// </summary>
		/// <param name="ptr">The address in unmanaged memory from which to read. </param>
		/// <param name="ofs">The offset from the ptr where the byte array is located.</param>
		/// <returns>The byte array read from the ptr parameter. </returns>
		public static byte[] ReadByteArray(IntPtr ptr, int ofs, int len)
		{
			byte[] data = new byte[len];
			Marshal.Copy(new IntPtr(ptr.ToInt32() + ofs), data, 0, len);

			return data;
		}
		#endregion

		#region Int64
		/// <summary>
		/// Reads an Int64 object from an unmanaged pointer.   
		/// </summary>
		/// <param name="ptr">The address in unmanaged memory from which to read. </param>
		/// <param name="ofs">The offset from the ptr where the Int64 is located.</param>
		/// <returns>The Int64 read from the ptr parameter. </returns>
		public static Int64 ReadInt64(IntPtr ptr, int ofs)
		{
			byte[] data = new byte[8];
			Marshal.Copy(new IntPtr(ptr.ToInt32() + ofs), data, 0, 8);

			return BitConverter.ToInt64(data,0);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ptr"></param>
		/// <param name="ofs"></param>
		/// <returns></returns>
		[CLSCompliant(false)]
		public static UInt64 ReadUInt64(IntPtr ptr, int ofs)
		{
			byte[] data = new byte[8];
			Marshal.Copy(new IntPtr(ptr.ToInt32() + ofs), data, 0, 8);

			return BitConverter.ToUInt64(data,0);
		}
		#endregion

		#region Bool

		/// <summary>
		/// Reads a bool from an unmanaged pointer.   
		/// </summary>
		/// <param name="ptr">The address in unmanaged memory from which to read. </param>
		/// <param name="ofs">The offset from the ptr where the bool is located.</param>
		/// <returns>The bool read from the ptr parameter. </returns>
		public static bool ReadBool(IntPtr ptr, int ofs)
		{
			bool b = false;

			byte[] data = new byte[4];
			Marshal.Copy(new IntPtr(ptr.ToInt32() + ofs), data, 0, 4);

			b = Convert.ToBoolean(data);

			return b;
		}

		#endregion

		#region Byte

		/// <summary>
		/// Reads a single byte from an unmanaged pointer.   
		/// </summary>
		/// <param name="ptr">The address in unmanaged memory from which to read. </param>
		/// <param name="ofs">The offset from the ptr where the byte is located.</param>
		/// <returns>The byte read from the ptr parameter. </returns>
		public static byte ReadByte(IntPtr ptr, int ofs)
		{
			return Marshal.ReadByte(ptr, ofs);

			/*byte b;

			byte[] data = new byte[1];
			Marshal.Copy(new IntPtr(ptr.ToInt32() + ofs), data, 0, 1);

			b = data[0];

			return b;*/
		}

		#endregion

		#endregion

		#region Write functions

		#region IntPtr

		/// <summary>
		/// Writes an IntPtr value to unmanaged memory.   
		/// </summary>
		/// <param name="ptr">The address in unmanaged memory from which to write. </param>
		/// <param name="ofs">The offset of the IntPtr from the ptr.</param>
		/// <param name="val">The value to write. </param>
		public static void WriteIntPtr(IntPtr ptr, int ofs, IntPtr val)
		{
			MarshalEx.WriteInt32(ptr, ofs, val.ToInt32());
		}
		
		#endregion

		#region Int32

		/// <summary>
		/// Writes an Int32 value to unmanaged memory.   
		/// </summary>
		/// <param name="ptr">The base address in unmanaged memory from which to write.</param>
		/// <param name="ofs">An additional byte offset, added to the ptr parameter before writing.</param>
		/// <param name="val">The value to write.</param>
		public static void WriteInt32(IntPtr ptr, int ofs, int val)
		{
			Marshal.WriteInt32(ptr, ofs, val);
			/*byte[] data = BitConverter.GetBytes(val);
			Marshal.Copy(data, 0, new IntPtr(ptr.ToInt32() + ofs), data.Length);*/
		}

		/// <summary>
		/// Writes a UInt32 value to unmanaged memory.
		/// </summary>
		/// <param name="ptr">The base address in unmanaged memory from which to write.</param>
		/// <param name="ofs">An additional byte offset, added to the ptr parameter before writing.</param>
		/// <param name="val">The value to write.</param>
		[CLSCompliant(false)]
		public static void WriteUInt32(IntPtr ptr, int ofs, uint val)
		{
			byte[] data = BitConverter.GetBytes(val);
			Marshal.Copy(data, 0, new IntPtr(ptr.ToInt32() + ofs), data.Length);
		}

		#endregion

		#region Int16

		/// <summary>
		/// Writes an Int16 value to unmanaged memory.   
		/// </summary>
		/// <param name="ptr">The address in unmanaged memory from which to write. </param>
		/// <param name="ofs">An additional byte offset, added to the ptr parameter before writing.</param>
		/// <param name="val">The value to write. </param>
		public static void WriteInt16(IntPtr ptr, int ofs, Int16 val)
		{
			Marshal.WriteInt16(ptr, ofs, val);

			/*byte[] data = BitConverter.GetBytes(val);
			Marshal.Copy(data, 0, new IntPtr(ptr.ToInt32() + ofs), data.Length);*/
		}

		/// <summary>
		/// Writes a 16-bit unsigned integer value to unmanaged memory.
		/// </summary>
		/// <param name="ptr">The base address in unmanaged memory from which to write.</param>
		/// <param name="ofs">An additional byte offset, added to the ptr parameter before writing.</param>
		/// <param name="val">The value to write.</param>
		[CLSCompliant(false)]
		public static void WriteUInt16(IntPtr ptr, int ofs, UInt16 val)
		{
			byte[] data = BitConverter.GetBytes(val);
			Marshal.Copy(data, 0, new IntPtr(ptr.ToInt32() + ofs), data.Length);
		}

		#endregion

		#region String

		#region ANSI String

		/// <summary>
		/// Copies the contents of a managed <see cref="System.String"/> into unmanaged memory, converting into ANSI format as it copies.
		/// </summary>
		/// <param name="s">A managed string to be copied. </param>
		/// <returns>The address, in unmanaged memory, to where s was copied, or 0 if a null reference (Nothing in Visual Basic) string was supplied.</returns>
		public static IntPtr StringToHGlobalAnsi(string s)
		{
			if(s == null)
				return IntPtr.Zero;

			int i = s.Length + 1;
			IntPtr ptr = LocalAlloc(LPTR, (uint)i);

			byte[] data = Encoding.ASCII.GetBytes(s);
			Marshal.Copy(data, 0, ptr, data.Length);

			return ptr;
		}

		#endregion

		#region Unicode String		
		/// <summary>
		/// Copies the contents of a managed <see cref="System.String"/> into unmanaged memory.
		/// </summary>
		/// <param name="s">A managed string to be copied.</param>
		/// <returns>The address, in unmanaged memory, to where s was copied, or 0 if a null reference (Nothing in Visual Basic) string was supplied.</returns>
		public static IntPtr StringToHGlobalUni(string s)
		{
			if(s == null)
				return IntPtr.Zero;
	
			int i = (s.Length + 1) * System.Text.UnicodeEncoding.CharSize;
			
			IntPtr ptr = LocalAlloc(LPTR,(uint)i);

			byte[] data = Encoding.Unicode.GetBytes(s);
			Marshal.Copy(data, 0, ptr, data.Length);

			return ptr;
		}

		#endregion

		#endregion

		#region Char

		/// <summary>
		/// Writes a single char value to unmanaged memory.   
		/// </summary>
		/// <param name="ptr">The address in unmanaged memory from which to write. </param>
		/// <param name="ofs">The offset of the char from the ptr.</param>
		/// <param name="val">The value to write. </param>
		public static void WriteChar(IntPtr ptr, int ofs, char val)
		{
			byte[] data = BitConverter.GetBytes(val);
			Marshal.Copy(data, 0, new IntPtr(ptr.ToInt32() + ofs), data.Length);
		}

		#endregion

		#region Byte[]

		/// <summary>
		/// Writes a byte array to unmanaged memory.   
		/// </summary>
		/// <param name="ptr">The address in unmanaged memory from which to write. </param>
		/// <param name="ofs">The offset of the byte array from the ptr.</param>
		/// <param name="val">The value to write. </param>
		public static void WriteByteArray(IntPtr ptr, int ofs, byte[] val)
		{
			Marshal.Copy(val, 0, new IntPtr(ptr.ToInt32() + ofs), val.Length);
		}

		#endregion

		#region Int64

		/// <summary>
		/// Writes an Int64 value to unmanaged memory.   
		/// </summary>
		/// <param name="ptr">The address in unmanaged memory from which to write. </param>
		/// <param name="ofs">An additional byte offset, added to the ptr parameter before writing.</param>
		/// <param name="val">The value to write.</param>
		public static void WriteInt64(IntPtr ptr, int ofs, Int64 val)
		{
			byte[] data = BitConverter.GetBytes(val);
			Marshal.Copy(data, 0, new IntPtr(ptr.ToInt32() + ofs), data.Length);
		}

		/// <summary>
		/// Writes a 64-bit unsigned integer value to unmanaged memory.
		/// </summary>
		/// <param name="ptr">The address in unmanaged memory from which to write.</param>
		/// <param name="ofs">An additional byte offset, added to the ptr parameter before writing.</param>
		/// <param name="val">The value to write.</param>
		[CLSCompliant(false)]
		public static void WriteUInt64(IntPtr ptr, int ofs, UInt64 val)
		{
			byte[] data = BitConverter.GetBytes(val);
			Marshal.Copy(data, 0, new IntPtr(ptr.ToInt32() + ofs), data.Length);
		}

		#endregion

		#region Bool

		/// <summary>
		/// Writes a bool value to unmanaged memory.   
		/// </summary>
		/// <param name="ptr">The address in unmanaged memory from which to write. </param>
		/// <param name="ofs">The offset of the bool from the ptr.</param>
		/// <param name="val">The value to write. </param>
		public static void WriteBool(IntPtr ptr, int ofs, bool val)
		{
			byte[] data = BitConverter.GetBytes(val);
			Marshal.Copy(data, 0, new IntPtr(ptr.ToInt32() + ofs), data.Length);
		}

		#endregion

		#region Byte

		/// <summary>
		/// Writes a single byte value to unmanaged memory.   
		/// </summary>
		/// <param name="ptr">The address in unmanaged memory from which to write. </param>
		/// <param name="ofs">The offset of the byte from the ptr.</param>
		/// <param name="val">The value to write. </param>
		public static void WriteByte(IntPtr ptr, int ofs, byte val)
		{
			Marshal.WriteByte(ptr, ofs, val);

			/*byte[] data = BitConverter.GetBytes(val);
			Marshal.Copy(data, 0, new IntPtr(ptr.ToInt32() + ofs), data.Length);*/
		}

		#endregion

		#endregion

		#region Copy Functions

		/// <summary>
		/// Copies data from an unmanaged memory pointer to a managed 8-bit unsigned integer array.  
		/// </summary>
		/// <param name="source">The memory pointer to copy from.</param>
		/// <param name="destination">The array to copy to.</param>
		/// <param name="startIndex">The zero-based index into the array where Copy should start.</param>
		/// <param name="length">The number of array elements to copy.</param>
		public static void Copy(IntPtr source, byte[] destination, int startIndex, int length) 
		{
			Marshal.Copy(source, destination, startIndex, length);
		}

		/// <summary>
		/// Copies data from an unmanaged memory pointer to a managed double-precision floating-point number array.  
		/// </summary>
		/// <param name="source">The memory pointer to copy from.</param>
		/// <param name="destination">The array to copy to.</param>
		/// <param name="startIndex">The zero-based index into the array where Copy should start.</param>
		/// <param name="length">The number of array elements to copy.</param>
		public static void Copy(IntPtr source, double[] destination, int startIndex, int length) 
		{
			Marshal.Copy(source, destination, startIndex, length);
		}

		/// <summary>
		/// Copies data from an unmanaged memory pointer to a managed single-precision floating-point number array.  
		/// </summary>
		/// <param name="source">The memory pointer to copy from.</param>
		/// <param name="destination">The array to copy to.</param>
		/// <param name="startIndex">The zero-based index into the array where Copy should start.</param>
		/// <param name="length">The number of array elements to copy.</param>
		public static void Copy(IntPtr source, float[] destination, int startIndex, int length) 
		{
			Marshal.Copy(source, destination, startIndex, length);
		}

		/// <summary>
		/// Copies data from an unmanaged memory pointer to a managed 64-bit signed integer array. 
		/// </summary>
		/// <param name="source">The memory pointer to copy from.</param>
		/// <param name="destination">The array to copy to.</param>
		/// <param name="startIndex">The zero-based index into the array where Copy should start.</param>
		/// <param name="length">The number of array elements to copy.</param>
		public static void Copy(IntPtr source, long[] destination, int startIndex, int length) 
		{
			Marshal.Copy(source, destination, startIndex, length);
		}

		/// <summary>
		/// Copies data from an unmanaged memory pointer to a managed 16-bit signed integer array. 
		/// </summary>
		/// <param name="source">The memory pointer to copy from.</param>
		/// <param name="destination">The array to copy to.</param>
		/// <param name="startIndex">The zero-based index into the array where Copy should start.</param>
		/// <param name="length">The number of array elements to copy.</param>
		public static void Copy(IntPtr source, short[] destination, int startIndex, int length) 
		{
			Marshal.Copy(source, destination, startIndex, length);
		}

		/// <summary>
		/// Copies data from an unmanaged memory pointer to a managed character array.  
		/// </summary>
		/// <param name="source">The memory pointer to copy from.</param>
		/// <param name="destination">The array to copy to.</param>
		/// <param name="startIndex">The zero-based index into the array where Copy should start.</param>
		/// <param name="length">The number of array elements to copy.</param>
		public static void Copy(IntPtr source, char[] destination, int startIndex, int length) 
		{
			Marshal.Copy(source, destination, startIndex, length);
		}

		/// <summary>
		/// Copies data from an unmanaged memory pointer to a managed 32-bit signed integer array.  
		/// </summary>
		/// <param name="source">The memory pointer to copy from.</param>
		/// <param name="destination">The array to copy to.</param>
		/// <param name="startIndex">The zero-based index into the array where Copy should start.</param>
		/// <param name="length">The number of array elements to copy.</param>
		public static void Copy(IntPtr source, int[] destination, int startIndex, int length) 
		{
			Marshal.Copy(source, destination, startIndex, length);
		}

		/// <summary>
		/// Copies data from a one-dimensional, managed 8-bit unsigned integer array to an unmanaged memory pointer.
		/// </summary>
		/// <param name="source">The one-dimensional array to copy from.</param>
		/// <param name="startIndex">The zero-based index into the array where Copy should start.</param>
		/// <param name="destination">The memory pointer to copy to.</param>
		/// <param name="length">The number of array elements to copy.</param>
		public static void Copy(byte[] source, int startIndex, IntPtr destination, int length)
		{
			Marshal.Copy(source, startIndex, destination, length);
		}

		/// <summary>
		/// Copies data from a one-dimensional, managed double-precision floating-point number array to an unmanaged memory pointer.  
		/// </summary>
		/// <param name="source">The one-dimensional array to copy from.</param>
		/// <param name="startIndex">The zero-based index into the array where Copy should start.</param>
		/// <param name="destination">The memory pointer to copy to.</param>
		/// <param name="length">The number of array elements to copy.</param>
		public static void Copy(double[] source, int startIndex, IntPtr destination, int length)
		{
			Marshal.Copy(source, startIndex, destination, length);
		}

		/// <summary>
		/// Copies data from a one-dimensional, managed single-precision floating-point number array to an unmanaged memory pointer
		/// </summary>
		/// <param name="source">The one-dimensional array to copy from.</param>
		/// <param name="startIndex">The zero-based index into the array where Copy should start.</param>
		/// <param name="destination">The memory pointer to copy to.</param>
		/// <param name="length">The number of array elements to copy.</param>
		public static void Copy(float[] source, int startIndex, IntPtr destination, int length)
		{
			Marshal.Copy(source, startIndex, destination, length);
		}

		/// <summary>
		/// Copies data from a one-dimensional, managed 64-bit signed integer array to an unmanaged memory pointer. 
		/// </summary>
		/// <param name="source">The one-dimensional array to copy from.</param>
		/// <param name="startIndex">The zero-based index into the array where Copy should start.</param>
		/// <param name="destination">The memory pointer to copy to.</param>
		/// <param name="length">The number of array elements to copy.</param>
		public static void Copy(long[] source, int startIndex, IntPtr destination, int length)
		{
			Marshal.Copy(source, startIndex, destination, length);
		}

		/// <summary>
		/// Copies data from a one-dimensional, managed 16-bit signed integer array to an unmanaged memory pointer. 
		/// </summary>
		/// <param name="source">The one-dimensional array to copy from.</param>
		/// <param name="startIndex">The zero-based index into the array where Copy should start.</param>
		/// <param name="destination">The memory pointer to copy to.</param>
		/// <param name="length">The number of array elements to copy.</param>
		public static void Copy(short[] source, int startIndex, IntPtr destination, int length)
		{
			Marshal.Copy(source, startIndex, destination, length);
		}

		/// <summary>
		/// Copies data from a one-dimensional, managed character array to an unmanaged memory pointer.  
		/// </summary>
		/// <param name="source">The one-dimensional array to copy from.</param>
		/// <param name="startIndex">The zero-based index into the array where Copy should start.</param>
		/// <param name="destination">The memory pointer to copy to.</param>
		/// <param name="length">The number of array elements to copy.</param>
		public static void Copy(char[] source, int startIndex, IntPtr destination, int length)
		{
			Marshal.Copy(source, startIndex, destination, length);
		}

		/// <summary>
		/// Copies data from a one-dimensional, managed 32-bit signed integer array to an unmanaged memory pointer. 
		/// </summary>
		/// <param name="source">The one-dimensional array to copy from.</param>
		/// <param name="startIndex">The zero-based index into the array where Copy should start.</param>
		/// <param name="destination">The memory pointer to copy to.</param>
		/// <param name="length">The number of array elements to copy.</param>
		public static void Copy(int[] source, int startIndex, IntPtr destination, int length)
		{
			Marshal.Copy(source, startIndex, destination, length);
		}

		#endregion

		#region Memory Functions

		private static bool IsNotWin32Atom(IntPtr ptr)
		{
			long b;
			b = (long)ptr;
			return (((long) 0) != (b & (long)MarshalEx.HIWORDMASK)); 
		}

		/// <summary>
		/// Allocates unmanaged memory.
		/// </summary>
		/// <param name="cb">The number of bytes in memory required. </param>
		/// <returns>An IntPtr to the newly allocated memory. This memory must be released using the Marshal.FreeHGlobal method.</returns>
		public static IntPtr AllocHGlobal(int cb)
		{
			IntPtr ptr = LocalAlloc(LPTR, (uint)cb);
			if (ptr == IntPtr.Zero)
			{
				throw new OutOfMemoryException();
 
			}
			return ptr; 
		}

		/// <summary>
		/// Allocates unmanaged memory.
		/// </summary>
		/// <param name="cb">The number of bytes in memory required. </param>
		/// <returns>An IntPtr to the newly allocated memory. This memory must be released using the Marshal.FreeHGlobal method.</returns>
		public static IntPtr AllocHGlobal(IntPtr cb)
		{
			return MarshalEx.AllocHGlobal((int)cb); 			
		}

		/// <summary>
		/// Resizes a block of memory previously allocated with <see cref="AllocHGlobal(System.IntPtr)"/>.
		/// <para><b>New in v1.2</b></para>
		/// </summary>
		/// <param name="pv">A pointer to memory allocated with <see cref="AllocHGlobal(System.IntPtr)"/>.</param>
		/// <param name="cb"> The new size of the allocated block.</param>
		/// <returns>An <see cref="System.IntPtr"/> to the reallocated memory.
		/// This memory must be released using <see cref="FreeHGlobal(System.IntPtr)"/>.</returns>
		/// <exception cref="OutOfMemoryException">There is insufficient memory to satisfy the request.</exception>
		public static IntPtr ReAllocHGlobal(IntPtr pv, IntPtr cb)
		{
			IntPtr ptr = LocalReAllocCE(pv, (int)cb, LPTR);

			if(ptr == IntPtr.Zero)
			{
				throw new OutOfMemoryException();
			}

			return ptr;
		}

		/// <summary>
		/// Frees memory previously allocated from unmanaged memory.
		/// </summary>
		/// <param name="hGlobal">The handle returned by the original matching call to AllocHGlobal.</param>
		public static void FreeHGlobal(IntPtr hGlobal)
		{
			if(MarshalEx.IsNotWin32Atom(hGlobal))
			{
				LocalFree(hGlobal);
			}
			else 
			{
				throw new ArgumentException("hGlobal is not an unmanaged atom.");
			}
		}

		/// <summary>
		/// Allocates unmanaged memory.
		/// </summary>
		/// <param name="cb">The number of bytes in memory required. </param>
		/// <returns>An IntPtr to the newly allocated memory. This memory must be released using the <see cref="M:MarshalEx.FreeHLocal(System.IntPtr)"/> method.</returns>
		[CLSCompliant(false)]
		public static IntPtr AllocHLocal(uint cb)
		{
			return LocalAlloc(LPTR, cb);
		}
		/// <summary>
		/// Allocates unmanaged memory.
		/// </summary>
		/// <param name="cb">The number of bytes in memory required. </param>
		/// <returns>An IntPtr to the newly allocated memory. This memory must be released using the <see cref="M:MarshalEx.FreeHLocal(System.IntPtr)"/> method.</returns>
		public static IntPtr AllocHLocal(int cb)
		{
			return LocalAlloc(LPTR, (uint)cb);
		}

		/// <summary>
		/// Frees memory previously allocated from unmanaged memory.
		/// </summary>
		/// <param name="hMem">The handle returned by the original matching call to AllocHGlobal.</param>
		public static void FreeHLocal(IntPtr hMem)
		{
			if(hMem != IntPtr.Zero)
				LocalFree(hMem);
		}
		
		/// <summary>
		/// Converts a time_t value to a DateTime value.
		/// </summary>
		/// <param name="time_t">The time_t value to convert.</param>
		/// <returns>A DateTime value equivalent to the time_t suppled.</returns>
		[CLSCompliant(false)]
		public static DateTime Time_tToDateTime(uint time_t) 
		{
			long win32FileTime = 10000000*(long)time_t + 116444736000000000;
			return DateTime.FromFileTimeUtc(win32FileTime);
		}

		/// <summary>
		/// Returns the unmanaged size of an object in bytes.  
		/// </summary>
		/// <param name="structure">The object whose size is to be returned.</param>
		/// <returns>The size of the structure parameter in unmanaged code</returns>
		public static int SizeOf(object structure) 
		{
			return Marshal.SizeOf(structure);
		}

		/// <summary>
		///  Returns the size of an unmanaged type in bytes.  
		/// </summary>
		/// <param name="t">The System.Type whose size is to be returned.</param>
		/// <returns>The size of the structure parameter in unmanaged code</returns>
		public static int SizeOf(Type t)
		{
			return Marshal.SizeOf(t);
		}

		/// <summary>
		/// Marshals data from a managed object to an unmanaged block of memory.  
		/// </summary>
		/// <param name="structure">A managed object holding the data to be marshaled. This object must be an instance of a formatted class.</param>
		/// <param name="ptr">A pointer to an unmanaged block of memory, which must be allocated before this method is called.</param>
		/// <param name="fDeleteOld">true to have the System.Runtime.InteropServices.Marshal.DestroyStructure(System.IntPtr,System.Type) method called on the ptr parameter before this method executes. Note that passing false can lead to a memory leak.</param>
		public static void StructureToPtr(object structure, IntPtr ptr, bool fDeleteOld)
		{
			Marshal.StructureToPtr(structure, ptr, fDeleteOld);
		}

		/// <summary>
		/// Returns the length of the string at the pointer
		/// </summary>
		/// <param name="ptr">The pointer to the string to measure.</param>
		/// <returns>The length of the string at the pointer.</returns>
		private static int lstrlenW(IntPtr ptr)
		{
			return String_wcslen(ptr);
		}

		#endregion

		#region Miscellaneous Functions

		#region GetHINSTANCE
		/// <summary>
		/// Returns the instance handle (HINSTANCE) for the specified module.
		/// <para><b>New in v1.3</b></para>
		/// </summary>
		/// <param name="m">The <see cref="System.Reflection.Module"/> whose HINSTANCE is desired.</param>
		/// <returns>The HINSTANCE for m; -1 if the module does not have an HINSTANCE.</returns>
		public static IntPtr GetHINSTANCE(System.Reflection.Module m )
		{
			IntPtr hinst = IntPtr.Zero;

			if(m.Assembly == System.Reflection.Assembly.GetCallingAssembly())
			{
				hinst = GetModuleHandle(null);
			}
			else
			{
				hinst = GetModuleHandle(m.Assembly.GetName().CodeBase);
			}

			if(hinst == IntPtr.Zero)
			{
				return new IntPtr(-1);
			}
			else
			{
				return hinst;
			}
		}
		[DllImport("coredll.dll", EntryPoint="GetModuleHandleW", SetLastError=true)]
		private static extern IntPtr GetModuleHandle(string lpszModule);
		#endregion

		/// <summary>
		///  Returns the error code returned by the last unmanaged function called using platform invoke that has the System.Runtime.InteropServices.DllImportAttribute.SetLastError flag set.
		/// </summary>
		/// <returns>The last error code set by a call to the Win32 SetLastError API method. </returns>
		public static int GetLastWin32Error()
		{
			return Marshal.GetLastWin32Error();
		}

		/// <summary>
		///  Indicates whether a specified object represents a COM object.
		/// </summary>
		/// <param name="o">The object to check. </param>
		/// <returns> true if the o parameter is a COM type; otherwise, false.</returns>
		public static bool IsComObject(object o)
		{
			return Marshal.IsComObject(o);
		}

		#endregion

		#region API Prototypes

		[DllImport("coredll.dll",EntryPoint="LocalAlloc",SetLastError=true)]
		static extern IntPtr LocalAlloc(uint uFlags, uint Bytes);

		[DllImport("coredll.dll", EntryPoint="LocalReAlloc", SetLastError=true)]
		static extern IntPtr LocalReAllocCE(IntPtr hMem, int uBytes, int fuFlags);

		[DllImport("coredll.dll",EntryPoint="LocalFree",SetLastError=true)]	
		static extern IntPtr LocalFree(IntPtr hMem);

		[DllImport("mscoree.dll",EntryPoint="#1",SetLastError=true)]
		static extern int String_wcslen(IntPtr pws);

		#endregion
	}
}
