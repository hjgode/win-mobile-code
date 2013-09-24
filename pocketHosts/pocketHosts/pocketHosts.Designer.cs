namespace pocketHosts
{
    partial class pocketHosts
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
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtHost = new System.Windows.Forms.TextBox();
            this.listIP = new System.Windows.Forms.ComboBox();
            this.listAliases = new System.Windows.Forms.ComboBox();
            this.txtExpires = new System.Windows.Forms.TextBox();
            this.btnIPadd = new System.Windows.Forms.Button();
            this.btnAliasAdd = new System.Windows.Forms.Button();
            this.mnuSaveChanges = new System.Windows.Forms.MenuItem();
            this.listHosts = new System.Windows.Forms.ListBox();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuItem1);
            this.mainMenu1.MenuItems.Add(this.mnuSaveChanges);
            // 
            // menuItem1
            // 
            this.menuItem1.MenuItems.Add(this.menuItem2);
            this.menuItem1.Text = "File";
            // 
            // menuItem2
            // 
            this.menuItem2.Text = "Save All";
            this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 15);
            this.label1.Text = "Hosts:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(4, 121);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 19);
            this.label2.Text = "host:";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(4, 147);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 19);
            this.label3.Text = "ip:";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(4, 175);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 19);
            this.label4.Text = "aliases:";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(4, 205);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 19);
            this.label5.Text = "expires:";
            // 
            // txtHost
            // 
            this.txtHost.Location = new System.Drawing.Point(69, 121);
            this.txtHost.Name = "txtHost";
            this.txtHost.Size = new System.Drawing.Size(168, 21);
            this.txtHost.TabIndex = 7;
            // 
            // listIP
            // 
            this.listIP.Location = new System.Drawing.Point(69, 147);
            this.listIP.Name = "listIP";
            this.listIP.Size = new System.Drawing.Size(134, 22);
            this.listIP.TabIndex = 8;
            // 
            // listAliases
            // 
            this.listAliases.Location = new System.Drawing.Point(69, 175);
            this.listAliases.Name = "listAliases";
            this.listAliases.Size = new System.Drawing.Size(134, 22);
            this.listAliases.TabIndex = 8;
            // 
            // txtExpires
            // 
            this.txtExpires.Location = new System.Drawing.Point(69, 203);
            this.txtExpires.Name = "txtExpires";
            this.txtExpires.Size = new System.Drawing.Size(134, 21);
            this.txtExpires.TabIndex = 9;
            // 
            // btnIPadd
            // 
            this.btnIPadd.Location = new System.Drawing.Point(209, 150);
            this.btnIPadd.Name = "btnIPadd";
            this.btnIPadd.Size = new System.Drawing.Size(26, 18);
            this.btnIPadd.TabIndex = 10;
            this.btnIPadd.Text = "...";
            this.btnIPadd.Click += new System.EventHandler(this.btnIPadd_Click);
            // 
            // btnAliasAdd
            // 
            this.btnAliasAdd.Location = new System.Drawing.Point(209, 176);
            this.btnAliasAdd.Name = "btnAliasAdd";
            this.btnAliasAdd.Size = new System.Drawing.Size(26, 18);
            this.btnAliasAdd.TabIndex = 10;
            this.btnAliasAdd.Text = "...";
            this.btnAliasAdd.Click += new System.EventHandler(this.btnAliasAdd_Click);
            // 
            // mnuSaveChanges
            // 
            this.mnuSaveChanges.Text = "SAVE change";
            this.mnuSaveChanges.Click += new System.EventHandler(this.mnuSaveChanges_Click);
            // 
            // listHosts
            // 
            this.listHosts.Location = new System.Drawing.Point(69, 7);
            this.listHosts.Name = "listHosts";
            this.listHosts.Size = new System.Drawing.Size(166, 72);
            this.listHosts.TabIndex = 16;
            this.listHosts.SelectedIndexChanged += new System.EventHandler(this.listHosts_SelectedIndexChanged);
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(159, 85);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(76, 22);
            this.btnNew.TabIndex = 17;
            this.btnNew.Text = "NEW entry";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(69, 85);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(72, 22);
            this.btnRemove.TabIndex = 18;
            this.btnRemove.Text = "remove";
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // pocketHosts
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnNew);
            this.Controls.Add(this.listHosts);
            this.Controls.Add(this.btnAliasAdd);
            this.Controls.Add(this.btnIPadd);
            this.Controls.Add(this.txtExpires);
            this.Controls.Add(this.listAliases);
            this.Controls.Add(this.listIP);
            this.Controls.Add(this.txtHost);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Menu = this.mainMenu1;
            this.MinimizeBox = false;
            this.Name = "pocketHosts";
            this.Text = "pocketHosts";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Form1_Closing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtHost;
        private System.Windows.Forms.ComboBox listIP;
        private System.Windows.Forms.ComboBox listAliases;
        private System.Windows.Forms.TextBox txtExpires;
        private System.Windows.Forms.Button btnIPadd;
        private System.Windows.Forms.Button btnAliasAdd;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.MenuItem mnuSaveChanges;
        private System.Windows.Forms.ListBox listHosts;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnRemove;
    }
}

