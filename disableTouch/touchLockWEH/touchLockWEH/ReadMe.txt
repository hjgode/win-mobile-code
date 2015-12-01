========================================================================
    WIN32 APPLICATION : touchLockWEH Project Overview
========================================================================

touchLockWEH will lock the touch screen

you need to enter the correct code within a time frame to unlock the touch
screen

if no code is entered within a timout, the device will be suspended.

========================================================================
configuration options
========================================================================

%<--------------------------------------------------
REGEDIT4

[HKEY_LOCAL_MACHINE\Software\Intermec\touchLockWEH]
"bkcolor"=dword:001FAB0F
"code"="00000"
%<--------------------------------------------------

code is a string holding the key sequence for unlock. Only numbers allowed!
default is "52401"

bkColor is the window background color as RGB hex value
default is 0xAABBCC

========================================================================
versions
========================================================================

v 0.2
	initial version
	
/////////////////////////////////////////////////////////////////////////////s