namespace SetTimeTest
{
    partial class TimeInfo
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.systemTimeUC2 = new SetTimeTest.SystemTimeUC();
            this.systemTimeUC1 = new SetTimeTest.SystemTimeUC();
            this.txtBias = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtTZstatus = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.timeZoneBiasUC1 = new SetTimeTest.TimeZoneBiasUC();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.timeZoneBiasUC2 = new SetTimeTest.TimeZoneBiasUC();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.lblAutoDST = new System.Windows.Forms.Label();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.mnuExit = new System.Windows.Forms.MenuItem();
            this.mnuRefresh = new System.Windows.Forms.MenuItem();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuItem1);
            this.mainMenu1.MenuItems.Add(this.mnuRefresh);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(240, 268);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.systemTimeUC2);
            this.tabPage1.Controls.Add(this.systemTimeUC1);
            this.tabPage1.Controls.Add(this.txtBias);
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Location = new System.Drawing.Point(0, 0);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(240, 245);
            this.tabPage1.Text = "Time";
            // 
            // systemTimeUC2
            // 
            this.systemTimeUC2.Location = new System.Drawing.Point(4, 84);
            this.systemTimeUC2.Name = "systemTimeUC2";
            this.systemTimeUC2.Size = new System.Drawing.Size(233, 77);
            this.systemTimeUC2.TabIndex = 6;
            // 
            // systemTimeUC1
            // 
            this.systemTimeUC1.Location = new System.Drawing.Point(4, 5);
            this.systemTimeUC1.Name = "systemTimeUC1";
            this.systemTimeUC1.Size = new System.Drawing.Size(233, 77);
            this.systemTimeUC1.TabIndex = 2;
            // 
            // txtBias
            // 
            this.txtBias.Location = new System.Drawing.Point(119, 221);
            this.txtBias.Name = "txtBias";
            this.txtBias.ReadOnly = true;
            this.txtBias.Size = new System.Drawing.Size(114, 21);
            this.txtBias.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtTZstatus);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(3, 162);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(232, 54);
            // 
            // txtTZstatus
            // 
            this.txtTZstatus.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.txtTZstatus.Location = new System.Drawing.Point(4, 23);
            this.txtTZstatus.Name = "txtTZstatus";
            this.txtTZstatus.Size = new System.Drawing.Size(148, 19);
            this.txtTZstatus.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(158, 20);
            this.label1.Text = "TimeZoneInformation:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(7, 224);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 18);
            this.label2.Text = "Offset (minutes):";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.Green;
            this.tabPage2.Controls.Add(this.timeZoneBiasUC1);
            this.tabPage2.Location = new System.Drawing.Point(0, 0);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(232, 242);
            this.tabPage2.Text = "Standard";
            // 
            // timeZoneBiasUC1
            // 
            this.timeZoneBiasUC1.Location = new System.Drawing.Point(7, 7);
            this.timeZoneBiasUC1.Name = "timeZoneBiasUC1";
            this.timeZoneBiasUC1.Size = new System.Drawing.Size(229, 207);
            this.timeZoneBiasUC1.TabIndex = 3;
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(128)))));
            this.tabPage3.Controls.Add(this.timeZoneBiasUC2);
            this.tabPage3.Location = new System.Drawing.Point(0, 0);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(232, 242);
            this.tabPage3.Text = "Daylight";
            // 
            // timeZoneBiasUC2
            // 
            this.timeZoneBiasUC2.Location = new System.Drawing.Point(8, 7);
            this.timeZoneBiasUC2.Name = "timeZoneBiasUC2";
            this.timeZoneBiasUC2.Size = new System.Drawing.Size(229, 208);
            this.timeZoneBiasUC2.TabIndex = 0;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.lblAutoDST);
            this.tabPage4.Location = new System.Drawing.Point(0, 0);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(232, 242);
            this.tabPage4.Text = "AutoDST";
            // 
            // lblAutoDST
            // 
            this.lblAutoDST.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular);
            this.lblAutoDST.Location = new System.Drawing.Point(20, 28);
            this.lblAutoDST.Name = "lblAutoDST";
            this.lblAutoDST.Size = new System.Drawing.Size(196, 60);
            this.lblAutoDST.Text = "AutoDST is";
            this.lblAutoDST.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // menuItem1
            // 
            this.menuItem1.MenuItems.Add(this.mnuExit);
            this.menuItem1.Text = "File";
            // 
            // mnuExit
            // 
            this.mnuExit.Text = "Exit";
            // 
            // mnuRefresh
            // 
            this.mnuRefresh.Text = "Refresh";
            this.mnuRefresh.Click += new System.EventHandler(this.mnuRefresh_Click);
            // 
            // TimeInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.tabControl1);
            this.Menu = this.mainMenu1;
            this.MinimizeBox = false;
            this.Name = "TimeInfo";
            this.Text = "TimeInfo";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private SystemTimeUC systemTimeUC1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtTZstatus;
        private System.Windows.Forms.TextBox txtBias;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPage2;
        private TimeZoneBiasUC timeZoneBiasUC1;
        private System.Windows.Forms.TabPage tabPage3;
        private TimeZoneBiasUC timeZoneBiasUC2;
        private SystemTimeUC systemTimeUC2;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Label lblAutoDST;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem mnuExit;
        private System.Windows.Forms.MenuItem mnuRefresh;

    }
}