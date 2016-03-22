using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Threading;

namespace SetTimeTest
{
    public partial class TimeInfo : Form
    {
        System.Windows.Forms.Timer timer1;

        TimeClass.SYSTEMTIME st;
        TimeClass.SYSTEMTIME stLocal;

        public TimeInfo()
        {
            InitializeComponent();
            timer1 = new System.Windows.Forms.Timer();
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = 1000;
            timer1.Enabled = true;
            
            TimeClass.GetSystemTime(out st);
            systemTimeUC1.setDateTime(st);
            systemTimeUC1.setType("System Time");

            TimeClass.GetLocalTime(out stLocal);
            systemTimeUC2.setDateTime(stLocal);
            systemTimeUC2.setType("Local Time");
        }

        void timer1_Tick(object sender, EventArgs e)
        {
            TimeClass.GetSystemTime(out st);
            systemTimeUC1.setDateTime(st);
            systemTimeUC1.setType("System Time");

            TimeClass.GetLocalTime(out stLocal);
            systemTimeUC2.setDateTime(stLocal);
            systemTimeUC2.setType("Local Time");

            test();
        }

        void test()
        {
            TimeClass.TimeZoneInformation tzi;
            int iStatus = TimeClass.GetTimeZoneInformation(out tzi);
            TimeClass.TIME_ZONE_ID idStatus = (TimeClass.TIME_ZONE_ID )iStatus;

            txtTZstatus.Text = idStatus.ToString();
            txtBias.Text = tzi.bias.ToString();

            timeZoneBiasUC1.setInfo(tzi.standardName, tzi.standardDate, tzi.standardBias);

            timeZoneBiasUC2.setInfo(tzi.daylightName, tzi.daylightDate, tzi.daylightBias);

            lblAutoDST.Text = TimeClass.isAutoDST() ? "AutoDST is ON" : "AutoDST is OFF";
        }

        private void mnuRefresh_Click(object sender, EventArgs e)
        {
            test();
        }
    }
}