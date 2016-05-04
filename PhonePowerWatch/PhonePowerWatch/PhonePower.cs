using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using Microsoft.WindowsMobile.Status;
using Intermec.DeviceManagement.SmartSystem;

namespace PhonePowerWatch
{
    class PhonePower:IDisposable
    {
        static ITCSSApi _ssAPI=null;
        SystemState phoneState = new SystemState(SystemProperty.PhoneRadioOff);
        /*  <Subsystem Name="WWAN Radio">
                <Field Name="Radio Power State">1</Field> 
            </Subsystem>
        */
        const string phonePowerXML = "<Subsystem Name=\"WWAN Radio\"><Field Name=\"Radio Power State\">{0}</Field></Subsystem>";
        const string phonePowerXMLquery = "<Subsystem Name=\"WWAN Radio\"><Field Name=\"Radio Power State\"></Field></Subsystem>";
        static bool bStopThread = false;
        System.Threading.Thread myThread;

        public PhonePower()
        {
            if (_ssAPI == null)
                _ssAPI = new ITCSSApi();
            phoneState.Changed += new ChangeEventHandler(phoneState_Changed);
            //myThread = new System.Threading.Thread(new System.Threading.ThreadStart(watchThread));
            //myThread.Start();
        }

        public bool getPhonePower()
        {
            if (_ssAPI == null)
                return false;
            bool bPower = false;
            StringBuilder sb = new StringBuilder(1024);
            int dSize = 1024;
            uint uError = _ssAPI.Get(phonePowerXMLquery, sb, ref dSize, 1000);
            if (uError == ITCSSErrors.E_SS_SUCCESS)
            {
                //look for field status
                int pos = sb.ToString().IndexOf("</Field>");
                string sRes = sb.ToString().Substring(pos - 1, 1);

                if (sRes == "1")
                    bPower = true;
                else
                    bPower = false;
            }
            return bPower;
        }

        static void watchThread()
        {
            try
            {
                StringBuilder sb = new StringBuilder(1024);
                int dSize = 1024;
                while (!bStopThread)
                {
                    _ssAPI.Get(phonePowerXMLquery, sb, ref dSize, 1000);
                    //look for field status
                    int pos = sb.ToString().IndexOf("</Field>");
                    string sRes = sb.ToString().Substring(pos - 1, 1);
                    
                    if(sRes=="1")
                        System.Diagnostics.Debug.WriteLine("SSAPI: Phone is ON");
                    else
                        System.Diagnostics.Debug.WriteLine("SSAPI: Phone is ON");
                    System.Threading.Thread.Sleep(1000);
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
            }
            catch (Exception)
            {

            }
        }

        void phoneState_Changed(object sender, ChangeEventArgs args)
        {
            bool newState = false;
            if (Convert.ToBoolean(SystemProperty.PhoneRadioOff)==true)
                newState=true;//phone off
            else
                newState = false; //phone on
            MyEventArgs a = new MyEventArgs(newState);
            onUpdateHandler(a);
        }

        public void switchPhoneOnOff(bool on)
        {
            StringBuilder sb = new StringBuilder(1024);
            int dSize=1024;
            string sXML = String.Format(phonePowerXML, on ? 1 : 0);
            System.Diagnostics.Debug.WriteLine("switching Phone Radio to: " + (on ? "on" : "off"));
            uint uError = _ssAPI.Set(sXML, sb, ref dSize, 2000);
            if (uError != ITCSSErrors.E_SS_SUCCESS)
            {
                System.Diagnostics.Debug.WriteLine("SSAPI error: " + uError.ToString() + "\n" + sb.ToString().Substring(0, dSize));

            }
        }

        public void Dispose()
        {
            if (_ssAPI != null)
            {
                _ssAPI.Dispose();
                _ssAPI = null;
            }
            if (myThread != null)
            {
                bStopThread = true;
                System.Threading.Thread.Sleep(2000);
                if (myThread != null)
                    myThread.Abort();
            }
        }

        //event/delegate #1
        public class MyEventArgs : EventArgs
        {
            //fields
            public bool stateRadioIsOff { get; set; }
            public MyEventArgs(bool s)
            {
                stateRadioIsOff = s;
            }
        }
        public delegate void updateEventHandler(object sender, MyEventArgs eventArgs);
        public event updateEventHandler updateEvent;
        void onUpdateHandler(MyEventArgs args)
        {
            //anyone listening?
            if (this.updateEvent == null)
                return;
            MyEventArgs a = args;
            this.updateEvent(this, a);
        }
    }

}
