using System;

using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

//using __int64 = System.UInt64;
using DWORD = System.UInt32;
using FILETIME = System.Int64;

namespace System.Process
{
    public class process
    {
        public UInt32 dwProcID;
        public List<thread> threadsProc;
        public string sName;
        public process(UInt32 id, string n, List<thread> lth)
        {
            dwProcID = id;
            threadsProc = lth;
            sName = n;
        }
    }

    public partial class Process
    {
        public static string exefile
        {
            get {
                string s = System.IO.Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
                return s;
            }
        }
        public Process()
        {
            
        }
        
        //Dictionary<UInt32, process> _procList;
        public static Dictionary<UInt32, process> procList
        {
            get {
                Dictionary<UInt32, process> pList;
                pList = getProcessNameList();
                return pList;
            }
        }
        public static Dictionary<UInt32, process> getProcessNameList()
        {
            int iCnt = 0;
            //List<processnames> name_list = new List<processnames>();
            Dictionary<UInt32, process> _pList = new Dictionary<uint, process>();
            uint procID = 0;
            IntPtr pHandle = CreateToolhelp32Snapshot(SnapshotFlags.Process | SnapshotFlags.NoHeaps, procID);
            if ((Int32)pHandle == INVALID_HANDLE_VALUE)
                throw new Exception("CreateToolhelp32Snapshot error: " + Marshal.GetLastWin32Error().ToString());

            if ((int)pHandle != INVALID_HANDLE_VALUE)
            {
                PROCESSENTRY32 pEntry = new PROCESSENTRY32();
                pEntry.dwSize = (uint)Marshal.SizeOf(pEntry);
                if (Process32First(pHandle, ref pEntry) == 1)
                {
                    do
                    {
                        //name_list.Add(new processnames(pEntry.th32ProcessID, pEntry.szExeFile));
                        _pList[pEntry.th32ProcessID] = new process(pEntry.th32ProcessID, pEntry.szExeFile, new List<thread>());
                        iCnt++;
                    } while (Process32Next(pHandle, ref pEntry) == 1);
                }
                else
                    System.Diagnostics.Debug.WriteLine("Process32First error: " + Marshal.GetLastWin32Error().ToString());
                CloseToolhelp32Snapshot(pHandle);
            }

            return _pList;
        }

        /// Convert a FILETIME to ticks (ms)
        public static long GetThreadTick(FILETIME time)
        {
            //__int64 tick = MAKEDWORDLONG( time.dwLowDateTime, time.dwHighDateTime );
            long tick = time;
            return (tick /= 10000);
        }

        public static bool GetProcessTimes(ref PROCESSTIMES pTimes, uint procID)
        {
            bool bRet = true;
            pTimes.lpCreationTime = 0;
            pTimes.lpExitTime = 0;
            pTimes.lpKernelTime = 0;
            pTimes.lpUserTime = 0;
            pTimes.processID = procID;

            List<THREADENTRY32> threads = new List<THREADENTRY32>();
            //CosmicPowers we_are_powerful;
            UInt32 old_permissions = SetProcPermissions(0xffffffff);
            IntPtr snapshot = CreateToolhelp32Snapshot(SnapshotFlags.Thread | SnapshotFlags.NoHeaps, 0);
            if ((int)snapshot != -1)
            {
                THREADENTRY32 te = new THREADENTRY32();
                te.dwSize = (uint)Marshal.SizeOf(te);
                Int32 bRes = Thread32First(snapshot, ref te);
                FILETIME creation, exit, kernel, user;
                if (bRes > 0)
                {
                    do
                    {
                        if (te.th32OwnerProcessID == procID)
                        {
                            creation = new FILETIME();
                            exit = new FILETIME();
                            kernel = new FILETIME();
                            user = new FILETIME();
                            uint hThread = te.th32ThreadID;
                            if (GetThreadTimes(hThread,
                                                  out creation,
                                                  out exit,
                                                  out kernel,
                                                  out user))
                            {
                                threads.Add(te);
                                pTimes.lpKernelTime += kernel;
                                pTimes.lpUserTime += user;
                            }
                        }
                    } while (Thread32Next(snapshot, out te) > 0);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("error=" + Marshal.GetLastWin32Error());
                    bRet = false;
                }

                CloseToolhelp32Snapshot(snapshot);
            }
            SetProcPermissions(old_permissions);
            //threadList=threads.ToArray();
            return bRet;
        }

        /// Gets the list of currently running threads
        public static Dictionary<uint, thread> GetThreadList()
        {
            Dictionary<uint, thread> process_list = new Dictionary<uint, thread>();
            //CosmicPowers we_are_powerful;
            UInt32 old_permissions = SetProcPermissions(0xffffffff);
            IntPtr snapshot = CreateToolhelp32Snapshot(SnapshotFlags.Thread | SnapshotFlags.NoHeaps, 0);
            if ((int)snapshot != -1)
            {
                THREADENTRY32 te = new THREADENTRY32();
                te.dwSize = (uint)Marshal.SizeOf(te);
                Int32 bRes = Thread32First(snapshot, ref te);
                if (bRes > 0)
                {
                    do
                    {
                        FILETIME creation = new FILETIME();
                        FILETIME exit = new FILETIME();
                        FILETIME kernel = new FILETIME();
                        FILETIME user = new FILETIME();
                        uint hThread = te.th32ThreadID;
                        if (GetThreadTimes(hThread,
                                              out creation,
                                              out exit,
                                              out kernel,
                                              out user))
                        {
                            threadtimes t = new threadtimes(user, kernel);// = { kernel, user };
                            //t.kernel = kernel;
                            //t.user = user;
                            process_list[te.th32ThreadID] = new thread(te.th32OwnerProcessID, te.th32ThreadID, t);
                            //System.Diagnostics.Debug.WriteLine(te.th32OwnerProcessID.ToString() + ": " +
                            //    te.th32ThreadID.ToString("x08") +
                            //    ", " + DateTime.FromFileTime(t.kernel).Ticks.ToString() +
                            //    ", " + DateTime.FromFileTime(t.user).Ticks.ToString()
                            //    );
                        }
                    } while (Thread32Next(snapshot, out te) > 0);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("error=" + Marshal.GetLastWin32Error());
                }

                CloseToolhelp32Snapshot(snapshot);
            }
            SetProcPermissions(old_permissions);
            return process_list;
        }

        class obsolete
        {
            ///// Time a thread has spent working
            //public struct thread_times
            //{
            //    /// Time a thread has spent in kernel space
            //    public FILETIME kernel;
            //    /// Time a thread has spent in user space
            //    public FILETIME user;
            //};

            /// <summary>
            /// return the sum of all thread times of this process
            /// </summary>
            /// <param name="processID"></param>
            /// <param name="lpKernelTime"></param>
            /// <param name="lpUserTime"></param>
            /// <returns></returns>
            public static int GetProcessTimes(uint processID, ref FILETIME lpKernelTime, ref FILETIME lpUserTime,
                ref long duration)
            {
                //threadStruct[] myThreads = new threadStruct[255];//hopefully enough
                int iCount = 0;
                uint startTicks = 0;//, duration=0;
                long FTprocKernel = 0, FTprocUser = 0, FTprocTotal = 0;
                CosmicPowers cosmicPower = new CosmicPowers();
                IntPtr snapshot = CreateToolhelp32Snapshot(SnapshotFlags.Thread | SnapshotFlags.NoHeaps, 0);
                if ((int)snapshot != INVALID_HANDLE_VALUE)
                {
                    THREADENTRY32 te = new THREADENTRY32();
                    te.dwSize = (uint)Marshal.SizeOf(te);
                    startTicks = GetTickCount();
                    Int32 bRes = Thread32First(snapshot, ref te);
                    if (bRes > 0)
                    {
                        do
                        {
                            FILETIME creation = new FILETIME();
                            FILETIME exit = new FILETIME();
                            FILETIME kernel = new FILETIME();
                            FILETIME user = new FILETIME();
                            uint hThread = te.th32ThreadID;
                            if (te.th32OwnerProcessID == processID)
                            {
                                if (GetThreadTimes(hThread,
                                                      out creation,
                                                      out exit,
                                                      out kernel,
                                                      out user))
                                {
                                    //add the thread's values to our sum
                                    FTprocKernel += kernel;
                                    FTprocUser += user;
                                    FTprocTotal += kernel + user;
                                    //System.Diagnostics.Debug.WriteLine(
                                    //    te.th32OwnerProcessID.ToString() + ": \t" +
                                    //    kernel.ToString() + "\t" +
                                    //    user.ToString());
                                    iCount++;
                                }
                            }
                        } while (Thread32Next(snapshot, out te) > 0);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("error=" + Marshal.GetLastWin32Error());
                    }
                    CloseToolhelp32Snapshot(snapshot);
                }
                cosmicPower.Dispose();
                lpKernelTime = FTprocKernel;
                lpUserTime = FTprocUser;
                //System.Diagnostics.Debug.WriteLine("usage: " + ((float)(((FTprocKernelDuration+FTprocUserDuration) / 100f) * duration)).ToString("0.00%"));
                return iCount;
            }

        }
        public struct PROCESSTIMES
        {
            public uint processID;
            public FILETIME lpCreationTime;
            public FILETIME lpExitTime;
            public FILETIME lpKernelTime;
            public FILETIME lpUserTime;
        }
    }

}
