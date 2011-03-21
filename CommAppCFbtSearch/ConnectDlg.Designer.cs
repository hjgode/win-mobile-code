namespace CommAppCFbtSearch
{
    partial class ConnectDlg
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
            this.mnuConnect = new System.Windows.Forms.MenuItem();
            this.lblComPort = new System.Windows.Forms.Label();
            this.cmbPortName = new System.Windows.Forms.ComboBox();
            this.cmbBaudRate = new System.Windows.Forms.ComboBox();
            this.lblBaudRate = new System.Windows.Forms.Label();
            this.cmbParity = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblStopBits = new System.Windows.Forms.Label();
            this.cmbStopBits = new System.Windows.Forms.ComboBox();
            this.lblDataBits = new System.Windows.Forms.Label();
            this.cmbDataBits = new System.Windows.Forms.ComboBox();
            this.cmbHandshake = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnBTPrinter = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.mnuCancel);
            this.mainMenu1.MenuItems.Add(this.mnuConnect);
            // 
            // mnuCancel
            // 
            this.mnuCancel.Text = "Cancel";
            this.mnuCancel.Click += new System.EventHandler(this.mnuCancel_Click);
            // 
            // mnuConnect
            // 
            this.mnuConnect.Text = "Connect";
            this.mnuConnect.Click += new System.EventHandler(this.mnuConnect_Click);
            // 
            // lblComPort
            // 
            this.lblComPort.Location = new System.Drawing.Point(27, 11);
            this.lblComPort.Name = "lblComPort";
            this.lblComPort.Size = new System.Drawing.Size(74, 13);
            this.lblComPort.Text = "COM Port:";
            // 
            // cmbPortName
            // 
            this.cmbPortName.Items.Add("COM0");
            this.cmbPortName.Items.Add("COM1");
            this.cmbPortName.Items.Add("COM2");
            this.cmbPortName.Items.Add("COM3");
            this.cmbPortName.Items.Add("COM4");
            this.cmbPortName.Location = new System.Drawing.Point(112, 3);
            this.cmbPortName.Name = "cmbPortName";
            this.cmbPortName.Size = new System.Drawing.Size(68, 22);
            this.cmbPortName.TabIndex = 18;
            // 
            // cmbBaudRate
            // 
            this.cmbBaudRate.Items.Add("1200");
            this.cmbBaudRate.Items.Add("2400");
            this.cmbBaudRate.Items.Add("4800");
            this.cmbBaudRate.Items.Add("");
            this.cmbBaudRate.Location = new System.Drawing.Point(112, 39);
            this.cmbBaudRate.Name = "cmbBaudRate";
            this.cmbBaudRate.Size = new System.Drawing.Size(68, 22);
            this.cmbBaudRate.TabIndex = 19;
            // 
            // lblBaudRate
            // 
            this.lblBaudRate.Location = new System.Drawing.Point(27, 47);
            this.lblBaudRate.Name = "lblBaudRate";
            this.lblBaudRate.Size = new System.Drawing.Size(79, 13);
            this.lblBaudRate.Text = "Baud Rate:";
            // 
            // cmbParity
            // 
            this.cmbParity.Items.Add("None");
            this.cmbParity.Items.Add("1");
            this.cmbParity.Items.Add("2");
            this.cmbParity.Items.Add("3");
            this.cmbParity.Items.Add("4");
            this.cmbParity.Items.Add("5");
            this.cmbParity.Items.Add("6");
            this.cmbParity.Location = new System.Drawing.Point(112, 79);
            this.cmbParity.Name = "cmbParity";
            this.cmbParity.Size = new System.Drawing.Size(68, 22);
            this.cmbParity.TabIndex = 21;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(27, 79);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.Text = "Parity:";
            // 
            // lblStopBits
            // 
            this.lblStopBits.Location = new System.Drawing.Point(27, 158);
            this.lblStopBits.Name = "lblStopBits";
            this.lblStopBits.Size = new System.Drawing.Size(70, 13);
            this.lblStopBits.Text = "Stop Bits:";
            // 
            // cmbStopBits
            // 
            this.cmbStopBits.Items.Add("1");
            this.cmbStopBits.Items.Add("2");
            this.cmbStopBits.Items.Add("3");
            this.cmbStopBits.Location = new System.Drawing.Point(112, 155);
            this.cmbStopBits.Name = "cmbStopBits";
            this.cmbStopBits.Size = new System.Drawing.Size(69, 22);
            this.cmbStopBits.TabIndex = 26;
            // 
            // lblDataBits
            // 
            this.lblDataBits.Location = new System.Drawing.Point(27, 119);
            this.lblDataBits.Name = "lblDataBits";
            this.lblDataBits.Size = new System.Drawing.Size(71, 13);
            this.lblDataBits.Text = "Data Bits:";
            // 
            // cmbDataBits
            // 
            this.cmbDataBits.Items.Add("5");
            this.cmbDataBits.Items.Add("6");
            this.cmbDataBits.Items.Add("7");
            this.cmbDataBits.Items.Add("8");
            this.cmbDataBits.Location = new System.Drawing.Point(112, 119);
            this.cmbDataBits.Name = "cmbDataBits";
            this.cmbDataBits.Size = new System.Drawing.Size(68, 22);
            this.cmbDataBits.TabIndex = 25;
            // 
            // cmbHandshake
            // 
            this.cmbHandshake.Items.Add("1");
            this.cmbHandshake.Items.Add("2");
            this.cmbHandshake.Items.Add("3");
            this.cmbHandshake.Location = new System.Drawing.Point(111, 192);
            this.cmbHandshake.Name = "cmbHandshake";
            this.cmbHandshake.Size = new System.Drawing.Size(69, 22);
            this.cmbHandshake.TabIndex = 26;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(26, 195);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.Text = "Handshake:";
            // 
            // btnBTPrinter
            // 
            this.btnBTPrinter.Location = new System.Drawing.Point(27, 230);
            this.btnBTPrinter.Name = "btnBTPrinter";
            this.btnBTPrinter.Size = new System.Drawing.Size(181, 22);
            this.btnBTPrinter.TabIndex = 31;
            this.btnBTPrinter.Text = "Bluetooth Printer";
            this.btnBTPrinter.Click += new System.EventHandler(this.btnBTPrinter_Click);
            // 
            // ConnectDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.ControlBox = false;
            this.Controls.Add(this.btnBTPrinter);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblStopBits);
            this.Controls.Add(this.cmbHandshake);
            this.Controls.Add(this.cmbStopBits);
            this.Controls.Add(this.lblDataBits);
            this.Controls.Add(this.cmbDataBits);
            this.Controls.Add(this.lblComPort);
            this.Controls.Add(this.cmbPortName);
            this.Controls.Add(this.cmbBaudRate);
            this.Controls.Add(this.lblBaudRate);
            this.Controls.Add(this.cmbParity);
            this.Controls.Add(this.label1);
            this.Menu = this.mainMenu1;
            this.MinimizeBox = false;
            this.Name = "ConnectDlg";
            this.Text = "ConnectDlg";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuItem mnuCancel;
        private System.Windows.Forms.MenuItem mnuConnect;
        private System.Windows.Forms.Label lblComPort;
        private System.Windows.Forms.ComboBox cmbPortName;
        private System.Windows.Forms.ComboBox cmbBaudRate;
        private System.Windows.Forms.Label lblBaudRate;
        private System.Windows.Forms.ComboBox cmbParity;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblStopBits;
        private System.Windows.Forms.ComboBox cmbStopBits;
        private System.Windows.Forms.Label lblDataBits;
        private System.Windows.Forms.ComboBox cmbDataBits;
        private System.Windows.Forms.ComboBox cmbHandshake;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnBTPrinter;
    }
}