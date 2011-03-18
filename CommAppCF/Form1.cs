using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using OpenNETCF.Net.Bluetooth;

namespace CommAppCF
{
    public partial class Form1 : Form
    {
        private SerialPort comport = new SerialPort();
        private bool bUseHexDecode = false;

        public Form1()
        {
            InitializeComponent();
            comport.DataReceived += new SerialDataReceivedEventHandler(comport_DataReceived);
            comport.ErrorReceived += new SerialErrorReceivedEventHandler(comport_ErrorReceived);
            enableControls(false);
        }

        void comport_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            txtReceive.Invoke(new EventHandler(delegate { txtReceive.Text += e.ToString(); }));
        }

        void comport_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // This method will be called when there is data waiting in the port's buffer

            // Determain which mode (string or binary) the user is in
            // Read all the data waiting in the buffer
            try
            {
                string data = "";
                data = comport.ReadLine();//.ReadExisting();
                string s = "";
                // Display the text to the user in the terminal
                if (bUseHexDecode)
                    s = Utility.HexEncoding.ToMixedString(data);
                else
                    s = data;
                txtReceive.Invoke(new EventHandler(delegate { txtReceive.Text += "<" + s + "\r\n"; })); //added < to mark incoming data
            }
            catch (Exception ex)
            {
                txtReceive.Invoke(new EventHandler(delegate { txtReceive.Text += ex.Message + "\r\n"; }));
                //MessageBox.Show(ex.Message);
            }
            //label2.Invoke(new EventHandler(delegate{label2.Text  = data;}));

            //Log(LogMsgType.Incoming, data);
        }

        private void mnuConnect_Click(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// enable controls based on active connection
        /// </summary>
        /// <param name="bEnable"></param>
        private void enableControls(bool bEnable)
        {
            btnSend.Enabled = bEnable;
            btnSendFile.Enabled = bEnable;
            btnSendLine.Enabled = bEnable;
        }
        delegate void SetTextCallback(string text);
        private void updateTxtRcv(string text)
        {
            if (this.txtReceive.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(updateTxtRcv);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                if (txtReceive.Text.Length > 2000)
                    txtReceive.Text = "";
                txtReceive.Text += text + "\r\n";
                txtReceive.SelectionLength = text.Length;
                txtReceive.SelectionStart = txtReceive.Text.Length - text.Length;
                txtReceive.ScrollToCaret();
            }

        }
        private void Form1_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                //comport.DiscardOutBuffer();
                //comport.DiscardInBuffer();
                //comport.Dispose();
                if (comport.IsOpen)
                {
                    comport.ReadExisting();
                    comport.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtSend.Text.Length > 0)
                {
                    string s = txtSend.Text;
                    if (chkUseHexEncoder.Checked)
                    {
                        byte[] b = Utility.HexEncoding.FromHexedString(s);
                        comport.Write(b, 0, b.Length);
                    }
                    else
                        comport.Write(s);
                }
            }
            catch (Exception)
            {

            }
        }

        private void btnSendLine_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtSend.Text.Length > 0)
                {
                    string s = txtSend.Text;
                    if (chkUseHexEncoder.Checked)
                    {
                        byte[] b = Utility.HexEncoding.FromHexedString(s);
                        comport.Write(b, 0, b.Length);
                        comport.WriteLine("");
                    }
                    else
                        comport.WriteLine(s);
                }
            }
            catch (Exception)
            {

            }

        }

        private void btnSendFile_Click(object sender, EventArgs e)
        {
            try
            {
                if (comport.IsOpen)
                {
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                    ofd.FilterIndex = 0;
                    if (ofd.ShowDialog()==DialogResult.OK){
                        string filename = ofd.FileName;
                        if (System.IO.File.Exists(filename))
                        {
                            Cursor.Current = Cursors.WaitCursor;
                            sendFile(filename);
                            Cursor.Current = Cursors.Default;
                        }
                    }
                }

            }
            catch (Exception x)
            {
                updateTxtRcv(x.Message);
            }
        }
        private void sendFile(string filename)
        {
            try
            {
                updateTxtRcv("Starting sendFile(" + filename + ")");

                System.IO.StreamReader sr = new System.IO.StreamReader(filename);
                int r;
                int blockSize = 4096;
                char[] buf = new char[blockSize];
                //how many blocks to send?
                System.IO.FileInfo fi = new System.IO.FileInfo(filename);
                long lBlocks = (long)(fi.Length / blockSize);
                if (lBlocks == 0)
                    lBlocks = 1;
                updateTxtRcv("Need to send " + lBlocks.ToString() + " blocks...");
                long lBlockNr = 1;
                do
                {
                    updateTxtRcv("Sending block " + lBlockNr.ToString() + " ...");
                    r = sr.Read(buf, 0, blockSize); //r = number of bytes read
                    comport.Write(buf, 0, r);                    
                }
                while (r!=-1 && !sr.EndOfStream);
                sr.Close();
                updateTxtRcv("Finished sendFile(" + filename + ")");
            }
            catch (Exception x)
            {
                updateTxtRcv("sendFile exception:" + x.Message);
                System.Diagnostics.Debug.WriteLine(x.Message);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtSend.Text = "";
        }

        private void chkUseHexEncoder_CheckStateChanged(object sender, EventArgs e)
        {
            bUseHexDecode = chkUseHexEncoder.Checked;
        }
        private bool bUseSocket;
        private myThread _thread;
        private void mnuSocketConnect_Click(object sender, EventArgs e)
        {
            byte[] bdAddress = new byte[6];
            BluetoothConnect dlg = new BluetoothConnect();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                bUseSocket = dlg.bUseSocket;
                if(bUseSocket)
                    bdAddress=dlg.bdAddress;
            }
            dlg.Dispose();
            if (bUseSocket)
            {
                if (_thread == null)
                {
                    System.Net.Sockets.NetworkStream ns = connectBT(bdAddress);
                    if (ns != null)
                    {
                        _thread.BTDataReceived += new myThread.BTDataReceivedEventHandler(_thread_BTDataReceived);
                        _thread = new myThread(ref ns);
                    }
                }
            }
        }

        void _thread_BTDataReceived(object sender, DataEventArgs d)
        {
            throw new NotImplementedException();
        }
        private void OnDataReceived(object sender, DataEventArgs da){
        }
        private System.Net.Sockets.NetworkStream connectBT(byte[] ba)
        {
            Cursor.Current = Cursors.WaitCursor;
            System.Net.Sockets.NetworkStream ns=null;
            try
            {
                BluetoothAddress bda = new BluetoothAddress(ba);
                //System.Net.Sockets.Socket socket = new System.Net.Sockets.Socket();
                BluetoothClient btClient = new BluetoothClient();
                btClient.Connect(new BluetoothEndPoint(bda, BluetoothService.SerialPort));
                ns = btClient.GetStream();
                return ns;
                //    System.IO.StreamWriter sw = new System.IO.StreamWriter(ns);
                //    if (sw.BaseStream != null)
                //    {
                //        if (sw.BaseStream.CanWrite)
                //        {
                //            //byte[] buf = Encoding.ASCII.GetBytes(fp_text);
                //            sw.Write(fp_text); //ns.Write(buf, 0, buf.Length);
                //            //ns.Flush();
                //            sw.Flush();
                //        }
                //        sw.Close();
                //    }
                //    ns.Close();
                //    btClient.CloseSocket();
                //    btClient.Close();
                //    Cursor.Current = Cursors.Default;
                //}
            }
            catch (Exception x)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show("Exception :" + x.Message);
                return ns;
            }
        }
        //private void btSearch_Click(object sender, EventArgs e)
        //{
        //    this.Enabled = false;
        //    Cursor.Current = Cursors.WaitCursor;
        //    BluetoothDeviceInfo[] bdi;
        //    BluetoothClient bc = new BluetoothClient();
        //    bdi = bc.DiscoverDevices();
        //    comboBox1.DisplayMember = "DeviceName";
        //    comboBox1.ValueMember = "DeviceID";
        //    comboBox1.DataSource = bdi;
        //    Cursor.Current = Cursors.Default;
        //    this.Enabled = true;
        //}


    }
}