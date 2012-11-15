//touchEnable.cpp

//shutdown existing DisableTouch window

#include <windows.h>
#include "hooks.h"

#define WM_ENABLETOUCH WM_USER + 5240
#define TOUCHWINDOWNAME L"DisableTouch"

int _tmain(int argc, _TCHAR* argv[])
{
	HWND hwndTouchDisable = FindWindow(TOUCHWINDOWNAME, TOUCHWINDOWNAME);
	if(hwndTouchDisable!=NULL){
		DEBUGMSG(1, (L"Posting quit to '%s'\n", TOUCHWINDOWNAME));
		PostMessage(hwndTouchDisable, WM_ENABLETOUCH, 0, 0);
		return 1;
	}
	else{
		DEBUGMSG(1, (L"No window found: '%s'\n", TOUCHWINDOWNAME));
		return -1;
	}

	return 0;
}