using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CpuMonRcv
{
    public partial class frmSelectIP : Form
    {
        public string sIP = "";
        public frmSelectIP()
        {
            InitializeComponent();
            DataAccess da = new DataAccess();
            string[] listIP = da.getKnownIPs();
            lbIP.Items.Clear();
            foreach (string s in listIP)
            {
                lbIP.Items.Add(s);
            }
            lbIP.SelectedIndex = 0;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (lbIP.SelectedIndex != -1)
                sIP = lbIP.SelectedItem.ToString();
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
