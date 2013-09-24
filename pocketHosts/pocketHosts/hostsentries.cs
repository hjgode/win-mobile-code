using System;

using System.Collections.Generic;
using System.Text;

using Microsoft.Win32;
using System.Collections;

using System.Security;
using System.Security.Permissions;

namespace pocketHosts
{
    public class hostsentries:IDisposable
    {
        const string regSubKey = @"Comm\Tcpip\Hosts";
        RegistryKey rKeyTCPIP;
        public Dictionary<string, hostsentry> allHosts;
        public hostsentries()
        {
            allHosts = new Dictionary<string, hostsentry>();
            init();
        }
        private void init()
        {
            try
            {
                rKeyTCPIP = Registry.LocalMachine.OpenSubKey(regSubKey, true);
                if (rKeyTCPIP == null)
                    System.Diagnostics.Debug.WriteLine("Error opening reg key: " + regSubKey);
                loadRegistry();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception opening reg key: " + regSubKey + ", " + ex.Message);
            }
        }
        
        public void Dispose()
        {
            try
            {
                rKeyTCPIP.Flush();
                rKeyTCPIP.Close();
                rKeyTCPIP = null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception in Dispose(): " + ex.Message);
            }
        }

        public int removeEntry(hostsentry he)
        {
            int iRet =0;
            try
            {
                string[] lVals = rKeyTCPIP.OpenSubKey(he.sHost, true).GetValueNames();
                foreach (string s in lVals)
                {
                    rKeyTCPIP.DeleteValue(s);
                }
                try
                {
                    rKeyTCPIP.DeleteValue("");//delete default value
                }
                catch (Exception) { }
                rKeyTCPIP.Close();
                rKeyTCPIP = null;
                if (myregistry.RegDelKey(regSubKey, he.sHost) == 0)
                {
                    iRet = 1;
                    this.allHosts.Remove(he.sHost);
                }
                rKeyTCPIP = Registry.LocalMachine.OpenSubKey(regSubKey, true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception in remove(): " + ex.Message);
            }
            return iRet;
        }

        public int saveAll(){
            int iRet = 0;
            foreach (hostsentry he in this.allHosts.Values)
            {
                saveEntry(he);
                iRet++;
            }
            return iRet;
        }

        public int saveEntry(hostsentry he)
        {
            int iRet = 0;
            RegistryKey regWork=null;
            //create subkey if not exist
            try
            {
                regWork = rKeyTCPIP.CreateSubKey(he.sHost);
                List<byte> b = new List<byte>();
                foreach (System.Net.IPAddress ip in he.ipAddress)
                {
                    b.AddRange(ip.GetAddressBytes());   //can be ip4 or ip6
                    iRet++;
                }
                if(b.Count>=4)
                    regWork.SetValue("ipaddr", b.ToArray(), RegistryValueKind.Binary);

                b.Clear();
                foreach (System.Net.IPAddress ip6 in he.ipAddress6)
                {
                    b.AddRange(ip6.GetAddressBytes());   //can be ip4 or ip6
                    iRet++;
                }
                if (b.Count >= 4)
                    regWork.SetValue("ipaddr6", b.ToArray(), RegistryValueKind.Binary);

                if(he.aliases.Count>0)
                    regWork.SetValue("aliases", he.aliases.ToArray(), RegistryValueKind.MultiString);

                if(he.expireTime!=0)
                    regWork.SetValue("expireTime", BitConverter.GetBytes(he.expireTime), RegistryValueKind.Binary);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception in saveEntry(): " + ex.Message);
            }
            return iRet;
        }

        int loadRegistry()
        {
            int iRet = 0;
            string[] subKeys = rKeyTCPIP.GetSubKeyNames();
            allHosts.Clear();
            foreach (string sHost in subKeys)
            {
                hostsentry hEntry = new hostsentry();
                hEntry.sHost = sHost;

                if (existsValue(sHost, "Aliases"))
                {
                    string[] sAliases = (string[])rKeyTCPIP.OpenSubKey(sHost).GetValue("Aliases", null);
                    if (sAliases != null)
                    {
                        foreach(string s in sAliases)
                            hEntry.aliases.Add(s);
                    }
                }

                hEntry.expireTime = 0; //do not save!
                if (existsValue(sHost, "ExpireTime"))
                {
                    byte[] bExpires = (byte[])rKeyTCPIP.OpenSubKey(sHost).GetValue("ExpireTime ", null);
                    if (bExpires != null)
                    {
                        ulong ul = BitConverter.ToUInt64(bExpires, 0);
                        hEntry.expireTime = ul;
                    }
                }

                if (existsValue(sHost, "ipAddr"))
                {
                    byte[] bIpAddress4 = (byte[])rKeyTCPIP.OpenSubKey(sHost).GetValue("ipAddr", null);
                    if (bIpAddress4 != null && bIpAddress4.Length >= 4)
                    {
                        if (bIpAddress4.Length > 4)
                        {
                            for (int offset = 0; offset < bIpAddress4.Length; offset += 4)
                            {
                                byte[] bIp = new byte[4];
                                Array.Copy(bIpAddress4, offset, bIp, 0, 4);
                                System.Net.IPAddress ip = System.Net.IPAddress.Parse(
                                    bIp[0].ToString() + "." +
                                    bIp[1].ToString() + "." +
                                    bIp[2].ToString() + "." +
                                    bIp[3].ToString());
                                hEntry.ipAddress.Add(ip);
                            }
                        }
                        else
                        {
                            System.Net.IPAddress ip = System.Net.IPAddress.Parse(
                                bIpAddress4[0].ToString() + "." +
                                bIpAddress4[1].ToString() + "." +
                                bIpAddress4[2].ToString() + "." +
                                bIpAddress4[3].ToString());
                            hEntry.ipAddress.Add(ip);
                        }
                    }
                }

                //ip6 address is 16 bytes
                // ie 2001:0db8:ac10:fe01:0000:0000:0000:0000
                if (existsValue(sHost, "ipAddr6"))
                {
                    byte[] bIpAddress6 = (byte[])rKeyTCPIP.OpenSubKey(sHost).GetValue("ipAddr6", null);
                    if (bIpAddress6 != null && bIpAddress6.Length >= 16)
                    {
                        if (bIpAddress6.Length > 16)
                        {
                            for (int offset = 0; offset < bIpAddress6.Length; offset += 16)
                            {
                                byte[] bIp = new byte[16];
                                Array.Copy(bIpAddress6, offset, bIp, 0, 16);
                                System.Net.IPAddress ip6 = getIP6fromByte(bIp);
                                if (ip6 != System.Net.IPAddress.None)
                                    hEntry.ipAddress6.Add(ip6);
                            }
                        }
                        else
                        {
                            System.Net.IPAddress ip6 = getIP6fromByte(bIpAddress6);
                            if(ip6!=System.Net.IPAddress.None)
                                hEntry.ipAddress6.Add(ip6);
                        }
                    }
                }

                this.allHosts.Add(hEntry.sHost, hEntry);
            }
            
            
            return iRet;
        }

        //convert 20010db8ac10fe010000000000000000
        //to 2001:0db8:ac10:fe01:0000:0000:0000:0000
        private string getIP6StrfromByte(byte[] bIpAddress6)
        {
            if (bIpAddress6.Length != 16)
                return "";
            string sIP6 = "";
            for (int i = 0; i < 16; i++)
            {
                byte b = bIpAddress6[i];
                sIP6 += b.ToString("x02");
                if (i > 0 && i % 2 != 0 && i != 15)
                    sIP6 += ":";
            }
            return sIP6;
        }

        private System.Net.IPAddress getIP6fromByte(byte[] bIpAddress6)
        {
            System.Net.IPAddress ip6 = new System.Net.IPAddress((long)0);
            string sIP = getIP6StrfromByte(bIpAddress6);
            if (sIP != "")
            {
                try{
                    ip6 = System.Net.IPAddress.Parse(sIP);
                }
                catch(Exception){}
            }
            return ip6;
        }

        /// <summary>
        /// add or change host entry
        /// </summary>
        /// <param name="he"></param>
        /// <returns>true for added a new entry
        /// false if existing entry is replaced</returns>
        public bool addChangeEntry(hostsentry he)
        {
            bool bRet = false;
            hostsentry hTest = new hostsentry();
            if (this.allHosts.TryGetValue(he.sHost, out hTest))
            {
                //there is already an entry
                this.allHosts.Remove(he.sHost);
                //replace old entry by new one
                this.allHosts.Add(he.sHost, he);
                bRet = false;
            }
            else
            {
                this.allHosts.Add(he.sHost, he);
                bRet = true;
            }
            return bRet;
        }

        public hostsentry getEntry(string sHost)
        {
            hostsentry he = new hostsentry();
            //test if entry exists
            if (this.allHosts.TryGetValue(sHost, out he))
                return he;
            return he;
        }

        /// <summary>
        /// check registry for existence of a reg key value
        /// </summary>
        /// <param name="sKey"></param>
        /// <param name="sVal"></param>
        /// <returns></returns>
        bool existsValue(string sKey, string sVal)
        {
            bool bRet = false;
            try
            {
                object o = rKeyTCPIP.OpenSubKey(sKey).GetValue(sVal, null);
                if (o != null)
                    bRet = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception in existsValue(): " + ex.Message);
            }
            return bRet;
        }
    }
}
