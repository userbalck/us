using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N9MTest.SDK.net
{
    public class CMSConnectInfo
    {
        [JsonProperty(PropertyName = "CPT", NullValueHandling = NullValueHandling.Ignore)]
        public int nCPT;

        [JsonProperty(PropertyName = "CS", NullValueHandling = NullValueHandling.Ignore)]
        public int nCS;

        [JsonProperty(PropertyName = "ADD", NullValueHandling = NullValueHandling.Ignore)]
        public string address;

        [JsonProperty(PropertyName = "M", NullValueHandling = NullValueHandling.Ignore)]
        public int nM;

        [JsonProperty(PropertyName = "E", NullValueHandling = NullValueHandling.Ignore)]
        public int nE;
    }
}
