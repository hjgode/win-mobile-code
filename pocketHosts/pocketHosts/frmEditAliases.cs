using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace pocketHosts
{
    public partial class frmEditAliases : Form
    {
        public List<string> m_Aliases;
        public frmEditAliases(List<string> lAliases)
        {
            InitializeComponent();
            m_Aliases = lAliases;
            foreach (string s in lAliases)
                lstAliases.Items.Add(s);
        }

        private void txtIP_TextChanged(object sender, EventArgs e)
        {
            //txtAlias.BackColor = Color.White;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            lstAliases.Items.Add(txtAlias.Text);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lstAliases.SelectedIndex == -1)
                return;
            if (MessageBox.Show("Remove " + lstAliases.Items[lstAliases.SelectedIndex].ToString() + "?", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
            {
                lstAliases.Items.RemoveAt(lstAliases.SelectedIndex);
            }

        }

        private void mnuOK_Click(object sender, EventArgs e)
        {
            m_Aliases.Clear();
            foreach (object o in lstAliases.Items)
            {
                m_Aliases.Add(o.ToString());
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}