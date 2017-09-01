using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    public class ExactTime
    {
        [DllImport("kernel32.dll")]
        extern static short QueryPerformanceCounter(ref long x);
        [DllImport("kernel32.dll")]
        extern static short QueryPerformanceFrequency(ref long x);

        public static long GetExactTime()
        {
            long freq = 0;
            long counter = 0;

            QueryPerformanceFrequency(ref freq);  //获取CPU频率
            QueryPerformanceCounter(ref counter); //获取初始前值

            return counter / freq;
        }

        public static long GetUTCTimeStamp()
        {
            DateTime dtStart = new DateTime(1970, 1, 1);
            DateTime dtNow = DateTime.Parse(GetNetworkTime().ToString());
            TimeSpan toNow = dtNow.Subtract(dtStart);
            string timeStamp = toNow.Ticks.ToString();
            timeStamp = timeStamp.Substring(0, timeStamp.Length - 7);
            return Convert.ToInt32(timeStamp);
        }

        public static long GetUTCTimeStamp(DateTime dt)
        {
            DateTime dtStart = new DateTime(1970, 1, 1);
            DateTime dtNow = DateTime.Parse(dt.ToString());
            TimeSpan toNow = dtNow.Subtract(dtStart);
            string timeStamp = toNow.Ticks.ToString();
            timeStamp = timeStamp.Substring(0, timeStamp.Length - 7);
            return Convert.ToInt32(timeStamp);
        }

        public static DateTime GetNetworkTime()
        {
            //返回国际标准时间
            //只使用的时间服务器的IP地址，未使用域名
            string[,] 时间服务器 = new string[14, 2];
            int[] 搜索顺序 = new int[] { 3, 2, 4, 8, 9, 6, 11, 5, 10, 0, 1, 7, 12 };
            时间服务器[0, 0] = "time-a.nist.gov";
            时间服务器[0, 1] = "129.6.15.28";
            时间服务器[1, 0] = "time-b.nist.gov";
            时间服务器[1, 1] = "129.6.15.29";
            时间服务器[2, 0] = "time-a.timefreq.bldrdoc.gov";
            时间服务器[2, 1] = "132.163.4.101";
            时间服务器[3, 0] = "time-b.timefreq.bldrdoc.gov";
            时间服务器[3, 1] = "132.163.4.102";
            时间服务器[4, 0] = "time-c.timefreq.bldrdoc.gov";
            时间服务器[4, 1] = "132.163.4.103";
            时间服务器[5, 0] = "utcnist.colorado.edu";
            时间服务器[5, 1] = "128.138.140.44";
            时间服务器[6, 0] = "time.nist.gov";
            时间服务器[6, 1] = "192.43.244.18";
            时间服务器[7, 0] = "time-nw.nist.gov";
            时间服务器[7, 1] = "131.107.1.10";
            时间服务器[8, 0] = "nist1.symmetricom.com";
            时间服务器[8, 1] = "69.25.96.13";
            时间服务器[9, 0] = "nist1-dc.glassey.com";
            时间服务器[9, 1] = "216.200.93.8";
            时间服务器[10, 0] = "nist1-ny.glassey.com";
            时间服务器[10, 1] = "208.184.49.9";
            时间服务器[11, 0] = "nist1-sj.glassey.com";
            时间服务器[11, 1] = "207.126.98.204";
            时间服务器[12, 0] = "nist1.aol-ca.truetime.com";
            时间服务器[12, 1] = "207.200.81.113";
            时间服务器[13, 0] = "nist1.aol-va.truetime.com";
            时间服务器[13, 1] = "64.236.96.53";
            int portNum = 13;
            string hostName;
            byte[] bytes = new byte[1024];
            int bytesRead = 0;
            System.Net.Sockets.TcpClient client = new TcpClient();
            for (int i = 0; i < 13; i++)
            {
                hostName = 时间服务器[搜索顺序[i], 1];
                try
                {
                    client.Connect(hostName, portNum);
                    NetworkStream ns = client.GetStream();
                    bytesRead = ns.Read(bytes, 0, bytes.Length);
                    client.Close();
                    break;
                }
                catch (Exception)
                {
                }
            }
            char[] sp = new char[1];
            sp[0] = ' ';
            DateTime dt = new DateTime();
            string str1;
            str1 = Encoding.ASCII.GetString(bytes, 0, bytesRead);

            string[] s;
            s = str1.Split(sp);
            dt = DateTime.Parse(s[1] + " " + s[2]);//得到标准时间
            return dt;

        }
    }
}
