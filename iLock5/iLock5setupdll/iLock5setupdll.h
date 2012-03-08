//iLocksetup.h
// worker code for iLock setupdll
#include "log2file.h"
#include "locktaskbar.h"
#include "registry.h"

int uninstall_iLock(TCHAR * name)
{
	TCHAR actName[MAX_PATH];
	TCHAR str[MAX_PATH];

	wsprintf(actName, name);

	//if process is running, kill it
	wsprintf(str,L"*** Uninstall %s started ***\n",actName);
	Add2Log(str, FALSE);
	
	wsprintf(str,L"\nChecking if process %s is running...\n",actName);
	Add2Log(str, FALSE);
	
	if (IsProcessRunning(actName))
	{
		wsprintf(str,L"%s is running, trying to kill...\n", actName);
		Add2Log(str, FALSE);
		if (KillExeWindow(actName))	//Hopefully, solLock.exe is killed now
			Add2Log(L"Success killing process\n", FALSE);
		else
			Add2Log(L"FAILED: killing process\n", FALSE);
		Add2Log(L"Sleeping 10 seconds...\n", FALSE);
		Sleep(10000);
	}	
	else	
		Add2Log(L"Process is not running, skipping KillExe\n", FALSE);

	////check install of solLock
	//Add2Log(L"\nCheck Registry for init entry...\n", FALSE);
	//if (0==OpenKey(L"init"))
	//	Add2Log(L"Success OpenKey 'init'\n", FALSE);
	//else
	//	Add2Log(L"FAILED: OpenKey 'init'\n", FALSE);

	//Add2Log(L"Reading Launch45...\n", FALSE);
	//TCHAR val[MAX_PATH];
	//if (0 == RegReadStr(L"Launch45", val))
	//{
	//	wsprintf(str,L"Success Reading Launch45. Now compare with %s...\n",actName);
	//	Add2Log(str, FALSE);
	//	if (wcsicmp(actName, val) == 0)	//if this is not true, something is wrong
	//	{
	//		wsprintf(str,L"Launch45 is '%s'. Now will delete the values...\n",actName);
	//		Add2Log(str, FALSE);
	//		//delete old registry entries
	//		if (RegDelValue(L"Depend45") == 0)	//g_hkey is HKLM\init
	//			Add2Log(L"Success deleting value 'Depend45'\n", FALSE);
	//		else
	//			Add2Log(L"FAILED deleting hkey 'Depend45'\n", FALSE);
	//		if (RegDelValue(L"Launch45") == 0)	//g_hkey is HKLM\init
	//			Add2Log(L"Success deleting value 'Launch45'\n", FALSE);
	//		else
	//			Add2Log(L"FAILED deleting hkey 'Launch45'\n", FALSE);
	//	}
	//	else
	//	{
	//		wsprintf(str,L"Launch45 is NOT '%s'. Skipp deleting values...\n",actName);
	//		Add2Log(str, FALSE);
	//	}
	//}
	//else
	//	Add2Log(L"FAILED Reading Launch45. Skipping delete of Launch45 and Depend45 values\n", FALSE);

	//delete apps entry for Intermec solLock, if any
	wsprintf(str,L"\nWill remove AppMgr registry entries for %s now...\n",actName);
	Add2Log(str,FALSE);
	TCHAR nameonly[MAX_PATH];

	memset(nameonly, L'\0', sizeof(nameonly));
	wcsncpy(nameonly,actName,wcslen(actName)-wcslen(L".exe"));
	
	//nameonly[wcslen(nameonly)]=L'\0';
	//wsprintf(nameonly, L"%s\0\0", nameonly);
	wsprintf(str, L"SOFTWARE\\Apps\\Intermec %s",nameonly);
	if (0==OpenKey(str))
	{
		wsprintf(str, L"Install information found. Success open key 'SOFTWARE\\Apps\\Intermec %s'.\n", nameonly);
		Add2Log(str,FALSE);
		if (0==OpenKey(L"SOFTWARE\\Apps"))
		{
			wsprintf(str, L"Key SOFTWARE\\Apps opened. Trying to delete key 'Intermec %s'\n", nameonly);
			Add2Log(str,FALSE);
			if (0 == RegDelKey(L"Intermec solLock"))
				Add2Log(L"Success deleting key\n",FALSE);
			else
				Add2Log(L"FAILED deleting key\n",FALSE);
		}
		else
			Add2Log(L"Could not open key SOFTWARE\\Apps\n",FALSE);
	}
	else
	{
		wsprintf(str, L"No install information found. Could not open key SOFTWARE\\Apps\\Intermec %s. Skipping removing keys\n", nameonly);
		Add2Log(str,FALSE);
	}
	//delete files for solLock
	wsprintf(str, L"\nWill remove %s files now...\n", nameonly);
	Add2Log(str,FALSE);
	wsprintf(str, L"\\Windows\\%s", actName);
	if (ExistFile(str))
	{
		if (DelFile(str))
			Add2Log(L"Success removing exe\n",FALSE);
		else
			Add2Log(L"Failed removing exe\n",FALSE);
	}
	else
	{
		wsprintf(str, L"File '\\Windows\\%s' not found\n", actName);
		Add2Log(str,FALSE);
	}

	//delete (iLock5).bmp
	memset(str, L'\0', sizeof(str));
	wsprintf(str, L"\\Windows\\%s.bmp", nameonly);
	if (ExistFile(str))
	{
		if (DelFile(str))
			Add2Log(L"Success removing bmp file\n",FALSE);
		else
			Add2Log(L"Failed removing bmp file\n",FALSE);
	}
	else
	{
		wsprintf(str, L"File '\\Windows\\%s.bmp' not found\n", nameonly);
		Add2Log(str, FALSE);
	}

	wsprintf(str, L"*** End of uninstall %s ***\n\n", actName);
	Add2Log(str, FALSE);

	return 0;
}

int UnistalliLock2(void)
{
	int res=0;
	res += uninstall_iLock(L"solLock.exe");
	res += uninstall_iLock(L"solLock2.exe");
	res += uninstall_iLock(L"solLock3.exe");
	res += uninstall_iLock(L"iLock.exe");
	res += uninstall_iLock(L"ilock2.exe");
	res += uninstall_iLock(L"ilock3.exe");
	return res;
}
