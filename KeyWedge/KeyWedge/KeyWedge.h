
#if !defined(AFX_KEYWEDGE_H__EA5DD0C4_7BFB_41F8_91B3_1F7F6DBB03CE__INCLUDED_)
#define AFX_KEYWEDGE_H__EA5DD0C4_7BFB_41F8_91B3_1F7F6DBB03CE__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "resource.h"
//#include "registry.h"

#define MYMSG_TASKBARNOTIFY  (WM_USER + 100)

// Returns number of elements
#define dim(x) (sizeof(x) / sizeof(x[0]))

HANDLE m_hEvent;

#define TEXTSIZE 256

#endif // !defined(AFX_KEYWEDGE_H__EA5DD0C4_7BFB_41F8_91B3_1F7F6DBB03CE__INCLUDED_)
