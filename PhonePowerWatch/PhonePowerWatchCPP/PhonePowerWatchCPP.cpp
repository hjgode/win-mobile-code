// PhonePowerWatchCPP.cpp : Defines the class behaviors for the application.
//

#include "stdafx.h"
#include "PhonePowerWatchCPP.h"
#include "PhonePowerWatchCPPDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CPhonePowerWatchCPPApp

BEGIN_MESSAGE_MAP(CPhonePowerWatchCPPApp, CWinApp)
END_MESSAGE_MAP()


// CPhonePowerWatchCPPApp construction
CPhonePowerWatchCPPApp::CPhonePowerWatchCPPApp()
	: CWinApp()
{
	// TODO: add construction code here,
	// Place all significant initialization in InitInstance
}


// The one and only CPhonePowerWatchCPPApp object
CPhonePowerWatchCPPApp theApp;

// CPhonePowerWatchCPPApp initialization

BOOL CPhonePowerWatchCPPApp::InitInstance()
{
    // SHInitExtraControls should be called once during your application's initialization to initialize any
    // of the Windows Mobile specific controls such as CAPEDIT and SIPPREF.
    SHInitExtraControls();

	// Standard initialization
	// If you are not using these features and wish to reduce the size
	// of your final executable, you should remove from the following
	// the specific initialization routines you do not need
	// Change the registry key under which our settings are stored
	// TODO: You should modify this string to be something appropriate
	// such as the name of your company or organization
	SetRegistryKey(_T("Local AppWizard-Generated Applications"));

	CPhonePowerWatchCPPDlg dlg;
	m_pMainWnd = &dlg;
	INT_PTR nResponse = dlg.DoModal();
	if (nResponse == IDOK)
	{
		// TODO: Place code here to handle when the dialog is
		//  dismissed with OK
	}

	// Since the dialog has been closed, return FALSE so that we exit the
	//  application, rather than start the application's message pump.
	return FALSE;
}
