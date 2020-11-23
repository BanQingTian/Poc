using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace NetWorkToolkit
{
    public class Utils
    {
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public static Task GetPublicIPAddressAsyn()
        {
            return new WebClient().DownloadStringTaskAsync("https://api.ipify.org");
        }

        public static string GetMacAddress()
        {
            var macAdress = "";
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            var i = 0;
            foreach (var adapter in nics)
            {
                var address = adapter.GetPhysicalAddress();
                if (address.ToString() != "")
                {
                    macAdress = address.ToString();
                    return macAdress;
                }
            }
            return "error lectura mac address";
        }
    }
}