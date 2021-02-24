using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GMCC.ZPS.Business
{
    public class IPv4To5GUtil
    {
        public string Set5GIPAddress(string ip, int len)
        {
            Console.WriteLine("==>" + len);
            if (len >= 17)
            {
                long[] aa = GetIPIntScope(ip, GetNetMask("" + len));
                return IntToIp(aa[0]) + "-" + IntToIp(aa[1]);
            }
            else
            {
                long[] aa = GetIPIntScope(ip, GetNetMask("" + len));
                long min = IpToInt(aa[0].ToString());
                long max = IpToInt(aa[1].ToString());
                return countHalfIP(min, max);
            }

        }
        /**
     * 按半个C类地址拆分地址段
     * @param min
     * @param max
     */
        private string countHalfIP(long min, long max)
        {
            long tmp = min;
            string tmpIP = "";
            string ipA = "", ipB = "";
            long count = max - min;
            if (count <= 32768)
            {
                ipA = IntToIp(min);
                ipB = IntToIp(max);
                Console.WriteLine(ipA + "-" + ipB);
            }
            else
            {
                while (tmp <= max)
                {
                    ipA = IntToIp(tmp);
                    tmp += 32768;
                    ipB = IntToIp(tmp - 1);
                    tmpIP += ipA + "-" + ipB + ",";
                    Console.WriteLine(ipA + "-" + ipB);
                }
                tmpIP = tmpIP.TrimEnd(',') + ";";
            }
            return tmpIP;
        }
        private long[] GetIPIntScope(string ipAddr, string mask)
        {
            long ipInt;
            long netMaskInt = 0, ipcount = 0;
            try
            {
                ipInt = IpToInt(ipAddr);
                if (null == mask || "".Equals(mask))
                {
                    return new long[] { ipInt, ipInt };
                }
                netMaskInt = IpToInt(mask);
                ipcount = IpToInt("255.255.255.255") - netMaskInt;
                long netIP = ipInt & netMaskInt;
                long hostScope = netIP + ipcount;
                return new long[] { netIP, hostScope };
            }
            catch (Exception e)
            {

            }
            return null;
        }
        /**
     * 把IP地址转化为int
     *
     * @param ipAddr
     * @return int
     */
        private long IpToInt(String ipAddr)
        {
            return BytesToInt(IpToBytesByInet(ipAddr));
        }
        /**
    * 根据位运算把 byte[] -> int
    *
    * @param bytes
    * @return int
    */
        private long BytesToInt(byte[] bytes)
        {
            long addr = bytes[3] & 0xFF;
            addr |= ((bytes[2] << 8) & 0xFF00);
            addr |= ((bytes[1] << 16) & 0xFF0000);
            addr |= ((bytes[0] << 24) & 0xFF000000);
            return addr;
        }
        /**
     * 把IP地址转化为字节数组
     *
     * @param ipAddr
     * @return byte[]
     */
        private byte[] IpToBytesByInet(String ipAddr)
        {
            IPAddress addr = IPAddress.Parse(ipAddr);
            return addr.GetAddressBytes();

        }
        /**
     * 根据掩码位数获取掩码
     */
        private string GetNetMask(string mask)
        {
            int inetMask = int.Parse(mask);
            if (inetMask > 32)
            {
                return null;
            }
            //子网掩码为1占了几个字节
            int num1 = inetMask / 8;
            //子网掩码的补位位数
            int num2 = inetMask % 8;
            int[] array = new int[4];
            for (int i = 0; i < num1; i++)
            {
                array[i] = 255;
            }
            for (int i = num1; i < 4; i++)
            {
                array[i] = 0;
            }
            for (int i = 0; i < num2; num2--)
            {
                array[num1] += Convert.ToInt32(Math.Pow(2, 8 - num2));
            }
            string netMask = array[0] + "." + array[1] + "." + array[2] + "." + array[3];
            return netMask;
        }
        /**
     * 把int->ip地址
     *
     * @param ipInt
     * @return String
     */
        private string IntToIp(long ipInt)
        {
            return new StringBuilder().Append(((ipInt >> 24) & 0xff)).Append('.')
                    .Append((ipInt >> 16) & 0xff).Append('.').Append(
                            (ipInt >> 8) & 0xff).Append('.').Append((ipInt & 0xff))
                    .ToString();
        }
    }
}
