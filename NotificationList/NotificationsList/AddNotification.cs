using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using OpenNETCF.WindowsCE.Notification;

namespace NotificationsList
{
    public partial class AddNotification : Form
    {
        public AddNotification()
        {
            InitializeComponent();
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void menuItem2_Click(object sender, EventArgs e)
        {
            OpenNETCF.WindowsCE.Notification.UserNotificationTrigger myTrig = new UserNotificationTrigger();

            if (!System.IO.File.Exists(txtApp.Text))
            {
                txtApp.BackColor = Color.Red;
                return;
            }
            myTrig.Application=txtApp.Text;
            
            myTrig.Arguments=txtArg.Text;

            try
            {
                myTrig.StartTime=DateTime.Parse(txtStart.Text);
            }
            catch (Exception)
            {
                txtStart.BackColor = Color.Red;                
                return;
            }
            try
            {
                myTrig.EndTime = DateTime.Parse(txtEnd.Text);
            }
            catch (Exception)
            {
                txtEnd.BackColor = Color.Red;
                return;
            }

            myTrig.Event= NotificationEvent.None;

            myTrig.Type=NotificationType.Time;

            UserNotification myUN = new UserNotification();
            

            int iRes = OpenNETCF.WindowsCE.Notification.Notify.SetUserNotification(myTrig, null);
            if (iRes != 0)
                MessageBox.Show("Added new UserNotification");
            else
                MessageBox.Show("Adding new UserNotification failed");

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Application|*.exe";
            ofd.FilterIndex = 0;
            if (ofd.ShowDialog() == DialogResult.OK)
                txtApp.Text = ofd.FileName;
            ofd.Dispose();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker1.Value = dateTimePicker2.Value;            
        }

        private void btnSetStart_Click(object sender, EventArgs e)
        {
            txtStart.Text = dateTimePicker1.Value.ToString();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void txtApp_TextChanged(object sender, EventArgs e)
        {
            txtApp.BackColor = Color.White;
        }

        private void txtStart_TextChanged(object sender, EventArgs e)
        {
            txtStart.BackColor = Color.White;
        }

        private void txtEnd_TextChanged(object sender, EventArgs e)
        {
            txtEnd.BackColor = Color.White;
        }

        private void btnSetEnd_Click(object sender, EventArgs e)
        {
            txtEnd.Text = dateTimePicker1.Value.ToString();
        }
    }
}