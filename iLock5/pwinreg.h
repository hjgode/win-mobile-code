//
// Copyright (c) Microsoft Corporation.  All rights reserved.
//
//
// Use of this source code is subject to the terms of the Microsoft end-user
// license agreement (EULA) under which you licensed this SOFTWARE PRODUCT.
// If you did not accept the terms of the EULA, you are not authorized to use
// this source code. For a copy of the EULA, please see the LICENSE.RTF on your
// install media.
//
/*++
THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
PARTICULAR PURPOSE.

Module Name:  

pwinreg.h

Abstract:

Private portion of winreg.h

Notes: 


--*/
    
   
    
#ifndef _PRIV_WINREG_H_
#define _PRIV_WINREG_H_

#ifdef __cplusplus
extern "C"  {
#endif

BOOL RegCopyFile(LPCWSTR lpszFile);
BOOL RegRestoreFile(LPCWSTR lpszFile);
LONG RegSaveKey(HKEY hKey, LPCWSTR lpszFile,
                LPSECURITY_ATTRIBUTES lpSecurityAttributes);
LONG RegReplaceKey(HKEY hKey, LPCWSTR lpszSubKey, LPCWSTR lpszNewFile,
                   LPCWSTR lpszOldFile);

// User profile OAK functions
BOOL SetCurrentUser(LPCWSTR lpszUserName, LPBYTE lpbUserData, DWORD dwDataSize,
                    BOOL bCreateIfNew);
BOOL SetUserData(LPBYTE lpbUserData, DWORD dwDataSize);
BOOL GetUserDirectory(LPWSTR lpszBuffer, LPDWORD lpdwSize);

// GetUserInformation flags -- used by GetUserNameEx and GetUserDirectory
#define USERINFO_NAME       1
#define USERINFO_DIRECTORY  2

// This function is also exported by FILESYS
BOOL CeGenRandom(DWORD dwLen, PBYTE pbBuffer);

#define REG_WRITE_BYTES_START 0x00000001
#define REG_WRITE_BYTES_PROBE 0x80000000
#define REG_READ_BYTES_START 0x00000001


// Hive cleanup flags, used with IOCTL_HAL_GET_HIVE_CLEAN_FLAG
#define HIVECLEANFLAG_SYSTEM  1
#define HIVECLEANFLAG_USERS   2


//------------------------------------------------------------------------------
// Registry Security
//------------------------------------------------------------------------------

// Used with IOCTL_HAL_GETREGSECUREKEYS

typedef struct RegSecureKey {
    WORD   wRoots;  // Bitmask of root hkeys this name is protected under
    WORD   wLen;    // Length of protected name
    LPWSTR pName;   // Protected name
} RegSecureKey;

typedef struct RegSecureKeyList {
    DWORD  dwNumKeys;     // Number of keys in the list
    RegSecureKey *pList;  // List of keys
} RegSecureKeyList;

// Definitions of the bits for each of the registry roots
#define REGSEC_ROOTMASK(hkRoot)  ((WORD)(1 << ((DWORD)(hkRoot) - (DWORD)HKEY_CLASSES_ROOT)))

// Shorthand definitions for clearer code
#define REGSEC_HKCR  REGSEC_ROOTMASK(HKEY_CLASSES_ROOT)     // 0001
#define REGSEC_HKCU  REGSEC_ROOTMASK(HKEY_CURRENT_USER)     // 0002
#define REGSEC_HKLM  REGSEC_ROOTMASK(HKEY_LOCAL_MACHINE)    // 0004
#define REGSEC_HKUS  REGSEC_ROOTMASK(HKEY_USERS)            // 0008


#ifdef __cplusplus
}
#endif /*__cplusplus*/

#endif //_PRIV_WINREG_H_
    
