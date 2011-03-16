using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace CommAppCF
{
    public partial class BluetoothConnect : Form
    {
        [DllImport("ibt.dll", SetLastError = true)]
        private static extern bool IBT_SetPrinter(StringBuilder AddrString, bool Register);
        
        [DllImport("ibt.dll", SetLastError = true)]
        private static extern UInt32 IBT_On (); 
        
        public BluetoothConnect()
        {
            InitializeComponent();
#if DEBUG
            txtBTAddress.Text = "0006660309E8";
#endif
        }

        private void mnuOK_Click(object sender, EventArgs e)
        {
            bool bSuccess=false;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                StringBuilder sb = new StringBuilder(txtBTAddress.Text);
                bSuccess = IBT_SetPrinter(sb, true);
                if (!bSuccess)
                    System.Diagnostics.Debug.WriteLine("IBT_SetPrinter failed:" + Marshal.GetLastWin32Error().ToString("x"));
            }
            catch (Exception)
            {
                bSuccess = false;
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }

            if (bSuccess)
            {
                MessageBox.Show("Connection success");
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Connection failed");
                this.DialogResult = DialogResult.Abort;
            }
            this.Close();
        }

        private void mnuCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}