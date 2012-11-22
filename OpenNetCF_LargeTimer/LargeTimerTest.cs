using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using CEGETUSERNOTIFICATION;

namespace OpenNetCF_LargeTimer
{
    public partial class LargeTimerTest : Form
    {
        OpenNETCF.WindowsCE.LargeIntervalTimer timer;
        public LargeTimerTest()
        {
            InitializeComponent();
            myDict = new Dictionary<string, string>();
            myDict2 = new Dictionary<string, string>();
        }

        Dictionary<string, string> myDict;
        Dictionary<string, string> myDict2;
        private void button1_Click(object sender, EventArgs e)
        {
            addLog("START LargeIntervaleTimer");
            myDict.Clear(); myDict2.Clear();
            timer = new OpenNETCF.WindowsCE.LargeIntervalTimer();
            listUserNotifications(true);
            timer.Interval = new TimeSpan(1, 0, 0);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Enabled = true;
            listUserNotifications(false);
        }

        void listUserNotifications(bool bBeforeAfter)
        {
            System.Diagnostics.Debug.WriteLine("-------------------------------------------------");
            addLog("-----------------start list--------------------------------");
            CeUserNotificationsClass cls = new CeUserNotificationsClass();
            CeUserNotificationsClass.EventEntry[] myEvents = cls.eventEntries;
            foreach (CeUserNotificationsClass.EventEntry entry in myEvents)
            {
                System.Diagnostics.Debug.WriteLine("Usernotification: " + entry.sHandle + ": '" + entry.sApp + "', '" + entry.sEvent + "', '" + entry.sStartTime + "'");
                addLog("Usernotification: " + entry.sHandle + ": '" + entry.sApp + "', '" + entry.sEvent + "', '" + entry.sStartTime + "'");
                if (bBeforeAfter)
                    myDict.Add(entry.sHandle, entry.sApp);
                else
                    myDict2.Add(entry.sHandle, entry.sApp);
            }
            if (!bBeforeAfter)
            {
                for(int i=0; i<myDict2.Count; i++)// (string sKey in myDict2.Keys)
                {
                    KeyValuePair<string, string> sKeyPair = myDict2.ElementAt(i);
                    if (!myDict.ContainsKey(sKeyPair.Key))
                    {
                        System.Diagnostics.Debug.WriteLine("New entry: " + sKeyPair);
                        addLog("++++++++++++++++++++\r\nNew entry: " + sKeyPair.Key + ", " + sKeyPair.Value + "\r\n++++++++++++++++++++");
                    }
                }
            }
            addLog("----------------end list---------------------------------");
        }

        void timer_Tick(object sender, EventArgs e)
        {
            
        }
        delegate void SetTextCallback(string text);
        public void addLog(string text)
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
                txtLog.SelectionLength = 0;
                txtLog.SelectionStart = txtLog.Text.Length - 1;
                txtLog.ScrollToCaret();
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            addLog("STOP LargeIntervaleTimer");
            if (timer != null)
            {
                timer.Tick -= timer_Tick;
                timer.Dispose();
                timer = null;                
            }
            myDict2.Clear();
            listUserNotifications(false);
        }
    }
}