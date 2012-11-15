//hooks.h

#pragma once
#if !defined(TOUCHLOCKPRO_HOOKS_H_INCLUDED_)
#define TOUCHLOCKPRO_HOOKS_H_INCLUDED_

#include <windows.h>
	static LRESULT CALLBACK TouchPanelHookWndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam);
	
	typedef BOOL (*TouchRegisterWindow_t)(HWND hWnd);

	typedef void (*TouchUnregisterWindow_t)(HWND hWnd);
	
	typedef VOID (*TouchPanelPowerHandler_t)(BOOL bOff);

	typedef BOOL (*SetKMode_t)(BOOL fMode);
	void HookTouchPanel(bool bHook);
	bool InitializeTouchDll();
	
	void hookInit(HINSTANCE hInstance);
	void unloadHook();

	extern bool touchPanelWindowRegistered;
    extern HINSTANCE m_hInstance;
    extern HWND	m_LockedWindow;
	extern bool lazyInitialized;
    extern HMODULE	m_TouchDLL;
	extern TouchRegisterWindow_t	m_fpTouchRegisterWindow;
	extern TouchUnregisterWindow_t	m_fpTouchUnregisterWindow;


#endif