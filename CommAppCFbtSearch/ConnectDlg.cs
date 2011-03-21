using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace CommAppCFbtSearch
{
    public partial class ConnectDlg : Form
    {
        private SerialPort _serialPort;
        private uint[] baudRates = { 1200, 2400, 4800, 9600, 19200, 38200, 57600, 115200, 230400, 460800, 921600 };

        private serialSet _mySettings = new serialSet();

        public ConnectDlg(ref SerialPort sp)
        {
            InitializeComponent();
            
            if(System.IO.File.Exists("\\windows\\ibt.dll"))
                btnBTPrinter.Enabled=true;
            else
                btnBTPrinter.Enabled=false;

            //port list
            cmbPortName.Items.Clear();
            for (int i = 0; i < 10; i++)
            {
                cmbPortName.Items.Insert(i, "COM" + i.ToString());
            }
            int ix = cmbPortName.Items.IndexOf(_mySettings.sPort);
            if (ix != -1)
                cmbPortName.SelectedIndex = ix;
            else
                cmbPortName.SelectedIndex = 0;

            //baud list
            cmbBaudRate.Items.Clear();
            int j = 0;
            foreach (uint u in _mySettings.baudRates)
            {
                cmbBaudRate.Items.Insert(j, u.ToString());
                j++;
            }
            ix = findInCombo(cmbBaudRate, _mySettings.baudRate.ToString());
            if (ix != -1)
                cmbBaudRate.SelectedIndex = ix;
            else
                cmbBaudRate.SelectedIndex = 0;

            //databits
            cmbDataBits.Items.Clear();
            for (int i = 6; i < 9; i++)
            {
                cmbDataBits.Items.Add(i.ToString());
            }
            ix = findInCombo(cmbDataBits, _mySettings.databits.ToString());
            if (ix != -1)
                cmbDataBits.SelectedIndex = ix;
            else
                cmbDataBits.SelectedIndex = 0;

            //parity
            cmbParity.Items.Clear();
            for (int i = 0; i < myParity.parity.Length; i++)
            {
                cmbParity.Items.Insert(i, myParity.ToString(i));
            }

            cmbParity.SelectedIndex = (int)_mySettings.parity;

            //stopBits
            cmbStopBits.Items.Clear();
            for (int i = 0; i < 3; i++)
            {
                cmbStopBits.Items.Insert(i, myStopBits.ToString(i));
            }
            ix = findInCombo(cmbStopBits, _mySettings.stopBits.ToString());
            if (ix != -1)
                cmbStopBits.SelectedIndex = ix;
            else
                cmbStopBits.SelectedIndex = 1;
//            cmbStopBits.SelectedIndex = cmbStopBits.Items.IndexOf(_mySettings.stopBits);

            //handshake
            cmbHandshake.Items.Clear();
            for (int i = 0; i < myHandshake.handshakes.Length; i++)
            {
                cmbHandshake.Items.Insert(i, myHandshake.ToString(i));
            }
            cmbHandshake.SelectedIndex = (int)_mySettings.handshake;

            this._serialPort = sp;

        }

        private int findInCombo(ComboBox cbo, string s)
        {
            for (int j = 0; j < cbo.Items.Count; j++)
            {
                if (cbo.Items[j].ToString().Equals(s, StringComparison.OrdinalIgnoreCase))
                    return j;
            }
            return -1;
        }
        private void mnuCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void mnuConnect_Click(object sender, EventArgs e)
        {
            if (_serialPort.IsOpen)
                _serialPort.Close();
            else
            {
                // Set the port's settings
                _serialPort.PortName = cmbPortName.Text;
                _serialPort.BaudRate = int.Parse(cmbBaudRate.Text);
                _serialPort.DataBits = int.Parse(cmbDataBits.Text);
                
                if(myStopBits.ToStopBits(cmbStopBits.Text) != StopBits.None)
                    _serialPort.StopBits = myStopBits.ToStopBits(cmbStopBits.Text);

                _serialPort.Parity = myParity.ToParity(cmbParity.Text);
                _serialPort.Handshake = myHandshake.ToHandshake(cmbHandshake.Text);
                _serialPort.ReadTimeout = 100;

                _mySettings.baudRate = (uint)_serialPort.BaudRate;
                _mySettings.parity = (uint)_serialPort.Parity;
                _mySettings.databits = (uint)_serialPort.DataBits;
                _mySettings.stopBits = (uint)_serialPort.StopBits;
                
                _mySettings.sPort = _serialPort.PortName;

                _mySettings.saveSettings();

                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    _serialPort.Open();
                }
                catch (Exception x)
                {
                    System.Diagnostics.Debug.WriteLine("Exception in OpenPort: " + x.Message);
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
            }
            this.Close();
        }

        private void btnBTPrinter_Click(object sender, EventArgs e)
        {
            BluetoothConnect dlg = new BluetoothConnect();
            dlg.ShowDialog();
        }
    }
}