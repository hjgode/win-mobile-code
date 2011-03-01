using System;

using System.Collections.Generic;
using System.Text;

    public class queueData
    {
        public int _iCount=0;
        private string _sHost = "";
        public string sHost
        {
            get { return _sHost; }
            set { _sHost = value; }
        }
        public System.Net.IPAddress IP;// = System.Net.IPAddress.Parse("127.0.0.1");

        private string _sIP = "0.0.0.0";
        /// <summary>
        /// server IP or host name for ping
        /// </summary>
        public string sIP
        {
            get { return _sIP; }
            set { _sIP = value; }
        }
        private int _iPingCount = 4;
        /// <summary>
        /// how many pings to send
        /// </summary>
        public int iPingCount{
            get { return _iPingCount; }
            set { _iPingCount = value; }
        }
        private int _iPingReplies = 0;
        /// <summary>
        /// number of good ping replies
        /// </summary>
        public int iPingReplies
        {
            get { return _iPingReplies; }
            set { _iPingReplies = value; }
        }
        private int _iPingTimeout = 255;
        /// <summary>
        /// ICMP TTL, time to live for ping reply, default=255
        /// </summary>
        public int iPingTimeout
        {
            get { return _iPingTimeout; }
            set { _iPingTimeout = (byte)value; }
        }
        private int _iPingReplyTime=0;
        /// <summary>
        /// average ping reply time
        /// </summary>
        public int iPingReplyTime
        {
            get { return _iPingReplyTime; }
            set { _iPingReplyTime = value; }
        }
        public queueData()
        {
            
        }
    }
