// iLock5setupdll.cpp : Defines the entry point for the DLL application.

/* ilock5setupdll.dll according */

//this setup.dll will uninstall a previously installed iLock

#define UseLogging		//to enable logging to file
#include <windows.h>
#include "ce_setup.h"   /* in the public SDK dir */
#include "iLock5setupdll.h"

/*
 =======================================================================================================================
 *
 =======================================================================================================================
 */
BOOL APIENTRY DllMain( HANDLE h, DWORD reason, LPVOID lpReserved )
{
    return TRUE;
}   /* DllMain */

	
/*
 =======================================================================================================================
 *
 =======================================================================================================================
 */
codeINSTALL_INIT Install_Init( HWND hwndParent, BOOL fFirstCall, BOOL fPreviouslyInstalled, LPCTSTR pszInstallDir )
{
	if (fFirstCall)
	{
		newfile("\\iLock5setup.log.txt");
		//UnistalliLock2();
	}
	//uninstall previous iLock5, stop process and delete files!
	
	//move from Install_Exit!
	uninstall_iLock(L"iLock5.exe");
	return codeINSTALL_INIT_CONTINUE;
}

/*
 =======================================================================================================================
 *
 =======================================================================================================================
 */
codeINSTALL_EXIT Install_Exit
(
    HWND    hwndParent,
    LPCTSTR pszInstallDir,
    WORD    cFailedDirs,
    WORD    cFailedFiles,
    WORD    cFailedRegKeys,
    WORD    cFailedRegVals,
    WORD    cFailedShortcuts
)
{
	//Put zero in Instl, so the OS does not worry about second install
	if (0==OpenKey(L"SOFTWARE\\Apps\\Intermec iLock5"))
	{
		Add2Log(L"Opened key 'SOFTWARE\\Apps\\Intermec iLock5'\n",false);
		if (0==RegWriteDword(L"Instl", 0))
			Add2Log(L"Put 0 into 'SOFTWARE\\Apps\\Intermec iLock5\\Instl'\n",false);
		else
			Add2Log(L"Could not put 0 into 'SOFTWARE\\Apps\\Intermec iLock5\\Instl'\n",false);
	}
	else
		Add2Log(L"Could not open 'SOFTWARE\\Apps\\Intermec iLock5'\n",false);

    if ( cFailedDirs || cFailedFiles || cFailedRegKeys || cFailedRegVals || cFailedShortcuts )
    {
		//DoReset();
        return codeINSTALL_EXIT_UNINSTALL;
    }
	DoReset();
    return codeINSTALL_EXIT_DONE;
}

/*
 =======================================================================================================================
 *
 =======================================================================================================================
 */
codeUNINSTALL_INIT Uninstall_Init( HWND hwndParent, LPCTSTR pszInstallDir )
{
    /* TODO: Perform the reverse of INSTALL_INIT here */
    return codeUNINSTALL_INIT_CONTINUE;
}

/*
 =======================================================================================================================
 *
 =======================================================================================================================
 */
codeUNINSTALL_EXIT Uninstall_Exit( HWND hwndParent )
{
    /* TODO: Perform the reverse of INSTALL_EXIT here */
    return codeUNINSTALL_EXIT_DONE;
}

