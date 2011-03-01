// PingAlertToast.cpp : Defines the entry point for the application.
//

#include "stdafx.h"
#include "PingAlertToast.h"


#define MAX_LOADSTRING 100

// Global Variables:
HINSTANCE			g_hInst;			// current instance
HWND				g_hWndMenuBar;		// menu bar handle
SHNOTIFICATIONDATA	g_shNotificationData = { 0 };
HWND				g_hWnd=NULL;
const int			g_NotificationID=0x57D7;

// GUID for the app {85EE47B2-57D7-EEFE-8E7A-36480443D062}
static const GUID guidNotifyApp = 
{ 0x85ee47b2, 0x57d7, 0xEEFE, { 0x8e, 0x7a, 0x36, 0x48, 0x4, 0x43, 0xd0, 0x62 } };
UINT				g_uiNotifyIDCount = 12540;					// id base for notify events
const DWORD	g_dwNotificationCmdID1 = WM_USER + 100;
const DWORD g_dwNotificationCmdID2 = g_dwNotificationCmdID1 + 1;

int showNotification(TCHAR* szText){
	LRESULT lRes=0;
    // This code will add an SHNotification notificaion 
    SHNOTIFICATIONDATA sn  = g_shNotificationData;
    sn.cbStruct = sizeof(sn);
    sn.dwID = g_NotificationID;
    sn.npPriority = SHNP_INFORM;
    sn.csDuration = -1;
    sn.hicon = LoadIcon(g_hInst, MAKEINTRESOURCE(IDI_SAMPLEICON));
    sn.clsid = guidNotifyApp;
    sn.grfFlags = 0;
    sn.pszTitle = TEXT("PingAlert");
	sn.pszHTML = TEXT("<html><body>There was at least one error. Please click <a href=\"file:///Windows\\PingAlertReport.html\">PingAlert Report</a> for more details.</body></html>");
    sn.rgskn[0].pszTitle = TEXT("Dismiss");
    sn.rgskn[0].skc.wpCmd = g_dwNotificationCmdID1;
	sn.rgskn[0].skc.grfFlags = NOTIF_SOFTKEY_FLAGS_STAYOPEN;
    sn.rgskn[1].pszTitle = TEXT("Hide");
    sn.rgskn[1].skc.wpCmd = g_dwNotificationCmdID2;
	sn.rgskn[1].skc.grfFlags = NOTIF_SOFTKEY_FLAGS_HIDE;
	sn.hwndSink=g_hWnd;
    //Add the notification to the tray
    lRes = SHNotificationAdd(&sn);

    //Put the data from an existing notification into a second SHNOTIFICATIONDATA struct
//    SHNotificationUpdate(SHNUM_TITLE | SHNUM_HTML | SHNUM_ICON | SHNUM_SOFTKEYCMDS | SHNUM_SOFTKEYS, &sn2);
    //Remove the notification from the tray
/*
	SHNotificationRemove(&guidNotifyApp, 1);
    //Add a new notification that utilizes the MRE functionality
    sn.cbStruct = sizeof(sn);
    sn.dwID = 1;
    sn.npPriority = SHNP_INFORM;
    sn.csDuration = 15;
    sn.hicon = LoadIcon(g_hInstance, MAKEINTRESOURCE(IDI_SAMPLEICON));
    sn.clsid = CLSID_SHNAPI_Test;
    sn.grfFlags = SHNF_STRAIGHTTOTRAY;
    sn.pszTodaySK = TEXT("New Task");
    sn.pszTodayExec = TEXT("\\windows\\tasks.exe");
    //Add the notification to the tray
    SHNotificationAdd(&sn);
*/
	return lRes;
}

void FreeNotificationData(SHNOTIFICATIONDATA * pnd)
{
    //Remove the notification from the tray
    LRESULT lRes = SHNotificationRemove(&guidNotifyApp, g_NotificationID);
	if(lRes!=ERROR_SUCCESS){
		DEBUGMSG(1,(L"Error in SHNotificationRemove: lRes=%u\r\n", lRes));
	}

	if(pnd!=NULL){
		LocalFree((HLOCAL)pnd->pszHTML);
		LocalFree((HLOCAL)pnd->pszTitle);
		LocalFree((HLOCAL)pnd->pszTodaySK);
		LocalFree((HLOCAL)pnd->pszTodayExec);
		if (pnd->grfFlags & SHNF_HASMENU)
		{
			LocalFree((HLOCAL)pnd->skm.prgskc);
		}
		else
		{
			LocalFree((HLOCAL)pnd->rgskn[0].pszTitle);
			LocalFree((HLOCAL)pnd->rgskn[1].pszTitle);
		}
	}
}

void showHTML(TCHAR* szLink){
	SHELLEXECUTEINFO cShellExecuteInfo = {0};
	cShellExecuteInfo.cbSize = sizeof(SHELLEXECUTEINFO);
	cShellExecuteInfo.fMask = SEE_MASK_NOCLOSEPROCESS;
	cShellExecuteInfo.hwnd = NULL;
	cShellExecuteInfo.lpVerb = L"Open";
	cShellExecuteInfo.lpFile = szLink;
	cShellExecuteInfo.nShow = SW_SHOWNORMAL;
	//try to start 
	BOOL bRes = ShellExecuteEx(&cShellExecuteInfo);
	if(!bRes){
		DEBUGMSG(1, (L"ShellExecuteEx failed for %s, %u\r\n", szLink, GetLastError()));
		//try with createprocess
		PROCESS_INFORMATION pi;
		if(CreateProcess(L"\\windows\\iexplore.exe", szLink, NULL, NULL, FALSE, 0, NULL, NULL, NULL, &pi)==0){
			DEBUGMSG(1,(L"CreateProcess failed with %u\r\n", GetLastError()));
		}
		else{
			CloseHandle(pi.hThread);
			CloseHandle(pi.hProcess);
		}
	}
}

// Forward declarations of functions included in this code module:
ATOM			MyRegisterClass(HINSTANCE, LPTSTR);
BOOL			InitInstance(HINSTANCE, int);
LRESULT CALLBACK	WndProc(HWND, UINT, WPARAM, LPARAM);
INT_PTR CALLBACK	About(HWND, UINT, WPARAM, LPARAM);

int WINAPI WinMain(HINSTANCE hInstance,
                   HINSTANCE hPrevInstance,
                   LPTSTR    lpCmdLine,
                   int       nCmdShow)
{
	MSG msg;

	// Perform application initialization:
	if (!InitInstance(hInstance, nCmdShow)) 
	{
		return FALSE;
	}

	HACCEL hAccelTable;
	hAccelTable = LoadAccelerators(hInstance, MAKEINTRESOURCE(IDC_PINGALERTTOAST));

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
	wc.hIcon         = LoadIcon(hInstance, MAKEINTRESOURCE(IDI_PINGALERTTOAST));
	wc.hCursor       = 0;
	wc.hbrBackground = (HBRUSH) GetStockObject(WHITE_BRUSH);
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
    LoadString(hInstance, IDC_PINGALERTTOAST, szWindowClass, MAX_LOADSTRING);

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
    if (g_hWndMenuBar)
    {
        RECT rc;
        RECT rcMenuBar;

        GetWindowRect(hWnd, &rc);
        GetWindowRect(g_hWndMenuBar, &rcMenuBar);
        rc.bottom -= (rcMenuBar.bottom - rcMenuBar.top);
		
        MoveWindow(hWnd, rc.left, rc.top, rc.right-rc.left, rc.bottom-rc.top, FALSE);
    }

    //ShowWindow(hWnd, nCmdShow);
	ShowWindow(hWnd, SW_MINIMIZE);
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
	TCHAR* szText = L"TEST PingAlert";
    static SHACTIVATEINFO s_sai;
	g_hWnd=hWnd;
/*
	typedef struct _NMSHN {
	  NMHDR hdr;
	  LPARAM lParam;
	  DWORD dwReturn;
		Union {
		  LPCTSTR pszLink;
		  BOOL fTimeout;
		  POINT pt;
		};
	} NMSHN;
*/
	NMSHN* notificationMsg;
	NMHDR  notificationHdr;
	TCHAR szLink[MAX_PATH]; //var to hold link text

    switch (message) 
    {
		case WM_NOTIFY:
			DEBUGMSG(1, (L"WM_NOTIFY\r\n"));
			if(wParam==g_NotificationID)
			{				
				notificationMsg = (NMSHN*) lParam;
				notificationHdr = notificationMsg->hdr;
				if(notificationHdr.code == SHNN_LINKSEL){
					//MessageBox(hWnd, L"PingAlert Notification", L"PingAlert Notification", MB_OK);
					wsprintf(szLink, L"%s", notificationMsg->pszLink);
					showHTML(szLink);
				}
				//either use WM_COMMAND or SHNotify
				/*
				if(notificationHdr.code==SHNN_DISMISS) // && notificationMsg->fTimeout==FALSE)
				{
					PostQuitMessage(2);
				}
				*/
			}
			break;
        case WM_COMMAND:
			DEBUGMSG(1, (L"WM_COMMAND\r\n"));
            wmId    = LOWORD(wParam); 
            wmEvent = HIWORD(wParam); 
			if(wmId==g_dwNotificationCmdID1)
			{		
				DEBUGMSG(1, (L"WM_COMMAND for notification menu Dismiss\r\n"));
				FreeNotificationData(&g_shNotificationData);
				PostQuitMessage(3);
			}
			else if(wmId==g_dwNotificationCmdID2)
			{		
				DEBUGMSG(1, (L"WM_COMMAND for notification menu Hide\r\n"));
			}
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
			DEBUGMSG(1, (L"WM_CREATE\r\n"));
            SHMENUBARINFO mbi;

            memset(&mbi, 0, sizeof(SHMENUBARINFO));
            mbi.cbSize     = sizeof(SHMENUBARINFO);
            mbi.hwndParent = hWnd;
            mbi.nToolBarId = IDR_MENU;
            mbi.hInstRes   = g_hInst;

            if (!SHCreateMenuBar(&mbi)) 
            {
                g_hWndMenuBar = NULL;
            }
            else
            {
                g_hWndMenuBar = mbi.hwndMB;
            }

            // Initialize the shell activate info structure
            memset(&s_sai, 0, sizeof (s_sai));
            s_sai.cbSize = sizeof (s_sai);

			//remove old notification
		    SHNotificationRemove(&guidNotifyApp, g_NotificationID);

			//show actual notification
			showNotification(szText);
			
            break;
        case WM_PAINT:
			DEBUGMSG(1, (L"WM_PAINT\r\n"));
            hdc = BeginPaint(hWnd, &ps);
            
            // TODO: Add any drawing code here...            
            EndPaint(hWnd, &ps);
			ShowWindow(hWnd, SW_MINIMIZE); // do not show window
            break;
		case WM_QUIT:
			DEBUGMSG(1, (L"WM_QUIT\r\n"));
			FreeNotificationData(&g_shNotificationData);
			break;
        case WM_DESTROY:
			DEBUGMSG(1, (L"WM_DESTROY\r\n"));
            CommandBar_Destroy(g_hWndMenuBar);
            PostQuitMessage(0);
            break;

        case WM_ACTIVATE:
			DEBUGMSG(1, (L"WM_ACTIVATE\r\n"));
            // Notify shell of our activate message
            SHHandleWMActivate(hWnd, wParam, lParam, &s_sai, FALSE);
            break;
        case WM_SETTINGCHANGE:
            SHHandleWMSettingChange(hWnd, wParam, lParam, &s_sai);
            break;
		case WM_CLOSE:
			DEBUGMSG(1, (L"WM_CLOSE\r\n"));
			FreeNotificationData(&g_shNotificationData);
			PostQuitMessage(1);
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
