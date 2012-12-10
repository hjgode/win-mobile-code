#include "registry.h"
//#include "RegEntry.h"

extern DWORD nBaud;
extern unsigned char nParity;
extern unsigned char nStopbits;// = ONESTOPBIT,
extern unsigned char nDatabits; // = 8,
extern unsigned char nHandshake;
extern TCHAR g_szCOM[32];
extern bool bsendcharbychar;
extern TCHAR g_szPostamble[32];
extern TCHAR g_szPreamble[32];
extern DWORD g_dwBeepAfterRead;

extern const LPCWSTR pSubKey; // = L"SOFTWARE\\Intermec\\SSKeyWedge";

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
		if ( regMyReg[L"Postamble"].Exists() ) 
			wsprintf(g_szPostamble, regMyReg[L"Postamble"]);
		else
			wsprintf(g_szPostamble, L"");
		//Read preamble
		if ( regMyReg[L"Preamble"].Exists() ) 
			wsprintf(g_szPreamble, regMyReg[L"Preamble"]);
		else
			wsprintf(g_szPreamble, L"");

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

		regMyReg[L"comport"]=g_szCOM;

		regMyReg[L"Postamble"]=g_szPostamble;
		regMyReg[L"Preamble"]=g_szPreamble;

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

