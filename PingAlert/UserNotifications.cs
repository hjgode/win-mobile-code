using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

public class Nachricht:IDisposable
{
    //this.notification1 = new Microsoft.WindowsCE.Forms.Notification();

    public Nachricht()
    {
        this.notification1 = new Microsoft.WindowsCE.Forms.Notification();
    }
    private Microsoft.WindowsCE.Forms.Notification notification1;
    public void Dispose(){
        notification1.Visible = false;
        notification1 = null;
        notification1.Dispose();
        Dispose();
    }
    private void showNotification(string s)
    {
        StringBuilder HTMLString = new StringBuilder();
        //HTMLString.Append(s);
        HTMLString.Append("<html><body>");
        HTMLString.Append(s);
        HTMLString.Append("<p>Dismiss Alert</p>");
        HTMLString.Append("<form method=\'GET\' action=notify>");
        HTMLString.Append("<input type='submit'>");
        HTMLString.Append("<input type=button name='cmd:2' value='Hide'>");
        HTMLString.Append("</body></html>");

        //Set the Text property to the HTML string.
        notification1.Text = HTMLString.ToString();

        //FileStream IconStream = new FileStream(".\\My Documents\\notify.ico",
        //    FileMode.Open, FileAccess.Read);
        //notification1.Icon = new Icon(IconStream, 16, 16);
        notification1.Caption = "PingAlert Notification";
        notification1.Critical = false;

        // Display icon up to 10 seconds.
        notification1.InitialDuration = 5;
        notification1.Visible = true;
        notification1.ResponseSubmitted += new Microsoft.WindowsCE.Forms.ResponseSubmittedEventHandler(notification1_ResponseSubmitted);
    }

    void notification1_ResponseSubmitted(object sender, Microsoft.WindowsCE.Forms.ResponseSubmittedEventArgs e)
    {
        notification1.Visible = false;
        notification1 = null;
        notification1.Dispose();
        
    }
    public void NewNotificationMethod(string str)
    {
        showNotification(str);
    }
}

namespace UserNotifications
{
    public sealed class UserNotifications
    {
        // Methods for memory allocation and marshalling.
        private const uint LMEM_FIXED = 0;
        private const uint LMEM_ZEROINIT = 0x0040;
        private const uint LPTR = (LMEM_FIXED | LMEM_ZEROINIT);

        private static IntPtr AllocHLocal( int bytes )
        {
            return( LocalAlloc(LPTR, (uint)bytes) );
        }

        private static void FreeHLocal( IntPtr hLocal )
        {
            if( hLocal != IntPtr.Zero ) 
            {
                if( IntPtr.Zero != LocalFree(hLocal) ) 
                {
                    throw new Win32Exception( Marshal.GetLastWin32Error() );
                }
                hLocal = IntPtr.Zero;
            }
        }

        private static IntPtr StringToHLocal( string s )
        {
            if( s == null ) 
            {
                return( IntPtr.Zero );
            } 
            else 
            {
                int nc = s.Length;
                int len = 2*(1+nc);
                IntPtr hLocal = AllocHLocal( len );
                if( hLocal == IntPtr.Zero ) 
                {
                    throw new OutOfMemoryException();
                } 
                else 
                {
                    Marshal.Copy( s.ToCharArray(), 0, hLocal, s.Length );
                    return( hLocal );
                }
            }
        }
        

        // Platform invoke declarations and calls
        [DllImport("coredll.dll",SetLastError=true)]
        private static extern IntPtr LocalAlloc( 
            uint uFlags, 
            uint uBytes 
            ); 

        [DllImport("coredll.dll",SetLastError=true)]
        private static extern IntPtr LocalFree( 
            IntPtr hMem 
            );

        [DllImport("coredll.dll",SetLastError=true)]
        private static extern IntPtr CeSetUserNotificationEx( 
            IntPtr h, 
            ref CE_NOTIFICATION_TRIGGER nt, 
            ref CE_USER_NOTIFICATION un 
            );

        [DllImport("coredll.dll",SetLastError=true)]
        private static extern IntPtr CeSetUserNotificationEx( 
            IntPtr h, 
            ref CE_NOTIFICATION_TRIGGER nt, 
            IntPtr un 
            );


        [DllImport("coredll.dll",SetLastError=true)]
        private static extern void GetSystemTime( 
            out SYSTEMTIME lpSystemTime 
            );

        [DllImport("coredll.dll",SetLastError=true)]
        private static extern bool SystemTimeToFileTime(
            ref SYSTEMTIME lpSystemTime,
            out long lpFileTime
            );

        [DllImport("coredll.dll",SetLastError=true)]
        private static extern bool FileTimeToLocalFileTime(
            ref long lpFileTime,
            out long lpLocalFileTime
            );

        [DllImport("coredll.dll",SetLastError=true)]
        private static extern bool FileTimeToSystemTime( 
            ref long lpFileTime, 
            out SYSTEMTIME lpSystemTime 
            ); 

        private static SYSTEMTIME DateTimeToSystemTime( DateTime dateTime )
        {
            SYSTEMTIME systemTime;
            long fileTime = dateTime.ToFileTime();
            FileTimeToLocalFileTime( ref fileTime, out fileTime );
            FileTimeToSystemTime( ref fileTime, out systemTime );
            return( systemTime );
        }

        // Structures. Note that all structures are LayoutKind.Sequential
        // on the .NET Compact Framework and there is no need to
        // plaform invoke [StructLayout(LayoutKind.Sequential)].

        private struct SmartString: IDisposable
        {
            public IntPtr szString;
            
            public SmartString( string s )
            {
                szString = StringToHLocal( s );
            }

            public void Dispose()
            {
                FreeHLocal( szString );
            }
        }

        private struct CE_NOTIFICATION_TRIGGER: IDisposable
        {
            public uint                    dwSize;
            public CNT_TYPE                dwType;
            public NOTIFICATION_EVENT    dwEvent;
            public SmartString            lpszApplication;
            public SmartString            lpszArguments;
            public SYSTEMTIME            startTime;
            public SYSTEMTIME            endTime;

            public CE_NOTIFICATION_TRIGGER( string application, string arguments,  
                DateTime start )
            {
                dwSize  = (uint)Marshal.SizeOf( typeof(CE_NOTIFICATION_TRIGGER) );
                dwType  = CNT_TYPE.CNT_TIME;
                dwEvent = NOTIFICATION_EVENT.NONE;
        
                lpszApplication    = new SmartString( application );
                lpszArguments    = new SmartString( arguments );

                startTime = DateTimeToSystemTime( start );
                endTime = new SYSTEMTIME();
            }

            public CE_NOTIFICATION_TRIGGER( DateTime start )
            {
                dwSize  = (uint)Marshal.SizeOf( typeof(CE_NOTIFICATION_TRIGGER) );
                dwType  = CNT_TYPE.CNT_TIME;
                dwEvent = NOTIFICATION_EVENT.NONE;
        
                lpszApplication    = new SmartString( null );
                lpszArguments    = new SmartString( null );

                startTime = DateTimeToSystemTime( start );
                endTime = new SYSTEMTIME();
            }

            public CE_NOTIFICATION_TRIGGER( string application, string arguments,
                NOTIFICATION_EVENT notificationEvent )
            {
                dwSize  = (uint)Marshal.SizeOf( typeof(CE_NOTIFICATION_TRIGGER) );
                dwType  = CNT_TYPE.CNT_EVENT;
                dwEvent = notificationEvent;
        
                lpszApplication    = new SmartString(application);
                lpszArguments    = new SmartString(arguments);

                startTime = DateTimeToSystemTime( DateTime.Now );
                endTime = new SYSTEMTIME();
            }

            public CE_NOTIFICATION_TRIGGER( NOTIFICATION_EVENT notificationEvent )
            {
                dwSize  = (uint)Marshal.SizeOf( typeof(CE_NOTIFICATION_TRIGGER) );
                dwType  = CNT_TYPE.CNT_EVENT;
                dwEvent = notificationEvent;
        
                lpszApplication    = new SmartString( null );
                lpszArguments    = new SmartString( null );

                startTime = DateTimeToSystemTime( DateTime.Now );
                endTime = DateTimeToSystemTime( DateTime.Now.AddDays(1) );
            }

            public void Dispose()
            {
                lpszApplication.Dispose();
                lpszArguments.Dispose();
            }
        }

        public enum ActionFlags
        {
            PUN_LED     = 1,
            PUN_VIBRATE = 2,
            PUN_DIALOG  = 4,
            PUN_SOUND   = 8,
            PUN_REPEAT  = 16
        }

        private struct CE_USER_NOTIFICATION: IDisposable
        {
            public ActionFlags actionFlags;
            public SmartString pwszDialogTitle;
            public SmartString pwszDialogText;
            public SmartString pwszSound;
            public uint           dwMaxSound;
            public uint           dwReserved;

            public CE_USER_NOTIFICATION( string title, string text )
            {
                actionFlags = ActionFlags.PUN_DIALOG;
                pwszDialogTitle = new SmartString( title );
                pwszDialogText = new SmartString( text );
                pwszSound = new SmartString( null );
                dwMaxSound = 0;
                dwReserved = 0;
            }

            public void Dispose()
            {
                pwszDialogTitle.Dispose();
                pwszDialogText.Dispose();
                pwszSound.Dispose();
            }

        }

        private enum CNT_TYPE: uint
        {
            CNT_EVENT        = 1,
            CNT_TIME        = 2,
            CNT_PERIOD        = 3,
            CNT_CLASSICTIME = 4
        }

        public enum NOTIFICATION_EVENT: uint
        {
            NONE           = 0,
            TIME_CHANGE    = 1,
            SYNC_END       = 2,
            ON_AC_POWER    = 3,
            OFF_AC_POWER   = 4,
            NET_CONNECT    = 5,
            NET_DISCONNECT = 6,
            DEVICE_CHANGE  = 7,
            IR_DISCOVERED  = 8,
            RS232_DETECTED = 9,
            RESTORE_END    = 10,
            WAKEUP         = 11,
            TZ_CHANGE      = 12
        }

        private struct SYSTEMTIME
        {
            public ushort Year;
            public ushort Month;
            public ushort DayOfWeek;
            public ushort Day;
            public ushort Hour;
            public ushort Minute;
            public ushort Second ;
            public ushort MilliSecond;
        }


        // Methods used by the application.
        private static DateTime SystemTimeToDateTime( SYSTEMTIME dateTime )
        {
            long lpFileTime;
            SystemTimeToFileTime( ref dateTime, out lpFileTime );
            return( DateTime.FromFileTime( lpFileTime ) );
        }

        public static void RunApplication( string application, string arguments,  
            DateTime start )
        {
            CE_NOTIFICATION_TRIGGER nt = 
                new CE_NOTIFICATION_TRIGGER( application, arguments, start );

            using( nt )
            {
                IntPtr hNotify = CeSetUserNotificationEx( IntPtr.Zero, ref nt, IntPtr.Zero );
                    
                if( hNotify == IntPtr.Zero )
                {
                    throw new Win32Exception( Marshal.GetLastWin32Error() );
                }
            }
        }

        public static void RunApplication( string application, string arguments,  
            NOTIFICATION_EVENT notificationEvent )
        {
            CE_NOTIFICATION_TRIGGER nt = 
                new CE_NOTIFICATION_TRIGGER( application, arguments, notificationEvent );

            using( nt )
            {
                IntPtr hNotify = CeSetUserNotificationEx( IntPtr.Zero, ref nt, IntPtr.Zero );
                    
                if( hNotify == IntPtr.Zero )
                {
                    throw new Win32Exception( Marshal.GetLastWin32Error() );
                }
            }
        }

        public static void NotifyDialog( string title, string text, DateTime start )
        {
            CE_NOTIFICATION_TRIGGER nt = 
                new CE_NOTIFICATION_TRIGGER( start );
            
            CE_USER_NOTIFICATION un = new CE_USER_NOTIFICATION( title, text );

            using( nt )
            using( un )
            {
                IntPtr hNotify = CeSetUserNotificationEx( IntPtr.Zero, ref nt, ref un );
                    
                if( hNotify == IntPtr.Zero )
                {
                    throw new Win32Exception( Marshal.GetLastWin32Error() );
                }
            }
        }

        public static void NotifyDialog( string title, string text,  
            NOTIFICATION_EVENT notificationEvent )
        {

            CE_NOTIFICATION_TRIGGER nt = 
                new CE_NOTIFICATION_TRIGGER( notificationEvent );

            CE_USER_NOTIFICATION un = new CE_USER_NOTIFICATION( title, text );

            using( nt )
            using( un )
            {
                IntPtr hNotify = CeSetUserNotificationEx( IntPtr.Zero, ref nt, ref un );
                    
                if( hNotify == IntPtr.Zero )
                {
                    throw new Win32Exception( Marshal.GetLastWin32Error() );
                }
            }
        }

    }
}

