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
        //using intermec bluetooth tool
        [DllImport("ibt.dll", SetLastError = true)]
        private static extern bool IBT_SetPrinter(StringBuilder AddrString, bool Register);
        [DllImport("ibt.dll", SetLastError = true)]
        private static extern UInt32 IBT_On (); 
        
        //using SetBtPrinter.DLL, needs pswdm0c.cab (pswdm0cDll.dll) installed on intermec!
        [DllImport("SetBtPrinter.dll", SetLastError = true)]
        private static extern int registerPrinter(StringBuilder AddrString);

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
            int iRes = -1;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                StringBuilder sb = new StringBuilder(txtBTAddress.Text);
                if (System.IO.File.Exists(@"\Windows\pswdm0cDLL.dll"))
                {
                    iRes = registerPrinter(sb); //this may take a while...
                    if (iRes != 0)
                    {
                        System.Diagnostics.Debug.WriteLine("registerPrinter failed:" + Marshal.GetLastWin32Error().ToString("x"));
                        bSuccess = false;
                    }
                    else
                        bSuccess = true;
                }
                else if (System.IO.File.Exists(@"\Windows\ibt.dll"))
                {
                    bSuccess = IBT_SetPrinter(sb, true);
                    if (!bSuccess)
                        System.Diagnostics.Debug.WriteLine("IBT_SetPrinter failed:" + Marshal.GetLastWin32Error().ToString("x"));
                }
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