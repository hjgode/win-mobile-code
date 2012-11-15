namespace disableTouchCS
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
            System.Windows.Forms.Label label1;
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.btnLockTouch = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            label1.Location = new System.Drawing.Point(22, 145);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(192, 88);
            label1.Text = "When touch screen is locked, press any key to unlock!";
            label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnLockTouch
            // 
            this.btnLockTouch.Location = new System.Drawing.Point(41, 31);
            this.btnLockTouch.Name = "btnLockTouch";
            this.btnLockTouch.Size = new System.Drawing.Size(157, 39);
            this.btnLockTouch.TabIndex = 0;
            this.btnLockTouch.Text = "Lock Touch Input";
            this.btnLockTouch.Click += new System.EventHandler(this.btnLockTouch_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblStatus.Location = new System.Drawing.Point(57, 91);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(125, 29);
            this.lblStatus.Text = "unlocked?";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(label1);
            this.Controls.Add(this.btnLockTouch);
            this.KeyPreview = true;
            this.Menu = this.mainMenu1;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "disableTouchCS";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnLockTouch;
        private System.Windows.Forms.Label lblStatus;
    }
}

