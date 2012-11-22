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

        public Form1()
        {
            InitializeComponent();
            widthX = this.Width - 2 * offsetX;

            uiCurrentStyle = winapi.getStyle(this);
            uiCurrentStyleEx = winapi.getStyleEx(this);
            System.Diagnostics.Debug.WriteLine("Style  =0x" + string.Format("{0:x}", uiCurrentStyle));
            System.Diagnostics.Debug.WriteLine("StyleEx=0x" + string.Format("{0:x}", uiCurrentStyleEx));
            buildOptions();
            buildOptionsEx();
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
    }
}