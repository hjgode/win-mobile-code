using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using System.Data;

using OpenNETCF.WindowsCE.Notification;

namespace NotificationsList
{
    public class CeUserNotificationsClass
    {
        public class EventEntry{
            private string _sApp;
            private string _sArg;
            private string _sEvent;
            private string _sStartTime;
            private string _sEndTime;
            private string _sType;
            private string _sHandle;

            private System.Collections.ArrayList entries = new System.Collections.ArrayList();

            public EventEntry()
            {
                    _sApp = "";
                    _sArg = "";
                    _sEvent = "";
                    _sStartTime = "";
                    _sEndTime = "";
                    _sType = "";
                    _sHandle = "0";
            }

            public EventEntry(string s)
            {
                string[] sIn = s.Split('|');
                if (sIn.Length == 7)
                {
                    _sApp = sIn[0];
                    _sArg = sIn[1];
                    _sEvent = sIn[2];
                    _sStartTime = sIn[3];
                    _sEndTime = sIn[4];
                    _sType = sIn[5];
                    _sHandle = sIn[6];
                }
            }
            public EventEntry(string[] sIn)
            {
                _sApp = sIn[0];
                _sArg = sIn[1];
                _sEvent = sIn[2];
                _sStartTime = sIn[3];
                _sEndTime = sIn[4];
                _sType = sIn[5];
                _sHandle = sIn[6];
            }

            public string sApp
            {
                get { return _sApp; }
                set { _sApp = value; }
            }
            public string sArg
            {
                get { return _sArg; }
                set { _sArg = value; }
            }
            public string sEvent
            {
                get { return _sEvent; }
                set { _sEvent = value; }
            }
            public string sStartTime
            {
                get { return _sStartTime; }
                set { _sStartTime = value; }
            }
            public string sEndTime
            {
                get { return _sEndTime; }
                set { _sEndTime = value; }
            }
            public string sType
            {
                get { return _sType; }
                set { _sType = value; }
            }
            public string sHandle
            {
                get { return _sHandle; }
                set { _sHandle = value; }
            }
            public int iHandle
            {
                get { return Convert.ToInt32(_sHandle); }
            }
        }

        public EventEntry[] eventEntries;
        public DataTable EventDB;

        //[DllImport("coredll.dll", EntryPoint = "CeGetUserNotificationHandles", SetLastError = true)]
        //private static extern bool CeGetUserNotificationHandles(IntPtr[] rghNotifications, uint cHandles, ref uint pcHandlesNeeded);
        //[DllImport("coredll.dll", EntryPoint = "CeGetUserNotification", SetLastError = true)]
        //private static extern bool CeGetUserNotification(IntPtr hNotification, uint cBufferSize, ref uint pcBytesNeeded, IntPtr pBuffer);

        private const uint _maxHandles = 128;
        private const uint _bufSize = 8192;

        private List<String> _sList;

        string[][] _strArr;
        public string[][] strArr
        {
            get { return _strArr; }
        }

        public string[] sAppList
        {
            get { return _sList.ToArray(); }
        }

        public CeUserNotificationsClass()
        {
            loadNotifications();
            fillDB();
        }

        public CeUserNotificationsClass(TreeView tv)
        {
            loadNotifications();
            loadNotifications(tv);
        }
        private int loadNotifications(TreeView tv)
        {
            int iCnt = 0;
            tv.Nodes.Clear();
            TreeNode tnMain = tv.Nodes.Add("Notifications");
            tnMain.Tag = "root";

            int[] iHandles = OpenNETCF.WindowsCE.Notification.Notify.GetUserNotificationHandles();
            _sList = new List<string>();
            _sList.Clear();

            _strArr = new string[iHandles.Length + 1][];

            _strArr[0] = new string[7] { "App", "Args", "Event", "Start", "End", "Type", "handle" };

            int i = 0;

            eventEntries = new EventEntry[iHandles.Length];

            foreach (int iHandle in iHandles)
            {
                OpenNETCF.WindowsCE.Notification.UserNotificationInfoHeader infoHeader = OpenNETCF.WindowsCE.Notification.Notify.GetUserNotification(iHandle);
                string[] strArray = new string[] { infoHeader.UserNotificationTrigger.Application, infoHeader.UserNotificationTrigger.Arguments };
                TreeNode tn = new TreeNode(infoHeader.UserNotificationTrigger.Application);
                tnMain.Nodes.Add(tn);
                tn.Tag = infoHeader;
                TreeNode tnSub = tn.Nodes.Add("Args: " + infoHeader.UserNotificationTrigger.Arguments);
                tnSub.Tag = infoHeader;
                tnSub = tn.Nodes.Add("Event: " + infoHeader.UserNotificationTrigger.Event.ToString());
                tnSub.Tag = infoHeader;
                tnSub = tn.Nodes.Add("Start: " + infoHeader.UserNotificationTrigger.StartTime.ToString("dd.MM.yyyy HH:mm"));
                tnSub.Tag = infoHeader;
                tnSub = tn.Nodes.Add("End: " + infoHeader.UserNotificationTrigger.EndTime.ToString("dd.MM.yyyy HH:mm"));
                tnSub.Tag = infoHeader;
                tnSub = tn.Nodes.Add("Type: " + infoHeader.UserNotificationTrigger.Type.ToString());
                tnSub.Tag = infoHeader;
                tnSub = tn.Nodes.Add("Handle: " + iHandles[i].ToString());
                tnSub.Tag = infoHeader;

                //eventEntries[i].sApp = infoHeader.UserNotificationTrigger.Application;
                //eventEntries[i].sEvent = infoHeader.UserNotificationTrigger.Event.ToString();
                //eventEntries[i].sArg = infoHeader.UserNotificationTrigger.Arguments;
                //eventEntries[i].sStartTime = infoHeader.UserNotificationTrigger.StartTime.ToString("dd.MM.yyyy HH:mm");
                //eventEntries[i].sEndTime = infoHeader.UserNotificationTrigger.EndTime.ToString("dd.MM.yyyy HH:mm");
                //eventEntries[i].sType = infoHeader.UserNotificationTrigger.Type.ToString();
                //eventEntries[i].sHandle = iHandles[i].ToString();

                iCnt++;
            }
            tv.Refresh();
            return iCnt;
        }
        private void fillDB()
        {
            EventDB = new DataTable("Notifications");
            DataColumn[] dc = new DataColumn[7];
            string[] _fieldNames = new string[7] { "App", "Args", "Event", "Start", "End", "Type", "handle" };
            //build the table columns
            for (int i = 0; i < 7; i++)
            {
                dc[i] = new DataColumn();
                dc[i].Caption = _fieldNames[i];     // "App";
                dc[i].ColumnName = _fieldNames[i];  // "App";
                dc[i].DataType = System.Type.GetType("System.String");
                dc[i].MaxLength = 256;
                dc[i].Unique = false;
                dc[i].AllowDBNull = false;
            }
            //add header
            EventDB.Columns.AddRange(dc);
            //add rows
            for (int i = 0; i < eventEntries.Length; i++)
            {
                DataRow dr = EventDB.NewRow();
                object[] o = new object[7]{ eventEntries[i].sApp, eventEntries[i].sArg, eventEntries[i].sEvent, 
                    eventEntries[i].sStartTime, eventEntries[i].sEndTime, eventEntries[i].sType, eventEntries[i].sHandle };
                dr.ItemArray = o;
                EventDB.Rows.Add(dr);
            }
        }
        public int deleteEntry(EventEntry evnt)
        {
            int iRet = 0;
            if (this.EventDB != null)
            {
                var dRows = EventDB.Select("handle='" + evnt.sHandle + "'");
                foreach (var r in dRows)
                {
                    if (CEGETUSERNOTIFICATION.CeGetUserNotification.ClearUserNotification((IntPtr)evnt.iHandle))
                    {
                        r.Delete();
                        iRet++;
                    }
                    else
                        System.Diagnostics.Debug.WriteLine("ClearUserNotification failed");
                }
                EventDB.AcceptChanges();
            }
            return iRet;
        }
        private int loadNotifications()
        {
            int[] iHandles = OpenNETCF.WindowsCE.Notification.Notify.GetUserNotificationHandles();
            _sList= new List<string>();
            _sList.Clear();
            
            _strArr = new string[iHandles.Length + 1][];

            _strArr[0] = new string[7] { "App", "Args", "Event", "Start", "End", "Type", "handle" };
            
            int i = 0;

            eventEntries = new EventEntry[iHandles.Length];
            //eventEntries[0] = new EventEntry("App|Args|Event|Start|End|Type");
            
            //###############################
            
            //###############################

            foreach (int iHandle in iHandles)
            {
                OpenNETCF.WindowsCE.Notification.UserNotificationInfoHeader infoHeader = OpenNETCF.WindowsCE.Notification.Notify.GetUserNotification(iHandle);
                
                _strArr[i]=new string[7];
    
                string sApp = infoHeader.UserNotificationTrigger.Application;
                _strArr[i][0]="'" + infoHeader.UserNotificationTrigger.Application + "'";

                sApp += "|'" + infoHeader.UserNotificationTrigger.Arguments + "'";
                _strArr[i][1]=infoHeader.UserNotificationTrigger.Arguments;

                sApp += "|" + infoHeader.UserNotificationTrigger.Event.ToString();
                _strArr[i][2]=infoHeader.UserNotificationTrigger.Event.ToString();

                sApp += "|" + infoHeader.UserNotificationTrigger.StartTime.ToString("ddMMyyyy HH:mm"); 
                _strArr[i][3] = infoHeader.UserNotificationTrigger.StartTime.ToString("ddMMyyyy HH:mm");

                sApp += "|" + infoHeader.UserNotificationTrigger.EndTime.ToString("ddMMyyyy HH:mm");
                _strArr[i][4] = infoHeader.UserNotificationTrigger.EndTime.ToString("ddMMyyyy HH:mm");

                sApp += "|" + infoHeader.UserNotificationTrigger.Type.ToString();
                _strArr[i][5] = infoHeader.UserNotificationTrigger.Type.ToString();

                sApp += "|" + iHandles[i].ToString();
                _strArr[i][6] = iHandles[i].ToString();

                eventEntries[i] = new EventEntry(sApp);
                //eventEntries[i] = new EventEntry(_strArr[i]);

                i++;
                _sList.Add(sApp);
            }
            
            return _sList.Count;
        }

        public void applyTableStyle(DataGrid dataGrid1)
        {
            // Creates two DataGridTableStyle objects, one for the Machine
            // array, and one for the Parts ArrayList.

            DataGridTableStyle EventsTable = new DataGridTableStyle();
            // Sets the MappingName to the class name plus brackets.    
            EventsTable.MappingName = "EventEntry[]";

            // Sets the AlternatingBackColor so you can see the difference.
            //EventsTable.AlternatingBackColor = System.Drawing.Color.LightBlue;

            // Creates column styles.
            DataGridTextBoxColumn eventApp = new DataGridTextBoxColumn();
            eventApp.MappingName = "sApp";
            eventApp.HeaderText = "sApp";
            eventApp.Width = dataGrid1.Width / 6 * 2;

            DataGridTextBoxColumn eventArg = new DataGridTextBoxColumn();
            eventArg.MappingName = "sArg";
            eventArg.HeaderText = "Arg";

            DataGridTextBoxColumn eventEvent = new DataGridTextBoxColumn();
            eventEvent.MappingName = "sEvent";
            eventEvent.HeaderText = "Event";

            DataGridTextBoxColumn eventStart = new DataGridTextBoxColumn();
            eventStart.MappingName = "sStartTime";
            eventStart.HeaderText = "Start";

            DataGridTextBoxColumn eventEnd = new DataGridTextBoxColumn();
            eventEnd.MappingName = "sEndTime";
            eventEnd.HeaderText = "End";

            DataGridTextBoxColumn eventType = new DataGridTextBoxColumn();
            eventType.MappingName = "sType";
            eventType.HeaderText = "Type";

            DataGridTextBoxColumn eventHandle = new DataGridTextBoxColumn();
            eventHandle.MappingName = "sHandle";
            eventHandle.HeaderText = "handle";

            // Adds the column styles to the grid table style.
            EventsTable.GridColumnStyles.Add(eventApp);
            EventsTable.GridColumnStyles.Add(eventArg);
            EventsTable.GridColumnStyles.Add(eventEvent);
            EventsTable.GridColumnStyles.Add(eventStart);
            EventsTable.GridColumnStyles.Add(eventEnd);
            EventsTable.GridColumnStyles.Add(eventType);
            EventsTable.GridColumnStyles.Add(eventHandle);

            // Add the table style to the collection, but clear the 
            // collection first.
            dataGrid1.TableStyles.Clear();
            dataGrid1.TableStyles.Add(EventsTable);

        }
    }
}
