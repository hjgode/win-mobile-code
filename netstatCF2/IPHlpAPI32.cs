using System;
using System.Net;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

using System.Collections.Generic;


namespace IpHlpApidotnet
{
    #region UDP

    [StructLayout(LayoutKind.Sequential)]
    public struct MIB_UDPSTATS
    {
        public int dwInDatagrams;
        public int dwNoPorts;
        public int dwInErrors;
        public int dwOutDatagrams;
        public int dwNumAddrs;
    }

    public struct MIB_UDPTABLE
    {
        public int dwNumEntries;
        public MIB_UDPROW[] table;

    }

    public struct MIB_UDPROW
    {
        public IPEndPoint Local;
    }

    public struct MIB_EXUDPTABLE
    {
        public int dwNumEntries;
        public MIB_EXUDPROW[] table;

    }

    public struct MIB_EXUDPROW
    {
        public IPEndPoint Local;
        public int dwProcessId;
        public string ProcessName;
    }

    #endregion

    #region TCP

    enum RtoAlgorithm
    {
        MIB_TCP_RTO_OTHER = 1,      //Other
        MIB_TCP_RTO_CONSTANT = 2,   //Constant Time-out
        MIB_TCP_RTO_RSRE = 3,       //MIL-STD-1778 Appendix B
        MIB_TCP_RTO_VANJ = 4,       //Van Jacobson's Algorithm
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MIB_TCPSTATS
    {
        public int dwRtoAlgorithm;
        public int dwRtoMin;
        public int dwRtoMax;
        public int dwMaxConn;
        public int dwActiveOpens;
        public int dwPassiveOpens;
        public int dwAttemptFails;
        public int dwEstabResets;
        public int dwCurrEstab;
        public int dwInSegs;
        public int dwOutSegs;
        public int dwRetransSegs;
        public int dwInErrs;
        public int dwOutRsts;
        public int dwNumConns;
    }


    public struct MIB_TCPTABLE
    {
        public int dwNumEntries;
        public MIB_TCPROW[] table;

    }

    public struct MIB_TCPROW
    {
        public string StrgState;
        public int iState;
        public IPEndPoint Local;
        public IPEndPoint Remote;
    }
#if !PocketPC
	public struct MIB_EXTCPTABLE
	{
		public int dwNumEntries;  
		public MIB_EXTCPROW[] table;

	}

	public struct MIB_EXTCPROW
	{
		public string StrgState;
		public int iState;
		public IPEndPoint Local;  
		public IPEndPoint Remote;
		public int dwProcessId;
		public string ProcessName;
	}
#endif



    #endregion


    public class IPHlpAPI32Wrapper
    {
        public const byte NO_ERROR = 0;
        public const int FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100;
        public const int FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200;
        public const int FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;
        public int dwFlags = FORMAT_MESSAGE_ALLOCATE_BUFFER |
            FORMAT_MESSAGE_FROM_SYSTEM |
            FORMAT_MESSAGE_IGNORE_INSERTS;



        [DllImport("iphlpapi.dll", SetLastError = true)]
        public extern static int GetUdpStatistics(ref MIB_UDPSTATS pStats);

        [DllImport("iphlpapi.dll", SetLastError = true)]
        public static extern int GetUdpTable(byte[] UcpTable, out int pdwSize, bool bOrder);

        [DllImport("iphlpapi.dll", SetLastError = true)]
        public extern static int GetTcpStatistics(ref MIB_TCPSTATS pStats);

        [DllImport("iphlpapi.dll", SetLastError = true)]
        public static extern int GetTcpTable(byte[] pTcpTable, out int pdwSize, bool bOrder);

        [DllImport("iphlpapi.dll", SetLastError = true)]
        public static extern int GetIpForwardTable(IntPtr pIpForwardTable, ref int pdwSize, bool bOrder);
        [DllImport("iphlpapi.dll", SetLastError = true)]
        public static extern int GetIpForwardTable(byte[] pIpForwardTable, ref int pdwSize, bool bOrder);
        const int ANY_SIZE = 1;
        [StructLayout(LayoutKind.Sequential)]
        public struct MIB_IPFORWARDTABLE
        {
            public UInt32 dwNumEntries;
            public MIB_IPFORWARDROW[] table /*[ANY_SIZE]*/;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct MIB_IPFORWARDROW
        {
            /// <summary>
            /// The destination IPv4 address of the route. An entry with a IPv4 address of 0.0.0.0 is considered a default route. 
            /// </summary>
            public UInt32 dwForwardDest;
            /// <summary>
            /// The IPv4 subnet mask to use with the destination IPv4 address before being compared to the value in the dwForwardDest member. 
            /// </summary>
            public UInt32 dwForwardMask;
            /// <summary>
            /// The set of conditions that would cause the selection of a multi-path route (the set of next hops for a given destination). This member is typically in IP TOS format. This encoding of this member is described in RFC 1354.
            /// </summary>
            public UInt32 dwForwardPolicy;
            /// <summary>
            /// For remote routes, the IPv4 address of the next system en route. Otherwise, this member should be an IPv4 address of 0.0.0.0.
            /// </summary>
            public UInt32 dwForwardNextHop;
            /// <summary>
            /// The index of the local interface through which the next hop of this route should be reached.
            /// </summary>
            public UInt32 dwForwardIfIndex;
            /// <summary>
            /// The route type as described in RFC 1354. For more information, see http://www.ietf.org/rfc/rfc1354.txt.
            /// This member can be one of the values defined in the Iprtmib.h header file. 
            /// </summary>
            public UInt32 dwForwardType;
            /*            
                MIB_IPROUTE_TYPE_OTHER      1   Some other type not specified in RFC 1354.
                MIB_IPROUTE_TYPE_INVALID    2   An invalid route. This value can result from a route added by an ICMP redirect.
                MIB_IPROUTE_TYPE_DIRECT     3   A local route where the next hop is the final destination (a local interface).
                MIB_IPROUTE_TYPE_INDIRECT   4   The remote route where the next hop is not the final destination (a remote destination).
            */
            /// <summary>
            /// The protocol or routing mechanism that generated the route as described in RFC 1354. For more information, see http://www.ietf.org/rfc/rfc1354.txt. See Protocol Identifiers for a list of possible protocol identifiers used by routing protocols. 
            /// </summary>
            public UInt32 dwForwardProto;
            /// <summary>
            /// The number of seconds since the route was added or modified in the network routing table. 
            /// </summary>
            public UInt32 dwForwardAge;
            /// <summary>
            /// The autonomous system number of the next hop. When this member is unknown or not relevant to the protocol or routing mechanism specified in dwForwardProto, this value should be set to zero. This value is documented in RFC 1354
            /// </summary>
            public UInt32 dwForwardNextHopAS;
            /// <summary>
            /// The primary routing metric value for this route. The semantics of this metric are determined by the routing protocol specified in the dwForwardProto member. If this metric is not used, its value should be set to -1. This value is documented in in RFC 1354. For more information, see http://www.ietf.org/rfc/rfc1354.txt
            /// </summary>
            public UInt32 dwForwardMetric1;
            /// <summary>
            /// An alternate routing metric value for this route. The semantics of this metric are determined by the routing protocol specified in the dwForwardProto member. If this metric is not used, its value should be set to -1. This value is documented in RFC 1354. For more information, see http://www.ietf.org/rfc/rfc1354.txt
            /// </summary>
            public UInt32 dwForwardMetric2;
            /// <summary>
            /// An alternate routing metric value for this route. The semantics of this metric are determined by the routing protocol specified in the dwForwardProto member. If this metric is not used, its value should be set to -1. This value is documented in RFC 1354. For more information, see http://www.ietf.org/rfc/rfc1354.txt
            /// </summary>
            public UInt32 dwForwardMetric3;
            /// <summary>
            /// An alternate routing metric value for this route. The semantics of this metric are determined by the routing protocol specified in the dwForwardProto member. If this metric is not used, its value should be set to -1. This value is documented in RFC 1354. For more information, see http://www.ietf.org/rfc/rfc1354.txt
            /// </summary>
            public UInt32 dwForwardMetric4;
            /// <summary>
            /// An alternate routing metric value for this route. The semantics of this metric are determined by the routing protocol specified in the dwForwardProto member. If this metric is not used, its value should be set to -1. This value is documented in RFC 1354. For more information, see http://www.ietf.org/rfc/rfc1354.txt
            /// </summary>
            public UInt32 dwForwardMetric5;
        }

#if !PocketPC
		[DllImport("iphlpapi.dll",SetLastError=true)]
		public extern static int AllocateAndGetTcpExTableFromStack(ref IntPtr pTable, bool bOrder, IntPtr heap ,int zero,int flags );

		[DllImport("iphlpapi.dll",SetLastError=true)]
		public extern static int AllocateAndGetUdpExTableFromStack(ref IntPtr pTable, bool bOrder, IntPtr heap,int zero,int flags );
#endif
        [DllImport("coredll.dll", SetLastError = true)]
        public static extern IntPtr GetProcessHeap();

        [DllImport("coredll.dll", SetLastError = true)]
        private static extern int FormatMessage(int flags, IntPtr source, int messageId,
            int languageId, StringBuilder buffer, int size, IntPtr arguments);


        public static string GetAPIErrorMessageDescription(int ApiErrNumber)
        {
            System.Text.StringBuilder sError = new System.Text.StringBuilder(512);
            int lErrorMessageLength;
            lErrorMessageLength = FormatMessage(FORMAT_MESSAGE_FROM_SYSTEM, (IntPtr)0, ApiErrNumber, 0, sError, sError.Capacity, (IntPtr)0);

            if (lErrorMessageLength > 0)
            {
                string strgError = sError.ToString();
                strgError = strgError.Substring(0, strgError.Length - 2);
                return strgError + " (" + ApiErrNumber.ToString() + ")";
            }
            return "none";

        }
    }

    public class IPHelper
    {
        private const int NO_ERROR = 0;
        private const int MIB_TCP_STATE_CLOSED = 1;
        private const int MIB_TCP_STATE_LISTEN = 2;
        private const int MIB_TCP_STATE_SYN_SENT = 3;
        private const int MIB_TCP_STATE_SYN_RCVD = 4;
        private const int MIB_TCP_STATE_ESTAB = 5;
        private const int MIB_TCP_STATE_FIN_WAIT1 = 6;
        private const int MIB_TCP_STATE_FIN_WAIT2 = 7;
        private const int MIB_TCP_STATE_CLOSE_WAIT = 8;
        private const int MIB_TCP_STATE_CLOSING = 9;
        private const int MIB_TCP_STATE_LAST_ACK = 10;
        private const int MIB_TCP_STATE_TIME_WAIT = 11;
        private const int MIB_TCP_STATE_DELETE_TCB = 12;

        private const int ERROR_INSUFFICIENT_BUFFER = 122;

        /*
         * Tcp Struct
         * */
        public IpHlpApidotnet.MIB_TCPTABLE TcpConnexion;
        public IpHlpApidotnet.MIB_TCPSTATS TcpStats;
#if !PocketPC
        public IpHlpApidotnet.MIB_EXTCPTABLE TcpExConnexions;
#endif

        /*
         * Udp Struct
         * */
        public IpHlpApidotnet.MIB_UDPSTATS UdpStats;
        public IpHlpApidotnet.MIB_UDPTABLE UdpConnexion;
#if !PocketPC
        public IpHlpApidotnet.MIB_EXUDPTABLE UdpExConnexion;
#endif

        public static string[] RtoAlgorithmStr = new string[] { 
            "none",                     //0
            "Other",                    //1
            "Constant Time-out",
            "MIL-STD-1778 Appendix B",
            "Van Jacobson's Algorithm",
        };

        public IPHelper()
        {
            _routeEntry = new System.Collections.Generic.List<IPHlpAPI32Wrapper.MIB_IPFORWARDROW>();
        }

        #region ROUTING
        //helper
        class adapterIDs
        {
            AdapterInfo.AdaptersInfo aInfo = new AdapterInfo.AdaptersInfo();
            List<ID2IP> ipList= new List<ID2IP>();
            public class ID2IP
            {
                public int id;
                public IPAddress ip;
                public ID2IP(int i, IPAddress p)
                {
                    this.id = i;
                    this.ip = p;
                }
            }
            public adapterIDs()
            {
                foreach (AdapterInfo.IP_ADAPTER_INFO info in aInfo._adapterList)
                {
                    ipList.Add(new ID2IP((int)info.Index, info.CurrentIpAddress.IpAddress.ipAddress));
                }
            }
            public string getIPstrForID(int id)
            {
                string s = id.ToString("000");
                foreach (ID2IP idip in this.ipList)
                {
                    if (idip.id == id)
                    {
                        s = idip.ip.ToString();
                        break;
                    }
                }
                return s;
            }
        }

        AdapterInfo.AdaptersInfo aInfo = new AdapterInfo.AdaptersInfo();

        public System.Collections.Generic.List<IPHlpAPI32Wrapper.MIB_IPFORWARDROW> _routeEntry;
        public void getRoutingTable()
        {

            _routeEntry.Clear();

            IPHlpAPI32Wrapper.MIB_IPFORWARDTABLE fTable = new IPHlpAPI32Wrapper.MIB_IPFORWARDTABLE();
            fTable.table = new IPHlpAPI32Wrapper.MIB_IPFORWARDROW[1];

            int MIB_SIZE = Marshal.SizeOf(new IPHlpAPI32Wrapper.MIB_IPFORWARDROW()) + Marshal.SizeOf(new UInt32());
            // Initialize unmanged memory to hold one struct entry.
            IntPtr pTable = Marshal.AllocHGlobal(MIB_SIZE);
            try
            {

                // Copy the struct to unmanaged memory.
                // Marshal.StructureToPtr(fTable, pTable, false);   //StructureToPtr does not like array inside struct
                byte[] buffer = new byte[MIB_SIZE];

                int dwSize = MIB_SIZE;
                int iRes = IPHlpAPI32Wrapper.GetIpForwardTable(buffer, ref dwSize, true);
                if (iRes == ERROR_INSUFFICIENT_BUFFER)
                {
                    //need to get dwSize memory allocated
                    buffer = new byte[dwSize];
                    iRes = IPHlpAPI32Wrapper.GetIpForwardTable(buffer, ref dwSize, true);
                    //now we have a list of structs

                    //see also http://www.pcreview.co.uk/forums/changing-route-table-iphlpapi-dll-and-p-invoke-t2891061.html
                    //now we have the buffer filled, create an empty entry
                    IPHlpAPI32Wrapper.MIB_IPFORWARDTABLE ipForwardTable = new IPHlpAPI32Wrapper.MIB_IPFORWARDTABLE();

                    // ...and fill the MIB_IPFORWARDTABLE with the bytes from the buffer
                    int nOffset = 0;
                    // number of rows in the table
                    ipForwardTable.dwNumEntries = Convert.ToUInt32(buffer[nOffset]);
                    nOffset += 4;

                    ipForwardTable.table = new IPHlpAPI32Wrapper.MIB_IPFORWARDROW[ipForwardTable.dwNumEntries];

                    for (int i = 0; i < ipForwardTable.dwNumEntries; i++)
                    {
                        //dwForwardDest
                        //((MIB_IPFORWARDROW)(ipForwardTable.table[i])).dwForwardDest = Convert.ToUInt32(buffer[nOffset]);
                        ((ipForwardTable.table[i])).dwForwardDest = toIPInt(buffer, nOffset);
                        nOffset += 4;

                        //dwForwardMask
                        ((ipForwardTable.table[i])).dwForwardMask = toIPInt(buffer, nOffset);
                        nOffset += 4;
                        //dwForwardPolicy
                        ((ipForwardTable.table[i])).dwForwardPolicy = Convert.ToUInt32(buffer[nOffset]);
                        nOffset += 4;
                        //dwForwardNextHop
                        ((ipForwardTable.table[i])).dwForwardNextHop = toIPInt(buffer, nOffset);
                        nOffset += 4;
                        //dwForwardIfIndex
                        ((ipForwardTable.table[i])).dwForwardIfIndex = Convert.ToUInt32(buffer[nOffset]);
                        nOffset += 8; //8 since we're skipping the next item (dwForwardType)
                        //dwForwardProto
                        ((ipForwardTable.table[i])).dwForwardProto = Convert.ToUInt32(buffer[nOffset]);
                        nOffset += 4;

                        _routeEntry.Add(ipForwardTable.table[i]);
                    }

                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception: " + ex.Message);
            }
            finally
            {
                Marshal.FreeHGlobal(pTable);
            }
        }

        private UInt32 toIPInt(byte[] bytes, int startOffset)
        {
            uint ipInt = 0;
            for (int i = 3; i >= 0; i--)
            {
                ipInt += (uint)(bytes[startOffset + i] << (8 * i));
            }
            return Convert.ToUInt32(ipInt);
        }

        public string dumpRouteEntry(IPHlpAPI32Wrapper.MIB_IPFORWARDROW row){
            adapterIDs idList = new adapterIDs();

            //Active Routes:
            //         1         2         3         4         5         6         7         8
            //1234567890123456789012345678901234567890123456789012345678901234567890123456789012
            //Network Destination        Netmask          Gateway          Interface           Metric
            string s = "";

            s += (new IPAddress(row.dwForwardDest)).ToString().PadRight(27);
            s += (new IPAddress(row.dwForwardMask)).ToString().PadRight(17);
            s += (new IPAddress(row.dwForwardNextHop)).ToString().PadRight(17);

            //interface
            //s += "".PadLeft(6) + row.dwForwardIfIndex.ToString("000") + "     ";
            s += "".PadLeft(2) + idList.getIPstrForID((int)row.dwForwardIfIndex).PadLeft(17) ;
            //metric
            s += row.dwForwardMetric1.ToString().PadLeft(5);

            return s;
        }
        #endregion

        #region Tcp Function

        public void GetTcpStats()
        {
            TcpStats = new MIB_TCPSTATS();
            IPHlpAPI32Wrapper.GetTcpStatistics(ref TcpStats);
        }

#if !PocketPC
        public void GetExTcpConnexions()
        {

            // the size of the MIB_EXTCPROW struct =  6*DWORD
            int rowsize = 24;
            int BufferSize = 100000;
            // allocate a dumb memory space in order to retrieve  nb of connexion
            IntPtr lpTable = Marshal.AllocHGlobal(BufferSize);
            //getting infos
            int res = IPHlpAPI32Wrapper.AllocateAndGetTcpExTableFromStack(ref lpTable, true, IPHlpAPI32Wrapper.GetProcessHeap(), 0, 2);
            if (res != NO_ERROR)
            {
                Debug.WriteLine("Erreur : " + IPHlpAPI32Wrapper.GetAPIErrorMessageDescription(res) + " " + res);
                return; // Error. You should handle it
            }
            int CurrentIndex = 0;
            //get the number of entries in the table
            int NumEntries = (int)Marshal.ReadIntPtr(lpTable);
            lpTable = IntPtr.Zero;
            // free allocated space in memory
            Marshal.FreeHGlobal(lpTable);

            ///////////////////
            // calculate the real buffer size nb of entrie * size of the struct for each entrie(24) + the dwNumEntries
            BufferSize = (NumEntries * rowsize) + 4;
            // make the struct to hold the resullts
            TcpExConnexions = new IpHlpApidotnet.MIB_EXTCPTABLE();
            // Allocate memory
            lpTable = Marshal.AllocHGlobal(BufferSize);
            res = IPHlpAPI32Wrapper.AllocateAndGetTcpExTableFromStack(ref lpTable, true, IPHlpAPI32Wrapper.GetProcessHeap(), 0, 2);
            if (res != NO_ERROR)
            {
                Debug.WriteLine("Erreur : " + IPHlpAPI32Wrapper.GetAPIErrorMessageDescription(res) + " " + res);
                return; // Error. You should handle it
            }
            // New pointer of iterating throught the data
            IntPtr current = lpTable;
            CurrentIndex = 0;
            // get the (again) the number of entries
            NumEntries = (int)Marshal.ReadIntPtr(current);
            TcpExConnexions.dwNumEntries = NumEntries;
            // Make the array of entries
            TcpExConnexions.table = new MIB_EXTCPROW[NumEntries];
            // iterate the pointer of 4 (the size of the DWORD dwNumEntries)
            CurrentIndex += 4;
            current = (IntPtr)((int)current + CurrentIndex);
            // for each entries
            for (int i = 0; i < NumEntries; i++)
            {

                // The state of the connexion (in string)
                TcpExConnexions.table[i].StrgState = this.convert_state((int)Marshal.ReadIntPtr(current));
                // The state of the connexion (in ID)
                TcpExConnexions.table[i].iState = (int)Marshal.ReadIntPtr(current);
                // iterate the pointer of 4
                current = (IntPtr)((int)current + 4);
                // get the local address of the connexion
                UInt32 localAddr = (UInt32)Marshal.ReadIntPtr(current);
                // iterate the pointer of 4
                current = (IntPtr)((int)current + 4);
                // get the local port of the connexion
                UInt32 localPort = (UInt32)Marshal.ReadIntPtr(current);
                // iterate the pointer of 4
                current = (IntPtr)((int)current + 4);
                // Store the local endpoint in the struct and convertthe port in decimal (ie convert_Port())
                TcpExConnexions.table[i].Local = new IPEndPoint(localAddr, (int)convert_Port(localPort));
                // get the remote address of the connexion
                UInt32 RemoteAddr = (UInt32)Marshal.ReadIntPtr(current);
                // iterate the pointer of 4
                current = (IntPtr)((int)current + 4);
                UInt32 RemotePort = 0;
                // if the remote address = 0 (0.0.0.0) the remote port is always 0
                // else get the remote port
                if (RemoteAddr != 0)
                {
                    RemotePort = (UInt32)Marshal.ReadIntPtr(current);
                    RemotePort = convert_Port(RemotePort);
                }
                current = (IntPtr)((int)current + 4);
                // store the remote endpoint in the struct  and convertthe port in decimal (ie convert_Port())
                TcpExConnexions.table[i].Remote = new IPEndPoint(RemoteAddr, (int)RemotePort);
                // store the process ID
                TcpExConnexions.table[i].dwProcessId = (int)Marshal.ReadIntPtr(current);
                // Store and get the process name in the struct
                TcpExConnexions.table[i].ProcessName = this.get_process_name(TcpExConnexions.table[i].dwProcessId);
                current = (IntPtr)((int)current + 4);

            }
            // free the buffer
            Marshal.FreeHGlobal(lpTable);
            // re init the pointer
            current = IntPtr.Zero;
        }
#endif //PocketPC

        public void GetTcpConnexions()
        {
            byte[] buffer = new byte[20000]; // Start with 20.000 bytes left for information about tcp table
            int pdwSize = 20000;
            int res = IPHlpAPI32Wrapper.GetTcpTable(buffer, out pdwSize, true);
            if (res != NO_ERROR)
            {
                buffer = new byte[pdwSize];
                res = IPHlpAPI32Wrapper.GetTcpTable(buffer, out pdwSize, true);
                if (res != 0)
                    return;     // Error. You should handle it
            }

            TcpConnexion = new IpHlpApidotnet.MIB_TCPTABLE();

            int nOffset = 0;
            // number of entry in the
            TcpConnexion.dwNumEntries = Convert.ToInt32(buffer[nOffset]);
            nOffset += 4;
            TcpConnexion.table = new MIB_TCPROW[TcpConnexion.dwNumEntries];

            for (int i = 0; i < TcpConnexion.dwNumEntries; i++)
            {
                // state
                int st = Convert.ToInt32(buffer[nOffset]);
                // state in string
                //((MIB_TCPROW)
                (TcpConnexion.table[i]).StrgState = convert_state(st);
                // state  by ID
                //((MIB_TCPROW)
                (TcpConnexion.table[i]).iState = st;
                nOffset += 4;
                // local address
                string LocalAdrr = buffer[nOffset].ToString() + "." + buffer[nOffset + 1].ToString() + "." + buffer[nOffset + 2].ToString() + "." + buffer[nOffset + 3].ToString();
                nOffset += 4;
                //local port in decimal
                int LocalPort = (((int)buffer[nOffset]) << 8) + (((int)buffer[nOffset + 1])) +
                    (((int)buffer[nOffset + 2]) << 24) + (((int)buffer[nOffset + 3]) << 16);

                nOffset += 4;
                // store the remote endpoint
                //((MIB_TCPROW)(
                (TcpConnexion.table[i]).Local = new IPEndPoint(IPAddress.Parse(LocalAdrr), LocalPort);

                // remote address
                string RemoteAdrr = buffer[nOffset].ToString() + "." + buffer[nOffset + 1].ToString() + "." + buffer[nOffset + 2].ToString() + "." + buffer[nOffset + 3].ToString();
                nOffset += 4;
                // if the remote address = 0 (0.0.0.0) the remote port is always 0
                // else get the remote port in decimal
                int RemotePort;
                //
                if (RemoteAdrr == "0.0.0.0")
                {
                    RemotePort = 0;
                }
                else
                {
                    RemotePort = (((int)buffer[nOffset]) << 8) + (((int)buffer[nOffset + 1])) +
                        (((int)buffer[nOffset + 2]) << 24) + (((int)buffer[nOffset + 3]) << 16);
                }
                nOffset += 4;
                //((MIB_TCPROW)
                (TcpConnexion.table[i]).Remote = new IPEndPoint(IPAddress.Parse(RemoteAdrr), RemotePort);
            }
        }


        #endregion

        #region Udp Functions

        public void GetUdpStats()
        {

            UdpStats = new MIB_UDPSTATS();
            IPHlpAPI32Wrapper.GetUdpStatistics(ref UdpStats);
        }


        public void GetUdpConnexions()
        {
            byte[] buffer = new byte[20000]; // Start with 20.000 bytes left for information about tcp table
            int pdwSize = 20000;
            int res = IPHlpAPI32Wrapper.GetUdpTable(buffer, out pdwSize, true);
            if (res != NO_ERROR)
            {
                buffer = new byte[pdwSize];
                res = IPHlpAPI32Wrapper.GetUdpTable(buffer, out pdwSize, true);
                if (res != 0)
                    return;     // Error. You should handle it
            }

            UdpConnexion = new IpHlpApidotnet.MIB_UDPTABLE();

            int nOffset = 0;
            // number of entry in the
            UdpConnexion.dwNumEntries = Convert.ToInt32(buffer[nOffset]);
            nOffset += 4;
            UdpConnexion.table = new MIB_UDPROW[UdpConnexion.dwNumEntries];
            for (int i = 0; i < UdpConnexion.dwNumEntries; i++)
            {
                string LocalAdrr = buffer[nOffset].ToString() + "." + buffer[nOffset + 1].ToString() + "." + buffer[nOffset + 2].ToString() + "." + buffer[nOffset + 3].ToString();
                nOffset += 4;

                int LocalPort = (((int)buffer[nOffset]) << 8) + (((int)buffer[nOffset + 1])) +
                    (((int)buffer[nOffset + 2]) << 24) + (((int)buffer[nOffset + 3]) << 16);
                nOffset += 4;
                //((MIB_UDPROW)
                (UdpConnexion.table[i]).Local = new IPEndPoint(IPAddress.Parse(LocalAdrr), LocalPort);
            }
        }

#if !PocketPC
        public void GetExUdpConnexions()
        {

            // the size of the MIB_EXTCPROW struct =  4*DWORD
            int rowsize = 12;
            int BufferSize = 100000;
            // allocate a dumb memory space in order to retrieve  nb of connexion
            IntPtr lpTable = Marshal.AllocHGlobal(BufferSize);
            //getting infos
            int res = IPHlpAPI32Wrapper.AllocateAndGetUdpExTableFromStack(ref lpTable, true, IPHlpAPI32Wrapper.GetProcessHeap(), 0, 2);
            if (res != NO_ERROR)
            {
                Debug.WriteLine("Erreur : " + IPHlpAPI32Wrapper.GetAPIErrorMessageDescription(res) + " " + res);
                return; // Error. You should handle it
            }
            int CurrentIndex = 0;
            //get the number of entries in the table
            int NumEntries = (int)Marshal.ReadIntPtr(lpTable);
            lpTable = IntPtr.Zero;
            // free allocated space in memory
            Marshal.FreeHGlobal(lpTable);

            ///////////////////
            // calculate the real buffer size nb of entrie * size of the struct for each entrie(24) + the dwNumEntries
            BufferSize = (NumEntries * rowsize) + 4;
            // make the struct to hold the resullts
            UdpExConnexion = new IpHlpApidotnet.MIB_EXUDPTABLE();
            // Allocate memory
            lpTable = Marshal.AllocHGlobal(BufferSize);
            res = IPHlpAPI32Wrapper.AllocateAndGetUdpExTableFromStack(ref lpTable, true, IPHlpAPI32Wrapper.GetProcessHeap(), 0, 2);
            if (res != NO_ERROR)
            {
                Debug.WriteLine("Erreur : " + IPHlpAPI32Wrapper.GetAPIErrorMessageDescription(res) + " " + res);
                return; // Error. You should handle it
            }
            // New pointer of iterating throught the data
            IntPtr current = lpTable;
            CurrentIndex = 0;
            // get the (again) the number of entries
            NumEntries = (int)Marshal.ReadIntPtr(current);
            UdpExConnexion.dwNumEntries = NumEntries;
            // Make the array of entries
            UdpExConnexion.table = new MIB_EXUDPROW[NumEntries];
            // iterate the pointer of 4 (the size of the DWORD dwNumEntries)
            CurrentIndex += 4;
            current = (IntPtr)((int)current + CurrentIndex);
            // for each entries
            for (int i = 0; i < NumEntries; i++)
            {
                // get the local address of the connexion
                UInt32 localAddr = (UInt32)Marshal.ReadIntPtr(current);
                // iterate the pointer of 4
                current = (IntPtr)((int)current + 4);
                // get the local port of the connexion
                UInt32 localPort = (UInt32)Marshal.ReadIntPtr(current);
                // iterate the pointer of 4
                current = (IntPtr)((int)current + 4);
                // Store the local endpoint in the struct and convertthe port in decimal (ie convert_Port())
                UdpExConnexion.table[i].Local = new IPEndPoint(localAddr, convert_Port(localPort));
                // store the process ID
                UdpExConnexion.table[i].dwProcessId = (int)Marshal.ReadIntPtr(current);
                // Store and get the process name in the struct
                UdpExConnexion.table[i].ProcessName = this.get_process_name(UdpExConnexion.table[i].dwProcessId);
                current = (IntPtr)((int)current + 4);

            }
            // free the buffer
            Marshal.FreeHGlobal(lpTable);
            // re init the pointer
            current = IntPtr.Zero;
        }
#endif //PocketPC

        #endregion

        #region helper fct

        public static string dump_MIB_TCPROW(MIB_TCPROW mib)
        {
            string s = "n/a";
            s = dumpEndPoint(mib.Local) + " " + dumpEndPoint(mib.Remote) + " " + mib.StrgState;
            return s;
        }

        public static string dump_MIB_UDPROW(MIB_UDPROW mib)
        {
            string s = "n/a";
            s = dumpEndPoint(mib.Local);
            return s;
        }

        public static string dump_MIB_TCPSTATS(MIB_TCPSTATS mib)
        {
            string s = "";
            //retransmission timeout (RTO)
            s += "Retransmission timeout (min/max): ".PadLeft(34) + IPHelper.RtoAlgorithmStr[mib.dwRtoAlgorithm];
            s += ": " + mib.dwRtoMin.ToString() + "/" + mib.dwRtoMax.ToString();

            //max connections
            s += "\r\n" + "max connnections: ".PadLeft(34) + mib.dwMaxConn.ToString();

            //active open connection
            s += "\r\n" + "active open: ".PadLeft(34) + mib.dwActiveOpens.ToString();
            //passive open connection
            s += "\r\n" + "passive open: ".PadLeft(34) + mib.dwPassiveOpens.ToString();

            //failed attempts
            s += "\r\n" + "failed attempts: ".PadLeft(34) + mib.dwAttemptFails.ToString();

            //established resets
            s += "\r\n" + "established resets: ".PadLeft(34) + mib.dwEstabResets.ToString();

            //current established
            s += "\r\n" + "current established: ".PadLeft(34) + mib.dwCurrEstab.ToString();

            //data segs in
            s += "\r\n" + "segments in: ".PadLeft(34) + mib.dwInSegs.ToString();

            //data segs out
            s += "\r\n" + "segments out: ".PadLeft(34) + mib.dwOutSegs.ToString();

            //data segs re-transmitted
            s += "\r\n" + "retransmitted segments: ".PadLeft(34) + mib.dwRetransSegs.ToString();

            //errors received
            s += "\r\n" + "in errors: ".PadLeft(34) + mib.dwInErrs.ToString();

            //out resets
            s += "\r\n" + "out resets: ".PadLeft(34) + mib.dwOutRsts.ToString();

            //num connections
            s += "\r\n" + "num connections: ".PadLeft(34) + mib.dwNumConns.ToString();

            return s;
        }

        public static string dump_MIB_UDPSTATS(MIB_UDPSTATS mib)
        {
            string s = "";
            //
            s += "in datagrams: ".PadLeft(34) + mib.dwInDatagrams.ToString();
            s += "\r\n" + "in errors: ".PadLeft(34) + mib.dwInErrors.ToString();
            s += "\r\n" + "num ports: ".PadLeft(34) + mib.dwNoPorts.ToString();
            s += "\r\n" + "num addresses: ".PadLeft(34) + mib.dwNumAddrs.ToString();
            s += "\r\n" + "out datagrams: ".PadLeft(34) + mib.dwOutDatagrams.ToString();
            return s;
        }

        static string dumpEndPoint(IPEndPoint addr)
        {
            string s = "";
            s = addr.Address.ToString().PadLeft(15) + ":" + addr.Port.ToString().PadLeft(6);
            return s;
        }

        private UInt16 convert_Port(UInt32 dwPort)
        {
            byte[] b = new Byte[2];
            // high weight byte
            b[0] = byte.Parse((dwPort >> 8).ToString());
            // low weight byte
            b[1] = byte.Parse((dwPort & 0xFF).ToString());
            return BitConverter.ToUInt16(b, 0);
        }


        private string convert_state(int state)
        {
            string strg_state = "";
            switch (state)
            {
                case MIB_TCP_STATE_CLOSED: strg_state = "CLOSED"; break;
                case MIB_TCP_STATE_LISTEN: strg_state = "LISTEN"; break;
                case MIB_TCP_STATE_SYN_SENT: strg_state = "SYN_SENT"; break;
                case MIB_TCP_STATE_SYN_RCVD: strg_state = "SYN_RCVD"; break;
                case MIB_TCP_STATE_ESTAB: strg_state = "ESTAB"; break;
                case MIB_TCP_STATE_FIN_WAIT1: strg_state = "FIN_WAIT1"; break;
                case MIB_TCP_STATE_FIN_WAIT2: strg_state = "FIN_WAIT2"; break;
                case MIB_TCP_STATE_CLOSE_WAIT: strg_state = "CLOSE_WAIT"; break;
                case MIB_TCP_STATE_CLOSING: strg_state = "CLOSING"; break;
                case MIB_TCP_STATE_LAST_ACK: strg_state = "LAST_ACK"; break;
                case MIB_TCP_STATE_TIME_WAIT: strg_state = "TIME_WAIT"; break;
                case MIB_TCP_STATE_DELETE_TCB: strg_state = "DELETE_TCB"; break;
            }
            return strg_state;
        }


        private string get_process_name(int processID)
        {
            //could be an error here if the process die before we can get his name
            try
            {
                Process p = Process.GetProcessById((int)processID);
                return p.StartInfo.FileName;// ProcessName;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return "unknown";
            }

        }
        #endregion
    }
}

