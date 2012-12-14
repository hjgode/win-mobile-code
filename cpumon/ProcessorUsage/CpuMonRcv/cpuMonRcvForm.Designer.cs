namespace CpuMonRcv
{
    partial class cpuMonRcvForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (recvr != null)
            {
                recvr.bRunThread = false;
                recvr.Dispose();
                recvr = null;
            }
            dataAccess.Dispose();

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
            this.txtLog = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuView = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewDetails = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTestForm = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.c2DPushGraph1 = new CustomUIControls.Graphing.C2DPushGraph();
            this.mnuUsage = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // txtLog
            // 
            this.txtLog.AcceptsReturn = true;
            this.txtLog.AcceptsTab = true;
            this.txtLog.Location = new System.Drawing.Point(118, 68);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLog.Size = new System.Drawing.Size(357, 146);
            this.txtLog.TabIndex = 0;
            this.txtLog.WordWrap = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFile,
            this.mnuView});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(487, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menuFile
            // 
            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuExit,
            this.mnuUsage});
            this.menuFile.Name = "menuFile";
            this.menuFile.Size = new System.Drawing.Size(37, 20);
            this.menuFile.Text = "File";
            // 
            // menuExit
            // 
            this.menuExit.Name = "menuExit";
            this.menuExit.Size = new System.Drawing.Size(152, 22);
            this.menuExit.Text = "Exit";
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // mnuView
            // 
            this.mnuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuViewDetails,
            this.mnuTestForm});
            this.mnuView.Name = "mnuView";
            this.mnuView.Size = new System.Drawing.Size(44, 20);
            this.mnuView.Text = "View";
            // 
            // mnuViewDetails
            // 
            this.mnuViewDetails.Name = "mnuViewDetails";
            this.mnuViewDetails.Size = new System.Drawing.Size(124, 22);
            this.mnuViewDetails.Text = "Details";
            this.mnuViewDetails.Click += new System.EventHandler(this.mnuViewDetails_Click);
            // 
            // mnuTestForm
            // 
            this.mnuTestForm.Name = "mnuTestForm";
            this.mnuTestForm.Size = new System.Drawing.Size(124, 22);
            this.mnuTestForm.Text = "TestForm";
            this.mnuTestForm.Click += new System.EventHandler(this.mnuTestForm_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 412);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(487, 22);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dataGridView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.c2DPushGraph1);
            this.splitContainer1.Size = new System.Drawing.Size(487, 388);
            this.splitContainer1.SplitterDistance = 288;
            this.splitContainer1.TabIndex = 6;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(487, 288);
            this.dataGridView1.TabIndex = 2;
            // 
            // c2DPushGraph1
            // 
            this.c2DPushGraph1.AutoAdjustPeek = false;
            this.c2DPushGraph1.BackColor = System.Drawing.Color.Black;
            this.c2DPushGraph1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.c2DPushGraph1.GridColor = System.Drawing.Color.Green;
            this.c2DPushGraph1.GridSize = ((ushort)(15));
            this.c2DPushGraph1.HighQuality = true;
            this.c2DPushGraph1.LineInterval = ((ushort)(5));
            this.c2DPushGraph1.Location = new System.Drawing.Point(0, 0);
            this.c2DPushGraph1.MaxLabel = "Max";
            this.c2DPushGraph1.MaxPeekMagnitude = 100;
            this.c2DPushGraph1.MinLabel = "Minimum";
            this.c2DPushGraph1.MinPeekMagnitude = 0;
            this.c2DPushGraph1.Name = "c2DPushGraph1";
            this.c2DPushGraph1.ShowGrid = true;
            this.c2DPushGraph1.ShowLabels = true;
            this.c2DPushGraph1.Size = new System.Drawing.Size(487, 96);
            this.c2DPushGraph1.TabIndex = 4;
            this.c2DPushGraph1.Text = "c2DPushGraph1";
            this.c2DPushGraph1.TextColor = System.Drawing.Color.Yellow;
            // 
            // mnuUsage
            // 
            this.mnuUsage.Name = "mnuUsage";
            this.mnuUsage.Size = new System.Drawing.Size(152, 22);
            this.mnuUsage.Text = "Export Usage";
            this.mnuUsage.Click += new System.EventHandler(this.mnuUsage_Click);
            // 
            // cpuMonRcvForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(487, 434);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "cpuMonRcvForm";
            this.Text = "Waiting for connect ...";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuFile;
        private System.Windows.Forms.ToolStripMenuItem menuExit;
        private System.Windows.Forms.ToolStripMenuItem mnuView;
        private System.Windows.Forms.ToolStripMenuItem mnuViewDetails;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private CustomUIControls.Graphing.C2DPushGraph c2DPushGraph1;
        private System.Windows.Forms.ToolStripMenuItem mnuTestForm;
        private System.Windows.Forms.ToolStripMenuItem mnuUsage;
    }
}

