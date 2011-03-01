// PingAlertScheduler.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "Notify.h"
#include "common\nclog.h"

const __int64 nano100SecInDay=(__int64)10000000*60*60*24;
const __int64 nano10Minutes=(__int64)10000000*60*10;

TCHAR str[MAX_PATH+1];

//name of the external executable to start
TCHAR szExtApp[] = L"\\Windows\\PingAlert.exe";
TCHAR szExtArg[] = L"pingsweep";
TCHAR szScheduleArg[] = L"PingAlert";
int iScheduleInterval = 15;
TCHAR* szSubKey = L"Software\\PingAlert\\";

////////////
//equivalent of DATEADD function from SQLServer
//Returns a new datetime value based on adding an interval
// to the specified date.
////////////*/
SYSTEMTIME /*new datetime*/
DT_AddDiff
			(	const __int64 datepart, /*datepart with we want to manipulate, 
			{nano100SecInDay ...}*/
			const __int64 num, /*value used to increment/decrement datepart*/
			const SYSTEMTIME* pst /*valid datetime which we want change*/
			)
{
	FILETIME ft;
	SYSTEMTIME st;
	__int64* pi; 

	SystemTimeToFileTime (pst,&ft); 
	pi = (__int64*)&ft; 
	(*pi) += (__int64)num*datepart; 

	/*convert FAILETIME to SYSTEMTIME*/
	FileTimeToSystemTime (&ft,&st); 

	/*now, st contain new valid datetime, so return it*/
	return st;
}

SYSTEMTIME AddDiff(SYSTEMTIME* pst, int minutes){
	FILETIME ft;
	SYSTEMTIME st;
	__int64* pi; 
	//LARGE_INTEGER li;

	/*convert SYSTEMTIME to FILETIME*/
	SystemTimeToFileTime (pst,&ft); 

	SystemTimeToFileTime (pst,&ft); 

	pi = (__int64*)&ft; 
	(*pi) += (__int64)minutes*(__int64)10000000*60; 
    
	//li.LowPart = ft.dwLowDateTime;
    //li.HighPart = ft.dwHighDateTime;

	/*convert FAILETIME to SYSTEMTIME*/
	FileTimeToSystemTime (&ft,&st); 

	/*now, st contain new valid datetime, so return it*/
	return st;

}

static HRESULT ScheduleRunApp(
  LPCTSTR szExeName,
  LPCTSTR szArgs)
{
	//do not add a schedule if actual date is 21.3.2003
	SYSTEMTIME t;
	memset(&t, 0, sizeof(SYSTEMTIME));
	GetLocalTime(&t);
	//check if the system clock is at factory default, device specific!
	if ( (t.wYear == 2003) && (t.wMonth == 3) && (t.wDay == 21) )
	{
		nclog(L"PingAlertScheduler: # no next run schedule as date is 21.03.2003!\n");
		return NOERROR;
	}

	

	HRESULT hr = S_OK;
	HANDLE hNotify = NULL;

	// set a CE_NOTIFICATION_TRIGGER
	CE_NOTIFICATION_TRIGGER notifTrigger;
	memset(&notifTrigger, 0, sizeof(CE_NOTIFICATION_TRIGGER));
	notifTrigger.dwSize = sizeof(CE_NOTIFICATION_TRIGGER);

	// calculate time
	SYSTEMTIME st = {0};
	GetLocalTime(&st);

	/*
	st = DT_AddDiff(nano100SecInDay, 1, &st);
	st.wHour = 3;
	st.wMinute=0;
	st.wSecond=33;
	*/
	st = AddDiff(&st, iScheduleInterval); //wake in x minutes
	wsprintf(str, L"Next run at: %02i.%02i.%02i %02i:%02i:%02i\n", 
										st.wDay, st.wMonth , st.wYear, 
										st.wHour , st.wMinute , st.wSecond );
	nclog(L"PingAlertScheduler: %s\n", str);
	
	notifTrigger.dwType = CNT_TIME;
	notifTrigger.stStartTime = st;

	// timer: execute an exe at specified time
	notifTrigger.lpszApplication = (LPTSTR)szExeName;
	notifTrigger.lpszArguments = (LPTSTR)szArgs;

	hNotify = CeSetUserNotificationEx(0, &notifTrigger, NULL);
	// NULL because we do not care the action
	if (!hNotify) {
		hr = E_FAIL;
		nclog(L"PingAlertScheduler: CeSetUserNotificationEx FAILED...\n");
	} else {
		// close the handle as we do not need to use it further
		CloseHandle(hNotify);
		nclog(L"PingAlertScheduler: CeSetUserNotificationEx succeeded...\n");
	}  
	return hr;
} 

//for debug use
static HRESULT ListNotiApps()
{
	HRESULT hr = S_OK;

	// hold a notification
	PBYTE pBuff = (PBYTE)LocalAlloc(LPTR, 8192);

	if (!pBuff) {
		return E_OUTOFMEMORY;
	}

	// at most 256 notification handles
	HANDLE hNotifHandlers[256];
	DWORD nNumHandlers, nNumClearedHandlers = 0;
	DWORD i = 0;
	int rc = CeGetUserNotificationHandles(hNotifHandlers, 255, &nNumHandlers);
	if (!rc) {
		ULONG uErr = GetLastError();
		hr = E_FAIL;
		nclog(L"PingAlertScheduler: no more handles? in CeGetUserNotificationHandles()? GetLastError()=%u\n", uErr);
		goto FuncExit;
	}
  
	// iterate all notifications
	// Notice: We do not care about the status of the notification.
	// Just clear it even if it is not filed??
	nclog(L"PingAlertScheduler: ########## ListNotiApps() ############\n");
	for (; i<nNumHandlers; i++) {
		// query info for this specific handler
		BOOL bClearThis = FALSE;
		DWORD dwSize = 0;
		rc = CeGetUserNotification(
		hNotifHandlers[i], 8192, &dwSize, pBuff);
		if (!rc) continue;

		PCE_NOTIFICATION_INFO_HEADER pnih = (PCE_NOTIFICATION_INFO_HEADER)pBuff;

		PCE_NOTIFICATION_TRIGGER pNotifTrigger = pnih->pcent;


		// Notice some events with NULL lpszApplication might be inserted!
		if ( pNotifTrigger && pNotifTrigger->lpszApplication )
		{
			nclog(L"PingAlertScheduler: %s\n", pNotifTrigger->lpszApplication);
			if (pNotifTrigger->lpszApplication != NULL){
				nclog(L"PingAlertScheduler-ListNotiApps: %s\n", pNotifTrigger->lpszApplication);
			}
			else
			{
				nclog(L"PingAlertScheduler: NULL app\n");
			}
			nclog(L"\n");
		}
	}
  
	FuncExit:
	nclog(L"PingAlertScheduler: ########## END of ListNotiApps()############\n");
	if (pBuff) {
		LocalFree(pBuff);
	}

	return hr;
}

//Clear all schedules registered for szExeName
static HRESULT ClearRunApp(LPCTSTR szExeName)
{
	HRESULT hr = S_OK;

	// hold a notification
	PBYTE pBuff = (PBYTE)LocalAlloc(LPTR, 8192);

	if (!pBuff) {
		return E_OUTOFMEMORY;
	}

	TCHAR mExeName[MAX_PATH];
	wsprintf(mExeName, L"%s", szExeName);

	// at most 256 notification handles
	HANDLE hNotifHandlers[256];
	DWORD nNumHandlers, nNumClearedHandlers = 0;
	DWORD i = 0;
	int rc = CeGetUserNotificationHandles(hNotifHandlers, 255, &nNumHandlers);
	if (!rc) {
		ULONG uErr = GetLastError();
		hr = E_FAIL;
		nclog(L"no more handles? in CeGetUserNotificationHandles()? GetLastError()=%u\n", uErr);
		goto FuncExit;
	}
  
	// iterate all notifications
	// Notice: We do not care about the status of the notification.
	// Just clear it even if it is not filed??
	nclog(L"PingAlertScheduler, ClearRunApp(): %s", L"######################\n");
	for (i=0; i<nNumHandlers; i++) {
		// query info for this specific handler
		BOOL bClearThis = FALSE;
		DWORD dwSize = 0;
		//get size for buffer first?
		rc = CeGetUserNotification(hNotifHandlers[i], 8192, &dwSize, pBuff);
		if (!rc) continue;

		PCE_NOTIFICATION_INFO_HEADER pnih = (PCE_NOTIFICATION_INFO_HEADER)pBuff;

		PCE_NOTIFICATION_TRIGGER pNotifTrigger = pnih->pcent;

		//nclog(L"PingAlertScheduler, ClearRunApp(): %s\n", pNotifTrigger->lpszApplication);

		// Notice some events with NULL lpszApplication might be inserted!
		if ( pNotifTrigger && pNotifTrigger->lpszApplication ){
			if(pNotifTrigger->lpszApplication != NULL){
				if (wcsicmp(pNotifTrigger->lpszApplication, mExeName)==0) {
					nclog(L"PingAlertScheduler, ClearRunApp()-CeClearUserNotification for handle: 0x%0x\n", pnih->hNotification);
					CeClearUserNotification(pnih->hNotification);
				}
			}
		}
	}
  
	FuncExit:
	nclog(L"##### PingAlertScheduler, ClearRunApp():FuncExit ############\n");
	if (pBuff) {
		LocalFree(pBuff);
	}

	return hr;
}

/*
void ClearRunAppAtTime(TCHAR FileName[MAX_PATH+1])
{
	CeRunAppAtTime(FileName, 0);
}
*/
int RunAppAtTime(TCHAR FileName[MAX_PATH+1])
{
	//get actual time
	SYSTEMTIME t;
	memset(&t, 0, sizeof(SYSTEMTIME));
	GetLocalTime(&t);

	SYSTEMTIME newTime;
	//add one day
	newTime = DT_AddDiff(nano100SecInDay, 1, &t);

	newTime.wHour = 0;
	newTime.wMinute = 59;
	newTime.wSecond = 59;

	wsprintf(str, L"Next run at: %02i.%02i.%02i %02i:%02i:%02i", 
										newTime.wDay, newTime.wMonth , newTime.wYear, 
										newTime.wHour , newTime.wMinute , newTime.wSecond );
	nclog(L"PingAlertScheduler, RunAppAtTime(): %s", str);

	return CeRunAppAtTime(FileName, &t);

}

//read schedule interval from registry
int getTimeInterval(){
	HKEY hKey;
	HRESULT hRes = RegOpenKeyEx(HKEY_LOCAL_MACHINE, szSubKey, 0, 0, &hKey);
	if(TRUE){//hRes==ERROR_SUCCESS){
		DWORD dwDisposition; // will get REG_CREATED_NEW_KEY if key is new
		hRes = RegCreateKeyEx(HKEY_LOCAL_MACHINE, szSubKey, 0, NULL, REG_OPTION_NON_VOLATILE, 0, NULL, &hKey, &dwDisposition);
		DWORD dwType = REG_DWORD;
		DWORD dwVal = 15;
		DWORD dwSize = sizeof(DWORD);
		if(dwDisposition==REG_CREATED_NEW_KEY){
			//if the key is new, create a new DWORD val fo TimeInterval
			RegSetValueEx(hKey, L"TimeInterval", 0, dwType, (LPBYTE)&dwVal, dwSize);
		}
		hRes = RegQueryValueEx(hKey, L"TimeInterval", 0, &dwType, (LPBYTE) &dwVal, &dwSize);
		if(hRes==ERROR_SUCCESS){
			RegCloseKey(hKey);
			return dwVal;
		}
		RegCloseKey(hKey);
	}
	return -1;
}

int _tmain(int argc, _TCHAR* argv[])
{
	TCHAR lpCmdLine[1024]; //create a buffer large enough to hold cmd args
	int x=0;
	do{
		if(x>0)
			wcscat(lpCmdLine, L" "); //insert blank between args
		wcscat(lpCmdLine, argv[x]);
		x++;
	}while (x<argc);
	wcscat(lpCmdLine, L"\0"); //terminate string

	int iRet=0;
 	// Get name of executable
	TCHAR lpFileName[MAX_PATH+1];
	GetModuleFileName(NULL, lpFileName, MAX_PATH); //lpFileName will contain the exe name of this running app!

	nclog(L"\n+---------------------------------------+\n");
	nclog(L"|        PingAlertScheduler started     |\n");
	nclog(L"+---------------------------------------+\n");
	nclog(L"+   %s\n", logDateTime());
	nclog(L"+---------------------------------------+\n");

	nclog(L"PingAlertScheduler: WinMain\n");
	if(argc==2)
		nclog(L"PingAlertScheduler: Command line: '%s'\n", lpCmdLine);

	nclog(L"PingAlertScheduler: Clearing schedules for this app...\n");

	//the app needs only the first arg in lpCmdLine
	if(argc==2){
		wsprintf(lpCmdLine, argv[1]);
		nclog(L"lpCmdLine now '%s'\n", lpCmdLine);
	}

	//ListNotiApps();
	//clear schedule for this app
	ClearRunApp(lpFileName);
#ifdef DEBUG
	ListNotiApps();
#endif

	//read TimeInterval from Reg
	iScheduleInterval = getTimeInterval();
	if(iScheduleInterval==-1) // there was an error use default interval of 15
		iScheduleInterval=15;

	//launched without args
	//if launched from scheduler, lpCmdLine will have the value reboot as specified below
	if (argc == 1) //the one arg is the started executable itself
	{
		//create a new schedule
		if ( !FAILED(ScheduleRunApp(lpFileName, szScheduleArg)) )
		{
			nclog(L"PingAlertScheduler: ScheduleRunApp OK\n");
#ifdef DEBUG
			MessageBox(NULL, str, lpFileName, MB_TOPMOST | MB_SETFOREGROUND);
#endif
		}
		else{
			nclog(L"PingAlertScheduler: ScheduleRunApp FAILED\n");
#ifdef DEBUG
			MessageBox(NULL, L"error in RunAppAtTime", lpFileName, MB_TOPMOST | MB_SETFOREGROUND);
#endif
		}
	}

	//Quiet install
	if (_wcsicmp(L"quiet", lpCmdLine)==0)
	{
		nclog(L"PingAlertScheduler: processing CmdLine=quiet...\n");
		//create a new schedule
		if ( !FAILED(ScheduleRunApp(lpFileName, szScheduleArg)) ){
			iRet=0;
			goto MainExit; //OK
		}
		else{
			iRet=-1;
			goto MainExit; //OK
		}
	}

	//check if launched with 'clear'
	if (_wcsicmp(L"clear", lpCmdLine)==0)
	{
		nclog(L"PingAlertScheduler: processing CmdLine=clear...\n");
		ClearRunApp(lpFileName);
		nclog(L"PingAlertScheduler: Schedules cleared for '%s'\n", lpFileName);
#ifdef DEBUG
		MessageBox(NULL, L"Schedule for this exe cleared", lpFileName, MB_TOPMOST | MB_SETFOREGROUND);
#endif
		iRet=1;
		goto MainExit; //OK
	}

	//launched from scheduler with 'magic word'
	if (_wcsicmp(szScheduleArg, lpCmdLine)==0)
	{

		nclog(L"PingAlertScheduler: processing CmdLine=%s...\n", szScheduleArg);
		nclog(L"PingAlertScheduler: ...will wakeup again after reschedule...\n");
		//schedule next run
		if ( !FAILED(ScheduleRunApp(lpFileName, szScheduleArg)) )
		{
			//the worker application should be launched here!
			nclog(L"PingAlertScheduler: starting target app %s...\n", szExtApp);
			//is the target already running, then send WM_USER msg
			//else start a new instance with cmdLine="pingsweep"
			PROCESS_INFORMATION pi;
			if( CreateProcess(szExtApp,szExtArg,
					NULL,NULL,NULL, 0, NULL,NULL,NULL, 
					&pi)!=0)
			{
				nclog(L"PingAlertScheduler: CreateProcess for '%s' OK\n", szExtApp); 
				CloseHandle(pi.hThread);
				CloseHandle(pi.hProcess);
			}
			else
				nclog(L"PingAlertScheduler: CreateProcess for '%s' FAILED: %u\n", szExtApp, GetLastError());
			//inform target
			//iRet=signalEvent();
			goto MainExit;
		}
		else{
			MessageBox(NULL, L"error in ScheduleRunApp", lpFileName, MB_TOPMOST | MB_SETFOREGROUND);
			nclog(L"PingAlertScheduler: error in ScheduleRunApp\n");
			iRet=-2;
			goto MainExit; //OK
		}
	}
MainExit:
	nclog(L"\n+---------------------------------------+\n");
	nclog(L"|        PingAlertScheduler ended       |\n");
	nclog(L"+---------------------------------------+\n");
	nclog(L"+   return code=%i \n", iRet);
	return iRet;
}

