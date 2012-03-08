//locktaskbar.h

#include "tlhelp32.h"
#pragma comment(lib, "toolhelp.lib")

#if defined(WIN32_PLATFORM_PSPC)
	#define PPC
	#include "aygshell.h"
#else
	#undef PPC
	#include "sipapi.h"
#endif

//==========================================================================================
// function definitions
//==========================================================================================
long LockTaskbar(bool lockIt);			//lock/unlock the taskbar
long GetLockState(bool *state);			//
long LockDesktop(bool lockIt);			//lock/unlock the desktop window
long SetTopWindow(long hwnd);			//set window to top
BOOL ShowSIP(bool show);				//show/hide the sip, is PPC WinCE aware!

void DoReset(void);		//perform a reset, either with __resetmeplease__.txt or the hard way (kernelIOctl)
BOOL IsProcessRunning( TCHAR *pname );  //check to see if process pname is running

static BOOL CALLBACK EnumWindowsProc(HWND hwnd, LPARAM lParam);		//helper for findwindow
bool KillExeWindow(TCHAR* exefile);		//will kill process

int ExistFile(TCHAR* filename);			//test, if file exists

DWORD FindPID(TCHAR *exename);			//helper to find process ID for exename

//recursive delete directory
BOOL DeleteDirectory(LPCTSTR pFolder);
BOOL DelFile(LPCTSTR pFile);

// for warmboot
#include "winioctl.h"
extern "C" __declspec(dllimport) BOOL KernelIoControl(DWORD dwIoControlCode, LPVOID lpInBuf, DWORD nInBufSize, LPVOID lpOutBuf, DWORD nOutBufSize, LPDWORD lpBytesReturned);
#define IOCTL_HAL_REBOOT CTL_CODE(FILE_DEVICE_HAL, 15, METHOD_BUFFERED, FILE_ANY_ACCESS)
void DoReset(void);		//perform a reset, either with __resetmeplease__.txt or the hard way (kernelIOctl)

//==========================================================================================
// global variables etc
//==========================================================================================
HWND hWindow=NULL;

bool foundIt=false;
bool killedIt=false;

DWORD dwPID=0;

PROCESS_INFORMATION procInfo;

//==========================================================================================
// function implementations
//==========================================================================================
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
		return FALSE;
	}
	else
	{
		CloseHandle(hFile);
		return TRUE;
	}
}
//
// FindPID will return ProcessID for an ExeName
// retruns 0 if no window has a process created by exename
//
DWORD FindPID(TCHAR *exename)
{
	TCHAR exe[MAX_PATH];
	wsprintf(exe, L"%s", exename);
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
		  if (wcsicmp (pe.szExeFile, exe) == 0)
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
	if (dwPID != 0)
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
				else
					killedIt=false;
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
	if (show)
		//show the sip
		return (SipShowIM(SIPF_ON));
	else
		return (SipShowIM(SIPF_OFF));
}

//===========================================================================================
BOOL ResetPocketPC()
{
	MessageBox(GetDesktopWindow(), L"The device needs to be reset for changes becoming active", L"iLock Setup", MB_OK | MB_ICONSTOP | MB_APPLMODAL | MB_SETFOREGROUND | MB_TOPMOST);
	return KernelIoControl(IOCTL_HAL_REBOOT, NULL, 0, NULL, 0, NULL);
}

/*$DOCBEGIN$
 =======================================================================================================================
 *    £
 *    BOOL IsProcessRunning( TCHAR * pname ); £
 *    * Description: Get process table snapshot, look for pname running. £
 *    * Arguments: pname - pointer to name of program to look for. £
 *    for example, app.exe. £
 *    * Returns: TRUE - process is running. £
 *    FALSE - process is not running. £
 *    $DOCEND$ £
 *
 =======================================================================================================================
 */

BOOL IsProcessRunning( TCHAR *pname )
{
    HANDLE          hProcList;
    PROCESSENTRY32  peProcess;
    DWORD           thDeviceProcessID;
    TCHAR           lpname[MAX_PATH];
    if ( !pname || !*pname )
    {
        return FALSE;
    }

    wcscpy( lpname, pname );
    _tcslwr( lpname );
    hProcList = CreateToolhelp32Snapshot( TH32CS_SNAPPROCESS, 0 );
    if ( hProcList == INVALID_HANDLE_VALUE )
    {
        return FALSE;
    }       /* end if */

    memset( &peProcess, 0, sizeof(peProcess) );
    peProcess.dwSize = sizeof( peProcess );
    if ( !Process32First( hProcList, &peProcess ) )
    {
        CloseToolhelp32Snapshot( hProcList );
        return FALSE;
    }       /* end if */

    thDeviceProcessID = 0;
    do
    {
        //_wcslwr( peProcess.szExeFile );
        if ( wcsicmp( peProcess.szExeFile, lpname ) == 0) //replaced wcsstr by wcsicmp
        {
            thDeviceProcessID = peProcess.th32ProcessID;
            break;
        }   /* end if */
    }
    while ( Process32Next( hProcList, &peProcess ) );
    if ( (GetLastError() == ERROR_NO_MORE_FILES) && (thDeviceProcessID == 0) )
    {
        CloseToolhelp32Snapshot( hProcList );
        return FALSE;
    }       /* end if */

    CloseToolhelp32Snapshot( hProcList );
    return TRUE;
}           /* IsProcessRunning */

//========================================================================================================
void DoReset(void)
{
    HANDLE  h;
    //TCHAR   srcfile[MAX_PATH];
    //TCHAR   dstfile[MAX_PATH];
    if ( IsProcessRunning( L"autocab.exe" ) )
    {
        h = CreateFile
            (
                L"\\Windows\\__resetmeplease__.txt",
                (GENERIC_READ | GENERIC_WRITE),
                0,
                NULL,
                CREATE_ALWAYS,
                FILE_ATTRIBUTE_HIDDEN,
                NULL
            );
        if ( h != INVALID_HANDLE_VALUE )
        {
            CloseHandle( h );
        }
        else
        {
            /* Couldn’t create the file. If it failed because the file already exists, it is not fatal.*/
            /* Otherwise, notify user of the inability to reset the device and they will have to*/
            /* perform it manually after all of the installations are complete. */
			/* Force a warm start NOW. */
			ResetPocketPC();            /* Won’t return, but we’ll show clean up anyway */
        }   /* end if */
    }
    else
    {
        /* Force a warm start NOW. */
		ResetPocketPC();            /* Won’t return, but we’ll show clean up anyway */
    }   /* end if */
}

//================================================================================================
void GetLastErrorText(LONG er, TCHAR *text)
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
	// Process any inserts in lpMsgBuf.
	// ...
	// Display the string.
	wsprintf(text, L"%s", (LPCTSTR)lpMsgBuf);
	//MessageBox( NULL, (LPCTSTR)lpMsgBuf, L"Error", MB_OK | MB_ICONINFORMATION | MB_TOPMOST | MB_SETFOREGROUND);
	// Free the buffer.
	LocalFree( lpMsgBuf );
}

//================================================================================================
BOOL DeleteDirectory(LPCTSTR pFolder)
{
	TCHAR szFullPathFileName[MAX_PATH];
	TCHAR szFilename[MAX_PATH];
	TCHAR sText[MAX_PATH];
	#ifdef useLogging
	wsprintf(sText, L"Deleting Directory '%s'...\n", pFolder);
	Add2Log(sText, FALSE);
	#endif

	if (!RemoveDirectory(pFolder))
	{
		DWORD err = GetLastError();
		if (err != ERROR_DIR_NOT_EMPTY)
		{
			#ifdef useLogging
				wsprintf(sText, L"Error '0x%0x' in RemoveDirectory '%s': \t", err, pFolder);
				Add2Log(sText, FALSE);
				GetLastErrorText(err, sText);
				Add2Log(sText, FALSE);
				Add2Log(L"\n",false);
			#endif

			return FALSE;
		}
		#ifdef useLogging
		wsprintf(sText, L"Error in RemoveDirectory '%s'\n", pFolder);
		Add2Log(sText, FALSE);
		#endif	
	}
	else
	{
		#ifdef useLogging
		wsprintf(sText, L"Removed directory: '%s'\n", pFolder);
		Add2Log(sText, FALSE);
		#endif	
	}

	
	// remove sub folders and files.

	WIN32_FIND_DATA FileData = {0};
	BOOL bFinished = FALSE; 
	DWORD dwSize = 0;

	_stprintf(szFullPathFileName, TEXT("%s\\*.*"), pFolder);
	HANDLE hSearch = FindFirstFile(szFullPathFileName, &FileData); 
	if (hSearch == INVALID_HANDLE_VALUE) 
	{
		wsprintf(sText, L"FindFirstFile: INVALID_HANDLE_VALUE for '%s' not found\n", szFullPathFileName);
		Add2Log(sText, FALSE);
		return 0;
	}
	while (!bFinished)
	{
		_stprintf(szFilename, TEXT("%s\\%s"),pFolder,FileData.cFileName);
		if (FileData.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY)
		{
			if (_tcscmp(FileData.cFileName,TEXT(".")) && _tcscmp(FileData.cFileName,TEXT("..")))
			{
				DeleteDirectory(szFilename);
				if (RemoveDirectory(szFilename))
				{
					wsprintf(sText, L"Removed directory: '%s'\n", szFilename);
					Add2Log(sText, FALSE);
				}
				else
				{
					wsprintf(sText, L"Error in RemoveDirectory '%s'\n", szFilename);
					Add2Log(sText, FALSE);
				}
			}
		}
		else
		{
			if (FileData.dwFileAttributes & FILE_ATTRIBUTE_READONLY)
				SetFileAttributes(szFilename, FILE_ATTRIBUTE_NORMAL);

			if (!DeleteFile(szFilename))
			{
				wsprintf(sText, L"Error in DeleteFile '%s'\n", szFilename);
				Add2Log(sText, FALSE);
				FindClose(hSearch);
				return FALSE;
			}
			else
			{
				wsprintf(sText, L"Deleted file '%s'\n", szFilename);
				Add2Log(sText, FALSE);
			}

		}
		if (!FindNextFile(hSearch, &FileData)) 
		{
			if (GetLastError() == ERROR_NO_MORE_FILES) 
				bFinished = TRUE;
		} 
	}
	FindClose(hSearch);

	// Here the directory is empty.
	if (RemoveDirectory(pFolder))
	{
		wsprintf(sText, L"Removed directory: '%s'\n", pFolder);
		Add2Log(sText, FALSE);
	}
	else
	{
		wsprintf(sText, L"Error in RemoveDirectory '%s'\n", pFolder);
		Add2Log(sText, FALSE);
	}
	return TRUE;
}

BOOL DelFile(LPCTSTR pFile)
{
	if (GetFileAttributes(pFile) != FILE_ATTRIBUTE_READONLY)
		SetFileAttributes(pFile, FILE_ATTRIBUTE_NORMAL);
	return (DeleteFile(pFile));
}