using System;

using System.Collections.Generic;
using System.Text;

namespace NativeSync
{
    public abstract class SyncBase:IDisposable
    {
        protected IntPtr _hSyncHandle;
        protected bool _firstInstance;
        const int INFINITE = -1;

        public bool FirstInstance
        {
            get { return _firstInstance; }
        }

        public IntPtr SyncHandle
        {
            get
            {
                return _hSyncHandle;
            }
        }

        public static SyncBase WaitForMultipleObjects(int timeout, params SyncBase[] syncObjectList)
        {
            IntPtr[] handleList = new IntPtr[syncObjectList.Length];
            for (int i = 0; i < syncObjectList.Length; ++i)
            {
                handleList[i] = syncObjectList[i].SyncHandle;
            }
            int retIndex = NativeSync.CoreDLL.WaitForMultipleObjects(syncObjectList.Length, handleList, false, timeout);
            if (retIndex < syncObjectList.Length)
                return syncObjectList[retIndex];
            return null;
        }

        public bool SetEvent()
        {
            return CoreDLL.EventModify(_hSyncHandle, (int)EventAction.Set);
        }


        public static SyncBase WaitForMultipleObjects(params SyncBase[] syncObjectList)
        {
            return WaitForMultipleObjects(INFINITE, syncObjectList);
        }

        public WaitObjectReturnValue Wait(int timeout)
        {
            return (WaitObjectReturnValue)CoreDLL.WaitForSingleObject(_hSyncHandle, timeout);
        }

        public WaitObjectReturnValue Wait()
        {
            return (WaitObjectReturnValue)CoreDLL.WaitForSingleObject(_hSyncHandle, -1);
        }


        #region IDisposable Members

        public virtual void Dispose()
        {
            CoreDLL.CloseHandle(_hSyncHandle);
        }

        #endregion
    }
}
