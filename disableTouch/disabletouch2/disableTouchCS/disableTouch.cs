using System;

using System.Collections.Generic;
using System.Text;

using System.Runtime.InteropServices;

namespace disableTouchCS
{
    public static class disableTouch
    {
        [DllImport("coredll.dll", SetLastError = true)]
        private static extern IntPtr GetDesktopWindow();

        [DllImport("touch.dll", SetLastError = true)]
        private static extern bool TouchRegisterWindow(IntPtr hwnd);

        [DllImport("touch.dll", SetLastError = true)]
        private static extern void TouchUnregisterWindow(IntPtr hwnd);

        private static IntPtr DesktopHandle = new IntPtr(0);

        public static Boolean Lock()
        {
            return TouchRegisterWindow(GetDesktopWindow());// do not use (DesktopHandle), will not work!
        }

        public static void UnLock()
        {
            TouchUnregisterWindow(GetDesktopWindow());
        }
    }
}
