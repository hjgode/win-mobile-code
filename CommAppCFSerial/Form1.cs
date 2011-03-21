using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

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
                txtReceive.Invoke(new EventHandler(delegate { txtReceive.Text += s + "\r\n"; }));
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
            if (mnuConnect.Text == "Disconnect")
            {
                comport.ReadExisting();
                comport.Close();
                mnuConnect.Text = "Connect";
                return;
            }

            ConnectDlg dlg = new ConnectDlg(ref comport);
            dlg.ShowDialog();
            if (comport.IsOpen)
            {
                mnuConnect.Text = "Disconnect";
                txtReceive.Invoke(new EventHandler(delegate { txtReceive.Text += comport.PortName + " opened\r\n"; }));
            }
            else
                mnuConnect.Text = "Connect";
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
    }
}