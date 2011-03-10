using System;

using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace NativeSync
{
    internal class CoreDLL
    {

        [DllImport("CoreDLL", SetLastError=true)]
        public static extern IntPtr CreateEvent(
            IntPtr AlwaysNull0, 
            [In, MarshalAs(UnmanagedType.Bool)]  bool ManualReset, 
            [In, MarshalAs(UnmanagedType.Bool)]  bool bInitialState,
            [In, MarshalAs(UnmanagedType.BStr)]  string Name);

        [DllImport("CoreDLL", SetLastError = true)]
        public static extern IntPtr CreateSemaphore(
          int alwaysZero,
          int lInitialCount,
          int lMaximumCount,
          string lpName
        );

 
        [DllImport("CoreDLL",SetLastError=true)]
        public static extern int ReleaseSemaphore(
            IntPtr hSemaphore,
            int lReleaseCount,
            out int lpPreviousCount
        );

        [DllImport("CoreDLL",SetLastError=true)]
        public static extern IntPtr CreateMutex(
          int alwaysZero ,
          [In, MarshalAs(UnmanagedType.Bool)] bool bInitialOwner,
          [In, MarshalAs(UnmanagedType.BStr)] string lpName
        );

        [DllImport("CoreDLL", SetLastError = true)]
        public static extern int ReleaseMutex(
            IntPtr hMutex
        );


        [DllImport("coredll.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("CoreDLL",SetLastError=true)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int WaitForMultipleObjects(int objectCount, IntPtr[] eventList, bool waitAll, int timeOut);

        [DllImport("CoreDLL", SetLastError = true)]
        public static extern int SetEventData(IntPtr hEvent, [In, MarshalAs(UnmanagedType.U4)]  int dwData);

        [DllImport("CoreDLL", SetLastError = true)]
        public static extern int GetEventData(IntPtr hEvent);

        [DllImport("CoreDLL", SetLastError = true)]
        public extern static int WaitForSingleObject(IntPtr hEvent, int timeout);

        [DllImport("coredll.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EventModify(IntPtr hEvent, [In, MarshalAs(UnmanagedType.U4)] int dEvent);

    }
}
