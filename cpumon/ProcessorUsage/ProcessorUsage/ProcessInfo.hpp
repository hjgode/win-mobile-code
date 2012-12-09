#include <tlhelp32.h>
#pragma comment( lib, "toolhelp.lib" )

/// required for SetProcPermissions(). It's part of platform builder. If you
/// don't have it, do some quick internet searching to get the function 
/// prototype.
#include <pkfuncs.h> 

#define LODWORD(l) ((DWORD)((DWORDLONG)(l)))
#define HIDWORD(l) ((DWORD)(((DWORDLONG)(l)>>32)&0xFFFFFFFF))
#define MAKEDWORDLONG(a,b) ((DWORDLONG)(((DWORD)(a))|(((DWORDLONG)((DWORD)(b)))<<32)))

namespace ProcessInfo {

/// Time a thread has spent working
struct thread_times {
    /// Time a thread has spent in kernel space
    FILETIME kernel;
    /// Time a thread has spent in user space
    FILETIME user;
};

/// Time each process has spent working
/// @param DWORD - TID
/// @param thread_times - Thread working times
typedef std::map< DWORD, thread_times > Threads;

/// Time each Process has spent working
/// @param DWORD - PID
/// @param Threads - Process' thread working times
typedef std::map< DWORD, Threads > Processes;

/// Associates process IDs to process names
/// @param DWORD - PID
/// @Param std::wstring - process name
typedef std::map< DWORD, std::wstring > ProcessNames;

/// Get the combined time a thread has spent in user and kernel space in ms
static DWORD GetThreadTick( const FILETIME& user, const FILETIME& kernel )
{
    __int64 tick = MAKEDWORDLONG( user.dwLowDateTime, user.dwHighDateTime );
    tick += MAKEDWORDLONG( kernel.dwLowDateTime, kernel.dwHighDateTime );
    tick /= 10000;
    return static_cast< DWORD >( tick );
}

/// Convert a FILETIME to ticks (ms)
static DWORD GetThreadTick( const FILETIME& time )
{
    __int64 tick = MAKEDWORDLONG( time.dwLowDateTime, time.dwHighDateTime );
    return static_cast< DWORD >( tick /= 10000 );
}

/// Get a list associating currently running process IDs with names
static ProcessNames GetProcessNameList()
{
    ProcessNames name_list;

    HANDLE snapshot = ::CreateToolhelp32Snapshot( TH32CS_SNAPPROCESS | 
                                                  TH32CS_SNAPNOHEAPS, 
                                                  0 );
    if( INVALID_HANDLE_VALUE != snapshot )
    {
        PROCESSENTRY32 pe = { 0 };
        pe.dwSize = sizeof( PROCESSENTRY32 );

        if( ::Process32First( snapshot, &pe ) )
        {
            do 
            {
                name_list[ pe.th32ProcessID ] = pe.szExeFile;
            } while( ::Process32Next( snapshot, &pe ) );
        }
        ::CloseToolhelp32Snapshot( snapshot );
    }

    return name_list;
}

/// Temporarily grant phenomenal cosmic powers. This may not be necessary for
/// versions of windows mobile earlier than 6.5.
struct CosmicPowers
{
    CosmicPowers()
    {
        old_permissions_ = ::SetProcPermissions( 0xFFFFFFFF );
    }

    ~CosmicPowers()
    {
        ::SetProcPermissions( old_permissions_ );
    }

private:
    DWORD old_permissions_;
}; // struct CosmicPowers

/// Gets the list of currently running processes
static Processes GetProcessList()
{
    Processes process_list;
    CosmicPowers we_are_powerful;

    HANDLE snapshot = ::CreateToolhelp32Snapshot( TH32CS_SNAPTHREAD, 0 );
    if( INVALID_HANDLE_VALUE != snapshot )
    {
        THREADENTRY32 te = { 0 };
        te.dwSize = sizeof( THREADENTRY32 );

        if( ::Thread32First( snapshot, &te ) )
        {
            do 
            {
                FILETIME creation = { 0 }, 
                         exit = { 0 }, 
                         kernel = { 0 }, 
                         user = { 0 };

                if( ::GetThreadTimes( ( HANDLE )te.th32ThreadID, 
                                      &creation, 
                                      &exit, 
                                      &kernel, 
                                      &user ) )
                {
                    thread_times t = { kernel, user };
                    process_list[ te.th32OwnerProcessID ][ te.th32ThreadID ] = t;
                }
            } while( ::Thread32Next( snapshot, &te ) );
        }

        ::CloseToolhelp32Snapshot( snapshot );
    }

    return process_list;
}

} // namespace ProcessInfo
