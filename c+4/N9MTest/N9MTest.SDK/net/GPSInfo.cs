using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N9MTest.SDK.net
{
    public class GPSInfo
    {
        public enum GPS_POSTION_STATE
        {
            Valid = 0,
            Invalid = 1,
        }

        //定位状态
        [JsonProperty(PropertyName = "V", NullValueHandling = NullValueHandling.Ignore)]
        public GPS_POSTION_STATE state;

        //经度。浮点数的字符串表示，保留6 位小数
        [JsonProperty(PropertyName = "J", NullValueHandling = NullValueHandling.Ignore)]
        public double longitude;

        //纬度。浮点数的字符串表示，保留6 位小数
        [JsonProperty(PropertyName = "W", NullValueHandling = NullValueHandling.Ignore)]
        public double latitude;

        //地面速率。整数，范围0~99999（0 公里/小时~999.99 公里/小时），单位百分之一公里/小时。单位公里
        [JsonProperty(PropertyName = "S", NullValueHandling = NullValueHandling.Ignore)]
        public int speed;

        //地面航向。整数，范围0~35999，表示0 度~359.99 度。以真北为参考基准顺时针旋转。
        [JsonProperty(PropertyName = "C", NullValueHandling = NullValueHandling.Ignore)]
        public int course;

        //1-14( 年月日时分秒:20120928121212， 表示2012 年9 月28 号12 点12 分12 秒)，带时区的当地时间
        public string time;

        public DateTime GetTime()
        {
            DateTime dt = DateTime.ParseExact(time, "yyyyMMddHHmmss", null);
            return dt;
        }

        public void Print()
        {
            Console.WriteLine("\r\n\r\n");
            Console.WriteLine("GPS 当前模块定位状态{0}, 位置{1}", state.ToString(), GetLocation());
            Console.WriteLine("\r\n\r\n");
        }

        public string GetLocation()
        {
            string location = "";

            if (longitude > 0)
            {
                location += Math.Abs(longitude)/1000000 + " E";
            }
            else if (longitude > 0)
            {
                location += Math.Abs(longitude) / 1000000 + " W";
            }
            else
            {
                location += Math.Abs(longitude)/1000000;
            }

            location += " ";

            if (latitude > 0)
            {
                location += Math.Abs(latitude)/1000000 + " N";
            }
            else if (latitude < 0)
            {
                location += Math.Abs(latitude) / 1000000 + " S";
            }
            else
            {
                location += Math.Abs(latitude) / 1000000;
            }

            return location;
        }

    }
}
