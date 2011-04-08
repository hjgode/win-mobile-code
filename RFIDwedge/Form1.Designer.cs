namespace RFID_Wedge
{
    partial class Form1
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
            this.mnuExit = new System.Windows.Forms.MenuItem();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.txtTagData = new System.Windows.Forms.TextBox();
            this.txtTagID = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.mnuExit);
            // 
            // mnuExit
            // 
            this.mnuExit.Text = "Exit";
            this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(3, 143);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLog.Size = new System.Drawing.Size(234, 122);
            this.txtLog.TabIndex = 0;
            this.txtLog.WordWrap = false;
            // 
            // txtTagData
            // 
            this.txtTagData.Location = new System.Drawing.Point(9, 60);
            this.txtTagData.Name = "txtTagData";
            this.txtTagData.Size = new System.Drawing.Size(190, 21);
            this.txtTagData.TabIndex = 1;
            // 
            // txtTagID
            // 
            this.txtTagID.Location = new System.Drawing.Point(9, 87);
            this.txtTagID.Name = "txtTagID";
            this.txtTagID.Size = new System.Drawing.Size(190, 21);
            this.txtTagID.TabIndex = 1;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(86, 14);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(71, 26);
            this.btnConnect.TabIndex = 2;
            this.btnConnect.Text = "connect";
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.ControlBox = false;
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.txtTagID);
            this.Controls.Add(this.txtTagData);
            this.Controls.Add(this.txtLog);
            this.Menu = this.mainMenu1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Form1_Closing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.TextBox txtTagData;
        private System.Windows.Forms.TextBox txtTagID;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.MenuItem mnuExit;
    }
}

