<html>
<body>
<pre>
<h1>Build Log</h1>
<h3>
--------------------Configuration: iKill2 - Win32 (WCE ARMV4I) Release--------------------
</h3>
<h3>Command Lines</h3>
Creating command line "rc.exe /l 0x409 /fo"ARMV4IRel/iKill2.res" /d UNDER_CE=500 /d _WIN32_WCE=500 /d "UNICODE" /d "_UNICODE" /d "NDEBUG" /d "WCE_PLATFORM_CK60Prem" /d "THUMB" /d "_THUMB_" /d "ARM" /d "_ARM_" /d "ARMV4I" /r "D:\C-Source\Archive\iLock Set2\iKill2\iKill2.rc"" 
Creating temporary file "c:\TEMP\RSP47.tmp" with contents
[
/nologo /W3 /GX /D _WIN32_WCE=500 /D "ARM" /D "_ARM_" /D "WCE_PLATFORM_CK60Prem" /D "ARMV4I" /D UNDER_CE=500 /D "UNICODE" /D "_UNICODE" /D "NDEBUG" /FR"ARMV4IRel/" /Fo"ARMV4IRel/" /QRarch4T /QRinterwork-return /O2 /MC /c 
"D:\C-Source\Archive\iLock Set2\iKill2\iKill2.cpp"
]
Creating command line "clarm.exe @c:\TEMP\RSP47.tmp" 
Creating temporary file "c:\TEMP\RSP48.tmp" with contents
[
/nologo /W3 /GX /D _WIN32_WCE=500 /D "ARM" /D "_ARM_" /D "WCE_PLATFORM_CK60Prem" /D "ARMV4I" /D UNDER_CE=500 /D "UNICODE" /D "_UNICODE" /D "NDEBUG" /FR"ARMV4IRel/" /Fp"ARMV4IRel/iKill2.pch" /Yc"stdafx.h" /Fo"ARMV4IRel/" /QRarch4T /QRinterwork-return /O2 /MC /c 
"D:\C-Source\Archive\iLock Set2\iKill2\StdAfx.cpp"
]
Creating command line "clarm.exe @c:\TEMP\RSP48.tmp" 
Creating temporary file "c:\TEMP\RSP49.tmp" with contents
[
commctrl.lib coredll.lib /nologo /base:"0x00010000" /stack:0x10000,0x1000 /entry:"WinMainCRTStartup" /incremental:no /pdb:"ARMV4IRel/iKill2.pdb" /nodefaultlib:"libc.lib /nodefaultlib:libcd.lib /nodefaultlib:libcmt.lib /nodefaultlib:libcmtd.lib /nodefaultlib:msvcrt.lib /nodefaultlib:msvcrtd.lib" /out:"ARMV4IRel/iKill2.exe" /subsystem:windowsce,5.00 /MACHINE:THUMB 
".\ARMV4IRel\iKill2.obj"
".\ARMV4IRel\StdAfx.obj"
".\ARMV4IRel\iKill2.res"
]
Creating command line "link.exe @c:\TEMP\RSP49.tmp"
<h3>Output Window</h3>
Compiling resources...
Compiling...
StdAfx.cpp
c:\programme\windows ce tools\wce500\windows mobile 5.0 pocket pc sdk\include\armv4i\windbase.h(662) : warning C4068: unknown pragma
Compiling...
iKill2.cpp
c:\programme\windows ce tools\wce500\windows mobile 5.0 pocket pc sdk\include\armv4i\windbase.h(662) : warning C4068: unknown pragma
Linking...




<h3>Results</h3>
iKill2.exe - 0 error(s), 2 warning(s)
</pre>
</body>
</html>
