// PhonePowerWatchCPP.h : main header file for the PROJECT_NAME application
//

#pragma once

#ifndef __AFXWIN_H__
	#error "include 'stdafx.h' before including this file for PCH"
#endif

#ifdef POCKETPC2003_UI_MODEL
#include "resourceppc.h"
#endif 

// CPhonePowerWatchCPPApp:
// See PhonePowerWatchCPP.cpp for the implementation of this class
//

class CPhonePowerWatchCPPApp : public CWinApp
{
public:
	CPhonePowerWatchCPPApp();
	
// Overrides
public:
	virtual BOOL InitInstance();

// Implementation

	DECLARE_MESSAGE_MAP()
};

extern CPhonePowerWatchCPPApp theApp;
