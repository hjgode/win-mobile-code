using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace SetTimeTest
{
    public partial class TimeZoneBiasUC : UserControl
    {
        public TimeZoneBiasUC()
        {
            InitializeComponent();
        }

        public void setInfo(string name, TimeClass.SYSTEMTIME st, int bias)
        {
            txtName.Text = name;
            txtBias.Text = bias.ToString();
            StringBuilder sb = new StringBuilder();
            sb.Append(String.Format("({3:D1}) {0:D4}-{1:D2}-{2:D2} {4:D2}:{5:D2}:{6:D2}.{7:D3}",
                st.year, st.month, st.day, st.dayOfWeek,
                st.hour, st.minute, st.second, st.milliseconds));

            txtSystemTime.Text = sb.ToString();

            if (st.year == 0)
            {
                //absolute date this year
                string s = TimeClass.getDayInMonth(st);
                txtDateTime.Text = s;
            }
            else
            {
                txtDateTime.Text = txtSystemTime.Text;
            }

        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
