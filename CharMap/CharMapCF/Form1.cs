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
        DrawUniMap drawUniPanel;
        byte bCurrCodepage = 0x04;
        //int iOffsetTop = 30;

        public Form1()
        {

            InitializeComponent();

            drawUniPanel = new DrawUniMap();
            //is this a VGA screen? HI_RES_AWARE
            if(Screen.PrimaryScreen.WorkingArea.Width>240)
                drawUniPanel.Location = new Point(0, 100);
            else
                drawUniPanel.Location = new Point(0, 50);
            drawUniPanel.Size = new Size(this.Width, this.Height - 50);
            this.Controls.Add(drawUniPanel);

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

            //add a fantasy font name to show what happens for non-existing fonts: 
            //windows replaces the font request with a 'matching' existing font
            cboFont.Items.Insert(cboFont.Items.Count, "Fanatasie");

            cboFont.SelectedIndex = 1;

            drawUniPanel._bCurrCodepage = (byte)cboUnicodePlane.SelectedIndex;

            this.FormBorderStyle = FormBorderStyle.Sizable;
            drawUniPanel.NewMessageHandler += new DrawUniMap.KlickedEventHandler(drawUni_NewMessageHandler);

            this.ResumeLayout();
        }

        void drawUni_NewMessageHandler(object sender, DrawUniMap.MessageEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Message: " + e.NewMessage);
            txtUniChar.Text = e.NewMessage;

            this.Text = e.getBytesString;
        }

        private void cboFont_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drawUniPanel == null)
                return;
            drawUniPanel._mapFont = new Font(cboFont.SelectedItem.ToString(), 30, FontStyle.Regular);
            lblFont.Text = drawUniPanel._mapFont.Name;
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cboUnicodePlane_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drawUniPanel == null)
                return;
            bCurrCodepage = (byte)cboUnicodePlane.SelectedIndex;
            drawUniPanel._bCurrCodepage = bCurrCodepage;
            this.Refresh();

        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(txtUniChar.Text);
        }

        private void mnuZoomIn_Click(object sender, EventArgs e)
        {
            drawUniPanel.zoomInOut(1.2);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txtUniChar.Font = new Font("Arial Unicode MS", 9, FontStyle.Bold);
        }

        private void mnuZoomOut_Click(object sender, EventArgs e)
        {
            drawUniPanel.zoomInOut(0.8);
        }

    }
}