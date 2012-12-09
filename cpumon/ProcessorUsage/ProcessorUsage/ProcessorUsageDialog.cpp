// ProcessorUsageDialog.cpp : implementation of the CProcessorUsageDialog class
//
/////////////////////////////////////////////////////////////////////////////
/*
AdditionalIncludeDirectories="
C:\WTL80\include;
C:\WINCE500\PUBLIC\COMMON\OAK\INC;
.\WTL\INCLUDE
C:\Program Files (x86)\boost\boost_1_51
*/

#include "stdafx.h"
#include "resourceppc.h"

#include "ProcessorUsageDialog.h"
#include <atlmisc.h>
#include <iostream>
#include <sstream>
#include <boost/bind.hpp>
#include <memory>

BOOL CProcessorUsageDialog::PreTranslateMessage( MSG* pMsg )
{
    return CWindow::IsDialogMessage( pMsg );
}

bool CProcessorUsageDialog::AppHibernate( bool bHibernate )
{
    // Insert your code here or delete member if not relevant
    return bHibernate;
}

bool CProcessorUsageDialog::AppNewInstance( LPCTSTR /*lpstrCmdLine*/ )
{
    // Insert your code here or delete member if not relevant
    return false;
}

void CProcessorUsageDialog::AppSave()
{
    CAppInfo info;
    // Insert your code here or delete member if not relevant
}

BOOL CProcessorUsageDialog::OnIdle()
{
    return FALSE;
}

LRESULT CProcessorUsageDialog::OnInitDialog( UINT /*uMsg*/, 
                                             WPARAM /*wParam*/, 
                                             LPARAM /*lParam*/, 
                                             BOOL& bHandled )
{
    HWND hMenuBar = CreateMenuBar( ATL_IDM_MENU_DONECANCEL );
    UIAddToolBar( hMenuBar );
    UIAddChildWindowContainer( m_hWnd );

    // register object for message filtering and idle updates
    CMessageLoop* pLoop = _Module.GetMessageLoop();
    ATLASSERT( pLoop != NULL );
    pLoop->AddMessageFilter( this );
    pLoop->AddIdleHandler( this );

    DoDataExchange( FALSE );

    process_list_ctrl_.AddColumn( L"Process", 0 );
    process_list_ctrl_.AddColumn( L"User", 1 );
    process_list_ctrl_.AddColumn( L"Kernel", 2 );
    process_list_ctrl_.SetExtendedListViewStyle( LVS_EX_DOUBLEBUFFER | 
                                                 LVS_EX_FULLROWSELECT );

    CRect list_rc;
    process_list_ctrl_.GetClientRect( &list_rc );

    process_list_ctrl_.SetColumnWidth( 0, 
        static_cast< int >( list_rc.Width() * 0.5 ) );
    process_list_ctrl_.SetColumnWidth( 1, 
        static_cast< int >( list_rc.Width() * 0.25 ) );
    process_list_ctrl_.SetColumnWidth( 2, LVSCW_AUTOSIZE_USEHEADER );

    meter_.Start( boost::bind( 
        &CProcessorUsageDialog::OnUsageUpdate, this, _1, _2 ), 3000 );

    return bHandled = FALSE;
}

LRESULT CProcessorUsageDialog::OnDestroy( UINT /*uMsg*/, 
                                          WPARAM /*wParam*/, 
                                          LPARAM /*lParam*/, 
                                          BOOL& bHandled )
{
    meter_.Stop();
    return bHandled = FALSE;
}

void CProcessorUsageDialog::OnUsageUpdate( const ProcessStatistics& statistics, 
                                           unsigned int total_used )
{
    // We copy the statistical data to marshal it to the UI thread.
    // - Don't touch the UI from a different thread.
    // - Don't us a SendMessage here as the app can freeze if meter_.Stop() is
    //   called while we're updating the UI.
    PostMessage( UWM_USAGE_UPDATE, 
                 ( WPARAM )new ProcessStatistics( statistics ), 
                 ( LPARAM )total_used );
}

LRESULT CProcessorUsageDialog::OnUsageUpdate( UINT /*uMsg*/, 
                                              WPARAM wParam, 
                                              LPARAM lParam, 
                                              BOOL& /*bHandled*/ )
{
    std::auto_ptr< ProcessStatistics > statistics( ( ProcessStatistics* )wParam );
    unsigned int total_used = static_cast< unsigned int >( lParam );

    static short int token = 0;

    LVFINDINFO find = { 0 };
    find.flags = LVFI_STRING;

    process_list_ctrl_.SetRedraw( FALSE );
    
    for( ProcessStatistics::const_iterator it = statistics->begin();
         it != statistics->end();
         ++it )
    {
        // locate the process name in our list view. if it doesn't exist, add it.
        find.psz = it->first.c_str();
        int found = process_list_ctrl_.FindItem( &find, -1 );
        if( found == -1 )
        {
            found = process_list_ctrl_.InsertItem( 0, it->first.c_str() );
        }

        // format the user time and display it in the list view
        std::wstringstream user_time;
        user_time.precision( 1 );
        user_time << std::fixed << it->second.user << L"%";
        process_list_ctrl_.AddItem( found, 1, user_time.str().c_str() );

        // format the kernel time and display it in the list view
        std::wstringstream kernel_time;
        kernel_time.precision( 1 );
        kernel_time << std::fixed << it->second.kernel << L"%";
        process_list_ctrl_.AddItem( found, 2, kernel_time.str().c_str() );

        // mark the item with our token to show that it is still active
        process_list_ctrl_.SetItemData( found, token + 1 );
    }

    // locate all the list view items that were not updated with the new token
    // in this round and remove them - they represent processes that have exited.
    find.flags = LVFI_PARAM;
    find.psz = NULL;
    find.lParam = token;
    int found = -1;
    while( ( found = process_list_ctrl_.FindItem( &find, found ) ) > -1 )
    {
        process_list_ctrl_.DeleteItem( found );
    }
    
    // if the list view has grown longer than 1 page, scroll bars will appear.
    // re-layout the columns to account for that.
    if( process_list_ctrl_.GetItemCount() > process_list_ctrl_.GetCountPerPage() )
    {
        CRect list_rc;
        process_list_ctrl_.GetClientRect( &list_rc );

        process_list_ctrl_.SetColumnWidth( 0, static_cast< int >( list_rc.Width() * 0.5 ) );
        process_list_ctrl_.SetColumnWidth( 1, static_cast< int >( list_rc.Width() * 0.25 ) );
        process_list_ctrl_.SetColumnWidth( 2, LVSCW_AUTOSIZE_USEHEADER );
    }

    process_list_ctrl_.SetRedraw( TRUE );

    /// format & display the number of running processes
    std::wstringstream count_str;
    count_str << statistics->size();
    process_count_.SetWindowText( count_str.str().c_str() );

    /// format & display the total cpu usage
    std::wstringstream usage_str;
    usage_str.precision( 1 );
    usage_str << total_used << L"%";
    cpu_usage_.SetWindowText( usage_str.str().c_str() );

    ++token;
    return 0;
}
