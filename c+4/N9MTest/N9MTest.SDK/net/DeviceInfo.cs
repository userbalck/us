using System;
using Newtonsoft.Json;

namespace N9MTest.SDK.net
{
    public class DeviceInfo
    {
        [JsonProperty(PropertyName = "ALARMIN", NullValueHandling = NullValueHandling.Ignore)]
        public int AlarmIn { get; set; }

        [JsonProperty(PropertyName = "ALARMOUT", NullValueHandling = NullValueHandling.Ignore)]
        public int AlarmOut { get; set; }

        [JsonProperty(PropertyName = "CHANNEL", NullValueHandling = NullValueHandling.Ignore)]
        public int Channel { get; set; }

        [JsonProperty(PropertyName = "CNAME", NullValueHandling = NullValueHandling.Ignore)]
        public string ChannelName { get; set; }

        [JsonProperty(PropertyName = "DEVCLASS", NullValueHandling = NullValueHandling.Ignore)]
        public int DevClass { get; set; }

        [JsonProperty(PropertyName = "DSNO", NullValueHandling = NullValueHandling.Ignore)]
        public string dsno { get; set; }

        [JsonProperty(PropertyName = "LEVEL", NullValueHandling = NullValueHandling.Ignore)]
        public int level { get; set; }

        [JsonProperty(PropertyName = "MTYPE", NullValueHandling = NullValueHandling.Ignore)]
        public int mType { get; set; }

        [JsonProperty(PropertyName = "PRO", NullValueHandling = NullValueHandling.Ignore)]
        public string pro { get; set; }

        [JsonProperty(PropertyName = "STYPE", NullValueHandling = NullValueHandling.Ignore)]
        public int stype { get; set; }

        [JsonProperty(PropertyName = "TYPE", NullValueHandling = NullValueHandling.Ignore)]
        public string type { get; set; }


        public void print()
        {
            Console.WriteLine("[{0}]AlarmIn = {1}", this.GetType().ToString(),this.AlarmIn);
            Console.WriteLine("[{0}]AlarmOut = {1}", this.GetType().ToString(),this.AlarmOut);
            Console.WriteLine("[{0}]Channel = {1}", this.GetType().ToString(),this.Channel);
            Console.WriteLine("[{0}]ChannelName = {1}", this.GetType().ToString(),this.ChannelName);
            Console.WriteLine("[{0}]DevClass = {1}", this.GetType().ToString(),this.DevClass);
            Console.WriteLine("[{0}]dsno = {1}", this.GetType().ToString(),this.dsno);
            Console.WriteLine("[{0}]level = {1}", this.GetType().ToString(),this.level);
            Console.WriteLine("[{0}]mType = {1}", this.GetType().ToString(),this.mType);
            Console.WriteLine("[{0}]pro = {1}", this.GetType().ToString(),this.pro);
            Console.WriteLine("[{0}]stype = {1}", this.GetType().ToString(),this.stype);
            Console.WriteLine("[{0}]type = {1}", this.GetType().ToString(),this.type);
        }
    }
}
