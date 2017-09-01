using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N9MTest.SDK.net
{
    public class CalendarData
    {
        [JsonProperty(PropertyName = "year", NullValueHandling = NullValueHandling.Ignore)]
        public int nYear;

        [JsonProperty(PropertyName = "month", NullValueHandling = NullValueHandling.Ignore)]
        public int nMonth;

        [JsonProperty(PropertyName = "day", NullValueHandling = NullValueHandling.Ignore)]
        public int nDay;

        [JsonProperty(PropertyName = "property", NullValueHandling = NullValueHandling.Ignore)]
        public int nProperty;

        public bool isEqualCalendar(CalendarData data)
        {
            if (this.nYear != data.nYear)
            {
                return false;
            }

            if (this.nMonth != data.nMonth)
            {
                return false;
            }

            if (this.nDay != data.nDay)
            {
                return false;
            }

            return true;
        }

        public static int Sort(CalendarData a, CalendarData b)
        {
            DateTime dtA = new DateTime(a.nYear, a.nMonth,a.nDay);
            DateTime dtB = new DateTime(b.nYear, b.nMonth, b.nDay);

            return DateTime.Compare(dtA, dtB);
        }

        public List<RemoteFileInfo> list;
    }
}
