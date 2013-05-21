//regRW.cpp

#include "RegRW.h"

//	convert a input string with \r, \n\, \t encoding
//	to a string with these vals decoded
static TCHAR* stringDecoded(TCHAR* szSringIn, int maxlen){

	if(szSringIn==NULL || wcslen(szSringIn)==0)
		return L"";

	static TCHAR* sRet = new TCHAR[MAX_BUFSIZE];
	memset(sRet, 0, MAX_BUFSIZE);
	wsprintf(sRet, L"");
	int i=0;

	tstring cString(szSringIn);

	while (cString.find(L"\\r")!=string::npos){
		cString.replace(cString.find(L"\\r"), 2,  L"\r");
	}
	while(cString.find(L"\\n")!=string::npos)
		cString.replace(cString.find(L"\\n"), 2,  L"\n");

	while(cString.find(L"\\t")!=string::npos)
		cString.replace(cString.find(L"\\t"), 2,  L"\t");

	wsprintf(sRet, L"%s", cString.c_str());

	return sRet; 
}

//	convert a input string with control codes
//	to a string with \r, \n\, \t encoding
TCHAR* stringEncoded(TCHAR * szSringIn, int maxLen){

	if(wcslen(szSringIn)==0)
		return L"";

	static TCHAR* sRet = new TCHAR[maxLen];
	TCHAR wIn[32] ={ 0 };
	TCHAR* szIn = wIn;
	wsprintf(szIn, L"%s", szSringIn);
	wsprintf(sRet, L"");
	int i=0;
	while(szIn != L'\0')
	{
		if(szIn == L"\r")
			wcscat(sRet, L"\\r");
		else if(szIn == L"\n")
			wcscat(sRet, L"\\n");
		else if(szIn == L"\t")
			wcscat(sRet, L"\\t");
		else
		{
			//sRet[i] = szIn;
			wcsncat(sRet, szIn, 1);
		}
		szIn++;
		i++;
	}
	sRet[i] = L'\0';
	return sRet; 
}

int ReadReg()
{
	CRegistry regMyReg( CREG_CREATE );  // No special flags
	if ( regMyReg.Open(pSubKey, HKEY_LOCAL_MACHINE) ) 
	{
//		TCHAR str[MAX_PATH+1];
		byte b=0;
//		LONG rc;
		DWORD dw=0;

		//Read if send char by char
		if ( regMyReg[L"sendcharbychar"].Exists() )
		{
			dw = regMyReg[L"sendcharbychar"];
			if (dw==0)
				bsendcharbychar=false;
			else if (dw==1)
				bsendcharbychar=true;
		}
		else
		{
			bsendcharbychar=true;
		}
		
		//Read if send via SendChars()
		if ( regMyReg[L"usecharsend"].Exists() )
		{
			dw = regMyReg[L"usecharsend"];
			if (dw==0)
				g_bUseCharSend=FALSE;
			else if (dw==1)
				g_bUseCharSend=TRUE;
		}
		else
		{
			g_bUseCharSend=FALSE;
		}

		//Read COM port
		if ( regMyReg[L"comport"].Exists() ) 
			wsprintf(g_szCOM, regMyReg[L"comport"]);
		else
			wsprintf(g_szCOM, L"COM1:");

		//Read Baudrate
		if ( regMyReg[L"baudrate"].Exists() ) 
			nBaud = regMyReg[L"baudrate"];
		else
			nBaud = CBR_9600;

		//Read parity
		if ( regMyReg[L"parity"].Exists() ) 
			nParity = (unsigned short)(DWORD) regMyReg[L"parity"];
		else
			nParity = NOPARITY;

		//Read stopbits
		if ( regMyReg[L"stopbits"].Exists() ) 
			nStopbits = (unsigned short)(DWORD) regMyReg[L"stopbits"];
		else
			nStopbits = ONESTOPBIT;

		//Read databits
		if ( regMyReg[L"databits"].Exists() ) 
			nDatabits = (unsigned short)(DWORD)regMyReg[L"Databits"];
		else
			nDatabits = 8;

		//Read nHandshake
		if ( regMyReg[L"handshake"].Exists() ) 
			nHandshake = (unsigned short)(DWORD)regMyReg[L"handshake"];
		else
			nHandshake = 3;

		//the keytable
		//regMyReg[L"keytab"].GetBinary((VOID *) vkTable, sizeof(KTABLE)*128);

		//Read postamble
		if ( regMyReg[L"Postamble"].Exists() ) {
			TCHAR* szTemp = regMyReg[L"Postamble"];
			if(szTemp!=NULL){
				wsprintf(g_szPostamble, L"%s", szTemp);
			}
			if(wcslen(g_szPostamble)>0){
				wsprintf(g_szPostambleDecoded, L"%s", stringDecoded(g_szPostamble, 32));
			}
			else
			{
				wsprintf(g_szPostamble, L"");
				wsprintf(g_szPostambleDecoded, L"");
			}
		}
		else{
			wsprintf(g_szPostamble, L"");
			wsprintf(g_szPostambleDecoded, L"");
		}
		//Read preamble
		if ( regMyReg[L"Preamble"].Exists() ) {
			wsprintf(g_szPreamble, regMyReg[L"Preamble"]);
			if(wcslen(g_szPreamble)>0){
				wsprintf(g_szPreambleDecoded, L"%s", stringDecoded(g_szPreamble, 32));
			}
			else{
				wsprintf(g_szPreamble, L"");//=NULL;
				wsprintf(g_szPreambleDecoded, L"");//=NULL;
			}
		}
		else{
			wsprintf(g_szPreamble, L"");//=NULL;
			wsprintf(g_szPreambleDecoded, L"");//=NULL;
		}
		//Read BeepAfterRead
		if ( regMyReg[L"BeepAfterRead"].Exists() ) 
			g_dwBeepAfterRead = (unsigned short)(DWORD)regMyReg[L"BeepAfterRead"];
		else
			g_dwBeepAfterRead = 1;

		regMyReg.Close();
	}
	return 0;
}

void WriteReg()
{
	CRegistry regMyReg( CREG_CREATE );  // No special flags
	if ( regMyReg.Open(pSubKey, HKEY_LOCAL_MACHINE) ) 
	{
		DWORD dw;
//		byte  b;

		//the keytable
		regMyReg[L"keytab"].SetBinary((UCHAR*) &vkTable, sizeof(KTABLE)*KTAB_SIZE);

	//	pKTABLE * pvkTable = new pKTABLE[128];
	//	*pvkTable = &vkTable[0];

		if (bsendcharbychar)
			dw=1;
		else
			dw=0;

		regMyReg[L"sendcharbychar"]=dw;

		regMyReg[L"UseCharSend"]=g_bUseCharSend;

		regMyReg[L"comport"]=g_szCOM;

		if(g_szPostambleDecoded != NULL){
			wsprintf(g_szPostamble, stringEncoded(g_szPostambleDecoded, 32));
			regMyReg[L"Postamble"]=g_szPostamble;
		}
		else
		{
			regMyReg[L"Postamble"]=L"";
		}
		if(g_szPreambleDecoded != NULL){
			wsprintf(g_szPreamble, stringEncoded(g_szPreambleDecoded, 32));
			regMyReg[L"Preamble"]=g_szPreamble;
		}
		else
		{
			regMyReg[L"Preamble"]=L"";
		}

		dw=g_dwBeepAfterRead;
		regMyReg[L"BeepAfterRead"]=dw;

		dw=nBaud;
		regMyReg[L"baudrate"]=dw;

		dw=nHandshake;
		regMyReg[L"handshake"]=dw;

		dw=nDatabits;
		regMyReg[L"databits"]=dw;

		dw=nStopbits;
		regMyReg[L"stopbits"]=dw;

		dw=nParity;
		regMyReg[L"parity"]=dw;

		regMyReg.Close();
	}
}

