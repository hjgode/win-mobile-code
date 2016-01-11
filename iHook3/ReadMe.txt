//history
//version	change
// 1.0		initial release
// 1.1		added registry functions, now the keys are configured by the registry
/*
			[HKEY_LOCAL_MACHINE\SOFTWARE\Intermec\iHook3]
			"ForwardKey"=hex:0
			"arg0"=""
			"exe0"="\\windows\\iRotateCN2.exe"
			"key0"=hex:\
				  72
			"arg1"="-toggle"
			"exe1"="\\windows\\LockTaskBar.exe"
			"key1"=hex:\
				  73
			"arg2"=""
			"exe2"="\\windows\\iSIP2.exe"
			"key2"=hex:\
				  74
			"arg3"=""
			"exe3"="explorer.exe"
			"key3"=hex:\
				  71
			"arg4"="iRun2.exe"
			"exe4"="\\windows\\iKill2.exe"
			"key4"=hex:\
				  1b
*/
/*			would only load once
			added arg -writereg to write default registry
	1.2		added CloseHandle on CreateProcess(...pi)
	1.3		20.6.2005
			added function IsIntermec to registry.h and now checks, if this is an intermec
	2.0		changed isIntermec in registry.h for CK60, which does not support platform/name
			added notification code (needs WS_OVERLAPPED as WinStyle for Main Window!!!!)
	3.1.1	changed bForward handling: did not forward any key if false
			now only processed keys are not forwarded if bForward=false
	3.1.2	changed isIntermec to look for itcscan.dll only
			added bDoCheckIntermec and cmd argument to enable to run on non-intermec devices
*/

optional arg:
	-writereg	write default reg entry samples
	-nointermec	do not check "is Intermec device"
	
========================================================================
       Windows CE APPLICATION : iHook3
========================================================================


AppWizard has created this iHook3 application for you.  

This file contains a summary of what you will find in each of the files that
make up your iHook3 application.

iHook3.cpp
    This is the main application source file.

iHook3.vcp
    This file (the project file) contains information at the project level and
    is used to build a single project or subproject. Other users can share the
    project (.vcp) file, but they should export the makefiles locally.
	

/////////////////////////////////////////////////////////////////////////////
Other standard files:

StdAfx.h, StdAfx.cpp
    These files are used to build a precompiled header (PCH) file
    named iHook3.pch and a precompiled types file named StdAfx.obj.


/////////////////////////////////////////////////////////////////////////////
Other notes:

AppWizard uses "TODO:" to indicate parts of the source code you
should add to or customize.


/////////////////////////////////////////////////////////////////////////////
