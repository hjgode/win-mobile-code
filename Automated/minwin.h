//minwin.h

#ifndef _MIN_WIN_H_
#define _MIN_WIN_H_

#include "stdafx.h"

int startWin(RECT* rect);
extern BOOL stopApp;
extern HANDLE stopHandle;
extern HWND hWndMain;
extern const TCHAR* const myclass;
#define WM_UPDATEWIN WM_USER + 47
#define WM_NEWTEXT   WM_USER + 48

typedef struct tagMYREC
{
   TCHAR  s1[80];
   DWORD n;
} MYREC;

extern COPYDATASTRUCT MyCDS;

#endif //_MIN_WIN_H_