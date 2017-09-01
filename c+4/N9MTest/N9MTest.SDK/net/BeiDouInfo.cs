using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N9MTest.SDK.net
{
    public class BeiDouInfo
    {
        public enum BeiDouModuleState
        {
            NoExists = 0,
            Normal = 1,
            Invalid = 2,
        }

        //北斗模块状态
        [JsonProperty(PropertyName = "S", NullValueHandling = NullValueHandling.Ignore)]
        public BeiDouModuleState state;

        //卫星数量
        [JsonProperty(PropertyName = "N", NullValueHandling = NullValueHandling.Ignore)]
        public int Number;

        //北斗模块状态
        [JsonProperty(PropertyName = "SI", NullValueHandling = NullValueHandling.Ignore)]
        public int nSignalIntensity;
    }
}
