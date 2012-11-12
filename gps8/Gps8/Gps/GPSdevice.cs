#region Using directives

using System;
using System.Runtime.InteropServices;
using PInvokeLibrary;

#endregion
namespace Microsoft.WindowsMobile.Samples.Location
{
    [StructLayout(LayoutKind.Sequential)]
    public class GpsDeviceControl
    {
        [DllImport("coredll.dll", SetLastError = true)]
        //public static extern int DeviceIoControl(
        //    IntPtr hDevice,
        //    uint dwIoControlCode,
        //    IntPtr lpInBuffer,
        //    int nInBufferSize,
        //    IntPtr lpOutBuffer,
        //    int nOutBufferSize,
        //    ref int lpBytesReturned,
        //    IntPtr lpOverlapped
        //);
        public static extern int DeviceIoControl(
            IntPtr hDevice,
            uint dwIoControlCode,
            IntPtr lpInBuffer,
            int nInBufferSize,
            IntPtr lpOutBuffer,
            int nOutBufferSize,
            IntPtr lpBytesReturned,
            IntPtr lpOverlapped
        );

        [DllImport("coredll.dll", SetLastError = true)]
        public static extern IntPtr CreateFile(
            String lpFileName,                          // file name
            uint dwDesiredAccess,                      // access mode
            uint dwShareMode,                                // share mode
            IntPtr pAttr, //SecurityAttributes attr,                // SD
            uint dwCreationDisposition,            // how to create
            uint dwFlagsAndAttributes,            // file attributes
            uint hTemplateFile);                      // handle to template file

        public const uint IOCTL_SERVICE_START = 0x41000004;
        public const uint IOCTL_SERVICE_STOP = 0x41000008;
        public const uint IOCTL_SERVICE_REFRESH = 0x4100000C;
        public const uint IOCTL_SERVICE_STATUS = 0x41000020;
        public const uint GENERIC_READ = 0x80000000;
        public const uint GENERIC_WRITE = 0x40000000;
        public const uint CREATE_NEW          =1;
        public const uint CREATE_ALWAYS       =2;
        public const uint OPEN_EXISTING       =3;
        public const uint OPEN_ALWAYS = 4;
        public const int INVALID_HANDLE_VALUE = -1;

        private static string GetGPSPort()
        {
            string szStr = "";
            if (Registry.GetStringValue(Registry.HKLM,
                            "System\\CurrentControlSet\\GPS Intermediate Driver\\Multiplexer",
                            "DriverInterface",
                            ref szStr)
                == 0)
            {
                return szStr;
            }
            else
            {
                if (Registry.GetStringValue(Registry.HKLM,
                    "System\\CurrentControlSet\\GPS Intermediate Driver\\Drivers",
                    "CurrentDriver",
                    ref szStr) == 0)
                {
                    string szPath = "System\\CurrentControlSet\\GPS Intermediate Driver\\Drivers\\" + szStr;
                    if (Registry.GetStringValue(Registry.HKLM, szPath, "CommPort", ref szStr) == 0)
                    {
                        return szStr;
                    }
                }
            }
            return "";
        }

        public static int SendIoctl(uint code)
        {
            int rc;
            IntPtr handle;
            string gpsPort = GetGPSPort();
            if (gpsPort == "")
                return -1;

            handle = CreateFile("GPD0:",
                GENERIC_READ | GENERIC_WRITE,
                0,
                IntPtr.Zero,
                OPEN_EXISTING, 0, 0);
            if (handle.ToInt32() == INVALID_HANDLE_VALUE)
            {
                rc = Marshal.GetLastWin32Error();
                return rc;
            }

            int numBytesReturned = 0;
            rc = DeviceIoControl( //GPSID.NativeFile.
                handle,
                IOCTL_SERVICE_REFRESH, //code, //IOCTL_SERVICE_STOP,
                IntPtr.Zero,
                0,
                IntPtr.Zero,
                0,
                IntPtr.Zero, //ref numBytesReturned,
                IntPtr.Zero);
            int error = Marshal.GetLastWin32Error();

            CloseHandle(handle);

            if (rc == 0)
            {
                return error;
            }
            else
            {
                return 0;
            }
        }
        //internal virtual bool CloseHandle(IntPtr hPort) { return false; }
        [DllImport("coredll.dll", EntryPoint = "CloseHandle", SetLastError = true)]
        private static extern int CECloseHandle(IntPtr hObject);
        
        private static bool CloseHandle(IntPtr hPort)
        {
            return Convert.ToBoolean(CECloseHandle(hPort));
        }

    }
}