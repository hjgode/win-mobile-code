//disableTouchDll.cpp

#include <windows.h>
#include "hooks.h"

//forward declarations
void TouchUnregisterWindow(HWND hwnd);
BOOL TouchRegisterWindow(HWND hwnd);

BOOL WINAPI DllMainCRTStartup(HANDLE hDLL, DWORD dwReason, LPVOID lpReserved)
{
	switch (dwReason){
		case DLL_PROCESS_ATTACH:
		case DLL_THREAD_ATTACH:
			DEBUGMSG(1,(L"DLL attached...\n"));
			return TRUE;
			break;
		case DLL_PROCESS_DETACH:
		case DLL_THREAD_DETACH:
			DEBUGMSG(1,(L"...DLL detached\n"));
			return TRUE;
			break;
		default:
			break;
	}
	return TRUE;
}

void __declspec( dllexport ) __stdcall disableTouch(BOOL bDisable){
	if(bDisable)
		TouchRegisterWindow(GetDesktopWindow());
	else
		TouchUnregisterWindow(GetDesktopWindow());
}

BOOL TouchRegisterWindow(HWND hwnd)
{
	TCHAR szText1[MAX_PATH];
	TCHAR szText2[MAX_PATH];
	GetWindowText(hwnd, szText1, MAX_PATH);
	GetClassName(hwnd, szText2, MAX_PATH);
	DEBUGMSG(1, (L"TouchRegisterWindow called with '0x%08x', '%s', '%s'\n", hwnd, szText1, szText2));

	InitializeTouchDll();
	BOOL bRes = m_fpTouchRegisterWindow(hwnd);
	DEBUGMSG(1, (L"real TouchRegisterWindow call returned %i, '%i'\n", bRes, GetLastError()));
	return TRUE;
}

void TouchUnregisterWindow(HWND hwnd)
{
	TCHAR szText1[MAX_PATH];
	TCHAR szText2[MAX_PATH];
	GetWindowText(hwnd, szText1, MAX_PATH);
	GetClassName(hwnd, szText2, MAX_PATH);
	DEBUGMSG(1, (L"TouchUnregisterWindow called with '0x%80x', '%s', '%s'\n", hwnd, szText1, szText2));

	InitializeTouchDll();
	m_fpTouchUnregisterWindow(hwnd);
	DEBUGMSG(1, (L"real TouchUnregisterWindow called\n"));
	return;
}
