using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N9MTest.SDK.DSM
{
    public class DSMAlarmData
    {
        [JsonProperty(PropertyName = "ATYPE", NullValueHandling = NullValueHandling.Ignore)]
        public DSMAlarmType alarmType;

        [JsonProperty(PropertyName = "AS", NullValueHandling = NullValueHandling.Ignore)]
        public DSMAlarmLevel alarmLevel;


        //形成当前报警时的速度，使用INTEGER表示，单位为km/h
        [JsonProperty(PropertyName = "AP", NullValueHandling = NullValueHandling.Ignore)]
        public int alarmSpeed;


        //产生该事件或者报警时的时间，STRING型，YYYY-MM-DD HH:MM:SS 
        [JsonProperty(PropertyName = "AT", NullValueHandling = NullValueHandling.Ignore)]
        public string alarmTime;

        //识别该事件的关联图片ID值，使用INTEGER表示
        [JsonProperty(PropertyName = "IMG", NullValueHandling = NullValueHandling.Ignore)]
        public int imageID;
    }
}
