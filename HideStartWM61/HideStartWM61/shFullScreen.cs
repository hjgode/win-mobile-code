using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Data;
using System.Runtime.InteropServices;

namespace KioskTest
{
    public class SHAPI
    {
        public const int SHFS_SHOWTASKBAR = 1;
        public const int SHFS_HIDETASKBAR = 2;
        public const int SHFS_SHOWSIPBUTTON = 4;
        public const int SHFS_HIDESIPBUTTON = 8;
        public const int SHFS_SHOWSTARTICON = 16;
        public const int SHFS_HIDESTARTICON = 32;

        [DllImport("aygshell.dll")]
        private extern static bool SHFullScreen(IntPtr hWnd, int dwState);

        public static bool showStart(IntPtr wHandle, bool bShowHide)
        {
            bool bRet = false;
            IntPtr hwnd = wHandle;
            if (!bShowHide)
                bRet = SHFullScreen(hwnd, SHFS_HIDESTARTICON);
            else
                bRet = SHFullScreen(hwnd, SHFS_SHOWSTARTICON);
            return bRet;
        }

        public static bool FullScreen(IntPtr hWnd)
        {
            return SHFullScreen(hWnd, SHFS_HIDESTARTICON | SHFS_HIDETASKBAR);
        }
    }

    public class API
    {
        [DllImport("coredll.dll", EntryPoint = "FindWindow")]
        private extern static IntPtr FindWindow(string lpClassName, string
        lpWindowName);

        public static IntPtr FindWindow(string windowName)
        {
            return FindWindow(null, windowName);
        }
    }
    /*
    public class Form1 : System.Windows.Forms.Form
    {
        public Form1()
        {
            System.Windows.Forms.Button button1 = new
            System.Windows.Forms.Button();
            button1.Location = new System.Drawing.Point(48, 80);
            button1.Size = new System.Drawing.Size(128, 48);
            button1.Text = "button1";
            button1.Click += new System.EventHandler(this.button1_Click);
            this.Controls.Add(button1);

            this.ClientSize = new System.Drawing.Size(240, 320);
            this.Text = "KioskMode";

        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        static void Main()
        {
            System.Windows.Forms.Application.Run(new Form1());
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            IntPtr hWnd = API.FindWindow(this.Text);
            if (hWnd != IntPtr.Zero)
            {
                this.MaximizeBox = false;

                this.MinimizeBox = false;

                this.Focus();

                SHAPI.SetForegroundWindow(hWnd);
                SHAPI.FullScreen(hWnd);
            }
        }
    }
    */
}