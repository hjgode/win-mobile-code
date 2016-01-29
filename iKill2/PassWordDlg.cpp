// PassWordDlg.cpp : implementation file
//

#include "stdafx.h"
//#include "ikill2.h"
#include "PassWordDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// PassWordDlg dialog


PassWordDlg::PassWordDlg(CWnd* pParent /*=NULL*/)
	: CDialog(PassWordDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(PassWordDlg)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
}


void PassWordDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(PassWordDlg)
		// NOTE: the ClassWizard will add DDX and DDV calls here
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(PassWordDlg, CDialog)
	//{{AFX_MSG_MAP(PassWordDlg)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// PassWordDlg message handlers

void PassWordDlg::OnOK() 
{
	// TODO: Add extra validation here
	
	CDialog::OnOK();
}

void PassWordDlg::OnCancel() 
{
	// TODO: Add extra cleanup here
	
	CDialog::OnCancel();
}
