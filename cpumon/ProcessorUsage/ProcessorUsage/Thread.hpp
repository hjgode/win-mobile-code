/// example usage:
///   void thread_function( int, bool, char );
///   CThread my_thread( boost::bind( thread_function, 1, true, 'a' ) );
///   // do interesting things...
///   my_thread.Join();
///
#pragma once
#include <memory>
#include <boost/shared_ptr.hpp>

/// A thread wrapper that provides exception-safe management of the thread
/// handle and type-safe thread functions.
class CThread
{
public:
    explicit CThread( HANDLE handle ) 
        : handle_( handle, &::CloseHandle )
    {
    };

    template< typename ThreadFunc >
    explicit CThread( const ThreadFunc& f ) 
        : handle_( DoCreateThread( f ), &::CloseHandle )
    {
    };

    /// Block until the thread completes
    BOOL Join( DWORD max_wait_time = INFINITE )
    {
        _ASSERT( NULL != handle_ );
        return ( WAIT_OBJECT_0 == ::WaitForSingleObject( handle_.get(), max_wait_time ) );
    };

private:

    /// thread handle
    boost::shared_ptr< void > handle_;

    friend DWORD WINAPI thread_function( LPVOID arg );
 
    /// The actual thread creation function.
    ///
    /// Yes, this adds slightly to the cost of creating a thread, but makes the
    /// actual thread-function typesafe from the user's point of view. If 
    /// performance is an issue, use the other constructor.
    template< typename ThreadFunc >
    static HANDLE DoCreateThread( const ThreadFunc& f )
    {
        DWORD thread_id = 0;
        std::auto_ptr< func_base > arg( new func< ThreadFunc >( f ) );
        HANDLE thread = ::CreateThread( NULL, 
                                        0, 
                                        thread_function, 
                                        arg.get(), 
                                        0, 
                                        &thread_id );
        if( NULL != thread )
            arg.release();
        return thread;
    };

    class func_base
    {
    public:
        virtual ~func_base() {};
        virtual void run() = 0;
    }; // class func_base

    template< typename Function >
    class func : public func_base
    {
    public:
        func( Function f ) : f_( f ) {};
        virtual void run() { f_(); };
    private:
        Function f_;
    }; // class func

}; // class CThread

/// @brief a wrapper for the actual user thread function
inline DWORD WINAPI thread_function( LPVOID arg )
{
    std::auto_ptr< CThread::func_base > func( 
        static_cast< CThread::func_base* >( arg ) );
    func->run();
    return 0;
}
