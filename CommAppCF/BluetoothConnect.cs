using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using OpenNETCF.Net.Bluetooth;

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

        public bool bUseSocket = false;
        public byte[] bdAddress;

        public BluetoothConnect()
        {
            InitializeComponent();
#if DEBUG
            txtBTAddress.Text = "0006660309E8";
#endif
        }
        /*
                private void connectBT(byte[] ba)
                {
                    Cursor.Current = Cursors.WaitCursor;
                    try
                    {
                        BluetoothAddress bda = new BluetoothAddress(ba);
                        //System.Net.Sockets.Socket socket = new System.Net.Sockets.Socket();
                        BluetoothClient btClient = new BluetoothClient();
                        btClient.Connect(new BluetoothEndPoint(bda, BluetoothService.SerialPort));
                        System.Net.Sockets.NetworkStream ns = btClient.GetStream();

                        System.IO.StreamWriter sw = new System.IO.StreamWriter(ns);
                        if (sw.BaseStream != null)
                        {
                            if (sw.BaseStream.CanWrite)
                            {
                                //byte[] buf = Encoding.ASCII.GetBytes(fp_text);
                                sw.Write(fp_text); //ns.Write(buf, 0, buf.Length);
                                //ns.Flush();
                                sw.Flush();
                            }
                            sw.Close();
                        }
                        ns.Close();
                        btClient.CloseSocket();
                        btClient.Close();
                        Cursor.Current = Cursors.Default;
                    }
                    catch (Exception x)
                    {
                        Cursor.Current = Cursors.Default;
                        MessageBox.Show("Exception :" + x.Message);
                    }
                }
                private void btSearch_Click(object sender, EventArgs e)
                {
                    this.Enabled = false;
                    Cursor.Current = Cursors.WaitCursor;
                    BluetoothDeviceInfo[] bdi;
                    BluetoothClient bc = new BluetoothClient();
                    bdi = bc.DiscoverDevices();
                    comboBox1.DisplayMember = "DeviceName";
                    comboBox1.ValueMember = "DeviceID";
                    comboBox1.DataSource = bdi;
                    Cursor.Current = Cursors.Default;
                    this.Enabled = true;
                }

        */
        private bool serialPortConnect()
        {
            bool bSuccess = false;
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
            return bSuccess;
        }
        private void mnuOK_Click(object sender, EventArgs e)
        {
            if (!bUseSocket)
                serialPortConnect();
            else
                ;
            this.Close();
        }

        private void mnuCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void radioSerialPort_CheckedChanged(object sender, EventArgs e)
        {
            bUseSocket = radioSocketConnect.Checked;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            Cursor.Current = Cursors.WaitCursor;
            BluetoothDeviceInfo[] bdi;
            BluetoothClient bc = new BluetoothClient();
            bdi = bc.DiscoverDevices();
            comboBox1.DisplayMember = "DeviceName";
            comboBox1.ValueMember = "DeviceID";
            comboBox1.DataSource = bdi;
            comboBox1.SelectedIndex = 0;
            bc.Close();
            Cursor.Current = Cursors.Default;
            this.Enabled = true;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int x = comboBox1.SelectedIndex;
            if (x == -1)
                return;

            BluetoothDeviceInfo BDI = (BluetoothDeviceInfo)(comboBox1.Items[x]);
            bdAddress = BDI.DeviceID;
            byte[] bDisplay = hexHelper.reverseBytes(bdAddress);
            txtBTAddress.Text = hexHelper.ToString(bDisplay);
        }
    }
}