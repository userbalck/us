using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N9MTest.SDK.net
{
    public class StorageInfoEx
    {
        [JsonProperty(PropertyName = "STORAGEINDEX", NullValueHandling = NullValueHandling.Ignore)]
        public int index;

        [JsonProperty(PropertyName = "STORAGELASTSIZE", NullValueHandling = NullValueHandling.Ignore)]
        public long lastsize;

        [JsonProperty(PropertyName = "STORAGEPOSITION", NullValueHandling = NullValueHandling.Ignore)]
        public int postion;

        [JsonProperty(PropertyName = "STORAGESTATUS", NullValueHandling = NullValueHandling.Ignore)]
        public int status;

        [JsonProperty(PropertyName = "STORAGETOTALSIZE", NullValueHandling = NullValueHandling.Ignore)]
        public long totalsize;

        [JsonProperty(PropertyName = "STORAGETYPE", NullValueHandling = NullValueHandling.Ignore)]
        public int type;

        [JsonProperty(PropertyName = "STORAGEUNIT", NullValueHandling = NullValueHandling.Ignore)]
        public int unit;
    }
}
