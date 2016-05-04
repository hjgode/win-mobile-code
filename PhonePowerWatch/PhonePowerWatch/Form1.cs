using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PhonePowerWatch
{
    public partial class Form1 : Form
    {
        PhonePower pp=null;
        Timer timer1 = new Timer();
        public Form1()
        {
            InitializeComponent();
            pp = new PhonePower();

            //add an update handler
            pp.updateEvent += new PhonePower.updateEventHandler(pp_updateEvent);

            //add a timed query using ssapi
            timer1.Interval = 3000;
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Enabled = true;
        }

        void timer1_Tick(object sender, EventArgs e)
        {
            if (pp.getPhonePower())
            {                
                lblSSAPIstate.Text = "Phone Power is ON";
            }
            else
                lblSSAPIstate.Text = "Phone Power is OFF";
        }

        void pp_updateEvent(object sender, PhonePower.MyEventArgs eventArgs)
        {
            if (eventArgs.stateRadioIsOff)
            {                
                lblStatus.Text = "Phone Power is OFF";
            }
            else
                lblStatus.Text = "Phone Power is ON";
        }

        private void Form1_Closing(object sender, CancelEventArgs e)
        {
            if (pp != null)
                pp.Dispose();
        }

        private void btnON_Click(object sender, EventArgs e)
        {
            pp.switchPhoneOnOff(true);
        }

        private void btnOFF_Click(object sender, EventArgs e)
        {
            pp.switchPhoneOnOff(false);
        }
    }
}