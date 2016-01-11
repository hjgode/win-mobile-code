// iSIP2.cpp : Defines the entry point for the application.
//

#include "stdafx.h"
#include "locktaskbar.h"
#include "registry.h"

int WINAPI WinMain(	HINSTANCE hInstance,
					HINSTANCE hPrevInstance,
					LPTSTR    lpCmdLine,
					int       nCmdShow)
{
	if (IsIntermec() != 0)
	{
		MessageBox(NULL, L"This is not an Intermec! Program execution stopped!", L"Fatal Error", MB_OK | MB_TOPMOST | MB_SETFOREGROUND);
		return -1;
	}
 	// TODO: Place code here.
	bool show=true;
	SIPINFO sip;
	int OK;

	if (wcslen(lpCmdLine) == 0)
	{
		memset(&sip, 0, sizeof(sip));
		sip.cbSize = sizeof (sip);

		OK = SipGetInfo(&sip);
		if (OK == FALSE)
		{
			MessageBeep(MB_ICONHAND);
			return -1;
		}
		show = !(sip.fdwFlags & SIPF_ON);
		BOOL bRet = ShowSIP(show); 
		if (bRet){
			MessageBeep(MB_OK);
		}
		else{
			MessageBeep(MB_ICONHAND);
			DEBUGMSG(1, (L"ShowSIP error: %i\n", (int)bRet));
		}
	}
	else if (wcsstr(lpCmdLine, L"-toggle") != NULL)
	{
		memset(&sip, 0, sizeof(sip));
		sip.cbSize = sizeof (sip);

		OK = SipGetInfo(&sip);
		if (OK == FALSE)
		{
			MessageBeep(MB_ICONHAND);
			return -1;
		}
		show = !(sip.fdwFlags & SIPF_ON);
		if (ShowSIP(show))
			MessageBeep(MB_OK);
		else
			MessageBeep(MB_ICONHAND);
	}
	else if (wcsstr(lpCmdLine, L"-show") != NULL)
	{
		ShowSIP(true);
		MessageBeep(MB_OK);
	}
	else if (wcsstr(lpCmdLine, L"-hide") != NULL)
	{
		ShowSIP(false);
		MessageBeep(MB_ICONHAND);
	}
	return 0;
}
