using Newtonsoft.Json;
using System;

namespace N9MTest.SDK.net
{
    public class RemoteFileInfo
    {
        [JsonProperty(PropertyName = "RECORDSIZE", NullValueHandling = NullValueHandling.Ignore)]
        public long dwFileSize;

        [JsonProperty(PropertyName = "RECORDCHANNEL", NullValueHandling = NullValueHandling.Ignore)]
        public int nChannel;

        [JsonProperty(PropertyName = "FILETYPE", NullValueHandling = NullValueHandling.Ignore)]
        public uint nFileType;

        [JsonProperty(PropertyName = "AT", NullValueHandling = NullValueHandling.Ignore)]
        public double nAT;

        [JsonProperty(PropertyName = "RECORD", NullValueHandling = NullValueHandling.Ignore)]
        public string szTime;

        [JsonProperty(PropertyName = "RECORDID", NullValueHandling = NullValueHandling.Ignore)]
        public string recordID;

        [JsonProperty(PropertyName = "LOCK", NullValueHandling = NullValueHandling.Ignore)]
        public uint bLocked;

        public string starttime
        {
            get
            {
                if (this.szTime == null)
                {
                    return null;
                }

                return this.szTime.Split('-')[0];
            }
        }

        public string endtime
        {
            get
            {
                if (this.szTime == null)
                {
                    return null;
                }

                return this.szTime.Split('-')[1];
            }
        }

        public DateTime GetStartTime()
        {
            DateTime dtStart = DateTime.ParseExact(starttime, "yyyyMMddHHmmss", null);

            return dtStart;
        }

        public DateTime GetEndTime()
        {
            DateTime dtEnd = DateTime.ParseExact(endtime, "yyyyMMddHHmmss", null);

            return dtEnd;
        }

        public TimeSpan GetTimeSpan()
        {
            return GetEndTime() - GetStartTime();
        }

        public static Comparison<RemoteFileInfo> comparison = new Comparison<RemoteFileInfo>
             ((RemoteFileInfo x, RemoteFileInfo y) =>
             {
                 if (x.nChannel < y.nChannel)
                 {
                     return -1;
                 }
                 else if (x.nChannel == y.nChannel)
                 {
                     if (x.GetStartTime() < y.GetStartTime())
                     {
                         return -1;
                     }
                     else if (x.GetStartTime() == y.GetStartTime())
                     {
                         return 0;
                     }
                     else if (x.GetStartTime() > y.GetStartTime())
                     {
                         return 1;
                     }
                 }

                 return 1;
             });
    }
}
