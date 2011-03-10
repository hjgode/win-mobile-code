using System;

using System.Collections.Generic;
using System.Text;

namespace NativeSync
{
    public enum WaitObjectReturnValue: long
    {
        WAIT_OBJECT_0  = 0x00000000L,
        WAIT_TIMEOUT   = 0x000000102L,
        WAIT_ABANDONED = 0x00000080L,
        WAIT_FAILED    = 0xffffffffL
    }
}
