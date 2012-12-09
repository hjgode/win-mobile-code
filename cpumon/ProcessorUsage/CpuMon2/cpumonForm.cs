using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Process;

namespace CpuMon2
{
    public partial class cpumonForm : Form
    {
        ProcessUsage.ProcInfo pInfo;
        public cpumonForm()
        {
            InitializeComponent();
            showProcList();
            //start the process usage analysis thread
            pInfo = new ProcessUsage.ProcInfo();
            //show ourself in the stats?
            pInfo.bIncludeMySelf = false;

            //init data
            createTable();

            pInfo.updateEvent += new ProcessUsage.ProcInfo.updateEventHandler(pInfo_updateEvent);
        }

        void showProcList()
        {
            //show an initial list of processes
            Dictionary<UInt32, System.Process.process> pList = System.Process.Process.procList;
            foreach (KeyValuePair<uint, System.Process.process> pEntry in pList)
                addText(pEntry.Value.sName);
        }
        void pInfo_updateEvent(object sender, ProcessStatsEventArgs eventArgs)
        {            
            setText( ProcessStatistics.dumpProcStats(eventArgs));
            addData(eventArgs);
        }
        delegate void SetTextCallback(string text);
        public void setText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.textBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(setText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                textBox1.Text = text;
            }
        }
        delegate void addTextCallback(string text);
        public void addText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.textBox1.InvokeRequired)
            {
                addTextCallback d = new addTextCallback(setText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                if (textBox1.Text.Length > 2000)
                    textBox1.Text = "";
                textBox1.Text += text + "\r\n";
                textBox1.SelectionLength = 0;
                textBox1.SelectionStart = textBox1.Text.Length - 1;
                textBox1.ScrollToCaret();
            }
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            pInfo.Dispose();
            System.Threading.Thread.Sleep(500);
            Application.Exit();
        }

        //################### data display etc...
        BindingSource bsProcesses;
        DataSet dsProcesses;
        DataTable dtProcesses;
        void createTable()
        {
            dtProcesses = new DataTable("Processes");
            dsProcesses = new DataSet();
            bsProcesses = new BindingSource();

            DataColumn[] dc = new DataColumn[6];
            string[] _fieldNames = new string[6] { "procID", "Process", "User", "Kernel", "Time", "Duration" };
            //build the table columns
            for (int i = 0; i < _fieldNames.Length; i++)
            {
                dc[i] = new DataColumn();
                dc[i].Caption = _fieldNames[i];     // "App";
                dc[i].ColumnName = _fieldNames[i];  // "App";
                dc[i].DataType = System.Type.GetType("System.String");
                dc[i].MaxLength = 256;
                if (dc[i].Caption == "procID")
                {
                    dc[i].Unique = true;
                }
                else
                {
                    dc[i].Unique = false;
                }
                dc[i].AllowDBNull = false;
                
            }
            //add header
            dtProcesses.Columns.AddRange(dc);

            DataColumn[] dcKey = new DataColumn[1];
            dcKey[0] = dc[0];
            dtProcesses.PrimaryKey = dcKey;
            
            dsProcesses.Tables.Add(dtProcesses);
            
            bsProcesses.DataSource = dsProcesses;
            bsProcesses.DataMember = dsProcesses.Tables[0].TableName;
            
            dataGrid1.DataSource = bsProcesses;
            dataGrid1.Refresh();
        }
        delegate void addDataCallback(ProcessStatsEventArgs procStats);
        void addData (ProcessStatsEventArgs procStats)
        {
            if (this.dataGrid1.InvokeRequired)
            {
                addDataCallback d = new addDataCallback(addData);
                this.Invoke(d, new object[] { procStats });
            }
            else
            {
                dataGrid1.SuspendLayout();
                //dtProcesses.Rows.Clear();
                foreach (KeyValuePair<string, ProcessStatistics.process_statistics> ps in procStats.procStatistics)
                {
                    object[] o = new object[] { 
                        ps.Value.processID,
                        ps.Value.sName, 
                        ps.Value.procUsage.user, 
                        ps.Value.procUsage.kernel,
                        ps.Value.dateTime,
                        ps.Value.duration
                        };
                    DataRow dr;
                    //check if data already exists
                    dr = dsProcesses.Tables[0].Rows.Find(ps.Value.processID.ToString());
                    if (dr == null)
                    {   //add a new row
                        dr = dtProcesses.NewRow();
                        dr.ItemArray = o;
                        dtProcesses.Rows.Add(dr);
                    }
                    else
                        dr.ItemArray = o;
                }
                dataGrid1.Refresh();
                dataGrid1.ResumeLayout();
                //object[] o = new object[7]{ procUsage.procStatistics. .procStatistics. [i].sApp, eventEntries[i].sArg, eventEntries[i].sEvent, 
                //        eventEntries[i].sStartTime, eventEntries[i].sEndTime, eventEntries[i].sType, eventEntries[i].sHandle };
            }
            
        }

        private void mnuDataView_Click(object sender, EventArgs e)
        {
            dataGrid1.Visible = true;
            textBox1.Visible = false;
        }

        private void mnuTextView_Click(object sender, EventArgs e)
        {
            textBox1.Visible = true;
            dataGrid1.Visible = false;
        }
        /*
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
        */
    }
}