Copy

  touchLockWEH.exe

to "\Flash File Store" (or "\Windows") directory.

touchLockWEH locks the touch screen when started.

To unlock the screen, enter the number 52401 within 5 seconds. Watch the screen of what is entered.

# Integrate with Intermec Launcher

To integrate touchLockWEH within Intermec Launcher and assign a key use the following configuration (touchLockWEH assigned as application 3 and to APP key 1):

<?xml version="1.0" encoding="UTF-8" ?> 
<DevInfo Action="Set" Persist="true">
  <Subsystem Name="Intermec Launcher">
    <Group Name="Application Launch Buttons">
      <Group Name="Application Button 3">
        <Field Name="Executable Path:">\Flash File Store\touchLockWEH.exe</Field> 
        <Field Name="Caption:">LOCK screen</Field> 
      </Group>
    </Group>
    <Group Name="Keypad Options">
      <Group Name="Application Shortcut Keys">
        <Field Name="Application Key 1 Path:">\Flash File Store\touchLockWEH.exe</Field> 
      </Group>
    </Group>
  </Subsystem>
</DevInfo>

What hardware key is assigned as what APP key is defined with the Device Resource Kit "Keyboard Remapper Utility".

# history

v0.1:
  first official release

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
	
/////////////////////////////////////////////////////////////////////////////