namespace PingAlert
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
            this.mnuOptions = new System.Windows.Forms.MenuItem();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.btnPingAll = new System.Windows.Forms.Button();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.btnSchedule = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.chkLog2File = new System.Windows.Forms.CheckBox();
            this.btnClearSchedule = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.mnuExit);
            this.mainMenu1.MenuItems.Add(this.mnuOptions);
            // 
            // mnuExit
            // 
            this.mnuExit.Text = "Exit";
            this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
            // 
            // mnuOptions
            // 
            this.mnuOptions.Text = "Options";
            // 
            // listBox1
            // 
            this.listBox1.Location = new System.Drawing.Point(3, 31);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(167, 72);
            this.listBox1.TabIndex = 0;
            // 
            // txtLog
            // 
            this.txtLog.AcceptsReturn = true;
            this.txtLog.AcceptsTab = true;
            this.txtLog.Location = new System.Drawing.Point(3, 191);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLog.Size = new System.Drawing.Size(231, 74);
            this.txtLog.TabIndex = 1;
            this.txtLog.WordWrap = false;
            // 
            // btnPingAll
            // 
            this.btnPingAll.Location = new System.Drawing.Point(5, 109);
            this.btnPingAll.Name = "btnPingAll";
            this.btnPingAll.Size = new System.Drawing.Size(92, 21);
            this.btnPingAll.TabIndex = 2;
            this.btnPingAll.Text = "Ping all";
            this.btnPingAll.Click += new System.EventHandler(this.btnPingAll_Click);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Increment = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.numericUpDown1.Location = new System.Drawing.Point(86, 139);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(50, 22);
            this.numericUpDown1.TabIndex = 3;
            // 
            // btnSchedule
            // 
            this.btnSchedule.Location = new System.Drawing.Point(142, 139);
            this.btnSchedule.Name = "btnSchedule";
            this.btnSchedule.Size = new System.Drawing.Size(92, 21);
            this.btnSchedule.TabIndex = 2;
            this.btnSchedule.Text = "Schedule";
            this.btnSchedule.Click += new System.EventHandler(this.btnSchedule_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(5, 139);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 16);
            this.label1.Text = "Interval:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(82, 4);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(152, 21);
            this.textBox1.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(5, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 16);
            this.label2.Text = "New Host:";
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(176, 31);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(58, 23);
            this.btnAdd.TabIndex = 7;
            this.btnAdd.Text = "Add";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(176, 60);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(58, 23);
            this.btnRemove.TabIndex = 7;
            this.btnRemove.Text = "Remove";
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // chkLog2File
            // 
            this.chkLog2File.Location = new System.Drawing.Point(141, 110);
            this.chkLog2File.Name = "chkLog2File";
            this.chkLog2File.Size = new System.Drawing.Size(92, 19);
            this.chkLog2File.TabIndex = 8;
            this.chkLog2File.Text = "Log to File";
            // 
            // btnClearSchedule
            // 
            this.btnClearSchedule.Location = new System.Drawing.Point(112, 164);
            this.btnClearSchedule.Name = "btnClearSchedule";
            this.btnClearSchedule.Size = new System.Drawing.Size(121, 21);
            this.btnClearSchedule.TabIndex = 2;
            this.btnClearSchedule.Text = "Clear Schedules";
            this.btnClearSchedule.Click += new System.EventHandler(this.btnClearSchedule_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.ControlBox = false;
            this.Controls.Add(this.chkLog2File);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.btnClearSchedule);
            this.Controls.Add(this.btnSchedule);
            this.Controls.Add(this.btnPingAll);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.listBox1);
            this.Menu = this.mainMenu1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Form1_Closing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.MenuItem mnuExit;
        private System.Windows.Forms.MenuItem mnuOptions;
        private System.Windows.Forms.Button btnPingAll;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Button btnSchedule;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.CheckBox chkLog2File;
        private System.Windows.Forms.Button btnClearSchedule;
    }
}

