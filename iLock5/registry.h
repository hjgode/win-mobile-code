//registry.h

//Create, write and read registry keys and values

//global
static HKEY g_hkey=NULL;
static TCHAR g_subkey[MAX_PATH+1]=L"Software\\Intermec\\iLock";

//functions
int OpenKey();
int OpenKey(TCHAR *subkey);
int OpenCreateKey(TCHAR *subkey);
int CloseKey();
int CreateSubKey(TCHAR *subkey);
int RegReadDword(TCHAR *valuename, DWORD* value);
int RegReadStr(TCHAR *valuename, TCHAR *value);

int RegWriteDword(TCHAR *valuename, DWORD value);
int RegWriteStr(TCHAR *valuename, TCHAR *str);
int RegWriteByte(TCHAR *valuename, byte value);

void ShowError(LONG er);

int IsIntermec(void);

int ReadBuildNumber(TCHAR *szBuildNumber);
int ReadPlatformName(TCHAR *szPlatformName);

//===============================================================
int ReadBuildNumber(TCHAR *szBuildNumber)
{
	HKEY oldKey=g_hkey;
	wsprintf(szBuildNumber, L"unknown");
	int ec;
	ec = OpenKey(L"Platform");
	if (ec == ERROR_SUCCESS)
	{
		ec = RegReadStr(L"Software Build Number", szBuildNumber);
		g_hkey=oldKey;
		return ec;
	}
	else
	{
		g_hkey=oldKey;
		return ec;
	}
}

int ReadPlatformName(TCHAR *szPlatformName)
{
	HKEY oldKey=g_hkey;
	wsprintf(szPlatformName, L"unknown");
	int ec;
	ec = OpenKey(L"Platform");
	if (ec == ERROR_SUCCESS)
	{
		ec = RegReadStr(L"Name", szPlatformName);
		g_hkey=oldKey;
		return ec;
	}
	else
	{
		g_hkey=oldKey;
		return ec;
	}
}

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
int RegReadDword(TCHAR *valuename, DWORD* value)
{
	static DWORD dwResult;
	LONG rc;
	DWORD dwType=REG_DWORD;
	DWORD dwSize=sizeof(DWORD);
	TCHAR *szValuename;
	szValuename=new TCHAR[wcslen(valuename)];
	wsprintf(szValuename, valuename);

	if (g_hkey==NULL)
		rc = OpenKey();
	if (g_hkey != NULL)
	{
		rc = RegQueryValueEx(g_hkey, szValuename, NULL, &dwType, (LPBYTE) &dwResult, &dwSize);
		if (rc == ERROR_SUCCESS)
		{
			*value = dwResult;
			delete szValuename;
			CloseKey();
			return rc;
		}
	}
	delete szValuename;
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
		return 0;
	else
	{
		g_hkey=NULL;
		return rc;
	}
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
	TCHAR temp[MAX_PATH];
	wsprintf(temp, (LPTSTR)lpMsgBuf);
	// Process any inserts in lpMsgBuf.
	// ...
#ifdef DEBUG
	DEBUGMSG( 1, ( temp ) ); 
#else
	// Display the string.
	MessageBox( NULL, (LPCTSTR)lpMsgBuf, L"Error", MB_OK | MB_ICONINFORMATION );
#endif
	// Free the buffer.
	LocalFree( lpMsgBuf );
}

//////////////////////////////////////////////////////////////////////////////////
// IsIntermec will test a reg key and return 0, if it contains Intermec
//////////////////////////////////////////////////////////////////////////////////
int IsIntermec(void)
{
	TCHAR val[MAX_PATH+1];
	OpenKey(L"Platform");
	if (g_hkey != NULL)
	{
		if ( RegReadStr(L"Name", val) == 0) //no error?
		{
			if ( wcsstr(val, L"Intermec") != NULL )
				return 0;
			else
			{
				//could not find Intermec
				OpenKey(L"SOFTWARE\\Intermec\\Version"); //alternative check
				if (g_hkey != NULL)
				{
					if ( RegReadStr(L"IVA", val) == 0) //no error?
						return 0;
					else
						return -3; //could not read IVA
				}
				else
					return -2; //could not openkey
				return 1;
			}
		}
		else //could not read platform\name
		{
			OpenKey(L"SOFTWARE\\Intermec\\Version"); //separate check for ck60
			if (g_hkey != NULL)
			{
				if ( RegReadStr(L"IVA", val) == 0) //no error?
					return 0;
				else
					return -3; //could not read IVA
			}
			else
				return -2; //could not openkey
		}
	}
	else
		return -1;
}
