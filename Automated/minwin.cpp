//minwin.cpp

#include "stdafx.h"

const TCHAR* const myclass = L"mymsgwin" ;
COPYDATASTRUCT MyCDS;
PCOPYDATASTRUCT pMyCDS;

DWORD _threadWndProcID;
HANDLE _threadWndProcH;
BOOL stopApp;
HANDLE stopHandle=NULL;
TCHAR* stopEventName=L"_stop_event_";
HWND hWndMain=NULL;

void SetWindowTop(HWND hWnd,bool Value)
{
	if(Value)
	{
         SetWindowPos(hWnd, 
            HWND_TOPMOST, 
            0, 0, 0, 0,
            SWP_NOSIZE|SWP_NOMOVE|SWP_SHOWWINDOW);
	}
	else
	{
         SetWindowPos(hWnd, 
            HWND_NOTOPMOST, 
            0, 0, 0, 0,
            SWP_NOSIZE|SWP_NOMOVE|SWP_SHOWWINDOW); 
	}
}

long __stdcall WindowProcedure( HWND window, unsigned int msg, WPARAM wp, LPARAM lp )
{
	HDC hDC;
	TCHAR tch;
	PAINTSTRUCT pps;
	RECT rect;
	static TCHAR tch1[1]={L'-'};
	static TCHAR szTxt[64];
	static BOOL bToggle=FALSE;
	HGDIOBJ original = NULL;

    switch(msg)
    {
        case WM_DESTROY:
            DEBUGMSG(1, (L"destroying window\n")) ;
            PostQuitMessage(0) ;
            return 0L ;
		case WM_LBUTTONDBLCLK:
			PostQuitMessage(1);
			return FALSE;
        case WM_LBUTTONDOWN:
            DEBUGMSG(1, (L"\nmouse left button down at (%u, %u)\n",  LOWORD(lp), HIWORD(lp)));
            // fall thru
		case WM_CHAR:
			//tch=(TCHAR)wp;
			if(wp==VK_ESCAPE)
				PostQuitMessage(3);
			return FALSE;
		case WM_UPDATEWIN:
			bToggle=!bToggle;
			wsprintf(tch1, bToggle? L"/" : L"\\");
			InvalidateRect(window,NULL, false);
			break;
		case WM_NEWTEXT:
			wsprintf(szTxt, L"%s", (TCHAR*)wp);
			UpdateWindow(window);
			break;
		case WM_COPYDATA:
			pMyCDS = (PCOPYDATASTRUCT) lp;
			wsprintf(szTxt, L"%s", (LPSTR) ((MYREC *)(pMyCDS->lpData))->s1);
			InvalidateRect(window,NULL, false);
			break;
		case WM_PAINT:
			SetForegroundWindow(window);
			hDC=BeginPaint(window, &pps);
			//original=SelectObject(hDC,GetStockObject(DC_BRUSH));
			SetBkColor(hDC,RGB(0xef,0x00,0x00)); 

			GetClientRect(window, &rect);
			if(wcslen(szTxt)>0)
				DrawText(hDC, szTxt, -1, &rect, DT_LEFT);
			else
				DrawText(hDC, tch1, -1, &rect, DT_LEFT);
			
			//Restore original object.
			//SelectObject(hDC,original);
			EndPaint(window, &pps);
			break;
        default:
            DEBUGMSG(1, (L".")) ;
            return DefWindowProc( window, msg, wp, lp ) ;
    }
	return TRUE;
}

DWORD myWndProcThread(LPVOID lpParam){
	RECT rectWin;
	memcpy(&rectWin, lpParam, sizeof(RECT));// (RECT)lpParam;

	HBRUSH redBrush = CreateSolidBrush(RGB(0xef,0x00,0x70));
    WNDCLASS wndclass = { 
		CS_DBLCLKS, 
		WindowProcedure,
        0, //clsExtra
		0, //winExtra
		GetModuleHandle(0), //instance
		NULL, //LoadIcon(0,IDI_APPLICATION),	//icon
        NULL, //LoadCursor(0,IDC_ARROW), //cursor
		HBRUSH(redBrush),//COLOR_WINDOW+1),	//background
        0, //menu name
		myclass //class name
	} ;
	int captH = GetSystemMetrics(SM_CYCAPTION);
    if( RegisterClass(&wndclass) )
    {
        HWND window = CreateWindowEx( 0, myclass, L"title",
                   WS_OVERLAPPED |													//WS_OVERLAPPED makes a floating window, 
				   WS_EX_CAPTIONOKBTN | WS_EX_TOPMOST | WS_EX_TOOLWINDOW | 
				   WS_SYSMENU,														// WS_SYSMENU shows the closing X in caption
				   rectWin.left, rectWin.top, //CW_USEDEFAULT, CW_USEDEFAULT,
                   rectWin.right, captH*2, //rectWin.bottom*2,// 200, 200, // CW_USEDEFAULT, CW_USEDEFAULT, 
				   0, 0, GetModuleHandle(0), 0 ) ;
        if(window)
        {
			hWndMain=window;
            ShowWindow( window, SW_SHOWNORMAL ) ;
			UpdateWindow(window);
			SetWindowTop(window, TRUE);
            MSG msg ;
			while( GetMessage( &msg, 0, 0, 0 ) ) {
				TranslateMessage(&msg);
				DispatchMessage(&msg);
			}
		}
		else{
			SetEvent(stopHandle);
			return -2;
		}
	}
	else {
		SetEvent(stopHandle);
		return -3;
	}
	stopApp=TRUE;
	SetEvent(stopHandle);
	return 0;
}

int startWin(RECT* rect){
	stopHandle=CreateEvent(0, FALSE, FALSE, stopEventName);
	if(stopHandle==NULL)
		return -1; //failed to create stopHandle

	_threadWndProcH = CreateThread(0,0,myWndProcThread, (LPVOID)rect, 0, &_threadWndProcID);
	if(_threadWndProcH==NULL)
		return -4; //failed to create thread
	else
		return -2; //failed to RegisterClass
	return 0;
}