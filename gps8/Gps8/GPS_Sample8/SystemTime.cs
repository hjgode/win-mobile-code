#region Using directives

using System;
using System.Runtime.InteropServices;

#endregion

namespace PInvokeLibrary
{
    /// <summary>
    /// Summary description for SystemTime.
    /// </summary>
    public class SystemTimeLib
    {
        [DllImport("coredll.dll")]
        private extern static void GetSystemTime(ref SYSTEMTIME lpSystemTime);

        [DllImport("coredll.dll")]
        private extern static uint SetSystemTime(ref SYSTEMTIME lpSystemTime);

        private struct SYSTEMTIME 
        {
            public ushort wYear;
            public ushort wMonth; 
            public ushort wDayOfWeek; 
            public ushort wDay; 
            public ushort wHour; 
            public ushort wMinute; 
            public ushort wSecond; 
            public ushort wMilliseconds; 
        }
        //
        // TODO: Add constructor logic here
        //
        public SystemTimeLib()
        {
        }

        public static DateTime GetTime()
        {
            // Call the native GetSystemTime method
            // with the defined structure.
            SYSTEMTIME stime = new SYSTEMTIME();
            GetSystemTime(ref stime);
            
            // Show the current time.           
            string strDateTime=(
                stime.wDay.ToString() + "." + stime.wMonth.ToString() + "." + stime.wYear.ToString() + " "+
                stime.wHour.ToString() + ":"+ stime.wMinute.ToString()+":"+stime.wSecond.ToString() 
                );
            DateTime dt;
            try
            {
                dt = DateTime.Parse(strDateTime);
            }
            catch (SystemException sx)
            {
                System.Diagnostics.Debug.WriteLine("Exception in GetTime(): " + sx.Message + "\r\n");
                dt=DateTime.Now;
            }
            return dt;
        }
        public static void SetTime(DateTime dt)
        {
            // Call the native GetSystemTime method
            // with the defined structure.
            SYSTEMTIME systime = new SYSTEMTIME();
            //GetSystemTime(ref systime);
            systime.wDay = (ushort)dt.Day;
            systime.wMonth = (ushort)dt.Month;
            systime.wYear = (ushort)dt.Year;
            systime.wHour = (ushort)dt.Hour;
            systime.wMinute = (ushort)dt.Minute;
            systime.wSecond = (ushort)dt.Second;
            // Set the system clock ahead one hour.
            //systime.wHour = (ushort)(systime.wHour + 1 % 24);
            SetSystemTime(ref systime);
            System.Diagnostics.Debug.WriteLine(string.Format("New time: " + systime.wHour.ToString() + ":"
                + systime.wMinute.ToString()+"\r\n"));
        }
     }
}

