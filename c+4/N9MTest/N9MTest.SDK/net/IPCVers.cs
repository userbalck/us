using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N9MTest.SDK.net
{
    public class IPCVers
    {
        [JsonProperty(PropertyName = "INDEX", NullValueHandling = NullValueHandling.Ignore)]
        public int index;

        [JsonProperty(PropertyName = "V", NullValueHandling = NullValueHandling.Ignore)]
        public string version;
    }
}
