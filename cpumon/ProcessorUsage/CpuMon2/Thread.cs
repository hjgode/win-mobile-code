using System;

using System.Collections.Generic;
using System.Text;

using DWORD = System.UInt32;
using FILETIME = System.Int64;

namespace System.Process
{
    /// <summary>
    /// FILETIME times spend in user and kernel
    /// </summary>
    public class threadtimes
    {
        public FILETIME kernel;
        public FILETIME user;
        public threadtimes(FILETIME u, FILETIME k)
        {
            kernel = k;
            user = u;
        }
        public byte[] ToByte()
        {
            List<byte> bufL = new List<byte>();
            bufL.AddRange(BitConverter.GetBytes(kernel));
            bufL.AddRange(BitConverter.GetBytes(user));
            return bufL.ToArray();
        }
        public threadtimes FromByte(byte[] buf, ref int offset)
        {            
            kernel = BitConverter.ToInt64(buf, offset); //8 bytes
            offset += sizeof(System.Int64);

            user = BitConverter.ToInt64(buf, offset);
            offset += sizeof(System.Int64);
            
            return this;
        }
        /// <summary>
        /// return a threadtimes object
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="offset">start offset</param>
        public threadtimes (byte[] buf, ref int offset)
        {
            this.FromByte(buf, ref offset);
        }
    }
    //struct threadStruct
    //{
    //    public UInt32 dwOwnerProcID;
    //    public UInt32 dwThreadID;
    //    public FILETIME ftUser;
    //    public FILETIME ftKernel;
    //}
    public class thread
    {
        public UInt32 dwOwnerProcID;
        public threadtimes thread_times;
        public UInt32 dwThreadID;
        public thread(UInt32 id, UInt32 thID, threadtimes tht)
        {
            dwOwnerProcID = id;
            thread_times = tht;
            dwThreadID = thID;
            if (thID == 0)
                System.Diagnostics.Debugger.Break();
        }
        public byte[] ToByte()
        {
            List<byte> bufL = new List<byte>();
            bufL.AddRange(BitConverter.GetBytes(dwOwnerProcID));

            bufL.AddRange(thread_times.ToByte());

            bufL.AddRange(BitConverter.GetBytes(dwThreadID));
            return bufL.ToArray();
        }
        public thread FromByte(byte[] buf, ref int offset)
        {
            dwOwnerProcID = BitConverter.ToUInt32(buf, offset); //4 bytes
            offset += sizeof(System.UInt32);

            thread_times = new threadtimes(buf, ref offset);
            
            dwThreadID = BitConverter.ToUInt32(buf, offset);
            offset += sizeof(System.UInt32);

            return this;
        }
        public thread(byte[] buf, ref int offs)
        {
            this.FromByte(buf, ref offs);
        }
    }
	/// <summary>
	/// class to store a thread's statistic data
	/// the timestamp of the capture and the spend user and kernel times
	/// and the measure interval 
	/// </summary>
    public class threadStatistic
    {
        public UInt32 dwOwnerProcID;
		/// <summary>
		/// store the spend times of in-user and in-kernel 
		/// </summary>
        public threadtimes thread_times;
        public UInt32 dwThreadID;
		/// <summary>
		/// the measured time interval in ms 
		/// </summary>
		public UInt32 duration;
		/// <summary>
		/// the timestamp of this captured statistic 
		/// </summary>
		public long timestamp;
        public threadStatistic(UInt32 id, UInt32 thID, threadtimes tht, UInt32 _duration, long _timestamp)
        {
            dwOwnerProcID = id;
            thread_times = tht;
            dwThreadID = thID;
            if (thID == 0)
                System.Diagnostics.Debugger.Break();
			duration=_duration;
			timestamp=_timestamp;
        }
        public byte[] ToByte()
        {
            List<byte> bufL = new List<byte>();
            bufL.AddRange(BitConverter.GetBytes(dwOwnerProcID));

            bufL.AddRange(thread_times.ToByte());

            bufL.AddRange(BitConverter.GetBytes(dwThreadID));
			
			bufL.AddRange(BitConverter.GetBytes(duration));
			
			bufL.AddRange(BitConverter.GetBytes(timestamp));
			
            return bufL.ToArray();
        }
        public threadStatistic FromByte(byte[] buf, ref int offset)
        {
            dwOwnerProcID = BitConverter.ToUInt32(buf, offset); //4 bytes
            offset += sizeof(System.UInt32);

            thread_times = new threadtimes(buf, ref offset);
            
            dwThreadID = BitConverter.ToUInt32(buf, offset);
            offset += sizeof(System.UInt32);

			duration=BitConverter.ToUInt32(buf, offset);
			offset += sizeof(System.UInt32);
			
			timestamp=BitConverter.ToInt64(buf, offset);
			offset += sizeof(System.UInt64);

            return this;
        }
        public threadStatistic(byte[] buf, ref int offs)
        {
            this.FromByte(buf, ref offs);
        }
    }
}
