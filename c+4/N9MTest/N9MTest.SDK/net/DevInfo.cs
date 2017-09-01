using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N9MTest.SDK
{
    public class DevInfo
    {
        [JsonProperty(PropertyName = "MANVERSION", NullValueHandling = NullValueHandling.Ignore)]
        public string mainVersion;

        [JsonProperty(PropertyName = "APPVERSION", NullValueHandling = NullValueHandling.Ignore)]
        public string appVersion;

        [JsonProperty(PropertyName = "PROTOCOLVERSION", NullValueHandling = NullValueHandling.Ignore)]
        public string protocolVersion;

        [JsonProperty(PropertyName = "UBOOT", NullValueHandling = NullValueHandling.Ignore)]
        public string uboot;

        [JsonProperty(PropertyName = "KERNEL", NullValueHandling = NullValueHandling.Ignore)]
        public string kernel;

        [JsonProperty(PropertyName = "ROOTFS", NullValueHandling = NullValueHandling.Ignore)]
        public string rootes;

        [JsonProperty(PropertyName = "MCU", NullValueHandling = NullValueHandling.Ignore)]
        public string mcu;

        [JsonProperty(PropertyName = "CP4", NullValueHandling = NullValueHandling.Ignore)]
        public string cp4;
    }
}
