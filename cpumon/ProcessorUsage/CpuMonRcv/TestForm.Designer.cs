namespace CpuMonRcv
{
    partial class TestForm
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.processUsageDataSet = new CpuMonRcv.ProcessUsageDataSet();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.processViewBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.processViewTableAdapter = new CpuMonRcv.ProcessUsageDataSetTableAdapters.ProcessViewTableAdapter();
            this.procIDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.processDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.durationDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.processViewBindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.usageDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.theTimeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.processUsageDataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.processViewBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.processViewBindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // bindingSource1
            // 
            this.bindingSource1.DataSource = this.processUsageDataSet;
            this.bindingSource1.Position = 0;
            // 
            // processUsageDataSet
            // 
            this.processUsageDataSet.DataSetName = "ProcessUsageDataSet";
            this.processUsageDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.procIDDataGridViewTextBoxColumn,
            this.processDataGridViewTextBoxColumn,
            this.durationDataGridViewTextBoxColumn,
            this.usageDataGridViewTextBoxColumn,
            this.theTimeDataGridViewTextBoxColumn});
            this.dataGridView1.DataSource = this.processViewBindingSource;
            this.dataGridView1.Location = new System.Drawing.Point(4, 22);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(477, 156);
            this.dataGridView1.TabIndex = 0;
            // 
            // processViewBindingSource
            // 
            this.processViewBindingSource.DataMember = "ProcessView";
            this.processViewBindingSource.DataSource = this.bindingSource1;
            // 
            // processViewTableAdapter
            // 
            this.processViewTableAdapter.ClearBeforeFill = true;
            // 
            // procIDDataGridViewTextBoxColumn
            // 
            this.procIDDataGridViewTextBoxColumn.DataPropertyName = "[ProcID]";
            this.procIDDataGridViewTextBoxColumn.HeaderText = "[ProcID]";
            this.procIDDataGridViewTextBoxColumn.Name = "procIDDataGridViewTextBoxColumn";
            // 
            // processDataGridViewTextBoxColumn
            // 
            this.processDataGridViewTextBoxColumn.DataPropertyName = "[Process]";
            this.processDataGridViewTextBoxColumn.HeaderText = "[Process]";
            this.processDataGridViewTextBoxColumn.Name = "processDataGridViewTextBoxColumn";
            // 
            // durationDataGridViewTextBoxColumn
            // 
            this.durationDataGridViewTextBoxColumn.DataPropertyName = "[Duration]";
            this.durationDataGridViewTextBoxColumn.HeaderText = "[Duration]";
            this.durationDataGridViewTextBoxColumn.Name = "durationDataGridViewTextBoxColumn";
            // 
            // chart1
            // 
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            this.chart1.DataSource = this.processViewBindingSource1;
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(27, 232);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            series1.XValueMember = "[theTime]";
            series1.YValueMembers = "[Usage]";
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(423, 121);
            this.chart1.TabIndex = 1;
            this.chart1.Text = "chart1";
            // 
            // processViewBindingSource1
            // 
            this.processViewBindingSource1.DataMember = "ProcessView";
            this.processViewBindingSource1.DataSource = this.bindingSource1;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "[Usage]";
            this.dataGridViewTextBoxColumn1.HeaderText = "[Usage]";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.DataPropertyName = "[theTime]";
            this.dataGridViewTextBoxColumn2.HeaderText = "[theTime]";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            // 
            // usageDataGridViewTextBoxColumn
            // 
            this.usageDataGridViewTextBoxColumn.DataPropertyName = "[Usage]";
            this.usageDataGridViewTextBoxColumn.HeaderText = "[Usage]";
            this.usageDataGridViewTextBoxColumn.Name = "usageDataGridViewTextBoxColumn";
            // 
            // theTimeDataGridViewTextBoxColumn
            // 
            this.theTimeDataGridViewTextBoxColumn.DataPropertyName = "[theTime]";
            this.theTimeDataGridViewTextBoxColumn.HeaderText = "[theTime]";
            this.theTimeDataGridViewTextBoxColumn.Name = "theTimeDataGridViewTextBoxColumn";
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(493, 383);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.dataGridView1);
            this.Name = "TestForm";
            this.Text = "TestForm";
            this.Load += new System.EventHandler(this.TestForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.processUsageDataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.processViewBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.processViewBindingSource1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.BindingSource bindingSource1;
        private ProcessUsageDataSet processUsageDataSet;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.BindingSource processViewBindingSource;
        private CpuMonRcv.ProcessUsageDataSetTableAdapters.ProcessViewTableAdapter processViewTableAdapter;
        private System.Windows.Forms.DataGridViewTextBoxColumn procIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn processDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn usageDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn durationDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn theTimeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.BindingSource processViewBindingSource1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
    }
}