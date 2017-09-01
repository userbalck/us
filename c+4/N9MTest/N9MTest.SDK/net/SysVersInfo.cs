using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N9MTest.SDK.net
{
    public class SysVersInfo
    {
        [JsonProperty(PropertyName = "device", NullValueHandling = NullValueHandling.Ignore)]
        public string device;

        [JsonProperty(PropertyName = "software", NullValueHandling = NullValueHandling.Ignore)]
        public string software;

        [JsonProperty(PropertyName = "mcu", NullValueHandling = NullValueHandling.Ignore)]
        public string mcu;

        [JsonProperty(PropertyName = "cp4Ver", NullValueHandling = NullValueHandling.Ignore)]
        public string cp4Ver;

        [JsonProperty(PropertyName = "serialNum", NullValueHandling = NullValueHandling.Ignore)]
        public string serialNum;
    }
}
