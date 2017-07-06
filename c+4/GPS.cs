using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static RM.GPS;

namespace RM
{
    /// <summary>
    /// 2017-03-27 NMEA解析
    /// http://www.gpsinformation.org/dale/nmea.htm
    /// https://wenku.baidu.com/view/c7501086e53a580216fcfe50.html
    /// http://baike.baidu.com/item/GPGGA?sefr=cr
    /// </summary>
    public class GPS
    {
        public enum eLat { N = 1, S = -1 }; public enum eLng { E = 1, W = -1 };
        /// <summary>
        /// //磁偏角
        /// </summary>
        public enum eMagnetic { E, W }
        public enum eValid { A, V }
        /// <summary>
        /// //A=自主定位，D=差分，E=估算，N=数据无效
        /// </summary>
        public enum eRMC_Mode { A, D, E, N }
        /// <summary>
        /// //GN：混合模式, GPS, GLONASS, Galileo, BDS
        /// </summary>
        public enum eGPSMode { GN, GP, GL, GA, BD}
        /// <summary>
        /// 系统标识符，旧版为None，未定义
        /// </summary>
        public enum eGPSFlag { None, GP, GL, GA, BD }
        public enum eIgnition { CLOSE, OPEN }
        /// <summary>
        /// //车辆运行状态
        /// </summary>
        public enum eVechileStatu { INVALID, NO_FIRE_STOP, FIRE_STOP, RUN }
        /// <summary>
        /// 定位模式，A=自动手动2D/3D，M=手动2D/3D
        /// </summary>
        public enum eGSA_Mode { A, M }
        /// <summary>
        /// 定位类型，1=未定位，2=2D定位，3=3D定位
        /// </summary>
        public enum eGSA_Fix { FixNo = 1, Fix2D = 2, Fix3D = 3 }
        /// <summary>
        /// GP时：0-定位模式不可用或无效；1-GPS SPS模式，定位有效；
        ///     2-差分GPSSPS模式，定位有效；3-GPS  PPS模式，定位有效；
        ///     4-实时动态（RTK），系统处于RTK模式中，有固定的整周数；
        ///     5-浮动的RTK，系统处于RTK模式中，整周数是浮动的；
        ///     6-估算模式（航位推算）；7-手动输入模式；8-模拟器模式。
        /// BD时：0-定位不可用或无效；1-无差分定位，定位有效；2-差分定位，定位有效；
        ///     3-双频定位，定位有效
        /// GN时：0-定位不可用或无效；1-兼容定位，定位有效
        /// </summary>
        public enum eGGA_Statu
        {
            Invalid, GPSFix, DGPSFix, PPSFix, RealTimeKinematic, FloatRTK,
            Estimated, InputMode, SimulationMode
        }
        /// <summary>
        /// 导航状态 
        /// Safe, Caution（完好性不可用）, UnSafe, V（无效）
        /// </summary>
        public enum eNaviStatu { S, C, U, V}
        public enum eVersion { V1, V2 }

        private DateTime _time = Convert.ToDateTime("1900-01-01");
        private GPS_RMC _rmcBD, _rmcGA, _rmcGL, _rmcGP, _rmcGN;
        private GPS_GSV _gsvBD, _gsvGP, _gsvGL, _gsvGA;
        private GPS_GGA _ggaBD, _ggaGP, _ggaGA, _ggaGL, _ggaGN;
        private GPS_GSA _gsaBD, _gsaGP, _gsaGL, _gsaGN, _gsaGA;
        private GPS_PUL _pul;
        private GPS_ACC _acc;

        public GPS_RMC RmcBD { get => _rmcBD; }
        public GPS_RMC RmcGP { get => _rmcGP; }
        public GPS_RMC RmcGN { get => _rmcGN; }
        public GPS_RMC RmcGA { get => _rmcGA; }
        public GPS_RMC RmcGL { get => _rmcGL; }

        public GPS_GSV GsvBD { get => _gsvBD; }
        public GPS_GSV GsvGP { get => _gsvGP; }
        public GPS_GSV GsvGL { get => _gsvGL; }
        public GPS_GSV GsvGA { get => _gsvGA; }

        public GPS_GGA GgaBD { get => _ggaBD; }
        public GPS_GGA GgaGP { get => _ggaGP; }
        public GPS_GGA GgaGN { get => _ggaGN; }
        public GPS_GGA GgaGA { get => _ggaGA; }
        public GPS_GGA GgaGL { get => _ggaGL; }

        public GPS_PUL Pul { get => _pul; }
        public GPS_ACC Acc { get => _acc; }

        public GPS_GSA GsaBD { get => _gsaBD; }
        public GPS_GSA GsaGP { get => _gsaGP; }
        public GPS_GSA GsaGL { get => _gsaGL; }
        public GPS_GSA GsaGN { get => _gsaGN; }
        public GPS_GSA GsaGA { get => _gsaGA; }

        public DateTime Time
        {
            get
            {
                if (this.RmcGP != null) this._time = this.RmcGP.Time;
                else if (this.RmcGN != null) this._time = this.RmcGN.Time;
                else if (this.RmcBD != null) this._time = this.RmcBD.Time;
                return (this._time);
            }
        }
        public eValid Valid
        {
            get
            {
                if (this._rmcGP != null) return (this._rmcGP.Valid);
                else if (this._rmcGN != null) return (this._rmcGN.Valid);
                else if (this._rmcBD != null) return (this._rmcBD.Valid);
                else return (eValid.V);
            }
        }
        /// <summary>
        /// 只返回正值，不带方向
        /// </summary>
        public float Lat
        {
            get
            {
                if (this._rmcGP != null) return (this._rmcGP.Lat);
                else if (this._rmcGN != null) return (this._rmcGN.Lat);
                else if (this._rmcBD != null) return (this._rmcBD.Lat);
                else return (0);
            }
        }
        /// <summary>
        /// 只返回正值，不带方向
        /// </summary>
        public float Lng
        {
            get
            {
                if (this._rmcGP != null) return (this._rmcGP.Lng);
                else if (this._rmcGN != null) return (this._rmcGN.Lng);
                else if (this._rmcBD != null) return (this._rmcBD.Lng);
                else return (0);
            }
        }
        public float getLat()
        {
            return (this.Lat * (ELat == eLat.S ? -1 : 1));
        }
        public float getLng()
        {
            return (this.Lng * (ELng == eLng.W ? -1 : 1));
        }
        public eLat ELat
        {
            get
            {
                if (this._rmcGP != null) return (this._rmcGP.ELat);
                else if (this._rmcGN != null) return (this._rmcGN.ELat);
                else if (this._rmcBD != null) return (this._rmcBD.ELat);
                else return (eLat.N);
            }
        }
        public eLng ELng
        {
            get
            {
                if (this._rmcGP != null) return (this._rmcGP.ELng);
                else if (this._rmcGN != null) return (this._rmcGN.ELng);
                else if (this._rmcBD != null) return (this._rmcBD.ELng);
                else return (eLng.E);
            }
        }
        public float Speed
        {
            get
            {
                if (this._rmcGP != null) return (this._rmcGP.Speed);
                else if (this._rmcGN != null) return (this._rmcGN.Speed);
                else if (this._rmcBD != null) return (this._rmcBD.Speed);
                else return (0);
            }
        }
        public float Bearing
        {
            get
            {
                if (this._rmcGP != null) return (this._rmcGP.Bearing);
                else if (this._rmcGN != null) return (this._rmcGN.Bearing);
                else if (this._rmcBD != null) return (this._rmcBD.Bearing);
                else return (0);
            }
        }
        public eIgnition Ignition
        {
            get { return (this._acc == null ? eIgnition.CLOSE : this._acc.Ignition); }
        }
        public eVechileStatu VechileStatu
        {
            get { return (this._acc == null ? eVechileStatu.INVALID : this._acc.VechileStatu); }
        }
        public int OverSpeedLimit
        {
            get { return (this._acc == null ? 0 : this._acc.OverSpeedLimit); }
        }
        public static bool IsTimeNull(DateTime dt)
        {
            return (dt.ToString("yyyy-MM-dd") == "1900-01-01");
        }
        public int AccX
        {
            get { return (this._acc == null ? 0 : this._acc.AccX); }
        }
        public int AccY
        {
            get { return (this._acc == null ? 0 : this._acc.AccY); }
        }
        public int AccZ
        {
            get { return (this._acc == null ? 0 : this._acc.AccZ); }
        }
        public int ImpulseSpeed
        {
            get { return (this._acc == null ? 0 : this._pul.ImpulseSpeed); }
        }

        public bool IsGPSSentence(string nmea)
        {
            return (GPS_RMC.IsRMC(nmea, eGPSMode.BD) || GPS_RMC.IsRMC(nmea, eGPSMode.GP)
                || GPS_RMC.IsRMC(nmea, eGPSMode.GN)
                || GPS_GSV.IsGSV(nmea, eGPSMode.BD) || GPS_GSV.IsGSV(nmea, eGPSMode.GP)
                || GPS_ACC.IsACC(nmea) || GPS_PUL.IsPUL(nmea)
                || GPS_GSA.IsGSA(nmea, eGPSMode.BD) || GPS_GSA.IsGSA(nmea, eGPSMode.GP)
                || GPS_GGA.IsGGA(nmea, eGPSMode.BD) || GPS_GGA.IsGGA(nmea, eGPSMode.GP)
                || GPS_GGA.IsGGA(nmea, eGPSMode.GN)
                );
        }

        public delegate void ParseProgress(int v);
        /// <summary>
        /// 多个单条nmea不断的解析到gps中,每一个元素为一组GPS数据，第一组和最后一组有可能没有时间RMC语句
        /// 可能有些不一定是合法的nmea语句
        /// </summary>
        /// <param name="nmea">单条语句</param>
        /// <param name="gps"></param>
        /// <returns></returns>
        public static List<GPS> Parse(string nmea, List<GPS> gpss)
        {
            if (!GPS.IsVerifySuccess(nmea)) return (gpss);
            
            GPS lastgps = null;
            if (gpss == null) gpss = new List<GPS>();
            if (gpss.Count > 0) lastgps = gpss[gpss.Count - 1];
            if (lastgps == null) { lastgps = new GPS(); gpss.Add(lastgps); }

            if (GPS_ACC.IsACC(nmea)) lastgps._acc = GPS_ACC.Parse(nmea);
            else if (GPS_PUL.IsPUL(nmea)) lastgps._pul = GPS_PUL.Parse(nmea);
            else if (GPS_GGA.IsGGA(nmea, eGPSMode.GP)) lastgps._ggaGP = GPS_GGA.Parse(nmea);
            else if (GPS_GGA.IsGGA(nmea, eGPSMode.BD)) lastgps._ggaBD = GPS_GGA.Parse(nmea);
            else if (GPS_GGA.IsGGA(nmea, eGPSMode.GN)) lastgps._ggaGN = GPS_GGA.Parse(nmea);
            else if (GPS_GGA.IsGGA(nmea, eGPSMode.GA)) lastgps._ggaGA = GPS_GGA.Parse(nmea);
            else if (GPS_GGA.IsGGA(nmea, eGPSMode.GL)) lastgps._ggaGL = GPS_GGA.Parse(nmea);

            else if (GPS_GSV.IsGSV(nmea, eGPSMode.GP)) lastgps._gsvGP = GPS_GSV.Parse(nmea, lastgps._gsvGP);
            else if (GPS_GSV.IsGSV(nmea, eGPSMode.BD)) lastgps._gsvBD = GPS_GSV.Parse(nmea, lastgps._gsvBD);
            else if (GPS_GSV.IsGSV(nmea, eGPSMode.GL)) lastgps._gsvGL = GPS_GSV.Parse(nmea, lastgps._gsvGL);
            else if (GPS_GSV.IsGSV(nmea, eGPSMode.GA)) lastgps._gsvGA = GPS_GSV.Parse(nmea, lastgps._gsvGA);

            else if (GPS_GSA.IsGSA(nmea, eGPSMode.GP)) lastgps._gsaGP = GPS_GSA.Parse(nmea);
            else if (GPS_GSA.IsGSA(nmea, eGPSMode.BD)) lastgps._gsaBD = GPS_GSA.Parse(nmea);
            else if (GPS_GSA.IsGSA(nmea, eGPSMode.GL)) lastgps._gsaGL = GPS_GSA.Parse(nmea);
            else if (GPS_GSA.IsGSA(nmea, eGPSMode.GN)) lastgps._gsaGN = GPS_GSA.Parse(nmea);
            else if (GPS_GSA.IsGSA(nmea, eGPSMode.GA)) lastgps._gsaGA = GPS_GSA.Parse(nmea);

            else if (GPS_RMC.IsRMC(nmea))
            {
                GPS_RMC rmc = GPS_RMC.Parse(nmea);
                if (rmc != null)
                {
                    if (!GPS.IsTimeNull(lastgps.Time)
                       && lastgps.Time.ToString("yyyyMMddHHmmss") != rmc.Time.ToString("yyyyMMddHHmmss"))
                    {
                        lastgps = new GPS(); gpss.Add(lastgps);
                    }
                    switch (rmc.GPSMode)
                    {
                        case eGPSMode.GP: lastgps._rmcGP = rmc; break;
                        case eGPSMode.BD: lastgps._rmcBD = rmc; break;
                        case eGPSMode.GN: lastgps._rmcGN = rmc; break;
                        case eGPSMode.GA: lastgps._rmcGA = rmc; break;
                        case eGPSMode.GL: lastgps._rmcGL = rmc; break;
                    }
                }
            }
            return (gpss);
        }
        /// <summary>
        /// 多个单条nmea不断的解析到gps中,每一个元素为一组GPS数据，第一组和最后一组有可能没有时间RMC语句
        /// 可能有些不一定是合法的nmea语句
        /// </summary>
        /// <param name="nmeasorfiltpath">多条所有nmea语句，以\r\n分隔，或者是nmea文本文件路径</param>
        /// <param name="isfile"></param>
        /// <returns></returns>
        public static List<GPS> Parse(string nmeasorfiltpath, bool isfile = false, ParseProgress parseProgress = null)
        {
            string nmeas = "";
            if (isfile)
            {
                StreamReader sr = new StreamReader(nmeasorfiltpath);
                nmeas = sr.ReadToEnd();
                sr.Close(); sr.Dispose();
            }
            else nmeas = nmeasorfiltpath;
            string[] data = nmeas.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            List<GPS> gpss = null; int lastpsb = 0;
            for (int i = 0; i < data.Length; i++)
            {
                int idx = data[i].IndexOf("$");
                if (idx < 0) continue;
                data[i] = data[i].Substring(idx);
                gpss = GPS.Parse(data[i], gpss);
                if (parseProgress != null)
                {
                    int v = (i + 1) * 100 / data.Length;
                    if (v % 10 == 0 && v != lastpsb)
                    {
                        lastpsb = v;
                        parseProgress(Convert.ToInt32(v));
                    }
                }
            }
            if (parseProgress != null) parseProgress(100);
            return (gpss);
        }

        /// <summary>
        /// 计算验证码
        /// </summary>
        /// <param name="nmea">$GNRMC,004159.000,A,2239.372351,N,11404.268622,E,14.464,357.181,031016,,E,A*6A</param>
        /// <returns></returns>
        public static string ComputerVerify(string nmea)
        {
            if (nmea.Substring(0, 1) == "$") nmea = nmea.Substring(1);
            if (nmea.IndexOf("*") > 0) nmea = nmea.Substring(0, nmea.IndexOf("*"));
            byte[] gbyte = Encoding.ASCII.GetBytes(nmea);
            byte b = gbyte[0];
            for (int j = 1; j < gbyte.Length; j++)
                b = (byte)(b ^ gbyte[j]);
            return (b.ToString("x").PadLeft(2, '0').ToUpper());
        }
        /// <summary>
        /// 所有语句检查验证码是否正确，以检查nmea是否合法
        /// </summary>
        /// <param name="nmea"></param>
        /// <returns></returns>
        public static bool IsVerifySuccess(string nmea)
        {
            Match m = Regex.Match(nmea, @"\*([A-FH\d]{2})[\r|\n|\s]*");
            if (m.Groups.Count != 2) return (false);
            return (GPS.ComputerVerify(nmea) == m.Groups[1].Value || m.Groups[1].Value == "HH");
        }


        private static double rad(double d)
        {
            return d * Math.PI / 180.0;
        }

        public static double getDistance(double lat1, double lng1, double lat2, double lng2)
        {
            const double EARTH_RADIUS = 6378.137;
            double a = rad(lat1) - rad(lat2);
            double b = rad(lng1) - rad(lng2);
            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +
                Math.Cos(rad(lat2)) * Math.Cos(rad(lat1)) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * EARTH_RADIUS;
            return (s);
        }

        /** 
   * 获取AB连线与正北方向的角度 
   * @param A  A点的经纬度 
   * @param B  B点的经纬度 
   * @return  AB连线与正北方向的角度（0~360） 
   */
        public static double getAngle(double lat1, double lng1, double lat0, double lng0)
        {
            double Rc = 6378137; double Rj = 6356725;
            double dx = (lng1 * Math.PI / 180 - lng0 * Math.PI / 180) * (Rj + (Rc - Rj) * (90f - lat0) / 90f) * Math.Cos(lat0 * Math.PI / 180f);
            double dy = (lat1 * Math.PI / 180 - lat0 * Math.PI / 180) * (Rj + (Rc - Rj) * (90f - lat0) / 90f);
            double angle = 0.0;
            angle = Math.Atan(Math.Abs(dx / dy)) * 180f / Math.PI;
            double dLo = lng1 - lng0;
            double dLa = lat1 - lat0;
            if (dLo > 0 && dLa <= 0)
            {
                angle = (90f - angle) + 90;
            }
            else if (dLo <= 0 && dLa < 0)
            {
                angle = angle + 180f;
            }
            else if (dLo < 0 && dLa >= 0)
            {
                angle = (90f - angle) + 270;
            }
            return angle;
        }

    }
    /// <summary>
    /// $GPPUL,1476540281,200,0,0,36000*HH，暂不检查校验值，所有字段可能为空，连逗号都没有
    /// </summary>
    public class GPS_PUL
    {
        private Int64 _timeStamp = 0;
        private TimeSpan _interval = TimeSpan.FromMilliseconds(0);
        private int _impulseCount = 0;
        private int _impulseSpeed = 0;
        private int _impulseCoefficient = 0;

        /// <summary>
        /// 时间戳，后续结构体中会去除
        /// </summary>
        public long TimeStamp { get => _timeStamp; set => _timeStamp = value; }
        /// <summary>
        /// 例：每200ms一个脉冲，脉冲采集间隔
        /// </summary>
        public TimeSpan Interval { get => _interval; set => _interval = value; }
        /// <summary>
        /// 脉冲数
        /// </summary>
        public int ImpulseCount { get => _impulseCount; set => _impulseCount = value; }
        /// <summary>
        /// 脉冲速度，公里每小时
        /// </summary>
        public int ImpulseSpeed { get => _impulseSpeed; }
        /// <summary>
        /// 脉冲系数
        /// </summary>
        public int ImpulseCoefficient { get => _impulseCoefficient; set => _impulseCoefficient = value; }

        public string NMEA
        {
            get
            {
                string nmea = $"$GPPUL,{_timeStamp},{(int)_interval.TotalMilliseconds},{_impulseCount},{_impulseSpeed},{_impulseCoefficient}";
                nmea = $"${nmea}*{GPS.ComputerVerify(nmea)}";
                return (nmea);
            }
        }
        /// <summary>
        /// $GPPUL,1476540281,200,0,0,36000*HH，暂不检查校验值，所有字段可能为空，连逗号都没有
        /// </summary>
        /// <param name="nmea"></param>
        /// <returns></returns>
        public static bool IsPUL(string nmea)
        {
            return (Regex.IsMatch(nmea, @"\$GPPUL"));
        }
        /// <summary>
        /// $GPPUL,1476540281,200,0,0,36000*HH，暂不检查校验值，所有字段可能为空，连逗号都没有
        /// </summary>
        /// <param name="nmea"></param>
        /// <returns></returns>
        public static GPS_PUL Parse(string nmea)
        {
            Match match = Regex.Match(nmea, @"(\$[A-Z]{2}ACC),(.+)\*([A-H\d]{2})");
            if (match.Groups.Count == 0 || match.Groups[0].Value == "") return (null);
            GPS_PUL pul = new GPS_PUL();
            string[] tmp = match.Groups[2].Value.Split(','); //1476540281,200,0,0,36000
            if (tmp[2] != "") pul._impulseSpeed = Convert.ToInt32(tmp[2]);
            return (pul);
        }
    }
    /// <summary>
    /// $GPACC,255,1,100*HH，暂不检查校验值，所有字段可能为空，连逗号都没有
    /// </summary>
    public class GPS_ACC
    {
        private eIgnition _ignition = eIgnition.CLOSE;
        private eVechileStatu _vechileStatu = eVechileStatu.INVALID;
        private int _overSpeedLimit = 0;
        private int _accX, _accY, _accZ;

        public eIgnition Ignition { get => _ignition; }
        public eVechileStatu VechileStatu { get => _vechileStatu; }
        public int OverSpeedLimit { get => _overSpeedLimit; }
        public int AccX { get => _accX; set => _accX = value; }
        public int AccY { get => _accY; set => _accY = value; }
        public int AccZ { get => _accZ; set => _accZ = value; }

        public string NMEA
        {
            get
            {
                string nmea = $"GPACC,{(int)_ignition},{(int)_vechileStatu},{_overSpeedLimit},{_accX},{_accY},{_accZ}";
                nmea = $"${nmea}*{GPS.ComputerVerify(nmea)}";
                return (nmea);
            }
        }
        /// <summary>
        /// $GPACC,255,1,100*HH，暂不检查校验值，所有字段可能为空，连逗号都没有
        /// </summary>
        /// <param name="nmea"></param>
        /// <returns></returns>
        public static bool IsACC(string nmea)
        {
            return (Regex.IsMatch(nmea, @"\$GPACC"));
        }
        /// <summary>
        /// $GPACC,0,1,60,-250,2,0,*HH，暂不检查校验值，所有字段可能为空，连逗号都没有
        /// </summary>
        /// <param name="nmea"></param>
        /// <returns></returns>
        public static GPS_ACC Parse(string nmea)
        {
            Match match = Regex.Match(nmea, @"(\$[A-Z]{2}ACC),(.+)\*([A-H\d]{2})");
            if (match.Groups.Count == 0 || match.Groups[0].Value == "") return (null);
            GPS_ACC acc = new GPS_ACC();
            string[] tmp = match.Groups[2].Value.Split(','); //0,1,60,-250,2,0,
            if (tmp.Length == 6)
            {
                if (tmp[0] != "") acc._ignition = (eIgnition)Convert.ToInt16(tmp[0]);
                if (tmp[1] != "") acc._vechileStatu = (eVechileStatu)Convert.ToInt16(tmp[1]);
                if (tmp[2] != "") acc._overSpeedLimit = Convert.ToInt32(tmp[2]);
                if (tmp[3] != "") acc._accX = Convert.ToInt32(tmp[3]);
                if (tmp[4] != "") acc._accY = Convert.ToInt32(tmp[4]);
                if (tmp[5] != "") acc._accZ = Convert.ToInt32(tmp[5]);
            }
            else if (tmp.Length == 3)
            {
                if (tmp[0] != "") acc._accX = Convert.ToInt32(tmp[0]);
                if (tmp[1] != "") acc._accY = Convert.ToInt32(tmp[1]);
                if (tmp[2] != "") acc._accZ = Convert.ToInt32(tmp[2]);
            }
            else acc = null;            
            return (acc);
        }
    }
    /// <summary>
    /// $GPGGA,014434.70,3817.13334637,N,12139.72994196,E,4,07,1.5,6.571,M,8.942,M,0.7,0016*7B
    /// GP/BD/GN
    /// </summary>
    public class GPS_GGA
    {
        private eGPSMode _gpsmode = eGPSMode.GP; //GPS or BD or GN or GL or GA
        private DateTime _time; //仅有时间部分：hhmmss.sss
        private float _lat, _lng;
        private eLat _elat; private eLng _elng;
        private eGGA_Statu _statu = eGGA_Statu.Invalid;
        private int _satelliteCount = 0;
        private float _HDOP;
        private float _altitude;
        private string _altitudeUnit;//海拔单位，如该值为"M"，则为米，一般为M
        private float _geoIdHeight;
        private string _geoIdHeightUnit; //M：米
        private int _rtcm;
        private int _gpsStationID;
        /// <summary>
        /// GPS or BD or Auto
        /// </summary>
        public eGPSMode GPSMode { get => _gpsmode; }
        /// <summary>
        /// 仅有时间部分：hhmmss.sss
        /// </summary>
        public DateTime Time { get => _time; set => _time = value; }
        public float Lat { get => _lat; }
        public float Lng { get => _lng; }
        public eLat Elat { get => _elat; }
        public eLng Elng { get => _elng; }
        public eGGA_Statu Statu { get => _statu; }
        /// <summary>
        /// 参与定位的卫星数量
        /// </summary>
        public int SatelliteCount { get => _satelliteCount; }
        /// <summary>
        /// HDOP水平精度因子（0.5 - 99.9）
        /// </summary>
        public float HDOP { get => _HDOP; }
        /// <summary>
        /// 天线大地高
        /// </summary>
        public float Altitude { get => _altitude; }
        /// <summary>
        /// 天线大地高单位：M，米
        /// </summary>
        public string AltitudeUnit { get => _altitudeUnit; }
        /// <summary>
        /// 高程异常
        /// </summary>
        public float GeoIdHeight { get => _geoIdHeight; }
        /// <summary>
        /// 高程异常单位：M，米
        /// </summary>
        public string GeoIdHeightUnit { get => _geoIdHeightUnit; }
        /// <summary>
        /// 差分数据龄期，一般为空
        /// </summary>
        public int Rtcm { get => _rtcm; }
        /// <summary>
        /// 差分站台ID号，一般为空
        /// </summary>
        public int GpsStationID { get => _gpsStationID; }

        /// <summary>
        /// $GPGGA,014434.70,3817.13334637,N,12139.72994196,E,4,07,1.5,6.571,M,8.942,M,0.7,0016*7B
        /// GP/BD
        /// </summary>
        /// <param name="nmea"></param>
        /// <param name="cate"></param>
        /// <returns></returns>
        public static bool IsGGA(string nmea, eGPSMode cate)
        {
            string partten = "";
            switch (cate)
            {
                case eGPSMode.BD: partten = @"\$BDGGA"; break;
                case eGPSMode.GP: partten = @"\$GPGGA"; break;
                case eGPSMode.GN: partten = @"\$GNGGA"; break;
                case eGPSMode.GA: partten = @"\$GAGGA"; break;
                case eGPSMode.GL: partten = @"\$GLGGA"; break;
            }
            return (Regex.IsMatch(nmea, partten));
        }

        public string NMEA
        {
            get
            {
                string nmea = "";
                switch (this.GPSMode)
                {
                    case eGPSMode.GP: nmea += "GP"; break;
                    case eGPSMode.BD: nmea += "BD"; break;
                    case eGPSMode.GN: nmea += "GN"; break;
                    case eGPSMode.GA: nmea += "GA"; break;
                    case eGPSMode.GL: nmea += "GL"; break;
                }
                string latd = ((int)this.Lat).ToString().PadLeft(2, '0');
                string latm = Math.Round((this.Lat - (int)this.Lat) * 60, 4).ToString();
                int latp = latm.IndexOf("."); if (latp < 0) latp = latm.Length;
                string latm0 = latm.Substring(0, latp).PadLeft(2, '0');
                string latm1 = "";
                if (latp == latm.Length) latm1 = "0000";
                else latm1 = latm.Substring(latp + 1).PadRight(4, '0');
                string latstr = latd + latm0 + "." + latm1;

                string lngd = ((int)this.Lng).ToString().PadLeft(3, '0');
                string lngm = Math.Round((this.Lng - (int)this.Lng) * 60, 4).ToString();
                int lngp = lngm.IndexOf("."); if (lngp < 0) lngp = lngm.Length;
                string lngm0 = lngm.Substring(0, lngp).PadLeft(2, '0');
                string lngm1 = "";
                if (lngp == lngm.Length) lngm1 = "0000";
                else lngm1 = lngm.Substring(lngp + 1).PadRight(4, '0');
                string lngstr = lngd + lngm0 + "." + lngm1;

                nmea += $"GGA,{this._time.ToString("HHmmss")}.000,{latstr},{this._elat.ToString()}"
                    + $",{lngstr},{this._elng.ToString()},{(int)this._statu},{this._satelliteCount.ToString().PadLeft(2, '0')}"
                    + $",{Math.Round(this._HDOP, 1)},{Math.Round(this._altitude, 1)},M,{Math.Round(this._geoIdHeight, 1)},M"
                    + $",{this._rtcm},{this._gpsStationID.ToString().PadLeft(4, '0')}";

                nmea = $"${nmea}*{GPS.ComputerVerify(nmea)}";
                return (nmea);
            }
        }

        public static GPS_GGA Parse(string nmea)
        {
            Match match = Regex.Match(nmea,
                @"(\$[A-Z]{2}GGA),(\d{6})\.\d{0,3},(\d{4}\.\d+),([NS]{1}),(\d{5}\.\d+),([EW]{1}),(\d{1}),(\d{0,2}),(\d*\.*\d*)"
                + @",(-*\d*\.*\d*),(M{0,1}),(-*\d*\.*\d*),(M{0,1}),(.*),(\d*)\*([A-F\d]{2})");
            if (match.Groups.Count == 0 || match.Groups[0].Value == "") return (null);
            else
            {
                if (GPS.ComputerVerify(nmea) != match.Groups[match.Groups.Count - 1].Value) return (null);
                GPS_GGA gga = new GPS_GGA();
                gga._time = DateTime.ParseExact(match.Groups[2].Value, "HHmmss", null);
                switch (match.Groups[1].Value)
                {
                    case "$GNGGA": gga._gpsmode = eGPSMode.GN; break;
                    case "$GPGGA": gga._gpsmode = eGPSMode.GP; break;
                    case "$BDGGA": gga._gpsmode = eGPSMode.BD; break;
                    case "$GAGGA": gga._gpsmode = eGPSMode.GA; break;
                    case "$GLGGA": gga._gpsmode = eGPSMode.GL; break;
                }
                gga._lat = (int)float.Parse(match.Groups[3].Value.Substring(0, 2))
                    + Convert.ToSingle(match.Groups[3].Value.Substring(2)) / 60;
                gga._elat = (eLat)Enum.Parse(typeof(eLat), match.Groups[4].Value);
                gga._lng = (int)float.Parse(match.Groups[5].Value.Substring(0, 3))
                    + Convert.ToSingle(match.Groups[5].Value.Substring(3)) / 60;
                gga._elng = (eLng)Enum.Parse(typeof(eLng), match.Groups[6].Value);
                gga._statu = (eGGA_Statu)Convert.ToInt16(match.Groups[7].Value);
                gga._satelliteCount = match.Groups[8].Value != "" ? Convert.ToInt16(match.Groups[8].Value) : 0;
                gga._HDOP = match.Groups[9].Value != "" ? Convert.ToSingle(match.Groups[9].Value) : 0;
                gga._altitude = match.Groups[10].Value != "" ? Convert.ToSingle(match.Groups[10].Value) : 0;
                gga._altitudeUnit = match.Groups[11].Value != "" ? match.Groups[11].Value : "M";
                gga._geoIdHeight = match.Groups[12].Value != "" ? Convert.ToSingle(match.Groups[12].Value) : 0;
                gga._geoIdHeightUnit = match.Groups[13].Value != "" ? match.Groups[13].Value : "M";
                gga._rtcm = match.Groups[14].Value != "" ? Convert.ToInt16(match.Groups[14].Value) : 0;
                gga._gpsStationID = match.Groups[15].Value != "" ? Convert.ToInt16(match.Groups[15].Value) : 0;
                return (gga);
            }
        }
    }
    /// <summary>
    /// $GPGSA,A,3,01,20,19,13,,,,,,,,,40.4,24.4,32.2*0A
    /// GP/BD/GL/GA/GN
    /// </summary>
    public class GPS_GSA
    {
        private eGPSMode _gpsMode = eGPSMode.GP; //GP or BD or GL or GN or GA
        private eGSA_Mode _mode = eGSA_Mode.A;
        private eGSA_Fix _fix = eGSA_Fix.FixNo;
        private string[] _prn = new string[12];
        private float _PDOP, _HDOP, _VDOP;
        private eGPSFlag _gpsFlag = eGPSFlag.None;
        private eVersion _version = eVersion.V1;

        public eVersion Version { get => _version; set => _version = value; }
        public eGPSMode GPSMode { get => _gpsMode; set => _gpsMode = value; }
        public eGPSFlag GPSFlag { get => _gpsFlag; set => _gpsFlag = value; }
        /// <summary>
        /// 定位模式，A=自动手动2D/3D，M=手动2D/3D
        /// </summary>
        public eGSA_Mode EMode { get => _mode; }
        /// <summary>
        /// 定位类型，1=未定位，2=2D定位，3=3D定位
        /// </summary>
        public eGSA_Fix ECategory { get => _fix; }
        /// <summary>
        /// 按顺序第N信道正在使用的卫星PRN码编号
        /// </summary>
        public string[] Prn { get => _prn; }
        /// <summary>
        /// PDOP综合位置精度因子（0.5 - 99.9）
        /// </summary>
        public float PDOP { get => _PDOP; set => _PDOP = value; }
        /// <summary>
        /// HDOP水平精度因子（0.5 - 99.9）
        /// </summary>
        public float HDOP { get => _HDOP; set => _HDOP = value; }
        /// <summary>
        /// VDOP垂直精度因子（0.5 - 99.9）
        /// </summary>
        public float VDOP { get => _VDOP; set => _VDOP = value; }
        public string NMEA
        {
            get
            {
                string nmea = "";
                switch (this.GPSMode)
                {
                    case eGPSMode.GP: nmea += "GP"; break;
                    case eGPSMode.BD: nmea += "BD"; break;
                    case eGPSMode.GL: nmea += "GL"; break;
                    case eGPSMode.GN: nmea += "GN"; break;
                    case eGPSMode.GA: nmea += "GA"; break;
                }
                nmea += $"GSA,{(int)this._mode},{(int)this._fix}";
                for (int i = 0; i < this._prn.Length; i++)
                    nmea += $",{this._prn[i]}";
                nmea += $",{Math.Round(this._PDOP, 1).ToString()},{Math.Round(this._HDOP, 1).ToString()}" +
                    $",{Math.Round(this._VDOP, 1).ToString()}";
                if (this.GPSFlag != eGPSFlag.None) nmea += $",{(int)this.GPSFlag}";
                nmea = $"${nmea}*{GPS.ComputerVerify(nmea)}";
                return (nmea);
            }
        }

        /// <summary>
        /// $GPGSA,A,3,01,20,19,13,,,,,,,,,40.4,24.4,32.2*0A
        /// GP/BD
        /// </summary>
        /// <param name="nmea"></param>
        /// <param name="gpsmode"></param>
        /// <returns></returns>
        public static bool IsGSA(string nmea, eGPSMode gpsmode)
        {
            string partten = "";
            switch (gpsmode)
            {
                case eGPSMode.BD: partten = @"\$BDGSA"; break;
                case eGPSMode.GP: partten = @"\$GPGSA"; break;
                case eGPSMode.GL: partten = @"\$GLGSA"; break;
                case eGPSMode.GN: partten = @"\$GNGSA"; break;
                case eGPSMode.GA: partten = @"\$GAGSA"; break;
            }
            return (Regex.IsMatch(nmea, partten));
        }

        public static GPS_GSA Parse(string nmea)
        {
            eVersion ver = eVersion.V1;
            string partten = @"(\$[A-Z]{2}GSA),([A|M]{0,1}),([1-3]{0,1})";
            for (int i = 0; i < 12; i++) //_prn[12]
                partten += @",(\d{0,2})";
            partten += @",(\d{0,2}\.{0,1}\d{0,1}),(\d{0,2}\.{0,1}\d{0,1}),(\d{0,2}\.{0,1}\d{0,1})\*([A-F\d]{2})";
            //{ [$GPGSA,A,3,01,20,19,13,,,,,,,,,40.4,24.4,32.2*0A], [$GPGSA], [A], [3], [01], [20], [19], [13], [], [], [], [], [], [], [], [], [40.4], [24.4], [32.2], [0A] }
            Match match = Regex.Match(nmea, partten);
            if (match.Groups.Count == 0 || match.Groups[0].Value == "") //兼容新版，增加了一个字段
            {
                ver = eVersion.V2;
                partten = @"(\$[A-Z]{2}GSA),([A|M]{0,1}),([1-3]{0,1})";
                for (int i = 0; i < 12; i++) //_prn[12]
                    partten += @",(\d{0,2})";
                partten += @",(\d{0,2}\.{0,1}\d{0,1}),(\d{0,2}\.{0,1}\d{0,1}),(\d{0,2}\.{0,1}\d{0,1}),{\d{1}}\*([A-F\d]{2})";
                match = Regex.Match(nmea, partten);
            }
            if (match.Groups.Count == 0 || match.Groups[0].Value == "") return (null);
            else
            {
                if (GPS.ComputerVerify(nmea) != match.Groups[match.Groups.Count - 1].Value) return (null);
                GPS_GSA gsa = new GPS_GSA();
                gsa.Version = ver;
                switch (match.Groups[1].Value)
                {
                    case "$GPGSA": gsa._gpsMode = eGPSMode.GP; break;
                    case "$BDGSA": gsa._gpsMode = eGPSMode.BD; break;
                    case "$GLGSA": gsa._gpsMode = eGPSMode.GL; break;
                    case "$GNGSA": gsa._gpsMode = eGPSMode.GN; break;
                }
                gsa._mode = (eGSA_Mode)Enum.Parse(typeof(eGSA_Mode), match.Groups[2].Value);
                gsa._fix = (eGSA_Fix)Convert.ToInt16(match.Groups[3].Value);
                for (int i = 0; i < gsa._prn.Length; i++)
                    gsa._prn[i] = match.Groups[i + 4].Value.ToString();
                gsa._PDOP = match.Groups[16].Value != "" ? Convert.ToSingle(match.Groups[16].Value) : 0;
                gsa._HDOP = match.Groups[17].Value != "" ? Convert.ToSingle(match.Groups[17].Value) : 0;
                gsa._VDOP = match.Groups[18].Value != "" ? Convert.ToSingle(match.Groups[18].Value) : 0;
                if (gsa.Version == eVersion.V2)
                    gsa._gpsFlag = (eGPSFlag)(match.Groups[match.Groups.Count - 2].Value != "" 
                        ? Convert.ToInt16(match.Groups[match.Groups.Count - 2].Value) : 0);
                return (gsa);
            }
        }
    }
    /// <summary>
    /// $GPGSV,3,3,11,20,36,255,28,25,21,262,23,29,39,315,07*44
    /// $BDGSV,4,1,13,01,34,128,30,02,47,214,24,03,51,168,31,04,,,34*58
    /// 没有GNGSV
    /// </summary>
    public class GPS_GSV
    {
        private eGPSMode _gpsMode = eGPSMode.GP; //GP or BD or GL or GA
        //private int _satelliteCount = 0;
        private Dictionary<int, Satellite> _satellites = new Dictionary<int, Satellite>();        
        private eGPSFlag _gpsFlag = eGPSFlag.None;
        private eVersion _version = eVersion.V1;

        public eVersion Version { get => _version; set => _version = value; }
        public eGPSMode GPSMode { get => _gpsMode; set => _gpsMode = value; }
        public eGPSFlag GPSFlag { get => _gpsFlag; set => _gpsFlag = value; }

        /// <summary>
        /// 卫星数
        /// </summary>
        public int SatelliteCount { get => _satellites.Count; }
        public Dictionary<int, Satellite> Satellites { get => _satellites; }

        public string[] NMEA
        {
            get
            {
                string[] nmea = new string[(int)Math.Round(this.Satellites.Count / 4F + 0.5, 0)];
                for (int i = 0; i < nmea.Length; i++)
                {
                    nmea[i] = "";
                    switch (this.GPSMode)
                    {
                        case eGPSMode.GP: nmea[i] += "GP"; break;
                        case eGPSMode.BD: nmea[i] += "BD"; break;
                        case eGPSMode.GL: nmea[i] += "GL"; break;
                        case eGPSMode.GA: nmea[i] += "GA"; break;
                    }
                    nmea[i] += $"GSV,{nmea.Length},{i + 1}";
                    for (int j = 0; j < 4; j++)
                    {
                        int idx = i * 4 + j;
                        if (idx > this.Satellites.Count - 1) break;
                        nmea[i] += $",{this.Satellites[i].Prn},{this.Satellites[i].Elevation}" +
                            $",{this.Satellites[i].Azimuth},{this.Satellites[i].Snr}";
                    }
                    if (this.GPSFlag != eGPSFlag.None) nmea[i] += $",{(int)this.GPSFlag}";
                    nmea[i] = $"${nmea[i]}*{GPS.ComputerVerify(nmea[i])}";
                }
                return (nmea);
            }
        }
        /// <summary>
        /// $GPGSV,3,3,11,20,36,255,28,25,21,262,23,29,39,315,07*44
        /// $BDGSV,4,1,13,01,34,128,30,02,47,214,24,03,51,168,31,04,,,34*58
        /// GP/BD
        /// </summary>
        /// <param name="nmea"></param>
        /// <param name="cate"></param>
        /// <returns></returns>
        public static bool IsGSV(string nmea, eGPSMode cate)
        {
            string partten = "";
            switch (cate)
            {
                case eGPSMode.BD: partten = @"\$BDGSV"; break;
                case eGPSMode.GP: partten = @"\$GPGSV"; break;
                case eGPSMode.GL: partten = @"\$GLGSV"; break;
                case eGPSMode.GA: partten = @"\$GAGSV"; break;
            }
            return (Regex.IsMatch(nmea, partten));
        }
        /// <summary>
        /// 因有多条GSV语句，因此需要Parse多次；GPGSV与BDGSV不能解析到一起，如果不一样，返回null
        /// Parse每次解析一句nmea，先判断下是否是GSV:IsGSV(nmea,eCategory)
        /// </summary>
        /// <param name="nmea"></param>
        /// <param name="gsv"></param>
        /// <returns></returns>
        public static GPS_GSV Parse(string nmea, GPS_GSV gsv)
        {
            eVersion ver = eVersion.V1;
            Match match = null; string regstr = "";
            for (int i = 0; i < 4; i++)
            {
                regstr += @",(\d{1,2}),(\d{0,2}),(\d{0,3}),(\d{0,2})";
                match = Regex.Match(nmea,
                    @"(\$[A-Z]{2}GSV),(\d{1}),(\d{1}),(\d{1,2})" + regstr + @"\*([A-F\d]{2})");
                if (match.Groups.Count > 0 && match.Groups[0].Value != "") break;
            }
            if (match.Groups.Count == 0 || match.Groups[0].Value == "")
            {
                ver = eVersion.V2;
                regstr = "";
                for (int i = 0; i < 4; i++)
                {
                    regstr += @",(\d{1,2}),(\d{0,2}),(\d{0,3}),(\d{0,2})";
                    match = Regex.Match(nmea,
                        @"(\$[A-Z]{2}GSV),(\d{1}),(\d{1}),(\d{1,2})" + regstr + @",(\d{1})\*([A-F\d]{2})");
                    if (match.Groups.Count > 0 && match.Groups[0].Value != "") break;
                }
            }
            if (match.Groups.Count == 0 || match.Groups[0].Value == "") return (gsv);
            if (GPS.ComputerVerify(nmea) != match.Groups[match.Groups.Count - 1].Value) return (gsv);
            if (gsv != null && $"${gsv._gpsMode.ToString().Substring(0, 2)}GSV" != match.Groups[1].Value) return (gsv);
            if (gsv == null)
            {
                gsv = new GPS_GSV();
                gsv.Version = ver;
                switch (match.Groups[1].Value)
                {
                    case "$GPGSV": gsv._gpsMode = eGPSMode.GP; break;
                    case "$BDGSV": gsv._gpsMode = eGPSMode.BD; break;
                    case "$GLGSV": gsv._gpsMode = eGPSMode.GL; break;
                    case "$GAGSV": gsv._gpsMode = eGPSMode.GA; break;
                }
            }
            //gsv._satelliteCount = Convert.ToInt32(match.Groups[4].Value);
            for (int i = 5; i <= match.Groups.Count - (gsv.Version == eVersion.V1 ? 2 : 3); i += 4)
                if (!gsv.Satellites.ContainsKey(Convert.ToInt32(match.Groups[i + 0].Value)))
                    gsv.Satellites.Add(Convert.ToInt32(match.Groups[i + 0].Value),
                        new Satellite(Convert.ToInt32(match.Groups[i + 0].Value)
                            , match.Groups[i + 1].Value != "" ? Convert.ToInt32(match.Groups[i + 1].Value) : 0
                            , match.Groups[i + 2].Value != "" ? Convert.ToInt32(match.Groups[i + 2].Value) : 0
                            , match.Groups[i + 3].Value != "" ? Convert.ToInt32(match.Groups[i + 3].Value) : 0)
                        );

            if (gsv.Version == eVersion.V2)
                gsv._gpsFlag = (eGPSFlag)(match.Groups[match.Groups.Count - 2].Value != ""
                    ? Convert.ToInt16(match.Groups[match.Groups.Count - 2].Value) : 0);
            return (gsv);
        }

        public GPS_GSV() { }
        public GPS_GSV(eGPSMode gpsmode) { this._gpsMode = gpsmode; }

        public Satellite AddSatellite(Satellite satellite)
        {
            this._satellites.Add(satellite.Prn, satellite);return (satellite);
        }
        public Satellite AddSatellite(int prn, int snr)
        {
            Satellite satellite = new Satellite(prn, snr);
            return (this.AddSatellite(satellite));
        }
        public Bitmap ToBMP()
        {
            return (this.ToBMP(240, 60));
        }
        public Bitmap ToBMP(int width, int height)
        {
            if (width < 240) width = 240;
            if (height < 60) height = 60;
            Bitmap bmp = new Bitmap(width, height);
            bmp.MakeTransparent(Color.FromArgb(255, 0x00, 0x00, 0x00));
            Rectangle Canvas = new Rectangle(0, 0, width, height);
            //Graphics gb = Graphics.FromImage(bmp);
            //BufferedGraphics bufferGraphics = BufferedGraphicsManager.Current.Allocate(gb, Canvas);
            //Graphics g = bufferGraphics.Graphics;
            Graphics g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality; //高像素偏移质量            
            
            Font baseFont = new Font("宋体", 10);
            Color baseFontColor = Color.White;
            Color baseBgColor = Color.FromArgb(255, 0x00, 0x00, 0x00);
            Color baseFrameColor = Color.FromArgb(128, 0xE4, 0xE4, 0xE4);
            Color baseGPSColor0 = Color.Green;
            Color baseGPSColor1 = Color.Gray;
            Color baseGPSFontColor = Color.Yellow;
            Color baseTipColor = Color.Red;

            g.FillRectangle(new SolidBrush(baseBgColor), new RectangleF(0, 0, bmp.Width, bmp.Height));
            Size baseFontSize = g.MeasureString("abc", baseFont).ToSize();
            int padding = 2;
            Canvas = new Rectangle(padding, padding, width - padding * 2, height - padding * 2);

            Rectangle rectPrn = new Rectangle(Canvas.X, Canvas.Bottom - baseFontSize.Height - padding * 2
                , Canvas.Width, baseFontSize.Height + padding * 2);
            g.DrawRectangle(new Pen(baseFrameColor), rectPrn);
            Rectangle rectSnr = new Rectangle(Canvas.X, Canvas.Y, Canvas.Width, rectPrn.Top - Canvas.Y);
            float unit = Canvas.Width * 1F / this._satellites.Count;
            int idx = 0;
            foreach (KeyValuePair<int, Satellite> itm in this._satellites)
            {
                //先写Prn卫星编号
                RectangleF rect = new RectangleF(rectPrn.X + idx * unit, rectPrn.Y, unit, rectPrn.Height);
                g.DrawString(itm.Key.ToString(), baseFont, new SolidBrush(baseFontColor), rect);
                //再画卫星SNR图
                float snrH = rectSnr.Height * itm.Value.Snr / 100F;
                rect = new RectangleF(rectSnr.X + idx * unit, rectSnr.Y + rectSnr.Height - snrH, unit, snrH);
                g.FillRectangle(new SolidBrush(
                    itm.Value.Snr >= 30 ? baseGPSColor0 : baseGPSColor1
                    ), rect);
                rect = new RectangleF(rectSnr.X + idx * unit, rectSnr.Y, unit, rectSnr.Height);
                g.DrawRectangle(new Pen(baseFrameColor), Rectangle.Round(rect));
                rect = new RectangleF(rect.X, rect.Y + (rect.Height - baseFontSize.Height) / 2F
                    , rect.Width, baseFontSize.Height);
                g.DrawString(itm.Value.Snr.ToString().PadLeft(2, '0'), baseFont
                    , new SolidBrush(baseGPSFontColor), rect);
                idx++;
            }
            g.DrawString(this._gpsMode.ToString(), baseFont, new SolidBrush(baseTipColor)
                , new Point(rectSnr.X + padding, rectSnr.Y + padding));
            //bufferGraphics.Render(gb);
            g.Dispose(); //bufferGraphics.Dispose();
            return (bmp);
        }

        public class Satellite
        {
            private int _prn = 0;
            private int _elevation = 0;
            private int _azimuth = 0;
            private int _snr = 0;

            public Satellite(int prn, int elevation, int azimuth, int snr)
            {
                this._prn = prn;
                this._elevation = elevation;
                this._azimuth = azimuth;
                this._snr = snr;
            }

            public Satellite(int prn, int snr) : this(prn, 0, 0, snr) { }

            /// <summary>
            /// 卫星号
            /// </summary>
            public int Prn { get => _prn; }
            /// <summary>
            /// 仰角0-90
            /// </summary>
            public int Elevation { get => _elevation; }
            /// <summary>
            /// 方位角0-359
            /// </summary>
            public int Azimuth { get => _azimuth; }
            /// <summary>
            /// 信噪比0-99
            /// </summary>
            public int Snr { get => _snr; }
        }

    }
    /// <summary>
    /// $GNRMC,060440.000,A,3251.5359,N,10338.6840,E,0.00,302.31,151016,,,A*7B
    /// </summary>
    public class GPS_RMC : GPSPoint
    {
        private eGPSMode _gpsMode = eGPSMode.GP;
        private DateTime _time;
        private eValid _valid = eValid.V;
        private float _speed; //公里
        private float _bearing;
        private float _magnetic = 0;
        private eMagnetic _eMagnetic = eMagnetic.E;
        private eRMC_Mode _eMode = eRMC_Mode.A;
        private eNaviStatu _naviStatu = eNaviStatu.S;
        private eVersion _version = eVersion.V1;

        public GPS_RMC(float lng, float lat) : base(lng, lat) { }
        public GPS_RMC() { }

        public eGPSMode GPSMode { get => _gpsMode; }
        public eNaviStatu NaviStatu { get => _naviStatu; set => _naviStatu = value; }
        public eVersion Version { get => _version; set => _version = value; }
        public new DateTime Time { get => _time; set => _time = value; }
        public eValid Valid { get => _valid; set => _valid = value; }        
        /// <summary>
        /// 公里每小时
        /// </summary>
        public new float Speed { get => _speed; set => _speed = value; }
        public float Bearing { get => _bearing; }
        public eMagnetic EMagnetic { get => _eMagnetic; }
        public float Magnetic { get => _magnetic; }
        public eRMC_Mode EMode { get => _eMode; }

        private new float ImpulseSpeed; //隐藏GPSPoint属性

        public string NMEA
        {
            get
            {
                string nmea = "";
                switch (this.GPSMode)
                {
                    case eGPSMode.GP: nmea += "GP"; break;
                    case eGPSMode.BD: nmea += "BD"; break;
                    case eGPSMode.GN: nmea = "GN"; break;
                    case eGPSMode.GA: nmea = "GA"; break;
                    case eGPSMode.GL: nmea = "GL"; break;
                }
                string latd = ((int)this.Lat).ToString().PadLeft(2, '0');
                string latm = Math.Round((this.Lat - (int)this.Lat) * 60, 4).ToString();
                int latp = latm.IndexOf("."); if (latp < 0) latp = latm.Length;
                string latm0 = latm.Substring(0, latp).PadLeft(2, '0');
                string latm1 = "";
                if (latp == latm.Length) latm1 = "0000";
                else latm1 = latm.Substring(latp + 1).PadRight(4, '0');
                string latstr = latd + latm0 + "." + latm1;

                string lngd = ((int)this.Lng).ToString().PadLeft(3, '0');
                string lngm = Math.Round((this.Lng - (int)this.Lng) * 60, 4).ToString();
                int lngp = lngm.IndexOf("."); if (lngp < 0) lngp = lngm.Length;
                string lngm0 = lngm.Substring(0, lngp).PadLeft(2, '0');
                string lngm1 = "";
                if (lngp == lngm.Length) lngm1 = "0000";
                else lngm1 = lngm.Substring(lngp + 1).PadRight(4, '0');
                string lngstr = lngd + lngm0 + "." + lngm1;

                float sp = (float)Math.Round(this.Speed / 1.852, 1);
                string sp0 = ((int)sp).ToString().PadLeft(3, '0');
                string sp1 = Math.Round(sp - (int)sp, 1).ToString();
                if (sp1.Length >= 3) sp1 = sp1.Substring(2, 1);

                float br = (float)Math.Round(this.Bearing, 1);
                string br0 = ((int)br).ToString().PadLeft(3, '0');
                string br1 = Math.Round(br - (int)br, 1).ToString();
                if (br1.Length >= 3) br1 = br1.Substring(2, 1);

                float ma = (float)Math.Round(this.Magnetic, 1);
                string ma0 = ((int)ma).ToString().PadLeft(3, '0');
                string ma1 = Math.Round(ma - (int)ma, 1).ToString();
                if (ma1.Length >= 3) ma1 = ma1.Substring(2, 1);

                nmea += $"RMC,{this.Time.ToString("HHmmss")}.000,{this.Valid.ToString()},"
                    + $"{latstr},{this.ELat.ToString()},"
                    + $"{lngstr},{this.ELng.ToString()},"
                    + $"{sp0 + "." + sp1},"
                    + $"{br0 + "." + br1},{this.Time.ToString("ddMMyy")},"
                    + $"{ma0 + "." + ma1},"
                    + $"{this.EMagnetic.ToString()},{this.EMode.ToString()}";
                //V2版本增加导航状态
                nmea += $",{this.NaviStatu.ToString()}";
                string verify = GPS.ComputerVerify(nmea);
                nmea = $"${nmea}*{verify}";
                return (nmea);
            }
        }
        
        /// <summary>
        /// $GNRMC,060440.000,A,3251.5359,N,10338.6840,E,0.00,302.31,151016,,,A*7B
        /// GN/BD/GP
        /// </summary>
        /// <param name="nmea"></param>
        /// <param name="cate"></param>
        /// <returns></returns>
        public static bool IsRMC(string nmea, eGPSMode cate)
        {
            string partten = "";
            switch (cate)
            {
                case eGPSMode.BD: partten = @"\$BDRMC"; break;
                case eGPSMode.GP: partten = @"\$GPRMC"; break;
                case eGPSMode.GN: partten = @"\$GNRMC"; break;
                case eGPSMode.GA: partten = @"\$GARMC"; break;
                case eGPSMode.GL: partten = @"\$GLRMC"; break;
            }
            return (Regex.IsMatch(nmea, partten));
        }
        public static bool IsRMC(string nmea)
        {
            return (IsRMC(nmea, eGPSMode.GP) || IsRMC(nmea, eGPSMode.BD) || IsRMC(nmea, eGPSMode.GN));
        }

        public static GPS_RMC Parse(string nmea)
        {
            eVersion ver = eVersion.V1;
            //partten多了一个全部的总括号，所以Groups的序号往后偏了一位
            //Groups的第0，1元素均为整句nmea
            Match match = Regex.Match(nmea,
                @"((\$[A-Z]{2}RMC),(\d{6})\.\d{0,3},([A|V]),(\d{4}\.\d+),([NS]{1}),(\d{5}\.\d+),([EW]{1}),(\d*\.*\d*),(\d*\.*\d*),(\d{6}),(\d*\.*\d*),([EW]{0,1}),([ADEN]{0,1})\*([A-F\d]{2}))");
            if (match.Groups.Count == 0 || match.Groups[0].Value == "")
            {
                ver = eVersion.V2;
                match = Regex.Match(nmea,
                    @"((\$[A-Z]{2}RMC),(\d{6})\.\d{0,3},([A|V]),(\d{4}\.\d+),([NS]{1}),(\d{5}\.\d+),([EW]{1}),(\d*\.*\d*),(\d*\.*\d*),(\d{6}),(\d*\.*\d*),([EW]{0,1}),([ADEN]{0,1}),([SCUV]{1})\*([A-F\d]{2}))");
            }

            if (match.Groups.Count == 0 || match.Groups[0].Value == "") return (null);
            else
            {
                if (GPS.ComputerVerify(nmea) != match.Groups[match.Groups.Count - 1].Value) return (null);
                GPS_RMC rmc = new GPS_RMC();
                rmc.Version = ver;
                rmc._time = DateTime.ParseExact(match.Groups[11].Value + match.Groups[3].Value, "ddMMyyHHmmss", null);
                switch (match.Groups[2].Value)
                {
                    case "$GNRMC": rmc._gpsMode = eGPSMode.GN; break;
                    case "$GPRMC": rmc._gpsMode = eGPSMode.GP; break;
                    case "$BDRMC": rmc._gpsMode = eGPSMode.BD; break;
                    case "$GARMC": rmc._gpsMode = eGPSMode.GA; break;
                    case "$GLRMC": rmc._gpsMode = eGPSMode.GL; break;
                }
                rmc._valid = match.Groups[4].Value == "A" ? eValid.A : eValid.V;
                rmc._lat = (int)float.Parse(match.Groups[5].Value.Substring(0, 2))
                    + Convert.ToSingle(match.Groups[5].Value.Substring(2)) / 60;
                rmc._eLat = (eLat)Enum.Parse(typeof(eLat), match.Groups[6].Value);
                rmc._lng = (int)float.Parse(match.Groups[7].Value.Substring(0, 3))
                    + Convert.ToSingle(match.Groups[7].Value.Substring(3)) / 60;
                rmc._eLng = (eLng)Enum.Parse(typeof(eLng), match.Groups[8].Value);
                rmc._speed = match.Groups[9].Value != "" ? (float.Parse(match.Groups[9].Value) * 1.852F) : 0;
                rmc._bearing = match.Groups[10].Value != "" ? float.Parse(match.Groups[10].Value) : 0;
                rmc._magnetic = match.Groups[12].Value != "" ? float.Parse(match.Groups[12].Value) : 0;
                rmc._eMagnetic = match.Groups[13].Value != "" ?
                    (eMagnetic)Enum.Parse(typeof(eMagnetic), match.Groups[13].Value) : eMagnetic.E;
                rmc._eMode = match.Groups[14].Value != "" ?
                    (eRMC_Mode)Enum.Parse(typeof(eRMC_Mode), match.Groups[14].Value) : eRMC_Mode.A;
                if (rmc.Version == eVersion.V2)
                {
                    rmc.NaviStatu= (eNaviStatu)Enum.Parse(typeof(eNaviStatu), match.Groups[match.Groups.Count - 2].Value);
                }
                return (rmc);
            }
        }

    }

    public class GPSPoint
    {
        internal float _lat, _lng;
        internal eLat _eLat; internal eLng _eLng;
        /// <summary>
        /// 只返回正值，不带方向
        /// </summary>
        public float Lat
        {
            get => _lat; set
            {
                this._lat = Math.Abs(value);
                this._eLat = value < 0 ? eLat.S : eLat.N;
            }
        }
        public float getLat()
        {
            return (this._lat * (_eLat == eLat.S ? -1 : 1));
        }
        public float getLng()
        {
            return (this._lng * (_eLng == eLng.W ? -1 : 1));
        }
        /// <summary>
        /// 只返回正值，不带方向
        /// </summary>
        public float Lng
        {
            get => _lng; set
            {
                this._lng = Math.Abs(value);
                this._eLng = value < 0 ? eLng.W : eLng.E;
            }
        }
        public eLat ELat { get => _eLat; }
        public eLng ELng { get => _eLng; }
        public float Speed = 0;
        public float ImpulseSpeed = 0;
        public DateTime Time;
        public int id = 0;
        public object data = null;

        public GPSPoint() { }
        public GPSPoint(float lng, float lat)
        {
            this.Lat = lat; this.Lng = lng;
        }

        /// <summary>latlng距本点的距离，单位公里
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public double getDistance(GPSPoint pt)
        {
            const double EARTH_RADIUS = 6378.137;
            double a = rad(pt.getLat()) - rad(this.getLat());
            double b = rad(pt.getLng()) - rad(this.getLng());
            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +
                Math.Cos(rad(this.getLat())) * Math.Cos(rad(pt.getLat())) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * EARTH_RADIUS;
            return (s);
        }
        private double rad(double d)
        {
            return d * Math.PI / 180.0;
        }

        public bool IsInPolygon(GPSPoint[] polygon)
        {
            int j = polygon.Length - 1;
            bool isIn = false;
            for (int i = 0; i < polygon.Length; i++)
            {
                if (polygon[i].getLng() < this.getLng() && polygon[j].getLng() >= this.getLng()
                    || polygon[j].getLng() < this.getLng() && polygon[i].getLng() >= this.getLng())
                {
                    if (polygon[i].getLat() + (this.getLng() - polygon[i].getLng())
                        / (polygon[j].getLng() - polygon[i].getLng()) * (polygon[j].getLat() - polygon[i].getLat()) 
                        < this.getLat())
                        isIn = !isIn;
                }
                j = i;
            }
            return (isIn);
        }

        public bool IsInRectangle(GPSPoint pt0, GPSPoint pt1)
        {
            GPSPoint[] pts = new GPSPoint[4] { pt0, null, pt1, null };
            pts[1] = new GPSPoint(pt1.getLng(), pt0.getLat());
            pts[3] = new GPSPoint(pt0.getLng(), pt1.getLat());
            return (this.IsInPolygon(pts));
        }

    }
}
