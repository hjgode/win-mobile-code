using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace SetTimeTest
{
    public partial class SystemTimeUC : UserControl
    {
        public SystemTimeUC(TimeClass.SYSTEMTIME st, string sType)
        {
            InitializeComponent();
            this.setType(sType);
            this.setDate(st);
            this.setTime(st);

        }
        public SystemTimeUC()
        {
            InitializeComponent();
            lblType.Text = "Design";
            txtDate.Text = "yyyy-mm-dd";
            txtTime.Text = "HH:MM:ss";
        }
        public void setDateTime(TimeClass.SYSTEMTIME st)
        {
            txtDate.Text = TimeClass.st2dateStr(st);
            this.setTime(st);
        }
        public void setDate(TimeClass.SYSTEMTIME st)
        {
            txtDate.Text = TimeClass.st2dateStr(st);
        }
        public void setTime(TimeClass.SYSTEMTIME st)
        {
            txtTime.Text = TimeClass.st2TimeStr(st);
        }
        public void setType(String st)
        {
            lblType.Text = st;
        }

    }
}
