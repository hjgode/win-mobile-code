//regRW.h
#pragma once

#ifndef _REGRW_
#define _REGRW_
	#include "registry.h"
	#include "keymap.h"

	#define MAX_BUFSIZE 1000

	extern DWORD nBaud;
	extern unsigned char nParity;
	extern unsigned char nStopbits;// = ONESTOPBIT,
	extern unsigned char nDatabits; // = 8,
	extern unsigned char nHandshake;
	extern TCHAR g_szCOM[32];

	extern BOOL g_bUseCharSend;
	extern BOOL bsendcharbychar;

	//extern TCHAR* g_szPostamble;
	extern TCHAR g_szPostamble[32];
	extern TCHAR* g_szPreamble;
	extern TCHAR* g_szPostambleDecoded;
	extern TCHAR* g_szPreambleDecoded;

	extern DWORD g_dwBeepAfterRead;

	extern const LPCWSTR pSubKey; // = L"SOFTWARE\\Intermec\\SSKeyWedge";

	static TCHAR* stringDecoded(TCHAR* szSringIn, int maxlen);
	TCHAR* stringEncoded(TCHAR * szSringIn, int maxLen);

	int ReadReg();
	void WriteReg();

#endif //_REGRW_