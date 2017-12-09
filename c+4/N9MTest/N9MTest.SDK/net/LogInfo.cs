using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N9MTest.SDK.net
{
    public class LogInfo
    {
        [JsonProperty(PropertyName = "logType", NullValueHandling = NullValueHandling.Ignore)]
        public int logType;

        [JsonProperty(PropertyName = "logContent", NullValueHandling = NullValueHandling.Ignore)]
        public string logContent;

        [JsonProperty(PropertyName = "user", NullValueHandling = NullValueHandling.Ignore)]
        public string user;

        [JsonProperty(PropertyName = "ip", NullValueHandling = NullValueHandling.Ignore)]
        public string ip;

        [JsonProperty(PropertyName = "st", NullValueHandling = NullValueHandling.Ignore)]
        public string starttime;

        [JsonProperty(PropertyName = "ed", NullValueHandling = NullValueHandling.Ignore)]
        public string endtime;

        public DateTime GetStartTime()
        {
            DateTime dt = DateTime.ParseExact(starttime,"yyyy-MM-dd HH:mm:ss", null);

            return dt;
        }

        public DateTime GetEndTime()
        {
            DateTime dt = DateTime.ParseExact(endtime, "yyyy-MM-dd HH:mm:ss", null);

            return dt;
        }
    }
}
