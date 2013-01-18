using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MoveableFormCF2
{
    public partial class Form1 : Form
    {
        uint uiCurrentStyle = 0;
        uint uiCurrentStyleEx = 0;

        int offsetY = 36;
        int offsetX = 10;
        int widthX=200;
        int heightX = 32;
        winapi.subclassForm subClassedForm;
        public Form1()
        {
            InitializeComponent();

            subClassedForm = new winapi.subclassForm(this);
            subClassedForm.wndProcEvent += new winapi.subclassForm.wndProcEventHandler(subClassedForm_wndProcEvent);
            widthX = this.Width - 2 * offsetX;

            uiCurrentStyle = winapi.getStyle(this);
            uiCurrentStyleEx = winapi.getStyleEx(this);
            System.Diagnostics.Debug.WriteLine("Style  =0x" + string.Format("{0:x}", uiCurrentStyle));
            System.Diagnostics.Debug.WriteLine("StyleEx=0x" + string.Format("{0:x}", uiCurrentStyleEx));
            buildOptions();
            buildOptionsEx();
        }

        /// <summary>
        /// called by every wndproc message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="wndProcArgs"></param>
        void subClassedForm_wndProcEvent(object sender, winapi.subclassForm.wndprocEventArgs wndProcArgs)
        {
            //WM_MOVE xPos = (int)LOWORD(lParam);   yPos = (int)HIWORD(lParam);
            if (wndProcArgs.msg == (uint)winapi.subclassForm.WNDMSGS.WM_MOVE)
                addLog("WM_MOVE: X/Y = " + LoWord(wndProcArgs.lParam).ToString() + "/" + HiWord(wndProcArgs.lParam).ToString());
        }
        private int LoWord(IntPtr param)
        {
            return (ushort)(param.ToInt32() & ushort.MaxValue);
        }

        private int HiWord(IntPtr param)
        {
            return (ushort)(param.ToInt32() >> 16);
        }
        //build a list of chk options for WSYTLES
        void buildOptions()
        {
            string[] stylesList = winapi.getStyles();

            int iCount=0;
            foreach (string s in stylesList)
            {
                CheckBox chkBox = new CheckBox();
                chkBox.Left = offsetX;
                chkBox.Top = iCount * offsetY;
                chkBox.Size = new Size(widthX, heightX);
                chkBox.Text = s;
                uint uStyle = (uint)Enum.Parse(typeof(winapi.WINSTYLES),s,false);
                if ((uiCurrentStyle & uStyle) == uStyle)
                    chkBox.Checked = true;
                chkBox.CheckStateChanged += new EventHandler(chkBox_CheckStateChanged);
                tabPage1.Controls.Add(chkBox);
                iCount++;
            }
        }
        void buildOptionsEx()
        {
            string[] stylesList = winapi.getStylesEx();

            int iCount = 0;
            foreach (string s in stylesList)
            {
                CheckBox chkBox = new CheckBox();
                chkBox.Left = offsetX;
                chkBox.Top = iCount * offsetY;
                chkBox.Size = new Size(widthX, heightX);
                chkBox.Text = s;
                uint uStyle = (uint)Enum.Parse(typeof(winapi.WINEXSTYLES), s, false);
                if ((uiCurrentStyleEx & uStyle) == uStyle)
                    chkBox.Checked = true;
                chkBox.CheckStateChanged += new EventHandler(chkBox_CheckStateChangedEx);
                tabPage2.Controls.Add(chkBox);
                iCount++;
            }
        }

        void chkBox_CheckStateChanged(object sender, EventArgs e)
        {
            string s = ((CheckBox)sender).Text;
            uint uStyle = (uint)Enum.Parse(typeof(winapi.WINSTYLES), s, false);
            if(((CheckBox)sender).Checked)
                winapi.setStyle(this, uStyle);
            else
                winapi.unsetStyle(this, uStyle);
            uiCurrentStyle = winapi.getStyle(this);

            addLog("Style  =0x" + string.Format("{0:x}", uiCurrentStyle));
            addLog("StyleEx=0x" + string.Format("{0:x}", uiCurrentStyleEx));

            this.Refresh();
        }
        void chkBox_CheckStateChangedEx(object sender, EventArgs e)
        {
            string s = ((CheckBox)sender).Text;
            uint uStyle = (uint)Enum.Parse(typeof(winapi.WINEXSTYLES), s, false);
            if (((CheckBox)sender).Checked)
                winapi.setStyleEx(this, uStyle);
            else
                winapi.unsetStyleEx(this, uStyle);
            uiCurrentStyleEx = winapi.getStyleEx(this);

            addLog("Style  =0x" + string.Format("{0:x}", uiCurrentStyle));
            addLog("StyleEx=0x" + string.Format("{0:x}", uiCurrentStyleEx));

            this.Refresh();
        }

        private void chkMaximized_CheckStateChanged(object sender, EventArgs e)
        {
            if (chkMaximized.Checked)
            {
                this.WindowState = FormWindowState.Maximized;
                //this.Menu = null;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
                //this.Menu = mainMenu1;
            }
        }

        private void chkMenu_CheckStateChanged(object sender, EventArgs e)
        {
            if (chkMenu.Checked)
                this.Menu = mainMenu1;
            else
                this.Menu = null;
        }
        delegate void SetTextCallback(string text);
        public void addLog(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.txtLog.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(addLog);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                if (txtLog.Text.Length > 2000)
                    txtLog.Text = "";
                txtLog.Text += text + "\r\n";
                txtLog.SelectionLength = 0;
                txtLog.SelectionStart = txtLog.Text.Length - 1;
                txtLog.ScrollToCaret();
            }
        }
    }
}