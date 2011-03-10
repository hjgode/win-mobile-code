using System;
using System.Collections.Generic;
using System.Text;

namespace NativeSync
{
    class ScanEvents
    {
        private static class Logging
        {
            public static bool logEnabled = false;
            public static void doLog(string s)
            {
                if(logEnabled)
                    System.Diagnostics.Debug.WriteLine(s);
            }
        }
        /// <summary>
        /// defines the time the scan button is pressed down
        /// </summary>
        public int scanHoldTime = 100;
        private SystemEvent scanEvent;// = new SystemEvent("StateLeftScan", false, false);
        private SystemEvent scanEventDelta;// = new SystemEvent("DeltaLeftScan", false, false);

        public ScanEvents()
        {
            scanEvent = new SystemEvent("StateLeftScan", false, false);
            scanEventDelta = new SystemEvent("DeltaLeftScan", false, false);
        }
        public void doScanButton()
        {
            //Set StateLeftScan, Set DeltaLeftScan, wait a bit, 
            //Unset StateLeftScan, Set DeltaLeftScan

            if (scanEvent.SetEvent())
                Logging.doLog("set StateLeftScan event OK");
            else
                Logging.doLog("set StateLeftScan event failed!");

            if (scanEventDelta.SetEvent())
                Logging.doLog("set scanEventDelta event OK");
            else
                Logging.doLog("set scanEventDelta event failed!");

            System.Threading.Thread.Sleep(scanHoldTime);

            if (scanEvent.ResetEvent())
                Logging.doLog("reset StateLeftScan event OK");
            else
                Logging.doLog("reset StateLeftScan event failed!");

            if (scanEventDelta.SetEvent())
                Logging.doLog("set scanEventDelta2 event OK");
            else
                Logging.doLog("set scanEventDelta2 event failed!");
        }
    }
}
