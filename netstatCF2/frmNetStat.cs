using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Threading;
using System.IO;

namespace netstatCF2
{
    public partial class frmNetStat : Form
    {
#if DEBUG
        int iLogInterval = 3;
#else
        int iLogInterval = 30;
#endif
        Thread loggingThread = null;
        bool bStopThread = false;
        IpHlpApidotnet.IPHelper g_iphlp=null;

        static string sAppPath = "\\";
        string getAppPath
        {
            get
            {
                if (sAppPath == "\\")
                {
                    string AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
                    if (!AppPath.EndsWith(@"\"))
                        AppPath += @"\";
                    sAppPath = AppPath;
                    return sAppPath;
                }
                else
                    return sAppPath;
            }
        }

        public frmNetStat()
        {
            InitializeComponent();
            refreshScreen();
            mnuBackgroundLogging_Click(this, null);
        }
        void refreshScreen()
        {
            SuspendLayout();
            textBox1.Text = doLog();

            //IpHlpApidotnet.IPHelper iphlp = new IpHlpApidotnet.IPHelper();
            //iphlp.GetTcpConnexions();
            //System.Diagnostics.Debug.WriteLine("TCP table\n" + "local".PadRight(28) + "remote");
            //textBox1.Text += "======= TCP table ========\r\n" + "local".PadRight(28) + "remote\r\n";
            //foreach (IpHlpApidotnet.MIB_TCPROW stat in iphlp.TcpConnexion.table)
            //{
            //    //System.Diagnostics.Debug.WriteLine( stat.Local.Address.ToString() + " " +stat.Remote.Address.ToString() );
            //    System.Diagnostics.Debug.WriteLine(IpHlpApidotnet.IPHelper.dump_MIB_TCPROW(stat));
            //    textBox1.Text += IpHlpApidotnet.IPHelper.dump_MIB_TCPROW(stat) + "\r\n";
            //}

            //iphlp.GetUdpConnexions();
            //System.Diagnostics.Debug.WriteLine("UDP table\n" + "local");
            //textBox1.Text += "======= UDP table ========\r\n";
            //foreach (IpHlpApidotnet.MIB_UDPROW stat in iphlp.UdpConnexion.table)
            //{
            //    System.Diagnostics.Debug.WriteLine(IpHlpApidotnet.IPHelper.dump_MIB_UDPROW(stat));
            //    textBox1.Text += IpHlpApidotnet.IPHelper.dump_MIB_UDPROW(stat) + "\r\n";
            //}

            //iphlp.GetTcpStats();
            //System.Diagnostics.Debug.WriteLine("TCP statistics:");
            //textBox1.Text += "======= TCP statistics ========\r\n";
            //System.Diagnostics.Debug.WriteLine(IpHlpApidotnet.IPHelper.dump_MIB_TCPSTATS(iphlp.TcpStats));
            //textBox1.Text += IpHlpApidotnet.IPHelper.dump_MIB_TCPSTATS(iphlp.TcpStats) + "\r\n";

            //iphlp.GetUdpStats();
            //System.Diagnostics.Debug.WriteLine("UDP statistics:");
            //textBox1.Text += "======= UDP statistics ========\r\n";
            //System.Diagnostics.Debug.WriteLine(IpHlpApidotnet.IPHelper.dump_MIB_UDPSTATS(iphlp.UdpStats));
            //textBox1.Text += IpHlpApidotnet.IPHelper.dump_MIB_UDPSTATS(iphlp.UdpStats) + "\r\n";


            //AdapterInfo.AdaptersInfo aInfo = new AdapterInfo.AdaptersInfo();
            //System.Diagnostics.Debug.WriteLine("Adapter infos: ");
            //textBox1.Text += "======= Adapter infos ==========\r\n";
            //foreach (AdapterInfo.IP_ADAPTER_INFO info in aInfo._adapterList)
            //{
            //    System.Diagnostics.Debug.WriteLine(info.Index.ToString() + ": " + info.AdapterName + ", " + info.CurrentIpAddress.IpAddress.String);
            //    textBox1.Text += info.Index.ToString() + ": " + info.AdapterName + ", " + info.CurrentIpAddress.IpAddress.String + "\r\n";
            //}

            //iphlp.getRoutingTable();
            ////                                                              1         2         3         4         5         6         7         8
            ////                                                     12345678901234567890123456789012345678901234567890123456789012345678901234567890123
            //System.Diagnostics.Debug.WriteLine("Route entries: \r\nNetwork Destination        Netmask          Gateway          Interface           Metric");
            //textBox1.Text += "Route entries: \r\nNetwork Destination        Netmask          Gateway          Interface     Metric\r\n";
            //foreach (IpHlpApidotnet.IPHlpAPI32Wrapper.MIB_IPFORWARDROW row in iphlp._routeEntry)
            //{
            //    System.Diagnostics.Debug.WriteLine(iphlp.dumpRouteEntry(row));
            //    textBox1.Text += iphlp.dumpRouteEntry(row) + "\r\n";
            //}

            ResumeLayout();
            textBox1.Refresh();

        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }

        private void mnuRefresh_Click(object sender, EventArgs e)
        {
            refreshScreen();
        }

        private void mnuSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog ofd = new SaveFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                using (System.IO.TextWriter tw = new System.IO.StreamWriter(ofd.FileName))
                {
                    tw.WriteLine(textBox1.Text);
                    tw.Flush();
                }
                MessageBox.Show("File saved to " + ofd.FileName);
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == System.Windows.Forms.Keys.Up))
            {
                // Up
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Down))
            {
                // Down
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Left))
            {
                // Left
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Right))
            {
                // Right
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Enter))
            {
                // Enter
            }

        }

        private void mnuBackgroundLogging_Click(object sender, EventArgs e)
        {
            mnuBackgroundLogging.Checked = !mnuBackgroundLogging.Checked;
            if (mnuBackgroundLogging.Checked)
            {
                stopThread();
                startThread();
            }
            else
                stopThread();
        }

        void startThread()
        {
            stopThread();
            bStopThread = false;
            loggingThread = new Thread(loggerThread);
            loggingThread.Name = "logger thread";
            loggingThread.Start();
            System.Diagnostics.Debug.WriteLine("startThread: " + loggingThread.ManagedThreadId.ToString() + ", '" + loggingThread.Name + "'");
        }

        void stopThread()
        {
            if (loggingThread == null)
                return;
            try
            {
                bStopThread = true;
                bool bJoined = loggingThread.Join(1000);
                if (!bJoined)
                    loggingThread.Abort();
                loggingThread = null;
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine("Exception in stopThread: " + ex.Message);
            }
        }

        void loggerThread()
        {
            System.Diagnostics.Debug.WriteLine("+++++ loggerThread started");
            try
            {
                int iCount = 0;
                //log to file
                doLog();
                do{
                    //wait
                    Thread.Sleep(1000);
                    if (iCount > iLogInterval)
                    {
                        doLog();
                        iCount = 0;
                    }
                    iCount++;
                }while(!bStopThread);                
            }
            catch (ThreadAbortException ex)
            {
                System.Diagnostics.Debug.WriteLine("ThreadAbortException in loggerThread: " + ex.Message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception in loggerThread: " + ex.Message);
            }
            System.Diagnostics.Debug.WriteLine("----- loggerThread ended");
        }

        string doLog()
        {
            DateTime dt = DateTime.Now;
            string dtStr = dt.ToShortDateString() + ", " + dt.ToShortTimeString();
            if (g_iphlp == null)
                g_iphlp = new IpHlpApidotnet.IPHelper();

            string sOut = "";
            g_iphlp.GetTcpConnexions();
            sOut += "======= TCP table ========\r\n" + "local".PadRight(28) + "remote\r\n";
            foreach (IpHlpApidotnet.MIB_TCPROW stat in g_iphlp.TcpConnexion.table)
            {
                sOut += IpHlpApidotnet.IPHelper.dump_MIB_TCPROW(stat) + "\r\n";
            }

            g_iphlp.GetUdpConnexions();
            sOut += "======= UDP table ========\r\n";
            foreach (IpHlpApidotnet.MIB_UDPROW stat in g_iphlp.UdpConnexion.table)
            {
                sOut += IpHlpApidotnet.IPHelper.dump_MIB_UDPROW(stat) + "\r\n";
            }

            g_iphlp.GetTcpStats();
            sOut += "======= TCP statistics ========\r\n";
            sOut += IpHlpApidotnet.IPHelper.dump_MIB_TCPSTATS(g_iphlp.TcpStats) + "\r\n";

            g_iphlp.GetUdpStats();
            sOut += "======= UDP statistics ========\r\n";
            sOut += IpHlpApidotnet.IPHelper.dump_MIB_UDPSTATS(g_iphlp.UdpStats) + "\r\n";


            AdapterInfo.AdaptersInfo aInfo = new AdapterInfo.AdaptersInfo();
            sOut += "======= Adapter infos ==========\r\n";
            foreach (AdapterInfo.IP_ADAPTER_INFO info in aInfo._adapterList)
            {
                sOut += info.Index.ToString() + ": " + info.AdapterName + ", " + info.CurrentIpAddress.IpAddress.String + "\r\n";
            }

            g_iphlp.getRoutingTable();
            sOut += "======= Route entries ==========\r\n";
            //                                   1         2         3         4         5         6         7         8
            //                          12345678901234567890123456789012345678901234567890123456789012345678901234567890123
            sOut += "Network Destination        Netmask          Gateway          Interface     Metric\r\n";
            foreach (IpHlpApidotnet.IPHlpAPI32Wrapper.MIB_IPFORWARDROW row in g_iphlp._routeEntry)
            {
                sOut += g_iphlp.dumpRouteEntry(row) + "\r\n";
            }

            string sFile = "\\netstat.log";
            try
            {
                StreamWriter sw = new StreamWriter(sFile, true, Encoding.UTF8, 2000);
                sw.WriteLine(dtStr + " #################\r\n" + sOut);
                sw.Flush();
                sw.Close();
                System.Diagnostics.Debug.WriteLine("logged to file");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception in writeLog: " + ex.Message);
            }
            return sOut;
        }

        private void frmNetStat_Closing(object sender, CancelEventArgs e)
        {
            stopThread();
        }
   }
}