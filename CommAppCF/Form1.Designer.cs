namespace CommAppCF
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
            this.mnuConnectMain = new System.Windows.Forms.MenuItem();
            this.mnuSerialConnect = new System.Windows.Forms.MenuItem();
            this.mnuSocketConnect = new System.Windows.Forms.MenuItem();
            this.txtSend = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSend = new System.Windows.Forms.Button();
            this.txtReceive = new System.Windows.Forms.TextBox();
            this.btnSendFile = new System.Windows.Forms.Button();
            this.btnSendLine = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.chkUseHexEncoder = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.mnuExit);
            this.mainMenu1.MenuItems.Add(this.mnuConnectMain);
            // 
            // mnuExit
            // 
            this.mnuExit.Text = "Exit";
            this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
            // 
            // mnuConnectMain
            // 
            this.mnuConnectMain.MenuItems.Add(this.mnuSerialConnect);
            this.mnuConnectMain.MenuItems.Add(this.mnuSocketConnect);
            this.mnuConnectMain.Text = "Connect";
            // 
            // mnuSerialConnect
            // 
            this.mnuSerialConnect.Text = "Serial";
            this.mnuSerialConnect.Click += new System.EventHandler(this.mnuSerialConnect_Click);
            // 
            // mnuSocketConnect
            // 
            this.mnuSocketConnect.Text = "BT Connect";
            this.mnuSocketConnect.Click += new System.EventHandler(this.mnuSocketConnect_Click);
            // 
            // txtSend
            // 
            this.txtSend.AcceptsReturn = true;
            this.txtSend.AcceptsTab = true;
            this.txtSend.Location = new System.Drawing.Point(3, 31);
            this.txtSend.Multiline = true;
            this.txtSend.Name = "txtSend";
            this.txtSend.Size = new System.Drawing.Size(234, 91);
            this.txtSend.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(3, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 18);
            this.label1.Text = "SEND:";
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(175, 126);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(62, 23);
            this.btnSend.TabIndex = 2;
            this.btnSend.Text = "Send";
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // txtReceive
            // 
            this.txtReceive.AcceptsReturn = true;
            this.txtReceive.AcceptsTab = true;
            this.txtReceive.Location = new System.Drawing.Point(3, 180);
            this.txtReceive.Multiline = true;
            this.txtReceive.Name = "txtReceive";
            this.txtReceive.ReadOnly = true;
            this.txtReceive.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtReceive.Size = new System.Drawing.Size(234, 85);
            this.txtReceive.TabIndex = 0;
            // 
            // btnSendFile
            // 
            this.btnSendFile.Location = new System.Drawing.Point(3, 126);
            this.btnSendFile.Name = "btnSendFile";
            this.btnSendFile.Size = new System.Drawing.Size(72, 23);
            this.btnSendFile.TabIndex = 2;
            this.btnSendFile.Text = "Send File";
            this.btnSendFile.Click += new System.EventHandler(this.btnSendFile_Click);
            // 
            // btnSendLine
            // 
            this.btnSendLine.Location = new System.Drawing.Point(96, 126);
            this.btnSendLine.Name = "btnSendLine";
            this.btnSendLine.Size = new System.Drawing.Size(73, 23);
            this.btnSendLine.TabIndex = 2;
            this.btnSendLine.Text = "Send Line";
            this.btnSendLine.Click += new System.EventHandler(this.btnSendLine_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(175, 5);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(62, 23);
            this.btnClear.TabIndex = 2;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // chkUseHexEncoder
            // 
            this.chkUseHexEncoder.Location = new System.Drawing.Point(3, 155);
            this.chkUseHexEncoder.Name = "chkUseHexEncoder";
            this.chkUseHexEncoder.Size = new System.Drawing.Size(234, 19);
            this.chkUseHexEncoder.TabIndex = 4;
            this.chkUseHexEncoder.Text = "use \\xAB en/decoding";
            this.chkUseHexEncoder.CheckStateChanged += new System.EventHandler(this.chkUseHexEncoder_CheckStateChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.ControlBox = false;
            this.Controls.Add(this.chkUseHexEncoder);
            this.Controls.Add(this.btnSendFile);
            this.Controls.Add(this.btnSendLine);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtReceive);
            this.Controls.Add(this.txtSend);
            this.Menu = this.mainMenu1;
            this.Name = "Form1";
            this.Text = "CommApp";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Form1_Closing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuItem mnuExit;
        private System.Windows.Forms.MenuItem mnuConnectMain;
        private System.Windows.Forms.TextBox txtSend;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox txtReceive;
        private System.Windows.Forms.Button btnSendFile;
        private System.Windows.Forms.Button btnSendLine;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.CheckBox chkUseHexEncoder;
        private System.Windows.Forms.MenuItem mnuSerialConnect;
        private System.Windows.Forms.MenuItem mnuSocketConnect;
    }
}

