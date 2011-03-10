using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.Diagnostics;

namespace CommAppCF
{
    class serialSet : IDisposable
    {
        // Track whether Dispose has been called.
        private bool disposed = false;

        private const string _regSubKey = @"Software\CommAppCF";
        private RegistryKey _regKey;

        private RegistryValueKind[] _regValueTypes = 
            {   RegistryValueKind.String, 
                RegistryValueKind.DWord ,
                RegistryValueKind.DWord ,
                RegistryValueKind.DWord ,
                RegistryValueKind.DWord ,
                RegistryValueKind.DWord 
            };
        private string[] _regValueNames = 
            {   "port", 
                "baudrate" ,
                "parity",
                "databits",
                "stopbits",
                "handshake"
            };
        
        private const string _regValueNameTimeInterval = "TimeInterval";
        private const string _regValueLog2File = "Log2File";

        public serialSet()
        {
            //try to read settings from Reg
            if (regOpenKey())
            {
                regReadSettings();
            }
            else
                this.Dispose();

        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    //save settings?
                    regWriteSettings();
                    regCloseKey();
                    //component.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.
                //CloseHandle(handle);
                //handle = IntPtr.Zero;

                // Note disposing has been done.
                disposed = true;

            }
        }

        private bool regReadSettings()
        {
            if (_regKey == null)
                return false;

            string s = "";
            uint   d = 0;
            //try to read settings
            for (int x = 0; x < _regValueNames.Length; x++)
            {
                if (_regValueTypes[x] == RegistryValueKind.String)
                {
                    s = regReadString(_regValueNames[x]);
                }
                else if (_regValueTypes[x] == RegistryValueKind.DWord)
                {
                    d = regReadUInt(_regValueNames[x]);
                }
                switch (x)
                {
                    case 0: if(s.Length>0)
                                _sPort = s; 
                            break;
                    case 1: if(d>0)
                                _baudRate = d; 
                            break;
                    case 2: if (d >= 0)
                                _parity = d; 
                            break;
                    case 3: if (d > 0)
                                _databits = d; 
                            break;
                    case 4: if (d > 0)
                                _stopBits = d; 
                            break;
                    case 5: if (d >= 0)
                                _handshake = d; 
                            break;
                }
            }

            return true;
        }
        public int saveSettings()
        {
            if (this.regWriteSettings())
                return 0;
            else
                return -1;
        }
        private bool regWriteSettings()
        {
            regWriteString(_regValueNames[0], _sPort);
            regWriteInt(_regValueNames[1], _baudRate);
            regWriteInt(_regValueNames[2], _parity);
            regWriteInt(_regValueNames[3], _databits);
            regWriteInt(_regValueNames[4], _stopBits);
            regWriteInt(_regValueNames[5], _handshake);

            return true;
        }
        #region REGISTRY
        private int regReadInt(string sValuename)
        {
            if (_regKey == null)
                return -1;
            try
            {
                int i = (int)_regKey.GetValue(sValuename, -1);
                return i;
            }
            catch (Exception x)
            {
                Debug.WriteLine("Exception in regReadInt for '" + _regSubKey + "'/" + sValuename + ". " + x.Message);
            }
            return -1;
        }
        private uint regReadUInt(string sValuename)
        {
            if (_regKey == null)
                return 0;
            try
            {
                int i = (int)_regKey.GetValue(sValuename, 0);
                return (uint)i;
            }
            catch (Exception x)
            {
                Debug.WriteLine("Exception in regReadUInt for '" + _regSubKey + "'/" + sValuename + ". " + x.Message);
            }
            return 0;
        }
        private bool regWriteInt(string sValuename, uint i)
        {
            if (_regKey == null)
                return false;
            try
            {
                _regKey.SetValue(sValuename, i, RegistryValueKind.DWord);
                return true;
            }
            catch (Exception x)
            {
                Debug.WriteLine("Exception in regWriteUInt for '" + _regSubKey + "'/" + sValuename + ". " + x.Message);
            }
            return false;
        }
        private bool regWriteInt(string sValuename, int i)
        {
            if (_regKey == null)
                return false;
            try
            {
                _regKey.SetValue(sValuename, i, RegistryValueKind.DWord);
                return true;
            }
            catch (Exception x)
            {
                Debug.WriteLine("Exception in regWriteInt for '" + _regSubKey + "'/" + sValuename + ". " + x.Message);
            }
            return false;
        }

        private string regReadString(string sValuename)
        {
            if (_regKey == null)
                return null;
            try
            {
                string s = (string)_regKey.GetValue(sValuename, "");
                return s;
            }
            catch (Exception x)
            {
                Debug.WriteLine("Exception in regReadStrings for '" + _regSubKey + "'/" + sValuename + ". " + x.Message);
            }
            return null;
        }

        private string[] regReadStrings(string sValuename)
        {
            if (_regKey == null)
                return null;
            try
            {
                string[] s = (string[])_regKey.GetValue(sValuename, "");
                return s;
            }
            catch (Exception x)
            {
                Debug.WriteLine("Exception in regReadStrings for '" + _regSubKey + "'/" + sValuename + ". " + x.Message);
            }
            return null;
        }
        private bool regWriteString(string sValuename, string s)
        {
            if (_regKey == null)
                return false;
            try
            {
                _regKey.SetValue(sValuename, s);
                _regKey.Flush();
                return true;
            }
            catch (Exception x)
            {
                Debug.WriteLine("Exception in regWriteStrings for '" + _regSubKey + "'. " + x.Message);
                return false;
            }
        }

        private bool regWriteStrings(string sValuename, string[] s)
        {
            if (_regKey == null)
                return false;
            try
            {
                _regKey.SetValue(sValuename, s);
                _regKey.Flush();
                return true;
            }
            catch (Exception x)
            {
                Debug.WriteLine("Exception in regWriteStrings for '" + _regSubKey + "'. " + x.Message);
                return false;
            }
        }
        private bool regOpenKey()
        {
            try
            {
                _regKey = Registry.LocalMachine.CreateSubKey(_regSubKey);
                return true;
            }
            catch (Exception x)
            {
                Debug.WriteLine("Exception in OpenSubKey/CreateSubKey for '" + _regSubKey + "'. " + x.Message);
            }
            return false;
        }
        private void regCloseKey()
        {
            if (_regKey != null)
            {
                try
                {
                    Registry.LocalMachine.Close();
                }
                catch (Exception x)
                {
                    Debug.WriteLine("Exception in CloseKey for '" + _regSubKey + "'. " + x.Message);
                }
            }
        }
#endregion
        #region FIELDS
        private string _sPort="COM0";
        public string sPort
        {
            get { return _sPort; }
            set { _sPort = value; }
        }
        private uint[] _baudRates = { 1200, 2400, 4800, 9600, 19200, 38200, 57600, 115200, 230400, 460800, 921600 };
        public uint[] baudRates
        {
            get { return _baudRates; }
        }
        private uint _baudRate = 1200;
        public uint baudRate
        {
            get { return _baudRate; }
            set { _baudRate = value; }
        }
        private uint _parity = 0;
        public uint parity
        {
            get { return _parity; }
            set { _parity = value; }
        }
        private uint _databits = 8;
        public uint databits
        {
            get { return _databits; }
            set { _databits = value; }
        }
        private uint _stopBits = 1;
        public uint stopBits
        {
            get { return _stopBits; }
            set { _stopBits = value; }
        }
        private uint _handshake = 0;
        public uint handshake
        {
            get { return _handshake; }
            set { _handshake = value; }
        }
        #endregion

    }
    public static class myParity
    {
        public static string[] parity = { "None", "Odd", "Even", "Mark", "Space" };
        public static int ToInt(string s)
        {
            for (int i = 0; i < parity.Length; i++)
            {
                if(s.Equals(parity[i],StringComparison.OrdinalIgnoreCase))
                    return i;
            }
            return 0;
        }
        public static string ToString(int i)
        {
            return parity[i];
        }
        public static Parity ToParity(int i)
        {
            switch (i)
            {
                case 0:
                    return Parity.None;
                case 1:
                    return Parity.Odd;
                case 2:
                    return Parity.Even;
                case 3:
                    return Parity.Mark;
                case 4:
                    return Parity.Space;
            }
            return Parity.None;
        }
        public static Parity ToParity(string text)
        {
            if (text.Equals("None", StringComparison.OrdinalIgnoreCase))
                return Parity.None;
            if (text.Equals("Odd", StringComparison.OrdinalIgnoreCase))
                return Parity.Odd;
            if (text.Equals("Even", StringComparison.OrdinalIgnoreCase))
                return Parity.Even;
            if (text.Equals("Mark", StringComparison.OrdinalIgnoreCase))
                return Parity.Mark;
            if (text.Equals("Space", StringComparison.OrdinalIgnoreCase))
                return Parity.Space;

            return Parity.None;
        }
    }//class myParity
    public static class myStopBits
    {
        public static StopBits ToStopBits(string text)
        {
            if (text.Equals("None", StringComparison.OrdinalIgnoreCase))
                return StopBits.None;
            if (text.Equals("One", StringComparison.OrdinalIgnoreCase))
                return StopBits.One;
            if (text.Equals("Two", StringComparison.OrdinalIgnoreCase))
                return StopBits.Two;
            if (text.Equals("OnePointFive", StringComparison.OrdinalIgnoreCase))
                return StopBits.OnePointFive;

            return StopBits.One;
        }
        public static StopBits ToStopBits(int i)
        {
            switch (i)
            {
                case 0: return StopBits.None;
                case 1: return StopBits.One;
                case 2: return StopBits.Two;
                case 3: return StopBits.OnePointFive;
            }
            return StopBits.One;
        }
        public static string ToString(StopBits sb)
        {
            switch (sb)
            {
                case StopBits.None: return "None";
                case StopBits.One: return "One";
                case StopBits.Two: return "Two";
                case StopBits.OnePointFive: return "OnePointFive";
            }
            return "One";
        }
        public static string ToString(int i)
        {
            switch (i)
            {
                case 0: return "None";
                case 1: return "One";
                case 2: return "Two";
                case 3: return "OnePointFive";
            }
            return "One";
        }
        public static int ToInt(StopBits sb)
        {
            switch (sb)
            {
                case StopBits.None: return 0;
                case StopBits.One: return 1;
                case StopBits.Two: return 2;
                case StopBits.OnePointFive: return 3;
            }
            return 1;
        }
    }//class myStopbits
    public static class myHandshake
    {
        public static string[] handshakes = { "None", "XOnXOff", "RequestToSend", "RequestToSendXOnXOff" };
        public static Handshake ToHandshake(int i)
        {
            switch (i)
            {
                case 0: return Handshake.None;
                case 1: return Handshake.XOnXOff;
                case 2: return Handshake.RequestToSend;
                case 3: return Handshake.RequestToSendXOnXOff;
            }
            return Handshake.None;
        }
        public static Handshake ToHandshake(string text)
        {
            if (text.Equals("None", StringComparison.OrdinalIgnoreCase))
                return Handshake.None;
            if (text.Equals("XOnXOff", StringComparison.OrdinalIgnoreCase))
                return Handshake.XOnXOff;
            if (text.Equals("RequestToSend", StringComparison.OrdinalIgnoreCase))
                return Handshake.RequestToSend;
            if (text.Equals("RequestToSendXOnXOff", StringComparison.OrdinalIgnoreCase))
                return Handshake.RequestToSendXOnXOff;
            return Handshake.None;
        }
        public static int ToInt(Handshake hs)
        {
            switch (hs)
            {
                case Handshake.None: return 0;
                case Handshake.XOnXOff: return 1;
                case Handshake.RequestToSend: return 2;
                case Handshake.RequestToSendXOnXOff: return 3;
            }
            return 0;
        }
        public static string ToString(Handshake hs)
        {
            switch (hs)
            {
                case Handshake.None: return "None";
                case Handshake.XOnXOff: return "XOnXOff";
                case Handshake.RequestToSend: return "RequestToSend";
                case Handshake.RequestToSendXOnXOff: return "RequestToSendXOnXOff";
            }
            return "None";
        }
        public static string ToString(int i)
        {
            switch (i)
            {
                case 0: return "None";
                case 1: return "XOnXOff";
                case 2: return "RequestToSend";
                case 3: return "RequestToSendXOnXOff";
            }
            return "None";
        }
    }//class myHandshake
}
