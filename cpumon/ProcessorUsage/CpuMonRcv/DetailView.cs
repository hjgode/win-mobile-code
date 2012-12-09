using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Data.SQLite;

using CustomUIControls.Graphing;

namespace CpuMonRcv
{
    public partial class DetailView : Form
    {
        string sDataFile = "ProcessUsage.sdb";
        string sDataFileFull
        {
            get
            {
                string sAppPath = System.Environment.CurrentDirectory;
                if (!sAppPath.EndsWith(@"\"))
                    sAppPath += @"\";
                return sAppPath + sDataFile;
            }
        }
        //private DataGridView masterDataGridView = new DataGridView();
        private BindingSource masterBindingSource = new BindingSource();
        //private DataGridView detailsDataGridView = new DataGridView();
        private BindingSource detailsBindingSource = new BindingSource();
        SQLiteConnection sql_conn;
        SQLiteDataAdapter masterDataAdapter;

        DataSet dsData;
        bool updateGraph = false;

        public DetailView()
        {
            InitializeComponent();
            PushGraph1.MaxLabel = "3000";
            PushGraph1.MinLabel = "0";
            PushGraph1.LineInterval = 10;

            masterDataGridView.DataSource = masterBindingSource;
            detailsDataGridView.DataSource = detailsBindingSource;
            GetData();

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
        void GetData()
        {
            sql_conn = new SQLiteConnection("Data Source=" + sDataFileFull + ";Version=3;New=False;Compress=True;Synchronous=Off");
            dsData = new DataSet();
            //MS ticks back to DateTime: http://sqlite.phxsoftware.com/forums/p/137/9643.aspx
            masterDataAdapter = new SQLiteDataAdapter("Select "+
                "Process, "+
                "User, "+
                "duration, "+
                "datetime(Time/10000000-62135596800, 'unixepoch') as theTime, "+
                "ProcID, "+
                "idx " +
                " FROM Processes", sql_conn);
                //"Select Process,User,duration,ProcID||Time as ProcTime from Processes", sql_conn);
            SQLiteDataAdapter detailsDataAdapter = new SQLiteDataAdapter("SELECT ThreadID,user,idx FROM Threads", sql_conn);
                //"Select ThreadID,user,ProcID||timestamp as ThrdTime from Threads", sql_conn);
            masterDataAdapter.Fill(dsData, "Processes");
            detailsDataAdapter.Fill(dsData, "Threads");
            DataRelation relation = new DataRelation(
                "ThreadsByProcess",
                dsData.Tables["Processes"].Columns["idx"], dsData.Tables["Threads"].Columns["idx"]);
            dsData.Relations.Add(relation);

            masterBindingSource.DataSource = dsData;
            masterBindingSource.DataMember = "Processes";

            detailsBindingSource.DataSource = masterBindingSource;
            detailsBindingSource.DataMember = "ThreadsByProcess";

            //masterDataGridView = new DataView(dsData.Tables["Processes"]);
            //masterDataGridView.DataSource = dataView;
            //masterDataGridView.Refresh();

        }

        const int lineNumber = 42;
        private void masterDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (!updateGraph)
                return;
            DataRow drow = dsData.Tables["Processes"].Rows[e.RowIndex];
            string sProcID = drow.ItemArray[4].ToString();
            
            DataView dView = new DataView(dsData.Tables["Processes"], "ProcId=" + sProcID, "theTime", DataViewRowState.CurrentRows);
            /*
            //using Linq
            DataTable orders = dsData.Tables["Processes"];
            EnumerableRowCollection<DataRow> query =
                from order in orders.AsEnumerable()
                where order.Field<bool>("OnlineOrderFlag") == true
                orderby order.Field<decimal>("TotalDue")
                select order;
            DataView view = query.AsDataView();
            */

            C2DPushGraph.LineHandle lHandle = PushGraph1.AddLine(lineNumber, Color.Red);
            if(lHandle!=null)
                lHandle.Clear();

            DataTable dTable = dView.ToTable("processtimes", false, new string[] { "User", "theTime" });
            System.Diagnostics.Debug.WriteLine("----------");
            foreach (DataRow dr in dTable.Rows)
            {
                object[] sA = dr.ItemArray;
                int iVal = Convert.ToInt16(sA[0]);
                System.Diagnostics.Debug.WriteLine(iVal.ToString());
                PushGraph1.Push(iVal, lineNumber);
            }
            PushGraph1.UpdateGraph();
        }

        private void masterDataGridView_Validated(object sender, EventArgs e)
        {
        }

        private void masterDataGridView_Enter(object sender, EventArgs e)
        {
            updateGraph = true;

        }
    }
}
