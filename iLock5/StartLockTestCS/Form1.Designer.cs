namespace StartLockTestCS
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
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_LockStart = new System.Windows.Forms.Button();
            this.btn_UnlockStart = new System.Windows.Forms.Button();
            this.btn_LockTaskbar = new System.Windows.Forms.Button();
            this.btn_UnlockTaskbar = new System.Windows.Forms.Button();
            this.btn_LockDown = new System.Windows.Forms.Button();
            this.btn_Unlock = new System.Windows.Forms.Button();
            this.btn_Launch = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuItem1);
            // 
            // menuItem1
            // 
            this.menuItem1.Text = "Exit";
            this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(221, 48);
            this.label1.Text = "label1";
            // 
            // btn_LockStart
            // 
            this.btn_LockStart.Location = new System.Drawing.Point(10, 66);
            this.btn_LockStart.Name = "btn_LockStart";
            this.btn_LockStart.Size = new System.Drawing.Size(100, 26);
            this.btn_LockStart.TabIndex = 1;
            this.btn_LockStart.Text = "Lock Start";
            this.btn_LockStart.Click += new System.EventHandler(this.btn_LockStart_Click);
            // 
            // btn_UnlockStart
            // 
            this.btn_UnlockStart.Location = new System.Drawing.Point(129, 66);
            this.btn_UnlockStart.Name = "btn_UnlockStart";
            this.btn_UnlockStart.Size = new System.Drawing.Size(100, 26);
            this.btn_UnlockStart.TabIndex = 1;
            this.btn_UnlockStart.Text = "Unlock Start";
            this.btn_UnlockStart.Click += new System.EventHandler(this.btn_UnlockStart_Click);
            // 
            // btn_LockTaskbar
            // 
            this.btn_LockTaskbar.Location = new System.Drawing.Point(10, 112);
            this.btn_LockTaskbar.Name = "btn_LockTaskbar";
            this.btn_LockTaskbar.Size = new System.Drawing.Size(100, 26);
            this.btn_LockTaskbar.TabIndex = 1;
            this.btn_LockTaskbar.Text = "Lock Taskbar";
            this.btn_LockTaskbar.Click += new System.EventHandler(this.btn_LockTaskbar_Click);
            // 
            // btn_UnlockTaskbar
            // 
            this.btn_UnlockTaskbar.Location = new System.Drawing.Point(129, 112);
            this.btn_UnlockTaskbar.Name = "btn_UnlockTaskbar";
            this.btn_UnlockTaskbar.Size = new System.Drawing.Size(100, 26);
            this.btn_UnlockTaskbar.TabIndex = 1;
            this.btn_UnlockTaskbar.Text = "Unlock Taskbar";
            this.btn_UnlockTaskbar.Click += new System.EventHandler(this.btn_UnlockTaskbar_Click);
            // 
            // btn_LockDown
            // 
            this.btn_LockDown.Location = new System.Drawing.Point(10, 158);
            this.btn_LockDown.Name = "btn_LockDown";
            this.btn_LockDown.Size = new System.Drawing.Size(100, 26);
            this.btn_LockDown.TabIndex = 1;
            this.btn_LockDown.Text = "Lockdown";
            this.btn_LockDown.Click += new System.EventHandler(this.btn_LockDown_Click);
            // 
            // btn_Unlock
            // 
            this.btn_Unlock.Location = new System.Drawing.Point(129, 158);
            this.btn_Unlock.Name = "btn_Unlock";
            this.btn_Unlock.Size = new System.Drawing.Size(100, 26);
            this.btn_Unlock.TabIndex = 1;
            this.btn_Unlock.Text = "Unlock";
            this.btn_Unlock.Click += new System.EventHandler(this.btn_Unlock_Click);
            // 
            // btn_Launch
            // 
            this.btn_Launch.Location = new System.Drawing.Point(22, 221);
            this.btn_Launch.Name = "btn_Launch";
            this.btn_Launch.Size = new System.Drawing.Size(195, 26);
            this.btn_Launch.TabIndex = 3;
            this.btn_Launch.Text = "Launch File Explorer";
            this.btn_Launch.Click += new System.EventHandler(this.btn_Launch_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.ControlBox = false;
            this.Controls.Add(this.btn_Launch);
            this.Controls.Add(this.btn_Unlock);
            this.Controls.Add(this.btn_UnlockTaskbar);
            this.Controls.Add(this.btn_UnlockStart);
            this.Controls.Add(this.btn_LockDown);
            this.Controls.Add(this.btn_LockTaskbar);
            this.Controls.Add(this.btn_LockStart);
            this.Controls.Add(this.label1);
            this.Menu = this.mainMenu1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.Button btn_LockStart;
        private System.Windows.Forms.Button btn_UnlockStart;
        private System.Windows.Forms.Button btn_LockTaskbar;
        private System.Windows.Forms.Button btn_UnlockTaskbar;
        private System.Windows.Forms.Button btn_LockDown;
        private System.Windows.Forms.Button btn_Unlock;
        private System.Windows.Forms.Button btn_Launch;
    }
}

