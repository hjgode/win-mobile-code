// 'Windows CE 3.0 Programming' Source Code Samples (Prentice Hall, 2000)
// Source Code Author: Nick Grattan (nick@softwarepaths.com)
// Version 1.00
/* USAGE

	//global
	#include "cOutClass.h"

	COutput cout;
	HWND				hWndEdit;		// Read only edit box for output display
	
	//in wndproc for WM_CREATE:
	//#### COutput stuff start ####
		GetClientRect(hWnd, &rClient);
		nCmdHt = CommandBar_Height(mbi.hwndMB);
		hWndEdit = CreateWindow(_T("Edit"), 
				NULL, WS_CHILD | WS_VISIBLE | ES_READONLY
						| ES_MULTILINE | WS_VSCROLL | WS_HSCROLL, 
				0, 0,
				rClient.right, rClient.bottom - 25, 
				hWnd, NULL, g_hInst, NULL);
		cout.SetOutputWindow(hWndEdit);
	//#### COutput stuff END ####
	
	// in code use cout.CLS() to clear window
	// cout << L"sometext"; cout << intVar
	// cout.print(L"Hello %s", szText);
*/
// Input and output routines for sample files

#include "stdafx.h"
#include "coutclass.h"

const TCHAR endl[] = _T("\r\n");
const TCHAR tab[] = _T("\t");

void COutput::refresh(){
	::UpdateWindow(m_hEdit);
}

void COutput::print(const wchar_t *fmt, ...){
	int nIndex;
    va_list vl;
    va_start(vl,fmt);
    wchar_t buf[1024]; // to bad CE hasn't got wvnsprintf
    wvsprintf(buf,fmt,vl);
	
	nIndex = GetWindowTextLength(m_hEdit); 
    SendMessage(m_hEdit, EM_SETSEL, nIndex, nIndex); 
    SendMessage(m_hEdit, EM_REPLACESEL, 0, (LPARAM)buf); 
}

COutput& COutput::operator<<(wchar_t* lpwcharOutput){
	int nIndex;
	//LPCWSTR lpOutput=new WCHAR(wcslen(lpwcharOutput));
	wchar_t* lpOutput=new WCHAR(wcslen(lpwcharOutput));

	memcpy(lpOutput, lpwcharOutput, (wcslen(lpwcharOutput)+1)*sizeof(WCHAR));

    nIndex = GetWindowTextLength(m_hEdit); 
    SendMessage(m_hEdit, EM_SETSEL, nIndex, nIndex); 
    SendMessage(m_hEdit, EM_REPLACESEL, 0, (LPARAM)lpOutput); 
	delete lpOutput;
	return *this;
}

COutput& COutput::operator<<(LPCTSTR lpOutput)
{
	int nIndex;

    nIndex = GetWindowTextLength(m_hEdit); 
    SendMessage(m_hEdit, EM_SETSEL, nIndex, nIndex); 
    SendMessage(m_hEdit, EM_REPLACESEL, 0, (LPARAM)lpOutput); 
	return *this;
}

COutput& COutput::operator <<(char* lpcharOutput)
{
	LPTSTR lpBuffer = new TCHAR[strlen(lpcharOutput) + 1];
	mbstowcs(lpBuffer, lpcharOutput, strlen(lpcharOutput));
	lpBuffer[strlen(lpcharOutput)] = '\0';
	*this << lpBuffer;
	return *this;
}

COutput& COutput::operator <<(CHAR cOutput)
{
	TCHAR szBuffer[2];
	szBuffer[0] = cOutput;
	szBuffer[1] = '\0';
	*this << szBuffer;
	return *this;
}

COutput& COutput::operator<<(DWORD dwOutput)
{
	TCHAR szBuffer[20];
	wsprintf(szBuffer, _T("%lu"), dwOutput);
	*this << szBuffer;
	return *this;
}

COutput& COutput::operator<<(WORD wOutput)
{
	TCHAR szBuffer[20];
	wsprintf(szBuffer, _T("%u"), wOutput);
	*this << szBuffer;
	return *this;
}

COutput& COutput::operator<<(FLOAT fOutput)
{
	TCHAR szBuffer[20];
	swprintf(szBuffer, _T("%f"), fOutput);
	*this << szBuffer;
	return *this;
}

COutput& COutput::operator<<(int nOutput)
{
	TCHAR szBuffer[20];
	wsprintf(szBuffer, _T("%d"), nOutput);
	*this << szBuffer;
	return *this;
}
void COutput::CLS()
{
	SetWindowText(m_hEdit, NULL);
}

