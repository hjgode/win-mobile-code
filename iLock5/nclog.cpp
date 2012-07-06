// --- nclog.cpp ---

//#include "winsock.h"
#include "winsock2.h"
#pragma comment (lib, "ws2.lib")

#include <stdarg.h>
#include <stdio.h>

static SOCKET wsa_socket=INVALID_SOCKET;
#pragma comment(lib , "winsock")

static unsigned short theLogPort=9998;

//file stuff
//global
static char	logFileName[MAX_PATH];
static TCHAR	logFileNameW[MAX_PATH];
static BOOL bFirstFileCall = true;
static int iUseLogging=0;
static BOOL bUseSocket=FALSE;

//max log file size ~250KByte
#define MAX_LOG_SIZE 0x0002FFFF

// enable disable logging globally
void nclogEnable(BOOL bEnable){
	if(bEnable)
		iUseLogging=1;
	else
		iUseLogging=0;
}

extern void nclogDisableSocket(BOOL bDisable){
	bUseSocket = !bDisable;
}

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
		//already loaded?
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
        if (wsa_bind(theLogPort)) 
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
	TCHAR lpFileName[MAX_PATH+1];
	GetModuleFileName(NULL, lpFileName, MAX_PATH); //lpFileName will contain the exe name of this running app!
	//add txt extension
	TCHAR txtFileName[MAX_PATH+1];
	wsprintf(txtFileName, L"%s.log.txt", lpFileName);
	//store the filename to use in char and tchar
	TCHAR logFileNameW[MAX_PATH];
	wsprintf(logFileNameW, txtFileName);
	wcstombs(logFileName, logFileNameW, sizeof(logFileNameW)*sizeof(logFileNameW[0]));

	FILE	*fp;
	fp = fopen(logFileName, "a+");
	//get the file size
	if(fseek(fp, 0, SEEK_END)==0){
		long iEnd = ftell(fp);
		if(iEnd > MAX_LOG_SIZE)
		{
			fclose(fp);
			fp=fopen(logFileName, "w+"); //creates an empty file
		}
	}

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

	//wsprintf(str, L"Date and Time: %s %s", lpDateStr, lpTimeStr);
	wsprintf(str, L"%s %s", lpDateStr, lpTimeStr);
	return str;
}

static int writefile(TCHAR *filetext){
	/* File Write Function, written by professor chemicalX */
	FILE	*fp;						/* Declare FILE structure */
	TCHAR  szTemp[255];
	char  szTempA[255];

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
	fclose(fp); /* Remember to close the file stream after calling file functions, */

	/* otherwise the file wont be created. */
	return 0;
}
//========================== end of file stuff =============================

// format input, convert to 8-bit and send.
void nclog (const wchar_t *fmt, ...)
{
		TCHAR StrW[1024];
        va_list vl;
        va_start(vl,fmt);
        wchar_t buf[1024]; // to bad CE hasn't got wvnsprintf
        wvsprintf(buf,fmt,vl);

		if(bUseSocket)
			wsa_init();
        char bufOut[512];
		
		//insert data/time
		wsprintf(StrW, L"%s: %s", logDateTime(), buf);
		wsprintf(buf, L"%s", StrW);

        WideCharToMultiByte(CP_ACP,0,buf,-1,bufOut,400, NULL, NULL);
	
	if(iUseLogging==1){
#ifdef DEBUG
		wsa_send(bufOut);
		DEBUGMSG(1, (buf));
#else
		if(bUseSocket)
			wsa_send(bufOut);

		RETAILMSG(1, (buf));
#endif
		writefile(buf);
	}//iUseLogging
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
