using System;

using System.Collections.Generic;
using System.Text;
using OpenNETCF.Net.NetworkInformation;
using System.Threading;
using System.Net;

namespace PingNG
{
    class myPing:IDisposable
    {
        string _ipAddress="127.0.0.1";
        PingOptions _pingOptions = new PingOptions();        

        public void doPing(string ipAddress)
        {
            _ipAddress = ipAddress;
            _pingOptions = new PingOptions();
            _ipAddress = ipAddress;
            //Thread thread = new Thread(new ThreadStart(startPing));
            ThreadStart myThread = delegate { startPing(ipAddress, new PingOptions()); };
            thread = new Thread(myThread);
            thread.Start();
        }

        Thread thread;
        public void doPing(string ipAddress, PingOptions pingOptions)
        {
            string _ipAddress = ipAddress;
            PingOptions _pingOptions = pingOptions;

            ThreadStart myThread = delegate { startPing(_ipAddress, _pingOptions); };
            thread = new Thread(myThread);
            thread.Name="ping thread";
            thread.Start();
        }

        public void Dispose()
        {
            if (thread != null)
            {
                thread.Abort();
                thread = null;
            }
        }

        class HostParms
        {
            public string host;
            public int retry;
            public IPAddress ipAddress=null;
            public bool isValid = false;
            public HostParms(string sHost, int iRetry)
            {
                this.host = sHost;
                this.retry = iRetry;
            }
        }

        bool checkHost(ref HostParms hostParms)
        {
            bool bRet = false;
            IPHostEntry ipHost = new IPHostEntry();
            try
            {
                IPHostEntry entry = null;
                int maxSeconds = hostParms.retry/* 10 */, counter = 0;
                // see http://jianmingli.com/wp/?p=22
                // Start the asynchronous request for DNS information. 
                // This example does not use a delegate or user-supplied object
                // so the last two arguments are null.
                IAsyncResult result = Dns.BeginGetHostEntry(hostParms.host/* ipAddress */, null, null);
                // Poll for completion information.
                while (result.IsCompleted != true && counter < maxSeconds)
                {
                    Thread.Sleep(1000);
                    counter++;
                }
                if (result.IsCompleted) //when we got here, the result is ready
                    entry = Dns.EndGetHostEntry(result);    //blocks?
                else
                    hostParms.isValid = false; // Thread.CurrentThread.Abort();

                //IPHostEntry entry = Dns.Resolve(hostNameOrAddress);
                //IPHostEntry entry = Dns.GetHostEntry(ipAddress);
                hostParms.ipAddress = entry.AddressList[0];
                hostParms.isValid = true;
            }
            catch (ThreadAbortException e)
            {
                Thread.CurrentThread.Abort();// ("DNS failed within timeout", e);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(string.Format( "Exception in checkHost({0}): {1}", hostParms.host, e.Message));
                //throw new PingUnknownHostException("Impossible to resolve host or address.", e);
            }

            return hostParms.isValid;
        }
        //thread to do ping
        public void startPing(string sHost, PingOptions pOptions)
        {
            System.Diagnostics.Debug.WriteLine("+++thread started");
            string ipAddress = sHost;//make a local copy
            Ping myPing = new Ping();
            PingOptions myPingOptions = pOptions;//make a local copy
            
            int iTimeout = pOptions.TimeOut;
            int _numberOfPings = pOptions.numOfPings;
            bool doNotFragment = pOptions.DontFragment;
            int bufferSize = pOptions.bufferSize;
            byte[] buf = new byte[bufferSize];

            //long sumRoundtripTime=0;
            onReply(new PingReplyEventArgs("ping started...",PingReplyTypes.info));
            replyStats _replyStats = new replyStats(ipAddress);
            PingReply reply = null;
            try
            {
                onReply(new PingReplyEventArgs(pOptions.ToString(), PingReplyTypes.info));
                //check DNS first as this may block for long time
                IPAddress address;
                HostParms hostParms = new HostParms(ipAddress, 4);
                try
                {
                    //is IP address
                    address = IPAddress.Parse(ipAddress);
                    hostParms.isValid = true;
                }
                catch
                {
                    if (checkHost(ref hostParms))
                        ipAddress = hostParms.ipAddress.ToString();
                }
                if (!hostParms.isValid)
                    throw new PingUnknownHostException("Unkown host: " + ipAddress);

                for (int ix = 0; ix < _numberOfPings; ix++)
                {
                    reply = myPing.Send(ipAddress, buf, iTimeout, myPingOptions);
                    string sReply = "";
                    if (reply.Status == IPStatus.Success)
                    {
                        //sumRoundtripTime += reply.RoundTripTime;
                        _replyStats.add(1, 1, reply.RoundTripTime);
                        sReply = myResources.getReply(reply, ipAddress);
                    }
                    else if (reply.Status == IPStatus.DestinationHostUnreachable)
                    {
                        _replyStats.add(1, 0, reply.RoundTripTime);
                        throw new PingUnknownHostException("Destination unreachable");
                    }
                    else
                    {                        
                        _replyStats.add(1, 0, reply.RoundTripTime);
                        sReply = myResources.getReply(reply, ipAddress);
                    }
                    System.Diagnostics.Debug.WriteLine(sReply);
                    onReply(new PingReplyEventArgs(sReply));
                }
                onReply(new PingReplyEventArgs(_replyStats.ToString(), PingReplyTypes.info));
            }
            catch (PingUnknownHostException ex)
            {
                System.Diagnostics.Debug.WriteLine("PingUnknownHostException: " + ex.Message);
                onReply(new PingReplyEventArgs("Unknown host: "+ ipAddress, PingReplyTypes.info));
            }
            catch (PingException ex)
            {
                System.Diagnostics.Debug.WriteLine("PingException: " + ex.Message);
            }
            catch (ThreadAbortException ex)
            {
                onReply(new PingReplyEventArgs("ping failed", PingReplyTypes.info));
                Thread.CurrentThread.Abort();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("PingException: " + ex.Message);
            }
            finally
            {
                onReply(new PingReplyEventArgs("done", PingReplyTypes.done));
                System.Diagnostics.Debug.WriteLine("---thread ended");
            }
        }

        class PingUnknownHostException : Exception
        {
            public PingUnknownHostException(string message)
                : base(message)
            {
            }
            public PingUnknownHostException(string message, Exception innerException)
                : base(message, innerException)
            {
            }
        }

        public delegate void ReplyEventHandler(object sender, PingReplyEventArgs args);
        public static event ReplyEventHandler onReplyEvent;
        static void onReply(PingReplyEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("onReply: " + args.message);
            ReplyEventHandler handler = onReplyEvent;
            if (handler != null)
            {
                handler("pingReply", args);
            }
        }
        public class PingReplyEventArgs : EventArgs
        {
            public string message;
            public PingReplyTypes replytype;
            public PingReplyEventArgs(string msg)
            {
                message = msg;
                replytype = PingReplyTypes.info;
            }
            public PingReplyEventArgs(string msg, PingReplyTypes rtype)
            {
                message = msg;
                replytype = rtype;
            }
        }
        public enum PingReplyTypes
        {
            info,
            reply,
            done
        }
    }
}
