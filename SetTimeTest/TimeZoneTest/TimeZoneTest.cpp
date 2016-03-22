// TimeZoneTest.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

// time zone test: http://support.microsoft.com/kb/234735

#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include <ctype.h>
#include <time.h>
//#include <sys\timeb.h>
#include <windows.h>
//#include <process.h>
//#include <errno.h>
//#include <process.h>

#define BIAS1 ( *((DWORD*)0x7FFe0020) )
#define BIAS2 ( *((DWORD*)0x7FFe0024) )

TCHAR buf[200];   // message buffer

VOID FormatSt( SYSTEMTIME st, TCHAR* buf)
{
    swprintf(buf,L"%02d/%02d/%02d %02d:%02d:%02d",
        st.wYear, st.wMonth, st.wDay,
        st.wHour, st.wMinute, st.wSecond );
}

VOID PrintTZInfo()
{
    TIME_ZONE_INFORMATION tzi;
    DWORD dwSta;

    dwSta= GetTimeZoneInformation( &tzi );
   
    DEBUGMSG(1,(L"GetTimeZoneInformation: \n "));
    switch( dwSta )
    {
        case TIME_ZONE_ID_UNKNOWN:
            DEBUGMSG(1,(L"returned TIME_ZONE_ID_UNKNOWN\n"));
            break;

        case TIME_ZONE_ID_STANDARD:
            FormatSt( tzi.StandardDate, buf );
            DEBUGMSG(1,(L"Bias %d  Name: %S  SysDate: %s  Bias: %d\n",
                   tzi.Bias, tzi.StandardName, buf, tzi.StandardBias ));
            break;

        case TIME_ZONE_ID_DAYLIGHT:
            FormatSt( tzi.DaylightDate, buf );
            DEBUGMSG(1,(L"Bias %d  Name: %S  SysDate: %s  Bias: %d\n",
                   tzi.Bias, tzi.DaylightName, buf, tzi.DaylightBias ));
            break;

        default:
            DEBUGMSG(1,(L"returned undoced status: %d",dwSta));
            break;
    }
//    DEBUGMSG(1,(L" User_Shared_Data bias: %08x %08x\n\n", BIAS2, BIAS1 ));
}

VOID TstSetTime( int year, int mon, int day, int hour, int minute, int sec)
{
    SYSTEMTIME st,tst;
    BOOL bSta;

    st.wYear=  year;
    st.wMonth= mon;
    st.wDay=   day;
    st.wHour=  hour;
    st.wMinute= minute;
    st.wSecond= sec;

    st.wDayOfWeek= 0;
    st.wMilliseconds= 0;

    bSta= SetLocalTime( &st );

    if( bSta == FALSE )
    {
        FormatSt( st, buf);
        DEBUGMSG(1,(L"Failed to set date/time: %s\n",buf));
    }
    else
    {
        FormatSt( st, buf);
        DEBUGMSG(1,(L"SetLocalTime:  %s\n",buf));

        GetLocalTime( &tst );
        FormatSt( tst, buf);
        DEBUGMSG(1,(L"GetLocalTime:  %s\n", buf));

        GetSystemTime( &tst );
        FormatSt( tst, buf );
        DEBUGMSG(1,(L"GetSystemTime: %s\n", buf));
    }
    DEBUGMSG(1,(L"\n"));

}

VOID PrintTime( TCHAR* msg )
{
    SYSTEMTIME st;

    GetLocalTime( &st );

    FormatSt( st, (TCHAR*) buf );

    DEBUGMSG(1,(L"%s %s\n", msg, buf));

}

void doTest()
{

    // pick date in savings time

    TstSetTime( 1998, 8, 30, 22, 59, 0 );
    PrintTZInfo();

    // pick date outside of savings time

    DEBUGMSG(1,(L"\n"));
    TstSetTime( 1998, 12, 29, 22, 59, 0 );
    PrintTZInfo();
}

int _tmain(int argc, _TCHAR* argv[])
{
	SYSTEMTIME st;
	
	GetSystemTime(&st);

	doTest();

	SetSystemTime(&st);

	return 0;
}

