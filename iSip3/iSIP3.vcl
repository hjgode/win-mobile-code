<html>
<body>
<pre>
<h1>Build Log</h1>
<h3>
--------------------Configuration: iSIP3 - Win32 (WCE ARMV4I) Release--------------------
</h3>
<h3>Command Lines</h3>
Creating command line "rc.exe /l 0x409 /fo"ARMV4IRel/iSIP3.res" /d UNDER_CE=420 /d _WIN32_WCE=420 /d "UNICODE" /d "_UNICODE" /d "NDEBUG" /d "WCE_PLATFORM_ICE-PREMIUM" /d "THUMB" /d "_THUMB_" /d "ARM" /d "_ARM_" /d "ARMV4I" /r "D:\C-Source\Active\iSIP3\iSIP3.rc"" 
Creating temporary file "G:\Temp\RSP3A3.tmp" with contents
[
/nologo /W3 /D _WIN32_WCE=420 /D "ARM" /D "_ARM_" /D "WCE_PLATFORM_ICE-PREMIUM" /D "ARMV4I" /D UNDER_CE=420 /D "UNICODE" /D "_UNICODE" /D "NDEBUG" /FR"ARMV4IRel/" /Fp"ARMV4IRel/iSIP3.pch" /Yu"stdafx.h" /Fo"ARMV4IRel/" /QRarch4T /QRinterwork-return /O2 /MC /c 
"D:\C-Source\Active\iSIP3\iSIP3.cpp"
]
Creating command line "clarm.exe @G:\Temp\RSP3A3.tmp" 
Creating temporary file "G:\Temp\RSP3A4.tmp" with contents
[
/nologo /W3 /D _WIN32_WCE=420 /D "ARM" /D "_ARM_" /D "WCE_PLATFORM_ICE-PREMIUM" /D "ARMV4I" /D UNDER_CE=420 /D "UNICODE" /D "_UNICODE" /D "NDEBUG" /FR"ARMV4IRel/" /Fp"ARMV4IRel/iSIP3.pch" /Yc"stdafx.h" /Fo"ARMV4IRel/" /QRarch4T /QRinterwork-return /O2 /MC /c 
"D:\C-Source\Active\iSIP3\StdAfx.cpp"
]
Creating command line "clarm.exe @G:\Temp\RSP3A4.tmp" 
Creating temporary file "G:\Temp\RSP3A5.tmp" with contents
[
commctrl.lib coredll.lib /nologo /base:"0x00010000" /stack:0x10000,0x1000 /entry:"WinMainCRTStartup" /incremental:no /pdb:"ARMV4IRel/iSIP3.pdb" /nodefaultlib:"libc.lib /nodefaultlib:libcd.lib /nodefaultlib:libcmt.lib /nodefaultlib:libcmtd.lib /nodefaultlib:msvcrt.lib /nodefaultlib:msvcrtd.lib" /out:"ARMV4IRel/iSIP3.exe" /subsystem:windowsce,4.20 /MACHINE:THUMB 
".\ARMV4IRel\iSIP3.obj"
".\ARMV4IRel\StdAfx.obj"
".\ARMV4IRel\iSIP3.res"
]
Creating command line "link.exe @G:\Temp\RSP3A5.tmp"
<h3>Output Window</h3>
Compiling resources...
Compiling...
StdAfx.cpp
Compiling...
iSIP3.cpp
d:\c-source\active\isip2\locktaskbar.h(280) : warning C4800: 'int' : forcing value to bool 'true' or 'false' (performance warning)
d:\c-source\active\isip2\locktaskbar.h(285) : warning C4800: 'int' : forcing value to bool 'true' or 'false' (performance warning)
d:\c-source\active\isip2\locktaskbar.h(290) : warning C4305: '=' : truncation from 'const int' to 'bool'
Linking...
LINK : warning LNK4089: all references to 'toolhelp.dll' discarded by /OPT:REF
Creating command line "bscmake.exe /nologo /o"ARMV4IRel/iSIP3.bsc"  ".\ARMV4IRel\StdAfx.sbr" ".\ARMV4IRel\iSIP3.sbr""
Creating browse info file...
<h3>Output Window</h3>




<h3>Results</h3>
iSIP3.exe - 0 error(s), 4 warning(s)
</pre>
</body>
</html>
