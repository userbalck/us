using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N9MTest.SDK.net
{
    public class StorageInfo
    {
        [JsonProperty(PropertyName = "id", NullValueHandling = NullValueHandling.Ignore)]
        public int id;

        [JsonProperty(PropertyName = "type", NullValueHandling = NullValueHandling.Ignore)]
        public int type;

        [JsonProperty(PropertyName = "state", NullValueHandling = NullValueHandling.Ignore)]
        public int state;

        [JsonProperty(PropertyName = "user", NullValueHandling = NullValueHandling.Ignore)]
        public int user;

        [JsonProperty(PropertyName = "left", NullValueHandling = NullValueHandling.Ignore)]
        public long left;

        [JsonProperty(PropertyName = "totle", NullValueHandling = NullValueHandling.Ignore)]
        public long total;

        [JsonProperty(PropertyName = "remaintime", NullValueHandling = NullValueHandling.Ignore)]
        public int remaintime;

        [JsonProperty(PropertyName = "internal", NullValueHandling = NullValueHandling.Ignore)]
        public int _internal;

        public void Print()
        {
            Console.WriteLine("\r\n\r\n");
            Console.WriteLine("磁盘编号:{0}", id);
            Console.WriteLine("磁盘类型:{0}", type);
            Console.WriteLine("磁盘状态:{0}", state);
            Console.WriteLine("磁盘剩余空间:{0}", left);
            Console.WriteLine("磁盘总空间:{0}", total);
            Console.WriteLine("\r\n\r\n");
        }
    }
}
