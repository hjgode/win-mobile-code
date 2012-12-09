#include "StdAfx.h"
#include "ProcessorMeter.h"
#include "ProcessInfo.hpp"

namespace PI = ProcessInfo;

ProcessorMeter::ProcessorMeter( void ) 
    : stop_event_( ::CreateEvent( NULL, TRUE, FALSE, NULL ), ::CloseHandle ),
      interval_( 1000 )
{
}

ProcessorMeter::~ProcessorMeter( void )
{
}

void ProcessorMeter::Start( OnUsageUpdate callback, unsigned int interval )
{
    ::ResetEvent( stop_event_.get() );
    interval_ = interval;
    callback_ = callback;
    monitor_thread_.reset( new CThread( 
        boost::bind( &ProcessorMeter::MonitorThread, this ) ) );
}

void ProcessorMeter::Stop( DWORD timeout )
{
    ::SetEvent( stop_event_.get() );
    if( !monitor_thread_->Join( timeout ) )
    {
        // TODO: throw exception. The thread is frozen.
    }
}

unsigned int ProcessorMeter::Interval() const
{
    return interval_;
}

/// This thread collects the statistical information on running processes.
/// 1. Get an initial list of the names and PIDs of all running processes.
/// 2. Get an initial list of how long each PID has spent in kernel and user time.
/// 3. After the interval has expired, get another list of each PID and its
///    kernel and user time.
/// 4. Calculate how much time was spent for each process in kernel and user
///    space during the wait interval.
/// 5. If any process PID isn't in our list of names, refresh our process name
///    list. It means we have a new process.
/// 6. Send the user callback that statistical information.
/// 7. Repeat at step 3.
void ProcessorMeter::MonitorThread()
{
    PI::ProcessNames names = PI::GetProcessNameList();
    PI::Processes old_list = PI::GetProcessList();

    ProcessStatistics statistics;

    DWORD start = ::GetTickCount();

    while( ::WaitForSingleObject( stop_event_.get(), interval_ ) == WAIT_TIMEOUT )
    {
        PI::Processes new_list = PI::GetProcessList();	//thread list
        DWORD duration = ::GetTickCount() - start;
        
        statistics.clear();

        DWORD system_total = 0;
        for( PI::Processes::const_iterator p2 = new_list.begin();
             p2 != new_list.end();
             ++p2 )
        {
            PI::Processes::const_iterator p1 = old_list.find( p2->first );
            if( p1 != old_list.end() )
            {
                DWORD user_total = 0;
                DWORD kernel_total = 0;

                for( PI::Threads::const_iterator t2 = p2->second.begin();
                     t2 != p2->second.end();
                     ++t2 )
                {
                    PI::Threads::const_iterator t1 = p1->second.find( t2->first );
                    if( t1 != p1->second.end() )
                    {
                        kernel_total += PI::GetThreadTick( t2->second.kernel ) - 
                                        PI::GetThreadTick( t1->second.kernel );
                        user_total += PI::GetThreadTick( t2->second.user ) - 
                                      PI::GetThreadTick( t1->second.user );
                    }
                }
                
                float user_percent = ( user_total ) / static_cast< float >( duration ) * 100.0f;
                float kernel_percent = ( kernel_total ) / static_cast< float >( duration ) * 100.0f;
                system_total += user_total + kernel_total;

                // locate the process name by its ID
                PI::ProcessNames::const_iterator found_name = names.find( p2->first );

                // if the process ID isn't in the name list, it must be new. 
                // refresh the name list and try again.
                if( found_name == names.end() )
                {
                    names = PI::GetProcessNameList();

                    found_name = names.find( p2->first );

                    // still can't find the process ID? Just move on.
                    if( found_name == names.end() )
                        continue;
                }
                
                // update the statistics with this process' info
                ProcessUsage usage = { 
                    static_cast< unsigned int >( kernel_percent ), 
                    static_cast< unsigned int >( user_percent ) };
                statistics[ found_name->second ] = usage;
            }
        }

        // calculate the total processor percent used
        float percent_used = system_total / static_cast< float >( duration ) * 100.0f;

        // activate the user-defined callback
        callback_( statistics, static_cast< unsigned int >( percent_used ) );

        old_list = new_list;
        start = ::GetTickCount();
    }
}
