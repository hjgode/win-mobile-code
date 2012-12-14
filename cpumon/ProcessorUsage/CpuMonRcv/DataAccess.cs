using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Threading;

using System.Windows.Forms;

using System.Data;
using System.Data.SQLite;

namespace CpuMonRcv
{
    class DataAccess:IDisposable
    {
        class _fieldsDefine {
            public string FieldName;
            public string FieldType;
            public _fieldsDefine(string s, string t)
            {
                FieldName = s;
                FieldType = t;
            }
        }
        _fieldsDefine[] _fieldsProcess = new _fieldsDefine[]{
            new _fieldsDefine("ProcID", "System.UInt32"),
            new _fieldsDefine("Process", "System.String"),
            new _fieldsDefine("User", "System.Int64"),
            new _fieldsDefine("Kernel", "System.Int64"),
            new _fieldsDefine("Time", "System.DateTime"),
            new _fieldsDefine("Duration", "System.UInt32"),
            new _fieldsDefine("idx", "System.UInt64")
        };

        _fieldsDefine[] _fieldsThread = new _fieldsDefine[]{
            new _fieldsDefine("ThreadID", "System.UInt32"),
            new _fieldsDefine("ProcID", "System.UInt32"),
            new _fieldsDefine("user", "System.UInt32"),
            new _fieldsDefine("kernel", "System.UInt32"),
            new _fieldsDefine("duration", "System.UInt32"),
            new _fieldsDefine("timestamp", "System.DateTime"),
            new _fieldsDefine("idx", "System.UInt64")
        };

        public float lastUserValue = 0;
        public DateTime lastUserMeasure;

        static string sDataFile = "ProcessUsage.sqlite";
        public static string sDataFileFull{
            get{            
                string sAppPath = System.Environment.CurrentDirectory;
                if(!sAppPath.EndsWith(@"\"))
                    sAppPath+=@"\";
                return sAppPath+sDataFile;
            }
        }

        private SQLiteConnection sql_con;
        private SQLiteCommand sql_cmd;
        private SQLiteDataAdapter sql_dap;

        BindingSource bsProcesses;
        DataSet dsProcesses;
        DataTable dtProcesses;
        DataTable dtThreads;

        DataGridView _dataGrid;

        Queue<System.Process.ProcessStatistics.process_statistics> dataQueue;
        Thread myDataThread;

        public EventWaitHandle waitHandle;

        public DataAccess()
        {

        }

        public DataAccess(DataGridView dg, ref Queue<System.Process.ProcessStatistics.process_statistics> dQueue)
        {
            _dataGrid = dg;
            dataQueue = dQueue;

            sql_cmd = new SQLiteCommand();

            createTables();

            connectDB();
            createTablesSQL();

            waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

            if (System.IO.File.Exists(sDataFileFull))
                ;//open db
            else
                ;//create new db
            myDataThread = new Thread(dataAddThread);
            myDataThread.Start();
        }
        public void Dispose()
        {
            myDataThread.Abort();
        }
        void dataAddThread()
        {
            try
            {
                while (true)
                {
                    waitHandle.WaitOne();
                    if (dataQueue.Count > 10)
                    {
                        while (dataQueue.Count > 0)
                        {
                            System.Process.ProcessStatistics.process_statistics procStats = dataQueue.Dequeue();
                            addSqlData(procStats);
                        }
                    }
                    Thread.Sleep(10);
                }
            }
            catch (ThreadAbortException ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception: " + ex.Message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception: " + ex.Message);
            }
        }

        private void connectDB()
        {
            sql_con = new SQLiteConnection("Data Source=" + sDataFileFull + ";Version=3;New=False;Compress=True;Synchronous=Off");
            sql_con.Open();
            
            DataTable dtTest = sql_con.GetSchema("Tables",new string []{ null, null, "Processes", null});
            sql_cmd.Connection = sql_con;
        }

        public bool addData(System.Process.ProcessStatistics.process_statistics procStats)
        {
            bool bRet = false;
            try
            {
                //string txtSQLQuery = "insert into  processes (desc) values ('" + txtDesc.Text + "')";
                //ExecuteQuery(txtSQLQuery);
                object[] o = new object[]{
                    procStats.processID,
                    procStats.sName,
                    procStats.procUsage.user,
                    procStats.procUsage.kernel,
                    new DateTime(procStats.dateTime),
                    procStats.duration,
                    0
                };

                //some data for the live view
                lastUserValue += procStats.procUsage.user;
                lastUserMeasure = new DateTime(procStats.dateTime);

                DataRow dr;
                //check if data already exists
                dr = dsProcesses.Tables[0].Rows.Find(procStats.processID.ToString());
                if (dr == null)
                {   //add a new row
                    dr = dtProcesses.NewRow();
                    dr.ItemArray = o;
                    dtProcesses.Rows.Add(dr);
                }
                else
                    dr.ItemArray = o;

                dr.AcceptChanges();
                dtProcesses.AcceptChanges();
                dsProcesses.AcceptChanges();
                bRet = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("addData Exception: " + ex.Message);
            }
            return bRet;
        }
        
        public int ExportProcess2CSV(string sFileCSV)
        {
            //pause data read thread (socksrv)?
            sql_cmd = new SQLiteCommand();
            sql_con = new SQLiteConnection();
            connectDB();
            if (sql_con.State != ConnectionState.Open)
            {
                sql_con.Close();
                sql_con.Open();
            }
            sql_cmd = sql_con.CreateCommand();
            int iCnt = 0;
            sql_cmd.CommandText="select * from processes";
            SQLiteDataReader rdr = null;
            try
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter(sFileCSV);
                rdr = sql_cmd.ExecuteReader(CommandBehavior.CloseConnection);
                sw.Write(
                        "Process" + ";" +
                        "ProcID" + ";" +
                        "User" + ";" +
                        "Time" + ";" +
                        "Duration" + ";" +
                        "Idx" +
                        "\r\n"
                        );

                while (rdr.Read())
                {
                    iCnt++;
                    //Console.WriteLine(rdr["ProcID"] + " " + rdr["User"]);
                    sw.Write(
                        "\"" + rdr["Process"] + "\";" + 
                        rdr["ProcID"] + ";" +
                        rdr["User"] + ";" +
                        DateTime.FromBinary((long)rdr["Time"]).ToString("hh:mm:ss.fff") + ";" +
                        rdr["Duration"] + ";" +
                        rdr["Idx"] +
                        "\r\n"
                        );
                }
            }
            finally
            {
                rdr.Close();
            }
            
            sql_con.Close();
            return iCnt;
        }

        public int ExportThreads2CSV(string sFileCSV)
        {
            //pause data read thread (socksrv)?
            sql_cmd = new SQLiteCommand();
            sql_con = new SQLiteConnection();
            connectDB();
            if (sql_con.State != ConnectionState.Open)
            {
                sql_con.Close();
                sql_con.Open();
            }
            sql_cmd = sql_con.CreateCommand();
            int iCnt = 0;
            sql_cmd.CommandText = "select * from threads";
            SQLiteDataReader rdr = null;
            try
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter(sFileCSV);
                rdr = sql_cmd.ExecuteReader(CommandBehavior.CloseConnection);
                string sFields = "";
                foreach (_fieldsDefine ft in _fieldsThread)
                {
                    sFields+=(ft.FieldName+ ";");
                }
                sFields.TrimEnd(new char[] { ';' });                
                sw.Write(sFields +"\r\n");

                while (rdr.Read())
                {
                    iCnt++;
                    sFields = "";
                    //Console.WriteLine(rdr["ProcID"] + " " + rdr["User"]);
                    foreach (_fieldsDefine fd in _fieldsThread)
                    {
                        if (fd.FieldType == "System.String")
                            sFields += "\"" + rdr[fd.FieldName] + "\";";
                        else if (fd.FieldType == "System.DateTime")
                            sFields += DateTime.FromBinary((long)rdr[fd.FieldName]).ToString("hh:mm:ss.fff") + ";";
                        else
                            sFields += rdr[fd.FieldName] + ";";
                    }
                    sFields.TrimEnd(new char[] { ';' });
                    sw.Write(sFields);
                    sw.Write(sFields + "\r\n");
                }
            }
            finally
            {
                rdr.Close();
            }

            sql_con.Close();
            return iCnt;
        }

        public DataRow[] executeQuery(string sSQL)
        {
            //setup
            List<DataRow> dataRows = new List<DataRow>();
            sql_cmd = new SQLiteCommand();
            sql_con = new SQLiteConnection();
            SQLiteDataReader sql_rdr= null;
            connectDB();
            if (sql_con.State != ConnectionState.Open)
            {
                sql_con.Close();
                sql_con.Open();
            }
            sql_cmd = sql_con.CreateCommand();
            sql_cmd.CommandText = sSQL;
            sql_rdr = sql_cmd.ExecuteReader(CommandBehavior.CloseConnection);
            List<string> sColList = new List<string>();
            DataColumn dc;
            while (sql_rdr.Read())
            {
                for (int i = 0; i < sql_rdr.FieldCount; i++)
                {
                    sColList.Add(sql_rdr.GetName(i));
                    dc = new DataColumn(sql_rdr.GetName(i));
                    //dc = sql_rdr.GetValue(i);
                }

            }
            return dataRows.ToArray();
        }

        #region Transform
        class PROCESS_USAGE
        {
            public string procname;
            public int user;
            public UInt64 timestamp;
            public PROCESS_USAGE(string sProcessName, int iUserTime, UInt64 iTimeStamp)
            {
                procname = sProcessName;
                user = iUserTime;
                timestamp = iTimeStamp;
            }

        }

        public int export2CSV2(string sFileCSV)
        {
            //### setup
            sql_cmd = new SQLiteCommand();
            sql_con = new SQLiteConnection();
            SQLiteDataReader sql_rdr;
            connectDB();
            if (sql_con.State != ConnectionState.Open)
            {
                sql_con.Close();
                sql_con.Open();
            }
            sql_cmd = sql_con.CreateCommand();
            long lCnt = 0;

            //### Build a List of known processes
            sql_cmd.CommandText = "Select DISTINCT Process from processes order by Process";
            List<string> lProcesses= new List<string>();
            sql_rdr = sql_cmd.ExecuteReader();
            while (sql_rdr.Read())
            {
                lProcesses.Add((string)sql_rdr["Process"]);
            }
            sql_rdr.Close();
            sql_rdr.Dispose();

            //create a new table with the process names as fields
            string sProcField = "";
            foreach (string sProc in lProcesses)
            {
                sProcField += "[" + sProc + "] INTEGER,";
            }
            sProcField = sProcField.TrimEnd(new char[] { ',' });
            sProcField = "[Time] INTEGER, " + sProcField;
            //delete existing table            
            lCnt = executeNonQuery("DROP Table IF EXISTS [ProcUsage] ;");
            //create new one
            lCnt = executeNonQuery("Create Table [ProcUsage] (" + sProcField + ");");
            
            //### get all process,user,time data
            List<PROCESS_USAGE> lProcessUsages = new List<PROCESS_USAGE>();
            sql_cmd.CommandText = "Select Process,User,Time from processes order by Time";
            sql_rdr = sql_cmd.ExecuteReader();
            while (sql_rdr.Read())
            {
                string sP = (string)sql_rdr["Process"];
                int iUT = Convert.ToInt32(sql_rdr["User"]);
                ulong uTI = Convert.ToUInt64(sql_rdr["Time"]);
                lProcessUsages.Add(new PROCESS_USAGE(sP, iUT, uTI));
            }
            sql_rdr.Close();

            //### get all distinct times
            List<ulong> lTimes = new List<ulong>();
            sql_cmd.CommandText = "Select DISTINCT Time from processes order by Time";
            sql_rdr = sql_cmd.ExecuteReader();
            while (sql_rdr.Read())
            {
                lTimes.Add(Convert.ToUInt64(sql_rdr["Time"]));
            }
            sql_rdr.Close();

            string sUpdateCommand = "";
            //### file the new ProcUsage table
            SQLiteTransaction tr = sql_con.BeginTransaction();
            //sql_cmd.CommandText = "insert into [ProcUsage]  (Time, [device.exe]) SELECT Time, User from [Processes] WHERE Time=631771077815940000 AND Process='device.exe';";
            //lCnt = sql_cmd.ExecuteNonQuery();
            foreach (ulong uTime in lTimes)
            {
                System.Diagnostics.Debug.WriteLine("Updating for Time=" + uTime.ToString());
                //insert an empty row
                sql_cmd.CommandText = "Insert Into ProcUsage (Time) VALUES(" + uTime.ToString() + ");";
                lCnt = sql_cmd.ExecuteNonQuery();
                foreach (string sPro in lProcesses)
                {
                    //is there already a line?
                    //lCnt = executeNonQuery("Select Time " + "From ProcUsage Where Time="+uTime.ToString());

                    // http://stackoverflow.com/questions/4495698/c-sharp-using-listt-find-with-custom-objects
                    PROCESS_USAGE pu = lProcessUsages.Find(x => x.procname == sPro && x.timestamp == uTime);
                    if (pu != null)
                    {
                        System.Diagnostics.Debug.WriteLine("\tUpdating User="+ pu.user +" for Process=" + sPro);
                        //update values
                        sUpdateCommand = "Update [ProcUsage] SET " +
                            "[" + sPro + "]=" + pu.user +
                            //"(SELECT User from [Processes]
                            " WHERE Time=" + uTime.ToString() + //" AND Process=" + "'" + sPro + "'"+
                            ";";
                        sql_cmd.CommandText = sUpdateCommand;
                        //System.Diagnostics.Debug.WriteLine(sUpdateCommand);
                        lCnt = sql_cmd.ExecuteNonQuery();
                        //lCnt = executeNonQuery(sInsertCommand);
                        //"insert into [ProcUsage]  (Time, [device.exe]) SELECT Time, User from [Processes] WHERE Time=631771077815940000 AND Process='device.exe';"
                    }
                }
            }
            tr.Commit();

            lCnt = 0;
            SQLiteDataReader rdr = null;
            System.IO.StreamWriter sw = null;
            try
            {
                sw = new System.IO.StreamWriter(sFileCSV);
                string sFields = "";
                List<string> lFields = new List<string>();
                lFields.Add("Time");
                lFields.AddRange(lProcesses);
                foreach (string ft in lFields)
                {
                    sFields += ("'" + ft + "'" + ";");
                }
                sFields.TrimEnd(new char[] { ';' });
                sw.Write(sFields + "\r\n");

                sql_cmd.CommandText = "Select * from ProcUsage;";
                rdr = sql_cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (rdr.Read())
                {
                    lCnt++;
                    sFields = "";
                    //Console.WriteLine(rdr["ProcID"] + " " + rdr["User"]);
                    foreach (string ft in lFields)
                    {
                        sFields += rdr[ft] + ";";
                    }
                    sFields.TrimEnd(new char[] { ';' });
                    sw.Write(sFields + "\r\n");
                    sw.Flush();
                }
            }
            catch (Exception) { }
            finally
            {
                sw.Close();
                rdr.Close();
            }

            return 0;
        }
        #endregion

        private void createTablesSQL()
        {
            // Define the SQL Create table statement, IF NOT EXISTS 
            string createAppUserTableSQL = "CREATE TABLE IF NOT EXISTS [Processes] (" +
                "[ProcID] INTEGER NOT NULL, " +
                "[Process] TEXT NOT NULL, " +
                "[User] INTEGER NOT NULL, " +
                "[Kernel] INTEGER NOT NULL, " +
                "[Time] INTEGER NOT NULL, " +
                "[Duration] INTEGER NOT NULL, " +
                "[idx] INTEGER PRIMARY KEY AUTOINCREMENT " +
                ")";
            executeNonQuery(createAppUserTableSQL);

            string SqlIndex = "CREATE INDEX [ProcID] on processes (ProcID ASC);";
            executeNonQuery(SqlIndex);

            string createThreadsTable = "CREATE TABLE IF NOT EXISTS [Threads] (" +
                "[ThreadID] INTEGER NOT NULL, " +
                "[ProcID] INTEGER NOT NULL, " +
                "[user] INTEGER NOT NULL, " +
                "[kernel] INTEGER NOT NULL, " +
                "[duration] INTEGER NOT NULL, " +
                "[timestamp] INTEGER NOT NULL, " +
                "[idx] INTEGER NOT NULL " +
                ")";
            executeNonQuery(createThreadsTable);
            SqlIndex = "CREATE INDEX [ThreadID] on Threads (ThreadID ASC);";
            executeNonQuery(SqlIndex);

            //a view
            string createView = "CREATE VIEW IF NOT EXISTS ProcessView AS " +
                "SELECT " +
                "Processes.ProcID, Processes.Process, Processes.[User] * 1.0 / Processes.[Time] * 1.0 * 100.0 AS Usage, Processes.Duration, strftime('%H:%M:%f', " +
                "datetime(Processes.[Time] / 10000000 - 62135596800, 'unixepoch')) AS theTime " +
                "FROM " +
                "Processes INNER JOIN " +
                "Threads ON Processes.idx = Threads.idx " +
                "ORDER BY theTime";
            executeNonQuery(createView);

        }

        static StringBuilder FieldsProcessTable = new StringBuilder();
        static StringBuilder FieldsThreadsTable = new StringBuilder();
        //see also http://www.techcoil.com/blog/my-experience-with-system-data-sqlite-in-c/
        public void addSqlData(System.Process.ProcessStatistics.process_statistics procStats)
        {
            //System.Diagnostics.Debug.WriteLine(procStats.dumpStatistics());

            long rowID = 0; //last inserted row
            #region Process_data
            //build a list of field names of process table
            if (FieldsProcessTable.Length == 0)
            {
                //StringBuilder 
                //FieldsProcessTable = new StringBuilder();
                for (int ix = 0; ix < _fieldsProcess.Length; ix++)
                {
                    FieldsProcessTable.Append(_fieldsProcess[ix].FieldName);
                    if (ix < _fieldsProcess.Length - 1)
                        FieldsProcessTable.Append(", ");
                }
            }

            StringBuilder FieldsProcessValues = new StringBuilder();
            FieldsProcessValues.Append(procStats.processID.ToString()+", ");
            FieldsProcessValues.Append("'" + procStats.sName + "', ");
            FieldsProcessValues.Append(procStats.procUsage.user.ToString() + ", ");
            FieldsProcessValues.Append(procStats.procUsage.kernel.ToString() + ", ");
            FieldsProcessValues.Append(procStats.dateTime.ToString() + ", ");
            FieldsProcessValues.Append(procStats.duration.ToString() + ", ");
            FieldsProcessValues.Append("NULL");    //add an idx although it is autoincrement

            string sqlStatement = "INSERT INTO processes " +
                "(" + 
                FieldsProcessTable +
                ")" +
                " VALUES(" + 
                FieldsProcessValues.ToString() +
                ")";

            rowID = executeNonQuery(sqlStatement);
            #endregion

            #region Threads_data
            if (FieldsThreadsTable.Length == 0)
            {
                for (int ix = 0; ix<_fieldsThread.Length; ix++)
                {
                    FieldsThreadsTable.Append(_fieldsThread[ix].FieldName);
                    if (ix < _fieldsThread.Length - 1)
                        FieldsThreadsTable.Append(", ");
                }
            }

            StringBuilder FieldsThreadValues = new StringBuilder();
            System.Process.threadStatistic[] threadList=procStats.ThreadStatList.ToArray();
            for(int it=0; it<threadList.Length; it++){
                FieldsThreadValues = new StringBuilder();
                FieldsThreadValues.Append(threadList[it].dwThreadID.ToString() +", ");
                FieldsThreadValues.Append(threadList[it].dwOwnerProcID.ToString() + ", ");
                FieldsThreadValues.Append(threadList[it].thread_times.user.ToString() + ", ");
                FieldsThreadValues.Append(threadList[it].thread_times.kernel.ToString() + ", ");

                FieldsThreadValues.Append(threadList[it].duration.ToString() + ", ");
                FieldsThreadValues.Append(threadList[it].timestamp.ToString() + ", ");
                FieldsThreadValues.Append(rowID.ToString());

                sqlStatement = "INSERT INTO Threads " +
                    "(" +
                    FieldsThreadsTable +
                    ")" +
                    " VALUES(" +
                    FieldsThreadValues.ToString() +
                    ")";
                executeNonQuery(sqlStatement);
            }
            #endregion
        }

        long executeNonQuery(string sSQL)
        {
            long rowId = 0;
            try
            {
                if (sql_con.State != ConnectionState.Open)
                {
                    sql_con.Close();
                    sql_con.Open();
                }
                using (SQLiteTransaction sqlTransaction = sql_con.BeginTransaction())
                {
                    sql_cmd.CommandText = sSQL;
                    sql_cmd.ExecuteNonQuery();
                    // Commit the changes into the database
                    sqlTransaction.Commit();

                    sql_cmd.CommandText = "SELECT last_insert_rowid()";
                    rowId = (long)sql_cmd.ExecuteScalar();
                    
                } // end using
            }
            catch (SQLiteException ex)
            {
                System.Diagnostics.Debug.WriteLine("executeNonQuery SQLiteException: " + ex.Message + "\r\n'" + sSQL +"'");
                rowId = 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("executeNonQuery Exception: " + ex.Message + "\r\n'" + sSQL + "'");
                rowId = 0;
            }
            return rowId;
        }
        private void createTables()
        {
            dtProcesses = new DataTable("Processes");
            dsProcesses = new DataSet();
            bsProcesses = new BindingSource();
            #region proc_table
            DataColumn[] dc = new DataColumn[_fieldsProcess.Length];
            //string[] _fieldNames = new string[6] { "procID", "Process", "User", "Kernel", "Time", "Duration" };
            //build the process table columns
            for (int i = 0; i < _fieldsProcess.Length ; i++)
            {
                dc[i] = new DataColumn();
                dc[i].Caption = _fieldsProcess[i].FieldName;     // "App";
                dc[i].ColumnName = _fieldsProcess[i].FieldName;  // "App";
                dc[i].DataType = System.Type.GetType(_fieldsProcess[i].FieldType);

                //if (dc[i].DataType = System.Type.GetType("System.DateTime"))
                //    dc[i].DateTimeMode = DataSetDateTime.Local;

                if (dc[i].DataType == System.Type.GetType("System.String"))
                    dc[i].MaxLength = 256;

                if (dc[i].Caption.Equals("ProcID",StringComparison.CurrentCultureIgnoreCase))
                    dc[i].Unique = true;
                else
                    dc[i].Unique = false;
                dc[i].AllowDBNull = false;

            }
            //add header
            dtProcesses.Columns.AddRange(dc);

            DataColumn[] dcKey = new DataColumn[1];
            dcKey[0] = dc[0];
            dtProcesses.PrimaryKey = dcKey;

            dsProcesses.Tables.Add(dtProcesses);
            #endregion

            #region threadsTable
            dtThreads = new DataTable("Threads");
            dc = new DataColumn[_fieldsThread.Length];
            //build the process table columns
            for (int i = 0; i < _fieldsThread.Length; i++)
            {
                dc[i] = new DataColumn();
                dc[i].Caption = _fieldsThread[i].FieldName;     // "App";
                dc[i].ColumnName = _fieldsThread[i].FieldName;  // "App";
                dc[i].DataType = System.Type.GetType(_fieldsThread[i].FieldType);

                if (dc[i].DataType == System.Type.GetType("System.String"))
                    dc[i].MaxLength = 256;

                if (dc[i].Caption.Equals("ThreadID",StringComparison.CurrentCultureIgnoreCase))
                    dc[i].Unique = true;
                else
                    dc[i].Unique = false;
                dc[i].AllowDBNull = false;

            }
            //add header
            dtThreads.Columns.AddRange(dc);

            DataColumn[] dcKeyThread = new DataColumn[1];
            dcKeyThread[0] = dc[0];
            dtThreads.PrimaryKey = dcKeyThread;

            dsProcesses.Tables.Add(dtThreads);
            #endregion

            bsProcesses.DataSource = dsProcesses;
            bsProcesses.DataMember = dsProcesses.Tables[0].TableName;

            dtProcesses.AcceptChanges();
            dsProcesses.AcceptChanges();

            this._dataGrid.DataSource = bsProcesses;
            this._dataGrid.Refresh();
        }
        private void ExecuteQuery(string txtQuery)
        {
            connectDB();
            sql_con.Open();
            sql_cmd = sql_con.CreateCommand();
            sql_cmd.CommandText = txtQuery;
            sql_cmd.ExecuteNonQuery();
            sql_con.Close();
        }
        private void LoadData()
        {
            connectDB();
            sql_con.Open();
            sql_cmd = sql_con.CreateCommand();
            string CommandText = "select * from processes";
            sql_dap = new SQLiteDataAdapter(CommandText, sql_con);
            dsProcesses.Reset();
            sql_dap.Fill(dsProcesses);
            dtProcesses = dsProcesses.Tables[0];
            this._dataGrid.DataSource = dtProcesses;
            sql_con.Close();
        }
    }
}
