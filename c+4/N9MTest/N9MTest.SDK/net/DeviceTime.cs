using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N9MTest.SDK.net
{
    public class DeviceTime
    {
        public DateTime Now
        {
            get
            {
                return this.GetLocalTime();
            }
        }

        public DateTime UtcNow
        {
            get
            {
                return this.GetUTCTime();
            }
        }

        //设备端时间戳
        private string timez = "0A";
        private long curt = 0;

        DateTime dtDeviceTime;
        DateTime dtStartTime;

        private DeviceTime()
        {

        }

        public DeviceTime(string timez, long curt)
        {
            this.timez = timez;
            this.curt = curt;
            dtStartTime = DateTime.UtcNow;
        }

        public double GetTime()
        {
            TimeSpan timespan = DateTime.UtcNow - dtStartTime;
            return this.curt + timespan.TotalSeconds;
        }

        private  DateTime GetUTCTime()
        {
            TimeSpan timespan = DateTime.UtcNow - dtStartTime;
            return new DateTime(1970, 1, 1).AddSeconds(curt + timespan.TotalSeconds);
        }

        private DateTime GetLocalTime()
        {
            TimeSpan timespan = DateTime.UtcNow - dtStartTime;
            return new DateTime(1970, 1, 1).AddSeconds(curt + timespan.TotalSeconds).AddMinutes(Convert.ToInt32(this.timez.Substring(0, this.timez.Length - 1)));
        }
    }
}
