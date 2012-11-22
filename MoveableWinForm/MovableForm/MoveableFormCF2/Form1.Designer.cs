namespace MoveableFormCF2
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabExtras = new System.Windows.Forms.TabPage();
            this.chkMaximized = new System.Windows.Forms.CheckBox();
            this.chkMenu = new System.Windows.Forms.CheckBox();
            this.tabControl1.SuspendLayout();
            this.tabExtras.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabExtras);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(226, 265);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.AutoScroll = true;
            this.tabPage1.Location = new System.Drawing.Point(0, 0);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(226, 242);
            this.tabPage1.Text = "Styles";
            // 
            // tabPage2
            // 
            this.tabPage2.AutoScroll = true;
            this.tabPage2.Location = new System.Drawing.Point(0, 0);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(218, 239);
            this.tabPage2.Text = "EX Styles";
            // 
            // tabExtras
            // 
            this.tabExtras.Controls.Add(this.chkMenu);
            this.tabExtras.Controls.Add(this.chkMaximized);
            this.tabExtras.Location = new System.Drawing.Point(0, 0);
            this.tabExtras.Name = "tabExtras";
            this.tabExtras.Size = new System.Drawing.Size(226, 242);
            this.tabExtras.Text = "Extras";
            // 
            // chkMaximized
            // 
            this.chkMaximized.Location = new System.Drawing.Point(18, 28);
            this.chkMaximized.Name = "chkMaximized";
            this.chkMaximized.Size = new System.Drawing.Size(100, 22);
            this.chkMaximized.TabIndex = 0;
            this.chkMaximized.Text = "Maximized";
            this.chkMaximized.CheckStateChanged += new System.EventHandler(this.chkMaximized_CheckStateChanged);
            // 
            // chkMenu
            // 
            this.chkMenu.Checked = true;
            this.chkMenu.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMenu.Location = new System.Drawing.Point(18, 56);
            this.chkMenu.Name = "chkMenu";
            this.chkMenu.Size = new System.Drawing.Size(100, 22);
            this.chkMenu.TabIndex = 0;
            this.chkMenu.Text = "MainMenu";
            this.chkMenu.CheckStateChanged += new System.EventHandler(this.chkMenu_CheckStateChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(226, 265);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.Menu = this.mainMenu1;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "Form1";
            this.tabControl1.ResumeLayout(false);
            this.tabExtras.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabExtras;
        private System.Windows.Forms.CheckBox chkMaximized;
        private System.Windows.Forms.CheckBox chkMenu;
    }
}

