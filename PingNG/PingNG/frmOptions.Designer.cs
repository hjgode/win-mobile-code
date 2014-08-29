namespace PingNG
{
    partial class frmOptions
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
            this.mnuCancel = new System.Windows.Forms.MenuItem();
            this.mnuOK = new System.Windows.Forms.MenuItem();
            this.chkDoNotFragment = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.numTTL = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numTimeout = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.numPings = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.numBuffSize = new System.Windows.Forms.NumericUpDown();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.mnuCancel);
            this.mainMenu1.MenuItems.Add(this.mnuOK);
            // 
            // mnuCancel
            // 
            this.mnuCancel.Text = "Cancel";
            this.mnuCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // mnuOK
            // 
            this.mnuOK.Text = "OK";
            this.mnuOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // chkDoNotFragment
            // 
            this.chkDoNotFragment.Location = new System.Drawing.Point(9, 116);
            this.chkDoNotFragment.Name = "chkDoNotFragment";
            this.chkDoNotFragment.Size = new System.Drawing.Size(181, 30);
            this.chkDoNotFragment.TabIndex = 0;
            this.chkDoNotFragment.Text = "do not fragment (DF)";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(9, 82);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 18);
            this.label1.Text = "TTL:";
            // 
            // numTTL
            // 
            this.numTTL.Location = new System.Drawing.Point(84, 82);
            this.numTTL.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
            this.numTTL.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numTTL.Name = "numTTL";
            this.numTTL.Size = new System.Drawing.Size(94, 24);
            this.numTTL.TabIndex = 2;
            this.numTTL.Value = new decimal(new int[] {
            128,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(9, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 18);
            this.label2.Text = "Timeout:";
            // 
            // numTimeout
            // 
            this.numTimeout.Location = new System.Drawing.Point(85, 46);
            this.numTimeout.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numTimeout.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numTimeout.Name = "numTimeout";
            this.numTimeout.Size = new System.Drawing.Size(93, 24);
            this.numTimeout.TabIndex = 2;
            this.numTimeout.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(184, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 21);
            this.label3.Text = "hops";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(185, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 21);
            this.label4.Text = "ms";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(138, 228);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(87, 29);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(8, 228);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(87, 29);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(8, 152);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(88, 18);
            this.label5.Text = "Num pings:";
            // 
            // numPings
            // 
            this.numPings.Location = new System.Drawing.Point(138, 152);
            this.numPings.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numPings.Name = "numPings";
            this.numPings.Size = new System.Drawing.Size(87, 24);
            this.numPings.TabIndex = 2;
            this.numPings.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(9, 182);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(88, 18);
            this.label6.Text = "Buffer Size:";
            // 
            // numBuffSize
            // 
            this.numBuffSize.Location = new System.Drawing.Point(138, 182);
            this.numBuffSize.Maximum = new decimal(new int[] {
            9000,
            0,
            0,
            0});
            this.numBuffSize.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numBuffSize.Name = "numBuffSize";
            this.numBuffSize.Size = new System.Drawing.Size(87, 24);
            this.numBuffSize.TabIndex = 2;
            this.numBuffSize.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            // 
            // frmOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(238, 295);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numBuffSize);
            this.Controls.Add(this.numPings);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.numTimeout);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.numTTL);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkDoNotFragment);
            this.MaximizeBox = false;
            this.Menu = this.mainMenu1;
            this.MinimizeBox = false;
            this.Name = "frmOptions";
            this.Text = "Ping options";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuItem mnuCancel;
        private System.Windows.Forms.MenuItem mnuOK;
        private System.Windows.Forms.CheckBox chkDoNotFragment;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numTTL;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numTimeout;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numPings;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numBuffSize;
    }
}