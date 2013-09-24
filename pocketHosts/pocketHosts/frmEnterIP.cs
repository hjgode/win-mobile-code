using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace pocketHosts
{
    public partial class frmEnterIP : Form
    {
        public List<System.Net.IPAddress> m_ipList;
        public frmEnterIP(List<System.Net.IPAddress> ipList)
        {
            InitializeComponent();
            foreach (System.Net.IPAddress ip in ipList)
                lstIPaddresses.Items.Add(ip);
            m_ipList = ipList;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                System.Net.IPAddress ip = System.Net.IPAddress.Parse(txtIP.Text);
                lstIPaddresses.Items.Add(ip.ToString());
            }
            catch (Exception)
            {
                txtIP.BackColor = Color.LightPink;
            }
        }

        private void txtIP_TextChanged(object sender, EventArgs e)
        {
            txtIP.BackColor = Color.White;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lstIPaddresses.SelectedIndex == -1)
                return;
            if (MessageBox.Show("Remove " + lstIPaddresses.Items[lstIPaddresses.SelectedIndex].ToString() + "?", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
            {
                lstIPaddresses.Items.RemoveAt(lstIPaddresses.SelectedIndex);
            }
        }

        private void mnuOK_Click(object sender, EventArgs e)
        {
            m_ipList.Clear();
            foreach (object o in lstIPaddresses.Items)
            {
                System.Net.IPAddress ip = System.Net.IPAddress.Parse(o.ToString());
                m_ipList.Add(ip);
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}