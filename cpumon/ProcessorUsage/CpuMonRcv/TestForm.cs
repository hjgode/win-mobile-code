using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CpuMonRcv
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
        }

        private void TestForm_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'processUsageDataSet.ProcessView' table. You can move, or remove it, as needed.
            this.processViewTableAdapter.Fill(this.processUsageDataSet.ProcessView);

        }
    }
}
