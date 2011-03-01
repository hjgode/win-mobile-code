//#define USES_EVENT_PUBLISHING
//use the above if you can fix the exception in Form1^, when setting the textbox1.text
//See project properties if there is already a conditional symbol USES_EVENT_PUBLISHING in the
//build settings
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Net;
using System.Windows.Forms;
using System.ComponentModel;

    public static class myPing{
        #region ICMP helper stuff
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct ICMP_OPTIONS
        {
            public Byte Ttl;
            public Byte Tos;
            public Byte Flags;
            public Byte OptionsSize;
            public IntPtr OptionsData;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct ICMP_ECHO_REPLY
        {
            public int Address;
            public int Status;
            public int RoundTripTime;
            public Int16 DataSize;
            public Int16 Reserved;
            public IntPtr DataPtr;
            public ICMP_OPTIONS Options;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 250)]
            public String Data;
        }

        [DllImport("iphlpapi.dll", SetLastError = true)]
        private static extern IntPtr IcmpCreateFile();
        [DllImport("iphlpapi.dll", SetLastError = true)]
        private static extern bool IcmpCloseHandle(IntPtr handle);
        [DllImport("iphlpapi.dll", SetLastError = true)]
        private static extern Int32 IcmpSendEcho(IntPtr icmpHandle, Int32 destinationAddress, String requestData, Int32 requestSize, ref ICMP_OPTIONS requestOptions, ref ICMP_ECHO_REPLY replyBuffer, Int32 replySize, Int32 timeout);
#endregion

        public static int Ping(IPAddress IP)
        {
            IntPtr ICMPHandle;
            Int32 iIP;
            String sData;
            ICMP_OPTIONS oICMPOptions = new ICMP_OPTIONS();
            ICMP_ECHO_REPLY ICMPReply = new ICMP_ECHO_REPLY();
            Int32 iReplies;

            ICMPHandle = IcmpCreateFile();
            iIP = BitConverter.ToInt32(IP.GetAddressBytes(), 0);
            sData = "x";
            oICMPOptions.Ttl = 255;

            iReplies = IcmpSendEcho(ICMPHandle, iIP,
                sData, sData.Length, ref oICMPOptions, ref ICMPReply,
                Marshal.SizeOf(ICMPReply), 30);

            IcmpCloseHandle(ICMPHandle);
            return iReplies;
        }
        public static int PingQdata(ref queueData qData)
        {
            
            IntPtr ICMPHandle;
            Int32 iIP;
            String sData;
            ICMP_OPTIONS oICMPOptions = new ICMP_OPTIONS();
            ICMP_ECHO_REPLY ICMPReply = new ICMP_ECHO_REPLY();
            Int32 iReplies;

            int[] roundTrips= new int[qData.iPingCount];
            
            int averageRoundTrip=0;
            
            //reset ping reply count
            qData.iPingReplies = 0;
            /*
            qData.iPingTimeout = (byte)qData.iPingTimeout;
            qData.sIP = IP.ToString();
            qData.IP = IP;
            */

            ICMPHandle = IcmpCreateFile();
            iIP = BitConverter.ToInt32(qData.IP.GetAddressBytes(), 0);
            sData = "x";

            oICMPOptions.Ttl = (byte) qData.iPingTimeout; //time to live

            for(int i=0; i<qData.iPingCount; i++){

                
                iReplies = IcmpSendEcho(ICMPHandle, iIP,
                    sData, sData.Length, ref oICMPOptions, ref ICMPReply,
                    Marshal.SizeOf(ICMPReply), 30);

                if (iReplies == 1)
                {
                    roundTrips[i] = ICMPReply.RoundTripTime;
                    qData.iPingReplies++;
                }
                else
                    roundTrips[i] = -1;
            }
            //calc average
            int counts = 0;
            for (int i = 0; i < qData.iPingCount; i++)
            {
                if (roundTrips[i] != -1)
                {
                    averageRoundTrip += roundTrips[i];
                    counts++;
                }
            }
            if(counts>0)
                averageRoundTrip = averageRoundTrip / counts;
            else
                averageRoundTrip = 0;

            qData.iPingReplyTime = averageRoundTrip;

            IcmpCloseHandle(ICMPHandle);
            return qData.iPingReplies;
        }
    }
