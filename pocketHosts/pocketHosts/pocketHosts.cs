using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace pocketHosts
{
    public partial class pocketHosts : Form
    {
        hostsentries hEntries;

        public pocketHosts()
        {
            InitializeComponent();
            hEntries = new hostsentries();
            
            loadItems();
        }

        void loadItems()
        {
            listHosts.Items.Clear();
            foreach (hostsentry he in hEntries.allHosts.Values)
            {
                listHosts.Items.Add(he);
            }
            if(hEntries.allHosts.Count>0)
                listHosts.SelectedIndex = 0;
        }

        private void Form1_Closing(object sender, CancelEventArgs e)
        {
            if (MessageBox.Show("Are you sure to end now?", "EXIT?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Cancel)
                e.Cancel = true;
        }

        private void listHosts_SelectedIndexChanged(object sender, EventArgs e)
        {
            hostsentry he = (hostsentry)listHosts.SelectedItem;
            txtHost.Text = he.sHost;
            listIP.Items.Clear();
            if (he.ipAddress.Count > 0)
            {
                foreach (System.Net.IPAddress ip in he.ipAddress)
                    listIP.Items.Add(ip.ToString());
                listIP.SelectedIndex = 0;
            }
            if (he.ipAddress6.Count > 0)
            {
                foreach (System.Net.IPAddress ip in he.ipAddress6)
                    listIP.Items.Add(ip.ToString());
                listIP.SelectedIndex = 0;
            }

            listAliases.Items.Clear();
            if (he.aliases.Count > 0)
            {
                foreach (string s in he.aliases)
                    listAliases.Items.Add(s);
                listAliases.SelectedIndex = 0;
            }

            txtExpires.Text = he.expireTime.ToString();
        }

        private void btnIPadd_Click(object sender, EventArgs e)
        {
            List<System.Net.IPAddress> ipTmp=new List<System.Net.IPAddress>();
            foreach (object o in listIP.Items)
            {
                ipTmp.Add(System.Net.IPAddress.Parse(o.ToString()));
            }
            frmEnterIP dlg = new frmEnterIP(ipTmp);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                listIP.Items.Clear();
                foreach (System.Net.IPAddress ip in dlg.m_ipList)
                    listIP.Items.Add(ip);
                listIP.SelectedIndex = 0;
                string sHost = txtHost.Text;
            }
            dlg.Dispose();
        }

        private void btnAliasAdd_Click(object sender, EventArgs e)
        {
            List<string> sAlias=new List<string>();
            foreach(object o in listAliases.Items)
                sAlias.Add(o.ToString());
            frmEditAliases dlg = new frmEditAliases(sAlias);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                listAliases.Items.Clear();
                foreach (string s in dlg.m_Aliases)
                    listAliases.Items.Add(s);
                listAliases.SelectedIndex = 0;
            }
            dlg.Dispose();

        }

        private void menuItem2_Click(object sender, EventArgs e)
        {
            int iCount = this.hEntries.saveAll();
        }

        private void mnuSaveChanges_Click(object sender, EventArgs e)
        {
            if (txtHost.Text.Length == 0 || listIP.Items.Count==0)
                return;
            hostsentry heTmp = (hostsentry) listHosts.SelectedItem;

            List<string> lAliases = new List<string>();
            List<System.Net.IPAddress> lIP4 = new List<System.Net.IPAddress>();
            List<System.Net.IPAddress> lIP6 = new List<System.Net.IPAddress>();
            ulong lExpireTime = Convert.ToUInt64(txtExpires.Text);

            foreach (object o in listAliases.Items)
            {
                lAliases.Add(o.ToString());
            }

            foreach (object o in listIP.Items)
            {
                try
                {
                    System.Net.IPAddress ipTemp = System.Net.IPAddress.Parse(o.ToString());
                    if(ipTemp.AddressFamily==System.Net.Sockets.AddressFamily.InterNetwork)
                        lIP4.Add(ipTemp);
                    else if(ipTemp.AddressFamily==System.Net.Sockets.AddressFamily.InterNetworkV6)
                        lIP6.Add(ipTemp);
                }
                catch { }
            }

            heTmp.sHost = txtHost.Text;
            heTmp.aliases = lAliases;
            if(lExpireTime!=0)
                heTmp.expireTime = lExpireTime;
            heTmp.ipAddress = lIP4;
            heTmp.ipAddress6 = lIP6;
            hEntries.addChangeEntry(heTmp);
            hEntries.saveEntry(heTmp);
            loadItems();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            hostsentry he = new hostsentry();
            he.sHost = "NEW ENTRY - PLEASE CHANGE";
            he.ipAddress.Add(new System.Net.IPAddress(0));
            listHosts.Items.Insert(0, he);
            listHosts.SelectedIndex = 0;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (listHosts.SelectedIndex == -1)
                return;
            hostsentry he = (hostsentry)listHosts.SelectedItem;
            if (MessageBox.Show("Are you sure to remove '" + he.sHost + "' now?", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Cancel)
                return;
            if (hEntries.removeEntry(he) == 1)
                loadItems();
            else
                MessageBox.Show("Remove failed");
        }

    }
}