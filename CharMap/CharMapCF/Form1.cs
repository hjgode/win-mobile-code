using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using CharMapTool;

namespace CharMapCF
{
    public partial class Form1 : Form
    {
        DrawUniMap drawUni;
        byte bCurrCodepage = 0x04;
        //int iOffsetTop = 30;

        public Form1()
        {
            drawUni = new DrawUniMap(this, 50);

            InitializeComponent();

            //combo to select 'codepage'
            cboUnicodePlane.Items.Clear();
            for (int i = 0; i < 0xff; i++)
            {
                cboUnicodePlane.Items.Insert(i, i.ToString("x02"));
            }
            cboUnicodePlane.SelectedIndex = 0x04;

            //combo to select font
            cboFont.Items.Clear();
            //get a list of available fonts
            FontClass fontClass = new FontClass();
            string[] sFonts = fontClass.getFontList();
            for (int iF = 0; iF < sFonts.Length; iF++)
            {
                cboFont.Items.Insert(0, sFonts[iF]);
            }
            //cboFont.Items.Insert(0, "Arial");
            //cboFont.Items.Insert(1, "Arial Unicode MS");
            //cboFont.Items.Insert(2, "Times New Roman");

            //add a fantasy font name to show what happens for non-existing fonts: 
            //windows replaces the font request with a 'matching' existing font
            cboFont.Items.Insert(cboFont.Items.Count, "Fanatasie");

            cboFont.SelectedIndex = 1;

            drawUni._bCurrCodepage = (byte)cboUnicodePlane.SelectedIndex;

            this.FormBorderStyle = FormBorderStyle.Sizable;
            drawUni.NewMessageHandler += new DrawUniMap.KlickedEventHandler(drawUni_NewMessageHandler);

            this.MouseUp += new MouseEventHandler(drawUni._form_MouseUp);
            drawUni._iWidth = 320;
            drawUni._iHeight = 320;

            this.ResumeLayout();
        }

        void drawUni_NewMessageHandler(object sender, DrawUniMap.MessageEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Message: " + e.NewMessage);
            txtUniChar.Text = e.NewMessage;
        }

        private void cboFont_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drawUni == null)
                return;
            drawUni._mapFont = new Font(cboFont.SelectedItem.ToString(), 30, FontStyle.Regular);
            lblFont.Text = drawUni._mapFont.Name;
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cboUnicodePlane_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drawUni == null)
                return;
            bCurrCodepage = (byte)cboUnicodePlane.SelectedIndex;
            drawUni._bCurrCodepage = bCurrCodepage;
            this.Refresh();

        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(txtUniChar.Text);
        }

    }
}