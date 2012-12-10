// KeyWedge.cpp : Defines the entry point for the application.

#pragma warning ( disable : 4244 4068 )

#define USE_SSAPI
#undef USE_SSAPI

//
/*
	REGEDIT4

	[HKEY_LOCAL_MACHINE\SOFTWARE\HGO\KeyWedge]
	"parity"=dword:00000000
	;		0 NOPARITY
	;		1 ODDPARITY
	;		2 EVENPARITY
	;		3 MARKPARITY
	;		4 SPACEPARITY
	"stopbits"=dword:00000000
	;		0 ONESTOPBIT	
	;		1 ONE5STOPBITS
	;		2 TWOSTOPBITS	
	"databits"=dword:00000008
	;	 7
	;	 8
	;	16
	"handshake"=dword:00000003
	;	1	Xon/Xoff
	;	2	Hardware
	;	3	None
	"baudrate"=dword:0000e100
	;	  9600
	;	 19200
	;	 38400
	;	 57600
	;	115200
	"comport"="COM4:"
	;	string with trailing :
	"sendcharbychar"=dword:00000000
	;	1	send byte for byte as received, Post- and Preamble will not be used!
	;	0	send keys when \n received
	"keytab"=hex:\
		  00,4e,55,4c,00,00,00,00,00,00,00,00,00,00,00,00,00,00,01,53,4f,48,00,00,00,\
		  00,00,00,00,00,00,00,00,00,00,00,02,53,54,58,00,00,00,00,00,00,00,00,00,00,\
		  00,00,00,00,03,45,54,58,00,00,00,00,00,00,00,00,00,00,00,00,00,00,04,45,4f,\
		  54,00,00,00,00,00,00,00,00,00,00,00,00,00,00,05,45,4e,51,00,00,00,00,00,00,.......
    "postamble"="\r\n"
	"preamble"=""
	"BeepAfterRead"=dword:00000001
*/

#ifdef USE_SSAPI
	#pragma comment (user, "##### Compiled for use with SSAPI #####")
	#pragma message ("##### Compiled for use with SSAPI #####")
	#pragma exestr ("##### Compiled for use with SSAPI #####")
#else
	#pragma comment (user, "##### Compiled for use without SSAPI #####")
	#pragma message ("##### Compiled for use without SSAPI #####")
	#pragma exestr ("##### Compiled for use without SSAPI #####")
#endif

//#include "stdafx.h"

#include <windows.h>                 // For all that Windows stuff
#include <commctrl.h>                // Command bar includes
#include <aygshell.h>                // Pocket PC includes
#pragma comment (lib, "aygshell.lib")

#include "KeyWedge.h"

#include "keymap.h" // a map with a translation table
#include "RegRW.h"
// #include "UIHelper.h"
#include "./common/nclog.h"

#include "PowerMsgQueue.h"

#define MAX_LOADSTRING 100
#define BUFF_SIZE 1024

//fuer SmartSystems
//	#define SSKEYWEDGE L"SSKeyWedge"	// not used
	const LPCWSTR pSubKey = L"SOFTWARE\\Intermec\\SSKeyWedge";

// Global Variables:
HKEY				m_hKey;			// the handle to the registry
HINSTANCE			hInst;			// The current instance
HWND				hwndCB;			// The command bar handle
HWND				hwndMenuBar;	// handle for menubar

//removed in v1.3: BOOL b_MainWindowVisible=false; //new in v1.2 to track window is already loaded

NOTIFYICONDATA nid;
TCHAR szAppName[MAX_LOADSTRING];

// Our DDB handle is a global variable.
HBITMAP hbm;

HANDLE g_hSendEvent = INVALID_HANDLE_VALUE;
HANDLE hReadThread = INVALID_HANDLE_VALUE;

// Forward declarations of functions included in this code module:
ATOM				MyRegisterClass	(HINSTANCE, LPTSTR);
BOOL				InitInstance	(HINSTANCE, int);
LRESULT CALLBACK	WndProc			(HWND, UINT, WPARAM, LPARAM);
LRESULT CALLBACK	About			(HWND, UINT, WPARAM, LPARAM);
LRESULT CALLBACK	OptionsDlgProc	(HWND, UINT, WPARAM, LPARAM) ;

BOOL                InWideMode();

//================================================================
int TermInstance (HINSTANCE , int );
DWORD WINAPI SendThread (PVOID pArg);

//================================================================
DWORD WINAPI CommReadThreadFunc(LPVOID );	//	thread to receive comm input
HANDLE g_hCommReadThread=NULL;				//	global handle to thread

DWORD WINAPI CommWatchdog(LPVOID );			//	thread to retry OpenCOMM
HANDLE g_hThreadWatchdog=NULL;				//	global handle to CommWatchDog

void WriteCOM(TCHAR *txt);					//	function to write txt to COMM port
void CloseCOMM();							//	function to Close COMM port
void OpenCOMM(TCHAR *szPort);				//	function to open COMM port
void showError(long er);					//	function to show error txt for error er

#define KEYEVENTF_KEYDOWN 0x0000			//	defines the code for KEYEVENTF_KEYDOWN

void SendKeys(char * szTxt);				//	this sends szTxt as keystrokes
void ReportCommError(LPTSTR lpszMessage);	//	function to show CommErrors

BOOL g_bContinue = TRUE;					//	global to stopp all threads!
HANDLE g_hCommPort = INVALID_HANDLE_VALUE;	//	global handle to com port

void resumeCOMM();							//	function which resumes threads
void suspendCOMM();							//	function to resume threads

extern int ReadReg();						//	function to read values from reg to global vars
bool bsendcharbychar = false;				//	global to store if receive will be done char by char

bool LaunchNotes(HWND hNotes);				//	function launches notes.exe for testing purpose

//================================================================
// defines for setup dialog
struct st_txtval
{	TCHAR	txt[15];
	/*WORD*/unsigned char	val;
}; 

struct st_txtvald
{	TCHAR	txt[15];
	DWORD	val;
}; 
//Baudrates
struct st_txtvald  baud[5]={
			{TEXT("9600"), CBR_9600},
			{TEXT("19200"), CBR_19200}, 
			{TEXT("38400"), CBR_38400}, 
			{TEXT("57600"), CBR_57600}, 
			{TEXT("115200"), CBR_115200}};

struct st_txtval stopbits[3]={	
				{TEXT("ONESTOPBIT"),	ONESTOPBIT		},
				{TEXT("ONE5STOPBITS"),	ONE5STOPBITS	},
				{TEXT("TWOSTOPBITS"),	TWOSTOPBITS		}};

struct st_txtval parity[5]=
			{	{TEXT("NOPARITY"),		NOPARITY},
				{TEXT("ODDPARITY"),		ODDPARITY},
				{TEXT("EVENPARITY"),	EVENPARITY},
				{TEXT("MARKPARITY"),	MARKPARITY},
				{TEXT("SPACEPARITY"),	SPACEPARITY}};
//databits
struct st_txtval databits[3]=
			{	
				{TEXT("7 Bit"), 7},
				{TEXT("8 Bit"), 8},
				{TEXT("16 Bit"), 16}};
//handshake
struct st_txtval handshake[3]=
{	{TEXT("Xon/Xoff"), 1},
	{TEXT("Hardware"), 2},
	{TEXT("None"), 3}};

//Globale Variablen für SetCommState
DWORD	nBaud = CBR_57600;
unsigned char	
	nParity = NOPARITY,
	nStopbits = ONESTOPBIT,
	nDatabits = 8,
	nHandshake = 3;

TCHAR g_szCOM[32]=L"COM1:";
TCHAR g_szPostamble[32]=L"\\r\\n"; //internally we will use printable control codes
TCHAR g_szPreamble[32]=L"";
DWORD g_dwBeepAfterRead=0;

//BOOL bPAmblesConverted=false;
typedef std::basic_string<TCHAR, std::char_traits<TCHAR>,std::allocator<TCHAR> > tstring;

//================================================================
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
#ifdef DEBUG
	DEBUGMSG( 1, ( temp ) ); 
	nclog(L"ShowError: '%s'\r\n", temp);
#else
	// Display the string.
	RETAILMSG( 1,(temp));
#endif
	// Free the buffer.
	LocalFree( lpMsgBuf );
}

#ifdef USE_SSAPI
//watches the event and updates settings from registry
DWORD UpdateFields( LPVOID pParam )
{
	while( TRUE )
	{
		WaitForSingleObject( m_hEvent, INFINITE );
		ReadReg();
	}
}
#endif

//================================================================

//
//  FUNCTION: WinMain(...)
//
//  PURPOSE: main entry point
//
//  COMMENTS:
//
int WINAPI WinMain(	HINSTANCE hInstance,
					HINSTANCE hPrevInstance,
					LPTSTR    lpCmdLine,
					int       nCmdShow)
{
	MSG msg;
	HACCEL hAccelTable;

	// Perform application initialization:
	if (!InitInstance (hInstance, nCmdShow)) 
	{
		return FALSE;
	}

	hAccelTable = LoadAccelerators(hInstance, (LPCTSTR)IDC_KEYWEDGE);

#ifdef DEBUG
	//dump vkTable
	byte vCode;
	bool bShift;
	for (int j=0; j<KTAB_SIZE; j++)
	{
		vCode=vkTable[j].kVKval;
		bShift=vkTable[j].kShift;
		DEBUGMSG(1, (L"j=%3i\t0x%2x\tShift:%i\r\n",j,vCode,bShift));
	}
#endif

	if(wcsstr(lpCmdLine, L"usefilelog")!=NULL)
		bUseFileLog=TRUE;
	else
		bUseFileLog=FALSE;
	// Main message loop:
	while (GetMessage(&msg, NULL, 0, 0)) 
	{
		if (!TranslateAccelerator(msg.hwnd, hAccelTable, &msg)) 
		{
			TranslateMessage(&msg);
			DispatchMessage(&msg);
		}
	}
	//HGO return msg.wParam;
	// Instance cleanup
    return TermInstance (hInstance, msg.wParam);
}

//
//  FUNCTION: MyRegisterClass()
//
//  PURPOSE: Registers the window class.
//
//  COMMENTS:
//
//    It is important to call this function so that the application 
//    will get 'well formed' small icons associated with it.
//
ATOM MyRegisterClass(HINSTANCE hInstance, LPTSTR szWindowClass)
{
	WNDCLASS	wc;

    wc.style			= CS_HREDRAW | CS_VREDRAW;
    wc.lpfnWndProc		= (WNDPROC) WndProc;
    wc.cbClsExtra		= 0;
    wc.cbWndExtra		= 0;
    wc.hInstance		= hInstance;
    wc.hIcon			= LoadIcon(hInstance, MAKEINTRESOURCE(IDI_KEYWEDGE));
    wc.hCursor			= 0;
    wc.hbrBackground	= (HBRUSH) GetStockObject(WHITE_BRUSH);
    wc.lpszMenuName		= 0;
    wc.lpszClassName	= szWindowClass;

	return RegisterClass(&wc);
}

//
//  FUNCTION: InitInstance(HANDLE, int)
//
//  PURPOSE: Saves instance handle and creates main window
//
//  COMMENTS:
//
//    In this function, we save the instance handle in a global variable and
//    create and display the main program window.
//
BOOL InitInstance(HINSTANCE hInstance, int nCmdShow)
{
	HWND	hWnd;
	TCHAR	szTitle[MAX_LOADSTRING];			// The title bar text
	TCHAR	szWindowClass[MAX_LOADSTRING];		// The window class name

	hInst = hInstance;		// Store instance handle in our global variable
	// Initialize global strings
	LoadString(hInstance, IDC_KEYWEDGE, szWindowClass, MAX_LOADSTRING);
	MyRegisterClass(hInstance, szWindowClass);

	LoadString(hInstance, IDS_APP_TITLE, szTitle, MAX_LOADSTRING);
	wcscpy(szAppName,szTitle);

    //Allow only one instance!
	hWnd = FindWindow (szAppName, NULL);
    if (hWnd) {
		SetForegroundWindow ((HWND)(((DWORD)hWnd) | 0x01));
        return 0;
    }

	hWnd = CreateWindow(szWindowClass, szTitle, WS_VISIBLE,
		CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT, NULL, NULL, hInstance, NULL);

	if (!hWnd)
	{	
		return FALSE;
	}

	// Save program instance handle in global variable.
    hInst = hInstance;

	//Read the registry
	ReadReg();

    // Create unnamed auto-reset event initially false.
    g_hSendEvent = CreateEvent (NULL, FALSE, FALSE, NULL);

    // Create write thread. Read thread created when port opened.
    HANDLE hThread;
    ULONG rc;
    hThread = CreateThread (NULL, 0, SendThread, hWnd, 0, &rc);
    if (hThread){
        CloseHandle (hThread);
		nclog(L"InitInstance: SendThread created with id=0x%x\r\n", rc);
	}
    else {
        DestroyWindow (hWnd);
        return 0;
    }
	//Create COMM Watchdog thread //test
	g_hThreadWatchdog = CreateThread(NULL, 0, CommWatchdog, g_szCOM, 0, &rc);

	//create PowerResume watchdog thread
	hwndMain=hWnd;
	if(startMsgThread()==0)
		nclog(L"InitInstance: startMsgThread() with id=0x%x\r\n", rc);
	else
		nclog(L"InitInstance: startMsgThread() FAILED\r\n");

	//helps
	resumeCOMM();

	ShowWindow(hWnd, SW_HIDE);//nCmdShow);
	UpdateWindow(hWnd);

	//Notification icon
	HICON hIcon;
	hIcon=(HICON) LoadImage (hInstance, MAKEINTRESOURCE (IWEDGE_STARTED), IMAGE_ICON, 16,16,0);
	nid.cbSize = sizeof (NOTIFYICONDATA);
	nid.hWnd = hWnd;
	nid.uID = 1;
	nid.uFlags = NIF_ICON | NIF_MESSAGE;
	// NIF_TIP not supported    
	nid.uCallbackMessage = MYMSG_TASKBARNOTIFY;
	nid.hIcon = hIcon;
	nid.szTip[0] = '\0';
	BOOL res = Shell_NotifyIcon (NIM_ADD, &nid);
#ifdef DEBUG
	if (!res)
		ShowError(GetLastError());
#endif

	if (hwndCB)
		CommandBar_Show(hwndCB, TRUE);

	return TRUE;
}

//----------------------------------------------------------------------
// TermInstance - Program cleanup
//
int TermInstance (HINSTANCE hInstance, int nDefRC) {
    HANDLE hPort = g_hCommPort;

    g_bContinue = FALSE;

	suspendCOMM();
    g_hCommPort = INVALID_HANDLE_VALUE;
	CloseHandle(g_hCommReadThread);
	CloseHandle(g_hThreadWatchdog);
    if (hPort != INVALID_HANDLE_VALUE)
	{
        CloseHandle (hPort);
		Sleep(500);
	}

	Shell_NotifyIcon(NIM_DELETE, &nid);
	//refresh screen
	HWND hWndTaskbar = FindWindow(L"HHTaskBar", NULL);
	if (hWndTaskbar!=NULL)
		PostMessage(hWndTaskbar, WM_PAINT, 0, 0);

    if (g_hSendEvent != INVALID_HANDLE_VALUE) {
        PulseEvent (g_hSendEvent);
        Sleep(100);
        CloseHandle (g_hSendEvent);
    }
    return nDefRC;
}

//
//  FUNCTION: WndProc(HWND, unsigned, WORD, LONG)
//
//  PURPOSE:  Processes messages for the main window.
//
//  WM_COMMAND	- process the application menu
//  WM_PAINT	- Paint the main window
//  WM_DESTROY	- post a quit message and return
//
//
LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	HDC hdc;
	HDC hdcMem;//bmp
	HGDIOBJ hOldSel; //bmp
	int wmId, wmEvent;
	PAINTSTRUCT ps;
	TCHAR szHello[MAX_LOADSTRING];

	DWORD screenW=240;
	DWORD screenH=320;
	HWND hNotes = NULL;

	switch (message) 
	{
		case WM_SHOWWINDOW:
			if(!wParam)
				ShowWindow(hwndMenuBar, SW_HIDE);
			else
				ShowWindow(hwndMenuBar, SW_SHOWNORMAL);
				//SH_HIDE?;
			break;
		case WM_SIZE:
			screenW = GetSystemMetrics(SM_CXSCREEN);
			screenH = GetSystemMetrics(SM_CYSCREEN);
			break;
		case WM_COMMAND:
			suspendCOMM();
			wmId    = LOWORD(wParam); 
			wmEvent = HIWORD(wParam); 
			// Parse the menu selections:
			switch (wmId)
			{
				case IDM_HELP_ABOUT:
				   DialogBox(hInst, (LPCTSTR)IDD_ABOUTBOX, hWnd, (DLGPROC)About);
				   break;
				case IDM_FILE_EXIT:
				   DestroyWindow(hWnd);
				   break;
				case IDM_OPTIONS:
					//DialogBox(hInst, (LPCTSTR)IDD_OPTIONSBOX, hWnd, (DLGPROC)OptionsDlgProc);
					//replaced by modal dialog box
					if (InWideMode())
						DialogBoxParam(hInst, (LPCTSTR)IDD_OPTIONSBOX_WIDE, hWnd, (DLGPROC)OptionsDlgProc, 0);
					else
						DialogBoxParam(hInst, (LPCTSTR)IDD_OPTIONSBOX, hWnd, (DLGPROC)OptionsDlgProc, 0);
					WriteReg();
					break;
				case IDM_HIDE:
					resumeCOMM(); //suspend is called in MYMSG_TASKBARNOTIFY
					ShowWindow(hWnd, SW_HIDE);
//					b_MainWindowVisible=false;
					break;
				default:
				   return DefWindowProc(hWnd, message, wParam, lParam);
			}
			break;
		case WM_USER_RESUMECOMM: //will be called from bg thread after suspend/resume
			nclog(L"WndProc: received WM_USER_RESUMECOMM: restarting COMM\n");
			suspendCOMM();
			Sleep(100);
			resumeCOMM(); //suspend is called in MYMSG_TASKBARNOTIFY
			break;
		case WM_CREATE:
			//load bitmap
			hbm = LoadBitmap(hInst,MAKEINTRESOURCE(IDB_BITMAP1));
#ifdef WIN32_PLATFORM_PSPC
//#########################
			// Create a menu bar.
			SHMENUBARINFO mbi;
			memset(&mbi, 0, sizeof(SHMENUBARINFO));
			mbi.cbSize = sizeof(SHMENUBARINFO);
			mbi.hwndParent = hWnd;                  // Parent window
			mbi.dwFlags = SHCMBF_HMENU | SHCMBF_HIDESIPBUTTON; //SHCMBF_EMPTYBAR;          // Flags like hide SIP btn
			mbi.nToolBarId = IDM_MENU;				// 0;                     // ID of toolbar resource
			mbi.hInstRes = hInst;                       // Inst handle of app
			mbi.nBmpId = 0;                         // ID of bitmap resource
			mbi.cBmpImages = 0;                     // Num of images in bitmap 
			mbi.hwndMB = 0;                         // Handle of bar returned

			// Create menu bar and check for errors.
			if (SHCreateMenuBar(&mbi))
				hwndMenuBar = mbi.hwndMB;           // Save the menu bar handle.
			else
			{
				DWORD dwErr=GetLastError();
				DEBUGMSG(1, (L"## SHCreateMenuBar: error 0x%08x", dwErr));
			}
//######
#else
			hwndCB = CommandBar_Create(hInst, hWnd, 1);			
			CommandBar_InsertMenubar(hwndCB, hInst, IDM_MENU, 0);
			CommandBar_AddAdornments(hwndCB, 0, 0);
#endif
			/// start openCOMM watchdog
			g_bContinue=true;

			//ULONG rc;
			//hThread = CreateThread (NULL, 0, SendThread, hWnd, 0, &rc);	//see InitInstance
			
			//test moved to InitInstance
			//g_hThreadWatchdog = CreateThread(NULL, 0, CommWatchdog, g_szCOM, 0, &rc);

			#ifdef USE_SSAPI
				//SS integration *START
				// Create the event for being awakened by the configlet
				m_hEvent = CreateEvent( NULL, FALSE, FALSE, L"SSKeyWedge" );
				if( !m_hEvent  ||  GetLastError() != ERROR_ALREADY_EXISTS )
				{
					DEBUGMSG(1, (L"Cannot communicate with SSSample configlet\r\n"));
					//MessageBox( NULL, L"Cannot communicate with SSSample configlet", L"Error", MB_OK );
					//PostQuitMessage(0);
				}
			#endif
			// Open the registry key for the SSSample configuration items.
			LONG lStatus ;
			lStatus = RegOpenKeyEx( 
				HKEY_LOCAL_MACHINE, 
				pSubKey,
				0, 
				0, 
				&m_hKey 
				);

			#ifdef USE_SSAPI
				if( ERROR_SUCCESS != lStatus )
				{
					// TODO: Get this message from resource file
					DEBUGMSG(1, (L"SSKeyWedge Not Smart-System Enabled\r\n"));
					//MessageBox( NULL, L"SSKeyWedge Not Smart-System Enabled", L"Error", MB_OK );
					//PostQuitMessage(0); 
				}
				else
				{
					ReadReg(); //RefreshFields();
					DWORD dwThreadID;
					CreateThread( NULL, 0, UpdateFields, NULL, 0, &dwThreadID );
				}
			#endif
			//SS integration END*

			break;
		case WM_PAINT:
			RECT rt;
			hdc = BeginPaint(hWnd, &ps);
			GetClientRect(hWnd, &rt);

			hdcMem = CreateCompatibleDC(NULL);
			//HBITMAP hbmT = ::SelectBitmap(hdcMem,hbm);
			// Select the bitmap into the compatible device context.
			hOldSel = SelectObject(hdcMem, hbm);
			// Now, the BitBlt function is used to transfer the contents of the 
			// drawing surface from one DC to another. Before we can paint the
			// bitmap however we need to know how big it is. We call the GDI
			// function GetObject to get the relevent details.
			BITMAP bm;
			GetObject(hbm,sizeof(bm),&bm);

			BitBlt(hdc,0,0,bm.bmWidth,bm.bmHeight,hdcMem,0,0,SRCCOPY);

			LoadString(hInst, IDS_HELLO, szHello, MAX_LOADSTRING);
			DrawText(hdc, szHello, _tcslen(szHello), &rt, 
				DT_SINGLELINE | DT_VCENTER | DT_CENTER);

			// Restore original bitmap selection and destroy the memory DC.
			SelectObject (hdcMem, hOldSel);
			DeleteDC(hdcMem);

			EndPaint(hWnd, &ps);
			break;
		case WM_QUIT:
		case WM_DESTROY:
			if(stopMsgThread()==1)
				nclog(L"stopMsgThread OK\n");
			else
				nclog(L"stopMsgThread failed\n");

			CloseCOMM();

			CommandBar_Destroy(hwndCB);
			PostQuitMessage(0);
			break;
		case MYMSG_TASKBARNOTIFY:
				switch (lParam) {
					case WM_LBUTTONUP:
						//if (b_MainWindowVisible){
							//bring window to front
							SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
							ShowWindow(hWnd, SW_SHOWNORMAL);
							PostMessage(hWnd, WM_PAINT, 0, 0);
							return 0;
						//}
						suspendCOMM();
						//ShowWindow(hWnd, SW_SHOWNORMAL);
						//b_MainWindowVisible=true;
						/*
						int antw=MessageBox(hWnd, L"KeyWedge is loaded. End?", szAppName, 
							MB_YESNOCANCEL | MB_ICONQUESTION | MB_APPLMODAL | MB_SETFOREGROUND | MB_TOPMOST);
						if (antw==IDYES)
						{
							Shell_NotifyIcon(NIM_DELETE, &nid);
							//refresh screen
							HWND hWndTaskbar = FindWindow(L"HHTaskBar", NULL);
							if (hWndTaskbar!=NULL)
								PostMessage(hWndTaskbar, WM_PAINT, 0, 0);
							PostQuitMessage (0) ; 
						}
						else if(antw==IDCANCEL)
						{
							ShowWindow(hWnd, SW_HIDE);
							resumeCOMM();
						}
						else if(antw==IDNO)
						{
							ShowWindow(hWnd, SW_SHOWNORMAL); //SW_SHOWMAXIMIZED);
						}
						*/
						PostMessage(hWnd, WM_PAINT, 0, 0);
						break;
					}
			return 0;
			break;
		default:
			return DefWindowProc(hWnd, message, wParam, lParam);
   }
   return 0;
}


//======================================================================
// Options Dialog procedure
//
LRESULT CALLBACK OptionsDlgProc (HWND hDlg, UINT wMsg, WPARAM wParam, LPARAM lParam) 
{
    TCHAR szOut[128];
    INT i;
	HANDLE hLocal;
	i=0;
	hLocal = g_hCommPort;
	//STOPPS communication!
    g_hCommPort = INVALID_HANDLE_VALUE;     
	//Lokale Variablen zur Zwischenspeicherung, falls Cancel gewählt wird
	//old values
	TCHAR	oTxt[32], szTxt[32];
	wsprintf(oTxt, g_szCOM);
	DWORD	oBaud;
	unsigned char	oParity, oStopbits, oDatabits, oHandshake;
	oBaud	=	nBaud;
	oParity	=	nParity;
	oStopbits	=	nStopbits;
	oDatabits	=	nDatabits;
	oHandshake	=	nHandshake;
	bool obsendcharbychar = bsendcharbychar;
	SHINITDLGINFO shidi;
	BOOL bSuccessInit;
//	TCHAR oPostamble[32];
//	TCHAR oPreamble[32];
	DWORD oBeepAfterRead=1;

	switch (wMsg) {
		/*
		case WM_SIZE:
			RelayoutDialog(hInst, hDlg, InWideMode() ?
						MAKEINTRESOURCE(IDD_OPTIONSBOX_WIDE) :  
						MAKEINTRESOURCE(IDD_OPTIONSBOX));
			return true;
		*/
		case WM_INITDIALOG:
			//resize
			// Create a Done button and size it.
			shidi.dwMask = SHIDIM_FLAGS;
			shidi.dwFlags = SHIDIF_SIZEDLGFULLSCREEN;// | SHIDIF_EMPTYMENU;
			shidi.hDlg = hDlg;
			SHInitDialog(&shidi);
			bSuccessInit = SHInitDialog(&shidi);
			//COM port
			SetDlgItemText(hDlg, IDC_COMPORT, g_szCOM);
			//post and preamble
			SetDlgItemText(hDlg, IDC_Pre, g_szPreamble);
			SetDlgItemText(hDlg, IDC_Post, g_szPostamble);
			//BeepAfterRead
			oBeepAfterRead=g_dwBeepAfterRead;
			//****************** Baud
			for (i = 0; i < dim(baud); i++) 
			{
				SendDlgItemMessage (hDlg, IDC_BAUD, (WPARAM)CB_INSERTSTRING, i, (LPARAM)baud[i].txt);
				SendDlgItemMessage (hDlg, IDC_BAUD, CB_SETITEMDATA , i, (DWORD)baud[i].val);
				//TESTING ONLY

				wsprintf(szOut, TEXT("%lu"), dim(baud) );
				SetDlgItemText(hDlg, IDC_EDIT1, szOut);
			}
			//Change the Selection to active Value
			for (i = 0; i < dim(baud); i++) 
			{
				if (baud[i].val==nBaud)
					SendDlgItemMessage (hDlg, IDC_BAUD, CB_SETCURSEL, (WPARAM)i, 0);
			}

			//****************** Databits
			for (i = 0; i < dim(databits); i++) 
			{
				//wsprintf (szOut, TEXT("Item %s"), databits[i].txt);
				//hControl = GetDlgItem(hDlg, IDC_BAUD);
				SendDlgItemMessage (hDlg, IDC_DATABITS, (WPARAM)CB_INSERTSTRING, i, (LPARAM)databits[i].txt);
				SendDlgItemMessage (hDlg, IDC_DATABITS, CB_SETITEMDATA , i, (DWORD)databits[i].val);
			}
			for (i = 0; i < dim(databits); i++) 
			{
				if (databits[i].val==nDatabits)
					SendDlgItemMessage (hDlg, IDC_DATABITS, CB_SETCURSEL, (WPARAM)i, 0);
			}
			
			//****************** STOPBITS
			for (i = 0; i < dim(stopbits); i++) 
			{
				SendDlgItemMessage (hDlg, IDC_STOPBITS, (WPARAM)CB_INSERTSTRING, i, (LPARAM)stopbits[i].txt);
				SendDlgItemMessage (hDlg, IDC_STOPBITS, CB_SETITEMDATA , i, (DWORD)stopbits[i].val);
			}
			for (i = 0; i < dim(stopbits); i++) 
			{
				if (stopbits[i].val==nStopbits)
					SendDlgItemMessage (hDlg, IDC_STOPBITS, CB_SETCURSEL, (WPARAM)i, 0);
			}

			//****************** PARITY
			for (i = 0; i < dim(parity); i++) 
			{
				SendDlgItemMessage (hDlg, IDC_PARITY, (WPARAM)CB_INSERTSTRING, i, (LPARAM)parity[i].txt);
				SendDlgItemMessage (hDlg, IDC_PARITY, CB_SETITEMDATA , i, (DWORD)parity[i].val);
			}
			for (i = 0; i < dim(parity); i++) 
			{
				if (parity[i].val==nParity)
					SendDlgItemMessage (hDlg, IDC_PARITY, CB_SETCURSEL, (WPARAM)i, 0);
			}

			//****************** Handshake
			for (i = 0; i < dim(handshake); i++) 
			{
				SendDlgItemMessage (hDlg, IDC_HANDSHAKE, (WPARAM)CB_INSERTSTRING, i, (LPARAM)handshake[i].txt);
				SendDlgItemMessage (hDlg, IDC_HANDSHAKE, CB_SETITEMDATA , i, handshake[i].val);
			}
			for (i = 0; i < dim(handshake); i++) 
			{
				if (handshake[i].val==nHandshake)
					SendDlgItemMessage (hDlg, IDC_HANDSHAKE, CB_SETCURSEL, (WPARAM)i, 0);
			}

			//******************* bsendcharbychar
			if (bsendcharbychar)
			{
				SendDlgItemMessage(hDlg, IDC_CHECK1, BM_SETCHECK, (WPARAM) BST_CHECKED, 0);
				EnableWindow(GetDlgItem(hDlg, IDC_Pre), FALSE);
				EnableWindow(GetDlgItem(hDlg, IDC_Post), FALSE);
			}
			else
			{
				SendDlgItemMessage(hDlg, IDC_CHECK1, BM_SETCHECK, (WPARAM) BST_UNCHECKED, 0);
				EnableWindow(GetDlgItem(hDlg, IDC_Pre), TRUE);
				EnableWindow(GetDlgItem(hDlg, IDC_Post), TRUE);
			}
			//******************* BeepAfterRead
			if (oBeepAfterRead>0)
			{
				SendDlgItemMessage(hDlg, IDC_CHECK2, BM_SETCHECK, (WPARAM) BST_CHECKED, 0);
			}
			else
			{
				SendDlgItemMessage(hDlg, IDC_CHECK2, BM_SETCHECK, (WPARAM) BST_UNCHECKED, 0);
			}

			return TRUE;    
		
        case WM_COMMAND:
            switch (LOWORD (wParam)) {

				case IDC_BAUD:
					// Get the position of the selected item
					i = SendDlgItemMessage(hDlg, IDC_BAUD, CB_GETCURSEL, 0, 0);
					// Get the ItemData from Control ************
					nBaud = (DWORD)SendDlgItemMessage(hDlg, IDC_BAUD, CB_GETITEMDATA, i,0);

                    return TRUE;

				case IDC_DATABITS:
					// Get the position of the selected item
					i = SendDlgItemMessage(hDlg, IDC_DATABITS, CB_GETCURSEL, 0, 0);
					nDatabits = (UCHAR)SendDlgItemMessage(hDlg, IDC_DATABITS, CB_GETITEMDATA, i,0);
					//test it
					/*wsprintf(szOut, TEXT("%lu"), nDatabits);
					SetDlgItemText(hDlg, IDC_EDIT1, szOut);
					*/
                    return TRUE;
					
				case IDC_STOPBITS:
					// Get the position of the selected item
					i = SendDlgItemMessage(hDlg, IDC_STOPBITS, CB_GETCURSEL, 0, 0);
					nStopbits = (UCHAR)SendDlgItemMessage(hDlg, IDC_STOPBITS, CB_GETITEMDATA, i,0);

                    return TRUE;

				case IDC_PARITY:
					// Get the position of the selected item
					i = SendDlgItemMessage(hDlg, IDC_PARITY, CB_GETCURSEL, 0, 0);
					nParity = (UCHAR)SendDlgItemMessage(hDlg, IDC_PARITY, CB_GETITEMDATA, i,0);

                    return TRUE;
					
				case IDC_HANDSHAKE:
					// Get the position of the selected item
					i = SendDlgItemMessage(hDlg, IDC_HANDSHAKE, CB_GETCURSEL, 0, 0);
					nHandshake = (UCHAR)SendDlgItemMessage(hDlg, IDC_HANDSHAKE, CB_GETITEMDATA, i,0);

                    return TRUE;
				case IDC_CHECK1: //Send Char by Char
					i = SendDlgItemMessage(hDlg, IDC_CHECK1, BM_GETCHECK, 0, 0);
					if (i == BST_CHECKED){
						bsendcharbychar=true;
						EnableWindow(GetDlgItem(hDlg, IDC_Pre), FALSE);
						EnableWindow(GetDlgItem(hDlg, IDC_Post), FALSE);
					}
					else{
						bsendcharbychar=false;
						EnableWindow(GetDlgItem(hDlg, IDC_Pre), TRUE);
						EnableWindow(GetDlgItem(hDlg, IDC_Post), TRUE);
					}
					return true;
				case IDC_CHECK2: //BeepAfterRead
					i = SendDlgItemMessage(hDlg, IDC_CHECK2, BM_GETCHECK, 0, 0);
					if (i == BST_CHECKED)
						oBeepAfterRead=1;
					else
						oBeepAfterRead=0;
					return true;
                case IDOK:
					//read the com port
					if (GetDlgItemText(hDlg, IDC_COMPORT, szTxt, dim(szTxt)) != 0)
						wsprintf(g_szCOM, szTxt);

					//read the Preamble
					if (GetDlgItemText(hDlg, IDC_Pre, szTxt, dim(szTxt)) != 0)
						wsprintf(g_szPreamble, szTxt);
					//read the Postamble
					if (GetDlgItemText(hDlg, IDC_Post, szTxt, dim(szTxt)) != 0)
						wsprintf(g_szPostamble, szTxt);

					//BeepAfterRead setting
					g_dwBeepAfterRead=oBeepAfterRead;
					// Get the position of the selected item
					// i = SendDlgItemMessage(hDlg, IDC_BAUD, CB_GETCURSEL, 0, 0);
					// Get the name of the selected item
					// SendDlgItemMessage(hDlg, IDC_BAUD, CB_GETLBTEXT, i, (LPARAM)szOut);
					//i = lstrlen (szOut);
					wsprintf(szOut,TEXT("You selected: Port='%s', Baud=%lu, Databits=%lu, Stopbits=%lu, Parity=%lu, Handshake=%lu"),
										g_szCOM, nBaud,nDatabits,nStopbits,nParity,nHandshake);
					MessageBox(hDlg, szOut, TEXT("Information"),0);
					EndDialog (hDlg, 0);
                    return TRUE;

                case IDCANCEL:
					//restore old values
					nBaud=oBaud;
					nDatabits=oDatabits;
					nStopbits=oStopbits;
					nParity=oParity;
					nHandshake=oHandshake;
					bsendcharbychar=obsendcharbychar;
                    EndDialog (hDlg, 0);
                    return TRUE;
				default:
				return FALSE;
			}

		break;
    }
    return FALSE;
}

// Mesage handler for the About box.
LRESULT CALLBACK About(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam)
{
	RECT rt, rt1;
	int DlgWidth, DlgHeight;	// dialog width and height in pixel units
	int NewPosX, NewPosY;

	switch (message)
	{
		case WM_INITDIALOG:
			// trying to center the About dialog
			if (GetWindowRect(hDlg, &rt1)) {
				GetClientRect(GetParent(hDlg), &rt);
				DlgWidth	= rt1.right - rt1.left;
				DlgHeight	= rt1.bottom - rt1.top ;
				NewPosX		= (rt.right - rt.left - DlgWidth)/2;
				NewPosY		= (rt.bottom - rt.top - DlgHeight)/2;
				
				// if the About box is larger than the physical screen 
				if (NewPosX < 0) NewPosX = 0;
				if (NewPosY < 0) NewPosY = 0;
				SetWindowPos(hDlg, 0, NewPosX, NewPosY,
					0, 0, SWP_NOZORDER | SWP_NOSIZE);
			}
			return TRUE;
		case WM_COMMAND:
			if ((LOWORD(wParam) == IDOK) || (LOWORD(wParam) == IDCANCEL))
			{
				EndDialog(hDlg, LOWORD(wParam));
				return TRUE;
			}
			break;
		case WM_QUIT:
			PostQuitMessage (0);
			break;
	}
    return FALSE;
}

//======================================================================
// SendThread - Sends characters to the serial port
//
DWORD WINAPI SendThread (PVOID pArg) {
    HWND hWnd;
	//HGO , hwndSText;
    INT rc;
	DWORD cBytes;
    TCHAR szText[TEXTSIZE];

	DEBUGMSG(1, (L"### Entering SendThread\r\n"));
    hWnd = (HWND)pArg;
    //HGO hwndSText = GetDlgItem (hWnd, ID_SENDTEXT);
    while (1) {
        rc = WaitForSingleObject (g_hSendEvent, INFINITE);
        if (rc == WAIT_OBJECT_0) {
            if (!g_bContinue)
                break;
			// Disable send button while sending
			//HGO EnableWindow (GetDlgItem (hWnd, ID_SENDBTN), FALSE);
            //HGO GetWindowText (hwndSText, szText, dim(szText));
            lstrcat (szText, TEXT ("\r\n"));
            rc = WriteFile (g_hCommPort, szText, 
			               lstrlen (szText)*sizeof (TCHAR), &cBytes, 0);
			if (rc) 
			{
				// Copy sent text to output window 
				//HGO SendDlgItemMessage (hWnd, ID_RCVTEXT, EM_REPLACESEL, 0, (LPARAM)TEXT (" >"));
	            //HGO SetWindowText (hwndSText, TEXT (""));  // Clear text box
			}
			else 
			{
				// Else, print error message
				wsprintf (szText, TEXT ("Send failed rc=%d\r\n"), GetLastError());
				DEBUGMSG(1, (szText));
			// Put text in receive text box
			//HGO SendDlgItemMessage (hWnd, ID_RCVTEXT, EM_REPLACESEL, 0, (LPARAM)szText);
			//HGO EnableWindow (GetDlgItem (hWnd, ID_SENDBTN), TRUE);
			}
        } else
            break;
    }
	DEBUGMSG(1, (L"### Leaving SendThread\r\n"));
    return 0;
}

void showYellowIcon()
{
		//Notification icon
	HICON hIcon;
	hIcon=(HICON) LoadImage (hInst, MAKEINTRESOURCE (IWEDGE_WAITING), IMAGE_ICON, 16,16,0);
	nid.cbSize = sizeof (NOTIFYICONDATA);
	//nid.hWnd = hWnd;
	//nid.uID = 1;
	//nid.uFlags = NIF_ICON | NIF_MESSAGE;
	// NIF_TIP not supported    
	//nid.uCallbackMessage = MYMSG_TASKBARNOTIFY;
	nid.hIcon = hIcon;
	//nid.szTip[0] = '\0';
	BOOL res = Shell_NotifyIcon(NIM_MODIFY, &nid);
}

void showGreenIcon()
{
		//Notification icon
	HICON hIcon;
	hIcon=(HICON) LoadImage (hInst, MAKEINTRESOURCE (IWEDGE_STARTED), IMAGE_ICON, 16,16,0);
	nid.cbSize = sizeof (NOTIFYICONDATA);
	//nid.hWnd = hWnd;
	//nid.uID = 1;
	//nid.uFlags = NIF_ICON | NIF_MESSAGE;
	// NIF_TIP not supported    
	//nid.uCallbackMessage = MYMSG_TASKBARNOTIFY;
	nid.hIcon = hIcon;
	//nid.szTip[0] = '\0';
	BOOL res = Shell_NotifyIcon(NIM_MODIFY, &nid);
}

void showRedIcon()
{
		//Notification icon
	HICON hIcon;
	hIcon=(HICON) LoadImage (hInst, MAKEINTRESOURCE (IDI_COMMERROR), IMAGE_ICON, 16,16,0);
	nid.cbSize = sizeof (NOTIFYICONDATA);
	//nid.hWnd = hWnd;
	//nid.uID = 1;
	//nid.uFlags = NIF_ICON | NIF_MESSAGE;
	// NIF_TIP not supported    
	//nid.uCallbackMessage = MYMSG_TASKBARNOTIFY;
	nid.hIcon = hIcon;
	//nid.szTip[0] = '\0';
	BOOL res = Shell_NotifyIcon(NIM_MODIFY, &nid);
}

//
//	OpenCOMM(szPort)
//
//	opens the COM port and fills the global COM handle
//
void OpenCOMM(TCHAR *szPort)
{
	g_hCommPort = CreateFile (szPort, // Port Name (Unicode compatible)
			GENERIC_READ | GENERIC_WRITE, // Open for Read-Write
            0,             // COM port cannot be shared
            NULL,          // Always NULL for Windows CE
            OPEN_EXISTING, // For communication resource
            0,             // Non-overlapped operation only
            NULL);         // Always NULL for Windows CE
	if(g_hCommPort == INVALID_HANDLE_VALUE)
	{
		ReportCommError(_T("Opening Comms Port."));
		showRedIcon();
		return;
	}
	// set the timeouts to specify the behavor of reads and writes.
	COMMTIMEOUTS ct;
	ct.ReadIntervalTimeout = MAXDWORD; 
	ct.ReadTotalTimeoutMultiplier = 0; 
    ct.ReadTotalTimeoutConstant = 0; 
    ct.WriteTotalTimeoutMultiplier = 10; 
    ct.WriteTotalTimeoutConstant = 1000; 
	if(!SetCommTimeouts(g_hCommPort, &ct))
	{
		ReportCommError(_T("Setting comm. timeouts."));
		CloseCOMM(); // close comm port
		return;
	}
	// get the current communications parameters, and configure baud rate
	DCB dcb;
	dcb.DCBlength = sizeof(DCB);
	if(!GetCommState(g_hCommPort, &dcb))
	{
		ReportCommError(_T("Getting Comms. State."));
		CloseCOMM(); // close comm port
		return;
	}
	
	dcb.BaudRate =  nBaud; //CBR_19200;		// set baud rate to 19,200

	switch (nHandshake)
	{
	case 1: //xon/xoff
			//RTS/DTR Hardware handshake
			dcb.fRtsControl     = RTS_CONTROL_DISABLE;
			dcb.fDtrControl     = DTR_CONTROL_DISABLE;
			dcb.fOutxDsrFlow    = FALSE;
			dcb.fOutxCtsFlow	= FALSE;
			dcb.fDsrSensitivity = FALSE;
			// XON/XOFF software handshake
			dcb.fOutX           = TRUE; // no XON/XOFF control
			dcb.fInX            = TRUE;

		break;
	case 2: //hardware
			//RTS/DTR Hardware handshake
			dcb.fRtsControl     = RTS_CONTROL_HANDSHAKE;
			dcb.fDtrControl     = DTR_CONTROL_HANDSHAKE;
			dcb.fOutxDsrFlow    = TRUE;
			dcb.fOutxCtsFlow	= TRUE;
			dcb.fDsrSensitivity = TRUE;
			// XON/XOFF software handshake
			dcb.fOutX           = FALSE; // no XON/XOFF control
			dcb.fInX            = FALSE;

		break;
	case 3:
			// no handshake
			dcb.fRtsControl     = RTS_CONTROL_DISABLE;
			dcb.fDtrControl     = DTR_CONTROL_DISABLE;
			dcb.fOutxDsrFlow    = FALSE;
			dcb.fOutxCtsFlow	= FALSE;
			dcb.fDsrSensitivity = FALSE;
			// XON/XOFF software handshake
			dcb.fOutX           = FALSE; // no XON/XOFF control
			dcb.fInX            = FALSE;
		break;
	}
/*
	//RTS/DTR Hardware handshake
	dcb.fRtsControl     = RTS_CONTROL_HANDSHAKE;
	dcb.fDtrControl     = DTR_CONTROL_ENABLE;
	dcb.fOutxDsrFlow    = FALSE;
	dcb.fOutxCtsFlow	= TRUE;
	// XON/XOFF software handshake
	dcb.fOutX           = FALSE; // no XON/XOFF control
	dcb.fInX            = FALSE;
*/
	dcb.ByteSize        = nDatabits; // 8;
	dcb.Parity          = nParity; //NOPARITY;
	dcb.StopBits        = nStopbits; //ONESTOPBIT;

	if(!SetCommState(g_hCommPort, &dcb))
	{
		ReportCommError(_T("Setting Comms. State."));
		CloseCOMM(); // close comm port
		return;
	}

	DEBUGMSG(1, (L"COM port opened. Starting thread now...\r\n"));
	WriteCOM(L"hello\r\n");
	showGreenIcon();
	
	// now need to create the thread that will be reading the comms port
	DWORD dwThreadID=0;
	g_hCommReadThread = CreateThread(NULL, 0, CommReadThreadFunc, NULL, 0, &dwThreadID);
	if(g_hCommReadThread == NULL)	
	{
		ReportCommError(_T("Creating Thread."));
		CloseCOMM(); // close comm port
		return;
	}
	else{
		nclog(L"Created CommThread with id=0x%x\r\n", dwThreadID);
	}
}

//
// reports any errors encountered in these communications functions.
//
void ReportCommError(LPTSTR lpszMessage)
{
	TCHAR szBuffer[200];
	DWORD lastErr=GetLastError();

	wsprintf(szBuffer, _T("Communications Error %d \r\n%s"), 
				lastErr,
				lpszMessage);
	DEBUGMSG (1, (szBuffer));
	ShowError(lastErr);
}

// Thread function to read communications port. This thread is responsible
// for reading any incoming text and adding it to the text box.
// reads byte by byte until \n
DWORD WINAPI CommReadThreadFunc(LPVOID lpParam)
{
	DWORD dwBytesRead;
	char szSentence[1000], c;
	TCHAR szwcsSentence[1000];
	BYTE nc = 0;
	SetThreadPriority(GetCurrentThread(),
	THREAD_PRIORITY_BELOW_NORMAL);

	while(g_hCommPort != INVALID_HANDLE_VALUE)
	{
		DWORD dwCommModemStatus = EV_BREAK | EV_CTS | EV_DSR  | EV_ERR | EV_RLSD | EV_RXCHAR | EV_RXFLAG;
		//setup comm wait events
		SetCommMask(g_hCommPort, dwCommModemStatus);
		// Wait for an event to occur for the port.
		WaitCommEvent (g_hCommPort, &dwCommModemStatus, 0);		//blocks until event state changes

		// Re-specify the set of events to be monitored for the port.
		SetCommMask (g_hCommPort, EV_RXCHAR | EV_CTS | EV_DSR | EV_RING);

		if (dwCommModemStatus & EV_RXCHAR) {
			do{
				if(!ReadFile(g_hCommPort, &c, 1, &dwBytesRead, NULL))
				{
					ReportCommError(_T("Reading comms port."));
					return 0;
				}
				if(!bsendcharbychar)
				{
					if( (c == '\n') || nc==1000 ) // LF marks end of sentence
					{
						//add the return char
						szSentence[nc++] = c; //sprintf(szSentence, "%s", c); //
						// add term char
						szSentence[nc] = '\0';
						//send the keys
						SendKeys(szSentence);
						mbstowcs(szwcsSentence, szSentence, 1000);
						DEBUGMSG(1, (L"received: %s\r\n", szwcsSentence));
						nc = 0;
						sprintf(szSentence,"");
					}
					else
						szSentence[nc++] = c;
				}
				else
				{
					SendKeys(&c);
				}//bsendcharbychar
			}while (dwBytesRead==1);
		}//dwCommModemStatus
	}//while
	return 0;
}

// *** Listing 9.2
//
// Closes comm serial port.
//
// this will also stop threads waiting for CommEvents, 
// as the handle gets inavlid
// 
void CloseCOMM()
{
//	DeleteCriticalSection(&g_cs);
	if(g_hCommPort != INVALID_HANDLE_VALUE)
	{
		CloseHandle(g_hCommPort);
		g_hCommPort = INVALID_HANDLE_VALUE;
		DEBUGMSG(1, (L"Com. port closed"));
	}
	else
		DEBUGMSG(1, (L"Com. port %s was not open\r\n", g_szCOM));
	showRedIcon();
}

// *** Listing 9.3
//
// Write text to serial port.
// 
void WriteCOM(TCHAR *txt)
{
	DWORD dwBytesToWrite;
	DWORD dwBytesWritten;

	TCHAR szwcsBuffer[BUFF_SIZE];
	char szBuffer[BUFF_SIZE];

	wsprintf(szwcsBuffer, txt);

	// convert to ANSI character set
	dwBytesToWrite = wcstombs(szBuffer, szwcsBuffer, BUFF_SIZE);
	// append a carrage return/line feed pair
	//szBuffer[dwBytesToWrite++] = '\r';
	//szBuffer[dwBytesToWrite++] = '\n';
	if(!WriteFile(g_hCommPort,	// where to write to (the open comm port)
			szBuffer,			// what to write
			dwBytesToWrite,		// number of bytes to be written to port
			&dwBytesWritten,	// number of bytes that were actually written
			NULL))				// overlapped I/O not supported			
	{
		ReportCommError(_T("Sending text."));
		return;
	}
	else{
		if(g_dwBeepAfterRead>0)
			MessageBeep(MB_OK);
	}
}

//
// showError(LONG er)
//
// will show error text for err number 'er'
//
void showError(LONG er)
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
#ifdef DEBUG
	DEBUGMSG( 1, ( temp ) ); 
#else
	// Display the string.
	RETAILMSG(1, (temp));
	//MessageBox( NULL, (LPCTSTR)lpMsgBuf, L"Error", MB_OK | MB_ICONINFORMATION );
#endif
	// Free the buffer.
	LocalFree( lpMsgBuf );
}

// for future use
// simulate keystrokes by synthesizing the real world :-)
//
void SendKeysToWnd(char * szTxt)
{
	/* VK_0 thru VK_9 are the same as ASCII '0' thru '9' (0x30 - 0x39) */
	/* VK_A thru VK_Z are the same as ASCII 'A' thru 'Z' (0x41 - 0x5A) */
	DEBUGMSG(1, (L"\r\nSending Key...\r\n"));
	HWND hTarget = FindWindow(NULL, L"Notes");
	if (hTarget == INVALID_HANDLE_VALUE)
	{
		DEBUGMSG(1, (L"SendKeysToWnd: Target Window not found.\r\n"));
		return;
	}
	for (uint i=0; i < strlen(szTxt); i++)
	{
		if ( ((char)szTxt[i] >= 0x20) && ((char)szTxt[i] < KTAB_SIZE) )
		{
			DEBUGMSG(1, (L"(0x%2x )", (char)szTxt[i]));
//#ifndef DEBUG
			//for WM_KEYxxx, WM_CHAR the lParam has the following flags
			//Bit 0-15 = repeat count, set to 1
			//  always 1 for 1 for WM_KEYUP
			//  always one for a WM_KEYUP message
			//bit 16-23 = hardware scan code, dont used, set to 0
			//bit 24 = extended key, set to 1 for ALT, CTRL
			//bit 25-28 = not used, reserved
			//Bit 29 = context code, 
			//  set to 1 for ALT key pressed
			//  always 0 for KEYDOWN
			//  always 0 for a WM_KEYUP message
			//Bit 30 = previous key state, set to 1 if key previously down
			//  WM_KEYDOWN: the value is 1 if the key is down before the message is sent, or it is 0 if the key is up.
			//  the value is always 1 for a WM_KEYUP message.
			//Bit 31 = transition state, set to 1 if key is being released 
			//  always 0 for a WM_KEYDOWN message.
			//  always 1 for a WM_KEYUP message
			/*	// possible other funtion to send keys
				BOOL PostKeybdMessage (HWND hwnd, UINT VKey, 
									   KEY_STATE_FLAGS KeyStateFlags, 
									   UINT cCharacters, UINT *pShiftStateBuffer, 
									   UINT *pCharacterBuffer );
				This function sends a series of keys to the specified window. 
				The hwnd parameter is the target window. 
				This window must be owned by the calling thread. 
				The VKey parameter should be zero. KeyStateFlags specifies 
				the key state for all the keys being sent. The 
				cCharacters parameter specifies the number of keys being sent. 
				The pShiftStateBuffer parameter points to an array that 
				contains a shift state for each key sent, while pCharacterBuffer 
				points to the VK codes of the keys being sent. 
				Unlike keybd_event, this function doesn’t change 
				the global state of the keyboard.
			*/
			/*
			press key '1'
			KEYDOWN	00000031	00160001
			CHAR	1(0x31)		00160001
			KEYUP	00000031	c0160001

			press key 'a'
			KEYDOWN	00000041	001c0001
			CHAR	A(0x41)		001c0001
			KEYUP	00000041	c01c0001

			0x01000000	extended code? 
			0x20000000	alt pressed? 
			0x40000000	previously pressed?
			0x80000000	now pressed?

			0x0000yyyy	repeat count

			so a capital A becomes:

			KEYDOWN VK_SHIFT 0x00000001
			KEYDOWN VK_A	 0x00000001
			CHAR	A		 0x00000001
			KEYUP	VK_A	 0x80000001
			KEYUP	VK_SHIFT 0xC0000001

			*/
			SendMessage(hTarget, WM_KEYDOWN, (WPARAM)(char)szTxt[i], (LPARAM)0);
			keybd_event((char)szTxt[i], 0, KEYEVENTF_KEYDOWN | KEYEVENTF_SILENT, 0); 
			keybd_event((char)szTxt[i], 0, KEYEVENTF_KEYUP | KEYEVENTF_SILENT, 0); 
			Sleep(100);
//#endif
		}
	}
	DEBUGMSG(1, (L"\r\n"));
}

//	convert a input string with control codes
//	to a string with \r, \n\, \t encoding
TCHAR* stringEncoded(TCHAR * szSringIn){

	static TCHAR* sRet = new TCHAR[1000];
	wsprintf(sRet, L"");
	int i=0;
	while(szSringIn[i] != L'\0'){
		if(szSringIn[i] == L'\r')
			wcscat(sRet, L"\\r");
		else if(szSringIn[i] == L'\n')
			wcscat(sRet, L"\\n");
		else if(szSringIn[i] == L'\t')
			wcscat(sRet, L"\\t");
		else
		{
			sRet[i] = szSringIn[i];
			//wcsncat(sRet, szSringIn[i], 1);
		}
	}
	sRet[i] = L'\0';
	return sRet; 
}

//	convert a input string with \r, \n\, \t encoding
//	to a string with these vals decoded
TCHAR* stringDecoded(TCHAR * szSringIn){

	static TCHAR* sRet = new TCHAR[1000];
	wsprintf(sRet, L"");
	int i=0;

	tstring cString(szSringIn);

	while (cString.find(L"\\r")!=string::npos){
		cString.replace(cString.find(L"\\r"), 2,  L"\r");
	}
	while(cString.find(L"\\n")!=string::npos)
		cString.replace(cString.find(L"\\n"), 2,  L"\n");

	while(cString.find(L"\\t")!=string::npos)
		cString.replace(cString.find(L"\\t"), 2,  L"\t");

	wsprintf(sRet, L"%s", cString.c_str());

	return sRet; 
}

//
// SendKeys (szTxt)
//
// sends the keystrokes for szTxt
//
// invoked by ReadCommThread
//
void SendKeys(char * szTxt)
{
	/* VK_0 thru VK_9 are the same as ASCII '0' thru '9' (0x30 - 0x39) */
	/* VK_A thru VK_Z are the same as ASCII 'A' thru 'Z' (0x41 - 0x5A) */
	/* small ASCII letters are 'a' thru 'z', 0x61 - 0x7A */
	DEBUGMSG(1, (L"\r\nSendKeys: Sending Key...\r\n"));
	byte bCode;
	byte vCode;
	bool bShift;
	TCHAR szwTxt[1000];
	char  sTempA[1000];
	TCHAR sTempW[1000];

	//to remember, only convert once
	//g_szPreamble and g_szPostamble are the untranslated strings
	//get the decoded string
	TCHAR *szPostamble = new TCHAR[1000];
	TCHAR *szPreamble = new TCHAR[1000];
	szPreamble = stringDecoded(g_szPreamble);
	szPostamble = stringDecoded(g_szPostamble);
	DEBUGMSG(1, (L"Preamble:  g_szPreamble ='%s', decoded='%s'\r\n", g_szPreamble, szPreamble));
	DEBUGMSG(1, (L"Postamble: g_szPostamble='%s', decoded='%s'\r\n", g_szPostamble, szPostamble));

	//using a temp copy of what to send
	strcpy(sTempA, szTxt);
	mbstowcs(szwTxt, sTempA, 1000);

	if(!bsendcharbychar){
		//add the Postamble if any
		if(wcslen(szPostamble)>0 && wcslen(szPostamble)==0)
			wsprintf(sTempW, L"%s%s", szwTxt, szPostamble);
		//add the Preamble if any
		if(wcslen(szPreamble)>0 && wcslen(szPreamble)==0)
			wsprintf(sTempW, L"%s%s", szPreamble, szwTxt);
		//add post and preamble
		if(wcslen(szPostamble)>0 && wcslen(szPreamble)>0)
			wsprintf(sTempW, L"%s%s%s", szPreamble, szwTxt, szPostamble);
		//no Pre and Postamble
		if(wcslen(szPostamble)==0 && wcslen(szPostamble)==0)
			wsprintf(sTempW, L"%s", szwTxt);
			
		wcstombs(sTempA, sTempW, 1000);
	}
	else
		sprintf(sTempA, "%s", szTxt);

	mbstowcs(szwTxt, sTempA, 1000);
	DEBUGMSG(1, (L"processing '%s'\n", szwTxt));

	for (uint i=0; i < strlen(sTempA); i++)
	{
		//chars ' ' thru z
		//WARNING only 0-9, a-z is equal to VK_0-VK_9 and VK_A-VK_Z
		//the REST must be translated! ie 
		//ASCII ';' 0x3B to VK_SEMICOLON 0xBA !!!!!!!!!
		//ASCII '$' to VK_SHIFT + VK_4 !!!!!!!!
		//lookup for the VK code to send
		bCode=(char)sTempA[i];
		vCode=vkTable[bCode].kVKval;
		bShift=vkTable[bCode].kShift;
		if ((char)sTempA[i] < KTAB_SIZE)  // ( ((char)szTxt[i] >= 0x30) && ((char)szTxt[i] <= 0x7F) )
		{
			DEBUGMSG(1, (L"Char=(0x%2x)\tbCode=%2x\tvCode=%2x\tShift=%2x\r\n", (char)sTempA[i], bCode, vCode, bShift));
#ifndef DEBUG1
			if (bShift) //has to be shifted?
				keybd_event(VK_SHIFT, 0, KEYEVENTF_KEYDOWN | KEYEVENTF_SILENT, 0); 

			//send the code with key down and then key up
			keybd_event(vCode, 0, KEYEVENTF_KEYDOWN | KEYEVENTF_SILENT, 0); 
			keybd_event(vCode, 0, KEYEVENTF_KEYUP | KEYEVENTF_SILENT, 0); 
			DEBUGMSG(1, (L"keybd_event: %c\r\n", vCode));

			if (bShift) //has to be unshifted?
				keybd_event(VK_SHIFT, 0, KEYEVENTF_KEYUP | KEYEVENTF_SILENT, 0); 

#else
			DEBUGMSG(1, (L"keybd_event: %c\r\n", vCode));
#endif
			Sleep(10);
		}
	}
	if(!bsendcharbychar){
		if(g_dwBeepAfterRead>0)
			MessageBeep(MB_OK);
	}

	delete(szPostamble);
	delete(szPreamble);
	DEBUGMSG(1, (L"\r\n"));
}

//
// Thread to retry openCOMM(szPrt)
DWORD WINAPI CommWatchdog(LPVOID lpParam)
{
	TCHAR sComPort[32];
	wsprintf(sComPort, (TCHAR *)lpParam);
	DEBUGMSG(1, (L"### CommWatchdog started with %s\r\n", sComPort));
	nclog(L"### CommWatchdog started with %s\r\n", sComPort);
	while(g_bContinue)
	{
		if(g_hCommPort==INVALID_HANDLE_VALUE){
			DEBUGMSG(1, (L"### CommWatchdog trying to open comm port\r\n"));
			nclog(L"### CommWatchdog trying to open comm port\r\n");
			OpenCOMM(sComPort);
		}
		else{
			DEBUGMSG(1, (L"### CommWatchdog comm port is already open\r\n"));
			nclog(L"### CommWatchdog comm port is already open\r\n");
		}
		Sleep(3000);
	}
	DEBUGMSG(1, (L"### CommWatchdog ended\r\n"));
	nclog(L"### CommWatchdog ended\r\n");
	return 0;
}

//for testing
bool LaunchNotes(HWND hNotes)
{

	HWND hTarget = FindWindow(L"Notes", L"Notes");
	hNotes=NULL;
	if (hTarget == NULL)
	{
		PROCESS_INFORMATION pi;
		int rc;
		TCHAR *szFileName = L"\\Windows\\Notes.exe";
		TCHAR *szCmdLine = L"\\notes.txt";
		DWORD dwCreationFlags = 0;
		rc = CreateProcess (szFileName, szCmdLine, NULL, NULL, FALSE, 
							dwCreationFlags, NULL, NULL, NULL, &pi);
		if (rc) {
			CloseHandle (pi.hThread);
			CloseHandle (pi.hProcess);
			hTarget = FindWindow(NULL, L"Notes");
			hNotes = hTarget;
			return true;
		}		
		else
			return false;
	}
	else
	{
		hNotes = hTarget;
		return true;
	}
}

void suspendCOMM(void)
{
	nclog(L"### suspendCOMM\r\n");
	SuspendThread(g_hThreadWatchdog);
	SuspendThread(g_hCommReadThread);
	CloseCOMM();
	showYellowIcon();
}

void resumeCOMM(void)
{
	nclog(L"### resumeCOMM\r\n");
	OpenCOMM(g_szCOM);
	ResumeThread(g_hCommReadThread);
	ResumeThread(g_hThreadWatchdog);
	//showGreenIcon();
}

//////////////////////////////////////////////////////////////////////////////
// FUNCTION: InWideMode()
//
// PURPOSE: returns true if there is not enough space to display the 
//    crossword in "tall mode".
//
BOOL InWideMode()
{
    int height = GetSystemMetrics(SM_CYSCREEN);
	int width  = GetSystemMetrics(SM_CXSCREEN);
	if (width > height)
		return TRUE;
	else
		return FALSE;
    //return (height < SCALEY(320)) ? TRUE : FALSE;
}
