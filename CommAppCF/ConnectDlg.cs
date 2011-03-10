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
    public partial class ConnectDlg : Form
    {
        private SerialPort _serialPort;
        public ConnectDlg(ref SerialPort sp)
        {
            InitializeComponent();
            cmbPortName.Items.Clear();
            for (int i = 0; i < 10; i++)
            {
                cmbPortName.Items.Insert(i, "COM" + i.ToString());
            }
            cmbBaudRate.Items.Clear();
            int baud=1200;
            for (int i = 0; i < 7; i++)
            {
                baud = (int)(1200 * System.Math.Pow(2, i));
                cmbBaudRate.Items.Insert(i, baud.ToString());
            }

            cmbDataBits.Items.Clear();
            for (int i = 6; i < 9; i++)
            {
                cmbDataBits.Items.Add(i.ToString());
            }

            this._serialPort = sp;
            cmbBaudRate.SelectedIndex = 0;
            cmbDataBits.SelectedIndex = cmbDataBits.Items.Count-1;
            cmbParity.SelectedIndex = 0;
            cmbPortName.SelectedIndex = 0;
            cmbStopBits.SelectedIndex = 0;
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
                _serialPort.BaudRate = int.Parse(cmbBaudRate.Text);
                _serialPort.DataBits = int.Parse(cmbDataBits.Text);
                _serialPort.StopBits = stopBits.getStopBits(cmbStopBits.Text);
                _serialPort.Parity = parityBits.getParityBits(cmbParity.Text);
                _serialPort.PortName = cmbPortName.Text;
                _serialPort.ReadTimeout = 100;
                _serialPort.Open();
            }
            this.Close();
        }
    }
    public static class stopBits
    {
        public static StopBits getStopBits(string text)
        {
            if (text.Equals("None", StringComparison.OrdinalIgnoreCase))
                return StopBits.None;
            if (text.Equals("1.5", StringComparison.OrdinalIgnoreCase))
                return StopBits.OnePointFive;
            int i = Convert.ToInt16(text);
            switch (i)
            {
                case 1: return StopBits.One;
                case 2: return StopBits.Two;
                case 3: return StopBits.OnePointFive;
            }
            return StopBits.None;
        }
    }
    public static class parityBits
    {
        public static Parity getParityBits(string text)
        {
            if (text.Equals("None", StringComparison.OrdinalIgnoreCase))
                return Parity.None;

            int i = Convert.ToInt16(text);
            switch (i)
            {
                case 0: return Parity.None;
                case 1: return Parity.Odd;
                case 2: return Parity.Even;
                case 3: return Parity.Mark;
                case 4: return Parity.Space;
            }
            return Parity.None;
        }
    }
}