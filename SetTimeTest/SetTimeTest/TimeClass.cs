using System;

using System.Collections.Generic;
using System.Text;

using System.Runtime.InteropServices;

//see http://support.microsoft.com/kb/234735

namespace SetTimeTest
{
    public class TimeClass
    {
        [DllImport("coredll.dll", SetLastError = true)]
        static extern bool SetLocalTime(ref SYSTEMTIME lpSystemTime);
        
        [DllImport("coredll.dll")]
        public static extern void GetLocalTime(out SYSTEMTIME lpSystemTime);

        [DllImport("coredll.dll")]
        public static extern void GetSystemTime(out SYSTEMTIME lpSystemTime);

        //special for daylight savings
        /*StandardDate
            Specifies a SYSTEMTIME structure that contains a date and local time when the transition from daylight time to standard time occurs on this operating system.
            If this date is not specified, the wMonth member in the SYSTEMTIME structure must be zero.
            If this date is specified, the DaylightDate value in the TIME_ZONE_INFORMATION structure must also be specified.
            This member supports two date formats:
            Absolute format specifies an exact date and time when standard time begins. In this form, the wYear, wMonth, wDay, wHour, wMinute, wSecond, and wMilliseconds members of the SYSTEMTIME structure are used to specify an exact date.
            
         * Day-in-month format is specified by setting the wYear member to zero, 
         * setting the wDayOfWeek member to an appropriate weekday, 
         * and using a wDay value in the range 1 through 5 to select the 
         * correct day in the month. 
         * Using this notation, the first Sunday in April can be specified, as can the last Thursday in October (5 is equal to the last).
        */
        public struct SYSTEMTIME
        {
            public short year;
            public short month;
            public short dayOfWeek;
            public short day;
            public short hour;
            public short minute;
            public short second;
            public short milliseconds;
        }

        public static string st2dateStr(SYSTEMTIME st)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(String.Format("{0:D4}-{1:D2}-{2:D2}",
                st.year, st.month, st.day));
            return sb.ToString();
        }
        public static string st2TimeStr(SYSTEMTIME st)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(String.Format("{0:D2}:{1:D2}:{2:D2}.{3:D3}",
                st.hour, st.minute, st.second, st.milliseconds));
            return sb.ToString();
        }

        static string[] daysOfWeek = new string[] {  "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
        public static string getDayOfWeekStr(int iDay)
        {
            if (iDay > 6)
                iDay = iDay % 7;
            return daysOfWeek[iDay];
        }
        public static string getDayInMonth(SYSTEMTIME st)
        {
            string sRet = "";
            if (st.year != 0)
                return TimeClass.st2dateStr(st);
            //year must be zero
            //dayOfWeek: 0 = Sun, 1=Mon ...
            //wDay = 1: first, 2: second, 3: third, 4: fourth, 5: last
            //ie dayOfWeek=0 => Sunday
            //wDay = 5 => last dayOfWeek(here=Sunday) in month 
            DateTime dt = new DateTime(DateTime.Now.Year, st.month, 1, st.hour, st.minute, st.second);

            DateTime dtSavings = new DateTime();
            dtSavings = dt;
            if (st.day == 5) //last occurence of ??? in month
            {
                dtSavings = LastDayOfWeekInMonth(dt, (DayOfWeek)(st.dayOfWeek));
            }
            else
            {
                int day = DayFinder.FindDay(DateTime.Now.Year, st.month, (DayOfWeek)(st.dayOfWeek), st.day);
                dtSavings = new DateTime(DateTime.Now.Year, st.month, day, st.hour, st.minute, st.second);
            }
            sRet = String.Format("{0:D4}.{1:D2}.{2:D2} {3:D2}:{4:D2}:{5:D2}",
                dtSavings.Year, dtSavings.Month, dtSavings.Day,
                dtSavings.Hour, dtSavings.Minute, dtSavings.Second);
            return sRet;
        }

        static DateTime LastDayOfWeekInMonth(DateTime day, DayOfWeek dow)
        {
            DateTime lastDay = new DateTime(day.Year, day.Month, 1, day.Hour, day.Minute, day.Second).AddMonths(1).AddDays(-1);
            DayOfWeek lastDow = lastDay.DayOfWeek;

            int diff = dow - lastDow;

            if (diff > 0) diff -= 7;

            System.Diagnostics.Debug.Assert(diff <= 0);

            return lastDay.AddDays(diff);
        }
        class DayFinder
        {

            //For example to find the day for 2nd Friday, February, 2016
            //=>call FindDay(2016, 2, DayOfWeek.Friday, 2)
            public static int FindDay(int year, int month, DayOfWeek Day, int occurance)
            {

                if (occurance == 0 || occurance > 5)
                    throw new Exception("Occurance is invalid");

                DateTime firstDayOfMonth = new DateTime(year, month, 1);
                //Substract first day of the month with the required day of the week 
                var daysneeded = (int)Day - (int)firstDayOfMonth.DayOfWeek;
                //if it is less than zero we need to get the next week day (add 7 days)
                if (daysneeded < 0) daysneeded = daysneeded + 7;
                //DayOfWeek is zero index based; multiply by the Occurance to get the day
                var resultedDay = (daysneeded + 1) + (7 * (occurance - 1));

                if (resultedDay > (firstDayOfMonth.AddMonths(1) - firstDayOfMonth).Days)
                    throw new Exception(String.Format("No {0} occurance of {1} in the required month", occurance, Day.ToString()));

                return resultedDay;
            }
        }
        static void SetLocalTime(System.DateTime dt)
        {
            SYSTEMTIME time2;
            time2.year = (short)dt.Year;
            time2.month = (short)dt.Month;
            time2.day = (short)dt.Day;
            time2.hour = (short)dt.Hour;
            time2.minute = (short)dt.Minute;
            time2.second = (short)dt.Second;
            time2.milliseconds = (short)dt.Millisecond;
            time2.dayOfWeek = (short)dt.DayOfWeek;

            SetLocalTime(ref time2);
        }

        static DateTime getLocalTime()
        {
            DateTime dt = DateTime.Now;
            SYSTEMTIME time1;
            GetLocalTime(out time1);
            dt = new DateTime((int)time1.year, (int)time1.month, (int)time1.day, (int)time1.hour, (int)time1.minute, (int)time1.second, (int)time1.milliseconds);
            return dt;
        }

        //#############################################
        /// <summary>
        /// [Win32 API call]
        /// The GetTimeZoneInformation function retrieves the current time-zone parameters.
        /// These parameters control the translations between Coordinated Universal Time (UTC)
        /// and local time.
        /// </summary>
        /// <param name="lpTimeZoneInformation">[out] Pointer to a TIME_ZONE_INFORMATION structure to receive the current time-zone parameters.</param>
        /// <returns>
        /// If the function succeeds, the return value is one of the following values.
        /// <list type="table">
        /// <listheader>
        /// <term>Return code/value</term>
        /// <description>Description</description>
        /// </listheader>
        /// <item>
        /// <term>TIME_ZONE_ID_UNKNOWN == 0</term>
        /// <description>
        /// The system cannot determine the current time zone. This error is also returned if you call the SetTimeZoneInformation function and supply the bias values but no transition dates.
        /// This value is returned if daylight saving time is not used in the current time zone, because there are no transition dates.
        /// </description>
        /// </item>
        /// <item>
        /// <term>TIME_ZONE_ID_STANDARD == 1</term>
        /// <description>
        /// The system is operating in the range covered by the StandardDate member of the TIME_ZONE_INFORMATION structure.
        /// </description>
        /// </item>
        /// <item>
        /// <term>TIME_ZONE_ID_DAYLIGHT == 2</term>
        /// <description>
        /// The system is operating in the range covered by the DaylightDate member of the TIME_ZONE_INFORMATION structure.
        /// </description>
        /// </item>
        /// </list>
        /// If the function fails, the return value is TIME_ZONE_ID_INVALID. To get extended error information, call GetLastError.
        /// </returns>
        [DllImport("coredll.dll", CharSet = CharSet.Auto)]
        public static extern int GetTimeZoneInformation(out TimeZoneInformation lpTimeZoneInformation);

        /// <summary>
        /// [Win32 API call]
        /// The SetTimeZoneInformation function sets the current time-zone parameters.
        /// These parameters control translations from Coordinated Universal Time (UTC)
        /// to local time.
        /// </summary>
        /// <param name="lpTimeZoneInformation">[in] Pointer to a TIME_ZONE_INFORMATION structure that contains the time-zone parameters to set.</param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
        /// </returns>
        [DllImport("coredll.dll", CharSet = CharSet.Auto)]
        private static extern bool SetTimeZoneInformation([In] ref TimeZoneInformation lpTimeZoneInformation);

        /// <summary>
        /// The TimeZoneInformation structure specifies information specific to the time zone.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct TimeZoneInformation
        {
            /// <summary>
            /// Current bias for local time translation on this computer, in minutes. The bias is the difference, in minutes, between Coordinated Universal Time (UTC) and local time. All translations between UTC and local time are based on the following formula:
            /// <para>UTC = local time + bias</para>
            /// <para>This member is required.</para>
            /// </summary>
            public int bias;
            /// <summary>
            /// Pointer to a null-terminated string associated with standard time. For example, "EST" could indicate Eastern Standard Time. The string will be returned unchanged by the GetTimeZoneInformation function. This string can be empty.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string standardName;
            /// <summary>
            /// A SystemTime structure that contains a date and local time when the transition from daylight saving time to standard time occurs on this operating system. If the time zone does not support daylight saving time or if the caller needs to disable daylight saving time, the wMonth member in the SystemTime structure must be zero. If this date is specified, the DaylightDate value in the TimeZoneInformation structure must also be specified. Otherwise, the system assumes the time zone data is invalid and no changes will be applied.
            /// <para>To select the correct day in the month, set the wYear member to zero, the wHour and wMinute members to the transition time, the wDayOfWeek member to the appropriate weekday, and the wDay member to indicate the occurence of the day of the week within the month (first through fifth).</para>
            /// <para>Using this notation, specify the 2:00a.m. on the first Sunday in April as follows: wHour = 2, wMonth = 4, wDayOfWeek = 0, wDay = 1. Specify 2:00a.m. on the last Thursday in October as follows: wHour = 2, wMonth = 10, wDayOfWeek = 4, wDay = 5.</para>
            /// </summary>
            public SYSTEMTIME standardDate;
            /// <summary>
            /// Bias value to be used during local time translations that occur during standard time. This member is ignored if a value for the StandardDate member is not supplied.
            /// <para>This value is added to the value of the Bias member to form the bias used during standard time. In most time zones, the value of this member is zero.</para>
            /// </summary>
            public int standardBias;
            /// <summary>
            /// Pointer to a null-terminated string associated with daylight saving time. For example, "PDT" could indicate Pacific Daylight Time. The string will be returned unchanged by the GetTimeZoneInformation function. This string can be empty.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string daylightName;
            /// <summary>
            /// A SystemTime structure that contains a date and local time when the transition from standard time to daylight saving time occurs on this operating system. If the time zone does not support daylight saving time or if the caller needs to disable daylight saving time, the wMonth member in the SystemTime structure must be zero. If this date is specified, the StandardDate value in the TimeZoneInformation structure must also be specified. Otherwise, the system assumes the time zone data is invalid and no changes will be applied.
            /// <para>To select the correct day in the month, set the wYear member to zero, the wHour and wMinute members to the transition time, the wDayOfWeek member to the appropriate weekday, and the wDay member to indicate the occurence of the day of the week within the month (first through fifth).</para>
            /// </summary>
            public SYSTEMTIME daylightDate;
            /// <summary>
            /// Bias value to be used during local time translations that occur during daylight saving time. This member is ignored if a value for the DaylightDate member is not supplied.
            /// <para>This value is added to the value of the Bias member to form the bias used during daylight saving time. In most time zones, the value of this member is –60.</para>
            /// </summary>
            public int daylightBias;
        }

        /// <summary>
        /// Sets new time-zone information for the local system.
        /// </summary>
        /// <param name="tzi">Struct containing the time-zone parameters to set.</param>
        public static void SetTimeZone(TimeZoneInformation tzi)
        {
            // set local system timezone
            SetTimeZoneInformation(ref tzi);
        }

        /// <summary>
        /// Gets current timezone information for the local system.
        /// </summary>
        /// <returns>Struct containing the current time-zone parameters.</returns>
        public static TimeZoneInformation GetTimeZone()
        {
            // create struct instance
            TimeZoneInformation tzi;

            // retrieve timezone info
            int currentTimeZone = GetTimeZoneInformation(out tzi);

            return tzi;
        }

        //########################### TEST #############################
        const long BIAS1 = ( 0x7FFe0020 );
        const long BIAS2 = ( 0x7FFe0024 );

        public enum TIME_ZONE_ID:uint
        {
            TIME_ZONE_ID_INVALID = 0xFFFFFFFF,
            TIME_ZONE_ID_UNKNOWN  = 0,
            TIME_ZONE_ID_STANDARD = 1,
            TIME_ZONE_ID_DAYLIGHT = 2,
        }
        const long TIME_ZONE_ID_INVALID = 0xFFFFFFFF;
        const long TIME_ZONE_ID_UNKNOWN  = 0;
        const long TIME_ZONE_ID_STANDARD = 1;
        const long TIME_ZONE_ID_DAYLIGHT = 2;

        static void FormatSt( SYSTEMTIME st, ref StringBuilder buf)
        {
            string s = st.year.ToString("00") + "/" +
                st.month.ToString("00") + "/" +
                st.day.ToString("00") + " " +
                st.hour.ToString("00") + ":" +
                st.minute.ToString("00") + ":" +
                st.second.ToString("00")
                ;
            buf = new StringBuilder(s);
            //sprintf(buf,"%02d/%02d/%02d %02d:%02d:%02d",
            //    st.wYear, st.wMonth, st.wDay,
            //    st.wHour, st.wMinute, st.wSecond );
        }

        static string PrintTZInfo()
        {
            TimeZoneInformation tzi;
            long dwSta;
            string sRet = "";
            StringBuilder buf = new StringBuilder();

            dwSta = GetTimeZoneInformation(out tzi);

            sRet += String.Format("GetTimeZoneInformation: \r\n");

            switch (dwSta)
            {
                case TIME_ZONE_ID_UNKNOWN:
                    sRet += String.Format("returned TIME_ZONE_ID_UNKNOWN\r\n");
                    break;

                case TIME_ZONE_ID_STANDARD:
                    FormatSt(tzi.standardDate, ref buf);
                    sRet += String.Format("TIME_ZONE_ID_STANDARD:\r\nBias {0}  Name: {1}  SysDate: {2}\r\nBias: {3}\r\n",
                           tzi.bias.ToString(), tzi.standardName, buf, tzi.standardBias.ToString());
                    break;

                case TIME_ZONE_ID_DAYLIGHT:
                    FormatSt(tzi.daylightDate, ref buf);
                    sRet += String.Format("TIME_ZONE_ID_DAYLIGHT:\r\nBias {0}  Name: {1}  SysDate: {2}  Bias: {3}\r\n",
                           tzi.bias.ToString(), tzi.daylightName, buf, tzi.daylightBias.ToString());
                    break;

                default:
                    sRet += String.Format("returned undoced status: {0}\r\n", dwSta);
                    break;
            }
            sRet += String.Format(" User_Shared_Data bias:\r\n\t{0:08x}\r\n\t{1:08x}\r\n\r\n", BIAS2, BIAS1);
            return sRet;
        }

        static string TstSetTime(int year, int mon, int day, int hour, int minute, int sec)
        {
            StringBuilder buf = new StringBuilder(250);   // message buffer
            string sRet = "";

            SYSTEMTIME st, tst;
            bool bSta;

            st.year = (short)year;
            st.month = (short)mon;
            st.day = (short)day;
            st.hour = (short)hour;
            st.minute = (short)minute;
            st.second = (short)sec;

            st.dayOfWeek = 0;
            st.milliseconds = 0;

            bSta = SetLocalTime(ref st);

            if (bSta == false)
            {
                FormatSt(st, ref buf);
                sRet = String.Format("Failed to set date/time: {0}\r\n", buf.ToString());
            }
            else
            {
                FormatSt(st, ref buf);
                sRet = String.Format("SetLocalTime:  {0}\r\n", buf.ToString());

                GetLocalTime(out tst);
                FormatSt(tst, ref buf);
                sRet += String.Format("GetLocalTime:  {0}\r\n", buf.ToString());

                GetSystemTime(out tst);
                FormatSt(tst, ref buf);
                sRet += String.Format("GetSystemTime: {0}\r\n", buf);
            }
            sRet += "\r\n";
            return sRet;
        }
        /*
                                                        DST START 2013 	DST END 2013 	DST START 2014 	
        EUROPE (except Iceland) - (time is UTC/GMT) 	31-Mar, 01:00h 	27-Oct, 01:00h 	30-Mar, 01:00h
        */
        public static string DoTZtest()
        {
            StringBuilder sb = new StringBuilder();
            DateTime dtOld = getLocalTime();
            sb.Append("Saved actual time" +dtOld.ToShortDateString()+","+dtOld.ToShortTimeString() + "\r\n");

            sb.Append("Setting cleanboot date...\r\n");
            //SetLocalTime(new DateTime(1980,1,6,0,0,0));
            sb.Append(TstSetTime(1980, 1, 6, 0, 0, 0));
            sb.Append("2nd Setting cleanboot date...\r\n");
            sb.Append(TstSetTime(1980, 1, 6, 0, 0, 0));

            // pick date in savings time
            sb.Append("Setting date within DST...\r\n");
            sb.Append(TstSetTime(2013, 5, 1, 22, 59, 0));   //date in DST frame
            sb.Append(PrintTZInfo());

            // pick date outside of savings time

            sb.Append("\r\n");
            sb.Append(TstSetTime(2013, 12, 29, 22, 59, 0));
            sb.Append (PrintTZInfo());

            sb.Append("restore old time\n");
            SetLocalTime(dtOld);

            return sb.ToString();
        }

        public static bool isAutoDST()
        {
            /*
            [HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Clock]
            “AutoDST”=dword:1
            */
            bool bRet = false;
            try
            {
                int iAutoDST = (int)Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Clock").GetValue("AutoDST", -1);
                if (iAutoDST > 0)
                    bRet = true;
            }
            catch (Exception) { }
            return bRet;
        }
    }
}
