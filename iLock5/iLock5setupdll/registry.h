//registry.h

//Create, write and read registry keys and values

//global
static HKEY g_hkey=NULL;
static TCHAR g_subkey[MAX_PATH+1]=L"init";

//functions
int OpenKey();
int OpenKey(TCHAR *subkey);
int OpenCreateKey(TCHAR *subkey);
int CloseKey();
int CreateSubKey(TCHAR *subkey);
int RegReadDword(TCHAR *valuename, DWORD value);
int RegReadStr(TCHAR *valuename, TCHAR *value);

int RegWriteDword(TCHAR *valuename, DWORD value);
int RegWriteStr(TCHAR *valuename, TCHAR *str);
int RegWriteByte(TCHAR *valuename, byte value);

int RegDelValue(TCHAR *valuename);
int RegDelKey(TCHAR *keyname);

void ShowError(LONG er);

int IsIntermec(void);

//======================================================================
int RegWriteDword(TCHAR *valuename, DWORD value)
{
	LONG rc=0;
	if (g_hkey==NULL)
		rc = OpenKey();
	rc = RegSetValueEx(	g_hkey, 
						valuename, 
						NULL,
						REG_DWORD, 
						(LPBYTE) value,
						sizeof(DWORD)); 
 
	return rc;
}

int RegWriteByte(TCHAR *valuename, byte value)
{
	LONG rc=0;
	byte b = value;
	if (g_hkey==NULL)
		rc = OpenKey();
	rc = RegSetValueEx(	g_hkey, 
						valuename, 
						NULL,
						REG_BINARY, 
						&b,
						sizeof(byte)); 
 
	return rc;
}

int RegWriteStr(TCHAR *valuename, TCHAR *str)
{
	LONG rc=0;
	if (g_hkey==NULL)
		rc = OpenKey();
	TCHAR txt[MAX_PATH+1];
	wcscpy(txt, str);
	rc = RegSetValueEx(	g_hkey, 
						valuename, 
						NULL,
						REG_SZ, 
						(LPBYTE)txt,
						(wcslen(txt) + 1) * sizeof(txt[0]));
 	return rc;
}

int RegReadByte(TCHAR *valuename, byte *value)
{
	static byte dwResult;
	LONG rc;
	DWORD dwType=REG_BINARY;
	DWORD dwSize=sizeof(byte);
	if (g_hkey==NULL)
		rc = OpenKey();
	if (g_hkey != NULL)
	{
		rc = RegQueryValueEx(g_hkey, valuename, NULL, &dwType, &dwResult, &dwSize);
		if (rc == ERROR_SUCCESS)
		{
			CloseKey();
			*value = dwResult;
			return rc;
		}
	}
	CloseKey();
	return rc;
}


//RegReadDword
int RegReadDword(TCHAR *valuename, DWORD *value)
{
	static DWORD dwResult;
	//DWORD *pdwResult = &dwResult;

	LONG rc;
	DWORD dwType=REG_DWORD;
	DWORD dwSize=sizeof(DWORD);
	if (g_hkey==NULL)
		rc = OpenKey();
	if (g_hkey != NULL)
	{
		rc = RegQueryValueEx(g_hkey, valuename, NULL, &dwType, (LPBYTE) &dwResult, &dwSize);
		if (rc == ERROR_SUCCESS)
		{
			CloseKey();
			*value = dwResult;
			return rc;
		}
	}
	CloseKey();
	return rc;
}

//RegReadStr
int RegReadStr(TCHAR *valuename, TCHAR *value)
{
	static TCHAR szStr[MAX_PATH+1];
	LONG rc;
	DWORD dwType=REG_SZ;
	DWORD dwSize=0;
	if (g_hkey == NULL)
	{
		if (OpenKey()==0) //use default g_hkey
		{
			dwSize = sizeof(szStr) * sizeof(TCHAR);
			rc = RegQueryValueEx(g_hkey, valuename, NULL, &dwType, (LPBYTE)szStr, &dwSize);
			if (rc == ERROR_SUCCESS)
			{
				CloseKey();
				wcscpy(value, szStr);
				return 0;
			}
		}
	}
	else
	{
		//use already opened g_hkey
		dwSize = sizeof(szStr) * sizeof(TCHAR);
		rc = RegQueryValueEx(g_hkey, valuename, NULL, &dwType, (LPBYTE)szStr, &dwSize);
		if (rc == ERROR_SUCCESS)
		{
			CloseKey();
			wcscpy(value, szStr);
			return 0;
		}
	}

	wcscpy(value, L"");
	CloseKey();
	return -1;
}

//OpenKey to iHook2
int OpenKey()
{
	//open key to gain access to subkeys
	LONG rc = RegOpenKeyEx(
        HKEY_LOCAL_MACHINE, 
        g_subkey, 
        0,
        0, 
        &g_hkey);
	if (rc == ERROR_SUCCESS)
		return 0;
	else
	{
		g_hkey=NULL;
		return rc;
	}
}

int OpenCreateKey(TCHAR *subkey)
{
	DWORD dwDisp;
	LONG rc;
	if (wcslen(subkey)==0)
		wcscpy(subkey, g_subkey);
	//create the key if it does not exist
	rc = RegCreateKeyEx(HKEY_LOCAL_MACHINE,
						subkey, 
						0, 
						NULL, 
						0, 
						0, 
						NULL,
						&g_hkey,
						&dwDisp);
	return rc;
}

//OpenKey with a specified SubKey
int OpenKey(TCHAR *subkey)
{
	//open key to gain access to subkeys
	LONG rc = RegOpenKeyEx(
        HKEY_LOCAL_MACHINE, 
        subkey, 
        0,
        0, 
        &g_hkey);
	if (rc == ERROR_SUCCESS)
	{
		wsprintf(g_subkey, L"%s", subkey);
		return 0;
	}
	else
	{
		g_hkey=NULL;
		return rc;
	}
}

int RegDelValue(TCHAR *valuename)
{
	if (g_hkey==NULL)
	{
		if (OpenKey()!=0)
			return -1; //could not open default key
	}
	if ( ERROR_SUCCESS == RegDeleteValue(g_hkey, valuename) )
		return 0;	//success
	else
	{
/*
#ifdef DEBUG
		ShowError(GetLastError());
#endif
*/
		return -2; //error deleting key
	}
}

int RegDelKey(TCHAR *keyname)
{
	if (g_hkey==NULL)
	{
		if (OpenKey()!=0)
			return -1; //could not open default key
	}
	if (ERROR_SUCCESS == RegDeleteKey(g_hkey, keyname))
		return 0;	//success
	else
		return -2; //error deleting key
}

//close the global g_hkey
int CloseKey()
{
	if (g_hkey == NULL)
		return 0;
	LONG rc = RegCloseKey(g_hkey);
	g_hkey=NULL;
	return rc;
}

//will open or create a subkey and changes global g_hkey
int CreateSubKey(TCHAR *subkey)
{
	DWORD dwDisp;
	//L"\\Software\\Intermec\\iHook2"
	LONG rc = RegCreateKeyEx (HKEY_LOCAL_MACHINE, 
 					 subkey, 
					 0,
                     TEXT (""), 
					 0, 
					 0, 
					 NULL, 
                     &g_hkey, 
					 &dwDisp);
	return rc;
}

void ShowError(LONG er)
{
	LPVOID lpMsgBuf;
	FormatMessage( 
		FORMAT_MESSAGE_ALLOCATE_BUFFER | 
		FORMAT_MESSAGE_FROM_SYSTEM | 
		FORMAT_MESSAGE_IGNORE_INSERTS,
		NULL,
		er,
		0, // Default language
		(LPTSTR) &lpMsgBuf,
		0,
		NULL 
	);
	// Process any inserts in lpMsgBuf.
	// ...
	// Display the string.
	MessageBox( NULL, (LPCTSTR)lpMsgBuf, L"Error", MB_OK | MB_ICONINFORMATION | MB_TOPMOST | MB_SETFOREGROUND);
	// Free the buffer.
	LocalFree( lpMsgBuf );
}

//////////////////////////////////////////////////////////////////////////////////
// IsIntermec will test a reg key and return 0, if it contains Intermec
//////////////////////////////////////////////////////////////////////////////////
int IsIntermec(void)
{
	OpenKey(L"Platform");
	if (g_hkey != NULL)
	{
		TCHAR val[MAX_PATH+1];
		if ( RegReadStr(L"Name", val) == 0) //no error?
		{
			if ( wcsstr(val, L"Intermec") != NULL )
				return 0;
			else
				return 1;
		}
		else
			return -2;
	}
	else
		return -1;
}
