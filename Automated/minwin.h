//minwin.h

#ifndef _MIN_WIN_H_
#define _MIN_WIN_H_

#include "stdafx.h"

int startWin(RECT* rect);
extern BOOL stopApp;
extern HANDLE stopHandle;
extern HWND hWndMain;
#define WM_UPDATEWIN WM_USER + 47

#endif //_MIN_WIN_H_