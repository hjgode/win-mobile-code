#define ASYNCHRONOUS
#define USE_SCANNER
/*
 * define USE_SCANNER to use barcode scanner code
 * define ASYNCHRONOUS to use the sensor in async mode
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Intermec.Device;
#if USE_SCANNER
using Intermec.DataCollection;
#endif

using NativeSync; // scanner control events

namespace SensorScan5
{
    public partial class mainFrm : Form
    {
        private Sensor mySensor;
        //private Timer myTimer; //a windows forms timer, inaccurate
#if !ASYNCHRONOUS
        bgThread myThread;
#endif
        ShakeDetection.ShakeClass1 shaker1;
        ShakeDetection.ShakeClass2 shaker2;
        ShakeDetection.ShakeClass3 shaker3;
        ShakeDetection.ShakeClass4 shaker4;
        ShakeDetection.ShakeClass5 shaker5;
        ShakeDetection.ShakeClass6 shaker6;
        ShakeDetection.ShakeClass7 shaker7;
        ShakeDetection.ShakeClass8 shaker8;
        ShakeDetection.ShakeClass9 shaker9;

        Movedetection.MovementClass1 move1;

#if USE_SCANNER
        //scan event control
        ScanEvents scanEvents;
        BarcodeReader bcr;
        Timer scanTimer;
        int scanTimeout = 20; //switch scanner off after xx seconds
#endif
        private bool starting = true;
        public mainFrm()
        {
            InitializeComponent();
#if ASYNCHRONOUS
            try
            {
                mySensor = new Sensor(this, Sensor.SensorsEnabled.AccelerometerEnabled, true);
            }
            catch (Exception)
            {
                MessageBox.Show("No sensor available. Did you install SensorCab runtime? Exit");
                Application.Exit();
            }
            //call mySensor_AccelerationEvent for Acceleration events from sensor
            mySensor.AccelerationMinimumNotifyDelta = 0;
            mySensor.SensorDataUpdateInterval = 10;
            mySensor.AccelerationEvent += new AccelerationEventHandler(mySensor_AccelerationEvent);
#else
            //mySensor = new Sensor(this, Sensor.SensorsEnabled.AccelerometerEnabled, false);
#endif
            cbSensity.SelectedIndex = 3; //default is 0.01, 0 means all events
            trackBar1.Value = 100;

            //mySensor.SensorDataUpdateInterval = 50;

            //
            shaker1 = new ShakeDetection.ShakeClass1("shake1");
            shaker1.shakeTreshold = 8.0;
            //shaker1.logEnabled = true;
            //better implementation
            shaker1.ShakeDetected += new ShakeDetection.ShakeClass.ShakeDetectedEventHandler(shaker1_ShakeDetected);
            //
            shaker2 = new ShakeDetection.ShakeClass2("shake2");
            //shaker2.logEnabled = true;
            shaker2.ShakeDetected += new ShakeDetection.ShakeClass.ShakeDetectedEventHandler(shaker2_ShakeDetected);

            //
            shaker3 = new ShakeDetection.ShakeClass3("shake3");
            //shaker3.logEnabled = true;
            shaker3.ShakeDetected += new ShakeDetection.ShakeClass.ShakeDetectedEventHandler(shaker3_ShakeDetected);

            //
            shaker4 = new ShakeDetection.ShakeClass4("shake4");
            //shaker4.logEnabled = true;
            shaker4.ShakeDetected += new ShakeDetection.ShakeClass.ShakeDetectedEventHandler(shaker4_ShakeDetected);

            //
            shaker5 = new ShakeDetection.ShakeClass5("shake5");
            trackBar5.Value = 8; trackBar1_ValueChanged(this, null);
            //shaker5.logEnabled = true;
            shaker5.ShakeDetected += new ShakeDetection.ShakeClass.ShakeDetectedEventHandler(shaker5_ShakeDetected);

            //
            shaker6 = new ShakeDetection.ShakeClass6("shake6");
            //shaker6.logEnabled = true;
            shaker6.ShakeDetected += new ShakeDetection.ShakeClass.ShakeDetectedEventHandler(shaker6_ShakeDetected);
            trackBar6.Value = 120;

            //
            shaker7 = new ShakeDetection.ShakeClass7("shake7");
            //shaker7.logEnabled = true;
            shaker7.ShakeDetected += new ShakeDetection.ShakeClass.ShakeDetectedEventHandler(shaker7_ShakeDetected);

            //
            shaker8 = new ShakeDetection.ShakeClass8("shake8");
            //shaker8.logEnabled = true;
            shaker8.ShakeDetected += new ShakeDetection.ShakeClass.ShakeDetectedEventHandler(shaker8_ShakeDetected);

            //
            shaker9 = new ShakeDetection.ShakeClass9("shake9");
            //shaker8.logEnabled = true;
            shaker9.ShakeDetected += new ShakeDetection.ShakeClass.ShakeDetectedEventHandler(shaker9_ShakeDetected);

            //
            move1 = new Movedetection.MovementClass1("move1");
            move1.logEnabled = true;
            move1.MoveDetected += new Movedetection.MovementClass.MoveDetectedEventHandler(move1_MoveDetected);
            move1.IdleDetected += new Movedetection.MovementClass.IdleDetectedEventHandler(move1_IdleDetected);

#if !ASYNCHRONOUS
            //myTimer.Interval = 50; //approx 20 events per second
            //myTimer.Tick += new EventHandler(myTimer_Tick);
            //myTimer.Enabled = true; //do not enable timer before assigning the shaker/move classes
            //myTimer.Enabled = true;
            myThread = new bgThread();
            myThread.bgThreadEvent += new bgThread.bgThreadEventHandler(myThread_bgThreadEvent);
#endif
            //perfChart1.maxDecimal = 1500;
            perfChart1._SymmetricDisplay= true;
            perfChart1._ScaleMode = SpPerfChart.ScaleMode.Relative;
            //perfChart2.maxDecimal = 1500;
            perfChart2._SymmetricDisplay = true;
            perfChart2._ScaleMode = SpPerfChart.ScaleMode.Relative;
            //perfChart3.maxDecimal = 1500;
            perfChart3._SymmetricDisplay = true;
            perfChart3._ScaleMode = SpPerfChart.ScaleMode.Relative;

            perfChart4._SymmetricDisplay = true;
            perfChart4._ScaleMode = SpPerfChart.ScaleMode.Relative;

            starting = false;

#if USE_SCANNER
            scanEvents = new ScanEvents();
            try
            {
                bcr = new BarcodeReader();
                bcr.ContinuesScan = true;
                scanTimer = new Timer();
                scanTimer.Interval = 1000; //fire every second
                scanTimer.Tick += new EventHandler(scanTimer_Tick);
                scanTimer.Enabled = true;
            }
            catch (Exception)
            {
                //scanner not available                
            }
#endif
        }
#if USE_SCANNER
        private int scanTimeCounter = 0;
        private bool bScannerIsOn = false;
        /// <summary>
        /// light the scanner for xx seconds
        /// restart 'timer' if a scan was detected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void scanTimer_Tick(object sender, EventArgs e)
        {
            scanTimeCounter++;
            if (scanTimeCounter > scanTimeout)
            {
                bcr.ScannerOn = false;
                bScannerIsOn = false;
                scanTimeout = 0;
            }
        }
#endif
        void myThread_bgThreadEvent(object sender, ShakeDetection.ShakeEventArgs bte)
        {
            processData(bte._gvector);
        }

        void shaker9_ShakeDetected(object sender, ShakeDetection.ShakeEventArgs bre)
        {
            shakeLabel(lblShake9);
        }

        void move1_IdleDetected(object sender, Movedetection.MovementEventArgs bre)
        {
            //System.Diagnostics.Debug.WriteLine("Idle event fired");
            //get actual delta average
            double d = move1._Avg_deltaG_rms;
            perfChart4._AddValue((decimal)d);
        }

        void move1_MoveDetected(object sender, Movedetection.MovementEventArgs bre)
        {
            //System.Diagnostics.Debug.WriteLine("Move event fired");
            shakeLabel(lblShake9);
        }

        void shaker8_ShakeDetected(object sender, ShakeDetection.ShakeEventArgs bre)
        {
            shakeLabel(lblShake8);
        }

        void shaker7_ShakeDetected(object sender, ShakeDetection.ShakeEventArgs bre)
        {
            shakeLabel(lblShake7);
        }

        void shaker6_ShakeDetected(object sender, ShakeDetection.ShakeEventArgs bre)
        {
            shakeLabel(lblShake6);
        }

        void shaker5_ShakeDetected(object sender, ShakeDetection.ShakeEventArgs bre)
        {
            shakeLabel(lblShake5);
        }

        void shaker4_ShakeDetected(object sender, ShakeDetection.ShakeEventArgs bre)
        {
            shakeLabel(lblShake4);   
        }

        void shaker3_ShakeDetected(object sender, ShakeDetection.ShakeEventArgs bre)
        {
            shakeLabel(lblShake3);
#if USE_SCANNER
            if (!bScannerIsOn)
            {
                //scanEvents.doScanButton();  //use for remote control
                bcr.ScannerOn = true;
                bScannerIsOn = true;
            }
            scanTimeout = 0; //reset scanner timeout
#endif
        }

        void shaker2_ShakeDetected(object sender, ShakeDetection.ShakeEventArgs e)
        {
            shakeLabel(lbShake2);
        }

        void shaker1_ShakeDetected(object sender, ShakeDetection.ShakeEventArgs bre)
        {
            shakeLabel(lblShake1);
        }


        void mySensor_AccelerationEvent(object sender, Sensor.AccelerationArgs AccelerationArgs)
        {
            ShakeDetection.GVector gv = new ShakeDetection.GVector(AccelerationArgs.GForceX,
                AccelerationArgs.GForceY,
                AccelerationArgs.GForceZ);
            
            processData(gv);
        }

        void formatChart(ref SpPerfChart.PerfChart pc)
        {
            pc._SymmetricDisplay = true;
            pc._ScaleMode = SpPerfChart.ScaleMode.Relative;
        }

        void myTimer_Tick(object sender, EventArgs e)
        {
            ShakeDetection.GVector gv = new ShakeDetection.GVector(mySensor.Acceleration.GForceX,
                mySensor.Acceleration.GForceY,
                mySensor.Acceleration.GForceZ);
            processData(gv);
        }

        private void processData(ShakeDetection.GVector gv)
        {
            if (starting)
                return;
            labelGFX.Text = gv.X.ToString("0.00000");
            labelGFY.Text = gv.Y.ToString("0.00000");
            labelGFZ.Text = gv.Z.ToString("0.00000");

            lblOrientation.Text = gv.ToScreenOrientation().ToString();
            lblDirection.Text = gv.direction.ToString();

            lblTilt.Text = gv.Tilt.ToString("0");
            lblRoll.Text = gv.Roll.ToString("0");            
            lblPitch.Text = gv.Pitch.ToString("0");

            lblAcceleration.Text = gv.Length.ToString("0.0");

            addLog(gv.ToString());

            shaker1.addValues(gv);

            shaker2.addValues(gv);

            shaker3.addValues(gv);

            shaker4.addValues(gv);

            shaker5.addValues(gv);

            shaker6.addValues(gv);

            shaker7.addValues(gv);

            shaker8.addValues(gv);

            shaker9.addValues(gv);

            Movedetection.GMVector gmv = new Movedetection.GMVector(gv.X, gv.Y, gv.Z);
            move1.addValues(gmv);

            perfChart1._AddValue((decimal)gv.X);
            perfChart2._AddValue((decimal)gv.Y);
            perfChart3._AddValue((decimal)gv.Z);
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            //myTimer.Enabled = false;
            if (MessageBox.Show("Exit?", "SensorScan", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
#if !ASYNCHRONOUS
                if (myThread != null)
                    myThread.Dispose();
#endif
                Application.Exit();
            }
        }

        delegate void SetTextCallback(string text);
        private void addLog(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.txtLog.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(addLog);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                if (txtLog.Text.Length > 2000)
                    txtLog.Text = "";
                txtLog.Text += text + "\r\n";
                txtLog.SelectionLength = text.Length;
                txtLog.SelectionStart = txtLog.Text.Length - text.Length;
                txtLog.ScrollToCaret();
            }
        }
        delegate void SetShakeLabelCallback(Label lb);
        void shakeLabel(Label lb)
        {
            if (lb.InvokeRequired)
            {
                SetShakeLabelCallback d = new SetShakeLabelCallback(shakeLabel);
                this.Invoke(d, new object[] { lb });
            }
            else
            {
                lb.BackColor = Color.Red;
                Application.DoEvents();
                System.Threading.Thread.Sleep(50);
                lb.BackColor = Color.White;
                Application.DoEvents();
            }
        }

        private void cbSensity_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (starting)
                return;
            mySensor.AccelerationMinimumNotifyDelta = float.Parse(cbSensity.SelectedItem.ToString());
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            shaker1.shakeTreshold = (double) numericUpDown1.Value;
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            float d = trackBar1.Value / 100f;
            txtShake2Treshold.Text = d.ToString("0.00");
            Application.DoEvents();
            if(shaker2!=null)
                shaker2.shakeTreshold = (double)d;
        }

        private void trackBar2_ValueChanged(object sender, EventArgs e)
        {
            float d = trackBar2.Value / 100f;
            txtShake3Treshold.Text = d.ToString("0.00");
            Application.DoEvents();
            if(shaker3!=null)
                shaker3.shakeTreshold = (double)d;
        }

        private void trackBar4_ValueChanged(object sender, EventArgs e)
        {
            float d = trackBar4.Value / 100f;
            txtShake4Treshold.Text = d.ToString("0.00");
            Application.DoEvents();
            if (shaker4 != null)
                shaker4.shakeTreshold = (double)d;

        }

        private void trackBar5_ValueChanged(object sender, EventArgs e)
        {
            //need treshold around 800
            double d = trackBar5.Value * 100;
            txtShake5Treshold.Text = d.ToString();
            Application.DoEvents();
            if (shaker5 != null)
                shaker5.shakeTreshold = d;
        }

        private void trackBar6_ValueChanged(object sender, EventArgs e)
        {
            float d = trackBar6.Value / 100f;
            txt6Treshold.Text = d.ToString("0.00");
            Application.DoEvents();
            if (shaker6 != null)
                shaker6.shakeTreshold = (double)d;
        }

        private void btnTEST_Click(object sender, EventArgs e)
        {
            //scanEvents.doScanButton();
            shaker3_ShakeDetected(this, new ShakeDetection.ShakeEventArgs(new ShakeDetection.GVector(0, 0, -.8)));    
        }

    }
}