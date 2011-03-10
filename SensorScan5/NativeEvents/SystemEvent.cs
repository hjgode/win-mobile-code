using System;

using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace NativeSync
{

    public class SystemEvent : SyncBase
    {
        bool _manualReset;
        
        public  SystemEvent(string name, bool manualReset, bool initialState)
        {
            _manualReset = manualReset;
            _hSyncHandle = CoreDLL.CreateEvent(IntPtr.Zero, manualReset, initialState, name);
            this._firstInstance= (0 == Marshal.GetLastWin32Error());
        }

        public bool ManualReset
        {
            get { return _manualReset; }
        }

        public bool  PulseEvent()
        {
            return CoreDLL.EventModify(_hSyncHandle, (int)EventAction.Pulse);
        }

        public bool  ResetEvent()
        {
            return CoreDLL.EventModify(_hSyncHandle, (int)EventAction.Reset);
        }

        public void SetEventData(int data)
        {
            CoreDLL.SetEventData(_hSyncHandle, data);
        }

        public int GetEventData()
        {
            return CoreDLL.GetEventData(_hSyncHandle);
        }

    }
}
