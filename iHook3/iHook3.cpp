// iHook3.cpp : Defines the entry point for the application.
//

//history
//version	change
// 1.0		initial release
// 1.1		added registry functions, now the keys are configured by the registry
/*
			[HKEY_LOCAL_MACHINE\SOFTWARE\Intermec\iHook3]
			"ForwardKey"=hex:0
			"arg0"=""
			"exe0"="\\windows\\iRotateCN2.exe"
			"key0"=hex:\
				  72
			"arg1"="-toggle"
			"exe1"="\\windows\\LockTaskBar.exe"
			"key1"=hex:\
				  73
			"arg2"=""
			"exe2"="\\windows\\iSIP2.exe"
			"key2"=hex:\
				  74
			"arg3"=""
			"exe3"="explorer.exe"
			"key3"=hex:\
				  71
			"arg4"="iRun2.exe"
			"exe4"="\\windows\\iKill2.exe"
			"key4"=hex:\
				  1b
*/
/*			would only load once
			added arg -writereg to write default registry
	1.2		added CloseHandle on CreateProcess(...pi)
	1.3		20.6.2005
			added function IsIntermec to registry.h and now checks, if this is an intermec
	2.0		changed isIntermec in registry.h for CK60, which does not support platform/name
			added notification code (needs WS_OVERLAPPED as WinStyle for Main Window!!!!)
	3.1.1	changed bForward handling: did not forward any key if false
			now only processed keys are not forwarded if bForward=false
	3.1.2	changed isIntermec to look for itcscan.dll only
	3.1.3	changed isIntermec check
			old: if (bDoCheckIntermec && IsIntermec() != 0)
			new: if (bDoCheckIntermec && (IsIntermec() != 0))
*/
//	ReportEvent

#include "stdafx.h"
#include "hooks.h"
#include "registry.h"
#include "resource.h"

#include "log2file.h"

TCHAR szAppName[] = L"iHook3v3.1.3";

LONG FAR PASCAL WndProc (HWND , UINT , UINT , LONG) ;
int ReadReg();
void WriteReg();

//global to hold keycodes and commands assigned
typedef struct {
	byte keyCode;
	TCHAR keyCmd[MAX_PATH+1];
	TCHAR keyArg[MAX_PATH+1];
} hookmap;

bool bForwardKey=false;
bool bDoCheckIntermec=TRUE;

static hookmap kMap[10];
int lastKey=-1;

	NOTIFYICONDATA nid;

// Global variables can still be your...friend.
// CIHookDlg* g_This			= NULL;			// Needed for this kludgy test app, allows callback to update the UI
HINSTANCE g_hInstance		= NULL;			// Handle to app calling the hook (me!)
HINSTANCE  g_hHookApiDLL	= NULL;			// Handle to loaded library (system DLL where the API is located)
//HHOOK g_hInstalledLLKBDhook = NULL;			// Handle to low-level keyboard hook

// Global functions: The original Open Source
BOOL g_HookDeactivate();
BOOL g_HookActivate(HINSTANCE hInstance);

//
void ShowIcon(HWND hWnd, HINSTANCE hInst);
void RemoveIcon(HWND hWnd);

#pragma data_seg(".HOOKDATA")									//	Shared data (memory) among all instances.
	HHOOK g_hInstalledLLKBDhook = NULL;						// Handle to low-level keyboard hook
	//HWND hWnd	= NULL;											// If in a DLL, handle to app window receiving WM_USER+n message
#pragma data_seg()

#pragma comment(linker, "/SECTION:.HOOKDATA,RWS")		//linker directive

// The command below tells the OS that this EXE has an export function so we can use the global hook without a DLL
__declspec(dllexport) LRESULT CALLBACK g_LLKeyboardHookCallback(
   int nCode,      // The hook code
   WPARAM wParam,  // The window message (WM_KEYUP, WM_KEYDOWN, etc.)
   LPARAM lParam   // A pointer to a struct with information about the pressed key
) 
{
	/*	typedef struct {
	    DWORD vkCode;
	    DWORD scanCode;
	    DWORD flags;
	    DWORD time;
	    ULONG_PTR dwExtraInfo;
	} KBDLLHOOKSTRUCT, *PKBDLLHOOKSTRUCT;*/
	
	// Get out of hooks ASAP; no modal dialogs or CPU-intensive processes!
	// UI code really should be elsewhere, but this is just a test/prototype app
	// In my limited testing, HC_ACTION is the only value nCode is ever set to in CE
	static int iActOn = HC_ACTION;
	PROCESS_INFORMATION pi;
	int i;
	bool processed_key=false;
	if (nCode == iActOn) 
	{ 
		PKBDLLHOOKSTRUCT pkbhData = (PKBDLLHOOKSTRUCT)lParam;
		if ( (wParam == WM_KEYUP) && (processed_key==false) )
		{
			Add2Log(L"# hook got 0x%02x (%i). Looking for match...\r\n", pkbhData->vkCode, pkbhData->vkCode);
			BOOL bMatchFound=FALSE;
			for (i=0; i<=lastKey; i++) 
			{
				if (pkbhData->vkCode == kMap[i].keyCode)
				{
					bMatchFound=TRUE;
					DEBUGMSG(1, (L"# hook Catched key 0x%0x, launching '%s'\n", kMap[i].keyCode, kMap[i].keyCmd));
					Add2Log(L"# hook Matched key 0x%0x, launching '%s'\n", kMap[i].keyCode, kMap[i].keyCmd);
					if (CreateProcess(kMap[i].keyCmd, kMap[i].keyArg, NULL, NULL, NULL, 0, NULL, NULL, NULL, &pi))
					{
						Add2Log(L"# hook CreateProcess OK\r\n", FALSE);
						CloseHandle(pi.hProcess);
						CloseHandle(pi.hThread);
					}
					else{
						Add2Log(L"# hook CreateProcess FAILED. LastError=%i (0x%x)\r\n", GetLastError(), GetLastError());
					}
					processed_key=true;
					Add2Log(L"# hook processed_key is TRUE\r\n", FALSE);

				}
			}
			if(!bMatchFound)
				Add2Log(L"# hook No match found\r\n", FALSE);
		}
		else if(wParam == WM_KEYDOWN){
			Add2Log(L"# hook got keydown: %i (0x%x). processed_key is '%i'\r\n", pkbhData->vkCode, pkbhData->vkCode, processed_key);
		}
	}
	//shall we forward processed keys?
	if (processed_key)
	{
		if (bForwardKey){
			Add2Log(L"# hook bForwardKey is TRUE. Resetting processed_key\r\n", FALSE);
			processed_key=false; //reset flag
			Add2Log(L"# hook CallNextHookEx() with processed_key=false\r\n", FALSE);
			return CallNextHookEx(g_hInstalledLLKBDhook, nCode, wParam, lParam);
		}
		else{
			Add2Log(L"# hook bForwardKey is FALSE. Returning...\r\n", FALSE);
			return true;
		}
	}
	else{
		Add2Log(L"# hook CallNextHookEx()\r\n", FALSE);
		return CallNextHookEx(g_hInstalledLLKBDhook, nCode, wParam, lParam);
	}
}

BOOL g_HookActivate(HINSTANCE hInstance)
{
	// We manually load these standard Win32 API calls (Microsoft says "unsupported in CE")
	SetWindowsHookEx		= NULL;
	CallNextHookEx			= NULL;
	UnhookWindowsHookEx	= NULL;

	// Load the core library. If it's not found, you've got CErious issues :-O
	//TRACE(_T("LoadLibrary(coredll.dll)..."));
	g_hHookApiDLL = LoadLibrary(_T("coredll.dll"));
	if(g_hHookApiDLL == NULL) {
		Add2Log(L"g_HookActivate: LoadLibrary FAILED...\r\n", FALSE);
		return false;
	}
	else {
		// Load the SetWindowsHookEx API call (wide-char)
		//TRACE(_T("OK\nGetProcAddress(SetWindowsHookExW)..."));
		SetWindowsHookEx = (_SetWindowsHookExW)GetProcAddress(g_hHookApiDLL, _T("SetWindowsHookExW"));
		if(SetWindowsHookEx == NULL) {
			Add2Log(L"g_HookActivate: GetProcAddress(SetWindowsHookEx) FAILED...\r\n", FALSE);
			return false;
		}
		else
		{
			Add2Log(L"g_HookActivate: GetProcAddress(SetWindowsHookEx) OK...\r\n", FALSE);
			// Load the hook.  Save the handle to the hook for later destruction.
			//TRACE(_T("OK\nCalling SetWindowsHookEx..."));
			g_hInstalledLLKBDhook = SetWindowsHookEx(WH_KEYBOARD_LL, g_LLKeyboardHookCallback, hInstance, 0);
			if(g_hInstalledLLKBDhook == NULL) {
				Add2Log(L"g_HookActivate: SetWindowsHookEx FAILED...\r\n", FALSE);
				return false;
			}
			else
				Add2Log(L"g_HookActivate: SetWindowsHookEx OK...\r\n", FALSE);

		}

		// Get pointer to CallNextHookEx()
		//TRACE(_T("OK\nGetProcAddress(CallNextHookEx)..."));
		CallNextHookEx = (_CallNextHookEx)GetProcAddress(g_hHookApiDLL, _T("CallNextHookEx"));
		if(CallNextHookEx == NULL) {
			Add2Log(L"g_HookActivate: GetProcAddress(CallNextHookEx) FAILED...\r\n", FALSE);
			return false;
		}
		else
			Add2Log(L"g_HookActivate: GetProcAddress(CallNextHookEx) OK...\r\n", FALSE);

		// Get pointer to UnhookWindowsHookEx()
		//TRACE(_T("OK\nGetProcAddress(UnhookWindowsHookEx)..."));
		UnhookWindowsHookEx = (_UnhookWindowsHookEx)GetProcAddress(g_hHookApiDLL, _T("UnhookWindowsHookEx"));
		if(UnhookWindowsHookEx == NULL) {
			Add2Log(L"g_HookActivate: GetProcAddress(UnhookWindowsHookEx) FAILED...\r\n", FALSE);
			return false;
		}
		else
			Add2Log(L"g_HookActivate: GetProcAddress(UnhookWindowsHookEx) OK...\r\n", FALSE);
	}
	//TRACE(_T("OK\nEverything loaded OK\n"));
	return true;
}


BOOL g_HookDeactivate()
{
	Add2Log(L"g_HookDeactivate()...\r\n", FALSE);
	//TRACE(_T("Uninstalling hook..."));
	if(g_hInstalledLLKBDhook != NULL)
	{
		Add2Log(L"\tUnhookWindowsHookEx...\r\n", FALSE);
		UnhookWindowsHookEx(g_hInstalledLLKBDhook);		// Note: May not unload immediately because other apps may have me loaded
		g_hInstalledLLKBDhook = NULL;
	}
	else
		Add2Log(L"\tg_hInstalledLLKBDhook is NULL\r\n", FALSE);

	Add2Log(L"\tUnloading coredll.dll...\r\n", FALSE);
	if(g_hHookApiDLL != NULL)
	{
		FreeLibrary(g_hHookApiDLL);
		g_hHookApiDLL = NULL;
	}
	//TRACE(_T("OK\nEverything unloaded OK\n"));
	Add2Log(L"\tEverything unloaded OK\r\n", FALSE);
	return true;
}


int WINAPI WinMain(	HINSTANCE hInstance,
					HINSTANCE hPrevInstance,
					LPTSTR    lpCmdLine,
					int       nCmdShow)
{
	MSG      msg      ;  
	HWND     hwnd     ;   
	WNDCLASS wndclass ; 

	newfile(L"\\ihook3.log.txt");
	Add2Log(L"###### iHook3 started...\r\n", FALSE);

	if (wcsstr(lpCmdLine, L"-writereg") != NULL){
		Add2Log(L"argument '-writereg' found\r\n", FALSE);
		WriteReg();
	}
	if (wcsstr(lpCmdLine, L"-nointermec") != NULL){
		Add2Log(L"argument '-nointermec' found\r\n", FALSE);
		bDoCheckIntermec=FALSE;
	}

	if (bDoCheckIntermec && (IsIntermec() != 0))
	{
		Add2Log(L"This is not an Intermec! Program execution stopped!\r\n", FALSE);
		MessageBox(NULL, L"This is not an Intermec! Program execution stopped!", L"Fatal Error", MB_OK | MB_TOPMOST | MB_SETFOREGROUND);
		return -1;
	}

	//allow only one instance!
	//obsolete, as hooking itself prevents multiple instances
/*	HWND hWnd = FindWindow (szAppName, NULL);    
	if (hWnd) 
	{        
		//SetForegroundWindow (hWnd);            
		return -1;
	}
*/

	  wndclass.style         = CS_HREDRAW | CS_VREDRAW  ; 
	  wndclass.lpfnWndProc   = WndProc ;
	  wndclass.cbClsExtra    = 0 ;
	  wndclass.cbWndExtra    = 0 ;
	  wndclass.hInstance     = hInstance   ;
	  wndclass.hIcon         = LoadIcon (NULL , L"appicon.ico") ;
	  wndclass.hCursor       = LoadCursor (NULL , IDC_ARROW)  ; 
	  wndclass.hbrBackground = (HBRUSH) GetStockObject (GRAY_BRUSH)  ;
	  wndclass.lpszMenuName  = NULL              ;
	  wndclass.lpszClassName = szAppName ; 

	  RegisterClass (&wndclass) ;    
	                                              
	g_hInstance=hInstance;
	hwnd = CreateWindow (szAppName , L"iHook3" ,   
			 WS_VISIBLE | WS_CAPTION | WS_SYSMENU | WS_OVERLAPPED,          // Style flags                         
			 CW_USEDEFAULT,       // x position                         
			 CW_USEDEFAULT,       // y position                         
			 CW_USEDEFAULT,       // Initial width                         
			 CW_USEDEFAULT,       // Initial height                         
			 NULL,                // Parent                         
			 NULL,                // Menu, must be null                         
			 hInstance,           // Application instance                         
			 NULL);               // Pointer to create
						  // parameters
	if (!IsWindow (hwnd)){ 
		Add2Log(L"Failed to create window! EXIT.\r\n", FALSE);
		return 0; // Fail if not created.
	}
	//show a hidden window
	Add2Log(L"ShowWindow hidden\r\n", FALSE);
	ShowWindow   (hwnd , SW_HIDE); // nCmdShow) ;  
	UpdateWindow (hwnd) ;

	//Notification icon
	Add2Log(L"Adding notification icon\r\n", FALSE);
	HICON hIcon;
	hIcon=(HICON) LoadImage (g_hInstance, MAKEINTRESOURCE (IHOOK_STARTED), IMAGE_ICON, 16,16,0);
	nid.cbSize = sizeof (NOTIFYICONDATA);
	nid.hWnd = hwnd;
	nid.uID = 1;
	nid.uFlags = NIF_ICON | NIF_MESSAGE;
	// NIF_TIP not supported    
	nid.uCallbackMessage = MYMSG_TASKBARNOTIFY;
	nid.hIcon = hIcon;
	nid.szTip[0] = '\0';
	BOOL res = Shell_NotifyIcon (NIM_ADD, &nid);
	if(!res){
		DEBUGMSG(1 ,(L"Could not add taskbar icon. LastError=%i\r\n", GetLastError() ));
		Add2Log(L"Could not add taskbar icon. LastError=%i (0x%x)\r\n", GetLastError(), GetLastError());
	}else
		Add2Log(L"Taskbar icon added.\r\n", FALSE);

#ifdef DEBUG
	if (!res)
		ShowError(GetLastError());
#endif
	
 	// TODO: Place code here.


		Add2Log(L"Starting message pump...\r\n", FALSE);
	  while (GetMessage (&msg , NULL , 0 , 0))   
		{
		  TranslateMessage (&msg) ;         
		  DispatchMessage  (&msg) ;         
		} 
                                                                              
	  Add2Log(L"##### ....iHook3 ending...\r\n", FALSE);
	  return msg.wParam ;
}

                                        
LONG FAR PASCAL WndProc (HWND hwnd   , UINT message , 
                         UINT wParam , LONG lParam)                
                            
{ 

  switch (message)         
  {
	case WM_CREATE:
		Add2Log(L"WM_CREATE\r\nReadReg()\r\n", FALSE);
		ReadReg();
		if (g_HookActivate(g_hInstance))
		{
			Add2Log(L"g_HookActivate loaded OK\r\n", FALSE);
			MessageBeep(MB_OK);
			//system bar icon
			//ShowIcon(hwnd, g_hInstance);
			DEBUGMSG(1, (L"Hook loaded OK"));
		}
		else
		{
			MessageBeep(MB_ICONEXCLAMATION);
			Add2Log(L"g_HookActivate FAILED. EXIT!\r\n", FALSE);
			MessageBox(hwnd, L"Could not hook. Already running a copy of iHook3? Will exit now.", L"iHook3", MB_OK | MB_ICONEXCLAMATION);
			PostQuitMessage(-1);
		}
		//TRACE(_T("Hook did not success"));
		return 0;
		break;
	case WM_PAINT:
		PAINTSTRUCT ps;    
		RECT rect;    
		HDC hdc;     // Adjust the size of the client rectangle to take into account    
		// the command bar height.    
		GetClientRect (hwnd, &rect);    
		hdc = BeginPaint (hwnd, &ps);     
		DrawText (hdc, TEXT ("iHook3 loaded"), -1, &rect,
			DT_CENTER | DT_VCENTER | DT_SINGLELINE);    
		EndPaint (hwnd, &ps);     
		return 0;
		break;
	case MYMSG_TASKBARNOTIFY:
		    switch (lParam) {
				case WM_LBUTTONUP:
					//ShowWindow(hwnd, SW_SHOWNORMAL);
					SetWindowPos(hwnd, HWND_TOPMOST, 0,0,0,0, SWP_NOSIZE | SWP_NOREPOSITION | SWP_SHOWWINDOW);
					if (MessageBox(hwnd, L"Hook is loaded. End hooking?", szAppName, 
						MB_YESNO | MB_ICONQUESTION | MB_APPLMODAL | MB_SETFOREGROUND | MB_TOPMOST)==IDYES)
					{
						g_HookDeactivate();
						Shell_NotifyIcon(NIM_DELETE, &nid);
						PostQuitMessage (0) ; 
					}
					ShowWindow(hwnd, SW_HIDE);
				}
		return 0;
		break;
	case WM_DESTROY:
		//taskbar system icon
		RemoveIcon(hwnd);
		MessageBeep(MB_OK);
		g_HookDeactivate();
		//Shell_NotifyIcon(NIM_DELETE, &nid);
		PostQuitMessage (0) ; 
		return 0            ;
		break;
  }

  return DefWindowProc (hwnd , message , wParam , lParam) ;
}

void WriteReg()
{
	Add2Log(L"In WriteReg()...\r\n", FALSE);

	int i;
	TCHAR name[MAX_PATH+1];
//this is for CN2
	kMap[0].keyCode= 114; //F3
	wsprintf(kMap[0].keyCmd, L"\\windows\\iRotateCN2.exe");
	wsprintf(kMap[0].keyArg, L"");

	kMap[1].keyCode= 115; //F4
	wsprintf(kMap[1].keyCmd, L"\\windows\\LockTaskBar.exe");
	wsprintf(kMap[1].keyArg, L"-toggle");

	kMap[2].keyCode= 116; //F5
	wsprintf(kMap[2].keyCmd, L"\\windows\\iSIP2.exe");
	wsprintf(kMap[2].keyArg, L"");

	kMap[3].keyCode= 113; //F2
	wsprintf(kMap[3].keyCmd, L"explorer.exe");
	wsprintf(kMap[3].keyArg, L"");

	kMap[4].keyCode= 27; //ESC
	wsprintf(kMap[4].keyCmd, L"\\windows\\iKill2.exe");
	wsprintf(kMap[4].keyArg, L"iRun2.exe");
//for 700 keyb map has to be rewritten
//and use different keys (A1 to A4 should map to F1 to F4)
	OpenCreateKey(L"Software\\Intermec\\iHook3");
	for (i=0; i<5; i++)
	{
		wsprintf(name, L"key%i", i);
		RegWriteByte(name, kMap[i].keyCode );
		wsprintf(name, L"exe%i", i);
		RegWriteStr(name, kMap[i].keyCmd );
		wsprintf(name, L"arg%i", i);
		RegWriteStr(name, kMap[i].keyArg );
	}
	RegWriteByte(L"ForwardKey", 0);
	CloseKey();
	Add2Log(L"Out WriteReg()...\r\n", FALSE);
	//return 0;
}

int ReadReg()
{
	Add2Log(L"IN ReadReg()...\r\n", FALSE);
	int i;
	TCHAR str[MAX_PATH+1];
	byte dw=0;
	TCHAR name[MAX_PATH+1];
	lastKey=-1;
	LONG rc;
	int iRes = OpenKey(L"Software\\Intermec\\iHook3");
	Add2Log(L"\tOpenKey 'Software\\Intermec\\iHook3' returned %i (0x%x)\r\n", iRes, iRes);
	for (i=0; i<10; i++)
	{
		kMap[i].keyCode=0;
		wcscpy(kMap[i].keyCmd, L"");
		wcscpy(kMap[i].keyArg, L"");
		wsprintf(name, L"key%i", i);
		//look for keyX
		rc = RegReadByte(name, &dw);
		if (rc==0)
		{
			//look for exeX
			Add2Log(L"\tlooking for entry 'key%i' (name='%s') return code=%i read value=(0x%x)...OK\r\n", i, name, rc, dw);
			kMap[i].keyCode=dw;
			wsprintf(name, L"exe%i", i);
			iRes=RegReadStr(name, str);
			Add2Log(L"\t\tlooking for exe%i (name='%s'), result=%i, value='%s'\r\n", i, name, iRes, str);
			if(iRes==0)
			{
				wcscpy(kMap[i].keyCmd, str);
				//look for argX
				wsprintf(name, L"arg%i", i);
				iRes=RegReadStr(name, str);
				lastKey=i;	// a valid combination is a keyX and a cmdX entry
				if(iRes==0)
				{
					Add2Log(L"\t\tlooking for arg%i (name='%s'), result=%i, value='%s' OK\r\n", i, name, iRes, str);
					wcscpy(kMap[i].keyArg, str);
				}
				else
				{
					Add2Log(L"\t\tlooking for arg%i (name='%s') result=%i FAILED. Using empty arg.\r\n", i, name, iRes);
				}
			}
			else {
				Add2Log(L"\t\tlooking for exe%i FAILED with result=%i\r\n", i, iRes);
				break; //no exe name
			}
		}
		else
		{
			#ifdef DEBUG
						ShowError(rc);
			#endif
			Add2Log(L"\tlooking for entry 'key%i' (name='%s') return code=%i...FAILED\r\n", i, name, rc);
			break; //no key
		}
	}
	Add2Log(L"\tread a total of %i (0x%x) valid entries\r\n", lastKey+1, lastKey+1);
	//Read if we have to forward the keys
	rc=RegReadByte(L"ForwardKey", &dw);
	if(rc==0)
	{
		Add2Log(L"\tlooking for 'ForwardKey' OK\r\n",lastKey,lastKey);
		if (dw>0){
			Add2Log(L"\tForwardKey is TRUE \r\n", FALSE);
			bForwardKey=true;
		}
		else{
			Add2Log(L"\tForwardKey is FALSE \r\n", FALSE);
			bForwardKey=false;
		}
	}
	else
	{
		#ifdef DEBUG
			ShowError(rc);
		#endif
		Add2Log(L"\tlooking for 'ForwardKey' FAILED. Using default=TRUE.\r\n", FALSE);
		bForwardKey=true;
	}
	CloseKey();
	Add2Log(L"OUT ReadReg()...\r\n", FALSE);
	return 0;
}

void ShowIcon(HWND hWnd, HINSTANCE hInst)
{
    NOTIFYICONDATA nid;

    int nIconID=1;
    nid.cbSize = sizeof (NOTIFYICONDATA);
    nid.hWnd = hWnd;
    nid.uID = nIconID;
    nid.uFlags = NIF_ICON | NIF_MESSAGE;   // NIF_TIP not supported
    nid.uCallbackMessage = MYMSG_TASKBARNOTIFY;
    nid.hIcon = (HICON)LoadImage (g_hInstance, MAKEINTRESOURCE (ID_ICON), IMAGE_ICON, 16,16,0);
    nid.szTip[0] = '\0';

    BOOL r = Shell_NotifyIcon (NIM_ADD, &nid);
	if(!r){
		DEBUGMSG(1 ,(L"Could not add taskbar icon. LastError=%i\r\n", GetLastError() ));
		Add2Log(L"Could not add taskbar icon. LastError=%i (0x%x)\r\n", GetLastError(), GetLastError() );
	}

}

void RemoveIcon(HWND hWnd)
{
	NOTIFYICONDATA nid;

    memset (&nid, 0, sizeof nid);
    nid.cbSize = sizeof (NOTIFYICONDATA);
    nid.hWnd = hWnd;
    nid.uID = 1;

    Shell_NotifyIcon (NIM_DELETE, &nid);
	Add2Log(L"Shell_NotifyIcon(NIM_DELETE) done.\r\n",FALSE );

}