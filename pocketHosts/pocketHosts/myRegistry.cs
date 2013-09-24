using System;

using System.Collections.Generic;
using System.Text;

using System.Runtime.InteropServices;

namespace pocketHosts
{
    class myregistry
    {
        [DllImport("coredll.dll", SetLastError=true)]
        static extern int RegOpenKeyEx(UIntPtr hKey, string lpSubKey, uint ulOptions,int samDesired, out IntPtr phkResult);

        [DllImport("coredll.dll", SetLastError = true)]
        static extern int RegOpenKeyEx(uint hKey, string lpSubKey, uint ulOptions, int samDesired, out IntPtr phkResult);

        [DllImport("coredll.dll", SetLastError = true)]
        static extern int RegDeleteKey(IntPtr hKey, string lpSubKey);

        [DllImport("coredll.dll", SetLastError = true)]
        static extern int RegDeleteKey(UIntPtr hKey, string lpSubKey);
        
        [DllImport("coredll.dll")]
         static extern int RegCloseKey(IntPtr hKey);

        const uint HKEY_LOCAL_MACHINE = 0x80000002;// IntPtr (-2147483646); // 0x80000002

        static int mRegOpenKeyEx(UIntPtr hKey, string lpSubKey, out IntPtr phkResult){
            int iRes = RegOpenKeyEx(hKey, lpSubKey, 0, 0, out phkResult);
            if (iRes != 0)
                System.Diagnostics.Debug.WriteLine("RegOpenKeyEx: LastError=" + Marshal.GetLastWin32Error().ToString());
            return iRes;
        }
        static int mRegOpenKeyEx(uint hKey, string lpSubKey, out IntPtr phkResult){
            int iRes = RegOpenKeyEx(hKey, lpSubKey, 0, 0, out phkResult);
            if (iRes != 0)
                System.Diagnostics.Debug.WriteLine("RegOpenKeyEx: LastError=" + Marshal.GetLastWin32Error().ToString());
            return iRes;
        }

        public static int RegDelKey(string lpSubKey)
        {
            IntPtr hKey;
            int iRes = mRegOpenKeyEx(HKEY_LOCAL_MACHINE, lpSubKey, out hKey);
            iRes = RegDeleteKey(hKey, lpSubKey);
            if (iRes != 0)
                System.Diagnostics.Debug.WriteLine("RegDelKey: LastError=" + Marshal.GetLastWin32Error().ToString());
            
            RegCloseKey(hKey);
            return iRes;
        }

        public static int RegDelKey(string lpSubKey, string sKey)
        {
            IntPtr hKey;
            int iRes = mRegOpenKeyEx(HKEY_LOCAL_MACHINE, lpSubKey, out hKey);
            iRes = RegDeleteKey(hKey, sKey);
            if (iRes != 0)
                System.Diagnostics.Debug.WriteLine("RegDelKey: LastError=" + Marshal.GetLastWin32Error().ToString());
            RegCloseKey(hKey);
            return iRes;
        }    }
}
