// disabletouch1.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

#include "hooks.h"

int _tmain(int argc, _TCHAR* argv[])
{
	if(argc==2){
		InitializeTouchDll();
		HWND hwnd = GetDesktopWindow();
		DEBUGMSG(1,(L"GetDesktopWindow() = 0x%08x", hwnd));
		long lCmd;
		lCmd = _wtol(argv[1]);

		if(lCmd==1){		//disable touch
			DEBUGMSG(1, (L"+++ TouchRegisterWindow called\n"));
			return m_fpTouchRegisterWindow(hwnd);
		}
		else if (lCmd==0){	//enable touch
			SetLastError(0);
			DEBUGMSG(1, (L"--- TouchUnregisterWindow called\n"));
			m_fpTouchUnregisterWindow(hwnd);
			return GetLastError();
		}
		else
			return -2;//wrong arg
	}
	else
		return -1; //no args no fun
	return 0;
}

