using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using OpenNETCF.Net.NetworkInformation;
using System.Threading;

namespace PingNG
{
    public partial class Form1 : Form
    {
        //global
        myPing _myPing;
        public Form1()
        {
            InitializeComponent();
            lblPingOptions.Text = myPingOptions.ToString();
            _myPing = new myPing();
            myPing.onReplyEvent += new myPing.ReplyEventHandler(_myPing_onReplyEvent);
            //sample usage: _myPing.doPing("192.168.128.5", myPingOptions);
        }

        void _myPing_onReplyEvent(object sender, myPing.PingReplyEventArgs args)
        {
            addLog(args.message);
            if(args.replytype==myPing.PingReplyTypes.done)
                enableButton(true);
        }

        delegate void enableButtonCallback(bool bEnable);
        public void enableButton(bool bEnable)
        {
            if (this.btnPing.InvokeRequired)
            {
                enableButtonCallback d = new enableButtonCallback(enableButton);
                this.Invoke(d, new object[] { bEnable });
            }
            else
                this.btnPing.Enabled = bEnable;
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
                if (txtLog.Text.Length > 20000)
                    txtLog.Text = "";
                txtLog.Text += text + "\r\n";
                txtLog.SelectionLength = 0;
                txtLog.SelectionStart = txtLog.Text.Length - 1;
                txtLog.ScrollToCaret();
            }
        }

        private void btnPing_Click(object sender, EventArgs e)
        {
            btnPing.Enabled = false;
            _myPing.doPing(textBox1.Text, myPingOptions);
        }

        PingOptions myPingOptions = new PingOptions();
        private void btnOptions_Click(object sender, EventArgs e)
        {
            frmOptions dlg = new frmOptions(ref myPingOptions);
            if (dlg.ShowDialog() == DialogResult.OK)
                lblPingOptions.Text= myPingOptions.ToString();
        }
    }
}