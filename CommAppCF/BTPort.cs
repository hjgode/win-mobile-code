using System;

using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

using OpenNETCF.Net.Bluetooth;

namespace Comm.BT
{
    public class BTPort
    {
        private byte[] _bda;
        private BluetoothAddress _bta;
        private myThread _thread;
        private bool _bStopThread = false;
        //private Socket _socket;
        private BluetoothClient _bluetoothClient;
        private NetworkStream _networkStream = null;
        private StreamReader _streamReader;
        private StreamWriter _streamWriter;
        public BTPort()
        {
        }

        public BTPort(byte[] bdAddress)
        {
            this._bda = bdAddress;
        }

        public bool IsOpen()
        {
            if (_networkStream == null)
                return false;
            else
                return true;
        }

        public void Open(byte[] byteAddress)
        {
            this._bda = byteAddress;
            this.Open();
        }
        public void Open()
        {
            this.Close();

            if (_bda == null)
                return;
            try
            {
                //_socket = new System.Net.Sockets.Socket();
                _bluetoothClient = new BluetoothClient();
                _bta = new BluetoothAddress(_bda);
                _bluetoothClient.Connect(new BluetoothEndPoint(_bta, BluetoothService.SerialPort));

                _networkStream = _bluetoothClient.GetStream();
                if(_networkStream!=null)
                    _thread = new myThread(ref _networkStream);
            }
            catch (SocketException sx)
            {
                System.Diagnostics.Debug.WriteLine("SocketException in Open(): " + sx.Message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception in Open(): " +ex.Message);
            }

        }
        public void Write(string s)
        {
            if(_networkStream!=null){
                System.IO.StreamWriter sw = new System.IO.StreamWriter(_networkStream);
                if (sw.BaseStream != null)
                {
                    if (sw.BaseStream.CanWrite)
                    {
                        //byte[] buf = Encoding.ASCII.GetBytes(fp_text);
                        sw.Write(s); //ns.Write(buf, 0, buf.Length);
                        //ns.Flush();
                        sw.Flush();
                    }
                    sw.Close();
                }
            }
        }
        //public string Read is handled by DataReceived event handler

        //private System.Net.Sockets.NetworkStream connectBT(byte[] ba)
        //{
        //    this._networkStream = null;
        //    try
        //    {
        //        BluetoothAddress bda = new BluetoothAddress(ba);
        //        //System.Net.Sockets.Socket socket = new System.Net.Sockets.Socket();
        //        BluetoothClient btClient = new BluetoothClient();
        //        btClient.Connect(new BluetoothEndPoint(bda, BluetoothService.SerialPort));
        //        _networkStream = btClient.GetStream();
        //        return _networkStream;
        //        //    System.IO.StreamWriter sw = new System.IO.StreamWriter(ns);
        //        //    if (sw.BaseStream != null)
        //        //    {
        //        //        if (sw.BaseStream.CanWrite)
        //        //        {
        //        //            //byte[] buf = Encoding.ASCII.GetBytes(fp_text);
        //        //            sw.Write(fp_text); //ns.Write(buf, 0, buf.Length);
        //        //            //ns.Flush();
        //        //            sw.Flush();
        //        //        }
        //        //        sw.Close();
        //        //    }
        //        //    ns.Close();
        //        //    btClient.CloseSocket();
        //        //    btClient.Close();
        //        //    Cursor.Current = Cursors.Default;
        //        //}
        //    }
        //    catch (Exception x)
        //    {
        //        Cursor.Current = Cursors.Default;
        //        MessageBox.Show("Exception :" + x.Message);
        //        return ns;
        //    }
        //}

        public void Close()
        {
            if (_thread != null)
            {
                _networkStream.Close();
                
                _thread.Dispose();
                _thread = null;
            }
        }

        private BTDataReceivedEventHandler onDataReceived;

        public delegate void BTDataReceivedEventHandler(object sender, DataEventArgs d);

        //public event BTDataReceivedEventHandler BTDataReceived;
        public event BTDataReceivedEventHandler BTDataReceived
        {
            add
            {
                this.onDataReceived = ((BTDataReceivedEventHandler)Delegate.Combine(((Delegate)this.onDataReceived), ((Delegate)value)));
            }
            remove
            {
                this.onDataReceived = ((BTDataReceivedEventHandler)Delegate.Remove(((Delegate)this.onDataReceived), ((Delegate)value)));
            }

        }

        private void onDataReceivedEvent(object sender, DataEventArgs d)
        {
            BTDataReceivedEventHandler dataReceived = this.onDataReceived;

            if (dataReceived == null)
                return;
            dataReceived(this, d);
        }


        #region THREAD
        public class DataEventArgs : EventArgs
        {
            /* HACK: fields are typically private, but making this internal so it
             * can be accessed from other classes. In practice should use properties.
             */
            internal string _string;
            public DataEventArgs()
            {
            }
            public DataEventArgs(string str)
                : base()
            {
                this._string = str;
            }
        }

        public class myThread : System.ComponentModel.Component, IDisposable
        {
            private Thread _thread;
            private bool _bStopThread = false;
            #region EVENTSTUFF

            private BTDataReceivedEventHandler onDataReceived;

            public delegate void BTDataReceivedEventHandler(object sender, DataEventArgs d);

            //public event BTDataReceivedEventHandler BTDataReceived;
            public event BTDataReceivedEventHandler BTDataReceived
            {
                add
                {
                    this.onDataReceived = ((BTDataReceivedEventHandler)Delegate.Combine(((Delegate)this.onDataReceived), ((Delegate)value)));
                }
                remove
                {
                    this.onDataReceived = ((BTDataReceivedEventHandler)Delegate.Remove(((Delegate)this.onDataReceived), ((Delegate)value)));
                }

            }

            private void onDataReceivedEvent(object sender, DataEventArgs d)
            {
                BTDataReceivedEventHandler dataReceived = this.onDataReceived;

                if (dataReceived == null)
                    return;
                dataReceived(this, d);
            }

            private StreamReader _sr;

            #endregion
            public myThread(ref NetworkStream ns)
            {
                this.onDataReceived = (BTDataReceivedEventHandler)null;
                _sr = new StreamReader(ns);
                _bStopThread = false;
                _thread = new Thread(theThread);
                _thread.Start();
            }
            private bool disposed = false;
            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _thread.Abort();
                    //_bStopThread = true;
                }
                else
                {
                    _bStopThread = true;
                    Thread.Sleep(1000);
                }
                this.disposed = true;
                base.Dispose(disposing);
            }
            private void theThread()
            {
                System.Diagnostics.Debug.WriteLine("theThread: started...");
                try
                {
                    string s = "";
                    char[] buf = new char[1];
                    int iCount = 0;
                    DataEventArgs args = new DataEventArgs();
                    while (!_bStopThread && _sr != null)
                    {
                        iCount = _sr.Read(buf, 0, 1);
                        if (iCount != 0)
                        {
                            s = buf.ToString();
                            args._string = s;
                            onDataReceivedEvent(this, args);
                        }
                        Thread.Sleep(1000);
                    }

                }
                catch (ThreadAbortException tax)
                {
                    System.Diagnostics.Debug.WriteLine("theThread: ThreadAbortException=" + tax.Message);
                    _bStopThread = true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("theThread: Exception=" + ex.Message);
                }
                System.Diagnostics.Debug.WriteLine("theThread: ...ended");
            }
        }
        #endregion
    }
}
