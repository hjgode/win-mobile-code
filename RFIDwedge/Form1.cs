using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Runtime.InteropServices;

namespace RFID_Wedge
{
    public partial class Form1 : Form
    {
        [DllImport("coredll.dll", EntryPoint = "keybd_event", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, keyFlag dwFlags, int dwExtraInfo);

        [Flags]
        enum keyFlag:int
        {
            KEYEVENTF_KEYDOWN = 0x0000,
            KEYEVENTF_EXTENDEDKEY = 0x0001,  //If specified, the scan code was preceded by a prefix byte having the value 0xE0 (224).
            KEYEVENTF_KEYUP = 0x0002
        }

        ThreadClass.myThread mThread;

        public Form1()
        {
            InitializeComponent();
            btnConnect_Click(this, new EventArgs());
        }
        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (mThread == null)
            {
                mThread = new ThreadClass.myThread();
                mThread.MyEvent += new EventHandler<ThreadClass.myThread.MyEventArgs>(mThread_MyEvent);
            }
            this.Hide();
        }

        public Object thisLock = new Object();

        void mThread_MyEvent(object sender, ThreadClass.myThread.MyEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Received event from thread: " + e.Message);
            
            lock (thisLock){
                //stringQueue.Enqueue(e.Message);

                //better do the following from a separate thread
                byte[] bBuf = Encoding.ASCII.GetBytes(e.Message);
                //only digits and letters are the same in Byte and VK_ values

                byte bVkey = 0x00; 
                bool bShift = false;
                for (int i = 0; i < bBuf.Length; i++)
                {
                   // System.Diagnostics.Debug.WriteLine("buffer: " + bBuf[i]);
                    //so start a translation
                    
                    bVkey = virtual_key_codes.vkTable[bBuf[i]].VKval;
                    bShift = virtual_key_codes.vkTable[bBuf[i]].bShift;

                    if (bVkey != (byte)virtual_key_codes.V_KEY.VK_undef_0xff)
                    {
                        if (bShift)
                        {
                            keybd_event((byte)virtual_key_codes.V_KEY.VK_SHIFT, 0x00, keyFlag.KEYEVENTF_KEYDOWN, 0);
                            System.Threading.Thread.Sleep(2);
                        }
                        //send key
                        keybd_event(bVkey, 0x00, keyFlag.KEYEVENTF_KEYDOWN, 0);
                        System.Threading.Thread.Sleep(2);
                        keybd_event(bVkey, 0x00, keyFlag.KEYEVENTF_KEYUP, 0);
                        System.Threading.Thread.Sleep(2);
                        if (bShift)
                        {
                            keybd_event((byte)virtual_key_codes.V_KEY.VK_SHIFT, 0x00, keyFlag.KEYEVENTF_KEYUP, 0);
                            System.Threading.Thread.Sleep(2);
                        }
                    }
                }

            }
        }

        private void Form1_Closing(object sender, CancelEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("\n form closing...");
            if (mThread != null)
            {
                mThread.stopThread = true;
                System.Threading.Thread.Sleep(1000);
            }
            mThread.Dispose();
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}