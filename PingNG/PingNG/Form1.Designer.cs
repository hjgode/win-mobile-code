namespace PingNG
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
            this.txtLog = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblPingOptions = new System.Windows.Forms.Label();
            this.btnOptions = new System.Windows.Forms.Button();
            this.btnPing = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtLog
            // 
            this.txtLog.AcceptsReturn = true;
            this.txtLog.AcceptsTab = true;
            this.txtLog.BackColor = System.Drawing.Color.Black;
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtLog.Font = new System.Drawing.Font("Courier New", 8F, System.Drawing.FontStyle.Regular);
            this.txtLog.ForeColor = System.Drawing.Color.Lime;
            this.txtLog.Location = new System.Drawing.Point(0, 95);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLog.Size = new System.Drawing.Size(238, 176);
            this.txtLog.TabIndex = 0;
            this.txtLog.WordWrap = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblPingOptions);
            this.panel1.Controls.Add(this.btnOptions);
            this.panel1.Controls.Add(this.btnPing);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(238, 84);
            // 
            // lblPingOptions
            // 
            this.lblPingOptions.Location = new System.Drawing.Point(5, 34);
            this.lblPingOptions.Name = "lblPingOptions";
            this.lblPingOptions.Size = new System.Drawing.Size(130, 49);
            this.lblPingOptions.Text = "label1";
            // 
            // btnOptions
            // 
            this.btnOptions.Location = new System.Drawing.Point(147, 34);
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.Size = new System.Drawing.Size(74, 23);
            this.btnOptions.TabIndex = 1;
            this.btnOptions.Text = "options";
            this.btnOptions.Click += new System.EventHandler(this.btnOptions_Click);
            // 
            // btnPing
            // 
            this.btnPing.Location = new System.Drawing.Point(147, 5);
            this.btnPing.Name = "btnPing";
            this.btnPing.Size = new System.Drawing.Size(74, 23);
            this.btnPing.TabIndex = 1;
            this.btnPing.Text = "ping";
            this.btnPing.Click += new System.EventHandler(this.btnPing_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(5, 5);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(136, 23);
            this.textBox1.TabIndex = 0;
            this.textBox1.Text = "google.de";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(238, 271);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.txtLog);
            this.MaximizeBox = false;
            this.Menu = this.mainMenu1;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "pingNG";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnOptions;
        private System.Windows.Forms.Button btnPing;
        private System.Windows.Forms.Label lblPingOptions;
    }
}

