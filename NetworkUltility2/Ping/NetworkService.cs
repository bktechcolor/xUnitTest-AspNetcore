using NetworkUltility.DNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace NetworkUltility.Ping
{
    public class NetworkService
    {
        private readonly IDNS _dNS;

        public NetworkService(IDNS dNS)
        {
            _dNS = dNS;
        }
        public string SendPing()
        {
            //SearchDNS();
            //BuildPacket();
            var dnsSucess = _dNS.SendDNS();
            if (dnsSucess)
                return "Success: Ping Sent!";
            else 
                return "Failed: Ping not sent!";

        }
        public int PingTimeOut(int a, int b)
        {
            return a + b;
        }
        public DateTime LastPingDate()
        {
            return DateTime.Now;
        }
        public PingOptions GetPingOptions()
        {
            return new PingOptions()
            {
                DontFragment = true,
                Ttl = 1,
            };
        }
        public IEnumerable<PingOptions> MostRecentPings()
        {
            IEnumerable<PingOptions> pingOptions = new []
            {
                new PingOptions{
                DontFragment = true,
                Ttl = 1,
                },
                new PingOptions{
                DontFragment = true,
                Ttl = 1,
                },
                new PingOptions{
                DontFragment = true,
                Ttl = 1,
                }
            };
            return pingOptions;
        }
    }
}
