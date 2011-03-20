using System.Net;
using System.IO;
using System.Net.Sockets;
using InTheHand.Net.Sockets;
using InTheHand.Net.Bluetooth;
using InTheHand.Net;
public class ChatForm : System.Windows.Forms.Form {
    
    internal System.Windows.Forms.MainMenu MainMenu1;
    
    const int MAX_MESSAGE_SIZE = 128;
    
    const int MAX_TRIES = 3;
    
    private Guid ServiceName = new Guid("{E075D486-E23D-4887-8AF5-DAA1F6A5B172}");
    
    private BluetoothClient btClient = new BluetoothClient();
    
    private BluetoothListener btListener;
    
    private bool listening = true;
    
    private string str;
    
    // NOTE: The following procedure is required by the Windows Form Designer
    // It can be modified using the Windows Form Designer.  
    // Do not modify it using the code editor.
    internal System.Windows.Forms.TextBox txtMessagesArchive;
    
    internal System.Windows.Forms.TextBox txtMessage;
    
    internal System.Windows.Forms.MenuItem mnuSend;
    
    internal System.Windows.Forms.MenuItem mnuMenu;
    
    internal System.Windows.Forms.MenuItem mnuSearch;
    
    internal System.Windows.Forms.MenuItem mnuExit;
    
    internal System.Windows.Forms.ComboBox cboDevices;
    
    internal System.Windows.Forms.Button btnSend;
    
    public ChatForm() {
        // This call is required by the Windows Form Designer.
        InitializeComponent();
        // Add any initialization after the InitializeComponent() call
    }
    
    // Form overrides dispose to clean up the component list.
    protected override void Dispose(bool disposing) {
        base.Dispose(disposing);
    }
    
    private void InitializeComponent() {
        this.MainMenu1 = new System.Windows.Forms.MainMenu();
        this.mnuSend = new System.Windows.Forms.MenuItem();
        this.mnuMenu = new System.Windows.Forms.MenuItem();
        this.mnuSearch = new System.Windows.Forms.MenuItem();
        this.mnuExit = new System.Windows.Forms.MenuItem();
        this.txtMessagesArchive = new System.Windows.Forms.TextBox();
        this.txtMessage = new System.Windows.Forms.TextBox();
        this.cboDevices = new System.Windows.Forms.ComboBox();
        this.btnSend = new System.Windows.Forms.Button();
        // 
        // MainMenu1
        // 
        this.MainMenu1.MenuItems.Add(this.mnuSend);
        this.MainMenu1.MenuItems.Add(this.mnuMenu);
        // 
        // mnuSend
        // 
        this.mnuSend.Text = "Send";
        this.mnuMenu.MenuItems.Add(this.mnuSearch);
        this.mnuMenu.MenuItems.Add(this.mnuExit);
        this.mnuMenu.Text = "Menu";
        this.mnuSearch.Text = "Search Again";
        this.mnuExit.Text = "Exit";
        this.txtMessagesArchive.Location = new System.Drawing.Point(8, 72);
        this.txtMessagesArchive.Multiline = true;
        this.txtMessagesArchive.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        this.txtMessagesArchive.Size = new System.Drawing.Size(224, 184);
        this.txtMessagesArchive.Text = "";
        this.txtMessage.Location = new System.Drawing.Point(8, 8);
        this.txtMessage.Size = new System.Drawing.Size(176, 20);
        this.txtMessage.Text = "";
        this.cboDevices.Location = new System.Drawing.Point(8, 40);
        this.cboDevices.Size = new System.Drawing.Size(224, 21);
        // 
        // btnSend
        // 
        this.btnSend.Location = new System.Drawing.Point(184, 8);
        this.btnSend.Size = new System.Drawing.Size(48, 24);
        this.btnSend.Text = "Send";
        this.Controls.Add(this.btnSend);
        this.Controls.Add(this.cboDevices);
        this.Controls.Add(this.txtMessage);
        this.Controls.Add(this.txtMessagesArchive);
        this.Menu = this.MainMenu1;
        this.MinimizeBox = false;
        this.Text = "Bluetooth Chat";
    }
    
    private void sendMessage(int NumRetries, byte[] Buffer, int BufferLen) {
        BluetoothClient client = null;
        int CurrentTries = 0;
        for (
        ; ((client == null) 
                    && (CurrentTries < NumRetries)); 
        ) {
            try {
                client = new BluetoothClient();
                client.Connect(new BluetoothEndPoint(((BluetoothDeviceInfo)(cboDevices.SelectedItem)).DeviceAddress, ServiceName));
            }
            catch (SocketException se) {
                if ((CurrentTries >= NumRetries)) {
                    throw se;
                }
                client = null;
            }
            CurrentTries = (CurrentTries + 1);
        }
        if ((client == null)) {
            // timeout occurred
            MsgBox("Error establishing contact");
            return;
        }
        System.IO.Stream stream = null;
        try {
            stream = client.GetStream();
            stream.Write(Buffer, 0, BufferLen);
        }
        catch (Exception e) {
            MsgBox("Error sending");
        }
        finally {
            if (!(stream == null)) {
                stream.Close();
            }
            if (!(client == null)) {
                client.Close();
            }
        }
    }
    
    private string receiveMessage(int BufferLen) {
        int bytesRead = 0;
        BluetoothClient client = null;
        System.IO.Stream stream = null;
        byte[,] Buffer;
        try {
            client = btListener.AcceptBluetoothClient();
            //  blocking call
            stream = client.GetStream();
            bytesRead = stream.Read(Buffer, 0, BufferLen);
            str = (client.RemoteMachineName + ("->" 
                        + (System.Text.Encoding.Unicode.GetString(Buffer, 0, bytesRead) + "\r\n")));
        }
        catch (Exception e) {
            // dont display error if we are ending the listener
            if (listening) {
                MsgBox("Error listening to incoming message");
            }
        }
        finally {
            if (!(stream == null)) {
                stream.Close();
            }
            if (!(client == null)) {
                client.Close();
            }
        }
        return str;
    }
    
    private void Form1_Load(object sender, System.EventArgs e) {
        // Dim s As New InTheHand.Windows.Forms.SelectBluetoothDeviceDialog()
        // s.ForceAuthentication = True
        // s.ShowAuthenticated = True
        // s.ShowRemembered = True
        // s.ShowUnknown = True
        // s.ShowDialog()
        System.Threading.Thread t1;
        t1 = new Threading.Thread(new System.EventHandler(this.receiveLoop));
        t1.Start();
        btClient = new BluetoothClient();
        BluetoothDeviceInfo[] bdi = btClient.DiscoverDevices();
        cboDevices.DataSource = bdi;
        cboDevices.DisplayMember = "DeviceName";
    }
    
    public void receiveLoop() {
        string strReceived;
        btListener = new BluetoothListener(ServiceName);
        btListener.Start();
        strReceived = receiveMessage(MAX_MESSAGE_SIZE);
        while (listening) {
            // ---keep on listening for new message
            if ((strReceived != "")) {
                this.Invoke(new EventHandler(// TODO: Warning!!!! NULL EXPRESSION DETECTED...
                    .));
                strReceived = receiveMessage(MAX_MESSAGE_SIZE);
            }
        }
    }
    
    private void UpdateTextBox(object sender, EventArgs e) {
        // ---delegate to update the textbox control
        txtMessagesArchive.Text = (txtMessagesArchive.Text + str);
    }
    
    private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
        // stop receive loop
        listening = false;
        btListener.Stop();
        Application.Exit();
    }
    
    private void mnuSend_Click(object sender, System.EventArgs e) {
        sendMessage(MAX_TRIES, System.Text.Encoding.Unicode.GetBytes(txtMessage.Text), (txtMessage.Text.Length * 2));
    }
    
    private void mnuExit_Click(object sender, System.EventArgs e) {
        this.Close();
    }
    
    private void mnuSearch_Click(object sender, System.EventArgs e) {
        Cursor.Current = Cursors.WaitCursor;
        BluetoothDeviceInfo[] bdi = btClient.DiscoverDevices();
        cboDevices.DataSource = bdi;
        Cursor.Current = Cursors.Default;
    }
    
    private void btnSend_Click(object sender, System.EventArgs e) {
        mnuSend_Click(sender, e);
    }
}

