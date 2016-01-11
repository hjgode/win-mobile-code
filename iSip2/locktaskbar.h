//locktaskbar.h

#include "tlhelp32.h"
#pragma comment(lib, "toolhelp.lib")

//#define WIN32_PLATFORM_PSPC
/*
#if defined(WIN32_PLATFORM_PSPC)
	#define PPC
	#include "aygshell.h"
#pragma comment (lib, "aygshell.lib")
#else
	#undef PPC
	#include "sipapi.h"
#endif
*/
	#include "sipapi.h"

long LockTaskbar(bool lockIt);
long GetLockState(bool *state);
long LockDesktop(bool lockIt);
long SetTopWindow(long hwnd);
BOOL ShowSIP(bool show);

static BOOL CALLBACK EnumWindowsProc(HWND hwnd, LPARAM lParam);
bool KillExeWindow(TCHAR* exefile);

int ExistFile(TCHAR* filename);

DWORD FindPID(TCHAR exename);

HWND hWindow=NULL;

bool foundIt=false;
bool killedIt=false;

DWORD dwPID=0;

PROCESS_INFORMATION procInfo;
/*
procInfo has these members
    HANDLE hProcess;   // process handle
    HANDLE hThread;    // primary thread handle
    DWORD dwProcessId; // process PID
    DWORD dwThreadId;  // thread ID
*/

int ExistFile(TCHAR* filename)
{
	HANDLE hFile;

	hFile = CreateFile (filename,   // Open MYFILE.TXT
					  GENERIC_READ,           // Open for reading
					  FILE_SHARE_READ,        // Share for reading
					  NULL,                   // No security
					  OPEN_EXISTING,          // Existing file only
					  FILE_ATTRIBUTE_NORMAL,  // Normal file
					  NULL);                  // No template file

	if (hFile == INVALID_HANDLE_VALUE)
	{
		// Your error-handling code goes here.
		return -1;
	}
	else
	{
		CloseHandle(hFile);
		return 0;
	}
}
//
// FindPID will return ProcessID for an ExeName
// retruns 0 if no window has a process created by exename
//
DWORD FindPID(TCHAR* exename)
{
	  //No make a snapshot for all processes and find the matching processID
  HANDLE hSnap = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
  if (hSnap != NULL)
  {
	  PROCESSENTRY32 pe;
	  pe.dwSize = sizeof(PROCESSENTRY32);
	  if (Process32First(hSnap, &pe))
	  {
		do
		{
		  if (wcscmp (_wcslwr(pe.szExeFile), exename) == 0)
		  {
			CloseToolhelp32Snapshot(hSnap);
			dwPID=pe.th32ProcessID ;
			return dwPID;
		}
		} while (Process32Next(hSnap, &pe));
	  }//processFirst
  }//hSnap
  CloseToolhelp32Snapshot(hSnap);
  return 0;

}

BOOL CALLBACK EnumWindowsProc(HWND hwnd, LPARAM lParam) //find window for PID
{
	DWORD wndPid;
//	TCHAR Title[128];
	// lParam = procInfo.dwProcessId;
	// lParam = exename;

	// This gets the windows handle and pid of enumerated window.
	GetWindowThreadProcessId(hwnd, &wndPid);

	// This gets the windows title text
	// from the window, using the window handle
	//  GetWindowText(hwnd, Title, sizeof(Title)/sizeof(Title[0]));
	if (wndPid == (DWORD) lParam)
	{
		//found matching PID
		foundIt=true;
		hWindow=hwnd;
		return false; //stop EnumWindowsProc
	}
	return true;
}

bool KillExeWindow(TCHAR* exefile)
{
	//go thru all top level windows
	//get ProcessInformation for every window
	//compare szExeName to exefile
	// upper case conversion?
	foundIt=false;
	killedIt=false;
	//first find a processID for the exename
	dwPID = FindPID(exefile);
	if (dwPID !=0)
	{
		//now find the handle for the window that was created by the exe via the processID
		EnumWindows(EnumWindowsProc, (LPARAM) dwPID);
		if (foundIt)
		{
			//now try to close the window
			if (PostMessage(hWindow, WM_QUIT, 0, 0) == 0) //did not success?
			{
				//try the hard way
				HANDLE hProcess = OpenProcess(0, FALSE, dwPID);
				if (hProcess != NULL)
				{
					DWORD uExitCode=0;
					if ( TerminateProcess (hProcess, uExitCode) != 0)
					{
						//app terminated
						killedIt=true;
					}
				}

			}
			else
				killedIt=true;
		}
		else
		{
			//no window
			//try the hard way
			HANDLE hProcess = OpenProcess(0, FALSE, dwPID);
			if (hProcess != NULL)
			{
				DWORD uExitCode=0;
				if ( TerminateProcess (hProcess, uExitCode) != 0)
				{
					//app terminated
					killedIt=true;
				}
			}
		}
	}
	return killedIt;
}

long SetTopWindow(HWND hwnd)
{
	return (SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_SHOWWINDOW));
}

long LockDesktop(bool lockIt)
{
	HWND hWndDesk;
	HRESULT hr;
	if (lockIt)
	{
		hWndDesk = GetDesktopWindow();
		if (hWndDesk != NULL)
		{
			hr = EnableWindow(hWndDesk, false);
			UpdateWindow(hWndDesk);
			return 0;
		}
	}
	else
	{
		hWndDesk = GetDesktopWindow();
		if (hWndDesk != NULL)
		{
			hr = EnableWindow(hWndDesk, true);
			UpdateWindow(hWndDesk);
			return 0;
		}
	}
	return -1;
}

long LockTaskbar(bool lockIt)
{
	HWND hWndTb;
	HRESULT hr;
	bool enable;
	enable = !lockIt;
	hWndTb = FindWindow(L"HHTASKBAR", NULL);
	if (hWndTb != NULL)
	{
		
		hr = EnableWindow(hWndTb, enable);
		UpdateWindow(hWndTb);
		return 0; // no error
	}
	else
		return -1; //error window not found
}

long GetLockState(bool *state)
{
	HWND hWndTb;
	LONG lr;
	bool enable;
	enable = ! *state;
	hWndTb = FindWindow(L"HHTASKBAR", NULL);
	if (hWndTb != NULL)
	{
		lr = GetWindowLong (hWndTb, GWL_STYLE);
		if (lr == 0)
			return -2;
		if (lr & WS_DISABLED) //locked
			*state = true; 
		else //unlocked
			*state = false;
		return 0;
	}
	return -1;
}

BOOL ShowSIP(bool show)
{
	int iRet;
	if (show){
		//show the sip
		iRet=SipShowIM(SIPF_ON);		
		return (iRet);
	}
	else{
		iRet = SipShowIM(SIPF_OFF);
		return (iRet);
	}
}

int ShowSIPxxx(bool show)
{
	int iRes=1;	//no error
	HWND hWndSIP=NULL;
	hWndSIP=FindWindow(L"SipWndClass", NULL);
	HWND hWndSipButtonb=NULL;
	hWndSipButtonb=FindWindow(L"MS_SIPBUTTON",NULL);
	BOOL bSipUp=FALSE;

	if(hWndSIP!=INVALID_HANDLE_VALUE){
		//check window state
		RECT rect;
		GetWindowRect(hWndSIP, &rect);
		if(rect.top>26){
			//sip is up
			bSipUp=TRUE;
		}
		else
		{
			//SIP is down
			bSipUp=FALSE;
		}
		if(show && !bSipUp){
		//if(show){
			iRes = ShowWindow(hWndSIP, SW_SHOW);
			ShowWindow(hWndSipButtonb, SW_SHOW);
		}
		else
		{
			iRes = ShowWindow(hWndSIP, SW_HIDE);
			ShowWindow(hWndSipButtonb, SW_HIDE);
		}
	}
	else
		iRes=-5;
/*	if(show){
		iRes=SHSipPreference(GetActiveWindow(),SIP_UP);
		iRes=SHSipPreference(GetActiveWindow(),SIP_UNCHANGED);
	}
	else
	{
		iRes=SHSipPreference(GetActiveWindow(),SIP_DOWN);
		iRes=SHSipPreference(GetActiveWindow(),SIP_UNCHANGED);
	}
*/
	return iRes;
}