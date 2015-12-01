// touchLockWEH.cpp : Defines the entry point for the application.
//

#include "stdafx.h"
#include "touchLockWEH.h"
#include <pm.h>

#include "hooks.h"

#include "reg.h"

#include <string>
std::wstring keyPresses;
std::wstring passkey = L"52401";
DWORD bkColor;

#define MAX_LOADSTRING 100

// Global Variables:
HINSTANCE			g_hInst;			// current instance
HWND				g_hWndMenuBar;		// menu bar handle
HWND				g_hWndMain;

DWORD g_dwThreadID;
HANDLE g_hThread;
BOOL g_bRunThread=TRUE;
DWORD g_timerID=401;
DWORD g_PowerDownTimerID=402;

// Forward declarations of functions included in this code module:
ATOM			MyRegisterClass(HINSTANCE, LPTSTR);
BOOL			InitInstance(HINSTANCE, int);
LRESULT CALLBACK	WndProc(HWND, UINT, WPARAM, LPARAM);
INT_PTR CALLBACK	About(HWND, UINT, WPARAM, LPARAM);

void disableSIP(HWND hWnd){
	HWND hSipWindow = FindWindow(L"MS_SIPBUTTON", L"MS_SIPBUTTON");
	ShowWindow(hSipWindow, SW_HIDE);
	return;
	if(SHSipPreference(hWnd, SIP_FORCEDOWN))
		DEBUGMSG(1, (L"SHSipPreference SIP_FORCEDOWN OK\n"));
	else
		DEBUGMSG(1, (L"SHSipPreference SIP_FORCEDOWN Failed: %i\n", GetLastError()));
}
void enableSIP(HWND hWnd){
	HWND hSipWindow = FindWindow(L"MS_SIPBUTTON", L"MS_SIPBUTTON");
	ShowWindow(hSipWindow, SW_SHOWNORMAL);
	return;
	if(SHSipPreference(hWnd, SIP_UNCHANGED))
		DEBUGMSG(1, (L"SHSipPreference for SIP_UNCHANGED OK\n"));
	else
		DEBUGMSG(1, (L"SHSipPreference SIP_UNCHANGED Failed: %i\n", GetLastError()));
}

DWORD WINAPI myThread(LPVOID lParam){
	HWND hWnd=(HWND) lParam;
	do{
		SetForegroundWindow(hWnd);
		Sleep(2000);
	}while(g_bRunThread);
	return 0;
}

void startPowerTimeOut(HWND hWnd){
	//start power timeout timer
	SetTimer(hWnd, g_PowerDownTimerID, 10000, NULL);
	DEBUGMSG(1, (L"power timeout started\n"));
}
void stopPowerTimeOut(HWND hWnd){
	KillTimer(hWnd, g_PowerDownTimerID);
	DEBUGMSG(1, (L"power timeout stopped\n"));
}

int WINAPI WinMain(HINSTANCE hInstance,
                   HINSTANCE hPrevInstance,
                   LPTSTR    lpCmdLine,
                   int       nCmdShow)
{
	MSG msg;

	passkey=regGetCode();
	bkColor=regGetBKColor();

	// Perform application initialization:
	if (!InitInstance(hInstance, nCmdShow)) 
	{
		return FALSE;
	}

	HACCEL hAccelTable;
	hAccelTable = LoadAccelerators(hInstance, MAKEINTRESOURCE(IDC_TOUCHLOCKWEH));

	// Main message loop:
	while (GetMessage(&msg, NULL, 0, 0)) 
	{
		if (!TranslateAccelerator(msg.hwnd, hAccelTable, &msg)) 
		{
			TranslateMessage(&msg);
			DispatchMessage(&msg);
		}
	}

	return (int) msg.wParam;
}

//
//  FUNCTION: MyRegisterClass()
//
//  PURPOSE: Registers the window class.
//
//  COMMENTS:
//
ATOM MyRegisterClass(HINSTANCE hInstance, LPTSTR szWindowClass)
{
	WNDCLASS wc;

	wc.style         = CS_HREDRAW | CS_VREDRAW;
	wc.lpfnWndProc   = WndProc;
	wc.cbClsExtra    = 0;
	wc.cbWndExtra    = 0;
	wc.hInstance     = hInstance;
	wc.hIcon         = LoadIcon(hInstance, MAKEINTRESOURCE(IDI_TOUCHLOCKWEH));
	wc.hCursor       = 0;
	HBRUSH hBkColor = CreateSolidBrush(RGB(HIWORD(bkColor), HIBYTE(LOWORD(bkColor)), LOBYTE(LOWORD(bkColor))));
	wc.hbrBackground = hBkColor;// (HBRUSH) GetStockObject(WHITE_BRUSH);
	wc.lpszMenuName  = 0;
	wc.lpszClassName = szWindowClass;

	return RegisterClass(&wc);
}

//
//   FUNCTION: InitInstance(HINSTANCE, int)
//
//   PURPOSE: Saves instance handle and creates main window
//
//   COMMENTS:
//
//        In this function, we save the instance handle in a global variable and
//        create and display the main program window.
//
BOOL InitInstance(HINSTANCE hInstance, int nCmdShow)
{
    HWND hWnd;
    TCHAR szTitle[MAX_LOADSTRING];		// title bar text
    TCHAR szWindowClass[MAX_LOADSTRING];	// main window class name

    g_hInst = hInstance; // Store instance handle in our global variable

    // SHInitExtraControls should be called once during your application's initialization to initialize any
    // of the device specific controls such as CAPEDIT and SIPPREF.
    SHInitExtraControls();

    LoadString(hInstance, IDS_APP_TITLE, szTitle, MAX_LOADSTRING); 
    LoadString(hInstance, IDC_TOUCHLOCKWEH, szWindowClass, MAX_LOADSTRING);

    //If it is already running, then focus on the window, and exit
    hWnd = FindWindow(szWindowClass, szTitle);	
    if (hWnd) 
    {
        // set focus to foremost child window
        // The "| 0x00000001" is used to bring any owned windows to the foreground and
        // activate them.
        SetForegroundWindow((HWND)((ULONG) hWnd | 0x00000001));
        return 0;
    } 

    if (!MyRegisterClass(hInstance, szWindowClass))
    {
    	return FALSE;
    }

    hWnd = CreateWindow(szWindowClass, szTitle, WS_VISIBLE,
        CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT, NULL, NULL, hInstance, NULL);

    if (!hWnd)
    {
        return FALSE;
    }

    // When the main window is created using CW_USEDEFAULT the height of the menubar (if one
    // is created is not taken into account). So we resize the window after creating it
    // if a menubar is present
    //if (g_hWndMenuBar)
    //{
    //    RECT rc;
    //    RECT rcMenuBar;

    //    GetWindowRect(hWnd, &rc);
    //    GetWindowRect(g_hWndMenuBar, &rcMenuBar);
    //    rc.bottom -= (rcMenuBar.bottom - rcMenuBar.top);
		
    //    MoveWindow(hWnd, rc.left, rc.top, rc.right-rc.left, rc.bottom-rc.top, FALSE);
    //}

    ShowWindow(hWnd, SW_MAXIMIZE);// nCmdShow);
    UpdateWindow(hWnd);

    return TRUE;
}

//
//  FUNCTION: WndProc(HWND, UINT, WPARAM, LPARAM)
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
    int wmId, wmEvent;
    PAINTSTRUCT ps;
    HDC hdc;
	HFONT hFontOld;

    static SHACTIVATEINFO s_sai;
	static TCHAR szInput[32];
	static RECT rect;
	static BOOL firstChar=TRUE;
	static TCHAR szStatus[MAX_PATH];
	static HANDLE hPowerState=NULL;
	int vOffset=0;

    GetClientRect(hWnd, &rect);
    switch (message) 
    {
		case WM_TIMER:
			if(wParam==g_timerID){
				DEBUGMSG(1, (L"Timer stopped!\n"));
				KillTimer(hWnd, g_timerID);
				wsprintf(szInput,L"");//empty string
				keyPresses=L"";
				firstChar=TRUE;
				wsprintf(szStatus, L"no key"); 
				InvalidateRect(hWnd, &rect, TRUE);
				UpdateWindow(hWnd);
				MessageBeep(MB_ICONERROR);

				startPowerTimeOut(hWnd);
			}
			if(wParam==g_PowerDownTimerID){
				DEBUGMSG(1, (L"Timer g_PowerDownTimerID...\n"));
				if(hPowerState==NULL){
					hPowerState = SetPowerRequirement(L"BKL1:", D3, POWER_NAME | POWER_FORCE, NULL, 0);
					if(hPowerState==NULL)
						DEBUGMSG(1, (L"SetPowerRequirement failed %i", GetLastError()));
					else{
						if(SetSystemPowerState(NULL, POWER_STATE_SUSPEND, POWER_FORCE)!=ERROR_SUCCESS)
							DEBUGMSG(1, (L"SetSystemPowerState failed %i", GetLastError()));
					}
				}
			}
			break;
		case WM_CHAR:
			stopPowerTimeOut(hWnd);
			switch (wParam) 
			{ 
				// First, handle non-displayable characters by beeping.
				case 0x08:  // backspace.
				case 0x09:  // tab.
				case 0x0A:  // linefeed.
				case 0x0D:  // carriage return.
				case 0x20:  // space.
					MessageBeep((UINT) -1); 
				break;
				case 0x1B:  // escape.
					PostMessage(hWnd, WM_TIMER, g_timerID, 0);	//reset input
					break;

				// Next, handle displayable characters by appending them to our string.
				default:
					keyPresses += (wchar_t) wParam;
					if(firstChar){
						//start RESET timer
						SetTimer(hWnd, g_timerID, 5000, NULL);
						firstChar=FALSE;
					}
					wsprintf(szStatus, L"%s", keyPresses.c_str());
					InvalidateRect(hWnd, &rect, TRUE);
					UpdateWindow(hWnd);

					DEBUGMSG(1, (L"cumulated input: %s\n", keyPresses.c_str()));
					if(keyPresses.length()>passkey.length()){
						wsprintf(szStatus, L"key too long");
						InvalidateRect(hWnd, &rect, TRUE);
						UpdateWindow(hWnd);
						Sleep(1000);
						PostMessage(hWnd, WM_TIMER, g_timerID, 0);	//reset input
						keyPresses=L"";
					}
					else if(keyPresses.compare(passkey)==0){
						KillTimer(hWnd, g_timerID);
						wsprintf(szStatus, L"KEY OK. UNLOCKING...");
						InvalidateRect(hWnd, &rect, TRUE);
						UpdateWindow(hWnd);
						Sleep(1000);
						DEBUGMSG(1, (L"correct input, will exit now...\n"));
						MessageBeep(MB_OK);
						DEBUGMSG(1, (L"Screen unlocked\n"));
						m_fpTouchUnregisterWindow(GetDesktopWindow());
						PostQuitMessage(1);
					}
			} 
			break;
        case WM_COMMAND:
            wmId    = LOWORD(wParam); 
            wmEvent = HIWORD(wParam); 
            // Parse the menu selections:
            switch (wmId)
            {
                case IDM_HELP_ABOUT:
                    DialogBox(g_hInst, (LPCTSTR)IDD_ABOUTBOX, hWnd, About);
                    break;
                case IDM_OK:
                    SendMessage (hWnd, WM_CLOSE, 0, 0);				
                    break;
                default:
                    return DefWindowProc(hWnd, message, wParam, lParam);
            }
            break;
        case WM_CREATE:
			wsprintf(szStatus, L"no key");
//            GetClientRect(hWnd, &rect);
			
			InitializeTouchDll();
			m_fpTouchRegisterWindow(GetDesktopWindow());

			startPowerTimeOut(hWnd);

			g_hThread = CreateThread(NULL, 0, myThread, &hWnd, CREATE_ALWAYS, &g_dwThreadID);

            // Initialize the shell activate info structure
            memset(&s_sai, 0, sizeof (s_sai));
            s_sai.cbSize = sizeof (s_sai);

            break;
        case WM_PAINT:
			LOGFONT lf;
			HFONT font;
			memset(&lf, 0, sizeof(LOGFONT));		// zero out structure
			lf.lfHeight = 36; //-MulDiv(36, GetDeviceCaps(hdc, LOGPIXELSY), 72); //12;						// request a 12-pixel-height font
			lf.lfWeight=FW_BOLD;
			wcscpy(lf.lfFaceName, L"Tahoma");        // request a face name

            hdc = BeginPaint(hWnd, &ps);
            
            // TODO: Add any drawing code here...
			font = CreateFontIndirect(&lf);
			hFontOld = (HFONT)SelectObject( hdc, font );
			
			rect.top+=100;
			vOffset+=DrawText(hdc, L"Screen locked.", -1, &rect, DT_CENTER | DT_TOP);
			rect.top+=2*vOffset;
			DrawText(hdc, szStatus, -1, &rect, DT_CENTER | DT_TOP); //| DT_VCENTER);
			rect.top+=2*vOffset;
			DrawText(hdc, L"Enter code to unlock.", -1, &rect, DT_CENTER | DT_TOP);//DT_BOTTOM);

			SelectObject(hdc, hFontOld);
            EndPaint(hWnd, &ps);
            break;
        case WM_DESTROY:
            CommandBar_Destroy(g_hWndMenuBar);
			if(hPowerState!=NULL)
				ReleasePowerRequirement(hPowerState);
			if(g_hThread!=NULL){
				g_bRunThread=false;
				Sleep(3000);
			}
            PostQuitMessage(0);
            break;

        case WM_ACTIVATE:
            // Notify shell of our activate message
            SHHandleWMActivate(hWnd, wParam, lParam, &s_sai, FALSE);
            break;
        case WM_SETTINGCHANGE:
            SHHandleWMSettingChange(hWnd, wParam, lParam, &s_sai);
            break;

        default:
            return DefWindowProc(hWnd, message, wParam, lParam);
    }
    return 0;
}

// Message handler for about box.
INT_PTR CALLBACK About(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message)
    {
        case WM_INITDIALOG:
            {
                // Create a Done button and size it.  
                SHINITDLGINFO shidi;
                shidi.dwMask = SHIDIM_FLAGS;
                shidi.dwFlags = SHIDIF_DONEBUTTON | SHIDIF_SIPDOWN | SHIDIF_SIZEDLGFULLSCREEN | SHIDIF_EMPTYMENU;
                shidi.hDlg = hDlg;
                SHInitDialog(&shidi);
            }
            return (INT_PTR)TRUE;

        case WM_COMMAND:
            if (LOWORD(wParam) == IDOK)
            {
                EndDialog(hDlg, LOWORD(wParam));
                return TRUE;
            }
            break;

        case WM_CLOSE:
            EndDialog(hDlg, message);
            return TRUE;

    }
    return (INT_PTR)FALSE;
}
