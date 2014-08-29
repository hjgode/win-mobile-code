using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using OpenNETCF.Net.NetworkInformation;

namespace PingNG
{
    public partial class frmOptions : Form
    {
        PingOptions _PO;

        public frmOptions(ref PingOptions po)
        {
            InitializeComponent();
            _PO = po;
            read();
        }
        void read()
        {
            chkDoNotFragment.Checked = _PO.DontFragment;
            numTimeout.Value = _PO.TimeOut;
            numTTL.Value = _PO.Ttl;
            numPings.Value = _PO.numOfPings;
            numBuffSize.Value = _PO.bufferSize;
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            _PO.Ttl = (int) numTTL.Value;
            _PO.TimeOut = (int)numTimeout.Value;
            _PO.DontFragment = chkDoNotFragment.Checked;
            _PO.numOfPings = (int)numPings.Value;
            _PO.bufferSize = (int)numBuffSize.Value;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}