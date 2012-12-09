using System;
using System.Runtime.InteropServices;
/*
[C#]
AtomicCF.Timer oTimer = new AtomicCF.Timer();
oTimer.Start();
DoSomething();
long lDur = oTimer.Stop();
MessageBox.Show("DoSomething executed in " + lDur + "ms");
*/
namespace AtomicCF
{
    class Timer
    {
        [DllImport("coredll.dll")]
        extern static int QueryPerformanceCounter(ref long perfCounter);

        [DllImport("coredll.dll")]
        extern static int QueryPerformanceFrequency(ref long frequency);

        static private Int64 m_frequency;
        private Int64 m_start;

        // Static constructor to initialize frequency.
        static Timer()
        {
            if (QueryPerformanceFrequency(ref m_frequency) == 0)
            {
                throw new ApplicationException();
            }
            // Convert to ms.
            m_frequency /= 1000;
        }

        public void Start()
        {
            if (QueryPerformanceCounter(ref m_start) == 0)
            {
                throw new ApplicationException();
            }
        }

        public Int64 Stop()
        {
            Int64 stop = 0;
            if (QueryPerformanceCounter(ref stop) == 0)
            {
                throw new ApplicationException();
            }
            return (stop - m_start) / m_frequency;
        }
    }
}
