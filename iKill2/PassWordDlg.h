#if !defined(AFX_PASSWORDDLG_H__E37FD03E_0933_415C_9F14_78E3D7B6EF66__INCLUDED_)
#define AFX_PASSWORDDLG_H__E37FD03E_0933_415C_9F14_78E3D7B6EF66__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// PassWordDlg.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// PassWordDlg dialog

class PassWordDlg : public CDialog
{
// Construction
public:
	PassWordDlg(CWnd* pParent = NULL);   // standard constructor

// Dialog Data
	//{{AFX_DATA(PassWordDlg)
	enum { IDD = IDD_PWBOX };
		// NOTE: the ClassWizard will add data members here
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(PassWordDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(PassWordDlg)
	virtual void OnOK();
	virtual void OnCancel();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_PASSWORDDLG_H__E37FD03E_0933_415C_9F14_78E3D7B6EF66__INCLUDED_)
