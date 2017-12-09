using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N9MTest.SDK.net
{
    public class WIFIInfo
    {
        public enum WIFI_MODULE_STATE
        {
            NOTEXIST = 0,
            EXIST = 1,
            ENABLE = 2,
        };
        //WIFI 模块的状态
        [JsonProperty(PropertyName = "MS", NullValueHandling = NullValueHandling.Ignore)]
        public WIFI_MODULE_STATE ms;

        //已连接AP 的ESSID
        [JsonProperty(PropertyName = "ID", NullValueHandling = NullValueHandling.Ignore)]
        public string essid;

        //信号值
        [JsonProperty(PropertyName = "SL", NullValueHandling = NullValueHandling.Ignore)]
        public int signalLevel;

        //网卡名称
        [JsonProperty(PropertyName = "N", NullValueHandling = NullValueHandling.Ignore)]
        public string adpterName;

        //mac地址
        [JsonProperty(PropertyName = "MAC", NullValueHandling = NullValueHandling.Ignore)]
        public string mac;

        //IP地址
        [JsonProperty(PropertyName = "IP", NullValueHandling = NullValueHandling.Ignore)]
        public string ip;

        public void Print()
        {
            Console.WriteLine("\r\n\r\n");
            Console.WriteLine("WIFI 模块状态:{0}", ms.ToString());
            Console.WriteLine("已连接AP的ESSID:{0}", essid);
            Console.WriteLine("信号值:{0}", signalLevel);
            Console.WriteLine("网卡名称:{0}", essid);
            Console.WriteLine("Mac地址:{0}", essid);
            Console.WriteLine("IP地址:{0}", essid);
            Console.WriteLine("\r\n\r\n");
        }
    }
}
