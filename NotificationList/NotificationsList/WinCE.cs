using System;

using System.Collections.Generic;
using System.Text;

namespace WinCE
{
    class WinCE
    {
        public static int findEndOfString(byte[] buf, int idx, int iSize)
        {
            int iRet = -1;
            int iStart = idx;
            int iEnd = iSize;
            int iPos=idx;
            //search for two zero bytes in sequence
            do
            {
                if (buf[iPos] == 0 && buf[iPos + 1] == 0)
                    iRet = iPos+1;
                iPos++;
            } while (iPos < iSize-1 && iRet!=-1);
            return iRet;
        }
    }
}
