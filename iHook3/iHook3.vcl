<html>
<body>
<pre>
<h1>Build Log</h1>
<h3>
--------------------Configuration: iHook3 - Win32 (WCE ARMV4I) Debug--------------------
</h3>
<h3>Command Lines</h3>
Creating temporary file "C:\Temp\RSP1D.tmp" with contents
[
/nologo /W3 /Zi /Od /D "DEBUG" /D "ARM" /D "_ARM_" /D "ARMV4I" /D UNDER_CE=500 /D _WIN32_WCE=500 /D "WCE_PLATFORM_cv30prem" /D "UNICODE" /D "_UNICODE" /FR"ARMV4IDbg/" /Fo"ARMV4IDbg/" /Fd"ARMV4IDbg/" /QRarch4T /QRinterwork-return /MC /c 
"D:\C-Source\Active\iHook3\iHook3.cpp"
]
Creating command line "clarm.exe @C:\Temp\RSP1D.tmp" 
Creating temporary file "C:\Temp\RSP1E.tmp" with contents
[
commctrl.lib coredll.lib /nologo /base:"0x00010000" /stack:0x10000,0x1000 /entry:"WinMainCRTStartup" /incremental:yes /pdb:"ARMV4IDbg/iHook3.pdb" /debug /nodefaultlib:"libc.lib /nodefaultlib:libcd.lib /nodefaultlib:libcmt.lib /nodefaultlib:libcmtd.lib /nodefaultlib:msvcrt.lib /nodefaultlib:msvcrtd.lib" /out:"ARMV4IDbg/iHook3.exe" /subsystem:windowsce,5.00 /MACHINE:THUMB 
".\ARMV4IDbg\iHook3.obj"
".\ARMV4IDbg\StdAfx.obj"
".\ARMV4IDbg\iHook3.res"
]
Creating command line "link.exe @C:\Temp\RSP1E.tmp"
<h3>Output Window</h3>
Compiling...
iHook3.cpp
Linking...
   Creating library ARMV4IDbg/iHook3.lib and object ARMV4IDbg/iHook3.exp
corelibc.lib(pegwmain.obj) : warning LNK4209: debugging information corrupt; recompile module; linking object as if no debug info
corelibc.lib(crt0dat.obj) : warning LNK4209: debugging information corrupt; recompile module; linking object as if no debug info
corelibc.lib(crt0init.obj) : warning LNK4209: debugging information corrupt; recompile module; linking object as if no debug info
Creating command line "bscmake.exe /nologo /o"ARMV4IDbg/iHook3.bsc"  ".\ARMV4IDbg\StdAfx.sbr" ".\ARMV4IDbg\iHook3.sbr""
Creating browse info file...
<h3>Output Window</h3>




<h3>Results</h3>
iHook3.exe - 0 error(s), 3 warning(s)
</pre>
</body>
</html>
