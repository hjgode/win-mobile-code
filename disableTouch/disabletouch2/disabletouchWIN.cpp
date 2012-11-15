#include <windows.h>
#include "hooks.h"

#define WM_ENABLETOUCH WM_USER + 5240

HINSTANCE g_hInst;

#define TOUCHWINDOWNAME L"DisableTouch"

LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam);

int WINAPI WinMain(HINSTANCE hInstance,
                   HINSTANCE hPrevInstance,
                   LPTSTR    lpCmdLine,
                   int       nCmdShow)
{
	DEBUGMSG(1, (L"cmd line: '%s'\n", lpCmdLine));
	HWND hwndPrev = FindWindow(TOUCHWINDOWNAME, TOUCHWINDOWNAME);
	if(hwndPrev!=NULL && wcsicmp(lpCmdLine, L"0")==0)
	{
		PostMessage(hwndPrev, WM_ENABLETOUCH, 0, 0);
		return 1;
	}

	//previous instance running
	if(hwndPrev!=NULL)
		return 2;

	MSG msg          = {0};
	WNDCLASS wc      = {0}; 
	wc.lpfnWndProc   = WndProc;
	wc.hInstance     = hInstance;
	wc.hbrBackground = (HBRUSH)(COLOR_BACKGROUND);
	wc.lpszClassName = TOUCHWINDOWNAME;

	if( !RegisterClass(&wc) )
		return 1;

	HWND hWnd = CreateWindow(
			wc.lpszClassName, //class
			wc.lpszClassName, //title
			WS_VISIBLE,
			CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT,
			0,0,
			hInstance,
			NULL);
	if(hWnd==NULL)
		return 2;

	g_hInst=hInstance;

	ShowWindow(hWnd, SW_MINIMIZE);// nCmdShow);
	UpdateWindow(hWnd);

	while( GetMessage( &msg, NULL, 0, 0 ) > 0 ){
		TranslateMessage( &msg );    
		DispatchMessage( &msg );
	}

    return 0;
}

LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	HDC hdc;
	PAINTSTRUCT ps;
	switch(message)
	{
		case WM_ENABLETOUCH:
			PostQuitMessage(2);
			break;
		case WM_CREATE:
			hookInit(g_hInst);
			HookTouchPanel(true);
			break;
		case WM_DESTROY:
			unloadHook();
		case WM_SHOWWINDOW:
			if(wParam==FALSE)	//hide minimize
				PostQuitMessage(0);
			break;
		case WM_QUIT:
			DestroyWindow(hWnd);
		case WM_CLOSE:
			PostQuitMessage(0);
			break;
		case WM_PAINT:
            hdc = BeginPaint(hWnd, &ps);            
            // TODO: Add any drawing code here...
            
            EndPaint(hWnd, &ps);
            break;
		case WM_KEYUP:
			switch(wParam)
			{
				case VK_ESCAPE:
				{
					PostQuitMessage(0);
				}
				break;
			}
			break;
		case WM_MOUSEMOVE:
			DEBUGMSG(1, (L"X:%d Y:%d\n", HIWORD(lParam), LOWORD(lParam)));
			break;
		case WM_LBUTTONUP:
			DEBUGMSG(1, (L"Mouse Click: X:%d Y:%d\n", HIWORD(lParam), LOWORD(lParam)));
			break;
		default:
			return DefWindowProc(hWnd, message, wParam, lParam);
	}
	return 0;

}  
