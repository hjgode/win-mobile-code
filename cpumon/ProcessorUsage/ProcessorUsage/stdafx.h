// stdafx.h : include file for standard system include files,
//  or project specific include files that are used frequently, but
//      are changed infrequently
//

#pragma warning( push )
// disable all of the "warning C4201: nonstandard extension used : 
// nameless struct/union" warnings from old windows headers.
#pragma warning( disable : 4201 )

// Change this value to use different versions
//#define WINVER 0x0500
#define _WIN32_WCE_AYGSHELL 1
#define _SECURE_ATL 1
#define _WTL_CE_NO_ZOOMSCROLL

#define NOMINMAX
#include <algorithm>
using std::max;
using std::min;

#include <atlbase.h>
#include <atlapp.h>

extern CAppModule _Module;

#include <atlwin.h>
#include <atlframe.h>
#include <atlctrls.h>
#include <atldlgs.h>
#include <atlddx.h>
#include <atlctrlx.h>
#include <atlwince.h>

#include <aygshell.h>
#pragma comment(lib, "aygshell.lib")

#pragma warning( pop )
