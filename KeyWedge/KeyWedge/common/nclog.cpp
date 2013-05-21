// --- nclog.cpp ---
#pragma once

#include <windows.h>
#include "winsock2.h"
#pragma comment (lib, "ws2.lib")

#include <stdarg.h>
#include <stdio.h>
#include "nclog.h"

static SOCKET wsa_socket=INVALID_SOCKET;
#pragma comment(lib , "winsock")

static unsigned short theLogPort;

//file stuff
//global
static char	logFileName[MAX_PATH];
static TCHAR	logFileNameW[MAX_PATH];
static BOOL bFirstFileCall = true;

bool bUseFileLog=FALSE;

// bind the log socket to a specific port.
static bool wsa_bind(unsigned short port)
{
    SOCKADDR_IN addr;
    addr.sin_family = AF_INET;
    addr.sin_port = htons(port);
    addr.sin_addr.s_addr = htonl(INADDR_ANY);
    int r=bind(wsa_socket,(sockaddr*)&addr,sizeof(addr));
    if (r==0) 
		theLogPort=port;
    return (r==0);
}

// initialize everything, if the socket isn't open.
static bool wsa_init()
{
        if (wsa_socket != INVALID_SOCKET) return true;
        int r;
        WSADATA wd;
        BOOL bc=true;

        if (0 != WSAStartup(0x101, &wd)) 
			goto error;
        wsa_socket=socket(PF_INET, SOCK_DGRAM, 0);
        if (wsa_socket == INVALID_SOCKET) 
			goto error;
        r=setsockopt(wsa_socket, SOL_SOCKET, SO_BROADCAST, (char*)&bc, sizeof(bc));
        if (r!=0) 
			goto error;
        if (wsa_bind(9998)) 
			return true; // bind to default port. 
	error:
        if (wsa_socket != INVALID_SOCKET) 
			closesocket(wsa_socket);
#ifdef DEBUG 
			OutputDebugString(TEXT("nclog: TCP/IP Problem"));
#endif
        return false;

}

// can be called externally to select a different port for operations
bool set_nclog_port(unsigned short x) { return wsa_bind(x); }

static void wsa_send(const char *x)
{
        SOCKADDR_IN sa;
        sa.sin_family = AF_INET;
        sa.sin_port = htons(theLogPort);
        sa.sin_addr.s_addr = htonl(INADDR_BROADCAST);

        if (SOCKET_ERROR == sendto(wsa_socket,x,strlen(x), 0, (sockaddr*) &sa, sizeof(sa)))
        {
#ifdef DEBUG
        //if (debug_mode) 
//			OutputDebugString(TEXT("nclog: Send Error"));
#endif
        }

}

//========================== start of file stuff =============================
static int initFileNames()
{
	 // Get name of executable
	TCHAR lpFileName1[MAX_PATH+1];
	GetModuleFileName(NULL, lpFileName1, MAX_PATH); //lpFileName will contain the exe name of this running app!

	//add txt extension
	TCHAR txtFileName[MAX_PATH+1];
	wsprintf(txtFileName, L"%s.log.txt", lpFileName1);
	//store the filename to use in char and tchar
	TCHAR logFileNameW[MAX_PATH];

	//#### we maintain two log files, an actual and a bak one
	//get file size
	HANDLE hFile = CreateFile(txtFileName, 
       GENERIC_READ,          // open for writing
       FILE_SHARE_READ,       // share
       NULL,                   // default security
       OPEN_EXISTING,          // open existing
       FILE_ATTRIBUTE_NORMAL,  // normal file
       NULL);                  // no attr. template
	DWORD dwFileSize = GetFileSize(hFile, NULL);
	CloseHandle(hFile);

	if(dwFileSize != 0xFFFFFFFF){ //no error
		if(dwFileSize>0x100000){ //more than 1MB?
			//make a backup
			//delete previous bak
			TCHAR txtFileNameBAK[MAX_PATH];
			wsprintf(txtFileNameBAK, L"%s.bak", txtFileName);
			DeleteFile(txtFileNameBAK);
			//rename old file to .BAK
			MoveFile(txtFileName, txtFileNameBAK);
		}
	}

	//copy filename to global char and tchar var
	wsprintf(logFileNameW, txtFileName);
	wcstombs(logFileName, logFileNameW, sizeof(logFileNameW)*sizeof(logFileNameW[0]));

	FILE	*fp;
	fp = fopen(logFileName, "a+");
	fclose(fp);
	bFirstFileCall=false;
	return 0;
}

TCHAR* logDateTime(){
	static TCHAR str[64];
	TCHAR lpTimeStr[32];
	TCHAR lpDateStr[32];
	LONG res;
	wsprintf(str,L"");
	//Read the system time
	res = GetTimeFormat(LOCALE_SYSTEM_DEFAULT,
							TIME_FORCE24HOURFORMAT,
							NULL,
							L"hh:mm:ss",
							lpTimeStr,
							sizeof (lpTimeStr ) * sizeof(TCHAR));
	if (res == 0)
	{
		wcscpy(lpTimeStr, L"err");
	}

	//Read the system date
	res = GetDateFormat(  LOCALE_SYSTEM_DEFAULT,
						  NULL,
						  NULL,
						  L"dd.MM.yyyy",
						  lpDateStr,
						  sizeof (lpDateStr) * sizeof(TCHAR));
	if (res == 0)
	{
		wcscpy(lpDateStr, L"err");
	}

	wsprintf(str, L"Date and Time: %s %s", lpDateStr, lpTimeStr);
	return str;
}

static int writefile(TCHAR *filetext){
	/* File Write Function, written by professor chemicalX */
	FILE	*fp;						/* Declare FILE structure */
	TCHAR  szTemp[1024];
	char  szTempA[1024];

	wsprintf(szTemp, L"%s", filetext);
	wcstombs(szTempA, szTemp, sizeof(szTemp)/sizeof(TCHAR));

	if (bFirstFileCall){
		// Get name of executable
		initFileNames();
	}

	fp = fopen(logFileName, "a+");

	/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
	/* First of we open the file supplied by the filename paremeter */

	/*
	 * in the "a+" mode for appending, so if it doesnt exist its created. £
	 * fp = fopen(filename,"w"); // Open using the "w" mode for writing.
	 */
	long	fsize = strlen(szTempA);	/* Declare the long fsize with the length of the filetext */
	/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

	/* paremeter witch stores the text or data to right to the file. */
	fwrite(szTempA, 1, fsize, fp);		/* Write File */
	DEBUGMSG(1, (L"%s", filetext));
	fclose(fp); /* Remember to close the file stream after calling file functions, */

	/* otherwise the file wont be created. */
	return 0;
}

int writefile (const wchar_t *fmt ...){
    va_list vl;
    va_start(vl,fmt);
    wchar_t buf[1024]; // to bad CE hasn't got wvnsprintf
    wvsprintf(buf,fmt,vl);
	writefile(buf);
	return 0;
}

//========================== end of file stuff =============================

// format input, convert to 8-bit and send.
void nclog (const wchar_t *fmt, ...)
{
    va_list vl;
    va_start(vl,fmt);
    wchar_t buf[1024]; // to bad CE hasn't got wvnsprintf
    wvsprintf(buf,fmt,vl);

#ifdef DEBUG
	DEBUGMSG(1, (buf));
#else
	RETAILMSG(1, (buf));
#endif
	if(bUseFileLog==FALSE)
		return;


    wsa_init();
    char bufOut[512];
    WideCharToMultiByte(CP_ACP,0,buf,-1,bufOut,400, NULL, NULL);
    wsa_send(bufOut);

	writefile(buf);
}

// finalize the socket on program termination.
struct _nclog_module
{
        ~_nclog_module()
        {
                if (wsa_socket!=INVALID_SOCKET)
                {
                        nclog(L"nclog goes down\n");
                        shutdown(wsa_socket,2);
                        closesocket(wsa_socket);
                }
        }

};

static _nclog_module module; 
