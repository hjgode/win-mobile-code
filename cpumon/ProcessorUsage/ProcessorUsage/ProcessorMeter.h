#pragma once
#include <boost/bind.hpp>
#include <boost/function.hpp>
#include <boost/noncopyable.hpp>
#include <boost/shared_ptr.hpp>
#include <map>
#include <string>

#include "Thread.hpp"

/// Process statistics
typedef struct process_usage {
    /// % time the kernel spent in kernel space
    unsigned int kernel;
    /// % time the kernel spent in user space
    unsigned int user;
} ProcessUsage;

/// Statistics for all running processes
/// @param std::wstring - name of the process
/// @param ProcessUsage - process statistics
typedef std::map< std::wstring, ProcessUsage > ProcessStatistics;

/// Active object that updates a callback with process usage statistics.
class ProcessorMeter : private boost::noncopyable
{
public:
    ProcessorMeter( void );

    ~ProcessorMeter( void );

    typedef boost::function< void( const ProcessStatistics& statistics, unsigned int total_used ) > OnUsageUpdate;

    /// start receiving updates on processor usage statistics
    /// @param OnUsageUpdate - activated every interval ms.
    /// @param interval - how often (in ms) to activate the callback.
    void Start( OnUsageUpdate callback, unsigned int interval );
    
    /// stop receiving updates.
    void Stop( DWORD timeout = INFINITE );

    /// Get the interval between updates (in ms).
    unsigned int Interval() const;

private:

    void MonitorThread();

    /// callback updated when new statistics are updated
    OnUsageUpdate callback_;

    /// interval at which the statistics are updated. (in ms)
    unsigned int interval_;

    /// event activated when we should stop sending updates
    boost::shared_ptr< void > stop_event_;

    /// handle to the statistics thread.
    boost::shared_ptr< CThread > monitor_thread_;

}; // class ProcessorMeter
