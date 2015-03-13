// ipconfig.cpp : Defines the entry point for the application.
//
#ifdef _MSC_VER
#    pragma comment(linker, "/subsystem:windowsce /ENTRY:WinMain")
#endif

#include "stdafx.h"
#include "ipconfig.h"

#include "ipconfig_src.h"
//extern int GetConfigData();
//extern void FlushDNSCache(void);
//extern void setOutputWindow(HWND hWndEdit);

#define MAX_LOADSTRING 100

// Global Variables:
HINSTANCE			g_hInst;			// current instance
HWND				g_hWndMain;
HWND				g_hWndMenuBar;		// menu bar handle
HWND				g_hWndEdit;			// Edit window handle
DWORD				g_selectedAdapter=0;	// for release and renew
TCHAR				g_InfoText[MAX_PATH];

// Forward declarations of functions included in this code module:
ATOM			MyRegisterClass(HINSTANCE, LPTSTR);
BOOL			InitInstance(HINSTANCE, int);
LRESULT CALLBACK	WndProc(HWND, UINT, WPARAM, LPARAM);
INT_PTR CALLBACK	About(HWND, UINT, WPARAM, LPARAM);
INT_PTR CALLBACK	InputDialog(HWND, UINT, WPARAM, LPARAM);

int WINAPI WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance,LPTSTR lpCmdLine,int nCmdShow)
{
	MSG msg;
	
	//begin arg parsing
	LPWSTR *szArgList=__wargv;
    int argCount = __argc;
//    szArgList = CommandLineToArgvW(GetCommandLine(), &argCount);
	for(int i = 0; i < argCount; i++)
    {
		DEBUGMSG(1, (L"%i: '%s'\n", szArgList[i]));
    }
	/*
	if(argCount>0){
		DWORD Address =0; 
		BOOL NextParamIsAddress=FALSE, Error=FALSE, Renew=FALSE, Release=FALSE, FlushDNS=FALSE;

		for (int i=1;i<argc;i++) {

			if ((argv[i][0]==_T('-')) || (argv[i][0] ==_T('/'))) {
				if (NextParamIsAddress) {
					Error=TRUE;
				}
				switch (toupper(argv[i][1])) {
			case _T('D') :
				if (_tcsicmp(&(argv[i][1]),_T("D"))!=0)
					Error=TRUE;
				else
					v_fDebugOut=TRUE;
				break;

			case _T('?') :
				usage();
				exit(1);
				break; 

			case _T('A'):
				if (_tcsicmp(&(argv[i][1]), _T("ALL"))==0)
					bflagall =TRUE;
				break;

			case _T('R'):
				if ( _tcsicmp(&(argv[i][1]),_T("RENEW"))==0) {
					Renew=TRUE;
					NextParamIsAddress=TRUE;
				}
				else if (_tcsicmp(&(argv[i][1]),_T("RELEASE"))==0) {
					Release =TRUE;
					NextParamIsAddress=TRUE; 
				}
				break;

			case _T('F'):
				if ( _tcsicmp(&(argv[i][1]),_T("FLUSHDNS"))==0) {
					FlushDNS = TRUE;
				}
				break;

			default:
				Error=TRUE;
				}
			}
			else if (NextParamIsAddress) {
				if (!(Address=_ttoi(argv[i]))) {
					Error=TRUE;
				}
				else
					break;  
			}
			else {
				Error=TRUE; 
			}

		}//End of for
		if ((Renew && Release) || (Error==TRUE)) {
			OutputMessage(TEXT("Incorrect Parameters\r\n"));
			exit(1);
		}

		WSADATA WsaData;
		if (WSAStartup(MAKEWORD(1,1), &WsaData) != 0)
		{
			OutputMessage(TEXT("WSAStartup failed (error %ld)\r\n"), GetLastError());
			return 0;
		}

		if (Release)
			ReleaseAddress(Address);
		else if (Renew)
			RenewAddress(Address);
		else if (FlushDNS) {
			FlushDNSCache();
		} else {
			GetConfigData();
		}

		WSACleanup();

		return(0);
	}//argcount>0
	*/
	//end cmdline parsing
	LocalFree(szArgList);

	// Perform application initialization:
	if (!InitInstance(hInstance, nCmdShow)) 
	{
		return FALSE;
	}

	HACCEL hAccelTable;
	hAccelTable = LoadAccelerators(hInstance, MAKEINTRESOURCE(IDC_IPCONFIG));

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
	wc.hIcon         = LoadIcon(hInstance, MAKEINTRESOURCE(IDI_IPCONFIG));
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
    LoadString(hInstance, IDC_IPCONFIG, szWindowClass, MAX_LOADSTRING);

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

	g_hWndMain=hWnd;

    ShowWindow(hWnd, nCmdShow);
    UpdateWindow(hWnd);


    return TRUE;
}

void saveText(){
	TCHAR fileName[MAX_PATH];
	wsprintf(fileName,L"ipconfig.txt");
	OPENFILENAME ofn;
	memset(&ofn, 0, sizeof(OPENFILENAME));
	ofn.lStructSize=sizeof(OPENFILENAME);
	ofn.hwndOwner=g_hWndMain;
	ofn.lpstrFile=fileName;
	ofn.nMaxFile=MAX_PATH;
	ofn.lpstrInitialDir=L"\\MyDocuments";
	ofn.lpstrTitle =L"Save text to file";
	ofn.Flags=OFN_OVERWRITEPROMPT | OFN_PATHMUSTEXIST;

	if (GetSaveFileName(&ofn)){
		HANDLE hFile;
		BOOL bSuccess = FALSE;

		hFile = CreateFile(ofn.lpstrFile, GENERIC_WRITE, 0, NULL, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
		if(hFile != INVALID_HANDLE_VALUE)
		{
			DWORD dwTextLength;

			dwTextLength = GetWindowTextLength(g_hWndEdit);
			// No need to bother if there's no text.
			if(dwTextLength > 0)
			{
				LPTSTR pszText;
				DWORD dwBufferSize = (dwTextLength + 1)*sizeof(TCHAR);

				pszText = (LPTSTR)GlobalAlloc(GPTR, dwBufferSize);
				if(pszText != NULL)
				{
					if(GetWindowText(g_hWndEdit, pszText, dwBufferSize))
					{
						DWORD dwWritten;

						if(WriteFile(hFile, pszText, dwTextLength*sizeof(TCHAR), &dwWritten, NULL))
							bSuccess = TRUE;
					}
					GlobalFree(pszText);
				}
			}
			CloseHandle(hFile);
		}
	}
	
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
	RECT rClient;
//	HWND hwndInput;

    static SHACTIVATEINFO s_sai;
	
    switch (message) 
    {
        case WM_COMMAND:
            wmId    = LOWORD(wParam); 
            wmEvent = HIWORD(wParam); 
            // Parse the menu selections:
            switch (wmId)
            {
                case IDM_HELP_ABOUT:
                    DialogBox(g_hInst, (LPCTSTR)IDD_ABOUTBOX, hWnd, About);
                    break;
				case IDM_COMMANDS_IPINFO:
					GetConfigData();
					break;
				case ID_COMMANDS_FLUSHDNS:
					FlushDNSCache();
					break;
				case ID_COMMANDS_RELEASE:
					wsprintf(g_InfoText, L"please enter the adapter to release:");
					//hwndInput = CreateDialogParam(g_hInst, (LPCTSTR)IDD_GetInput, hWnd, InputDialog, (LPARAM)L"please enter the adapter to release:");
					//ShowWindow(hwndInput, SW_SHOWNA);
					if(DialogBox(g_hInst, (LPCTSTR)IDD_GetInput, hWnd, InputDialog)==IDOK)
						ReleaseAddress(g_selectedAdapter);
                    break;
				case ID_COMMANDS_RENEW:
					wsprintf(g_InfoText, L"please enter the adapter to renew:");
					//hwndInput = CreateDialogParam(g_hInst, (LPCTSTR)IDD_GetInput, hWnd, InputDialog, (LPARAM)L"please enter the adapter to release:");
					//ShowWindow(hwndInput, SW_SHOWNA);
					if(DialogBox(g_hInst, (LPCTSTR)IDD_GetInput, hWnd, InputDialog)==IDOK)
						RenewAddress(g_selectedAdapter);
                    break;
				case ID_COMMANDS_SAVETEXT:
					saveText();
					break;
                case IDM_OK:
                    SendMessage (hWnd, WM_CLOSE, 0, 0);				
                    break;
                default:
                    return DefWindowProc(hWnd, message, wParam, lParam);
            }
            break;
        case WM_CREATE:
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
			
			//#### COutput stuff start ####
			GetClientRect(hWnd, &rClient);
//			nCmdHt = CommandBar_Height(mbi.hwndMB);
			g_hWndEdit = CreateWindow(_T("Edit"), 
					NULL, 
					WS_CHILD | WS_VISIBLE | ES_READONLY | ES_MULTILINE | ES_WANTRETURN |
					WS_VSCROLL | WS_HSCROLL 
					//ES_AUTOHSCROLL | ES_AUTOVSCROLL 
					, 
					0, 0,
					rClient.right, rClient.bottom - 25, 
					hWnd, NULL, g_hInst, NULL);
			
			setOutputWindow(g_hWndEdit);


            break;
        case WM_PAINT:
            hdc = BeginPaint(hWnd, &ps);
            
            // TODO: Add any drawing code here...
            
            EndPaint(hWnd, &ps);
            break;
        case WM_DESTROY:
            CommandBar_Destroy(g_hWndMenuBar);
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

// Message handler for about box.
INT_PTR CALLBACK InputDialog(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam)
{
//	TCHAR* szTxt;
	BOOL lpTrans=FALSE;
	DWORD dwRes=0;
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
			//szTxt=(TCHAR*)lParam;
			//SetDlgItemText(hDlg, IDC_EDIT_INFO, szTxt);
			SetDlgItemText(hDlg, IDC_EDIT_INFO, g_InfoText);
			if(SetDlgItemInt(hDlg, IDC_EDIT_INPUT, g_selectedAdapter, 0)==0)
				DEBUGMSG(1, (L"SetDlgItemInt failed with code=%i\n", GetLastError()));
			else
				DEBUGMSG(1, (L"SetDlgItemInt OK \n"));
            return (INT_PTR)TRUE;

        case WM_COMMAND:
            if (LOWORD(wParam) == IDOK)
            {
				dwRes=GetDlgItemInt(hDlg,IDC_EDIT_INPUT, &lpTrans, false);
				if(lpTrans!=0)
					g_selectedAdapter=dwRes;
                EndDialog(hDlg, LOWORD(wParam));
				//DestroyWindow(hDlg);
                return TRUE;
            }
            if (LOWORD(wParam) == IDCANCEL)
            {
                EndDialog(hDlg, LOWORD(wParam));
				//DestroyWindow(hDlg);
                return TRUE;
            }
            break;

        case WM_CLOSE:
            EndDialog(hDlg, message);
			DestroyWindow(hDlg);
            return TRUE;

    }
    return (INT_PTR)FALSE;
}
