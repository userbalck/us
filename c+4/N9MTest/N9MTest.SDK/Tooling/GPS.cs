using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N9MTest.SDK.Tooling
{
    public class GPS: DataConnection
    {
        /// <summary>
        /// //（RMC）推荐定位信息$GPRMC,000259.067,V,,,,,0.00,0.00,060180,,,N*4D
        /// $GPRMC,<1>,<2>,<3>,<4>,<5>,<6>,<7>,<8>,<9>,<10>,<11>,<12>*hh<CR><LF>
        /// </summary>
        private class GPRMC
        {
            //UTC时间 <1> UTC时间，hhmmss（时分秒）格式
            public DateTime dtTime;

            //定位状态 <2> 定位状态，A=有效定位，V=无效定位
            public string status = "";

            //纬度 <3> 纬度ddmm.mmmm（度分）格式（前面的0也将被传输）
            public string latitude = "";

            //纬度半球 <4> 纬度半球N（北半球）或S（南半球）
            public string LatH = "";

            //经度 <5> 经度dddmm.mmmm（度分）格式（前面的0也将被传输）
            public string longitude = "";

            //经度半球 <6> 经度半球E（东经）或W（西经）
            public string LonH = "";

            //地面速率 <7> 地面速率（000.0~999.9节，前面的0也将被传输）
            public string rate = "";

            //地面航向 <8> 地面航向（000.0~359.9度，以真北为参考基准，前面的0也将被传输）
            public string direction = "";

            //磁偏角 <10> 磁偏角（000.0~180.0度，前面的0也将被传输）
            public string declination = "";

            //<11> 磁偏角方向，E（东）或W（西）
            public string declinationDirection = "";

            //模式指示(A-自主定位 D-差分 E-估算 N-数据无效)
            public string indication;


            public static GPRMC ToObject(string data)
            {
                GPRMC gprmc = new GPRMC();

                string[] items = data.Split(',');

                if (items[1] != null && items[1].Length > 0 && items[9] != null && items[9].Length > 0)
                {
                    string time = items[0] + items[8];
                    gprmc.dtTime = DateTime.ParseExact(time, "yyyyMMddHHmmss", null);
                }


                if (items[2] != null && items[2].Length > 0)
                {
                    gprmc.status = items[2];
                }
                
                gprmc.latitude = items[3];
                gprmc.LatH = items[4];
                gprmc.longitude = items[5];
                gprmc.LonH = items[6];
                gprmc.rate = items[7];
                gprmc.direction = items[8];
                gprmc.declination = items[10];
                gprmc.indication = items[12];

                return gprmc;
            }

            public static string ToString(GPRMC gprmc)
            {
                string data = "$GPRMC";

                data += ",";

                //<1> UTC时间，hhmmss（时分秒）格式
                data += gprmc.dtTime.ToString("hhmmss");
                data += ",";

                //<2> 定位状态，A=有效定位，V=无效定位
                data += gprmc.status;
                data += ",";

                //<3> 纬度ddmm.mmmm（度分）格式（前面的0也将被传输）
                data += gprmc.latitude;
                data += ",";

                //<4> 纬度半球N（北半球）或S（南半球）
                data += gprmc.LatH;
                data += ",";

                //<5> 经度dddmm.mmmm（度分）格式（前面的0也将被传输）
                data += gprmc.longitude;
                data += ",";

                //<6> 经度半球E（东经）或W（西经）
                data += gprmc.latitude;
                data += ",";

                //地面速率（000.0~999.9节，前面的0也将被传输）
                data += gprmc.rate;
                data += ",";

                //<8> 地面航向（000.0~359.9度，以真北为参考基准，前面的0也将被传输）
                data += gprmc.direction;
                data += ",";

                //<9> UTC日期，ddmmyy（日月年）格式
                data += gprmc.dtTime.ToString("ddmmyy");
                data += ",";

                //<10> 磁偏角（000.0~180.0度，前面的0也将被传输）
                data += gprmc.declination;
                data += ".";

                //<11> 磁偏角方向，E（东）或W（西）
                data += gprmc.declinationDirection;
                data += ",";

                //<12> 模式指示（仅NMEA0183 3.00版本输出，A=自主定位，D=差分，E=估算，N=数据无效）
                data += gprmc.indication;

                //添加校验结束符
                data += "*HH";

                //添加指令结束符
                data += "\r\n";

                return data;
            }
        }

        /// <summary>
        /// GPS定位信息
        /// </summary>

        private class GPGGA
        {
            public static GPGGA ToObject(string data)
            {
                GPGGA gpgga = new GPGGA();

                return gpgga;
            }

            public static string ToString(GPGGA gpgga)
            {
                string data = "";

                return data;
            }
        }

        /// <summary>
        /// 定位地理信息
        /// </summary>

        private class GPGLL
        {
            public static GPGLL ToObject(string data)
            {
                GPGLL gpgll = new GPGLL();

                return gpgll;
            }

            public static string ToString(GPGLL gpgll)
            {
                string data = "";
                return data;
            }
        }

        /// <summary>
        /// 地面速度信息
        /// </summary>
        private class GPVTG
        {
            public static GPVTG ToObject(string data)
            {
                GPVTG gpvtg = new GPVTG();

                return gpvtg;
            }

            public static string ToString(GPVTG gpvtg)
            {
                string data = "";

                return data;
            }
        }
    }
}
