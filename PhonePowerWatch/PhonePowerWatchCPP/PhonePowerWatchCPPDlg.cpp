// PhonePowerWatchCPPDlg.cpp : implementation file
//

#include "stdafx.h"
#include "PhonePowerWatchCPP.h"
#include "PhonePowerWatchCPPDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

static const TCHAR setWWANpowerXML[] =
_T("<Subsystem Name=\"WWAN Radio\">\r\n \
    <Field Name=\"Radio Power State\">%i</Field>\r\n \
   </Subsystem>");
static const TCHAR getWWANpowerXML[] =
L"<Subsystem Name=\"WWAN Radio\">\r\n \
   <Field Name=\"Radio Power State\"></Field>\r\n \
  </Subsystem>";

// CPhonePowerWatchCPPDlg dialog

CPhonePowerWatchCPPDlg::CPhonePowerWatchCPPDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CPhonePowerWatchCPPDlg::IDD, pParent)
	, m_statusText(_T(""))
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}

void CPhonePowerWatchCPPDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_STATUSTEXT, statusText);
	DDX_Text(pDX, IDC_STATUSTEXT, m_statusText);
}

BEGIN_MESSAGE_MAP(CPhonePowerWatchCPPDlg, CDialog)
#if defined(_DEVICE_RESOLUTION_AWARE) && !defined(WIN32_PLATFORM_WFSP)
	ON_WM_SIZE()
#endif
	//}}AFX_MSG_MAP
	ON_BN_CLICKED(IDC_BTN_ON, &CPhonePowerWatchCPPDlg::OnBnClickedBtnOn)
	ON_BN_CLICKED(IDC_BTN_OFF, &CPhonePowerWatchCPPDlg::OnBnClickedBtnOff)
	ON_WM_TIMER()
END_MESSAGE_MAP()


// CPhonePowerWatchCPPDlg message handlers

BOOL CPhonePowerWatchCPPDlg::OnInitDialog()
{
	CDialog::OnInitDialog();

	// Set the icon for this dialog.  The framework does this automatically
	//  when the application's main window is not a dialog
	SetIcon(m_hIcon, TRUE);			// Set big icon
	SetIcon(m_hIcon, FALSE);		// Set small icon

	// TODO: Add extra initialization here
	SetTimer(1, 3000, 0);

	return TRUE;  // return TRUE  unless you set the focus to a control
}

#if defined(_DEVICE_RESOLUTION_AWARE) && !defined(WIN32_PLATFORM_WFSP)
void CPhonePowerWatchCPPDlg::OnSize(UINT /*nType*/, int /*cx*/, int /*cy*/)
{
	if (AfxIsDRAEnabled())
	{
		DRA::RelayoutDialog(
			AfxGetResourceHandle(), 
			this->m_hWnd, 
			DRA::GetDisplayMode() != DRA::Portrait ? 
			MAKEINTRESOURCE(IDD_PHONEPOWERWATCHCPP_DIALOG_WIDE) : 
			MAKEINTRESOURCE(IDD_PHONEPOWERWATCHCPP_DIALOG));
	}
}
#endif


void CPhonePowerWatchCPPDlg::OnBnClickedBtnOn()
{
	// TODO: Add your control notification handler code here
	TCHAR pszconfig[512];
	wsprintf(pszconfig, setWWANpowerXML, 1);
	if(setWWANpower(pszconfig)==0)
		m_statusText=L"WWAN switched ON";
	else
		m_statusText=L"error";
}

void CPhonePowerWatchCPPDlg::OnBnClickedBtnOff()
{
	// TODO: Add your control notification handler code here
	TCHAR pszconfig[512];
	wsprintf(pszconfig, setWWANpowerXML, 0);
	if(setWWANpower(pszconfig)==0)
		m_statusText=L"WWAN switched off";
	else
		m_statusText=L"error";
}

void CPhonePowerWatchCPPDlg::OnTimer(UINT_PTR nIDEvent)
{
	// TODO: Add your message handler code here and/or call default
	int iRet = checkWWANpower();
	if(iRet==0)
		m_statusText=L"WWAN is off";
	else if(iRet==1)
		m_statusText=L"WWAN is ON";
	else
		m_statusText=L"WWAN check error";

	UpdateData(FALSE);
	CDialog::OnTimer(nIDEvent);
}

int CPhonePowerWatchCPPDlg::checkWWANpower(void)
{
	int iRet;
	TCHAR sResult[128] = {0};
	TCHAR szConfigData[512] = {0};
	TCHAR pRetData[512] = {0};
	size_t len = 512;
	int iResult=-1;

	//Set country value using the Smart System Configuration API
	
	_stprintf (szConfigData, getWWANpowerXML, _T(""));
	iRet = getConfigurationData (szConfigData, pRetData, &len);

	if (iRet == 0)
	{
			//Parse out Country from returned XML
			//XML should be like this <Field Name=\"Country\">xxxx</Field>
			TCHAR *tstStr=_T("<Field Name=\"Radio Power State\">");
			TCHAR *pData = _tcsstr (pRetData, tstStr);
			if (pData != NULL)
			{
				pData = pData + wcslen(tstStr);
				for (int ii=0; *pData != '<'; ii++, pData++)
				{
					sResult[ii] = *pData;
				}
				iResult = _wtoi(sResult);
				iRet=iResult;
			}
	}
	else
	{
		::MessageBox (NULL, _T("Error retrieving WWAN state"), _T("Error"), MB_OK);
	}

	return iRet;
}

int CPhonePowerWatchCPPDlg::setWWANpower(TCHAR* pszConfigData)
{
	int		iRet = 0;
	TCHAR	*pRetData;
	size_t  iRetDataSize = 0;
	int		iLen = 0;
	
	ITCSSAPI_RETURN_TYPE sRet;

	iLen = _tcslen (pszConfigData) + 1024;  //ensure enough space for returned data
	pRetData = (TCHAR *)malloc (iLen * sizeof (TCHAR)); 
	memset (pRetData, 0, iLen * sizeof (TCHAR));
	iRetDataSize = iLen;
	sRet = ITCSSSet (pszConfigData, pRetData, &iRetDataSize, 0);
	if (sRet != E_SS_SUCCESS)
	{
		iRet = -1;
	}
	else
	{
		iRet = 0;
	}

	if (pRetData != 0)
	{
		free (pRetData);
	}
	return iRet;
}

int CPhonePowerWatchCPPDlg::getConfigurationData(TCHAR* pszConfigData, TCHAR* pRetData, size_t* piLen)
{
	int  iRet = 0;
	int  iLen = 0;
	ITCSSAPI_RETURN_TYPE sRet;

	sRet = ITCSSGet (pszConfigData, pRetData, piLen, 0);
	if (sRet != E_SS_SUCCESS)
	{
		iRet = -1;
	}
	else
	{
		iRet = 0;
	}

	return iRet;
}
