//
// Copyright (c) Microsoft Corporation.  All rights reserved.
//
//
// Use of this source code is subject to the terms of the Microsoft end-user
// license agreement (EULA) under which you licensed this SOFTWARE PRODUCT.
// If you did not accept the terms of the EULA, you are not authorized to use
// this source code. For a copy of the EULA, please see the LICENSE.RTF on your
// install media.
using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.WindowsMobile.Samples.Location;
using System.IO;
using System.Text;
using OpenNETCF.IO.Serial;
using PInvokeLibrary2;

namespace GpsTest
{
    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    public class Form1 : System.Windows.Forms.Form
    {
        private System.Windows.Forms.MenuItem exitMenuItem;
        private System.Windows.Forms.MainMenu mainMenu1;
        private MenuItem menuItem2;


        private EventHandler updateDataHandler;
        GpsDeviceState device = null;
        GpsPosition position = null;
        private Microsoft.WindowsCE.Forms.InputPanel inputPanel1;

        Gps gps = new Gps();
        private Timer timer1;

        Port serialPort;
        private ListBox lbl_RawData;
        private TextBox status;
        private MenuItem GPSID_submenu;
        private MenuItem mnuGPSStart;
        private MenuItem mnuGPSStop;
        private MenuItem mnuGPSRestart;
        private MenuItem mnusubGPSRaw;
        private MenuItem mnuRAWStop;
        private MenuItem mnuRAWStart;
        private MenuItem mnuRAWRestart;
        private MenuItem mnusubLogFiles;
        private MenuItem mnuGPSLogOnOff;
        private MenuItem mnuGPSLogClear;
        private MenuItem mnuXMLLogOnOff;

        bool m_FixValid=false;
        bool m_AutoRestart = true;
        bool m_XMLLogging = true;
        bool m_RAWLogging = true;
        string RAWLoggingPath = @"\GPSraw.log.txt";
        private MenuItem mnuSetTime2GPS;
        private MenuItem mnusubSpecial;
        private MenuItem mnuNavInit;
        private Panel panel1;

        bool m_SetTime = false;
        /// <summary>
        /// used to store and retrieve SN values
        /// </summary>
        private Satellite[] sats;
        private MenuItem mnuAutoRestartGPS;
        private MenuItem subGPSdevice;
        private MenuItem mnuRestartGPSdevice;
        private MenuItem mnuStartRAW2;
        /// <summary>
        /// used to store 'Sat is used in Solution'
        /// </summary>
        private Satellite[] satSol;

        public Form1()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            inputPanel1.Enabled = false;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            base.Dispose( disposing );
        }
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.exitMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.GPSID_submenu = new System.Windows.Forms.MenuItem();
            this.mnuGPSStart = new System.Windows.Forms.MenuItem();
            this.mnuGPSStop = new System.Windows.Forms.MenuItem();
            this.mnuGPSRestart = new System.Windows.Forms.MenuItem();
            this.mnuAutoRestartGPS = new System.Windows.Forms.MenuItem();
            this.mnusubGPSRaw = new System.Windows.Forms.MenuItem();
            this.mnuRAWStop = new System.Windows.Forms.MenuItem();
            this.mnuRAWStart = new System.Windows.Forms.MenuItem();
            this.mnuRAWRestart = new System.Windows.Forms.MenuItem();
            this.mnusubLogFiles = new System.Windows.Forms.MenuItem();
            this.mnuGPSLogOnOff = new System.Windows.Forms.MenuItem();
            this.mnuGPSLogClear = new System.Windows.Forms.MenuItem();
            this.mnuXMLLogOnOff = new System.Windows.Forms.MenuItem();
            this.mnuSetTime2GPS = new System.Windows.Forms.MenuItem();
            this.mnusubSpecial = new System.Windows.Forms.MenuItem();
            this.mnuNavInit = new System.Windows.Forms.MenuItem();
            this.subGPSdevice = new System.Windows.Forms.MenuItem();
            this.mnuRestartGPSdevice = new System.Windows.Forms.MenuItem();
            this.inputPanel1 = new Microsoft.WindowsCE.Forms.InputPanel();
            this.timer1 = new System.Windows.Forms.Timer();
            this.lbl_RawData = new System.Windows.Forms.ListBox();
            this.status = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.mnuStartRAW2 = new System.Windows.Forms.MenuItem();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.exitMenuItem);
            this.mainMenu1.MenuItems.Add(this.menuItem2);
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Text = "Exit";
            this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.MenuItems.Add(this.GPSID_submenu);
            this.menuItem2.MenuItems.Add(this.mnusubGPSRaw);
            this.menuItem2.MenuItems.Add(this.mnusubLogFiles);
            this.menuItem2.MenuItems.Add(this.mnuSetTime2GPS);
            this.menuItem2.MenuItems.Add(this.mnusubSpecial);
            this.menuItem2.MenuItems.Add(this.subGPSdevice);
            this.menuItem2.Text = "Extras";
            // 
            // GPSID_submenu
            // 
            this.GPSID_submenu.MenuItems.Add(this.mnuGPSStart);
            this.GPSID_submenu.MenuItems.Add(this.mnuGPSStop);
            this.GPSID_submenu.MenuItems.Add(this.mnuGPSRestart);
            this.GPSID_submenu.MenuItems.Add(this.mnuAutoRestartGPS);
            this.GPSID_submenu.Text = "GPSID driver";
            // 
            // mnuGPSStart
            // 
            this.mnuGPSStart.Text = "Start GPS";
            this.mnuGPSStart.Click += new System.EventHandler(this.startGpsMenuItem_Click);
            // 
            // mnuGPSStop
            // 
            this.mnuGPSStop.Text = "Stop GPS";
            this.mnuGPSStop.Click += new System.EventHandler(this.stopGpsMenuItem_Click);
            // 
            // mnuGPSRestart
            // 
            this.mnuGPSRestart.Text = "Restart GPS";
            this.mnuGPSRestart.Click += new System.EventHandler(this.mnu_RestartGPS_Click);
            // 
            // mnuAutoRestartGPS
            // 
            this.mnuAutoRestartGPS.Checked = true;
            this.mnuAutoRestartGPS.Text = "Auto Restart";
            this.mnuAutoRestartGPS.Click += new System.EventHandler(this.mnuAutoRestartGPS_Click);
            // 
            // mnusubGPSRaw
            // 
            this.mnusubGPSRaw.MenuItems.Add(this.mnuRAWStop);
            this.mnusubGPSRaw.MenuItems.Add(this.mnuRAWStart);
            this.mnusubGPSRaw.MenuItems.Add(this.mnuRAWRestart);
            this.mnusubGPSRaw.MenuItems.Add(this.mnuStartRAW2);
            this.mnusubGPSRaw.Text = "GPS Raw";
            // 
            // mnuRAWStop
            // 
            this.mnuRAWStop.Enabled = false;
            this.mnuRAWStop.Text = "Stop RAW";
            this.mnuRAWStop.Click += new System.EventHandler(this.mnuRAWStop_Click);
            // 
            // mnuRAWStart
            // 
            this.mnuRAWStart.Text = "Start RAW";
            this.mnuRAWStart.Click += new System.EventHandler(this.mnuRAWStart_Click);
            // 
            // mnuRAWRestart
            // 
            this.mnuRAWRestart.Text = "Restart RAW";
            this.mnuRAWRestart.Click += new System.EventHandler(this.mnuRAWRestart_Click);
            // 
            // mnusubLogFiles
            // 
            this.mnusubLogFiles.MenuItems.Add(this.mnuGPSLogOnOff);
            this.mnusubLogFiles.MenuItems.Add(this.mnuGPSLogClear);
            this.mnusubLogFiles.MenuItems.Add(this.mnuXMLLogOnOff);
            this.mnusubLogFiles.Text = "Log Files";
            // 
            // mnuGPSLogOnOff
            // 
            this.mnuGPSLogOnOff.Text = "GPS logging off";
            this.mnuGPSLogOnOff.Click += new System.EventHandler(this.mnuGPSLogOnOff_Click);
            // 
            // mnuGPSLogClear
            // 
            this.mnuGPSLogClear.Text = "GPS clear log file";
            this.mnuGPSLogClear.Click += new System.EventHandler(this.mnuGPSLogClear_Click);
            // 
            // mnuXMLLogOnOff
            // 
            this.mnuXMLLogOnOff.Text = "XML logging off";
            this.mnuXMLLogOnOff.Click += new System.EventHandler(this.mnuXMLLogOnOff_Click);
            // 
            // mnuSetTime2GPS
            // 
            this.mnuSetTime2GPS.Checked = true;
            this.mnuSetTime2GPS.Text = "Set Time to GPS";
            this.mnuSetTime2GPS.Click += new System.EventHandler(this.mnuSetTime2GPS_Click);
            // 
            // mnusubSpecial
            // 
            this.mnusubSpecial.MenuItems.Add(this.mnuNavInit);
            this.mnusubSpecial.Text = "Special";
            // 
            // mnuNavInit
            // 
            this.mnuNavInit.Text = "NavigationInit";
            this.mnuNavInit.Click += new System.EventHandler(this.mnuNavInit_Click);
            // 
            // subGPSdevice
            // 
            this.subGPSdevice.MenuItems.Add(this.mnuRestartGPSdevice);
            this.subGPSdevice.Text = "GPS Device";
            // 
            // mnuRestartGPSdevice
            // 
            this.mnuRestartGPSdevice.Text = "Restart Device";
            this.mnuRestartGPSdevice.Click += new System.EventHandler(this.mnuRestartGPSdevice_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // lbl_RawData
            // 
            this.lbl_RawData.Location = new System.Drawing.Point(3, 193);
            this.lbl_RawData.Name = "lbl_RawData";
            this.lbl_RawData.Size = new System.Drawing.Size(234, 72);
            this.lbl_RawData.TabIndex = 2;
            // 
            // status
            // 
            this.status.AcceptsReturn = true;
            this.status.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.status.Font = new System.Drawing.Font("Courier New", 8F, System.Drawing.FontStyle.Regular);
            this.status.Location = new System.Drawing.Point(3, 4);
            this.status.Multiline = true;
            this.status.Name = "status";
            this.status.ReadOnly = true;
            this.status.Size = new System.Drawing.Size(233, 133);
            this.status.TabIndex = 1;
            this.status.Text = "status";
            this.status.WordWrap = false;
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(3, 138);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(234, 55);
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // mnuStartRAW2
            // 
            this.mnuStartRAW2.Text = "Start Direct RAW";
            this.mnuStartRAW2.Click += new System.EventHandler(this.mnuStartRAW2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.status);
            this.Controls.Add(this.lbl_RawData);
            this.Menu = this.mainMenu1;
            this.Name = "Form1";
            this.Text = "GpsSample (logger)";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.Closed += new System.EventHandler(this.Form1_Closed);
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        static void Main() 
        {
            Application.Run(new Form1());
        }

        //http://www.highoncoding.com/Articles/532_How%20to%20access%20GPS%20in%20Windows%20Mobile%20devices.aspx
        private delegate void MessageHandler(ListBox control, string message);

        private void UpdateListBoxControl(ListBox control, string message)
        {
            //avoid overrun
            if (control.Items.Count > 200)
                control.Items.Clear();
            control.Items.Add(message);
            control.SelectedIndex = control.Items.Count - 1;
        }
        //==================================================================
        private void AddRawText(string s)
        {
            if (s.Length != 0)
            {
                DateTime dt = DateTime.Now;
                SetTimeToGPS(dt);
                MessageHandler lb = UpdateListBoxControl;
                Invoke(lb, lbl_RawData, s);
                //lbl_RawData.Items.Add(s);
                //lbl_RawData.SelectedIndex = lbl_RawData.Items.Count - 1;
                if (m_RAWLogging)
                {
                    try
                    {
                        StreamWriter sw = System.IO.File.AppendText(RAWLoggingPath);
                        sw.Write("{0:00}.{1:00}.{2:0000} {3:00}:{4:00}:{5:00}\t", dt.Day, dt.Month, dt.Year, dt.Hour, dt.Minute, dt.Second);
                        sw.WriteLine(s);
                        sw.Flush();
                        sw.Close();
                    }
                    catch (SystemException)
                    {
                        lbl_RawData.Items.Add("exception in appendText");
                    }
                }
            }
        }

        #region CN50raw
        bgThread2 myStreamReaderThread;
        private void OpenStream()
        {
            //background thread already running?
            if (myStreamReaderThread == null)
            {
                string szPort="";
                szPort = GetGPSPort();
                if (szPort != "")
                {
                    AddRawText("Start reading stream at '" + szPort +"'");
                    //start a new thread
                    myStreamReaderThread = new bgThread2(szPort);
                    myStreamReaderThread.bgThread2Event += new bgThread2.bgThread2EventHandler(myStreamReaderThread_bgThread2Event);
                }
                else
                    AddRawText("No raw GPS port found");
            }
        }

        private void OpenStream(string szPort)
        {
            //background thread already running?
            if (myStreamReaderThread == null)
            {
                if (szPort != "")
                {
                    AddRawText("Start reading stream at '" + szPort + "'");
                    //start a new thread
                    myStreamReaderThread = new bgThread2(szPort);
                    myStreamReaderThread.bgThread2Event += new bgThread2.bgThread2EventHandler(myStreamReaderThread_bgThread2Event);
                }
                else
                    AddRawText("No raw GPS port found");
            }
        }
        void myStreamReaderThread_bgThread2Event(object sender, bgThread2.BgThreadEventArgs bte)
        {
            AddRawText(bte.sString);
        }
        private void CloseStream()
        {
            if (myStreamReaderThread != null)
            {
                myStreamReaderThread.Dispose();
                Application.DoEvents();
                myStreamReaderThread = null;
            }
            Application.DoEvents();
            mnuRAWStart.Enabled = true;
            mnuRAWStop.Enabled = false;
        }
        #endregion
        #region Serial
        private void OpenCOM(string szPort)
        {
            try
            {
                serialPort = new Port(szPort, new HandshakeNone());
                AddRawText("Trying to open " + szPort + ":...");
                serialPort.Settings.BaudRate = BaudRates.CBR_57600;
                serialPort.RThreshold = 1;
                serialPort.InputLen = 1;
                serialPort.DataReceived +=
                    new OpenNETCF.IO.Serial.Port.CommEvent(serialPort_DataReceived);
                serialPort.Open();
                AddRawText("port opened...");
            }
            catch (OpenNETCF.IO.Serial.CommPortException sx)
            {
                AddRawText("Exception in OpenCOM " + szPort);
                AddRawText(sx.Message);
                mnuRAWStart.Enabled = true;
                mnuRAWStop.Enabled = false;
            }
        }

        private void OpenCOM()
        {
            string szPort="";
            try
            {
                szPort = GetGPSPort();
                if (szPort == "")
                    szPort = "GPD1:";

                serialPort = new Port(szPort, new HandshakeNone());
                AddRawText("Trying to open " + szPort + ":...");
                serialPort.Settings.BaudRate = BaudRates.CBR_57600;
                serialPort.RThreshold = 1;
                serialPort.InputLen = 1;
                serialPort.DataReceived +=
                    new OpenNETCF.IO.Serial.Port.CommEvent(serialPort_DataReceived);
                serialPort.Open();
                AddRawText("port opened...");
            }
            catch(OpenNETCF.IO.Serial.CommPortException sx)
            {
                AddRawText("Exception in OpenCOM " + szPort);
                AddRawText(sx.Message);
                mnuRAWStart.Enabled = true;
                mnuRAWStop.Enabled = false;
            }
        }
        private void CloseCOM()
        {
            if (serialPort != null)
                serialPort.Close();
        }

        private void serialPort_DataReceived()
        {
            byte[] inputData = new byte[1];
            string s = String.Empty;
            while (serialPort.InBufferCount > 0)
            {
                inputData = serialPort.Input;
                if ((inputData[0] != '\n') && (inputData[0] != '\r'))
                    s += Encoding.ASCII.GetString(inputData, 0, inputData.Length);
                else
                {
                    //parseNmeaSentence(s);
                    AddRawText(s);
                    s = string.Empty;
                }
            }
        }
#endregion

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            if (gps.Opened)
            {
                gps.Close();
            }
            CloseCOM();
            CloseStream();
            Application.Exit();
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
            updateDataHandler = new EventHandler(UpdateData);
         
            status.Text = "";
            
            //status.Width = Screen.PrimaryScreen.WorkingArea.Width;
            //status.Height = (Screen.PrimaryScreen.WorkingArea.Height - btn_Restart.Height)/2;

            //lbl_RawData.Height=(Screen.PrimaryScreen.WorkingArea.Height - btn_Restart.Height)/2;

            gps.DeviceStateChanged += new DeviceStateChangedEventHandler(gps_DeviceStateChanged);
            gps.LocationChanged += new LocationChangedEventHandler(gps_LocationChanged);
            if (!this.gps.Opened)
            {
                this.gps.Open();
                status.Text += "gps.Open()\r\n";
            }
            if (this.gps.Opened)
            {
                gps.DeviceStateChanged += new DeviceStateChangedEventHandler(gps_DeviceStateChanged);
                gps.LocationChanged += new LocationChangedEventHandler(gps_LocationChanged);
            }
            else
            {
                status.Text += "gps.open() FAILED\r\n";
            }
            if (isCN50orCK70())
                mnuNavInit.Enabled = false;

            mnuGPSStart.Enabled = false;
            mnuGPSStop.Enabled = true;
            timer1.Interval = 30000;
            timer1.Enabled=true;
            BeginInvoke(updateDataHandler);
            //OpenCOM();
        }

        protected void gps_LocationChanged(object sender, LocationChangedEventArgs args)
        {
            position = args.Position;

            // call the UpdateData method via the updateDataHandler so that we
            // update the UI on the UI thread
            try
            {
                BeginInvoke(updateDataHandler);

            }
            catch (NullReferenceException )
            {
                
            }
        }
        ////http://www.highoncoding.com/Articles/532_How%20to%20access%20GPS%20in%20Windows%20Mobile%20devices.aspx
        //private delegate void MessageHandler(object sender, DeviceStateChangedEventArgs args);

        //private void UpdateControl(object sender, DeviceStateChangedEventArgs args)
        //{
        //    UpdateData(sender, args);
        //    //control.Text = message;
        //}
        //==========================================================================
        void gps_DeviceStateChanged(object sender, DeviceStateChangedEventArgs args)
        {
            ////http://www.highoncoding.com/Articles/532_How%20to%20access%20GPS%20in%20Windows%20Mobile%20devices.aspx
            //MessageHandler dev_change = UpdateControl; 

            //device = args.DeviceState;
            //Invoke(dev_change, status, device);
            //Invoke(cu, tbGPSDeviceState, device.DeviceState.ToString());
            //Invoke(cu, tbGPSServiceState, device.ServiceState.ToString());
        
            // call the UpdateData method via the updateDataHandler so that we
            // update the UI on the UI thread
            try
            {
                BeginInvoke(updateDataHandler);
            }
            catch (NullReferenceException)
            {
            }
        }
        void UpdateData(object sender, System.EventArgs args)
        {
            try
            {
                if (gps.Opened)
                {
                    string str = "";
                    if (device != null)
                    {
                        str = "Name: '" + device.FriendlyName + "' Svc: '" + device.ServiceState + "', Dvc: '" + device.DeviceState + "'\r\n";
                    }
                    else
                        str = "Name: '' Svc:''\r\n";

                    if (position != null)
                    {
                        //if all valid
                        if ((((((this.position.LatitudeValid && this.position.LongitudeValid) && this.position.SatellitesInSolutionValid) && this.position.SatellitesInViewValid) && this.position.SatelliteCountValid) && this.position.TimeValid) && (this.position.SatelliteCount > 2))
                        {
                            m_FixValid = true;
                            if (m_XMLLogging)
                                this.position2XMLFile(this.position);
                        }
                        else
                            m_FixValid = false;

                        //Latitude
                        if (position.LatitudeValid)
                        {
                            str += "Lat: " + position.Latitude.ToString("00.000") + " ";
                            //str += "Lat (D,M,S): " + position.LatitudeInDegreesMinutesSeconds + "\r\n";
                        }
                        else
                            str += "Lat: --.--- ";

                        //Longitude
                        if (position.LongitudeValid)
                        {
                            str += "Lon: " + position.Longitude.ToString("00.000") + "\r\n";
                            //str += "Lon (D,M,S): " + position.LongitudeInDegreesMinutesSeconds + "\r\n";
                        }
                        else
                            str += "Lon: --.---\r\n";

                        //sat in view and in solution
                        if (position.SatellitesInSolutionValid &&
                            position.SatellitesInViewValid &&
                            position.SatelliteCountValid)
                        {
                            str += "Sat. Count: " + position.GetSatellitesInSolution().Length + "/" +
                                position.GetSatellitesInView().Length + " (" +
                                position.SatelliteCount + ")\r\n";
                        }
                        //list sats in solution
                        if (position.SatellitesInSolutionValid)
                        {
                            satSol = position.GetSatellitesInSolution();
                        }
                        //list sats in view and there signal
                        if (position.SatellitesInViewValid)
                        {
                            /* Satellite[] */
                            sats = position.GetSatellitesInView();
                            str += "SV:";
                            foreach (Satellite sat in sats)
                            {
                                str += sat.Id.ToString("00") + " ";
                            }
                            str += "\r\nSN:";
                            foreach (Satellite sat in sats)
                            {
                                str += sat.SignalStrength.ToString("00") + " ";
                            }
                            str += "\r\n";
                            panel1.Invalidate();
                        }
                        else
                        {
                            //panel1.Invalidate();
                        }
                        //time
                        if (position.TimeValid)
                        {
                            str += "Time: " + position.Time.ToString() + "\r\n";
                            if (m_FixValid)
                            {
                                SetTimeToGPS(position.Time);
                            }
                        }

                        //fixtype
                        FixType fx = position.eFixType;
                        switch (fx)
                        {
                            case FixType.Unknown: str += "FixType: Unknown\r\n";
                                break;
                            case FixType.XyD: str += "FixType: 2D\r\n";
                                break;
                            case FixType.XyzD: str += "FixType: 3D\r\n";
                                break;
                        }
                        //fix quality
                        FixQuality fq = position.eFixQuality;
                        switch (fq)
                        {
                            case FixQuality.Unknown: str += "FixQuality: Unknown\r\n";
                                break;
                            case FixQuality.Gps: str += "FixQuality: GPS\r\n";
                                break;
                            case FixQuality.DGps: str += "FixQuality: DGPS\r\n";
                                break;
                        }

                    }
                    panel1.Invalidate();

                    status.Text = str;

                }
            }
            catch (NullReferenceException)
            {
                
            }
        }

        private void Form1_Closed(object sender, System.EventArgs e)
        {
            if (gps.Opened)
            {
                gps.Close();
            }
        }
        private void StopGPS()
        {
            if (gps.Opened)
            {
                gps.Close();
            }

            mnuGPSStart.Enabled = true;
            mnuGPSStop.Enabled = false;
        }
        private void stopGpsMenuItem_Click(object sender, EventArgs e)
        {
            StopGPS();
        }
        private void StartGPS()
        {
            if (!gps.Opened)
            {
                gps.Open();
                status.Text += "StartGPS()...\r\n";
            }

            mnuGPSStart.Enabled = false;
            mnuGPSStop.Enabled = true;
        }
        private void startGpsMenuItem_Click(object sender, EventArgs e)
        {
            StartGPS();
        }
        private void position2XMLFile(GpsPosition position)
        {
            DateTime dateTime1;
            using (StreamWriter streamWriter1 = new StreamWriter("bcPosition.xml"))
            {
                streamWriter1.WriteLine("<?xml version0x03d\"1.0\"?>");
                streamWriter1.WriteLine("<position>");
                streamWriter1.Write("<latitude>");
                streamWriter1.Write(position.Latitude);
                streamWriter1.WriteLine("</latitude>");
                streamWriter1.Write("<longitude>");
                streamWriter1.Write(position.Longitude);
                streamWriter1.WriteLine("</longitude>");
                streamWriter1.Write("<speed>");
                streamWriter1.Write(position.Speed);
                streamWriter1.WriteLine("</speed>");
                streamWriter1.Write("<cog>");
                streamWriter1.Write(position.Heading);
                streamWriter1.WriteLine("</cog>");
                streamWriter1.Write("<elev>");
                streamWriter1.Write(position.SeaLevelAltitude);
                streamWriter1.WriteLine("</elev>");
                streamWriter1.Write("<satsolution>");
                streamWriter1.Write(position.SatelliteCount);
                streamWriter1.WriteLine("</satsolution>");
                streamWriter1.Write("<ticks>");
                dateTime1 = position.Time;
                streamWriter1.Write(dateTime1.Ticks);
                streamWriter1.WriteLine("</ticks>");
                streamWriter1.Write("<jsdate>");
                streamWriter1.Write(((dateTime1 = position.Time).ToString("MM\'/\'dd\'/\'yyyy hh:mm:ss") + " UTC+0000"));
                streamWriter1.WriteLine("</jsdate>");
                streamWriter1.Write("<isodate>");
                streamWriter1.Write(((dateTime1 = position.Time).ToString("s") + "Z"));
                streamWriter1.WriteLine("</isodate>");
                streamWriter1.WriteLine("</position>");
            }
        }
        private void GPSRestart()
        {
            m_FixValid = false;
            status.Text = "Restarting GPS ...\r\n";
            Application.DoEvents();
            StopGPS();
            System.Threading.Thread.Sleep(1000);
            StartGPS();
            status.Text = "GPS has been restarted\r\n";
            Application.DoEvents();
        }
        private void btn_Restart_Click(object sender, EventArgs e)
        {
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!m_FixValid)
                btn_Restart_Click(sender, e);
        }

        private string GetGPSPort()
        {
            string szStr="";
            if (Registry.GetStringValue(Registry.HKLM,
                            "System\\CurrentControlSet\\GPS Intermediate Driver\\Multiplexer",
                            "DriverInterface",
                            ref szStr)
                == 0)
            {
                return szStr;
            }
            else 
            {
                if (Registry.GetStringValue(Registry.HKLM,
                    "System\\CurrentControlSet\\GPS Intermediate Driver\\Drivers",
                    "CurrentDriver",
                    ref szStr) == 0)
                {
                    string szPath = "System\\CurrentControlSet\\GPS Intermediate Driver\\Drivers\\" + szStr;
                    if (Registry.GetStringValue(Registry.HKLM, szPath, "CommPort", ref szStr) == 0)
                    {
                        return szStr;
                    }
                }
            }
            return "";
        }
        private string GetGPSRAWdirectPort()
        {
            string szStr = "";
            if (Registry.GetStringValue(Registry.HKLM,
                            "System\\CurrentControlSet\\GPS Intermediate Driver\\Drivers\\SWIGPSModem",
                            "CommPort",
                            ref szStr)
                == 0)
            {
                return szStr;
            }
            return "";
        }
        
        private void SetTimeToGPS(DateTime UTCtime)
        {
            if (m_SetTime)
            {
                // Get the local time zone and a base Coordinated Universal 
                // Time (UTC).
                TimeZone localZone = TimeZone.CurrentTimeZone;
                DateTime baseUTC = UTCtime; // new DateTime(2000, 1, 1);

                System.Diagnostics.Debug.WriteLine("\nLocal time: {0}\n",
                    localZone.StandardName);

                // Calculate the local time and UTC offset.
                DateTime localTime = localZone.ToLocalTime(baseUTC);
                TimeSpan localOffset =
                    localZone.GetUtcOffset(localTime);

                System.Diagnostics.Debug.WriteLine(string.Format("{0,-20:yyyy-MM-dd HH:mm}" +
                    "{1,-20:yyyy-MM-dd HH:mm}{2,-12}{3}",
                    baseUTC, localTime, localOffset,
                    localZone.IsDaylightSavingTime(localTime)));
                //adjust the clock
                //localTime += localOffset;
                PInvokeLibrary.SystemTimeLib.SetTime(localTime);
                m_SetTime = false;
            }
        }

        private void mnu_RestartGPS_Click(object sender, EventArgs e)
        {
            GPSRestart();
        }

        private void mnuRAWStop_Click(object sender, EventArgs e)
        {
            if (isCN50orCK70())
                CloseStream();
            else
                CloseCOM();
            mnuRAWStop.Enabled = false;
            mnuRAWStart.Enabled = true;
            mnuStartRAW2.Enabled = true;

        }

        private bool isCN50orCK70()
        {
            string sResult = "nn";
            try
            {
                Registry.GetStringValue(Registry.HKLM, "Platform", "Model", ref sResult);
                if (sResult.Substring(0, 4) == "CN50" || 
                    sResult.Substring(0, 4) == "CK70" || 
                    sResult.Substring(0, 4) == "CN70")
                    return true;
                else
                    return false;
            }
            catch { 
            }
            return false;
        }
        private void mnuRAWStart_Click(object sender, EventArgs e)
        {
            if (isCN50orCK70())
                OpenStream();
            else
                OpenCOM();
            mnuRAWStart.Enabled = false;
            mnuRAWStop.Enabled = true;
        }

        private void mnuRAWRestart_Click(object sender, EventArgs e)
        {
            if (isCN50orCK70())
                CloseStream();
            else
                CloseCOM();
            Application.DoEvents();
            System.Threading.Thread.Sleep(1000);
            if (isCN50orCK70())
                OpenStream();
            else
                OpenCOM();
            Application.DoEvents();
        }

        private void mnuGPSLogOnOff_Click(object sender, EventArgs e)
        {
            if (m_RAWLogging)
            {
                m_RAWLogging = false;
                mnuGPSLogOnOff.Text = "GPS logging On";
            }
            else 
            {
                m_RAWLogging = true;
                mnuGPSLogOnOff.Text = "GPS logging Off";
            }
        }

        private void mnuGPSLogClear_Click(object sender, EventArgs e)
        {
            bool oldval = m_RAWLogging;
            m_RAWLogging = false;
            Application.DoEvents();
            try
            {
                System.IO.File.Delete(RAWLoggingPath);
            }
            catch(SystemException x){
                AddRawText("Exception in LogClear() " + x.Message);
            }
            m_RAWLogging = oldval;
        }

        private void mnuXMLLogOnOff_Click(object sender, EventArgs e)
        {
            if (m_XMLLogging)
            {
                m_XMLLogging = false;
                mnuXMLLogOnOff.Text = "XML logging on";
            }
            else
            {
                m_XMLLogging = true;
                mnuXMLLogOnOff.Text = "XML logging off";
            }
        }

        private void mnuSetTime2GPS_Click(object sender, EventArgs e)
        {
            if (m_SetTime)
            {
                m_SetTime = false;
                mnuSetTime2GPS.Checked = false;
            }
            else
            {
                m_SetTime = true;
                mnuSetTime2GPS.Checked = true;
            }
        }

        // Calculates the checksum for a sentence
        private static string getChecksum(string sentence)
        {
            //Start with first Item
            int checksum = Convert.ToByte(sentence[sentence.IndexOf('$') + 1]);
            
            //first cheksum starts with first char after $
            checksum = Convert.ToByte(sentence[sentence.IndexOf('$') + 1]);
            // Loop through all chars to get a checksum
            for (int i = sentence.IndexOf('$') + 2; i < sentence.IndexOf('*'); i++)
            {
                // No. XOR the checksum with this character's value
                
                checksum ^= Convert.ToByte(sentence[i]);
            }
            // Return the checksum formatted as a two-character hexadecimal
            return checksum.ToString("X2");
        }

        private void SendRAWData(string s)
        {
            if (serialPort.IsOpen)
            {
                // Create an ASCII encoding.
                Encoding ascii = Encoding.ASCII;

                //add Cr/Lf
                s+="\r\n";
                Byte[] encodedBytes = ascii.GetBytes(s);
                try
                {
                    serialPort.Output = encodedBytes;
                    AddRawText("Send: " + s);
                }
                catch (OpenNETCF.IO.Serial.CommPortException cx)
                {
                    System.Diagnostics.Debug.WriteLine("Exception in SerialOutput: " + cx.Message + "\r\n");
                }
            }
        }

        //NavigationInitialization
        private void SendNavigationInitialization()
        {
            //string NavigationInitialization="$PSRF101,-2686700,-4304200,3851624,96000,497260,921,12,3*";//1C";
            string ColdStart = "$PSRF101,0,0,0,0,0,0,12,6*";
            /*
               1. COLD START : $PSRF101,0,0,0,000,0,0,12,6*12
               2. WARM START : $PSRF101,0,0,0,000,0,0,12,2*16
               3. HOT START : $PSRF101,0,0,0,000,0,0,12,1*15
               4. FACTORY RESET : $PSRF101,0,0,0,000,0,0,12,8*1C
            */
            string s=ColdStart;
            string strChk = getChecksum(s);
            s += strChk;
            SendRAWData(s);
        }

        private void mnuNavInit_Click(object sender, EventArgs e)
        {
            StopGPS();
            Application.DoEvents();
            SendNavigationInitialization();
            Application.DoEvents();
            StartGPS();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                int h = panel1.Height; //39
                int w = panel1.Width;  //234
                int count = 0;
                //System.Diagnostics.Debug.WriteLine(string.Format("h={0} / w={1}", h, w));
                if(sats!=null)
                    count = sats.Length;
                if (count == 0)
                    return;
                int[] SNs = new int[count]; //to store signal
                bool[] inSolution= new bool[count]; //to store sat is in solution
                int i,j;

                for (i = 0; i < sats.Length; i++)
                {
                    inSolution[i] = false;
                    if (satSol != null)
                    {
                        for (j = 0; j < satSol.Length; j++)
                        {
                            if (sats[i].Id == satSol[j].Id)
                                inSolution[i] = true;
                        }
                    }
                }

                for (i=0; i < count ; i++) //foreach (Satellite sat in sats)
                {
                    SNs[i] = sats[i].SignalStrength; // is about 15 to 40
                }

                System.Drawing.SolidBrush b;
                System.Drawing.Rectangle r;

                int rx, ry, rw, rh;
                if (count > 0)
                {
                    for (i = 0; i < count; i++) //foreach (Satellite sat in sats)
                    {
                        //adjust the color
                        b = new System.Drawing.SolidBrush(Color.LightGray);
                        if (SNs[i] == 0)
                        {
                            b = new System.Drawing.SolidBrush(Color.Red);
                            if (System.Diagnostics.Debugger.IsAttached)
                                SNs[i] = 3 * i + 1;
                            else
                                SNs[i] = 1;
                        }
                        else if (SNs[i] > 1 && SNs[i] < 28)
                            b = new System.Drawing.SolidBrush(Color.Yellow);
                        else if (SNs[i] >= 28)
                            b = new System.Drawing.SolidBrush(Color.Magenta);

                        if (inSolution[i])
                            b = new System.Drawing.SolidBrush(Color.LightGreen);

                        rx = (i * 18); // +15;
                        ry = h - 1;
                        rw = 15;
                        rh = -SNs[i];
                        r = new System.Drawing.Rectangle(rx, ry, rw, rh);// (mLeftMargin + (1 * mThick) + (bar * 20), mMaxHeihgt - height, 5, height);
                        //System.Diagnostics.Debug.WriteLine(string.Format("i={4} x={0} y={1} w={2} h={3}", rx, ry, rw, rh, i));
                        e.Graphics.FillRectangle(b, r);
                    }
                }
                else
                {
                    b = new System.Drawing.SolidBrush(Color.LightGray);
                    e.Graphics.DrawString("no data", new Font("Arial", 8, FontStyle.Regular), b, 6, 6);

                }
            }
            catch (Exception ee)
            {
                System.Diagnostics.Debug.WriteLine(ee.ToString());
            }
        }

        private void mnuAutoRestartGPS_Click(object sender, EventArgs e)
        {
            m_AutoRestart = !m_AutoRestart;
            mnuAutoRestartGPS.Checked = m_AutoRestart;

        }
        private void RestartGPSdevice()
        {
            if (GpsDeviceControl.SendIoctl(GpsDeviceControl.IOCTL_SERVICE_STOP)==0)
            {
                AddRawText("Service stopped");
                if (GpsDeviceControl.SendIoctl(GpsDeviceControl.IOCTL_SERVICE_START) == 0)
                    AddRawText("Service started");
                else
                    AddRawText("Start Service failed");
            }
            else
                AddRawText("Stop Service failed");
        }

        private void mnuRestartGPSdevice_Click(object sender, EventArgs e)
        {
            RestartGPSdevice();
        }

        private void mnuStartRAW2_Click(object sender, EventArgs e)
        {

            //OpenCOM("WMP6:"); // WMP6: SWI6: //both failed
            //OpenCOM("SWI6:"); 
            //OpenStream("SWI6:"); //OK but no DATA
//            OpenStream("WMP6:"); //OK but no DATA
//            return;

            string directPort = GetGPSRAWdirectPort();
            if (directPort != "")
                OpenCOM(directPort);
            mnuRAWStart.Enabled = false;
            mnuStartRAW2.Enabled = false;
            mnuRAWStop.Enabled = true;

        }
    }
}
