using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N9MTest.SDK.net
{
    public enum SensorState
    {
        LowLevel = 0,
        HighLevel = 1,
        OpenCircult = 3,
        ShortCircult = 4,
    }

    public enum SensorUse
    {
        None = 0,
    }

    public class IOInfo
    {
        [JsonProperty(PropertyName = "SNO", NullValueHandling = NullValueHandling.Ignore)]
        public int nSNO;

        [JsonProperty(PropertyName = "NSER", NullValueHandling = NullValueHandling.Ignore)]
        public string user;

        [JsonProperty(PropertyName = "S", NullValueHandling = NullValueHandling.Ignore)]
        public SensorState nS;

        [JsonProperty(PropertyName = "U", NullValueHandling = NullValueHandling.Ignore)]
        public int nU;
    }
}
