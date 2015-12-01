// reg.cpp

#include "stdafx.h"
#include "reg.h"

TCHAR* regGetCode(){
	static TCHAR szCode[MAX_PATH];
	wsprintf(szCode, L"52401");
	TCHAR szTemp[MAX_PATH];
	HKEY hKey;
	DWORD dwDispo=0;
	DWORD dwSize=MAX_PATH*sizeof(TCHAR);
	DWORD dwType=REG_SZ;
	DWORD dwRes = RegCreateKeyEx(HKEY_LOCAL_MACHINE, L"Software\\Intermec\\touchLockWEH", 0, NULL, REG_OPTION_NON_VOLATILE, KEY_ALL_ACCESS, NULL, &hKey, &dwDispo);
	if(dwRes==ERROR_SUCCESS){
		if(dwDispo==REG_OPENED_EXISTING_KEY){
			dwRes=RegQueryValueEx(hKey, L"code", NULL, &dwType, (LPBYTE)szTemp, &dwSize);
			if(dwRes==ERROR_SUCCESS){
				wsprintf(szCode, L"%s", szTemp);
				DEBUGMSG(1, (L"regGetCode read '%s'\n", szCode));
			}
			else{
				DEBUGMSG(1, (L"regGetCode read failed: %i\n", dwRes));
				dwSize=wcslen(szCode)*sizeof(TCHAR)+sizeof(TCHAR);
				dwRes=RegSetValueEx(hKey, L"code", 0, dwType, (LPBYTE)szCode, dwSize);
				if(dwRes==ERROR_SUCCESS)
					DEBUGMSG(1, (L"regGetCode write '%s' OK\n", szCode));
				else
					DEBUGMSG(1, (L"regGetCode write failed: %i\n", dwRes));
			}
		}
		else{ //NEW
			DEBUGMSG(1, (L"regGetCode create new key\n"));
			dwSize=wcslen(szCode)*sizeof(TCHAR)+sizeof(TCHAR);
			dwRes=RegSetValueEx(hKey, L"code", 0, dwType, (LPBYTE)szCode, dwSize);
			if(dwRes==ERROR_SUCCESS)
				DEBUGMSG(1, (L"regGetCode write '%s' OK\n", szCode));
			else
				DEBUGMSG(1, (L"regGetCode write failed: %i\n", dwRes));
		}
	}
	else
		DEBUGMSG(1, (L"regGetCode RegCreateKeyEx failed: %i. Code is '%s'\n", dwRes, szCode));
	RegFlushKey(hKey);
	RegCloseKey(hKey);
	return szCode;
}

DWORD regGetBKColor(){
	static DWORD dwColor=0x00AABBCC;	
	DWORD dwTemp=0;
	HKEY hKey;
	DWORD dwDispo=0;
	DWORD dwSize=sizeof(DWORD);
	DWORD dwType=REG_DWORD;
	DWORD dwRes = RegCreateKeyEx(HKEY_LOCAL_MACHINE, L"Software\\Intermec\\touchLockWEH", 0, NULL, REG_OPTION_NON_VOLATILE, KEY_ALL_ACCESS, NULL, &hKey, &dwDispo);
	if(dwRes==ERROR_SUCCESS){
		if(dwDispo==REG_OPENED_EXISTING_KEY){
			//existing key
			dwRes=RegQueryValueEx(hKey, L"bkcolor", NULL, &dwType, (LPBYTE)dwTemp, &dwSize);

			if(dwRes==ERROR_SUCCESS){
				dwColor=dwTemp;
				DEBUGMSG(1, (L"regGetBKColor read 0x%08x\n", dwColor));
			}
			else{
				DEBUGMSG(1, (L"regGetBKColor read failed: %i\n", dwRes));
				dwSize=sizeof(DWORD);
				dwType=REG_DWORD;
				dwRes=RegSetValueEx(hKey, L"bkcolor", 0, dwType, (LPBYTE)&dwColor, dwSize);
				if(dwRes==ERROR_SUCCESS)
					DEBUGMSG(1, (L"regGetBKColor write 0x%08x OK\n", dwColor));
				else
					DEBUGMSG(1, (L"regGetBKColor write failed: %i\n", dwRes));
			}
		}
		else{ 
			//NEW key
			DEBUGMSG(1, (L"regGetBKColor create new key\n"));
			dwSize=sizeof(DWORD);
			dwRes=RegSetValueEx(hKey, L"bkcolor", 0, dwType, (LPBYTE)dwColor, dwSize);
			if(dwRes==ERROR_SUCCESS)
				DEBUGMSG(1, (L"regGetBKColor write 0x%08x OK\n", dwColor));
			else
				DEBUGMSG(1, (L"regGetBKColor write failed: %i\n", dwRes));
		}
	}
	else
		DEBUGMSG(1, (L"regGetBKColor RegCreateKeyEx failed: %i. Code is 0x%08x\n", dwRes, dwColor));

	RegFlushKey(hKey);
	RegCloseKey(hKey);

	return dwColor;
}