//hooks.cpp
/*
usage:
 hookInit()
 unloadHook()
*/

#include "hooks.h"

#define LOGDEBUG(msg) DEBUGMSG(1, (msg))
#define LOGERROR(msg) DEBUGMSG(1, (msg))

#define WNDCLASSNAME L"TOUCHLOCKPRO/LOCKEDWINDOW"

#define TOUCH_SEND_TO_SYSTEM 2
#define TOUCH_SEND_TO_WINDOW 1

bool HookTouchPanelStatus = false;

static bool hooksInitialized = false; // only initialize once. so we have singleton behaviour
bool touchPanelDisabled = false; // keep track of current status, so we do not apply twice

//from hooks.h
bool touchPanelWindowRegistered=false;
HINSTANCE m_hInstance;
HWND	m_LockedWindow;
bool lazyInitialized;

HMODULE	m_TouchDLL;
TouchRegisterWindow_t	m_fpTouchRegisterWindow;
TouchUnregisterWindow_t	m_fpTouchUnregisterWindow;
//end hooks.h

TouchPanelPowerHandler_t	m_fpTouchPanelPowerHandler;

LRESULT TouchPanelHookWndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam) {
	if (touchPanelDisabled) {
		return 0;
	}
	return DefWindowProc(hWnd, message, wParam, lParam);
}

/// Create a new child window in the context of hInstance
void hookInit(HINSTANCE hInstance) {
	if (hooksInitialized) 
		return;
	lazyInitialized=false;
	hooksInitialized = true;
	touchPanelWindowRegistered = false;
	m_TouchDLL = NULL;
	
    LOGDEBUG(L"hookInit");

    m_hInstance = hInstance;

	WNDCLASS wc;
	wc.style		= CS_HREDRAW | CS_VREDRAW;
	wc.lpfnWndProc	= (WNDPROC) TouchPanelHookWndProc;
	wc.cbClsExtra	= 0;
	wc.cbWndExtra	= sizeof(LONG);
	wc.hInstance	= hInstance;
	wc.hIcon		= NULL;
	wc.hCursor		= 0;
	wc.hbrBackground = (HBRUSH)GetStockObject(WHITE_BRUSH);
	wc.lpszMenuName	 = 0;
	wc.lpszClassName = WNDCLASSNAME;
	RegisterClass(&wc);

	m_LockedWindow = CreateWindowEx(0, WNDCLASSNAME, L"TouchLockPro Panel locked", 0, 0, 0, 0, 0, NULL, NULL, hInstance, NULL);
	if (m_LockedWindow == NULL) {
		LOGERROR(L"Could not create Locked window");
		MessageBox(NULL, _T("Could not create Locked window"), _T("Information"), MB_OK | MB_ICONINFORMATION | MB_TOPMOST);
	} else {
		if (!InitializeTouchDll()) {
			LOGERROR(L"Could not create TouchPanel Hook");
			MessageBox(NULL, _T("Could not create TouchPanel Hook"), _T("Information"), MB_OK | MB_ICONINFORMATION | MB_TOPMOST);
		}
	}

	//if (!ActivateKBHook()) {
	//	LOGERROR(L"Could not create Keyboard Hook");
	//	MessageBox(NULL, _T("Could not create Keyboard Hook"), _T("Information"), MB_OK | MB_ICONINFORMATION | MB_TOPMOST);
	//}

	//CSettings settingsClass;
 //   pHooksSettingsClass = &settingsClass;
}

bool InitializeTouchDll() {
    LOGDEBUG(L"InitializeTouchDll()");
	if (m_TouchDLL == NULL) {
		m_TouchDLL = LoadLibrary(L"touch.dll");
		if(m_TouchDLL != NULL) {
			m_fpTouchPanelPowerHandler = (TouchPanelPowerHandler_t)GetProcAddress(m_TouchDLL, L"TouchPanelPowerHandler");
			m_fpTouchRegisterWindow = (TouchRegisterWindow_t)GetProcAddress(m_TouchDLL, _T("TouchRegisterWindow"));
			m_fpTouchUnregisterWindow = (TouchUnregisterWindow_t)GetProcAddress(m_TouchDLL, _T("TouchUnregisterWindow"));
			if ((m_fpTouchPanelPowerHandler != NULL) && (m_fpTouchRegisterWindow != NULL) && (m_fpTouchUnregisterWindow != NULL)) {
				return true; // everything Ok
			}
		}
	} else {
		LOGERROR(L"Error in initializing Touch dll");
		return false;
	}
	LOGERROR(L"Error in finding Touch dll register/unregister functions");
	return false; // something wrong
}

void ShutdownTouchDll() {
    LOGDEBUG(L"ShutdownTouchDll()");
	HookTouchPanel(false);

	if (m_TouchDLL != NULL) {
		FreeLibrary(m_TouchDLL);
		m_TouchDLL = NULL;
	}

	m_fpTouchRegisterWindow = NULL;
	m_fpTouchUnregisterWindow = NULL;
}

/// register zero size window with TouchPanelRegisterWindow
/// set zero size window extra memory with TOUCH_SEND_TO_SYSTEM
/// using SetWindowLong(hWnd, 0, x) sets the DWL_MSGRESULT of the window
/// DWL_MSGRESULT: Sets the return value of a message processed in the dialog box procedure
void HookTouchPanel(bool bHook) {
	if (HookTouchPanelStatus == bHook) {
        LOGDEBUG(L"HookTouchPanel nothing to do");
		return;
	}
    LOGDEBUG(L"HookTouchPanel");
    HookTouchPanelStatus = bHook;

	if (!lazyInitialized) {
		lazyInitialized = true;
		SetWindowLong(m_LockedWindow, 0, TOUCH_SEND_TO_SYSTEM);
		//SetWindowPos(m_LockedWindow, HWND_BOTTOM, 0,0,0,0, SWP_NOMOVE | SWP_NOSIZE | SWP_HIDEWINDOW);
	}

	if (bHook && !touchPanelWindowRegistered) {
		touchPanelWindowRegistered = true;
		BOOL result = m_fpTouchRegisterWindow(m_LockedWindow);
		if (!result) {
			LOGERROR(L"Error in registering m_LockedWindow");
		} else {
			LOGDEBUG(L"TouchRegisterWindow Ok");
		}
	}

	SetLastError(0);
	LONG nNew;
	if(bHook)
		nNew = TOUCH_SEND_TO_WINDOW;
	else
		nNew = TOUCH_SEND_TO_SYSTEM;
	///LONG nNew = bHook ? TOUCH_SEND_TO_WINDOW : TOUCH_SEND_TO_SYSTEM;
	LONG prevNew = GetWindowLong(m_LockedWindow, 0); //read DLG_MSG_RESULT
	if (prevNew != nNew) { ///if not equal, try a second time to register window
		LONG setResult = SetWindowLong(m_LockedWindow, 0, nNew);
		if (setResult == 0) {
			LOGERROR(L"Error in SetWindowLong");
		} else {
			LOGDEBUG(L"SetWindowLong result");
			//SetWindowPos(m_LockedWindow, HWND_BOTTOM, 0,0,0,0, SWP_NOMOVE | SWP_NOSIZE | SWP_HIDEWINDOW);
		}
	}

	//unregister wanted?
	if (!bHook && touchPanelWindowRegistered) {
		touchPanelWindowRegistered = false;
		m_fpTouchUnregisterWindow(m_LockedWindow);
        LOGDEBUG(L"TouchUnregisterWindow done");
	}
}

void unloadHook() {
	LOGDEBUG(L"unloadHook");
    ShutdownTouchDll();

	if (m_LockedWindow != NULL) {
		DestroyWindow(m_LockedWindow);
		m_LockedWindow = NULL;
	}
	UnregisterClass(WNDCLASSNAME, m_hInstance);

	//DeactivateKBHook(); // we are done with the hook. now uninstall it.
}

