// Undocumented though operational though unsupported Hooks: types and structs

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <windows.h>
#include <winuser.h>

#define MYMSG_TASKBARNOTIFY  (WM_USER + 100)

#define WH_MIN      				(-1)
#define WH_MSGFILTER    		(-1)
#define WH_JOURNALRECORD  		0
#define WH_JOURNALPLAYBACK  	1
#define WH_KEYBOARD    			2
#define WH_GETMESSAGE   		3
#define WH_CALLWNDPROC   		4
#define WH_CBT      				5
#define WH_SYSMSGFILTER   		6
#define WH_MOUSE     			7
#define WH_HARDWARE    			8
#define WH_DEBUG     			9
#define WH_SHELL     			10
#define WH_FOREGROUNDIDLE  	11
#define WH_MAX      				11

#define WH_KEYBOARD_LL   		20

#define HC_ACTION           	0
#define HC_GETNEXT          	1
#define HC_SKIP             	2
#define HC_NOREMOVE         	3
#define HC_SYSMODALON       	4
#define HC_SYSMODALOFF      	5

#define HC_NOREM            	HC_NOREMOVE

#define WH_CBT              	5
#define GWL_HINSTANCE       	-6
#define HCBT_ACTIVATE       	5

// Used by WH_KEYBOARD_LL
#define LLKHF_EXTENDED       	(KF_EXTENDED >> 8)
#define LLKHF_INJECTED       	0x00000010
#define LLKHF_ALTDOWN        	(KF_ALTDOWN >> 8)
#define LLKHF_UP             	(KF_UP >> 8)
#define LLMHF_INJECTED       	0x00000001

// Define the function types used by hooks
typedef LRESULT	(CALLBACK* HOOKPROC)(int code, WPARAM wParam, LPARAM lParam);
typedef HHOOK 		(WINAPI *_SetWindowsHookExW)(int, HOOKPROC, HINSTANCE, DWORD);
typedef LRESULT	(WINAPI *_CallNextHookEx)(HHOOK, int, WPARAM, LPARAM);
typedef LRESULT	(WINAPI *_UnhookWindowsHookEx)(HHOOK);

// Apparently undefined in HPC 2.11. Defined in basetsd.h in PocketPC 2002 (shared by HPC 2K).
typedef unsigned long ULONG_PTR, *PULONG_PTR;

// For the low level keyboard hook, you are passed a pointer to one of these
typedef struct {
    DWORD vkCode;
    DWORD scanCode;
    DWORD flags;
    DWORD time;
    ULONG_PTR dwExtraInfo;
} KBDLLHOOKSTRUCT, *PKBDLLHOOKSTRUCT;


// Win32 Hook APIs (manually loaded)
static _SetWindowsHookExW 		SetWindowsHookEx;
static _UnhookWindowsHookEx	UnhookWindowsHookEx;
static _CallNextHookEx  		CallNextHookEx;		

