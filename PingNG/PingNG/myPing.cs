using System;

using System.Collections.Generic;
using System.Text;
using OpenNETCF.Net.NetworkInformation;
using System.Threading;
using System.Net;

namespace PingNG
{
    class myPing
    {
        static string _ipAddress="127.0.0.1";
        static PingOptions _pingOptions = new PingOptions();
        static int _numberOfPings = 4;

        public void doPing(string ipAddress)
        {
            _ipAddress = ipAddress;
            _pingOptions = new PingOptions();
            _ipAddress = ipAddress;
            Thread thread = new Thread(new ThreadStart(startPing));
            thread.Start();
        }

        Thread thread;
        public void doPing(string ipAddress, PingOptions pingOptions)
        {
            _ipAddress = ipAddress;
            _pingOptions = pingOptions;
            _ipAddress = ipAddress;
            
            thread = new Thread(new ThreadStart(startPing));
            thread.Name="ping thread";
            thread.Start();
        }


        //thread to do ping
        static void startPing()
        {
            System.Diagnostics.Debug.WriteLine("+++thread started");
            string ipAddress = _ipAddress;
            Ping myPing = new Ping();
            PingOptions myPingOptions = _pingOptions;
            byte[] buf = new byte[20];
            int iTimeout = 10000;
            long sumRoundtripTime=0;
            onReply(new PingReplyEventArgs("ping started...",PingReplyTypes.info));
            try
            {
                //check DNS first as this may block for long time
                IPAddress address;
                try
                {
                    //is IP address
                    address = IPAddress.Parse(ipAddress);
                }
                catch
                {
                    //a host name
                    try
                    {
                        IPHostEntry entry = null;
                        int maxSeconds = 10, counter = 0;
                        // Start the asynchronous request for DNS information. 
                        // This example does not use a delegate or user-supplied object
                        // so the last two arguments are null.
                        IAsyncResult result = Dns.BeginGetHostEntry(ipAddress, null, null);
                        // Poll for completion information.
                        while (result.IsCompleted != true && counter < maxSeconds)
                        {
                            Thread.Sleep(1000);
                            counter++;
                        }
                        if (result.IsCompleted) //when we got here, the result is ready
                            entry = Dns.EndGetHostEntry(result);    //blocks?
                        else
                            Thread.CurrentThread.Abort();

                        //IPHostEntry entry = Dns.Resolve(hostNameOrAddress);
                        //IPHostEntry entry = Dns.GetHostEntry(ipAddress);
                        address = entry.AddressList[0];
                    }
                    catch (ThreadAbortException e)
                    {
                        Thread.CurrentThread.Abort();// ("DNS failed within timeout", e);
                    }
                    catch (Exception e)
                    {
                        throw new PingException("Impossible to resolve host or address.", e);
                    }
                }

                replyStats _replyStats = new replyStats(ipAddress);
                for (int ix = 0; ix < _numberOfPings; ix++)
                {
                    PingReply reply = myPing.Send(ipAddress, buf, iTimeout, myPingOptions);
                    sumRoundtripTime += reply.RoundTripTime;
                    if (reply.Status == IPStatus.Success)
                        _replyStats.add(1, 1, reply.RoundTripTime);
                    string sReply = myResources.getReply(reply, ipAddress);
                    System.Diagnostics.Debug.WriteLine(sReply);
                    onReply(new PingReplyEventArgs(sReply));
                }
                onReply(new PingReplyEventArgs(_replyStats.ToString(), PingReplyTypes.info));
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
