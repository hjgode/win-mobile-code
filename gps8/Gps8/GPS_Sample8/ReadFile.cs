using System;

using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace GPS_Sample8
{
    public static class ReadFileClass
    {
        [DllImport("coredll.dll", SetLastError = true)]
        private unsafe static extern bool ReadFile(
            int hFile,                        // handle to file
            byte[] lpBuffer,                // data buffer
            int nNumberOfBytesToRead,        // number of bytes to read
            ref int lpNumberOfBytesRead,    // number of bytes read
            IntPtr ptr
            //
            // ref OVERLAPPED lpOverlapped        // overlapped buffer
            );
        public const uint GENERIC_READ = 0x80000000;
        public const uint GENERIC_WRITE = 0x40000000;
        public const uint CREATE_NEW = 1;
        public const uint CREATE_ALWAYS = 2;
        public const uint OPEN_EXISTING = 3;
        public const uint OPEN_ALWAYS = 4;
        public const int INVALID_HANDLE_VALUE = -1;

        [DllImport("coredll.dll", SetLastError = true)]
        static extern IntPtr CreateFile(
            String lpFileName, 
            UInt32 dwDesiredAccess, 
            UInt32 dwShareMode, 
            IntPtr lpSecurityAttributes, 
            UInt32 dwCreationDisposition, 
            UInt32 dwFlagsAndAttributes, 
            IntPtr hTemplateFile);

        public static int readFile(string sFile, ref string sOut)
        {
            IntPtr handle;
            int rc;
            handle = CreateFile(sFile,
                GENERIC_READ | GENERIC_WRITE,
                0,
                IntPtr.Zero,
                OPEN_EXISTING, 0, IntPtr.Zero);
            if (handle.ToInt32() == INVALID_HANDLE_VALUE)
            {
                rc = Marshal.GetLastWin32Error();
                return rc;
            }
            int rxBufferSize = 500;
            byte[]			readbuffer	= new Byte[rxBufferSize];
			int				bytesread	= 0;
            int iHandle = handle.ToInt32();
            if (ReadFile(iHandle, readbuffer, rxBufferSize, ref bytesread, IntPtr.Zero))
            {
                sOut = System.Text.ASCIIEncoding.ASCII.GetString(readbuffer, 0, bytesread);
                return 0;
            }
            else
            {
                sOut = "";
                return -1;
            }
        }
    }
}
