using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace disableTouchCS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            disableTouch.UnLock();
            lblStatus.Text = "unlocked?";
        }

        private void btnLockTouch_Click(object sender, EventArgs e)
        {
            if (disableTouch.Lock())
                lblStatus.Text = "locked?";
            else
                lblStatus.Text = "unknown error";
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            MessageBeeps.MessageBeep(MessageBeeps.beepType.OK);
        }
    }
}