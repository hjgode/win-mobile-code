// DESCRIPTION:
//  Configuration configlet implementation of entry points.  Rename this file to reflect
//  the configlet's feature management set and include configlet2.h
//
// NOTES:
//  Assumes that the following registry keys have been created.
//   - HKEY_LOCAL_MACHINE\ConfigletTest\Age = some_dword
//   - HKEY_LOCAL_MACHINE\ConfigletTest\Name = "some_string"
//   - HKEY_LOCAL_MACHINE\ConfigletTest\EyeColor = some_dword
//
//
// COPYRIGHT (c) 2004-2005 INTERMEC TECHNOLOGIES CORPORATION, ALL RIGHTS RESERVED
//

// I N C L U D E S

#include "stdafx.h"
#include "configlet2.h"

// D E F I N E S

// E N U M S

// I N T E R N A L    C O N S T   D A T A
const HKEY hkCFG_ROOT_KEY           = HKEY_LOCAL_MACHINE;
const WCHAR sCFG_REG_PATH[ ]         = L"\\SOFTWARE\\Intermec\\SSKeyWedge";
/*
	[HKEY_LOCAL_MACHINE\SOFTWARE\HGO\KeyWedge]
	"parity"=dword:00000000
	;		0 NOPARITY
	;		1 ODDPARITY
	;		2 EVENPARITY
	;		3 MARKPARITY
	;		4 SPACEPARITY
	"stopbits"=dword:00000000
	;		0 ONESTOPBIT	
	;		1 ONE5STOPBITS
	;		2 TWOSTOPBITS	
	"databits"=dword:00000008
	;	 7
	;	 8
	;	16
	"handshake"=dword:00000003
	;	1	Xon/Xoff
	;	2	Hardware
	;	3	None
	"baudrate"=dword:0000e100
	;	  9600
	;	 19200
	;	 38400
	;	 57600
	;	115200
	"comport"="COM4:"
	;	string with trailing :
	"sendcharbychar"=dword:00000000
	;	1	send byte for byte as received
	;	0	send keys when \n received
*/
const WCHAR sCFG_REG_COMPORT_FIELD[ ]		= L"comport";
const WCHAR sCFG_REG_PARITY_FIELD[ ]		= L"parity";
const WCHAR sCFG_REG_STOPBITS_FIELD[ ]	= L"stopbits";
const WCHAR sCFG_REG_DATABITS_FIELD[ ]	= L"databits";
const WCHAR sCFG_REG_HANDSHAKE_FIELD[ ]	= L"handshake";
const WCHAR sCFG_REG_BAUDRATE_FIELD[ ]	= L"baudrate";
const WCHAR sCFG_REG_SENDCHARBYCHAR_FIELD[ ]	= L"sendcharbychar";

const WCHAR sCFG_DEFAULT_COMPORT[ ] = L"COM1:";
const DWORD dwCFG_DEFAULT_PARITY = 0;
const DWORD dwCFG_DEFAULT_STOPBITS = 0;
const DWORD dwCFG_DEFAULT_DATABITS = 8;
const DWORD dwCFG_DEFAULT_HANDSHAKE = 3;
const DWORD dwCFG_DEFAULT_BAUDRATE = 9600;
const DWORD dwCFG_DEFAULT_SENDCHARBYCHAR = 0;

const DWORD BUFFERSIZE=128;

// G L O B A L   C O N S T   D A T A

// I N T E R N A L   D A T A

// G L O B A L   D A T A

HANDLE g_hEvent = 0;

// G L O B A L   M E T H O D S

// I N T E R N A L   M E T H O D S

static HRESULT CfgStoreSetString( const HKEY hkRoot, LPCTSTR sStorePath, LPCTSTR sStoreItem, LPTSTR sValue );
static HRESULT CfgStoreGetString( const HKEY hkRoot, LPCTSTR sStorePath, LPCTSTR sStoreItem, LPTSTR* psValue, DWORD *pdwValueLen );
static HRESULT CfgRegSetDWORD( const HKEY hKey, LPCTSTR sRegKey, LPCTSTR sValueName, DWORD dwValue );
static HRESULT CfgRegGetDWORD( const HKEY hKey, LPCTSTR sRegKey, LPCTSTR sValueName, DWORD *pdwValue );

// M E T H O D   I M P L E M E N T A T I O N

// Configlet.cpp : Defines the entry point for the DLL application.
//

BOOL APIENTRY DllMain( HANDLE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved )
{
    switch( ul_reason_for_call )
    {
        case DLL_PROCESS_ATTACH:
        {
        } // End - DLL_PROCESS_ATTACH
        break;
        case DLL_THREAD_ATTACH:
        {
        } // End - DLL_THREAD_ATTACH
        break;
        case DLL_THREAD_DETACH:
        {
        } // End - DLL_THREAD_ATTACH
        break;
        case DLL_PROCESS_DETACH:
        {
        } // End - DLL_PROCESS_DETACH
        break;
    }
    return( TRUE );
} // End - DllMain

// INTERNAL METHOD IMPLEMENTATION

// EXPORTED METHOD IMPLEMENTATION

// See configlet2.h for details
CONFIGLET2_INT STDMETHODCALLTYPE IcfgGetInterfaceVersion( VOID )
{
    HRESULT hr = S_OK;

    const CONFIGLET2_INT iCONFIGLET2_VERSION2 = 2;

    return( iCONFIGLET2_VERSION2 );
} // End - GetInterfaceVersion

// See configlet2.h for details
CONFIGLET2_INT STDMETHODCALLTYPE IcfgGetSchemaVersion( CONFIGLET2_CSTR szSubSystem,
                                                       CONFIGLET2_INT *pnVersion,
                                                       CONFIGLET2_INT *pnVersionMin )
{

    HRESULT hr = S_OK;

    *pnVersion    = 1;
    *pnVersionMin = 0;

    return( hr );
} // End - IcfgGetSchemaVersion


// See configlet2.h for details
CONFIGLET2_INT STDMETHODCALLTYPE IcfgInitialize( VOID )
{
    HRESULT hr = S_OK;
	LONG	lResult;
	DWORD	dwSize;
	DWORD	dwType;
	DWORD	dwDisposition;
	HKEY	hKey;

	// Create the persistent copies of the configuration data.
	// If the keys and values already exist, leave them alone and don't overwrite
	// If they don't exist, create them and load default values.
	lResult = RegCreateKeyEx( 
		hkCFG_ROOT_KEY,
		sCFG_REG_PATH,
		0,
		NULL,
		0,
		0,
		NULL,
		&hKey,
		&dwDisposition
		);

	// If comport field doesn't exist, create it and load default value
	lResult = RegQueryValueEx( 
		hKey,
		sCFG_REG_COMPORT_FIELD,
		NULL,
		&dwType,
		NULL,
		&dwSize
		);
	if( lResult != ERROR_SUCCESS )
	{
		RegSetValueEx( 
			hKey,
			sCFG_REG_COMPORT_FIELD,
			0,
			REG_SZ,
			(BYTE*)sCFG_DEFAULT_COMPORT,
			sizeof(sCFG_DEFAULT_COMPORT)+2
			);
	}

	// If parity field doesn't exist, create it and load default value
	lResult = RegQueryValueEx( 
		hKey,
		sCFG_REG_PARITY_FIELD,
		NULL,
		&dwType,
		NULL,
		&dwSize
		);
	if( lResult != ERROR_SUCCESS )
	{
		lResult = RegSetValueEx( 
			hKey,
			sCFG_REG_PARITY_FIELD,
			0,
			REG_DWORD,
			(BYTE*)(&dwCFG_DEFAULT_PARITY),
			sizeof(dwCFG_DEFAULT_PARITY)
			);
	}

	// If stopbits field doesn't exist, create it and load default value
	lResult = RegQueryValueEx( 
		hKey,
		sCFG_REG_STOPBITS_FIELD,
		NULL,
		&dwType,
		NULL,
		&dwSize
		);
	if( lResult != ERROR_SUCCESS )
	{
		lResult = RegSetValueEx( 
			hKey,
			sCFG_REG_STOPBITS_FIELD,
			0,
			REG_DWORD,
			(BYTE*)(&dwCFG_DEFAULT_STOPBITS),
			sizeof(dwCFG_DEFAULT_STOPBITS)
			);
	}

	// If databits field doesn't exist, create it and load default value
	lResult = RegQueryValueEx( 
		hKey,
		sCFG_REG_DATABITS_FIELD,
		NULL,
		&dwType,
		NULL,
		&dwSize
		);
	if( lResult != ERROR_SUCCESS )
	{
		lResult = RegSetValueEx( 
			hKey,
			sCFG_REG_DATABITS_FIELD,
			0,
			REG_DWORD,
			(BYTE*)(&dwCFG_DEFAULT_DATABITS),
			sizeof(dwCFG_DEFAULT_DATABITS)
			);
	}

	// If handshake field doesn't exist, create it and load default value
	lResult = RegQueryValueEx( 
		hKey,
		sCFG_REG_HANDSHAKE_FIELD,
		NULL,
		&dwType,
		NULL,
		&dwSize
		);
	if( lResult != ERROR_SUCCESS )
	{
		lResult = RegSetValueEx( 
			hKey,
			sCFG_REG_HANDSHAKE_FIELD,
			0,
			REG_DWORD,
			(BYTE*)(&dwCFG_DEFAULT_HANDSHAKE ),
			sizeof(dwCFG_DEFAULT_HANDSHAKE)
			);
	}

	// If handshake field doesn't exist, create it and load default value
	lResult = RegQueryValueEx( 
		hKey,
		sCFG_REG_BAUDRATE_FIELD,
		NULL,
		&dwType,
		NULL,
		&dwSize
		);
	if( lResult != ERROR_SUCCESS )
	{
		lResult = RegSetValueEx( 
			hKey,
			sCFG_REG_BAUDRATE_FIELD,
			0,
			REG_DWORD,
			(BYTE*)(&dwCFG_DEFAULT_BAUDRATE ),
			sizeof(dwCFG_DEFAULT_BAUDRATE)
			);
	}

	// If sendcharbychar field doesn't exist, create it and load default value
	lResult = RegQueryValueEx( 
		hKey,
		sCFG_REG_SENDCHARBYCHAR_FIELD,
		NULL,
		&dwType,
		NULL,
		&dwSize
		);
	if( lResult != ERROR_SUCCESS )
	{
		lResult = RegSetValueEx( 
			hKey,
			sCFG_REG_SENDCHARBYCHAR_FIELD,
			0,
			REG_DWORD,
			(BYTE*)(&dwCFG_DEFAULT_SENDCHARBYCHAR ),
			sizeof(dwCFG_DEFAULT_SENDCHARBYCHAR)
			);
	}

    RegCloseKey( hKey );

	// Now create the global named event to communicate with the sample app
	g_hEvent = CreateEvent( NULL, FALSE, FALSE, L"SSKeyWedge" );

	return( hr );

} // End - IcfgInitialize


// See configlet2.h for details
CONFIGLET2_INT STDMETHODCALLTYPE IcfgUninitialize( VOID )
{
    HRESULT hr = S_OK;

    return( hr );
} // End - IcfgUninitialize


// See configlet2.h for details
CONFIGLET2_INT STDMETHODCALLTYPE IcfgSetField( CONFIGLET2_CSTR szFieldName,
                                               CONFIGLET2_CSTR szInstance,
                                               CONFIGLET2_CSTR szValue )
{
    HRESULT hr      = S_OK;
    LONG    lResult = ERROR_SUCCESS;

	// Convert the field name to Unicode
	WCHAR* pszFieldName = new WCHAR[BUFFERSIZE];  // to keep it simple, we'll just use a big buffer
	MultiByteToWideChar(
		CP_UTF8, 
		0, 
		szFieldName,
		-1, 
		pszFieldName, 
		BUFFERSIZE 
		);

    // If the field is the age, we're setting a DWORD
	if( 0 == wcscmp( pszFieldName, sCFG_REG_BAUDRATE_FIELD  ) ||
		0 == wcscmp( pszFieldName, sCFG_REG_DATABITS_FIELD ) ||
		0 == wcscmp( pszFieldName, sCFG_REG_HANDSHAKE_FIELD ) ||
		0 == wcscmp( pszFieldName, sCFG_REG_PARITY_FIELD  ) ||
		0 == wcscmp( pszFieldName, sCFG_REG_STOPBITS_FIELD  ) ||
		0 == wcscmp( pszFieldName, sCFG_REG_SENDCHARBYCHAR_FIELD ) )
    {
        INT iVal = atoi( const_cast< const CHAR * >( szValue ) );
        hr = CfgRegSetDWORD( hkCFG_ROOT_KEY, sCFG_REG_PATH, pszFieldName, ( DWORD )iVal );
    }
    
	// otherwise, if it's the name or the eye color, we're setting a string
	else if( 0 == wcscmp( pszFieldName, sCFG_REG_COMPORT_FIELD ) )
    {
		DWORD dwLength = MultiByteToWideChar(
			CP_UTF8, 
			0, 
			szValue,
			-1, 
			NULL, 
			0 
			);

		WCHAR* pszValue = new WCHAR[dwLength+1];
		
		MultiByteToWideChar(
			CP_UTF8, 
			0, 
			szValue,
			-1, 
			pszValue, 
			dwLength+1 
			);
        hr = CfgStoreSetString( hkCFG_ROOT_KEY, sCFG_REG_PATH, pszFieldName, pszValue );
		delete[] pszValue;
    }
    else
    {
        hr = E_FAIL;
    }
	delete[] pszFieldName;
    return( hr );
} // End - IcfgSetField


// See configlet2.h for details
CONFIGLET2_INT STDMETHODCALLTYPE IcfgGetField( CONFIGLET2_CSTR szFieldName,
                                               CONFIGLET2_CSTR szInstance,
                                               CONFIGLET2_INT  nMaxChars,
                                               CONFIGLET2_STR  szValue,
                                               CONFIGLET2_INT *pnValueChars )
{
    HRESULT        hr        = S_OK;
    LONG           lResult   = ERROR_SUCCESS;
    CONFIGLET2_INT iMaxChars = nMaxChars;

	WCHAR* pszFieldName = new WCHAR[BUFFERSIZE];
	DWORD dwLength = MultiByteToWideChar(
		CP_UTF8, 
		0, 
		szFieldName,
		-1, 
		pszFieldName, 
		BUFFERSIZE 
		);

    // Age is requested, so get a DWORD
	if( 0 == wcscmp( pszFieldName, sCFG_REG_BAUDRATE_FIELD  ) ||
		0 == wcscmp( pszFieldName, sCFG_REG_DATABITS_FIELD ) ||
		0 == wcscmp( pszFieldName, sCFG_REG_HANDSHAKE_FIELD ) ||
		0 == wcscmp( pszFieldName, sCFG_REG_PARITY_FIELD  ) ||
		0 == wcscmp( pszFieldName, sCFG_REG_STOPBITS_FIELD  ) ||
		0 == wcscmp( pszFieldName, sCFG_REG_SENDCHARBYCHAR_FIELD ) )
    {
		// Get the value out of the registry
		DWORD dwVal = 0;
		hr = CfgRegGetDWORD( hkCFG_ROOT_KEY, sCFG_REG_PATH, pszFieldName, &dwVal );
		
		// Convert it to text
		char digits[11];		// Biggest DWORD is 10 decimal digits, just allocate off the stack
		_ltoa( dwVal, digits, 10 );
		*pnValueChars = strlen(digits) + 1;   // These are numbers, always ASCII, so strlen works
		
		// Doesn't fit, return error
		if( nMaxChars < *pnValueChars )
		{
			hr = E_CONFIGLET2_DATA_TOO_BIG;
		}

		// Oh yes it does, return the digit string as instructed
		else
		{
			strcpy( szValue, digits );
		}

    }
    
	// Name or eye color requested, so get a string
	else if( 0 == wcscmp( pszFieldName, sCFG_REG_COMPORT_FIELD ) )
    {
		DWORD dwLen = 0;
        WCHAR* ppszValue = NULL;
		
		// If CfgStoreGetString succeeds, returns a buffer pointer in ppszValue.
		// WE MUST DELETE IT!!
		hr = CfgStoreGetString( hkCFG_ROOT_KEY, sCFG_REG_PATH, pszFieldName, &ppszValue, &dwLen );
		if( SUCCEEDED(hr) )
		{
			int nBytes = WideCharToMultiByte(
				CP_UTF8, 
				0, 
				ppszValue, 
				-1, 
				NULL, 
				0, 
				NULL, 
				NULL 
				);
			*pnValueChars = nBytes;

			// Data too big, return error
			if( nMaxChars < nBytes )
			{
				hr = E_CONFIGLET2_DATA_TOO_BIG;
			}
			
			// No it's not, convert and return result string
			else
			{
				WideCharToMultiByte(
					CP_UTF8, 
					0, 
					ppszValue, 
					-1, 
					szValue, 
					nMaxChars, 
					NULL, 
					NULL 
					);
			}
		}
		if( ppszValue )
		{
			delete[] ppszValue;
		}
    }
    else
    {
        hr = E_FAIL;
    }

	delete[] pszFieldName;

    return( hr );
} // End - IcfgGetField



// U T I L I T Y   M E T H O D S

HRESULT CfgStoreSetString( const HKEY hkRoot, LPCTSTR wsStorePath, LPCTSTR wsStoreItem, LPTSTR wsValue )
{
    HRESULT hr        = S_OK;
    DWORD   dwErr     = ERROR_SUCCESS;
    HKEY    hLocalKey = NULL;


    // Open the key
	LONG lRet = RegOpenKeyEx( hkRoot, wsStorePath, 0, 0, &hLocalKey );
    if( ERROR_SUCCESS == lRet )
    {        
		// Set the value
		DWORD dwLen = (wcslen(wsValue)+1)*sizeof(WCHAR);
        lRet = RegSetValueEx( hLocalKey, wsStoreItem, 0, REG_SZ, ( BYTE * )wsValue, dwLen );
        if( ERROR_SUCCESS != lRet )
        {
            hr = HRESULT_FROM_WIN32( lRet );
        }
		else
		{
			SetEvent( g_hEvent );
		}
    } // End - RegOpenKeyEx successful
    else
    {
        hr = HRESULT_FROM_WIN32( lRet );
    } // End - RegOpenKeyEx failed

    RegCloseKey( hLocalKey );

    // Close the key
	return( hr );
} // End - CfgStoreSetString


HRESULT CfgStoreGetString( const HKEY hkRoot, LPCTSTR wsStorePath, LPCTSTR wsStoreItem, WCHAR** ppwsValue, DWORD *pdwValueLen )
{

    HRESULT hr        = S_OK;
    DWORD   dwErr     = ERROR_SUCCESS;
    HKEY    hLocalKey = NULL;

	// Open the key
	LONG lRet = RegOpenKeyEx( hkRoot, wsStorePath, 0, 0, &hLocalKey );
    if( ERROR_SUCCESS == lRet )
    {
		// Key opened, get the length of the registry string
		DWORD dwType = 0;
		DWORD dwReadLen = 0;
        lRet = RegQueryValueEx( hLocalKey, wsStoreItem, 0, &dwType,
                                NULL, &dwReadLen );
		if( ERROR_SUCCESS == lRet )
		{
			// Value found, allocate the string (caller will delete)
			*ppwsValue = new WCHAR[dwReadLen/2+1];

			// Read it
			lRet = RegQueryValueEx( hLocalKey, wsStoreItem, 0, &dwType,
									(BYTE*)(*ppwsValue), &dwReadLen );
			if( ERROR_SUCCESS == lRet )
			{
				// Set the length return value
				*pdwValueLen = dwReadLen/2;
			}
		}
    } // End - RegOpenKeyEx successful

	hr = HRESULT_FROM_WIN32( lRet );
    RegCloseKey( hLocalKey );

    return( hr );
} // End - CfgStoreGetString


HRESULT CfgRegSetDWORD( const HKEY hKey, LPCTSTR wsStorePath, LPCTSTR wsStoreItem, DWORD dwValue )
{
    HRESULT hr        = S_OK;
    DWORD   dwErr     = ERROR_SUCCESS;
    HKEY    hLocalKey = NULL;


    LONG lRet = RegOpenKeyEx( hKey, wsStorePath, 0, 0, &hLocalKey );
    if( ERROR_SUCCESS == lRet )
    {
        DWORD dwType = 0;
        DWORD dwSize = sizeof( DWORD ); // indicate that we want sizeof(DWORD)
        lRet = RegSetValueEx( hLocalKey, wsStoreItem, NULL, REG_DWORD,
                              reinterpret_cast< const BYTE * >( &dwValue ),
                              static_cast< DWORD >( sizeof( dwValue ) ) );
		if( lRet == ERROR_SUCCESS )
		{
			SetEvent( g_hEvent );
		}
    } // End - RegOpenKeyEx failed
	
	hr = HRESULT_FROM_WIN32( lRet );
    return( hr );
} // End - RegSetDWORD


HRESULT CfgRegGetDWORD( const HKEY hKey, LPCTSTR wsStorePath, LPCTSTR wsStoreItem, DWORD *pdwValue )
{
    HRESULT hr        = S_OK;
    DWORD   dwErr     = ERROR_SUCCESS;
    HKEY    hLocalKey = NULL;

    LONG lRet = RegOpenKeyEx( hKey, wsStorePath, 0, 0, &hLocalKey );
    if( ERROR_SUCCESS == lRet )
    {
        DWORD dwType = REG_DWORD;
        DWORD dwSize = sizeof( DWORD ); 
        lRet = RegQueryValueEx( hLocalKey, wsStoreItem, NULL, &dwType,
                                reinterpret_cast< BYTE * >( pdwValue ), &dwSize );
    } // End - RegOpenKeyEx failed
    
	hr = HRESULT_FROM_WIN32( lRet );

    return( hr );
} // End - RegGetDWORD


// E N D   O F   F I L E
