using System;
using System.Collections.Generic;

using System.Process;

using System.Net.Sockets;
using System.Net;

using System.Runtime.InteropServices;

using System.Threading;

//using __int64 = System.UInt64;
using DWORD = System.UInt32;
using FILETIME = System.Int64;

namespace ProcessUsage
{
    /// <summary>
    /// Provide a list of processes and there thread's cpu load
    /// Build a list of processes and another of threads
    /// assign the threads to the processes
    /// get statistics
    /// </summary>
    public class ProcInfo : IDisposable
    {
        /// <summary>
        /// handle to release capture
        /// </summary>
        WaitHandle eventEnableCapture;
        /// <summary>
        /// handle to release data send
        /// </summary>
        WaitHandle eventEnableSend;
        /// <summary>
        /// how often to build a usage list
        /// </summary>
        public int _iTimeOut = 3000;
        /// <summary>
        /// stats for every process
        /// </summary>
        Dictionary<string, ProcessStatistics.process_statistics> statisticsTimes;

        bool bStopMainThread = false;
        bool bStopSocketThread = false;

        /// <summary>
        /// include ourself within statisics?
        /// </summary>
        public bool bIncludeMySelf
        {
            get;
            set;
        }

        Thread myThread = null;

        //for udo send
        Thread myThreadSocket = null;
        Socket sendSocket;

        //Queue<ProcessStatistics.process_statistics> procStatsQueue;
        Queue<byte[]> procStatsQueueBytes;

        object lockQueue = new object();

        public ProcInfo()
        {
            statisticsTimes = new Dictionary<string, ProcessStatistics.process_statistics>();

            eventEnableCapture = new AutoResetEvent(true);
            eventEnableSend = new AutoResetEvent(false);

            //procStatsQueue = new Queue<ProcessStatistics.process_statistics>();
            procStatsQueueBytes = new Queue<byte[]>();

            myThreadSocket = new Thread(socketThread);
            myThreadSocket.Start();

            myThread = new Thread(usageThread);
            myThread.Start();
        }
        public void Dispose()
        {
            bStopMainThread = true;
            bStopSocketThread = true;
            if (myThread != null)
            {
                myThread.Abort();
                Thread.Sleep(100);
                myThread = null;
            }
            if (myThreadSocket != null)
            {
                myThreadSocket.Abort();
                Thread.Sleep(100);
                myThreadSocket = null;
            }
        }

        public void sendEndOfTransfer()
        {
            if (sendSocket != null)
                sendSocket.Send(ByteHelper.endOfTransferBytes);
        }

        /// <summary>
        /// send enqueued objects via UDP broadcast
        /// </summary>
        void socketThread()
        {
            System.Diagnostics.Debug.WriteLine("Entering socketThread ...");
            try
            {
                const int ProtocolPort = 3001;
                sendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                sendSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                sendSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, 32768);

                IPAddress sendTo = IPAddress.Broadcast;// IPAddress.Parse("192.168.128.255");  //local broadcast
                EndPoint sendEndPoint = new IPEndPoint(sendTo, ProtocolPort);

                //UdpClient udpC = new UdpClient("255.255.255.255", 1111);
                System.Diagnostics.Debug.WriteLine("Socket ready to send");

                while (!bStopSocketThread)
                {
                    //block until released by capture
                    eventEnableSend.WaitOne();
                    lock (lockQueue)
                    {
                        //if (procStatsQueue.Count > 0)
                        while (procStatsQueueBytes.Count > 0)
                        {
                            //ProcessStatistics.process_statistics pStat = procStatsQueue.Dequeue();
                            //byte[] buf = pStat.ToByte();
                            byte[] buf = procStatsQueueBytes.Dequeue();
                            if (ByteHelper.isEndOfTransfer(buf))
                                System.Diagnostics.Debug.WriteLine("sending <EOT>");

                            sendSocket.SendTo(buf, buf.Length, SocketFlags.None, sendEndPoint);
                            //System.Diagnostics.Debug.WriteLine("Socket send " + buf.Length.ToString() + " bytes");
                            //System.Diagnostics.Debug.WriteLine(pStat.dumpStatistics());
                            System.Threading.Thread.Sleep(2);
                        }
                    }
                    ((AutoResetEvent)eventEnableCapture).Set();
                }

            }
            catch (ThreadAbortException ex)
            {
                System.Diagnostics.Debug.WriteLine("ThreadAbortException: socketThread(): " + ex.Message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception: socketThread(): " + ex.Message);
            }
            System.Diagnostics.Debug.WriteLine("socketThread ENDED");
        }


        /// <summary>
        /// build thread and process list periodically and fire update event and enqueue results for the socket thread
        /// </summary>
        void usageThread()
        {
            try
            {
                int interval = 3000;

                uint start = Process.GetTickCount();
                Dictionary<uint, thread> old_thread_List;// = Process.GetThreadList();

                string exeFile = Process.exefile;
                //read all processes
                Dictionary<uint, process> ProcList = Process.getProcessNameList();
                DateTime dtCurrent = DateTime.Now;

                //######### var declarations
                Dictionary<uint, thread> new_ThreadList;
                uint duration;
                long system_total;
                long user_total, kernel_total;      //total process spend in user/kernel
                long thread_user, thread_kernel;    //times the thread spend in user/kernel
                DWORD dwProc;
                float user_percent;
                float kernel_percent;    
                ProcessStatistics.process_usage usage;
                ProcessStatistics.process_statistics stats = null;
                
                string sProcessName = "";
                List<thread> processThreadList = new List<thread>();

                //extended list
                List<threadStatistic> processThreadStatsList = new List<threadStatistic>(); //to store thread stats
                //threadtimes threadTimesTotal;
                while (!bStopMainThread)
                {
                    eventEnableCapture.WaitOne();
                    old_thread_List = Process.GetThreadList();  //build a list of threads with user and kernel times

                    System.Threading.Thread.Sleep(interval);

                    //get a new thread list
                    new_ThreadList = Process.GetThreadList();   //build another list of threads with user and kernel times, to compare

                    duration = Process.GetTickCount() - start;

                    ProcList = Process.getProcessNameList();    //update process list
                    dtCurrent = DateTime.Now;
                    system_total = 0;
                    statisticsTimes.Clear();
                    //look thru all processes
                    foreach (KeyValuePair<uint, process> p2 in ProcList)
                    {
                        //empty the process's thread list
                        processThreadList=new List<thread>();
                        processThreadStatsList = new List<threadStatistic>();

                        user_total     = 0;  //hold sum of thread user times for a process
                        kernel_total   = 0;  //hold sum of thread kernel times for a process
                        sProcessName = p2.Value.sName;

                        //SUM over all threads with that ProcID
                        dwProc = p2.Value.dwProcID;
                        foreach (KeyValuePair<uint, thread> kpNew in new_ThreadList)
                        {
                            thread_user = 0;
                            thread_kernel = 0;
                            //if the thread belongs to the process
                            if (kpNew.Value.dwOwnerProcID == dwProc)
                            {
                                //is there an old thread entry we can use to calc?
                                thread threadOld;
                                if (old_thread_List.TryGetValue(kpNew.Value.dwThreadID, out threadOld))
                                {
                                    thread_user=Process.GetThreadTick(kpNew.Value.thread_times.user) - Process.GetThreadTick(old_thread_List[kpNew.Value.dwThreadID].thread_times.user);
                                    user_total += thread_user;
                                    thread_kernel =Process.GetThreadTick(kpNew.Value.thread_times.kernel) - Process.GetThreadTick(old_thread_List[kpNew.Value.dwThreadID].thread_times.kernel);
                                    kernel_total += thread_kernel;
                                }
                                //simple list
                                thread threadsOfProcess = new thread(kpNew.Value.dwOwnerProcID, kpNew.Value.dwThreadID, kpNew.Value.thread_times);
                                processThreadList.Add(threadsOfProcess);
                                
                                //extended list
                                threadStatistic threadStats = 
                                    new threadStatistic(
                                        kpNew.Value.dwOwnerProcID, 
                                        kpNew.Value.dwThreadID, 
                                        new threadtimes(thread_user, thread_kernel), 
                                        duration, 
                                        dtCurrent.Ticks);
                                processThreadStatsList.Add(threadStats);

                            }//if dwProcID matches
                        }
                        //end of sum for process
                        user_percent      = (float)user_total / (float)duration * 100f;
                        kernel_percent    = (float)kernel_total / (float)duration * 100f;
                        system_total = user_total + kernel_total;

                        // update the statistics with this process' info
                        usage = new ProcessStatistics.process_usage(kernel_total, user_total);
                        // update process statistics
                        //stats = new ProcessStatistics.process_statistics(p2.Value.dwProcID, p2.Value.sName, usage, dtCurrent.Ticks, duration, processThreadList);
                        stats = new ProcessStatistics.process_statistics(p2.Value.dwProcID, p2.Value.sName, usage, dtCurrent.Ticks, duration, processThreadStatsList);
                        
                        //add or update the proc stats
                        if (exeFile != p2.Value.sName || bIncludeMySelf)
                        {
                            //if (sProcessName == "device.exe")
                            //    System.Diagnostics.Debug.WriteLine(stats.ToString());

                            statisticsTimes[p2.Value.sName] = stats;
                            //lock (lockQueue)
                            //{
                                //System.Diagnostics.Debug.WriteLine("Queue Adding " + stats.sName);
                                //procStatsQueue.Enqueue(stats);
                                procStatsQueueBytes.Enqueue(stats.ToByte());
                            //}
                        }
                        
                        start = Process.GetTickCount();
                    }//foreach process
 
                    onUpdateHandler(new ProcessStatsEventArgs(statisticsTimes, duration));
                    procStatsQueueBytes.Enqueue(ByteHelper.endOfTransferBytes);
                    ((AutoResetEvent)eventEnableSend).Set();
                    //dumpStatistics(statisticsTimes);
                }//while true
            }
            catch (ThreadAbortException ex)
            {
                System.Diagnostics.Debug.WriteLine("ThreadAbortException: usageThread(): " + ex.Message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception: usageThread(): " + ex.Message);
            }
            System.Diagnostics.Debug.WriteLine("Thread ENDED");
        }

        public delegate void updateEventHandler(object sender, ProcessStatsEventArgs eventArgs);
        public event updateEventHandler updateEvent;
        void onUpdateHandler(ProcessStatsEventArgs procStats)
        {
            //anyone listening?
            if (this.updateEvent == null)
                return;
            this.updateEvent(this, procStats);
        }
    }
}
