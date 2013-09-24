using System;

using System.Collections.Generic;
using System.Text;

namespace pocketHosts
{
    public class hostsentry
    {

        //##################################################################
        public string sHost { get; set; }
        public List<System.Net.IPAddress> ipAddress { get; set; }
        public List<System.Net.IPAddress> ipAddress6 { get; set; }
        public List<string> aliases { get; set; }
        private ulong _expireTime = 0;
        public ulong expireTime { get { return (uint)_expireTime; } set { _expireTime = (ulong)value; } }

        public bool isIpaddress4(System.Net.IPAddress ip)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                return true;
            else
                return false;
        }
        public hostsentry()
        {
            sHost = "";
            aliases = new List<string>();
            ipAddress = new List<System.Net.IPAddress>();
            ipAddress6 = new List<System.Net.IPAddress>();
            expireTime = 0;
        }
        public override string ToString()
        {
            return sHost;
        }
    }
}
