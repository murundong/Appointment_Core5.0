using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CoreBaseClass
{
    public class NetworkHelper
    {
        /// <summary>
        /// ip转换为数字
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static long ConvertIpv4(string ip)
        {
            if (ip.IndexOf("\n", System.StringComparison.Ordinal) > -1)
            {
                ip = ip.Replace("\n", "");
            }

            //取出IP地址去掉‘.’后的string数组
            ip = ip == "::1" ? "127.0.0.1" : ip;
            if (!ValidateIpv4(ip))
            {
                throw new ArgumentException("Invalid IPv4 address");
            }
            string[] ips = ip.Split(".".ToCharArray());

            string hexIPstr = "";

            //循环数组，把数据转换成十六进制数，并合并数组(3dafe81e)
            foreach (string i in ips)
            {
                hexIPstr += Convert.ToInt16(i).ToString("x2");
            }

            //将十六进制数转换成十进制数
            return long.Parse(hexIPstr, System.Globalization.NumberStyles.HexNumber);
        }

        /// <summary>
        /// 验证是否是ip字符串
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool ValidateIpv4(string ip)
        {
            string regformat = @"^\d{1,3}[\.]\d{1,3}[\.]\d{1,3}[\.]\d{1,3}$";
            Regex regex = new Regex(regformat, RegexOptions.IgnoreCase);
            return !string.IsNullOrWhiteSpace(ip) && regex.IsMatch(ip);
        }

     

        /// <summary>
        /// 获取网卡IP列表
        /// </summary>
        /// <returns></returns>
        public static List<string> GetHostIp()
        {
            List<string> ipList = new List<string>();
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface networkIntf in networkInterfaces)
            {
                IPInterfaceProperties IPInterfaceProperties = networkIntf.GetIPProperties();
                UnicastIPAddressInformationCollection UnicastIPAddressInformationCollection = IPInterfaceProperties.UnicastAddresses;
                foreach (UnicastIPAddressInformation UnicastIPAddressInformation in UnicastIPAddressInformationCollection)
                {
                    if (UnicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ipList.Add(UnicastIPAddressInformation.Address.ToString());
                    }
                }
            }
            return ipList;
        }
        ///根据局域网IP获取主机名称 
        public static string GetHostNameByIp(string ip)
        {
            ip = ip.Trim();
            if (ip == string.Empty)
                return string.Empty;
            try
            {
                // 是否 Ping 的通
                if (Ping(ip))
                {

                    System.Net.IPHostEntry host = System.Net.Dns.GetHostEntry(ip);
                    return host.HostName;
                }
                return string.Empty;

            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        public static bool Ping(string ip)
        {
            Ping p = new Ping();
            PingOptions options = new PingOptions();
            options.DontFragment = true;
            string data = "Test Data!";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 1000; // Timeout 时间，单位：毫秒
            PingReply reply = p.Send(ip, timeout, buffer, options);
            if (reply.Status == IPStatus.Success)

                return true;
            else
                return false;
        }

    }
}
