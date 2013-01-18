using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Process;

namespace CpuMonRcv
{
    public partial class cpuMonRcvForm : Form
    {
        RecvBroadcst recvr;
        DataAccess dataAccess;
        Queue<System.Process.ProcessStatistics.process_statistics> dataQueue;

        public cpuMonRcvForm()
        {
            InitializeComponent();
            //dataGridView1.AutoGenerateColumns = false;
            //DataColumn dc = new DataColumn("ProcID");
            //dataGridView1.Columns.Add(dc);
            //dataGridView1.

            //the plot graph
            c2DPushGraph1.MaxLabel = "3000";
            c2DPushGraph1.MaxPeekMagnitude = 3000;
            c2DPushGraph1.MinPeekMagnitude = 0;
            c2DPushGraph1.MinLabel = "0";

            dataQueue = new Queue<ProcessStatistics.process_statistics>();

            dataAccess = new DataAccess(this.dataGridView1, ref dataQueue);

            recvr = new RecvBroadcst();
            recvr.onUpdate += new RecvBroadcst.delegateUpdate(recvr_onUpdate);
            recvr.onEndOfTransfer += new RecvBroadcst.delegateEndOfTransfer(recvr_onEndOfTransfer);
        }

        void recvr_onEndOfTransfer(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("<EOT>");
            //get data
            float fUser = dataAccess.lastUserValue;
            dataAccess.lastUserValue = 0f;
            DateTime dtUser = dataAccess.lastUserMeasure;
            if(fUser!=0)
                updateGraph((int)fUser);

            //asign the chart control
            //DataView dView = new DataView(
            //    dsData.Tables["Processes"],
            //    "ProcID=3197842474",
            //    "theTime",
            //    DataViewRowState.CurrentRows);

            //chart1.Series[0].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Time;
            //string sFormat = chart1.ChartAreas[0].AxisX.LabelStyle.Format = "HH:mm:ss";
            //chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            //chart1.Series[0].Points.DataBindXY(dView, "theTime", dView, "user");
            //            chart1.DataSource = dsData.Tables["Processes"];
        }
        delegate void updateGraphCallback(int val);
        void updateGraph(int val)
        {
            if (this.c2DPushGraph1.InvokeRequired)
            {
                updateGraphCallback d = new updateGraphCallback(updateGraph);
                this.Invoke(d, new object[] { val });
            }
            else
            {
                c2DPushGraph1.AddLine(42, Color.Red);
                c2DPushGraph1.Push(val, 42);
                c2DPushGraph1.UpdateGraph();
            }
        }
        public void myDispose()
        {
            recvr.onUpdate -= recvr_onUpdate;
            recvr.bRunThread = false;
            recvr.Dispose();
            recvr = null;
            
            base.Dispose();
        }
        //################### data display etc...


        delegate void addDataCallback(ProcessStatistics.process_statistics procStats);
        void addData(ProcessStatistics.process_statistics procStats)
        {
            if (this.dataGridView1.InvokeRequired)
            {
                addDataCallback d = new addDataCallback(addData);
                this.Invoke(d, new object[] { procStats });
            }
            else
            {
                //enqueue data to be saved to sqlite
                dataQueue.Enqueue(procStats);

                //dataAccess.addSqlData(procStats);

                dataGridView1.SuspendLayout();
                //dtProcesses.Rows.Clear();

                dataAccess.addData(procStats);
                
                //dataGridView1.Refresh();
                dataGridView1.ResumeLayout();

                //release queue data
                dataAccess.waitHandle.Set();

                //object[] o = new object[7]{ procUsage.procStatistics. .procStatistics. [i].sApp, eventEntries[i].sArg, eventEntries[i].sEvent, 
                //        eventEntries[i].sStartTime, eventEntries[i].sEndTime, eventEntries[i].sType, eventEntries[i].sHandle };
            }
        }

        void recvr_onUpdate(object sender, ProcessStatistics.process_statistics data)
        {
            //string s = data.processID.ToString() + ", " +
            //        data.sName + ", " +
            //        data.procUsage.user.ToString() + ", " +
            //        data.duration.ToString();
            ////addLog(s);
            
            //System.Diagnostics.Debug.WriteLine( data.dumpStatistics() );
            addData(data);
        }
        delegate void SetTextCallback(string text);
        public void addLog(string text)
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
                if (txtLog.Text.Length > 4000)
                    txtLog.Text = "";
                txtLog.Text += text + "\r\n";
                txtLog.SelectionLength = 0;
                txtLog.SelectionStart = txtLog.Text.Length - 1;
                txtLog.ScrollToCaret();
            }
        }

        private void mnuViewDetails_Click(object sender, EventArgs e)
        {
            DetailView dv = new DetailView();
            dv.ShowDialog();
        }

        private void menuExit_Click(object sender, EventArgs e)
        {
            myDispose();
            Application.Exit();
        }

        private void mnuTestForm_Click(object sender, EventArgs e)
        {
            TestForm tf = new TestForm();
            tf.Show();
        }

        private void mnuUsage_Click(object sender, EventArgs e)
        {
            frmSelectIP frmIP = new frmSelectIP();
            string sIP = "";
            if (frmIP.ShowDialog() == DialogResult.OK)
            {
                sIP = frmIP.sIP;
            }
            else
                return;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.CheckPathExists = true;
            sfd.DefaultExt = "csv";
            sfd.AddExtension = true;
            sfd.Filter = "CSV|*.csv|All|*.*";
            sfd.FilterIndex = 0;
            sfd.InitialDirectory = Environment.CurrentDirectory;
            sfd.OverwritePrompt = true;
            sfd.RestoreDirectory = true;
            sfd.ValidateNames = true;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                DataAccess da = new DataAccess();
                da.export2CSV2(sfd.FileName, sIP);
            } 
        }
    }
}
