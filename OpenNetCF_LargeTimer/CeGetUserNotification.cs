// http://onehourofdevelopment.blogspot.com/2009_10_01_archive.html

using System;
using System.Collections.Generic;
using System.Text;

using System.Runtime.InteropServices;

using System.Windows.Forms;

namespace CEGETUSERNOTIFICATION
{
    class CeGetUserNotification
    {
        /*
        CeGetUserNotificationHandles returns an array of notification handles. Each of the handles can then be used as 
        input for CeGetUserNotification which in turn will return the notification as a byte array.
        To determine the size of rghNotifications needed, call CeGetUserNotificationHandless with NULL parameters.

        uint dwHowMany=0;
        GetUserNotificationHandles(null, 0 ref dwHowMany);
        */
        /*
        You then use the value of dwHowMany to create an IntPtr array of the correct size.

        IntPtr rghNotifications = new IntPtr[dwHowMany];

        Calling CeGetUserNotificationHandles one more time will fill the IntPtr array.

        GetUserNotificationHandles(rghNotifications, dwHowMany, ref dwHowMany);

        Given that GetUserNotificationHandles returned TRUE you should now have an IntPtr array filled with notification handles that can you can use. 
    */
        [DllImport("CoreDLL.dll", SetLastError = false, EntryPoint = "CeClearUserNotification")]
        public static extern bool ClearUserNotification(IntPtr hNotification);

        /*
        IntPtr hNotification;
        Call CeSetUserNotificationEx and save its output in hNotification
        */

        /*
        if ( ClearUserNotification(hNotification) )
          MessageBox.Show("Successfully cleared the notification");
        else
          MessageBox.Show("Failed to clear the notification");
        */

        [DllImport("CoreDLL.dll", SetLastError = false, EntryPoint = "CeGetUserNotificationHandles")]
        public static extern bool GetUserNotificationHandles(IntPtr[] rghNotifications, uint cHandles, ref uint pcHandlesNeeded);

        [DllImport("CoreDLL.dll", SetLastError = false, EntryPoint = "CeGetUserNotification")]
        public static extern bool GetUserNotification(IntPtr hNotification, uint BufferSize, out uint BytesNeeded, byte[] Buffer);
        /*
        Calling GetUserNotification passing null values and a valid notification handle will return the bytes needed for the byte 
        buffer. You then call GetUserNotification once more passing both a notification handle as well as a byte buffer.

        GetUserNotification(hNotification, 0, out Size, null);
        Buf = new byte[Size];
        GetUserNotification(hNotification, Size, out Size, Buf);
        */
        
        [DllImport("CoreDLL.dll", SetLastError = false, EntryPoint = "CeSetUserNotificationEx")]
        public static extern uint SetUserNotificationEx([MarshalAs(UnmanagedType.U4)]uint hNotification, CE_NOTIFICATION_TRIGGER Trigger, CE_USER_NOTIFICATION Notification);

        public CE_USER_NOTIFICATION[] getUserNotifications(){
            //how many usernotifications are there?
            IntPtr[] unHandles = new IntPtr[1];
            uint iHandleCount = 1;
            uint iHandlesNeeded = 0;
            if (!GetUserNotificationHandles(unHandles, iHandleCount, ref iHandlesNeeded))
                return null;
            //we got some handle count
            unHandles = new IntPtr[iHandlesNeeded];
            iHandleCount = iHandlesNeeded;
            if (!GetUserNotificationHandles(unHandles, iHandleCount, ref iHandlesNeeded))
                return null;
            CE_USER_NOTIFICATION[] userNotifications = new CE_USER_NOTIFICATION[iHandleCount];
            for (int i = 0; i < iHandleCount; i++)
            {
                uint uBytesNeeded=0;
                if (GetUserNotification(unHandles[i], 0, out uBytesNeeded, null)==false)
                {
                    byte[] buf = new byte[uBytesNeeded];
                    uint uBufSize = uBytesNeeded;
                    GetUserNotification(unHandles[i], uBufSize, out uBytesNeeded, buf);
                    int iIdx = 0;
                    userNotifications[i] = new CE_USER_NOTIFICATION(buf, (uint)buf.Length, ref iIdx);
                }
                else
                    System.Diagnostics.Debug.WriteLine("GetUserNotification: " + Marshal.GetLastWin32Error().ToString());
            }
            return userNotifications;
        }
        public void runAppAtTime(string sApp, SYSTEMTIME stStart, SYSTEMTIME stEnd)
        {
            CE_NOTIFICATION_TRIGGER cnt = new CE_NOTIFICATION_TRIGGER();
            cnt.Type = CeNotificationType.CNT_TIME;
            cnt.StartTime = stStart;
            cnt.EndTime = stEnd;
            cnt.pApplication = @"\windows\fexplore.exe";
            cnt.pArgs = "";
            cnt.Size = (UInt32)Marshal.SizeOf(cnt); // Needs to compile with /unsafe

            uint hNotificationEx = SetUserNotificationEx(0, cnt, null);
            if (hNotificationEx > 0)
                MessageBox.Show("Successfully created a notification. Handle: " + hNotificationEx.ToString());
            else
                MessageBox.Show("Failed to create a notification");
        }
        /*
        The following code will create a time based trigger and connect that trigger to a notification that will fire up \windows\fexplore.exe.
        Note that you need to compile this sample with the /unsafe switch. You could always hard code the size of CE_NOTIFICATION_TRIGGER in which case /unsafe won't be necessary. If hard coded; the value is 52.
        Also don't forget to pass NULL instead of ceun if you change the trigger type to CNT_EVENT.
        */
        public void newRunAtTime(SYSTEMTIME stStart, SYSTEMTIME stEnd){
            CE_NOTIFICATION_TRIGGER cnt = new CE_NOTIFICATION_TRIGGER();
            CE_USER_NOTIFICATION ceun = new CE_USER_NOTIFICATION();
            cnt.Type =CeNotificationType.CNT_TIME;
            cnt.StartTime = stStart;
            cnt.EndTime = stEnd;
            cnt.pApplication = @"\windows\fexplore.exe";
            cnt.pArgs = "";
            cnt.Size = (UInt32)Marshal.SizeOf(cnt); // Needs to compile with /unsafe

            ceun.ActionFlags = PUN_FLAGS.PUN_LED;
            ceun.sDialogText = "Dialogtext";
            ceun.sDialogTitle = "Title";
            ceun.nMaxSound = 0;

            uint hNotificationEx = SetUserNotificationEx(0, cnt, ceun);
            if (hNotificationEx > 0)
                MessageBox.Show("Successfully created a notification. Handle: " + hNotificationEx.ToString());
            else
                MessageBox.Show("Failed to create a notification");
        }

        public enum CeNotificationType
        {
            CNT_EVENT = 1,          //@flag CNT_EVENT  | System event notification
            CNT_TIME,               //@flag CNT_TIME   | Time-based notification
            CNT_PERIOD,             //@flag CNT_PERIOD | Time-based notification is active for
                                    // time period between stStart and stEnd
            CNT_CLASSICTIME         //@flag CNT_CLASSICTIME | equivalent to using (obsolete)
            // CeSetUserNotification function - standard command line is
            // supplied. lpszArguments must be NULL
        }
        [StructLayout(LayoutKind.Sequential)]
        public class CE_NOTIFICATION_INFO_HEADER
        {
            [MarshalAs(UnmanagedType.U4)]
            public uint hNotification;
            [MarshalAs(UnmanagedType.U4)]
            public uint dwStatus;
            public CE_NOTIFICATION_TRIGGER cent;
            public CE_USER_NOTIFICATION ceun;

            public CE_NOTIFICATION_INFO_HEADER(byte[] Buf, uint Size)
            {
                int Index = 0;
                try
                {
                    this.hNotification = BitConverter.ToUInt32(Buf, Index);
                    Index += 4;
                    this.dwStatus = BitConverter.ToUInt32(Buf, Index);
                    Index += 4;
                    uint pcent = BitConverter.ToUInt32(Buf, Index);
                    Index += 4;
                    uint pceun = BitConverter.ToUInt32(Buf, Index);
                    Index += 4;
                    if (pcent > 0 && Buf[Index] == Marshal.SizeOf(new CE_NOTIFICATION_TRIGGER()))
                        this.cent = new CE_NOTIFICATION_TRIGGER(Buf, Size, ref Index);
                    if (pceun > 0)
                        this.ceun = new CE_USER_NOTIFICATION(Buf, Size, ref Index);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        public class CE_NOTIFICATION_TRIGGER
        {
            public UInt32 Size = 0;
            [MarshalAs(UnmanagedType.U4)]
            public CeNotificationType Type;
            [MarshalAs(UnmanagedType.U4)]
            public CeNotificationEvent Event;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pApplication;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pArgs;
            public SYSTEMTIME StartTime;
            public SYSTEMTIME EndTime;
            public CE_NOTIFICATION_TRIGGER()
            {
            }

            public CE_NOTIFICATION_TRIGGER(byte[] Buf, uint Size, ref int Index)
            {
                int iEndOfString;
                System.Text.UnicodeEncoding enc = new UnicodeEncoding();
                uint lpszApp = 0;
                uint lpszArgs = 0;
                this.Size = BitConverter.ToUInt32(Buf, Index);
                Index += 4;
                this.Type = (CeNotificationType)BitConverter.ToUInt32(Buf, Index);
                Index += 4;
                this.Event = (CeNotificationEvent)BitConverter.ToUInt32(Buf, Index);
                Index += 4;
                lpszApp = BitConverter.ToUInt32(Buf, Index);
                Index += 4;
                lpszArgs = BitConverter.ToUInt32(Buf, Index);
                Index += 4;
                this.StartTime = new SYSTEMTIME(Buf, Index);
                Index += 16;
                this.EndTime = new SYSTEMTIME(Buf, Index);
                Index += 16;
                if (lpszApp > 0)
                {
                    iEndOfString = WinCE.WinCE.findEndOfString(Buf, Index, (int)Size);
                    this.pApplication = enc.GetString(Buf, Index, iEndOfString - Index - 2);
                    Index = iEndOfString;
                }
                if (lpszArgs > 0)
                {
                    iEndOfString = WinCE.WinCE.findEndOfString(Buf, Index, (int)Size);
                    this.pArgs = enc.GetString(Buf, Index, iEndOfString - Index - 2); // Stupid solution to double null ending..
                    Index = iEndOfString;
                }
            }
        }
        public enum CeNotificationEvent
        {
            NOTIFICATION_EVENT_NONE,
            NOTIFICATION_EVENT_TIME_CHANGE,
            NOTIFICATION_EVENT_SYNC_END,
            NOTIFICATION_EVENT_ON_AC_POWER,
            NOTIFICATION_EVENT_OFF_AC_POWER,
            NOTIFICATION_EVENT_NET_CONNECT,
            NOTIFICATION_EVENT_NET_DISCONNECT,
            NOTIFICATION_EVENT_DEVICE_CHANGE,
            NOTIFICATION_EVENT_IR_DISCOVERED,
            NOTIFICATION_EVENT_RS232_DETECTED,
            NOTIFICATION_EVENT_RESTORE_END,
            NOTIFICATION_EVENT_WAKEUP,
            NOTIFICATION_EVENT_TZ_CHANGE,
            NOTIFICATION_EVENT_MACHINE_NAME_CHANGE,
            NOTIFICATION_EVENT_RNDIS_FN_DETECTED,
            NOTIFICATION_EVENT_INTERNET_PROXY_CHANGE, 
            NOTIFICATION_EVENT_LAST = NOTIFICATION_EVENT_INTERNET_PROXY_CHANGE
        };
        public enum PUN_FLAGS
        {
            PUN_LED = 1 ,            //@flag PUN_LED | LED flag.  Set if the LED should be
                // flashed when the notification occurs.
            
            PUN_VIBRATE = 2,         //@flag PUN_VIBRATE | Vibrate flag.  Set if the device should
                // be vibrated.
            
            PUN_DIALOG = 4,          //@flag PUN_DIALOG | Dialog flag.  Set if a dialog should be
                // displayed (the app must provide title and text
                // when calling <f CeSetUserNotification>).
            
            PUN_SOUND = 8,           //@flag PUN_SOUND | Sound flag.  Set if the sound specified
                // in pwszSound should be played.
            
            PUN_REPEAT = 16,         //@flag PUN_REPEAT | Sound repeat flag.  Set if the sound
                // specified in pwszSound should be repeated progressively.
            
            PUN_PRIVATE = 32        //@flag PUN_PRIVATE | Dialog box z-order flag.  Set if the
            // notification dialog box should come up behind the password.
        };
        [StructLayout(LayoutKind.Sequential)]
        public class CE_USER_NOTIFICATION
        {
            [MarshalAs(UnmanagedType.U4)]
            public PUN_FLAGS ActionFlags;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string sDialogTitle;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string sDialogText;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string sSound;
            [MarshalAs(UnmanagedType.U4)]
            public uint nMaxSound;
            [MarshalAs(UnmanagedType.U4)]
            public uint Reserved;
            public CE_USER_NOTIFICATION()
            {
            }

            public CE_USER_NOTIFICATION(byte[] Buf, uint Size, ref int Index)
            {
                int iEndOfString;
                System.Text.UnicodeEncoding enc = new UnicodeEncoding();

                this.ActionFlags = (PUN_FLAGS)BitConverter.ToUInt32(Buf, Index);
                Index += 4;
                uint lpszDialogTitle = BitConverter.ToUInt32(Buf, Index);
                Index += 4;
                uint lpszDialogText = BitConverter.ToUInt32(Buf, Index);
                Index += 4;
                uint lpszSound = BitConverter.ToUInt32(Buf, Index);
                Index += 4;
                this.nMaxSound = BitConverter.ToUInt32(Buf, Index);
                Index += 4;
                // read Reserved
                Index += 4;
                if (lpszDialogTitle > 0)
                {
                    iEndOfString = WinCE.WinCE.findEndOfString(Buf, Index, (int)Size);
                    this.sDialogTitle = enc.GetString(Buf, Index, iEndOfString - Index - 2);
                    Index = iEndOfString;
                }
                if (lpszDialogText > 0)
                {
                    iEndOfString = WinCE.WinCE.findEndOfString(Buf, Index, (int)Size);
                    this.sDialogText = enc.GetString(Buf, Index, iEndOfString - Index - 2);
                    Index = iEndOfString;
                }
                if (lpszSound > 0)
                {
                    iEndOfString = WinCE.WinCE.findEndOfString(Buf, Index, (int)Size);
                    this.sSound = enc.GetString(Buf, Index, iEndOfString - Index - 2);
                    Index = iEndOfString;
                }
            }
        }
        // CE_NOTIFICATION_INFO_HEADER cenih = new Notify.CE_NOTIFICATION_INFO_HEADER(Buf, Size);
    }
}
