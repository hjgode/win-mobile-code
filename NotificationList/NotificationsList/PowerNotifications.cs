using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Runtime.InteropServices;
using System.Threading;

//PowerNotifications_Queue.cs
/* usage
PowerNotifications pwrNot = new PowerNotifications();
pwrNot.Resume += new EventHandler(PowerNotifier_OnResume);
pwrNot.Suspend += new EventHandler(PowerNotifier_OnSuspend);
pwrNot.Start();
*/
namespace PWRNOTIFICATIONS
{
    public class PowerNotifications:IDisposable
    {
        public class PwrEventArgs : EventArgs
        {
            public string PWRmsg;
            public UInt32 PWRflags;
            public PwrEventArgs(string m, UInt32 f)
            {
                this.PWRmsg = m;
                this.PWRflags = f;
            }
        }

        #region Events
        public event EventHandler OnSuspend;
        public event EventHandler OnResume;
        public event OnMsgHandler OnMsg;
        public delegate void OnMsgHandler(object sender, PwrEventArgs e);

        [StructLayout(LayoutKind.Sequential)]
        struct POWER_BROADCAST
        {
            UInt32 dwMsg;
            UInt32 dwFlags;
            UInt32 dwLength;
            string sSystemPowerState; //WCHAR SystemPowerState[1];
        }
        #endregion

        #region const
        const uint POWER_NOTIFY_ALL = 0xFFFFFFFF;

        const uint PBT_TRANSITION          =  0x00000001;  // broadcast specifying system power state transition
        const uint PBT_RESUME = 0x00000002;  // broadcast notifying a resume, specifies previous state
        const uint PBT_POWERSTATUSCHANGE = 0x00000004;  // power supply switched to/from AC/DC
        const uint PBT_POWERINFOCHANGE = 0x00000008;

        //
        // System Power (Source/State/Option) Flags
        //
        [Flags]
        enum PowerState:uint
        {
            // upper bytes: common power state bits
            //#define POWER_STATE(f)           ((f) &  0xFFFF0000);        // power state mask
            POWER_STATE_NA              = 0x00,
            POWER_STATE_ON              = 0x00010000,        // on state
            POWER_STATE_OFF             = 0x00020000,        // no power, full off
            POWER_STATE_CRITICAL        = 0x00040000,        // critical off
            POWER_STATE_BOOT            = 0x00080000,        // boot state
            POWER_STATE_IDLE            = 0x00100000,        // idle state
            POWER_STATE_SUSPEND         = 0x00200000,        // suspend state
            POWER_STATE_UNATTENDED      = 0x00400000,        // Unattended state.
            POWER_STATE_RESET           = 0x00800000,        // reset state
            POWER_STATE_USERIDLE        = 0x01000000,        // user idle state
            POWER_STATE_BACKLIGHTON     = 0x02000000,        // device scree backlight on
            POWER_STATE_PASSWORD        = 0x10000000,        // This state is password protected.
        }
        [Flags]
        enum PowerEventType
        {
            PBT_TRANSITION = 0x00000001,
            PBT_RESUME = 0x00000002,
            PBT_POWERSTATUSCHANGE = 0x00000004,
            PBT_POWERINFOCHANGE = 0x00000008,
        }

        [Flags]
        enum PowerState1
        {
            POWER_STATE_ON = (0x00010000),
            POWER_STATE_OFF = (0x00020000),

            POWER_STATE_CRITICAL = (0x00040000),
            POWER_STATE_BOOT = (0x00080000),
            POWER_STATE_IDLE = (0x00100000),
            POWER_STATE_SUSPEND = (0x00200000),
            POWER_STATE_RESET = (0x00800000),
        }
        class POWER_BROADCAST1
        {
            public POWER_BROADCAST1(int size)
            {
                m_data = new byte[size];
            }
            byte[] m_data;
            public byte[] Data { get { return m_data; } }
            public PowerEventType Message { get { return (PowerEventType)BitConverter.ToInt32(m_data, 0); } }
            public PowerState1 Flags { get { return (PowerState1)BitConverter.ToInt32(m_data, 4); } }
            public int Length { get { return BitConverter.ToInt32(m_data, 8); } }
            public byte[] SystemPowerState { get { byte[] data = new byte[Length]; Buffer.BlockCopy(m_data, 12, data, 0, Length); return data; } }
        }
        #endregion

        #region Variables
        IntPtr ptr = IntPtr.Zero;
        Thread t = null;
        bool done = false;
        #endregion

        #region DllImports
        [DllImport("coredll.dll")]
        private static extern IntPtr RequestPowerNotifications(IntPtr hMsgQ, uint Flags);
        [DllImport("coredll.dll")]
        private static extern uint WaitForSingleObject(IntPtr hHandle, int wait);
        [DllImport("coredll.dll")]
        private static extern IntPtr CreateMsgQueue(string name, ref MsgQOptions options);
        [DllImport("coredll.dll")]
        private static extern bool ReadMsgQueue(IntPtr hMsgQ, byte[] lpBuffer, uint cbBufSize, ref uint lpNumRead, int dwTimeout, ref uint pdwFlags);
        #endregion

        public PowerNotifications()
        {
            MsgQOptions options = new MsgQOptions();
            options.dwFlags = 0;
            options.dwMaxMessages = 20;
            options.cbMaxMessage = 10000;
            options.bReadAccess = true;
            options.dwSize = (uint)Marshal.SizeOf(options);
            ptr = CreateMsgQueue("Test", ref options);
            RequestPowerNotifications(ptr, POWER_NOTIFY_ALL);
            t = new Thread(DoWork);
            t.Name = "Power events listening thread";
        }

        public void Start()
        {
            t.Start();
        }
        public void Stop()
        {
            done = true;
            if (!t.Join(2500))
                t.Abort();
        }

        public void Dispose(){
            Stop();
        }

        private bool isSet(UInt32 flag, UInt32 bit){
            if((flag & bit)==bit)
                return true;
            else
                return false;
        }
        private void DoWork()
        {
            byte[] buf = new byte[10000];
            uint nRead = 0, flags = 0, res = 0;

            System.Diagnostics.Debug.WriteLine("starting loop");
            try
            {
                while (!done)
                {
                    res = WaitForSingleObject(ptr, 2500);
                    if (res == 0)
                    {
                        ReadMsgQueue(ptr, buf, (uint)buf.Length, ref nRead, -1, ref flags);
                        //System.Diagnostics.Debug.WriteLine("message: " + ConvertByteArray(buf, 0) + " flag: " + ConvertByteArray(buf, 4));
                        uint flag = ConvertByteArray(buf, 4);
                        string msg = "";
                        msg += ((PowerState)flag).ToString();
                        /*
                        if( (flag & POWER_STATE_ON) == POWER_STATE_ON ){// 65536:
                            msg += " Power On";
                            if (OnResume != null)
                                OnResume(this, EventArgs.Empty);
                        }
                        if( (flag & POWER_STATE_OFF) == POWER_STATE_OFF){// 131072:
                                msg += " Power Off";
                        }
                        if ((flag & POWER_STATE_CRITICAL) == POWER_STATE_CRITICAL){// 262144:
                                msg += " Power Critical";
                        }
                        if(isSet(flag, POWER_STATE_BOOT)){// 524288:
                                msg += " Power Boot";
                        }
                        if(isSet(flag, POWER_STATE_IDLE)){// 1048576:
                                msg += " Power Idle";
                        }
                        if( isSet(flag, POWER_STATE_SUSPEND)){// 2097152:
                            msg += " Power Suspend";
                            if (OnSuspend != null)
                                OnSuspend(this, EventArgs.Empty);
                        }
                        if( isSet(flag, POWER_STATE_RESET)){// 8388608:
                            msg += " Power Reset";
                        }
                        if( isSet(flag, POWER_STATE_UNATTENDED)){
                            msg += " Power Unattended";
                        }
                        if( isSet(flag, POWER_STATE_BACKLIGHTON)){
                            msg += " Power BacklightON";
                        }
                        if( isSet(flag, POWER_STATE_USERIDLE)){
                            msg += "Power UserIdle";
                        }
                        if( isSet(flag, POWER_STATE_PASSWORD)){
                            msg += "Power Password";
                        }
                        //case 0:
                        //    // non power transition messages are ignored
                        //    break;
                        */
                        if (msg=="")
                            msg = "Unknown Flag: " + flag.ToString();
                        if (msg=="0")
                            msg = "POWER_STATE_NA";

                        if (msg != "")
                        {
                            if(OnMsg!=null)
                                OnMsg(this, new PwrEventArgs(msg, flag));
                            System.Diagnostics.Debug.WriteLine(msg);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (!done)
                {
                    System.Diagnostics.Debug.WriteLine("Got exception: " + ex);
                }
            }
            System.Diagnostics.Debug.WriteLine("loop ended");
        }

        uint ConvertByteArray(byte[] array, int offset)
        {
            uint res = 0;
            res += array[offset];
            res += array[offset + 1] * (uint)0x100;
            res += array[offset + 2] * (uint)0x10000;
            res += array[offset + 3] * (uint)0x1000000;
            return res;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MsgQOptions
        {
            public uint dwSize;
            public uint dwFlags;
            public uint dwMaxMessages;
            public uint cbMaxMessage;
            public bool bReadAccess;
        }
    }
}