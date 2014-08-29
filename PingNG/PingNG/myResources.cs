using System;

using System.Collections.Generic;
using System.Text;

using OpenNETCF.Net.NetworkInformation;

namespace PingNG
{
    public class myResources
    {
        /*
        Pinging google.de [74.125.224.183] with 32 bytes of data:
        Reply from 74.125.224.183: bytes=32 time=215ms TTL=34
        Reply from 74.125.224.183: bytes=32 time=214ms TTL=34
        Reply from 74.125.224.183: bytes=32 time=215ms TTL=34
        Reply from 74.125.224.183: bytes=32 time=215ms TTL=34

        Ping statistics for 74.125.224.183:
            Packets: Sent = 4, Received = 4, Lost = 0 (0% loss),
        Approximate round trip times in milli-seconds:
            Minimum = 214ms, Maximum = 215ms, Average = 214ms
        */
        const string PingServerOK = "Reply from {0}: bytes={1} time={2}ms TTL={3}";
        const string BadDestinationName = "Ping request could not find host {0}. Please check the name and try again.";
        const string BadOption = "Bad value for option: {0}";

        public static string getReply(PingReply _pingReply, string ipAddress)
        {
            string s = "";
            switch (_pingReply.Status){
                case IPStatus.Success:
                    s = string.Format("Reply from {0}: bytes={1} time={2}ms TTL={3}",
                        _pingReply.Address.ToString(),
                        _pingReply.DataSize,
                        _pingReply.RoundTripTime,
                        _pingReply.Options.Ttl);
                    break;
                case IPStatus.BadDestination:
                    s = string.Format(BadDestinationName, ipAddress);
                    break;
                case IPStatus.BadOption:
                    s=string.Format(BadOption, getPingOptions(_pingReply.Options));
                    break;
                case IPStatus.TimedOut:
                    s = string.Format("ping timedout for {0}.", ipAddress);
                    break;
                case IPStatus.DestinationHostUnreachable:
                    s = string.Format("Destination host {0} unreachable.", ipAddress);
                    break;
                case IPStatus.BadRoute:
                case IPStatus.DestinationNetworkUnreachable:
                case IPStatus.DestinationPortUnreachable:
                case IPStatus.DestinationProhibited:
                case IPStatus.DestinationScopeMismatch:
                case IPStatus.DestinationUnreachable:
                case IPStatus.HardwareError:
                case IPStatus.IcmpError:
                case IPStatus.NoResources:
                case IPStatus.PacketTooBig:
                case IPStatus.ParameterProblem:
                case IPStatus.SourceQuench:
                case IPStatus.TimeExceeded:
                case IPStatus.TtlExpired:
                case IPStatus.TtlReassemblyTimeExceeded:
                case IPStatus.Unknown:
                case IPStatus.UnrecognizedNextHeader:
                default:
                    s = "IPStatus." + _pingReply.Status.ToString();
                    break;
            }
            return s;
        }

        static string getPingOptions(PingOptions _pingOptions)
        {
            StringBuilder s = new StringBuilder();
            s.Append(string.Format("DF={0}, ", _pingOptions.DontFragment));
            s.Append(string.Format("TTL={0}", _pingOptions.Ttl));
            return s.ToString();
        }
        public myResources(string sTarget, int iBytes, int iTime, int iTTL)
        {
        }
    }
    public class replyStats
    {
        public int sent=0;
        public int rcvd=0;
        public int percentLost
        {
            get
            {
                if (sent != 0)
                    return ((lost * 100) / sent);
                else
                    return 100;
            }
        }
        public long minTrip=0;
        public long maxTrip=0;
        public long avgTrip{
            get
            {
                if (sent == 0)
                    return 0;
                if (tripValues.Count == 0)
                    return 0;
                int sum = 0;
                foreach (int v in tripValues)
                    sum += v;
                int avg = sum / tripValues.Count;
                return avg;
            }
        }
        List<long> tripValues=new List<long>();
        string ipAddress = "";
        public int lost
        {
            get
            {
                return sent - rcvd;
            }
        }
        public replyStats(string sIPaddress){
            ipAddress = sIPaddress;
            minTrip = long.MaxValue;
            maxTrip = 0;
        }
        public void add(int iSent, int iRcvd, long lTrip)
        {
            if (lTrip < minTrip)
                minTrip = lTrip;
            if (lTrip > maxTrip)
                maxTrip = lTrip;
            tripValues.Add(lTrip);
            sent += iSent;
            rcvd += iRcvd;
        }
        public override string ToString()
        {
            string s = string.Format(ReplyStats, ipAddress,
                sent, rcvd, lost, percentLost,
                minTrip, maxTrip, avgTrip);
            return s;
        }
        const string ReplyStats = "Ping statistics for {0}:\r\n" +
            "    Packets: Sent = {1}, Received = {2}, Lost = {3} ({4}% loss),\r\n" +
            "Approximate round trip times in milli-seconds:\r\n" +
            "    Minimum = {5}ms, Maximum = {6}ms, Average = {7}ms\r\n";
    }
}
