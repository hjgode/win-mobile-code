namespace CpuMonRcv
{
    partial class DetailView
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
            System.Windows.Forms.Label label2;
            this.masterDataGridView = new System.Windows.Forms.DataGridView();
            this.detailsDataGridView = new System.Windows.Forms.DataGridView();
            this.PushGraph1 = new CustomUIControls.Graphing.C2DPushGraph();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.masterDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.detailsDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 24);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(75, 13);
            label1.TabIndex = 2;
            label1.Text = "PROCESSES:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(12, 192);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(62, 13);
            label2.TabIndex = 2;
            label2.Text = "THREADS:";
            // 
            // masterDataGridView
            // 
            this.masterDataGridView.AllowUserToAddRows = false;
            this.masterDataGridView.AllowUserToDeleteRows = false;
            this.masterDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.masterDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.masterDataGridView.Location = new System.Drawing.Point(128, 24);
            this.masterDataGridView.MultiSelect = false;
            this.masterDataGridView.Name = "masterDataGridView";
            this.masterDataGridView.ReadOnly = true;
            this.masterDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.masterDataGridView.Size = new System.Drawing.Size(592, 158);
            this.masterDataGridView.TabIndex = 0;
            this.masterDataGridView.VirtualMode = true;
            this.masterDataGridView.Enter += new System.EventHandler(this.masterDataGridView_Enter);
            this.masterDataGridView.Validated += new System.EventHandler(this.masterDataGridView_Validated);
            this.masterDataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.masterDataGridView_RowEnter);
            // 
            // detailsDataGridView
            // 
            this.detailsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.detailsDataGridView.Location = new System.Drawing.Point(128, 192);
            this.detailsDataGridView.Name = "detailsDataGridView";
            this.detailsDataGridView.ReadOnly = true;
            this.detailsDataGridView.Size = new System.Drawing.Size(419, 149);
            this.detailsDataGridView.TabIndex = 1;
            // 
            // PushGraph1
            // 
            this.PushGraph1.AutoAdjustPeek = false;
            this.PushGraph1.BackColor = System.Drawing.Color.Black;
            this.PushGraph1.GridColor = System.Drawing.Color.Green;
            this.PushGraph1.GridSize = ((ushort)(15));
            this.PushGraph1.HighQuality = true;
            this.PushGraph1.LineInterval = ((ushort)(5));
            this.PushGraph1.Location = new System.Drawing.Point(129, 368);
            this.PushGraph1.MaxLabel = "Max";
            this.PushGraph1.MaxPeekMagnitude = 100;
            this.PushGraph1.MinLabel = "Minimum";
            this.PushGraph1.MinPeekMagnitude = 0;
            this.PushGraph1.Name = "PushGraph1";
            this.PushGraph1.ShowGrid = true;
            this.PushGraph1.ShowLabels = true;
            this.PushGraph1.Size = new System.Drawing.Size(417, 144);
            this.PushGraph1.TabIndex = 3;
            this.PushGraph1.Text = "c2DPushGraph1";
            this.PushGraph1.TextColor = System.Drawing.Color.Yellow;
            // 
            // DetailView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(775, 552);
            this.Controls.Add(this.PushGraph1);
            this.Controls.Add(label2);
            this.Controls.Add(label1);
            this.Controls.Add(this.detailsDataGridView);
            this.Controls.Add(this.masterDataGridView);
            this.Name = "DetailView";
            this.Text = "DetailView";
            ((System.ComponentModel.ISupportInitialize)(this.masterDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.detailsDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView masterDataGridView;
        private System.Windows.Forms.DataGridView detailsDataGridView;
        private CustomUIControls.Graphing.C2DPushGraph PushGraph1;
    }
}