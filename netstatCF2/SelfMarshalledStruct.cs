using System;
using System.Text;

namespace AdapterInfo
{
	/// <summary>
	/// This class implements base functionality for a bytearray-based structure
	/// </summary>
	public class SelfMarshalledStruct
	{
		#region Internal Data
		protected byte[] data;
		protected int baseOffset;
		#endregion

		public SelfMarshalledStruct(int size)
		{
			data = new byte[size];
			baseOffset = 0;
		}

		public SelfMarshalledStruct(byte[] data, int offset)
		{
			this.data = data;
			baseOffset = offset;
		}

		public Char GetChar( int offset )
		{
			return BitConverter.ToChar(data, baseOffset + offset);
		}

		public void SetChar( int offset, Char val )
		{
			Buffer.BlockCopy(BitConverter.GetBytes( val ), 0, data, baseOffset + offset, 2 );
		}

		public Int32 GetInt32( int offset )
		{
			return BitConverter.ToInt32(data, baseOffset + offset);
		}
		
		public void SetInt32( int offset, Int32 val )
		{
			Buffer.BlockCopy(BitConverter.GetBytes( val ), 0, data, baseOffset + offset, 4 );
		}

		public UInt32 GetUInt32( int offset )
		{
			return BitConverter.ToUInt32(data, baseOffset + offset);
		}
		
		public void SetUInt32( int offset, UInt32 val )
		{
			Buffer.BlockCopy(BitConverter.GetBytes( val ), 0, data, baseOffset + offset, 4 );
		}

		public Int16 GetInt16( int offset )
		{
			return BitConverter.ToInt16(data, baseOffset + offset);
		}
		
		public void SetInt16( int offset, Int16 val )
		{
			Buffer.BlockCopy(BitConverter.GetBytes( val ), 0, data, baseOffset + offset, 2 );
		}

		public UInt16 GetUInt16( int offset )
		{
			return BitConverter.ToUInt16(data, baseOffset + offset);
		}
		
		public void SetUInt16( int offset, UInt16 val )
		{
			Buffer.BlockCopy(BitConverter.GetBytes( val ), 0, data, baseOffset + offset, 2 );
		}

		public string GetStringUni(int offset, int len)
		{
			string s = Encoding.Unicode.GetString(data, baseOffset + offset, len);
			int nullPos = s.IndexOf('\0');
			if ( nullPos > -1 )
				s = s.Substring(0, nullPos);
			return s;
		}

		public string GetStringAscii(int offset, int len)
		{
			string s = Encoding.ASCII.GetString(data, baseOffset + offset, len);
			int nullPos = s.IndexOf('\0');
			if ( nullPos > -1 )
				s = s.Substring(0, nullPos);
			return s;
		}

		public byte[] GetSlice(int offset, int len)
		{
			byte[] ret = new byte[len];
			Buffer.BlockCopy(data, baseOffset + offset, ret, 0, len);
			return ret;
		}

		public void SetStringUni(string str, int offset, int len)
		{
			Encoding.Unicode.GetBytes(str, 0, Math.Min(str.Length, len), data, baseOffset + offset);
		}

		public byte this[int offset]
		{
			get { return data[baseOffset + offset]; }
			set { data[baseOffset + offset] = value; }
		}

		public object Get(Type t, int offset)
		{
			if ( t.IsPrimitive )
			{
				if ( t.BaseType == typeof(Int32) || t == typeof(Int32))
					return GetInt32(baseOffset + offset);
				else if ( t.BaseType == typeof(Int16) || t == typeof(Int16) )
					return GetInt16(baseOffset + offset);
				else if ( t.BaseType == typeof(UInt32) || t == typeof(UInt32) )
					return GetInt32(baseOffset + offset);
				else if ( t.BaseType == typeof(UInt16) || t == typeof(UInt16) )
					return GetUInt16(baseOffset + offset);
				else if ( t.BaseType == typeof(byte)|| t == typeof(byte) )
					return this[baseOffset + offset];
			}
			return null;
		}

		public void Set(Type t, int offset, object Val)
		{
			if ( t.IsPrimitive )
			{
				if ( t.BaseType == typeof(Int32) || t == typeof(Int32))
					SetInt32(baseOffset + offset, (int)Val);
				else if ( t.BaseType == typeof(Int16) || t == typeof(Int16) )
					SetInt16(baseOffset + offset, (short)Val);
				else if ( t.BaseType == typeof(UInt32) || t == typeof(UInt32) )
					SetUInt32(baseOffset + offset, (UInt32)Val);
				else if ( t.BaseType == typeof(UInt16) || t == typeof(UInt16) )
					SetUInt16(baseOffset + offset, (ushort)Val);
				else if ( t.BaseType == typeof(byte)|| t == typeof(byte) )
					this[baseOffset + offset] = (byte)Val;
			}
			else if ( t == typeof(string) )
			{
				SetStringUni((string)Val, baseOffset + offset, ((string)Val).Length);
			}
		}

		public byte[] Data { get { return data; } }
	}
}
