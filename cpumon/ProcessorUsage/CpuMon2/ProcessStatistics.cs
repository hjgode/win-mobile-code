using System;

using System.Collections.Generic;
using System.Text;

namespace System.Process
{
    public class ProcessStatistics
    {
        static void dumpStatistics(Dictionary<string, process_statistics> stats)
        {
            foreach (KeyValuePair<string, process_statistics> kp in stats)
            {
                System.Diagnostics.Debug.WriteLine(kp.Value.sName + ": " + kp.Value.procUsage.kernel.ToString() + "/" + kp.Value.procUsage.user.ToString());
            }
        }
        public static string dumpProcStats(List<process_statistics> procStats)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (process_statistics ps in procStats)
            {
                sb.Append(ps.sName + "\t");
                sb.Append((ps.procUsage.kernel + ps.procUsage.user).ToString() + "\n");
            }
            return sb.ToString();
        }

        public static string dumpProcStats(ProcessStatsEventArgs procStats)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (KeyValuePair<string, process_statistics> ps in procStats.procStatistics)
            {
                sb.Append(ps.Value.sName + "\t");
                sb.Append((ps.Value.procUsage.kernel + ps.Value.procUsage.user).ToString() + "\r\n");
            }
            sb.Append("Duration:\t" + procStats.duration + "\r\n");
            return sb.ToString();
        }

        public class process_usage
        {
            /// % time the kernel spent in kernel space
            public long kernel;
            /// % time the kernel spent in user space
            public long user;
            public process_usage(long k, long u)
            {
                kernel = k;
                user = u;
            }
            public byte[] ToByte()
            {
                List<byte> buf = new List<byte>();
                buf.AddRange(BitConverter.GetBytes(kernel));
                buf.AddRange(BitConverter.GetBytes(user));
                return buf.ToArray();
            }
            public process_usage FromByte(ref byte[] buf, ref int offs)
            {
                int offset = offs;
                kernel = BitConverter.ToInt64(buf, offset);
                offset += sizeof(Int64);
                user = BitConverter.ToInt64(buf, offset);
                offs = offset;
                return this;
            }
            public process_usage(ref byte[] buf, ref int offs)
            {
                int offset = offs;
                kernel = BitConverter.ToInt64(buf, offset);
                offset += sizeof(Int64);
                user = BitConverter.ToInt64(buf, offset);
                offset += sizeof(Int64);
                offs = offset;
            }

        }
        public class process_statistics
        {
            public string sName;
            /// <summary>
            /// DateTime as Ticks
            /// </summary>
            public long dateTime;
            public UInt32 duration;
            public process_usage procUsage;
            public UInt32 processID;
            public List<threadStatistic> ThreadStatList = new List<threadStatistic>();

            public string remoteIP = "0.0.0.0";

            //public process_statistics(uint uProcessID, string s, process_usage u, long dt, uint du)
            //{
            //    sName = s;
            //    processID = uProcessID;
            //    procUsage = u;
            //    dateTime = dt;
            //    duration = du;
            //}

            /// <summary>
            /// an object to hold informations about a process
            /// </summary>
            /// <param name="uProcessID">the Process ID</param>
            /// <param name="s">the process name</param>
            /// <param name="u">the process usage (kernel and user times)</param>
            /// <param name="dt">a timestamp</param>
            /// <param name="du">the elapsed time since last statistic</param>
            /// <param name="threadList">a list of all threads of this process</param>
            public process_statistics(uint uProcessID, string s, process_usage u, long dt, uint du, List<threadStatistic> threadList)
            {
                sName = s;
                processID = uProcessID;
                procUsage = u;
                dateTime = dt;
                duration = du;
                ThreadStatList = threadList;
            }
            public process_statistics(uint uProcessID, string s, process_usage u, long dt, uint du, List<threadStatistic> threadList, string sRemoteIP)
            {
                sName = s;
                processID = uProcessID;
                procUsage = u;
                dateTime = dt;
                duration = du;
                ThreadStatList = threadList;
                remoteIP = sRemoteIP;
            }

            public override string ToString()
            {
                return dumpStatistics();
            }
            public string dumpStatistics()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(  this.processID.ToString("x") + ", " +
                            this.sName + ", " +
                            "packetsize: " + this.ToByte().Length.ToString() + ", " +
                            this.procUsage.kernel.ToString() + "/" +
                            this.procUsage.user.ToString() + ", " +
                            this.duration.ToString() + 
                            this.remoteIP + "\r\n");
                foreach (threadStatistic th in this.ThreadStatList)
                {
                    sb.Append("    0x" + th.dwThreadID.ToString("x") + ":" + th.thread_times.kernel.ToString() + "/" + th.thread_times.user.ToString() + "\r\n");
                }
                
                return sb.ToString();
            }

            public process_statistics(byte[] buffer)
            {
                this.FromByte(buffer);
            }
            public byte[] ToByte()
            {
                List<byte> buf = new List<byte>();
                //string length
                Int16 bLen = (Int16)System.Text.Encoding.UTF8.GetByteCount(sName);
                buf.AddRange(BitConverter.GetBytes(bLen));
                byte[] bName = System.Text.Encoding.UTF8.GetBytes(sName);
                //string as byte[]
                buf.AddRange(bName);

                buf.AddRange(BitConverter.GetBytes(dateTime));
                buf.AddRange(BitConverter.GetBytes(duration));
                buf.AddRange(procUsage.ToByte());
                buf.AddRange(BitConverter.GetBytes(processID));
                
                //list count
                Int16 iCnt = (Int16) ThreadStatList.Count;
                threadStatistic[] threadsArray = ThreadStatList.ToArray();
                buf.AddRange(BitConverter.GetBytes(iCnt));
                //now add the threads of the list
                foreach (threadStatistic th in threadsArray)
                    buf.AddRange(th.ToByte());

                return buf.ToArray();
            }
            public process_statistics FromByte(byte[] buf)
            {
                int offset = 0;
                Int16 bLen = BitConverter.ToInt16(buf, 0); //2 bytes
                offset += sizeof(System.Int16);
                if (bLen > 0)
                    this.sName = System.Text.Encoding.UTF8.GetString(buf, offset, bLen);
                offset += bLen;
                this.dateTime = BitConverter.ToInt64(buf, offset);
                offset += sizeof(System.Int64);
                this.duration = BitConverter.ToUInt32(buf, offset);
                offset += sizeof(System.Int32);
                this.procUsage = new process_usage(ref buf, ref offset);
                //offset = offset; //has been changed by process_usage
                
                this.processID = BitConverter.ToUInt32(buf, offset);
                offset += sizeof(System.UInt32);

                //how many thtreads are in the byte stream
                Int16 iCnt = BitConverter.ToInt16(buf, offset);
                offset += sizeof(System.Int16);
                //start reading the threads
                List<threadStatistic> thList = new List<threadStatistic>();
                for (int x = 0; x < iCnt; x++)
                {
                    threadStatistic th = new threadStatistic(buf, ref offset);
                    thList.Add(th);
                }
                this.ThreadStatList = thList;
                this.remoteIP = "0.0.0.0";  //unknown
                return this;
            }
        }
    }
    public class ProcessStatsEventArgs : EventArgs
    {
        //fields
        Dictionary<string, ProcessStatistics.process_statistics> _procStatList;
        public Dictionary<string, ProcessStatistics.process_statistics> procStatistics
        {
            get { return _procStatList; }
        }
        uint _duration = 0;
        public uint duration
        {
            get { return _duration; }
        }
        public ProcessStatsEventArgs(Dictionary<string, ProcessStatistics.process_statistics> _list, uint durat)
        {
            _procStatList = _list;
            _duration = durat;
        }
    }
}
