using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.IO;
using OpenNETCF.WindowsCE.Notification;

namespace NotificationsList
{
    public partial class NotificationsList : Form
    {
        PWRNOTIFICATIONS.PowerNotifications myPwrNot;
        CeUserNotificationsClass clsNoti;
        public bool bLogPowerChanges = false;
        object lockObject = new object();

        public NotificationsList()
        {
            InitializeComponent();
            clsNoti = new CeUserNotificationsClass();
            fillList();
            myPwrNot = new PWRNOTIFICATIONS.PowerNotifications();
            myPwrNot.OnMsg += new PWRNOTIFICATIONS.PowerNotifications.OnMsgHandler(myPwrNot_OnMsg);
            myPwrNot.Start();

            this.treeView1.AfterSelect += new TreeViewEventHandler(treeView1_AfterSelect);
        }

        void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("TreeViewNode clicked: " + e.Node.ToString());
            TreeNode treeNode = (TreeNode)e.Node;
            if (treeNode.Tag.ToString() == "root")
                return;

            UserNotificationInfoHeader iHead=new UserNotificationInfoHeader();
            iHead = (UserNotificationInfoHeader)treeNode.Tag;
        }

        UserNotificationInfoHeader getNotiHandleOfTreeView(TreeNode treeNode)
        {
            UserNotificationInfoHeader uNoti = new UserNotificationInfoHeader();
            if (treeNode.Tag.ToString() == "root")
                return null;

            uNoti = (UserNotificationInfoHeader)treeNode.Tag;
            
            return uNoti;
        }

        void myPwrNot_OnMsg(object sender, PWRNOTIFICATIONS.PowerNotifications.PwrEventArgs e)
        {
            addLog(DateTime.Now.ToString("hh:mm:ss") + "-" + e.PWRmsg );
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
                if (txtLog.Text.Length > 20000)
                    txtLog.Text = "";
                txtLog.Text += text + "\r\n";
                txtLog.SelectionLength = text.Length;
                txtLog.SelectionStart = txtLog.Text.Length - text.Length;
                txtLog.ScrollToCaret();
                lock (lockObject)
                {
                    if (bLogPowerChanges)
                    {
                        try
                        {
                            using (StreamWriter sr = new StreamWriter(@"\powernotifications.log.txt"))
                            {
                                sr.WriteLine(text);
                                sr.Flush();
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
        }
        
        int fillList()
        {
            clsNoti = new CeUserNotificationsClass();
            string[] sList = clsNoti.sAppList;

            clsNoti.applyTableStyle(dataGrid1);
            dataGrid1.DataSource = clsNoti.eventEntries;
            //dataGrid1.DataSource = cls.EventDB;

            CeUserNotificationsClass cls2 = new CeUserNotificationsClass(this.treeView1);
            return sList.Length;
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            if (myPwrNot != null)
                myPwrNot.Dispose();
            this.Close();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            fillList();
        }

        private void mnuRefresh_Click_1(object sender, EventArgs e)
        {
            fillList();
            //CEGETUSERNOTIFICATION.CeGetUserNotification nClass = new CEGETUSERNOTIFICATION.CeGetUserNotification();
            //CEGETUSERNOTIFICATION.CeGetUserNotification.CE_USER_NOTIFICATION[] NotiList = nClass.getUserNotifications();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (tabControl1.SelectedIndex == 0)
            //{
            //    mnuRemove.Enabled=true;
            //}
            //else
            //    mnuRemove.Enabled = false;
        }

        private void mnuSave_Click(object sender, EventArgs e)
        {
            if (saveData())
                MessageBox.Show("Data saved");
        }
        private bool saveData()
        {
            // now, I need to write the contents out
            SaveFileDialog sfd=new SaveFileDialog();
            sfd.Filter="Text files (*.txt)|*.txt|All files|*.*";            
            sfd.FilterIndex = 0;            
            if (sfd.ShowDialog() != DialogResult.OK)
                return false;
            string fileName = sfd.FileName;
            sfd.Dispose();

            int iCount = ((CeUserNotificationsClass.EventEntry[])dataGrid1.DataSource).Length;
            
            string[] myStrings = new string[iCount + 1];
            myStrings[0] = "App\tArgs\tEvent\tStart\tEnd\tType";
            for (int i = 0; i < iCount; i++)
            {
                string dataItem = "";
                for (int j = 0; j < 6; j++) //6 = number of columns
                    dataItem = dataItem + dataGrid1[i, j].ToString() + "\t";
                dataItem = dataItem.Substring(0, dataItem.Length - 1);
                myStrings[i+1] = dataItem;
            }

            return myAllLineWrite(fileName, myStrings);
        }
        private bool myAllLineWrite(string fileName, string[] linesToWrite)
        {
            try
            {
                using (StreamWriter sr = new StreamWriter(fileName))
                {
                    for (int i = 0; i < linesToWrite.Length; i++)
                        sr.WriteLine(linesToWrite[i]);
                }
                return true;
            }
            catch (Exception)
            {
            }
            return false;
        }

        private void menuItem2_Click(object sender, EventArgs e)
        {
            AddNotification dlg = new AddNotification();
            dlg.ShowDialog();
        }

        private void dataGrid1_CurrentCellChanged(object sender, EventArgs e)
        {
            // String variable used to show message.
            string myString = "CurrentCellChanged event raised, cell focus is at ";
            // Get the co-ordinates of the focussed cell.
            string myPoint = dataGrid1.CurrentCell.ColumnNumber + "," + dataGrid1.CurrentCell.RowNumber;
            // Create the alert message.
            myString = myString + "(" + myPoint + ")";
            // Show Co-ordinates when CurrentCellChanged event is raised.
            System.Diagnostics.Debug.WriteLine(myString + "\nCurrent cell co-ordinates");

        }

        private void mnuRemove_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)//datagridView
            {
                CeUserNotificationsClass.EventEntry cNot = new CeUserNotificationsClass.EventEntry();
                if (dataGrid1.CurrentRowIndex != -1)
                {
                    CeUserNotificationsClass.EventEntry[] _entry = (CeUserNotificationsClass.EventEntry[])dataGrid1.DataSource;
                    cNot = _entry[dataGrid1.CurrentRowIndex];
                    if (MessageBox.Show("Are you sure to delete '" + cNot.sApp + "'?", "Remove Entry", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.No)
                        return;
                    int iCnt = clsNoti.deleteEntry(cNot);
                    if (iCnt > 0)
                    {
                        MessageBox.Show("Number of deleted entries: " + iCnt.ToString());
                        fillList();
                        //dataGrid1.Refresh();
                    }
                    else
                        MessageBox.Show("Nothing deleted");
                }
            }
            else if (tabControl1.SelectedIndex == 1)//treeView
            { 
                TreeNode tn = this.treeView1.SelectedNode;
                if (tn == null)
                    return;
                UserNotificationInfoHeader userNotification = getNotiHandleOfTreeView(tn);
                if (userNotification == null)
                    return; //not found
                if (MessageBox.Show("Are you sure to delete '" + userNotification.UserNotificationTrigger.Application + "'?", "Remove Entry", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.No)
                    return;
                bool bRes = CEGETUSERNOTIFICATION.CeGetUserNotification.ClearUserNotification((IntPtr)userNotification.hNotification);
                if (bRes)
                {
                    MessageBox.Show("Deleted entry: " + userNotification.UserNotificationTrigger.Application);
                    fillList();
                    //dataGrid1.Refresh();
                }
                else
                    MessageBox.Show("Nothing deleted");
            }
        }

        private void mnuLogPowerChanges_Click(object sender, EventArgs e)
        {
            mnuLogPowerChanges.Checked = !mnuLogPowerChanges.Checked;
            lock (lockObject)
            {
                bLogPowerChanges = mnuLogPowerChanges.Checked;
            }
        }
    }
}