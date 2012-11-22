using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MovableForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnMove_Click(object sender, EventArgs e)
        {
            //winapi.moveWindow(this);
            winapi.setStyle(this);
        }

        //global var
        TextBox currentTB = null;
        private void button1_Click(object sender, EventArgs e)
        {
            inputPanel1.Enabled = !inputPanel1.Enabled;
            if(currentTB!=null)
                currentTB.Focus();
        }

        private void textBox1_GotFocus(object sender, EventArgs e)
        {
            currentTB = (TextBox)sender;
        }
    }
}