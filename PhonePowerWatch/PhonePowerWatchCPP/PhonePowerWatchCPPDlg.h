// PhonePowerWatchCPPDlg.h : header file
//

#pragma once
#include "afxwin.h"

#include <ITCSSApi.h>
#pragma comment (lib, "ITCSSApi.lib")
#include <smartsyserrors.h>

// CPhonePowerWatchCPPDlg dialog
class CPhonePowerWatchCPPDlg : public CDialog
{
// Construction
public:
	CPhonePowerWatchCPPDlg(CWnd* pParent = NULL);	// standard constructor

// Dialog Data
	enum { IDD = IDD_PHONEPOWERWATCHCPP_DIALOG };


	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV support

// Implementation
protected:
	HICON m_hIcon;

	// Generated message map functions
	virtual BOOL OnInitDialog();
#if defined(_DEVICE_RESOLUTION_AWARE) && !defined(WIN32_PLATFORM_WFSP)
	afx_msg void OnSize(UINT /*nType*/, int /*cx*/, int /*cy*/);
#endif
	DECLARE_MESSAGE_MAP()
public:
	CEdit statusText;
	afx_msg void OnBnClickedBtnOn();
	afx_msg void OnBnClickedBtnOff();
	afx_msg void OnTimer(UINT_PTR nIDEvent);
	int checkWWANpower(void);
	int setWWANpower(TCHAR* pszConfigData);
	CString m_statusText;
	int getConfigurationData(TCHAR* pszConfigData, TCHAR* pRetData, size_t* piLen);
};
