using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N9MTest.SDK.net
{
    public class DialInfo
    {
        public enum DialState
        {
            Unknown = 0,
            Dialing = 1,
            Success = 2,
            Abnormal = 3,
        }

        public enum NetMode
        {
            NoService = 0,
            MixMode = 1,
            G2 = 2,
            G3 = 3,
            G4 = 4
        }

        public enum NetType
        {
            GPRS = 0,
            CDMA = 1,
            EVDO = 2,
            WCDMA = 3,
            EDGE = 4,
            TDSCDMA = 5,
            LTE_TDD = 6,
            LTE_FDD = 7,
            UNKNOWN = 0xff,
        }

        public enum ModuleState
        {
            EXIST = 1,
            NOEXIST = 2,
        }

        //由15 位数字组成的 International Mobile Equipment Identity
        [JsonProperty(PropertyName = "IMEI", NullValueHandling = NullValueHandling.Ignore)]
        public string IMEI;

        //Sim 卡IMSI 号(IMSI ：International Mobile Subscriber Identification Number)
        [JsonProperty(PropertyName = "SIM", NullValueHandling = NullValueHandling.Ignore)]
        public string SIM;

        //信号强度
        [JsonProperty(PropertyName = "SI", NullValueHandling = NullValueHandling.Ignore)]
        public int SignalLevel;

        //模块状态 1-存在 2-不存在
        [JsonProperty(PropertyName = "ST", NullValueHandling = NullValueHandling.Ignore)]
        public ModuleState moduleState;

        //Sim 的状态 0-不存在 1-存在 2- 无效 3-有效
        [JsonProperty(PropertyName = "SS", NullValueHandling = NullValueHandling.Ignore)]
        public int SIMState;

        //拨号状态 0：未知、1：正在拨号、2：拨号成功、3：拨号异常
        [JsonProperty(PropertyName = "BS", NullValueHandling = NullValueHandling.Ignore)]
        public DialState dialState;

        //网络类型
        [JsonProperty(PropertyName = "NT", NullValueHandling = NullValueHandling.Ignore)]
        public NetType netType;

        //网络模式
        [JsonProperty(PropertyName = "NM", NullValueHandling = NullValueHandling.Ignore)]
        public NetMode netMode;

        //字符串，网卡名称
        [JsonProperty(PropertyName = "N", NullValueHandling = NullValueHandling.Ignore)]
        public string AdapterName;

        //拨号成功后显示IP 格式点分十进制10.100.23.4 如果无效则显示0.0.0.0
        [JsonProperty(PropertyName = "IP", NullValueHandling = NullValueHandling.Ignore)]
        public string IP;

        //模块软件版本号，字符串，直接显示，64 个字节范围
        [JsonProperty(PropertyName = "SVER", NullValueHandling = NullValueHandling.Ignore)]
        public string Version;

        public void Print()
        {
            Console.WriteLine("\r\n\r\n");
            Console.WriteLine("IMEI:{0}", IMEI);
            Console.WriteLine("Sim 卡IMSI 号:{0}", SIM);
            Console.WriteLine("信号强度:{0}", SignalLevel);
            Console.WriteLine("模块状态:{0}", moduleState.ToString());
            Console.WriteLine("Sim 的状态:{0}", SIMState.ToString());
            Console.WriteLine("拨号状态:{0}", dialState.ToString());
            Console.WriteLine("网络类型:{0}", netType.ToString());
            Console.WriteLine("网络模式:{0}", netMode.ToString());
            Console.WriteLine("网卡名称:{0}", AdapterName);
            Console.WriteLine("IP:{0}", IP);
            Console.WriteLine("模块软件版本号:{0}", Version);
            Console.WriteLine("\r\n\r\n");
        }
    }
}
