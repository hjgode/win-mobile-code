//#define NORMALTAG
//change the above to read user TAG memory bank 3
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Intermec.DataCollection;
using Intermec.DataCollection.RFID;

// USAGE:
//using ThreadClass;
//...
//ThreadClass.myThread mThread;
//private void button1_Click(object sender, EventArgs e)
//{
//    mThread = new myThread();
//    mThread.MyEvent += new EventHandler<myThread.MyEventArgs>(onEvent);
//}
//public void onEvent(object o, ThreadClass.myThread.MyEventArgs s)
//{
//    System.Diagnostics.Debug.WriteLine("onEvent received: '" + s.Message + "'");
//    mThread.stopThread = true;
//}

namespace ThreadClass
{
    public class myThread:Control,IDisposable
    {
        public class EventArgs<T> : EventArgs
        {
            public EventArgs(T pValue)
            {
                m_value = pValue;
            }

            private T m_value;

            public T Value
            {
                get { return m_value; }
            }
        }
        public class MyEventArgs : EventArgs
        {
            private string msg;

            public MyEventArgs(string messageData)
            {
                msg = messageData;
            }
            public string Message
            {
                get { return msg; }
                set { msg = value; }
            }
        }

        private EventHandler<MyEventArgs> __MyEvent;
        public event EventHandler<MyEventArgs> MyEvent
        {
            add
            {
                lock (this) { __MyEvent += value; }
            }
            remove
            {
                lock (this) { __MyEvent -= value; }
            }
        }

        private EventHandler __pulseEvent;
        public event EventHandler MyPulseEvent
        {
            add
            {
                lock (this) { __pulseEvent += value; }
            }
            remove
            {
                lock (this) { __pulseEvent -= value; }
            }
        }
        private Thread m_Thread;
        private bool m_stopThread = false;
        public bool stopThread{
            get { return m_stopThread; }
            set { m_stopThread = value;
            if (m_stopThread)
                m_Thread.Abort();
            }
        }
        private bool m_threadStopped = false;
        public bool threadStopped
        {
            get {
                lock (this)
                {
                    return m_threadStopped;
                }
            }
        }

        public myThread()
        {
            m_Thread = new Thread(new ThreadStart (this.threadWorker));
            m_Thread.Name = "my Worker Thread";
            m_Thread.Start();
        }
        public void Dispose(){
            this.stopThread = true;
            base.Dispose();
        }
        private void threadWorker()
        {
            m_threadStopped = false;
            System.Diagnostics.Debug.WriteLine("+++ Thread started");
            try
            {
                do
                {
                    //DoSomething;
                    Thread.Sleep(1000);
                    doWork();
                    //fire the event handlers
                    __MyEvent(this, new MyEventArgs("Hello from Thread"));
                    __pulseEvent(this, null);
                } while (!m_stopThread);
            }
            catch (ThreadAbortException tx)
            {
                System.Diagnostics.Debug.WriteLine("ThreadAbortException: " + tx.Message + "\r\n ---Thread is aborted");
            }
            finally
            {
                //switch off any LED
                tools.SetLedStatus(3, tools.LedFlags.STATE_OFF);
            }
            m_threadStopped = true;
            System.Diagnostics.Debug.WriteLine("\n---Thread stopped");
        }

        void doWork()
        {
            int iDelay = 0; bool bLEDoff = false;
            logThis("Entering doWork()");
            while (!m_stopThread)
            {
                try
                {
                    ConnectReader();
                    while (m_Reader.IsConnected)
                    {
                        if (iDelay > 4)
                        {
                            tools.SetLedStatus(3, tools.LedFlags.STATE_ON);
                            bLEDoff = true; // need to turn off LED
                            iDelay = 0;
                        }
                        System.Threading.Thread.Sleep(200);
                        if (bLEDoff)
                        {
                            tools.SetLedStatus(3, tools.LedFlags.STATE_OFF);
                            bLEDoff = false;
                        }
                        iDelay++;
                    }
                    CloseReader();
                }
                catch (BasicReaderException brx)
                {
                    logThis("BasicReaderException in doWork(): " + brx.Message);
                }
                catch (BRIParserException bpx)
                {
                    logThis("BRIParserException in doWork(): " + bpx.Message);
                }
                catch (Exception ex)
                {
                    logThis("Exception in doWork(): " + ex.Message);
                }
            }
            logThis("Leaving doWork()");
        }

        BasicBRIReader m_Reader = null;
        bool m_bCenterEventProcessing = false;
        public string sCurrentCMD;
        Tag[] tags;
        bool m_bEventLogging = true;

        private int OpenReader()
        {
            logThis("OpenReader...");
            BasicBRIReader.LoggerOptions LogOp = new BasicBRIReader.LoggerOptions();
            LogOp.LogFilePath = ".\\IDLClassDebugLog.txt";
            //LogOp.LogFilePath="\\Program Files\\IP4IDLAPP\\IDLClassDebugLog.txt";
            LogOp.ShowNonPrintableChars = true;
            int res = 0;
            try
            {
                if (m_Reader != null)
                {
                    CloseReader();
                }
                m_Reader = new BasicBRIReader(this, LogOp);
                m_Reader.Open();
                AddEventHandlers();
            }
            catch (BasicReaderException brx)
            {
                logThis("BasicReaderException in OpenReader(): " + brx.Message);
                res = -1;
            }
            catch (SystemException sx)
            {
                logThis("SystemException in OpenReader(): " + sx.Message);
                res = -2;
            }
            catch (Exception sx)
            {
                logThis("Exception in OpenReader(): " + sx.Message);
                res = -2;
            }
            if (res != 0)
                System.Threading.Thread.Sleep(5000);
            return res;
        }
        /// <summary>
        /// This function fires when the center trigger on the IP4 is pulled or released
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="EvtArgs"></param>
        void brdr_EventHandlerCenterTrigger(object sender, BasicReaderEventArgs EvtArgs)
        {
            if (m_Reader.IsConnected == false)
            {
                //irda connection is still asleep after a 700 resume
                return;
            }
            //avoid multiple processing
//            if (m_bCenterEventProcessing)
//                return;
            m_bCenterEventProcessing = true;
            /*CenterTriggerState*/
            /*EVT_CenterTrigger_EventArgs.STATE.PULLED*/
            if (EvtArgs.EventType.Equals(BasicReaderEventArgs.EventTypes.EVT_DCE) && // BasicReaderEventArgs.EventTypes.EVT_CENTER_TRIGGER) && 
                EvtArgs.EventData.Equals("BUTTON CENTER 1"))
            {
                #region Write Single TAG
                #endregion
#if NORMALTAG
                sCurrentCMD = "READ HEX(0,12)"; //TAGID
#else
                sCurrentCMD = "READ HEX(3:0,12) REPORT=EVENTALL"; //TAGID
#endif
                // sCurrentCMD = "EPCID";
                if (ExecuteCMD(sCurrentCMD))
                {
                    logThis("OK READ HEX");
                }
                else
                {
                    logThis("failed");
                }

            }
            m_bCenterEventProcessing = false;
        }

        private int SetAttributes()
        {
            bool bStatus = false;
            if (m_Reader.IsConnected == false) { return -1; }
            sCurrentCMD = "ATTRIB TAGTYPE=EPCC1G2"; // comboBox1.Text;
            bStatus = ExecuteCMD(sCurrentCMD);
            if (bStatus == false)
            {
                logThis("SetAttrib failed, reader connected?");
                return -1;
            }
            else
            {
                logThis("RFID Connected");
                //This will set the Option IntermecSettings-RFID-Reader 1-Enable Reader
                //Configuring the DCE Transport Programmatically Using BRI
                Intermec.DataCollection.RFID.BasicBRIReader DCEConfig = new BasicBRIReader(null);
                try
                {
                    DCEConfig.Open("TCP://127.0.0.1:2189");
                    // BRI ‘device’ command.
                    DCEConfig.Execute("device 1 attrib adminstatus=on");
                    DCEConfig.Close();
                    return 0;
                }
                catch (BasicReaderException bex)
                {
                    logThis("SetAttrib() DCEConfig could not connect to DCE: " + bex.Message);
                    return -2;
                }
                catch (Exception ex)
                {
                    logThis("SetAttrib() DCEConfig exception " + ex.Message);
                    return -3;
                }
            }
        }

        private bool ExecuteCMD(string tCMD)
        {
            if (m_Reader == null)
                return false;
            byte[] aFieldSeparator = { (byte)' ' };
            byte[] aRespBuf;
            string tMsg = null;
            logThis("Sending->" + tCMD);
            try
            {
                if (m_Reader.IsConnected)
                    tMsg = m_Reader.Execute(tCMD);
                else
                {
                    logThis("Reader disconnected, please Connect");
                    return false;
                }
            }
            catch (BasicReaderException eBRI)
            {
                logThis("BasicReaderException: " + eBRI.ToString());
                return false;
            }
            catch (System.NullReferenceException nr)
            {
                logThis("Null Ref Exception: " + nr.Message);
                logThis("stack: " + nr.StackTrace);
                return false;
            }

            // if tags were successfully read
            if (tMsg.IndexOf("BRI ERR") >= 0)
                System.Diagnostics.Debugger.Break();
                //"H112233445566778899001122 HE2001040\r\nOK>"
               // System.Diagnostics.Debug.WriteLine("Msg: "+tMsg);
                //Show TAG ID
                #region READ EPCID/TAGID
                if (sCurrentCMD.StartsWith("READ ")) //TAGID
                {
                    if (tMsg.IndexOf("RDERR") >= 0)
                    {
                        //ChangeStatus(eStatus.TagReadError);
                        return false;
                    }
                    aRespBuf = tools.StrToByteArray(tMsg);
                    tags = BRIParser.GetTags(aRespBuf, aFieldSeparator);
                    System.Diagnostics.Debug.WriteLine("tags.Length: " + tags.Length);

                    if (tags == null)
                    {
                        return false;
                    }
                    System.Diagnostics.Debug.WriteLine("Msg: " + tMsg);
#if !NORMALTAG
                    #region ReadUserMemory
                    // remove newline and carriage returns
                    tMsg = tMsg.Replace('\n', ' ');
                    tMsg = tMsg.Replace("\r","");
                    string[] ids = tMsg.Split(' ');
                    // get the even arguments only
                    System.Diagnostics.Debug.WriteLine("id num " + ids.Length);
                    if (ids.Length > 1)
                    {
                        for (int j = 1; j < ids.Length; j+=2)
                        {
                            // System.Diagnostics.Debug.WriteLine("tag " +j+"  "+ tags[j].ToString());
                            string s = ids[j];
                            System.Diagnostics.Debug.WriteLine(j + "  Read id: " + s);
                            s = s.Substring(1,s.Length-1);
                            
                            for (int i = 0; i < s.Length; i++)
                            {
                                // remove first part of byte '3'
                                if (s.Substring(i, 1) == "3") s = s.Remove(i, 1);
                                else
                                {
                                    // chop off ending zeroes
                                    s = s.Substring(0, i);
                                    break;
                                }
                                //System.Diagnostics.Debug.WriteLine(i+"  length: "+start.Length+"   newString " + start);
                            }
                            // make multiple tags comma delimited
                            string sOut = "";
                            if (s.Length > 1)
                            {
                                //if (j != ids.Length - 2) s = s + ",";
                                for ( int i = 0; i < tags.Length; i++ )
                                {
                                    sOut += s[i];
                                    if (i != s.Length - 1)
                                        sOut += ",";
                                }
                                s = sOut;
                            }
                            __MyEvent(this, new MyEventArgs(s));
                        }
                    }
                    #endregion
#else
#region ReadNormalTagMemory
                            // make multiple tags comma delimited
                            string sOut = "";
                            if (tags.Length > 1)
                            {
                                //if (j != ids.Length - 2) s = s + ",";
                                for ( int i = 0; i < tags.Length; i++ )
                                {
                                    sOut += tags[i];
                                    if (i != tags.Length - 1)
                                        sOut += ",";
                                }                                
                            }
                            else{
                                sOut = tags[0].ToString();
                            }
                            __MyEvent(this, new MyEventArgs(sOut));
#endregion
#endif
                    /*
                    #region READ single TAG
                    if (tags.Length == 1)
                    {
                        //read data from the tag
                        System.Diagnostics.Debug.WriteLine("READ TAGID tag data: " + tags[0].ToString());
                    }
                    #endregion
                     * /
                     
                    //read tag data (aka EPCID)
                    /*
                    if (tMsg.StartsWith("H"))
                    {
                        string s;
                        s = tMsg.Substring(1, 24);
                        System.Diagnostics.Debug.WriteLine("Read data: " + s);
                        string[] args = tMsg.Split(' ');
                        if (args.Length > 1)
                        {
                            System.Diagnostics.Debug.WriteLine("Arg 2: " + args[1]);
                            string start = args[1].Substring(1,args[1].Length-1);
                            
                            for (int i = 0; i < start.Length; i++)
                            {
                                start = start.Remove(i, 1);
                                //System.Diagnostics.Debug.WriteLine(i+"  length: "+start.Length+"   newString " + start);
                            }
                            System.Diagnostics.Debug.WriteLine("final ver " + start);

                            __MyEvent(this, new MyEventArgs(start));
                        }
                        //txtTagID.Text = s;
                    }
                     */
                }
            #endregion
            #region ATTRIB
            if (sCurrentCMD.Equals("ATTRIB TAGTYPE=EPCC1G2") == true)
            {
                if (tMsg.IndexOf("OK") >= 0)
                    return true;
                else
                    return false;
            }
            if ((tMsg.IndexOf("ERR") >= 0) & (sCurrentCMD.Equals("ATTRIB SCHEDOPT=1") == false))
            {
                //MessageBox.Show("Warning, BRI ERR occured for: " + tCMD + "\r\n" + tMsg);
                logThis("BRI ERR for: " + tCMD + "\r\n" + tMsg);
                return false;
            }
            #endregion
            return true;
        }
        
        private void reportTAGS(string sTAGs)
        {
            /*
            Received event from thread: 
            ReaderEvent: Type=EVT_TAG
                EventData='H300833B2DDD9014035050000 MEMOVRN'
            id num 2
            1  Read id: MEMOVRN
            Received event from thread: 
            ReaderEvent: Type=EVT_TAG
                EventData='H3005FB63AC1F3681EC880468 H313233343536373839303132'
            id num 2
            1  Read id: H313233343536373839303132 
             * the above is the hex of the string 123456789012
            */

            #region ReadUserMemory
            // remove newline and carriage returns
            sTAGs = sTAGs.Replace('\n', ' ');
            sTAGs = sTAGs.Replace("\r", "");
            string[] ids = sTAGs.Split(' ');
            // get the even arguments only
            System.Diagnostics.Debug.WriteLine("id num " + ids.Length);
            if (ids.Length > 1)
            {
                for (int j = 1; j < ids.Length; j += 2)
                {
                    // System.Diagnostics.Debug.WriteLine("tag " +j+"  "+ tags[j].ToString());
                    string s = ids[j];
                    System.Diagnostics.Debug.WriteLine(j + "  Read id: " + s);
                    if(!s.StartsWith("H")) //is this a valid Hex string or possibly an error
                        return;
                    
                    s = s.Substring(1, s.Length - 1); //cut the H at the beginning

                    //convert hex back to string
                    int iErrCnt = 0;
                    byte[] b = Utility.HexEncoding.GetBytes(s, out iErrCnt);
                    s = System.Text.Encoding.Default.GetString(b, 0, b.Length); //this replaces the strange converter below
                    
                    ////for (int i = 0; i < s.Length; i++)
                    ////{
                    ////    // remove first part of byte '3'
                    ////    if (s.Substring(i, 1) == "3") s = s.Remove(i, 1);
                    ////    else
                    ////    {
                    ////        // chop off ending zeroes
                    ////        s = s.Substring(0, i);
                    ////        break;
                    ////    }
                    ////    //System.Diagnostics.Debug.WriteLine(i+"  length: "+start.Length+"   newString " + start);
                    ////}

                    //in continous mode REPORT=EVENT or EVENTALL we always get a separate event for each TAG reported
                    //so there will never be multiple TAGs in one event
                    __MyEvent(this, new MyEventArgs(s));
                }
            }
            #endregion
        }

        private int ParseResponseMessage(string tMsg)
        {
            int x = 0;
            string tString = "";
            char tChar = '0';

            //just to make parsing code uniform
            tMsg += "\r\n";

            char[] tMyCharList = tMsg.ToCharArray();

            //clear response list
            int RspCount = 0;

            string[] RspMsgList = new string[1000];
            RspMsgList.Initialize();

            //parse the response message
            for (x = 0; x < tMyCharList.Length; x++)
            {
                tChar = tMyCharList[x];
                if (tChar.Equals('\n') == false & tChar.Equals('\r') == false)
                {
                    tString += tChar;
                }
                else if (tChar.Equals('\r') == true)
                {
                    RspCount++;
                    RspMsgList[RspCount] = tString;
                    tString = "";
                }
            }

            //process the response messages
            for (x = 1; x <= RspCount; x++)
            {
                if (RspMsgList[x].IndexOf("H") == 0 & RspMsgList[x].IndexOf("HOP") < 0)
                {
                    //Tag Data
                    if (sCurrentCMD.Equals("R"))
                    {
                        //skip this data, its part of a bug work around regarding cont read modes
                    }
                    else
                    {
                        logThis(RspMsgList[x]);
                    }
                }
                else if (RspMsgList[x].IndexOf("OK>") == 0)
                {
                    //end of reader response
                    if (sCurrentCMD.StartsWith("R") == false & sCurrentCMD.StartsWith("W") == false)
                    {
                        logThis(RspMsgList[x]);
                    }
                    break;
                }
                else if (sCurrentCMD.IndexOf("ATTRIB") == 0)
                {
                    logThis(RspMsgList[x]);
                }
                else if (sCurrentCMD.IndexOf("UTIL") == 0)
                {
                    logThis(RspMsgList[x]);
                }
            }//END for (x = 1; x <= RspCount; x++)

            return 0;
        }
        private void ConnectReader()
        {
            Cursor.Current = Cursors.WaitCursor;
            System.Diagnostics.Debug.WriteLine("Opening Reader...");
            if (OpenReader() == 0)
            {
                System.Diagnostics.Debug.WriteLine("... Reader opened. Trying to set Attributes ...");
                //try first command
                int res = SetAttributes();
                if (res != 0)
                {
                    System.Diagnostics.Debug.WriteLine("... SetAttributes failed! Closing Reader");
                    m_Reader.Close();
                    m_Reader = null;
                }
            }

            if (m_Reader != null)
            {
                if (m_Reader.IsConnected)
                {
                    System.Diagnostics.Debug.WriteLine("Reader connected. ChangeStatus(ReadBarcode)");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Reader connected. ChangeStatus(ReadBarcode)");
                }
            }

            Cursor.Current = Cursors.Default;
        }
        private void CloseReader()
        {
            if (m_Reader != null)
                try
                {
                    if (m_Reader != null)
                        m_Reader.Close();
                    RemoveEventHandlers();
                }
                catch (BasicReaderException brx)
                {
                    logThis("CloseReader(): " + brx.Message);
                }
                catch (SystemException sx)
                {
                    logThis("CloseReader(): " + sx.Message);
                }
                finally
                {
                    //ChangeStatus(eStatus.Offline);
                }
        }
        /// <summary>
        /// Add the event handler to handle the tag events and trigger pulls.
        /// Not all of these are used but added as samples of what are available.
        /// </summary>
        /// <returns></returns>
        private int AddEventHandlers()
        {
            try
            {
                //this.m_Reader.EventHandlerRadio += new Radio_EventHandler(brdr_EventHandlerRadio);
                //this.m_Reader.EventHandlerTag += new Tag_EventHandler(brdr_EventHandlerTag);

                //deprecated from v2.1: this.m_Reader.EventHandlerCenterTrigger += new CenterTrigger_EventHandler(brdr_EventHandlerCenterTrigger);
                //deprecated from v2.1: this.m_Reader.EventHandlerDCE += new DCE_EventHandler(brdr_EventHandlerDCE);

                this.m_Reader.ReaderEvent += new BasicEvent(brdr_ReaderEventHandler);

                //this.m_Reader.EventHandlerOverflow += new Overflow_EventHandler(brdr_EventHandlerOverflow);
            }
            catch
            {
                MessageBox.Show("Exception trying to create event handlers");
                return -1;
            }
            return 0;
        }

        public static void logThis(string s)
        {
            // System.Diagnostics.Debug.WriteLine("LOG: " + s);
            try
            {
                System.IO.TextWriter tw = new System.IO.StreamWriter("\\RFIDWegdeLog.txt", true);
                string dt = DateTime.Now.ToShortTimeString();
                tw.WriteLine(dt + ": " + s);
                tw.Flush();
                tw.Close();

            }
            catch (Exception x)
            {
                System.Diagnostics.Debug.WriteLine("Exception: " + x.Message);
            }

        }
        private int RemoveEventHandlers()
        {
            try
            {
                //this.m_Reader.EventHandlerRadio -= brdr_EventHandlerRadio;
                //this.m_Reader.EventHandlerTag -= brdr_EventHandlerTag;

                //deprecated from v2.1: this.m_Reader.EventHandlerCenterTrigger -= brdr_EventHandlerCenterTrigger;
                //deprecated from v2.1: this.m_Reader.EventHandlerDCE -= brdr_EventHandlerDCE;

                this.m_Reader.ReaderEvent -= brdr_ReaderEventHandler;

                //this.m_Reader.EventHandlerOverflow -= brdr_EventHandlerOverflow;
            }
            catch
            {
                MessageBox.Show("Exception trying to create event handlers");
                return -1;
            }
            return 0;
        }
        void brdr_ReaderEventHandler(object sender, BasicReaderEventArgs EvtArgs)
        {
            System.Diagnostics.Debug.WriteLine("ReaderEvent: Type=" + EvtArgs.EventType.ToString() +
                "\n\tEventData='" + EvtArgs.EventData + "'");
            switch ((int)EvtArgs.EventType)
            {
                /*            
                    OLD:    UNKNOWN = 1,
                            SHUTDOWN = 2,
                            BUTTON = 3,
                            DEVICE = 4,
                    EventTypes.EVT_BATTERY
                    EventTypes.EVT_CENTER_TRIGGER
        EventTypes.EVT_DCE
Battery pulled
                 * ReaderEvent: Type=EVT_DCE
	                    EventData='DEVICE 1 DISCONNECTED'
                    ReaderEvent: Type=EVT_DCE
	                    EventData='DEVICE 1 CONNECTING'
                    The thread 0xc66b4f1a has exited with code 0 (0x0).
                    ReaderEvent: Type=EVT_DCE
	                    EventData='DEVICE 1 CONNECTED'
Center Trigger
                 * ReaderEvent: Type=EVT_TRIGGER
	                    EventData='TRIGPULL GPIO 0'
                    ReaderEvent: Type=EVT_DCE
	                    EventData='BUTTON CENTER 1'
                    ReaderEvent: Type=EVT_TRIGGER
	                    EventData='TRIGRELEASE GPIO 1'
                    ReaderEvent: Type=EVT_DCE
	                    EventData='BUTTON CENTER 0'

                    EventTypes.EVT_RADIO
                    EventTypes.EVT_READER_RECONNECTED
                    EventTypes.EVT_RESET
                    EventTypes.EVT_TAG
                    EventTypes.EVT_THERMAL
                    EventTypes.EVT_TRIGGER
                    EventTypes.EVT_TRIGGERACTION
                */
                case (int)BasicReaderEventArgs.EventTypes.EVT_RADIO:
                    brdr_EventHandlerRadio(sender, EvtArgs);
                    break;
                case (int)BasicReaderEventArgs.EventTypes.EVT_DCE: // EVT_CENTER_TRIGGER:
                    if (m_bEventLogging)
                        logThis("brdr_ReaderEvent() BUTTON:" + EvtArgs.EventData);
                    brdr_EventHandlerCenterTrigger(sender, EvtArgs);
                    break;
                case (int)BasicReaderEventArgs.EventTypes.EVT_TAG:
                    reportTAGS(EvtArgs.EventData);
                    break;
                case (int)BasicReaderEventArgs.EventTypes.EVT_BATTERY:
                case (int)BasicReaderEventArgs.EventTypes.EVT_CENTER_TRIGGER:
                case (int)BasicReaderEventArgs.EventTypes.EVT_READER_RECONNECTED:
                case (int)BasicReaderEventArgs.EventTypes.EVT_RESET:
                case (int)BasicReaderEventArgs.EventTypes.EVT_THERMAL:
                case (int)BasicReaderEventArgs.EventTypes.EVT_TRIGGER:
                case (int)BasicReaderEventArgs.EventTypes.EVT_TRIGGERACTION:
                case (int)BasicReaderEventArgs.EventTypes.EVT_UNKNOWN:
                    if (m_bEventLogging)
                    {
                        logThis("brdr_ReaderEvent() type: " + Convert.ToString((Int16)EvtArgs.EventType) + "=" + EvtArgs.EventType.ToString());
                        logThis("brdr_ReaderEvent() data: '" + EvtArgs.EventData + "'");
                    }
                    break;
                default:
                    // The DCE has encountered an undefined condition...
                    if (m_bEventLogging)
                        logThis("brdr_ReaderEvent(): Unknown Type " + Convert.ToString((Int16)EvtArgs.EventType) + " - Data: '" + EvtArgs.EventData + "'");
                    //CloseReader();
                    break;
            }
        }
        void brdr_EventHandlerRadio(object sender, BasicReaderEventArgs EvtArgs)
        {
            int iTimeLeft = Convert.ToInt32(EvtArgs.EventData);
            //pause a little bit to avoid duty cycle failures
            logThis("duty time left=" + iTimeLeft.ToString() + "Sleeping 500ms...");
            System.Threading.Thread.Sleep(500);
            logThis("...OK");
            //MessageBox.Show("brdr_EventHandlerRadio():\r\n" + iTimeLeft.ToString());
            if (m_bEventLogging)
                logThis("brdr_EventHandlerRadio() TimeLeft: " + iTimeLeft.ToString());
        }

    }
}
