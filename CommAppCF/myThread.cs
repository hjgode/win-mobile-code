using System;

using System.Collections.Generic;
using System.Text;

using System.Threading;

using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.CompilerServices;

namespace CommAppCF
{
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

    public class myThread:System.ComponentModel.Component,IDisposable
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
                while (!_bStopThread && _sr!=null)
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
}
