using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

using System.Collections.Generic;

using System.Process;

using System.Threading;

class RecvBroadcst:IDisposable
{
    public bool bRunThread = true;

    // Thread signal.
    public static ManualResetEvent allDone = new ManualResetEvent(false);

    public RecvBroadcst()
    {
        //dataStream = new byte[1024];
        //serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        //myThread = new Thread(startReceive);
        //myThread.Start();
        StartReceive();
    }

    void listener_onUpdate(object sender, ProcessStatistics.process_statistics data)
    {
        updateStatus(data);
    }
    public void Dispose(){
        if (receiveSocket != null)
            receiveSocket.Close();
        bRunThread = false;
        //if (myThread != null)
        //    myThread.Abort();
    }

    //#######################################################################################

    Socket receiveSocket;
    byte[] recBuffer;
    EndPoint bindEndPoint;
    const int maxBuffer = 32768;

    public bool StopReceive()
    {
        bool bRet = false;
        try
        {
            //receiveSocket.Disconnect(false);
            receiveSocket.Close();
            receiveSocket = null;
            bRet = true;
        }
        catch(SocketException ex) {
            System.Diagnostics.Debug.WriteLine("StopReceive(): SocketException=" + ex.Message);
        }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine("StopReceive(): Exception=" + ex.Message); }
        return bRet;
    }

    public void StartReceive()
    {
        if (receiveSocket != null)
            StopReceive();
        receiveSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        bindEndPoint = new IPEndPoint(IPAddress.Any, 3001);
        recBuffer = new byte[maxBuffer];
        receiveSocket.Bind(bindEndPoint);
        receiveSocket.BeginReceiveFrom(recBuffer, 0, recBuffer.Length, SocketFlags.None, ref bindEndPoint, new AsyncCallback(MessageReceivedCallback), (object)this);
    }

    void MessageReceivedCallback(IAsyncResult result)
    {
        EndPoint remoteEndPoint = new IPEndPoint(0, 0);

        //IPEndPoint LocalIPEndPoint = new IPEndPoint(IPAddress.Any, 3001);
        //EndPoint LocalEndPoint = (EndPoint)LocalIPEndPoint;
        //IPEndPoint remoteEP = (IPEndPoint)LocalEndPoint;
        //System.Diagnostics.Debug.WriteLine("Remote IP: " + remoteEP.Address.ToString());

        try
        {
            //all data should fit in one package!
            int bytesRead = receiveSocket.EndReceiveFrom(result, ref remoteEndPoint);
            //System.Diagnostics.Debug.WriteLine("Remote IP: " + ((IPEndPoint)(remoteEndPoint)).Address.ToString());

            byte[] bData = new byte[bytesRead];
            Array.Copy(recBuffer, bData, bytesRead);
            if (ByteHelper.isEndOfTransfer(bData))
                updateEndOfTransfer();// end of transfer
            else
            {
                ProcessStatistics.process_statistics stats = new ProcessStatistics.process_statistics(bData);
                stats.remoteIP = ((IPEndPoint)(remoteEndPoint)).Address.ToString();
                //System.Diagnostics.Debug.WriteLine( stats.dumpStatistics() );
                updateStatus(stats);
            }
        }
        catch (SocketException e)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("MessageReceivedCallback SocketException: {0} {1}", e.ErrorCode, e.Message));
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("MessageReceivedCallback Exception: {0}", e.Message));
        }
        try
        {
            //ready to receive next packet
            receiveSocket.BeginReceiveFrom(recBuffer, 0, recBuffer.Length, SocketFlags.None, ref bindEndPoint, new AsyncCallback(MessageReceivedCallback), (object)this);
        }
        catch (Exception) { }
    }

    public delegate void delegateUpdate(object sender, ProcessStatistics.process_statistics data);
    public event delegateUpdate onUpdate;

    private void updateStatus(ProcessStatistics.process_statistics data)
    {
        //System.Diagnostics.Debug.WriteLine("updateStatus: " + data.dumpStatistics());
        if (this.onUpdate != null)
            this.onUpdate(this, data);
    }

    public delegate void delegateEndOfTransfer(object sender, EventArgs e);
    public event delegateEndOfTransfer onEndOfTransfer;
    private void updateEndOfTransfer()
    {
        if (this.onEndOfTransfer != null)
            this.onEndOfTransfer(this, new EventArgs());
    }
    //#######################################################################################
   
}