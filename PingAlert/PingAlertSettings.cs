using System;
using Microsoft.Win32;

using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace PingAlert
{
    /// <summary>
    /// settings class to persist settings in registry
    /// </summary>
    class PingAlertSettings:IDisposable
    {
        // Track whether Dispose has been called.
        private bool disposed = false;

        private string[] _sHosts;
        public string[] sHosts
        {
            get {
                //_sHosts = regReadStrings(_regValueNameHosts);
                return _sHosts; 
            }
            set { 
                _sHosts = value;
                //regWriteStrings(_regValueNameHosts, _sHosts);
            }
        }
        private int _iTimeInterval=15;
        public int iTimeInterval
        {
            get { return _iTimeInterval; }
            set { _iTimeInterval = value; }
        }

        private int _iLog2File = 0;
        public int iLog2File
        {
            get { return _iLog2File; }
            set { _iLog2File = value; }
        }

        private const string _regSubKey = @"Software\PingAlert";
        private RegistryKey _regKey;
        private const string _regValueNameHosts = "Hosts";
        private const string _regValueNameTimeInterval = "TimeInterval";
        private const string _regValueLog2File = "Log2File";

        public PingAlertSettings(){
            //try to read settings from Reg
            if (regOpenKey())
            {
                regReadSettings();
            }
            else
                this.Dispose();
        }

        public void Dispose(){
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

            //try to read host list
            string[] s = regReadStrings(_regValueNameHosts);
            if (s == null)
                _sHosts = null;
            else
                _sHosts = s;

            int i=0;
            i=regReadInt(_regValueNameTimeInterval);
            if (i != -1)
            {
                _iTimeInterval = i;
            }
            else
                _iTimeInterval = 15;

            i = regReadInt(_regValueLog2File);
            if (i != -1)
            {
                _iLog2File = i;
            }
            else
                _iLog2File = 0;

            return true;
        }

        public int saveSettings()
        {
            this.regWriteSettings();
            if (_sHosts != null)
                return _sHosts.Length;
            else
                return 0;
        }
        private bool regWriteSettings()
        {
            if (_sHosts != null)
            {
                if (_sHosts.Length > 0)
                    regWriteStrings(_regValueNameHosts, _sHosts);
            }
            if (_iTimeInterval > -1)
                regWriteInt(_regValueNameTimeInterval, _iTimeInterval);

            if (_iLog2File == 1)
                regWriteInt(_regValueLog2File, 1);
            else
                regWriteInt(_regValueLog2File, 0);

            return true;
        }

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
                Debug.WriteLine("Exception in regReadInt for '" + _regSubKey + "'/"+sValuename+". " + x.Message);
            }
            return -1;
        }
        private bool regWriteInt(string sValuename, int i)
        {
            if (_regKey == null)
                return false;
            try
            {
                _regKey.SetValue(sValuename, i);
                return true;
            }
            catch (Exception x)
            {
                Debug.WriteLine("Exception in regWriteInt for '" + _regSubKey + "'/" + sValuename + ". " + x.Message);
            }
            return false;
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
    }
}
