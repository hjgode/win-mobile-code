using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using Microsoft.WindowsCE.Forms;
using System.Reflection;

namespace PingAlert
{
    public partial class Form1 : Form
    {
        //using a public queue for comm
        Queue theQueue = new Queue();
        bgThread2 theBGthread;
        private int numberOfQueuedData=0;
        StringBuilder sbHTML;
        const string szHtmlFile = "\\Windows\\PingAlertReport.html";
        private bool bRunAndQuit = false;
        private static bool bQuitIsOK = false;

        private System.Windows.Forms.Timer exitTimer1;

        private PingAlertSettings paSettings;
        private const string _sLogFile = @"\PingAlert.Log.txt";

        public Form1(String[] args)
        {
            InitializeComponent();

            paSettings = new PingAlertSettings();
            readSettings();

            //get cmdLine
            String myArg="";
            if (args.Length == 1)
            {
                myArg = args[0];
                if (myArg.Equals("pingsweep", StringComparison.OrdinalIgnoreCase))
                    bRunAndQuit = true;
            }

            //prepare html string
            startHTML();

            theQueue=new Queue();
            //start the background thread
            theBGthread = new bgThread2(theQueue);
            theBGthread.bgThread2Event += new bgThread2.bgThread2EventHandler(theBGthread_bgThread2Event);
#if DEBUG
            //addIp2Ping("127.0.0.1");
            //addIp2Ping("localhost");
            //addHost2Ping("google.com");
            //addHost2Ping("192.168.128.5");
            //addHost2Ping("smart");
            listBox1.Items.Clear();
            listBox1.Items.Add("google.com");
            listBox1.Items.Add("192.168.128.5");
            listBox1.Items.Add("smart");
#endif
            //timer needed to exit automatically?
            if (bRunAndQuit)
            {
                exitTimer1 = new System.Windows.Forms.Timer();
                exitTimer1.Interval = 2000;
                exitTimer1.Tick += new EventHandler(exitTimer1_Tick);
                exitTimer1.Enabled = true;
                int cnt = listBox1.Items.Count;
                for (int i = 0; i < cnt; i++)
                {
                    addHost2Ping(listBox1.Items[i].ToString());
                }
            }
        }

        private void readSettings()
        {
            string[] hostlist = paSettings.sHosts;

            if (hostlist!=null && hostlist.Length > 0)
            {
                listBox1.Items.Clear();
                for (int i = 0; i < hostlist.Length; i++)
                {
                    listBox1.Items.Add(hostlist[i]);
                }
            }
            int x = 0;
            x = paSettings.iTimeInterval;
            if (x > 0)
                numericUpDown1.Value = x;

            x = paSettings.iLog2File;
            if (x == 1)
                chkLog2File.Checked = true;
            else
                chkLog2File.Checked = false;
        }
        private void saveSettings()
        {
            int cnt = listBox1.Items.Count;
            if (cnt > 0)
            {
                string[] sList = new string[cnt];
                for (int i = 0; i < cnt; i++)
                {
                    sList[i] = (string)listBox1.Items[i];
                }
                paSettings.sHosts = sList;
            }
            int t = (int)numericUpDown1.Value;
            if (t > 0)
                paSettings.iTimeInterval = t;

            if (chkLog2File.Checked)
                paSettings.iLog2File = 1;
            else
                paSettings.iLog2File = 0;

            //save to reg
            paSettings.saveSettings();
        }

        private void exitTimer1_Tick(object sender, EventArgs e)
        {
            //is it OK to quit now
            if(bQuitIsOK){
                exitTimer1.Enabled = false;
                this.Close();//exit form
            }
        }

        private void startHTML()
        {
            sbHTML = new StringBuilder();
            sbHTML.Append("<html><body><h1>PingAlert report</h1><table border=\"1\" cellpadding=\"1\">\r\n");
            sbHTML.Append("<tr><th>Host</th></td><th>Ping result</th></tr>\r\n");
        }
        private void closeHTML()
        {
            sbHTML.Append("</table></body></html>\r\n");
        }

        void theBGthread_bgThread2Event(object sender, bgThread2.BgThreadEventArgs bte)
        {
            dbgOut("Host:" + bte.qData.sHost);
            try
            {
                //IP will be NULL if GetHost failed
                dbgOut("IP:" + bte.qData.IP.ToString());
            }
            catch (Exception)
            {
                dbgOut("IP:" + "null");       
            }
            dbgOut("Replies: " + bte.qData.iPingReplies);
            dbgOut("Count: " + bte.qData.iPingCount);
            dbgOut("ReplyTime: " + bte.qData.iPingReplyTime);
            dbgOut("Timeout: " + bte.qData.iPingTimeout);
            dbgOut("==================================");
            //n = new Nachricht();
            if (bte.qData.iPingCount != bte.qData.iPingReplies)
            {
                sbHTML.Append("<tr><td>" + bte.qData.sHost + "</td><td>failed" + "</td></tr>\r\n");
                dbgOut("Ping for host '" + bte.qData.sHost + "' failed");
            }
            else
            {
                sbHTML.Append("<tr><td>" + bte.qData.sHost + "</td><td>OK" + "</td></tr>\r\n");
                dbgOut("Ping for host '" + bte.qData.sHost + "' OK");
            }

            //dbgOut("qDataCount=" + bte.qData._iCount.ToString() + " numberOfQueuedData: " + numberOfQueuedData.ToString());

            if (bte.qData._iCount == numberOfQueuedData)
            {
                //end html
                closeHTML();
                //write report file
                //System.IO.TextWriter stringWriter = new System.IO.StringWriter();
                try
                {
                    System.IO.TextWriter streamWriter = new System.IO.StreamWriter(szHtmlFile);
                    streamWriter.WriteLine(sbHTML);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                catch (Exception x)
                {
                    addLog("Exception in write HTML: " + x.Message);
                }

                //start notification
                showNotification();
                bQuitIsOK = true;
            }
        }

        public void showNotification()
        {
            System.Diagnostics.ProcessStartInfo procInfo = new System.Diagnostics.ProcessStartInfo(@"\Windows\PingAlertToast.exe", "");
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = procInfo;
            if (proc.Start())
                addLog("PingAlertToast started");
            else
                addLog("PingAlertToast start failed");
            proc.Close();

        }

        public int addHost2Ping(string s)
        {
            int iRet = 0;
            queueData theData = new queueData();
            theData.sHost = s;
            //theData.sIP=s;
            lock (theQueue.SyncRoot)// syncedCollection.SyncRoot)
            {
                theQueue.Enqueue(theData);
                numberOfQueuedData++;
            }
            return iRet;
        }
        public void dbgOut(string s)
        {
            System.Diagnostics.Debug.WriteLine(s);
            addLog(s);
        }

        delegate void SetTextCallback(string text);
        private void addLog(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.txtLog.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(addLog);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                if (txtLog.Text.Length > 2000)
                    txtLog.Text = "";
                txtLog.Text += text + "\r\n";
                txtLog.SelectionLength = text.Length;
                txtLog.SelectionStart = txtLog.Text.Length - text.Length;
                txtLog.ScrollToCaret();
                if(chkLog2File.Checked)
                    fileLog(text);
            }
        }
        private void fileLog(string s){
            //cut log file size
            System.IO.FileInfo fi = new System.IO.FileInfo(_sLogFile);
            if (fi.Exists)
            {
                if (fi.Length > 1000000)
                    System.IO.File.Delete(_sLogFile);
            }

            System.IO.StreamWriter sw = new System.IO.StreamWriter(_sLogFile, true);
            sw.WriteLine(s);
            sw.Flush();
            sw.Close();
        }
        private void Form1_Closing(object sender, CancelEventArgs e)
        {
            if(theBGthread!=null)
                theBGthread.Dispose();
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            this.saveSettings();
            //first abort the thread!!!!!
            if (theBGthread != null)
                theBGthread.Dispose();
            System.Threading.Thread.Sleep(1000);
            Application.Exit();
        }

        private void btnPingAll_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0)
                return;
#if DEBUG
            numberOfQueuedData = 0;
            //addIp2Ping("127.0.0.1");
            //addIp2Ping("localhost");
            addHost2Ping("google.com");
            addHost2Ping("192.168.128.5");
            addHost2Ping("smart");

#endif
            startHTML();
            for (int j = 0; j < listBox1.Items.Count; j++)
            {
                addHost2Ping(listBox1.Items[j].ToString());
            }
            numberOfQueuedData = listBox1.Items.Count;
        }

        private void btnSchedule_Click(object sender, EventArgs e)
        {
            //Save current Settings
            paSettings.iTimeInterval = (int)numericUpDown1.Value;
            paSettings.saveSettings();
            System.Diagnostics.ProcessStartInfo procInfo = new System.Diagnostics.ProcessStartInfo(@"\Windows\PingAlertScheduler.exe", "");
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = procInfo;
            if (proc.Start())
                addLog("PingAlertScheduler started");
            else
                addLog("PingAlertScheduler start failed");
            proc.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
                listBox1.Items.Add(textBox1.Text);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            int d = listBox1.SelectedIndex;
            if (d == -1)
                return;
            if (MessageBox.Show("Remove '" + listBox1.Items[d] + "' from list", "PingAlert Remove Host", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                listBox1.Items.RemoveAt(d);
            }
        }

        private void btnClearSchedule_Click(object sender, EventArgs e)
        {
            System.Diagnostics.ProcessStartInfo procInfo = new System.Diagnostics.ProcessStartInfo(@"\Windows\PingAlertScheduler.exe", "clear");
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = procInfo;
            if (proc.Start())
                addLog("PingAlertScheduler start cleared");
            else
                addLog("PingAlertScheduler clearing failed");
            proc.Close();

        }
    }//form class
}//namespace