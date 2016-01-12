// iSIP3.cpp : Defines the entry point for the application.
//

#include "stdafx.h"
#include "locktaskbar.h"
#include "registry.h"

TCHAR* mutexString=L"iSIP3 running";

HWND hSipWnd=NULL;
const int ss_hide=0;
const int ss_show=1;
const int ss_toggle=2;

//0 for hide
//1 for show
//2 for toggle
int showSip(int bShowHide){
	if(hSipWnd==NULL){
		return -1;
	}
	BOOL bVisible=IsWindowVisible(hSipWnd);
	if(bVisible && bShowHide==ss_hide)
		ShowWindow(hSipWnd, SW_HIDE);
	else if(bVisible==FALSE && bShowHide==ss_show)
		ShowWindow(hSipWnd, SW_SHOWNORMAL|SW_SHOWNOACTIVATE);
	else if(bShowHide==ss_toggle){
		if(bVisible)
			ShowWindow(hSipWnd, SW_HIDE);
		else
			ShowWindow(hSipWnd, SW_SHOWNORMAL|SW_SHOWNOACTIVATE);
	}
	return 0;
}

int WINAPI WinMain(	HINSTANCE hInstance,
					HINSTANCE hPrevInstance,
					LPTSTR    lpCmdLine,
					int       nCmdShow)
{
	if (IsIntermec() != 0)
	{
		MessageBox(NULL, L"This is not an Intermec! Program execution stopped!", L"Fatal Error", MB_OK | MB_TOPMOST | MB_SETFOREGROUND);
		return -1;
	}
	//check if running
	HANDLE hMutex=CreateMutex(NULL, TRUE, mutexString);
	if(hMutex){
		if(GetLastError()==ERROR_ALREADY_EXISTS){
			CloseHandle(hMutex);
			return -2;
		}
	}

	hSipWnd=FindWindow(L"SipWndClass", NULL);
	//hSipWnd=FindWindow(L"MicrosoftIMWndClass", NULL);

	if (hSipWnd==INVALID_HANDLE_VALUE)
	{
		MessageBeep(MB_ICONHAND);
		return -1; //fatal, no SIP window
	}

	if (wcslen(lpCmdLine) == 0)
	{
		int iRet = showSip(ss_toggle);
		if (iRet==0){
			MessageBeep(MB_OK);
		}
		else{
			MessageBeep(MB_ICONHAND);
			DEBUGMSG(1, (L"ShowSIP error: %i\n", iRet));
		}
	}
	else if (wcsstr(lpCmdLine, L"-toggle") != NULL)
	{
		int iRet = showSip(ss_toggle);
		if (iRet==0){
			MessageBeep(MB_OK);
		}
		else{
			MessageBeep(MB_ICONHAND);
			DEBUGMSG(1, (L"ShowSIP error: %i\n", iRet));
		}
	}
	else if (wcsstr(lpCmdLine, L"-show") != NULL)
	{
		int iRet = showSip(ss_show);
		if (iRet==0){
			MessageBeep(MB_OK);
		}
		else{
			MessageBeep(MB_ICONHAND);
			DEBUGMSG(1, (L"ShowSIP error: %i\n", iRet));
		}
	}
	else if (wcsstr(lpCmdLine, L"-hide") != NULL)
	{
		int iRet = showSip(ss_hide);
		if (iRet==0){
			MessageBeep(MB_OK);
		}
		else{
			MessageBeep(MB_ICONHAND);
			DEBUGMSG(1, (L"ShowSIP error: %i\n", iRet));
		}
	}
	if(hMutex)
		CloseHandle(hMutex);
	return 0;
}
