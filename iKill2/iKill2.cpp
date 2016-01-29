// iKill2.cpp : Defines the entry point for the application.
//

#include "stdafx.h"
#include "locktaskbar.h"

//global for arguments array
int          argc; 
TCHAR**       argv; 

// Returns number of elements
#define dim(x) (sizeof(x) / sizeof(x[0])) 

int SplitArgs(LPTSTR lpCmdLine)
{
  unsigned int i; 
  int          j; 
  // parse a few of the command line arguments 
  // a space delimites an argument except when it is inside a quote 

  argc = 1; 
  int pos = 0; 
  for (i = 0; i < wcslen(lpCmdLine); i++) 
    { 
    while (lpCmdLine[i] == ' ' && i < wcslen(lpCmdLine)) 
      { 
      i++; 
      } 
    if (lpCmdLine[i] == '\"') 
      { 
      i++; 
      while (lpCmdLine[i] != '\"' && i < wcslen(lpCmdLine)) 
        { 
        i++; 
        pos++; 
        } 
      argc++; 
      pos = 0; 
      } 
    else 
      { 
      while (lpCmdLine[i] != ' ' && i < wcslen(lpCmdLine)) 
        { 
        i++; 
        pos++; 
        } 
      argc++; 
      pos = 0; 
      } 
    } 

  argv = (TCHAR**)malloc(sizeof(TCHAR*)* (argc+1)); 

  argv[0] = (TCHAR*)malloc(1024); 
  GetModuleFileName(0, argv[0],1024); 

  for(j=1; j<argc; j++) 
    { 
    argv[j] = (TCHAR*)malloc(wcslen(lpCmdLine)+10); 
    } 
  argv[argc] = 0; 

  argc = 1; 
  pos = 0; 
  for (i = 0; i < wcslen(lpCmdLine); i++) 
    { 
    while (lpCmdLine[i] == ' ' && i < wcslen(lpCmdLine)) 
      { 
      i++; 
      } 
    if (lpCmdLine[i] == '\"') 
      { 
      i++; 
      while (lpCmdLine[i] != '\"' && i < wcslen(lpCmdLine)) 
        { 
        argv[argc][pos] = lpCmdLine[i]; 
        i++; 
        pos++; 
        } 
      argv[argc][pos] = '\0'; 
      argc++; 
      pos = 0; 
      } 
    else 
      { 
      while (lpCmdLine[i] != ' ' && i < wcslen(lpCmdLine)) 
        { 
        argv[argc][pos] = lpCmdLine[i]; 
        i++; 
        pos++; 
        } 
      argv[argc][pos] = '\0'; 
      argc++; 
      pos = 0; 
      } 
    } 
  argv[argc] = 0; 

// Initialize the processes and start the application. 
//  retVal = MyMain(argc, argv); 

  // Delete arguments 
  TCHAR str[MAX_PATH+1];
  for(j=0; j<argc; j++) 
	{ 
		wsprintf(str, L"arg %i = %s\n", j, argv[j]);
		OutputDebugString(str);
		//free(argv[j]); 
	} 
  //free(argv); 
	return argc;
}

int WINAPI WinMain(	HINSTANCE hInstance,
					HINSTANCE hPrevInstance,
					LPTSTR    lpCmdLine,
					int       nCmdShow)
{
 	// TODO: Place code here.
	TCHAR exename[128];
	int iArgCount=0;
	
	iArgCount=SplitArgs(lpCmdLine); //arg 0 = process name, arg 1 = first arg, arg 2 = second arg
	if (iArgCount > 1)
	{
		if (iArgCount == 2)
		{
			wcscpy(exename, argv[1]);
			if (KillExeWindow(_wcslwr(exename), false))
				return 1;
		}
		else if (iArgCount == 3)
		{
			if (wcscmp(argv[1], L"-f") == 0) //argument -f used?
			{
				wcscpy(exename, argv[2]);
				if (KillExeWindow(_wcslwr(exename), true))
					return 1;
			}
			else
				return -2; //wrong arg
		}
		else
			return -3; //wrong arg count

	}
	else
		return 0;
	return -1;

}