// Windows Header Files:
#include "windows.h"
#include "winuser.h"

#define IDD_PWBOX                       101
#define IDC_PWTEXT                      1001

LRESULT CALLBACK PasswordProc(HWND hWnd,UINT iMsg,
                         WPARAM wParam, LPARAM lParam);
