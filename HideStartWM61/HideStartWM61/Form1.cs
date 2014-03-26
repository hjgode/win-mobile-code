using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace HideStartWM61
{
    public partial class Form1 : Form
    {
        bool bToggle = false;
        public Form1()
        {
            InitializeComponent();
        }
        void showStart(bool bShowHide)
        {
            KioskTest.SHAPI.showStart(this.Handle, bToggle);
            bToggle = !bToggle;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            showStart(bToggle);
        }
    }
}