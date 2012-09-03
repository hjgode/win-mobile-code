using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace StartLockTestCS
{
    class StartLock
    {
        [DllImport("startlock.dll", CharSet = CharSet.Unicode)]
        public static extern void LockStartMenu();

        [DllImport("startlock.dll", CharSet = CharSet.Unicode)]
        public static extern void UnlockStartMenu();

        [DllImport("startlock.dll", CharSet = CharSet.Unicode)]
        public static extern void LockStartBar();

        [DllImport("startlock.dll", CharSet = CharSet.Unicode)]
        public static extern void UnlockStartBar();

        [DllImport("startlock.dll", CharSet = CharSet.Unicode)]
        public static extern bool Lockdown(string windowText);

        [DllImport("startlock.dll", CharSet = CharSet.Unicode)]
        public static extern bool Unlockdown();
    }
}
