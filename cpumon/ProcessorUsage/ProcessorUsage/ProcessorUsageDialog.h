// ProcessorUsageDialog.h : interface of the CProcessorUsageDialog class
//
/////////////////////////////////////////////////////////////////////////////

#pragma once
#include "ProcessorMeter.h"

/// @brief Internal message sent from the CProcessorUsageDialog to itself when 
/// it needs to update the UI.
/// @param WPARAM wParam - [ ProcessStatistics* ] - per-process cpu usage
/// statistics.
/// @param LPARAM lParam - [ float ] - total cpu usage %
static const UINT UWM_USAGE_UPDATE = 
    ::RegisterWindowMessage( _T( "UWM_USAGE_UPDATE" ) );

class CProcessorUsageDialog : 
    public CAppStdDialogImpl< CProcessorUsageDialog >,
    public CUpdateUI< CProcessorUsageDialog >,
    public CWinDataExchange< CProcessorUsageDialog >,
    public CMessageFilter, 
    public CIdleHandler
{
public:
    DECLARE_APP_DLG_CLASS( NULL, IDR_MAINFRAME, L"Software\\WTL\\ProcessorUsage" )

    enum { IDD = IDD_MAINDLG };

    virtual BOOL PreTranslateMessage( MSG* pMsg );

// CAppWindow operations
    bool AppHibernate( bool bHibernate );

    bool AppNewInstance( LPCTSTR lpstrCmdLine );

    void AppSave();

    virtual BOOL OnIdle();

    BEGIN_UPDATE_UI_MAP( CProcessorUsageDialog )
    END_UPDATE_UI_MAP()

    BEGIN_MSG_MAP( CProcessorUsageDialog )
        MESSAGE_HANDLER( WM_INITDIALOG, OnInitDialog )
        MESSAGE_HANDLER( WM_DESTROY, OnDestroy )
        MESSAGE_HANDLER( UWM_USAGE_UPDATE, OnUsageUpdate )
        CHAIN_MSG_MAP( CAppStdDialogImpl< CProcessorUsageDialog > )
    END_MSG_MAP()

    BEGIN_DDX_MAP( CProcessorUsageDialog )
        DDX_CONTROL_HANDLE( IDC_PROCESS_LIST, process_list_ctrl_ )
        DDX_CONTROL_HANDLE( IDC_PROCESS_COUNT_STATIC, process_count_ );
        DDX_CONTROL_HANDLE( IDC_CPU_USAGE_STATIC, cpu_usage_ );
    END_DDX_MAP()

// Handler prototypes (uncomment arguments if needed):
//    LRESULT MessageHandler(UINT /*uMsg*/, WPARAM /*wParam*/, LPARAM /*lParam*/, BOOL& /*bHandled*/)
//    LRESULT CommandHandler(WORD /*wNotifyCode*/, WORD /*wID*/, HWND /*hWndCtl*/, BOOL& /*bHandled*/)
//    LRESULT NotifyHandler(int /*idCtrl*/, LPNMHDR /*pnmh*/, BOOL& /*bHandled*/)

    LRESULT OnInitDialog( UINT /*uMsg*/, WPARAM /*wParam*/, LPARAM /*lParam*/, BOOL& bHandled );

    LRESULT OnDestroy( UINT /*uMsg*/, WPARAM /*wParam*/, LPARAM /*lParam*/, BOOL& bHandled );

    LRESULT OnUsageUpdate( UINT /*uMsg*/, WPARAM /*wParam*/, LPARAM /*lParam*/, BOOL& bHandled );

private:

    /// callback activated by ProcessorMeter alerting us of new statistics
    void OnUsageUpdate( const ProcessStatistics& statistics, 
                        unsigned int total_used );

    /// active object watching CPU usage
    ProcessorMeter meter_;

    /// list of processes and their CPU usage
    CListViewCtrl process_list_ctrl_;

    /// number of processes in the list
    CStatic process_count_;
    
    /// total cpu usage
    CStatic cpu_usage_;

}; // class CProcessorUsageDialog

