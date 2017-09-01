using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N9MTest.SDK.net
{
    public class RecordStatus
    {
        [JsonProperty(PropertyName = "REA", NullValueHandling = NullValueHandling.Ignore)]
        public int nREA;

        [JsonProperty(PropertyName = "REMI", NullValueHandling = NullValueHandling.Ignore)]
        public int nREMI;

        [JsonProperty(PropertyName = "RES", NullValueHandling = NullValueHandling.Ignore)]
        public int nRES;

        [JsonProperty(PropertyName = "REV", NullValueHandling = NullValueHandling.Ignore)]
        public int nREV;

        [JsonProperty(PropertyName = "MT", NullValueHandling = NullValueHandling.Ignore)]
        public int nMT;

        [JsonProperty(PropertyName = "ST", NullValueHandling = NullValueHandling.Ignore)]
        public int nST;
    }
}
