// TimeZoneSet.cpp : Defines the entry point for the application.
//

#include "stdafx.h"

typedef void    (*INITCITYDB)(void);
typedef void    (*UNINITCITYDB)(void);
typedef void    (*LOADTZDATA)(void);
typedef void    (*FREETZDATA)(void);
typedef int        (*GETNUMZONES)(void);
typedef void *    (*GETTZDATABYOFFSET)(int, int*);
typedef void *    (*GETTZDATA)(int);

struct TZData
{
    TCHAR *Name;
    TCHAR *ShortName;
    TCHAR *DSTName;
    int GMTOffset;
    int DSTOffset;
};

typedef struct tagTZREG {	 //  44 bytes
    LONG    Bias;			 //   4 bytes
    LONG    StandardBias;
    LONG    DaylightBias;
    SYSTEMTIME StandardDate; // 8*2 bytes
    SYSTEMTIME DaylightDate;
} TZREG;

/*
typedef struct _SYSTEMTIME {
    WORD wYear;
    WORD wMonth;
    WORD wDayOfWeek;
    WORD wDay;
    WORD wHour;
    WORD wMinute;
    WORD wSecond;
    WORD wMilliseconds;
} SYSTEMTIME, *LPSYSTEMTIME;
*/
	/*
	typedef struct _TIME_ZONE_INFORMATION {
		LONG Bias;
		WCHAR StandardName[ 32 ];
		SYSTEMTIME StandardDate;
		LONG StandardBias;
		WCHAR DaylightName[ 32 ];
		SYSTEMTIME DaylightDate;
		LONG DaylightBias;
	} TIME_ZONE_INFORMATION, *LPTIME_ZONE_INFORMATION;
	88,ff,ff,ff,
	45,00,2e,00,20,00,45,00,75,00,72,00,6f,00,70,00,65,00,20,00,53,00,74,00,61,00,6e,00,64,00,61,00,72,00,64,00,20,00,54,00,69,00,6d,00,65,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,09,00,00,00,05,00,01,00,00,00,00,00,00,00,00,00,00,00,45,00,2e,00,20,00,45,00,75,00,72,00,6f,00,70,00,65,00,20,00,44,00,61,00,79,00,6c,00,69,00,67,00,68,00,74,00,20,00,54,00,69,00,6d,00,65,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,03,00,00,00,05,00,00,00,00,00,00,00,00,00,
	c4,ff,ff,ff
	
	*/

void ShowError(LONG er)
{
	LPVOID lpMsgBuf;
	FormatMessage( 
		FORMAT_MESSAGE_ALLOCATE_BUFFER | 
		FORMAT_MESSAGE_FROM_SYSTEM | 
		FORMAT_MESSAGE_IGNORE_INSERTS,
		NULL,
		er,
		0, // Default language
		(LPTSTR) &lpMsgBuf,
		0,
		NULL 
	);
	TCHAR temp[MAX_PATH];
	wsprintf(temp, (LPTSTR)lpMsgBuf);
	// Process any inserts in lpMsgBuf.
	// ...
	// Display the string.
	DEBUGMSG(1, ((LPCTSTR)lpMsgBuf));
	// Free the buffer.
	LocalFree( lpMsgBuf );
}

int getCityDB()
{
    TZData *pTZ = NULL;
    int index;

    // load the library
    HINSTANCE hLib = LoadLibrary(_T("CityDB.dll"));
	if (hLib==NULL)
		return -1;
    // load the CityDB functions
    INITCITYDB InitCityDB = (INITCITYDB)GetProcAddress(
            hLib, _T("InitCityDb"));
    UNINITCITYDB UninitCityDB = (UNINITCITYDB)GetProcAddress(
            hLib, _T("UninitCityDb"));
    LOADTZDATA ClockLoadAllTimeZoneData = (LOADTZDATA)GetProcAddress(
            hLib, _T("ClockLoadAllTimeZoneData"));
    FREETZDATA ClockFreeAllTimeZoneData = (FREETZDATA)GetProcAddress(
            hLib, _T("ClockFreeAllTimeZoneData"));
    GETNUMZONES ClockGetNumTimezones = (GETNUMZONES)GetProcAddress(
            hLib, _T("ClockGetNumTimezones"));
    GETTZDATABYOFFSET ClockGetTimeZoneDataByOffset =
            (GETTZDATABYOFFSET)GetProcAddress(hLib, _T("ClockGetTimeZoneDataByOffset"));
    GETTZDATA ClockGetTimeZoneData = (GETTZDATA)GetProcAddress(
            hLib, _T("ClockGetTimeZoneData"));

    // Init the library
    InitCityDB();

    // load the TZ data
    ClockLoadAllTimeZoneData();

    // find out how many zones are defined
    int zoneCount = ClockGetNumTimezones();

	HANDLE hFile = CreateFile (TEXT("\\TZ-cities.TXT"),      // Open Two.txt.
					GENERIC_WRITE,          // Open for writing
					0,                      // Do not share
					NULL,                   // No security
					OPEN_ALWAYS,            // Open or create
					FILE_ATTRIBUTE_NORMAL,  // Normal file
					NULL);                  // No template file

	if (hFile == INVALID_HANDLE_VALUE)
	{
		//wsprintf (szMsg, TEXT("Could not open TWO.TXT"));
		CloseHandle (hFile);            // Close the first file.
		return -2;
	}
	char buff[4096];
	TCHAR wBuff[4096];
	DWORD numToWrit, numWritten;

	TIME_ZONE_INFORMATION tzi;
	memset(&tzi, 0, sizeof(tzi));
	int ret=GetTimeZoneInformation(&tzi);
		wsprintf(wBuff, L"===================TimeZoneInformation (API)==================\n");
		wcstombs(buff, wBuff, sizeof(wBuff)*sizeof(TCHAR));
		WriteFile(hFile, buff, strlen(buff), &numWritten, NULL);

		wsprintf(wBuff, L"Bias: %i\n", tzi.Bias );
		wcstombs(buff, wBuff, sizeof(wBuff)*sizeof(TCHAR));
		WriteFile(hFile, buff, strlen(buff), &numWritten, NULL);

		wsprintf(wBuff, L"DaylightBias: %i\n", tzi.DaylightBias  );
		wcstombs(buff, wBuff, sizeof(wBuff)*sizeof(TCHAR));
		WriteFile(hFile, buff, strlen(buff), &numWritten, NULL);
	
		wsprintf(wBuff, L"DaylightDate: %i\n", tzi.DaylightDate );
		wcstombs(buff, wBuff, sizeof(wBuff)*sizeof(TCHAR));
		WriteFile(hFile, buff, strlen(buff), &numWritten, NULL);
		
		wsprintf(wBuff, L"DaylightName: %s\n", tzi.DaylightName );
		wcstombs(buff, wBuff, sizeof(wBuff)*sizeof(TCHAR));
		WriteFile(hFile, buff, strlen(buff), &numWritten, NULL);

		wsprintf(wBuff, L"StandardBias: %i\n", tzi.StandardBias );
		wcstombs(buff, wBuff, sizeof(wBuff)*sizeof(TCHAR));
		WriteFile(hFile, buff, strlen(buff), &numWritten, NULL);
		
		wsprintf(wBuff, L"StandardBias: %i\n", tzi.StandardDate );
		wcstombs(buff, wBuff, sizeof(wBuff)*sizeof(TCHAR));
		WriteFile(hFile, buff, strlen(buff), &numWritten, NULL);

		wsprintf(wBuff, L"DaylightName: %s\n", tzi.StandardName);
		wcstombs(buff, wBuff, sizeof(wBuff)*sizeof(TCHAR));
		WriteFile(hFile, buff, strlen(buff), &numWritten, NULL);

	HKEY phKey;
	TIME_ZONE_INFORMATION *tzi2;
	//memset(&tzi2, 0, sizeof(TIME_ZONE_INFORMATION));
	byte *data;
	data = (byte *)malloc(4096);
	DWORD count = 4096;
	DWORD type; //=REG_BINARY;

	wsprintf(wBuff, L"===================TimeZoneInformation (reg)==================\n");
	wcstombs(buff, wBuff, sizeof(wBuff)*sizeof(TCHAR));
	WriteFile(hFile, buff, strlen(buff), &numWritten, NULL);

	if (RegOpenKeyEx(HKEY_LOCAL_MACHINE , L"TIME", 0, 0, &phKey) == ERROR_SUCCESS)
	{
		int ret = RegQueryValueEx(phKey, L"TimeZoneInformation", NULL, &type, data, &count);
		if (ret == ERROR_SUCCESS)
		{
			//TIME_ZONE_INFORMATION 
			tzi2 = (TIME_ZONE_INFORMATION *) data;
			
			wsprintf(wBuff, L"Bias: %i\n", tzi2->Bias );
			wcstombs(buff, wBuff, sizeof(wBuff)*sizeof(TCHAR));
			WriteFile(hFile, buff, strlen(buff), &numWritten, NULL);
			
			wsprintf(wBuff, L"DaylightBias: %i\n", tzi2->DaylightBias);
			wcstombs(buff, wBuff, sizeof(wBuff)*sizeof(TCHAR));
			WriteFile(hFile, buff, strlen(buff), &numWritten, NULL);
			
			wsprintf(wBuff, L"DaylightDate: %i\n", tzi2->DaylightDate);
			wcstombs(buff, wBuff, sizeof(wBuff)*sizeof(TCHAR));
			WriteFile(hFile, buff, strlen(buff), &numWritten, NULL);
			
			wsprintf(wBuff, L"DaylightName: %s\n", tzi2->DaylightName);
			wcstombs(buff, wBuff, sizeof(wBuff)*sizeof(TCHAR));
			WriteFile(hFile, buff, strlen(buff), &numWritten, NULL);
			
			wsprintf(wBuff, L"StandardBias: %i\n", tzi2->StandardBias);
			wcstombs(buff, wBuff, sizeof(wBuff)*sizeof(TCHAR));
			WriteFile(hFile, buff, strlen(buff), &numWritten, NULL);
			
			wsprintf(wBuff, L"StandardDate: %i\n", tzi2->StandardDate);
			wcstombs(buff, wBuff, sizeof(wBuff)*sizeof(TCHAR));
			WriteFile(hFile, buff, strlen(buff), &numWritten, NULL);
			
			wsprintf(wBuff, L"StandardName: %i\n", tzi2->StandardName);
			wcstombs(buff, wBuff, sizeof(wBuff)*sizeof(TCHAR));
			WriteFile(hFile, buff, strlen(buff), &numWritten, NULL);
		}
		else
			ShowError(ret);
	}
	free (data);

	wsprintf(wBuff, L"=================CityDB====================\n");
	wcstombs(buff, wBuff, sizeof(wBuff)*sizeof(TCHAR));
	WriteFile(hFile, buff, strlen(buff), &numWritten, NULL);
    // interate through them all
    for(int zone = 0 ; zone < zoneCount ; zone++)
    {
        // these are pointers to a timezone data struct
        pTZ = (TZData*)ClockGetTimeZoneDataByOffset(zone, &index);

		wsprintf(wBuff, L"index: %i\n", index );
		wcstombs(buff, wBuff, sizeof(wBuff)*sizeof(TCHAR));
		WriteFile(hFile, buff, strlen(buff), &numWritten, NULL);

		wsprintf(wBuff, L"\tshort name: %s\n", pTZ->ShortName  );
		wcstombs(buff, wBuff, sizeof(wBuff)*sizeof(TCHAR));
		WriteFile(hFile, buff, strlen(buff), &numWritten, NULL);

		wsprintf(wBuff, L"\tname: %s\n", pTZ->Name );
		wcstombs(buff, wBuff, sizeof(wBuff)*sizeof(TCHAR));
		WriteFile(hFile, buff, strlen(buff), &numWritten, NULL);

		wsprintf(wBuff, L"\tGMT offset: %i\n", pTZ->GMTOffset  );
		wcstombs(buff, wBuff, sizeof(wBuff)*sizeof(TCHAR));
		WriteFile(hFile, buff, strlen(buff), &numWritten, NULL);

		wsprintf(wBuff, L"\tdst name: %s\n", pTZ->DSTName  );
		wcstombs(buff, wBuff, sizeof(wBuff)*sizeof(TCHAR));
		WriteFile(hFile, buff, strlen(buff), &numWritten, NULL);

		wsprintf(wBuff, L"\tDST offset: %i\n", pTZ->DSTOffset );
		wcstombs(buff, wBuff, sizeof(wBuff)*sizeof(TCHAR));
		WriteFile(hFile, buff, strlen(buff), &numWritten, NULL);
    }
	CloseHandle(hFile);

    // unload the TZ data
    ClockFreeAllTimeZoneData();

    // uninit the library
    UninitCityDB();

    return 0;
}

int WINAPI WinMain(	HINSTANCE hInstance,
					HINSTANCE hPrevInstance,
					LPTSTR    lpCmdLine,
					int       nCmdShow)
{
 	// TODO: Place code here.

	byte lSize=sizeof(LONG);

	getCityDB();
	TIME_ZONE_INFORMATION tzi;
	memset(&tzi, 0, sizeof(tzi));

	int ret=GetTimeZoneInformation(&tzi);
	switch (ret)
	{
		case TIME_ZONE_ID_UNKNOWN:
			break;
		case TIME_ZONE_ID_STANDARD:
			break;
		case TIME_ZONE_ID_DAYLIGHT:
			break;
		default:
			//error
			break;
	}

	//The bias is the difference, in minutes, between UTC and local time
	//so to get GMT+1 you have to specify -1 atoi
	int hourOffset = 0;
	int i=0;

	if (wcslen(lpCmdLine)>0)
	{
		i = _ttoi(lpCmdLine);
		if (i!=0)
			hourOffset=i;
	}

	TCHAR s[32];
	tzi.Bias = hourOffset*60;

	wsprintf(s, L"GMT DST");
	wcscpy(tzi.DaylightName, L"GMT DST");

	wsprintf(s, L"GMT");
	wcscpy(tzi.StandardName, L"GMT");

	ret=SetTimeZoneInformation(&tzi);
	if (ret==0)
		return ret; //error

	//TZREG
	return 0;
}

