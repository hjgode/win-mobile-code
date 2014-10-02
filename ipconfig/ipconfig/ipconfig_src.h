
#pragma once

#ifndef _IPCONFIG_SRC_H_
#define _IPCONFIG_SRC_H_

#include "stdafx.h"

// Function Declarations
void  usage();
int GetConfigData();
void ReleaseAddress(DWORD);
void RenewAddress(DWORD);
void FlushDNSCache(void);

void setOutputWindow(HWND);

#endif //_IPCONFIG_SRC_H_