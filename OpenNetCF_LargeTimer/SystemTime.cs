using System;

using System.Collections.Generic;
using System.Text;

using System.Runtime.InteropServices;

namespace CEGETUSERNOTIFICATION
{
    [StructLayout(LayoutKind.Sequential)]
    public class SYSTEMTIME
    {
        public ushort wYear;
        public ushort wMonth;
        public ushort wDayOfWeek;
        public ushort wDay;
        public ushort wHour;
        public ushort wMinute;
        public ushort wSecond;
        public ushort wMilliseconds;

        public SYSTEMTIME(DateTime dtDateTime)
        {
            this.wYear = (ushort)dtDateTime.Year;
            this.wMonth = (ushort)dtDateTime.Month;
            this.wDayOfWeek = (ushort)dtDateTime.DayOfWeek;
            this.wDay = (ushort)dtDateTime.Day;
            this.wHour = (ushort)dtDateTime.Hour;
            this.wMinute = (ushort)dtDateTime.Minute;
            this.wSecond = (ushort)dtDateTime.Second;
            this.wMilliseconds = 0;
        }
        public SYSTEMTIME(byte[] buf, int idx)
        {
            wYear = BitConverter.ToUInt16(buf, idx); idx += 2;
            wMonth = BitConverter.ToUInt16(buf, idx); idx += 2;
            wDayOfWeek = BitConverter.ToUInt16(buf, idx); idx += 2;
            wDay = BitConverter.ToUInt16(buf, idx); idx += 2;
            wHour = BitConverter.ToUInt16(buf, idx); idx += 2;
            wMinute = BitConverter.ToUInt16(buf, idx); idx += 2;
            wSecond = BitConverter.ToUInt16(buf, idx); idx += 2;
            wMilliseconds = BitConverter.ToUInt16(buf, idx); idx += 2;
        }
        public static DateTime GetAsDateTime(SYSTEMTIME st)
        {
            return new DateTime(st.wYear, st.wMonth, st.wDay, st.wHour, st.wMinute, st.wSecond);
        }

        public static DateTime dtLocalDateTime
        {
            get{
                SYSTEMTIME sysTime = new SYSTEMTIME(DateTime.Now);
                GetLocalTime(sysTime);
                DateTime dtLocalTime = GetAsDateTime(sysTime);
                return dtLocalDateTime;
            }
        }
        [DllImport("coredll.dll")]
        public static extern void GetLocalTime(SYSTEMTIME sysTime);
    }

}
