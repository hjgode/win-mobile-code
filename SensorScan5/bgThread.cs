// a background thread class enabled to do direct calls to GUI thread
// this variant uses locking and a global var to exchange data
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Microsoft.WindowsCE.Forms;
using System.ComponentModel;
using System.Threading;
using System.Runtime.InteropServices;

    public class bgThread : Component
    {
        /// <summary>
        /// the bgThreadEventHandler is the protoype of the eventhandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="bte = the EventArgs supplied later with the event"></param>
        public delegate void bgThreadEventHandler(object sender, ShakeDetection.ShakeEventArgs bte);
        /// <summary>
        /// this event function is the 'link' back to the subscriber of the event handler
        /// and will be used to fire the event in the other thread
        /// </summary>
        public event bgThreadEventHandler bgThreadEvent;
        /// <summary>
        /// global internal var to exchange data
        /// </summary>
        internal ShakeDetection.ShakeEventArgs _BGeventArgs;
        private Container components;
        /// <summary>
        /// we will run a background thread. this is the thread object we use
        /// </summary>
        private Thread myThread;
        /// <summary>
        /// a var to let the thread stop
        /// in reality the thread will be mostly aborted by using thread.Abort()
        /// </summary>
        private bool bRunThread;
        /// <summary>
        /// we will store the derived window handle for communication
        /// </summary>
        internal bgThreadWndProc bgWnd;
        /// <summary>
        /// we use a user window message ID, not any of the predefined WM_ constants
        /// </summary>
        internal const int msgID = 1025;
        /// <summary>
        /// object to lock access to class variable
        /// </summary>
        internal object theLock;

        #region Nested Class
        /// <summary>
        /// This is the message window we use to communicate back to the thread
        /// where the bgThread object is created
        /// so we avoid cross-threading updates problems especially with the GUI thread
        /// </summary>
        internal class bgThreadWndProc : MessageWindow
        {
            #region Fields
            private bgThread _bgThread;
            public IntPtr hwndControl;
            #endregion
            #region Constructors
            public bgThreadWndProc(bgThread Parent)
            {
                System.Diagnostics.Debug.WriteLine("Creating ThreadWnd...");
                this._bgThread = Parent;
                hwndControl = Hwnd;
                System.Diagnostics.Debug.WriteLine("bgThreadWndProc hWnd is 0x: "+hwndControl.ToInt32().ToString("x"));
            }
            #endregion
            #region Methods
            public const int WM_COPYDATA = 0x004A;
            protected override void WndProc(ref Message m)
            {
                int iMsg = m.Msg;
                System.Diagnostics.Debug.WriteLine("WndProc called...");
                switch (iMsg)
                {
                    case msgID:
                        {
                            System.Diagnostics.Debug.WriteLine("WndProc sending notification ...");
                            //this._bgThread.NotifyData(m.WParam);
                            //this._bgThread.NotifyData((int)m.WParam, _bgThread.sMsg);
                            this._bgThread.NotifyData();
                            break;
                        }
                    default:
                        {
                            base.WndProc(ref m);
                            break;
                        }
                }
            }
            #endregion //Methods

        }//MsgWnd
        #endregion
        #region theClass
        public bgThread()
        {
            bgWnd = new bgThreadWndProc(this);
            myThread = new Thread(myThreadStart);
            bRunThread = true;
            myThread.Start();
            _BGeventArgs = new ShakeDetection.ShakeEventArgs();
            theLock = new object();
        }
        #region TheTHREAD
        private void myThreadStart()
        {
            System.Diagnostics.Debug.WriteLine("Entering thread proc");
            int _i=0;
            Intermec.Device.Sensor theSensor = new Intermec.Device.Sensor(Intermec.Device.Sensor.SensorsEnabled.AccelerometerEnabled);
            theSensor.SensorDataUpdateInterval = 50;
            ShakeDetection.GVector gv = new ShakeDetection.GVector();
            try
            {
                do
                {
                    //The blocking function...
                    gv.X = theSensor.Acceleration.GForceX;
                    gv.Y = theSensor.Acceleration.GForceY;
                    gv.Z = theSensor.Acceleration.GForceZ;

                    System.Diagnostics.Debug.WriteLine("Calling into UI thread...");
                    Microsoft.WindowsCE.Forms.Message msg = Message.Create(bgWnd.Hwnd, msgID, new IntPtr(_i), IntPtr.Zero);
                    //to avoid reading and writing to var at same time from different threads
                    lock (theLock)
                    {
                        _BGeventArgs._gvector = gv;
                        //the PostMessage is catched in WndProc and the arguments are stored
                        //retrived using the _BGeventArgs variable
                        MessageWindow.PostMessage(ref msg); //hopefully there is no other posted msg
                    }
                    System.Diagnostics.Debug.WriteLine("Thread sleeps...");
                    _i++;
                    Thread.Sleep(50);
                } while (bRunThread);
            }
            catch (ThreadAbortException)
            {
                System.Diagnostics.Debug.WriteLine("Thread will abort");
                bRunThread = false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception in ThreadStart: " + ex.Message);
            }
            System.Diagnostics.Debug.WriteLine("ThreadProc ended");
        }
        #endregion
	    protected override void Dispose (bool disposing)
		{
			if (disposing)
			{
				if (this.components != null)
				{
					this.components.Dispose ();
				}
			}
            this.myThread.Abort();
			base.Dispose (disposing);
		}
        ~bgThread()
		{
            myThread.Abort();
			this.Dispose (false);
        }
        #region theNotifyHandlers
        private void NotifyData()
        {
            ShakeDetection.ShakeEventArgs _m_bgThreadEventArgs;
            lock (theLock)
            {
                _m_bgThreadEventArgs = _BGeventArgs;
            }
            //is there any subscriber
            if (this.bgThreadEvent == null)
            {
                return;
            }
            try
            {
                //_bgThreadEventArgs = new BgThreadEventArgs(i1, s);
                this.bgThreadEvent(this, _m_bgThreadEventArgs);
            }
            catch (MissingMethodException)
            {
            }
        }

        #endregion
        //==============================================================================
    }
    #endregion
