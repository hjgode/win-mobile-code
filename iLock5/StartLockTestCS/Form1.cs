using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace StartLockTestCS
{
    public partial class Form1 : Form
    {
        bool _bHooked = false;
        bool _bLocked = false;
        bool _bFullScreen = false;
        static string appName = "MobileCalculator";
        //strin appName = "fexplore";
        string app = @"\Windows\" + appName + ".exe";

        public Form1()
        {
            InitializeComponent();
            this.Text = "A very long Application title text";
            btn_Launch.Text = "Launch " + appName;
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            if (_bFullScreen)
                StartLock.Unlockdown();
            if (_bHooked)
                StartLock.UnlockStartMenu();
            if (_bLocked)
                StartLock.UnlockStartBar();

            Application.Exit();
        }

        private void btn_LockStart_Click(object sender, EventArgs e)
        {
            if (!_bHooked)
            {
                StartLock.LockStartMenu();
                _bHooked = true;
                label1.Text = "StartMenu locked";
            }
            else
                label1.Text = "StartMenu was already locked";
        }

        private void btn_UnlockStart_Click(object sender, EventArgs e)
        {
            if (_bHooked)
            {
                StartLock.UnlockStartMenu();
                _bHooked = false;
                label1.Text = "StartMenu unlocked";
            }
            else
                label1.Text = "StartMenu was not locked";
        }

        private void btn_LockTaskbar_Click(object sender, EventArgs e)
        {
            if (!_bLocked)
            {
                StartLock.LockStartBar();
                _bLocked = true;
                label1.Text = "StartBar locked";
            }
            else
            {
                label1.Text = "StartBar was locked";
            }
        }

        private void btn_UnlockTaskbar_Click(object sender, EventArgs e)
        {
            if (_bLocked)
            {
                StartLock.UnlockStartBar();
                label1.Text = "StartBar unlocked";
                _bLocked = false;
            }
            else
            {
                label1.Text = "StartBar was unlocked";
            }
        }

        private void btn_LockDown_Click(object sender, EventArgs e)
        {
            string title = this.Text;
            if (!_bFullScreen)
            {
                StartLock.Lockdown(title);
                _bFullScreen = true;
                label1.Text = "Form made fullscreen";
            }
            else
            {
                label1.Text = "Form was already fullscreen";
            }
        }

        private void btn_Unlock_Click(object sender, EventArgs e)
        {
            if (_bFullScreen)
            {
                StartLock.Unlockdown();
                label1.Text = "Form returned to normal state";
                _bFullScreen = false;
            }
            else
                label1.Text = "Forma was not fullscreen";
        }

        private void btn_Launch_Click(object sender, EventArgs e)
        {
            System.Diagnostics.ProcessStartInfo p = 
                new System.Diagnostics.ProcessStartInfo(app, ""); 
            System.Diagnostics.Process.Start(p);             
        }
    }
}