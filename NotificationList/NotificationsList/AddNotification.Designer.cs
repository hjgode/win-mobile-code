namespace NotificationsList
{
    partial class AddNotification
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
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.txtApp = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtArg = new System.Windows.Forms.TextBox();
            this.btnFile = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtStart = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtEnd = new System.Windows.Forms.TextBox();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.btnSetStart = new System.Windows.Forms.Button();
            this.btnSetEnd = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuItem1);
            this.mainMenu1.MenuItems.Add(this.menuItem2);
            // 
            // menuItem1
            // 
            this.menuItem1.Text = "Cancel";
            this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Text = "OK";
            this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 18);
            this.label1.Text = "Application:";
            // 
            // txtApp
            // 
            this.txtApp.Location = new System.Drawing.Point(3, 29);
            this.txtApp.Name = "txtApp";
            this.txtApp.Size = new System.Drawing.Size(234, 21);
            this.txtApp.TabIndex = 1;
            this.txtApp.TextChanged += new System.EventHandler(this.txtApp_TextChanged);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(3, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 18);
            this.label2.Text = "Arguments:";
            // 
            // txtArg
            // 
            this.txtArg.Location = new System.Drawing.Point(3, 88);
            this.txtArg.Name = "txtArg";
            this.txtArg.Size = new System.Drawing.Size(234, 21);
            this.txtArg.TabIndex = 1;
            // 
            // btnFile
            // 
            this.btnFile.Location = new System.Drawing.Point(203, 4);
            this.btnFile.Name = "btnFile";
            this.btnFile.Size = new System.Drawing.Size(34, 19);
            this.btnFile.TabIndex = 3;
            this.btnFile.Text = "...";
            this.btnFile.Click += new System.EventHandler(this.button1_Click);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(3, 122);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 18);
            this.label3.Text = "Start:";
            // 
            // txtStart
            // 
            this.txtStart.Location = new System.Drawing.Point(3, 143);
            this.txtStart.Name = "txtStart";
            this.txtStart.Size = new System.Drawing.Size(106, 21);
            this.txtStart.TabIndex = 1;
            this.txtStart.TextChanged += new System.EventHandler(this.txtStart_TextChanged);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(125, 122);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(93, 18);
            this.label4.Text = "End:";
            // 
            // txtEnd
            // 
            this.txtEnd.Location = new System.Drawing.Point(125, 143);
            this.txtEnd.Name = "txtEnd";
            this.txtEnd.Size = new System.Drawing.Size(109, 21);
            this.txtEnd.TabIndex = 1;
            this.txtEnd.TextChanged += new System.EventHandler(this.txtEnd_TextChanged);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker1.Location = new System.Drawing.Point(72, 190);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(88, 22);
            this.dateTimePicker1.TabIndex = 6;
            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.CustomFormat = "hh:mm:ss";
            this.dateTimePicker2.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker2.Location = new System.Drawing.Point(72, 218);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.ShowUpDown = true;
            this.dateTimePicker2.Size = new System.Drawing.Size(88, 22);
            this.dateTimePicker2.TabIndex = 6;
            this.dateTimePicker2.ValueChanged += new System.EventHandler(this.dateTimePicker2_ValueChanged);
            // 
            // btnSetStart
            // 
            this.btnSetStart.Location = new System.Drawing.Point(72, 167);
            this.btnSetStart.Name = "btnSetStart";
            this.btnSetStart.Size = new System.Drawing.Size(19, 17);
            this.btnSetStart.TabIndex = 7;
            this.btnSetStart.Text = "<";
            this.btnSetStart.Click += new System.EventHandler(this.btnSetStart_Click);
            // 
            // btnSetEnd
            // 
            this.btnSetEnd.Location = new System.Drawing.Point(141, 167);
            this.btnSetEnd.Name = "btnSetEnd";
            this.btnSetEnd.Size = new System.Drawing.Size(19, 17);
            this.btnSetEnd.TabIndex = 7;
            this.btnSetEnd.Text = ">";
            this.btnSetEnd.Click += new System.EventHandler(this.btnSetEnd_Click);
            // 
            // AddNotification
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.ControlBox = false;
            this.Controls.Add(this.btnSetEnd);
            this.Controls.Add(this.btnSetStart);
            this.Controls.Add(this.dateTimePicker2);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.btnFile);
            this.Controls.Add(this.txtEnd);
            this.Controls.Add(this.txtStart);
            this.Controls.Add(this.txtArg);
            this.Controls.Add(this.txtApp);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Menu = this.mainMenu1;
            this.Name = "AddNotification";
            this.Text = "AddNotification";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtApp;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtArg;
        private System.Windows.Forms.Button btnFile;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtStart;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtEnd;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.Button btnSetStart;
        private System.Windows.Forms.Button btnSetEnd;
    }
}