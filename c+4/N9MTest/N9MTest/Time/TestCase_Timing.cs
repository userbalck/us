using System;
using NUnit.Framework;
using N9MTest.SDK.net;
using Newtonsoft.Json.Linq;
using Util;
using System.Threading;
using System.Collections.Generic;
using RM;
using N9MTest.commons;
using System.Diagnostics;

namespace N9MTest.Time
{
	[TestFixture]
	public class TestCase_Timing : TestCase_Basecase
	{
        //操作步骤
        //[预置环境]
        //1、首先关闭所有的校时 将系统时间设定为错误的时间点上[2002.08.18.00.00.00]使用协议SETCTRLUTC
        //2、获取当前的时间 确保当前设备系统时间跟正确的时间相差比较大。

        //[执行测试步骤]
        //1、设定中心服务器校时
        //2、下发校时指令

        //[测试结果]
        //取设备时间与服务器时间误差在2秒内则成功

	[Ignore("11111")]
        [Test]
        public void TimeSyncSever()
        { 
            //中心服务器IP
            string csip = "192.168.6.124";
            int csport = 7300;

            csip = this.CFG.gNode("Time/TimeSyncSever/csip").InnerText;
            csport = Convert.ToInt32(this.CFG.gNode("Time/TimeSyncSever/csport").InnerText);

            Console.WriteLine("csip ={0}", csip);
            Console.WriteLine("csport ={0}", csport);


            foreach (string ip in IPList)
            {
                Console.WriteLine("ip ={0}", ip);

                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                JObject jresp = null;

                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();

                JObject jatp = new JObject();
                jatp.Add("GE", 0);
                jatp.Add("NE", 0);
                jatp.Add("CE", 0);
                jmdvr.Add("ATP", jatp);

                jparameter.Add("MDVR", jmdvr);

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"]), "关闭所有校时机制[gps. ntp. center server].错误码为{0}", Convert.ToInt32(jresp["ERRORCODE"]));

                long utctime = ExactTime.GetUTCTimeStamp(new DateTime(2002,08,18, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second));

                jparameter = new JObject();
                jparameter.Add("CURT", utctime);
                jparameter.Add("Z", "480A");

                jresp = session.SendCommand(Module.DEVEMM, Operation.SETCTRLUTC, jparameter);

                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"]), "初始化UTC时间设定(1029632400)失败.错误码为{0}", Convert.ToInt32(jresp["ERRORCODE"]));

                jresp = session.SendCommand(Module.DEVEMM, Operation.GETCTRLUTC, null);

                long curt = Convert.ToInt64(jresp["CURT"].ToString());

                //           Assert.IsTrue(curt == 1029632400, "初始化UTC(1029632400)时间失败");

                //首先获取中心服务器
                jparameter = new JObject();
                jmdvr = new JObject();
                JArray jarraysp = new JArray();
                JObject jsp = new JObject();
                JObject jmcms = new JObject();

                //协议类型[；0：默认N9，1:智达2: 808]
                jsp.Add("CP", 0);

                //中心信令服务器端口
                jsp.Add("CPORT", csport);

                //中心信令服务器地址
                jsp.Add("CS", csip);

                //当选择808 协议时， 作为主服务器的UDP 端口
          //      jsp.Add("CUPORT", 6222);

                //是否开启服务器
                jsp.Add("EN", 1);

                //媒体服务器端口
       //         jsp.Add("MPORT", 7300);

                //媒体服务器
       //         jsp.Add("MS", "192.168.6.124");


       //         jsp.Add("MUPORT", 6111);

                //网络类型
                jsp.Add("NWT", 0);
                jarraysp.Add(jsp);

                jmcms.Add("SP", jarraysp);
                jmdvr.Add("MCMS", jmcms);
                jparameter.Add("MDVR", jmdvr);
                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                //开启中心服务器校时
                jparameter = new JObject();
                jmdvr = new JObject();
                jatp = new JObject();
                jatp.Add("GE", 0);
                jatp.Add("NE", 0);
                jatp.Add("CE", 1);
                jatp.Add("SID", 0);
                jmdvr.Add("ATP", jatp);

                jparameter.Add("MDVR", jmdvr);

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"]), "开启CenterServer校时失败.错误码为{0}", Convert.ToInt32(jresp["ERRORCODE"]));

                Sleep(30000);

                jresp = session.SendCommand(Module.DEVEMM, Operation.GETCTRLUTC, null);

                Assert.IsNotNull(jresp["CURT"], "校时完毕指令下发后 重新获取UTC时间失败");
                Console.WriteLine("DateTime.UtcNow.ToUniversalTime = {0}", ExactTime.GetUTCTimeStamp());

                curt = Convert.ToInt64(jresp["CURT"].ToString());

                long utc = ExactTime.GetUTCTimeStamp();

                Assert.IsTrue(Math.Abs(utc - curt) <= 5, "校时失败 设备当前UTC时间 {0}  服务器UTC时间{1} 时间差值{2}", curt, utc, utc - curt);

                session.Logout();
            }
        }
		
		[Test]
        public void TimeSyncNTP()
        {
            foreach (string ip in IPList)
            {
                Console.WriteLine("ip ={0}", ip);

                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                JObject jresp = null;

                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();

                JObject jatp = new JObject();
                jatp.Add("GE", 0);
                jatp.Add("NE", 0);
                jatp.Add("CE", 0);
                jmdvr.Add("ATP", jatp);

                jparameter.Add("MDVR", jmdvr);

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"]), "关闭所有校时机制[gps. ntp. center server].错误码为{0}", Convert.ToInt32(jresp["ERRORCODE"]));

                DateTime dtUTC = new DateTime(2002, 08, 18, 08, 30, 00, DateTimeKind.Utc);

                jparameter = new JObject();
                jparameter.Add("CURT", (int)(dtUTC - new DateTime(1970,1,1)).TotalSeconds);

                jresp = session.SendCommand(Module.DEVEMM, Operation.SETCTRLUTC, jparameter);

                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"]), "初始化UTC时间设定(2012/08/18 08:30:00)失败.错误码为{0}", Convert.ToInt32(jresp["ERRORCODE"]));

                jresp = session.SendCommand(Module.DEVEMM, Operation.GETCTRLUTC, null);

                long curt = Convert.ToInt64(jresp["CURT"].ToString());

                //           Assert.IsTrue(curt == 1029632400, "初始化UTC(1029632400)时间失败");

                //开启NTP服务器校时
                jparameter = new JObject();
                jmdvr = new JObject();
                jatp = new JObject();
                jatp.Add("GE", 0);
                jatp.Add("NE", 1);
                jatp.Add("CE", 0);
                jmdvr.Add("ATP", jatp);

                jparameter.Add("MDVR", jmdvr);

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"]), "开启ntp校时失败.错误码为{0}", Convert.ToInt32(jresp["ERRORCODE"]));


                int nCount = 10;
                do
                {
                    jresp = session.SendCommand(Module.DEVEMM, Operation.GETCTRLUTC, null);

                    Assert.IsNotNull(jresp["CURT"], "校时完毕指令下发后 重新获取UTC时间失败");
                    Console.WriteLine("DateTime.UtcNow.ToUniversalTime = {0}", ExactTime.GetUTCTimeStamp());

                    curt = Convert.ToInt64(jresp["CURT"].ToString());

                    long utc = ExactTime.GetUTCTimeStamp();

                    Console.WriteLine("设备当前UTC时间 {0}  服务器UTC时间{1} 时间差值{2}", curt, utc, utc - curt);

                    if (Math.Abs(utc - curt) <= 5)
                    {
                        break;
                    }
                    else
                    {
                        if (nCount == 1)
                        {
                            Assert.IsTrue(Math.Abs(utc - curt) <= 5, "校时失败 设备当前UTC时间 {0}  服务器UTC时间{1} 时间差值{2}", curt, utc, utc - curt);
                        }
                        else
                        {
                            Sleep(10000);
                        }
                    }

                    

                } while (nCount-- > 0);

                

                session.Logout();
            }
        }

        [Test]
        public void TimeSyncGPS()
        {
            foreach (string ip in IPList)
            {
                Console.WriteLine("ip ={0}", ip);

                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                JObject jresp = null;

                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();

                JObject jatp = new JObject();
                jatp.Add("GE", 0);
                jatp.Add("NE", 0);
                jatp.Add("CE", 0);
                jmdvr.Add("ATP", jatp);

                jparameter.Add("MDVR", jmdvr);

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"]), "关闭所有校时机制[gps. ntp. center server].错误码为{0}", Convert.ToInt32(jresp["ERRORCODE"]));


                long utctime = ExactTime.GetUTCTimeStamp(new DateTime(2002, 08, 18, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second));

                jparameter = new JObject();
                jparameter.Add("CURT", utctime);
                jparameter.Add("Z", "480A");

                jresp = session.SendCommand(Module.DEVEMM, Operation.SETCTRLUTC, jparameter);

                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"]), "初始化UTC时间设定(1029632400)失败.错误码为{0}", Convert.ToInt32(jresp["ERRORCODE"]));

                jresp = session.SendCommand(Module.DEVEMM, Operation.GETCTRLUTC, null);

                long curt = Convert.ToInt64(jresp["CURT"].ToString());

                //           Assert.IsTrue(curt == 1029632400, "初始化UTC(1029632400)时间失败");

                //开启NTP服务器校时
                jparameter = new JObject();
                jmdvr = new JObject();
                jatp = new JObject();
                jatp.Add("GE", 1);
                jatp.Add("NE", 0);
                jatp.Add("CE", 0);
                jmdvr.Add("ATP", jatp);

                jparameter.Add("MDVR", jmdvr);

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"]), "开启ntp校时失败.错误码为{0}", Convert.ToInt32(jresp["ERRORCODE"]));

                Sleep(30000);

                jresp = session.SendCommand(Module.DEVEMM, Operation.GETCTRLUTC, null);

                Assert.IsNotNull(jresp["CURT"], "校时完毕指令下发后 重新获取UTC时间失败");
                Console.WriteLine("DateTime.UtcNow.ToUniversalTime = {0}", ExactTime.GetUTCTimeStamp());

                curt = Convert.ToInt64(jresp["CURT"].ToString());

                long utc = ExactTime.GetUTCTimeStamp();

                Assert.IsTrue(Math.Abs(utc - curt) <= 10, "校时失败 当前服务器UTC时间 {0}, 当前设备UTC时间 {1}  时间差值为 {2}", utc, curt, curt - utc);

                session.Logout();
            }
        }
	
		[Test]
        public void TimeSystem()
        {
            foreach (string ip in IPList)
            {
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                JObject jresp = null;

                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();

                JObject jatp = new JObject();
                jatp.Add("GE", 0);
                jatp.Add("NE", 0);
                jatp.Add("CE", 0);
                jmdvr.Add("ATP", jatp);

                jparameter.Add("MDVR", jmdvr);

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"]), "关闭所有校时机制[gps. ntp. center server].错误码为{0}", Convert.ToInt32(jresp["ERRORCODE"]));


                long utctime = ExactTime.GetUTCTimeStamp(new DateTime(2002, 08, 18, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second));

                jparameter = new JObject();
                jparameter.Add("CURT", utctime);
                jparameter.Add("Z", "480A");

                jresp = session.SendCommand(Module.DEVEMM, Operation.SETCTRLUTC, jparameter);

                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"]), "初始化UTC时间设定(1029632400)失败.错误码为{0}", Convert.ToInt32(jresp["ERRORCODE"]));

                jresp = session.SendCommand(Module.DEVEMM, Operation.GETCTRLUTC, null);

                long curt = Convert.ToInt64(jresp["CURT"].ToString());

                //           Assert.IsTrue(curt == 1029632400, "初始化UTC(1029632400)时间失败");

                //开启NTP服务器校时
                jparameter = new JObject();
                jmdvr = new JObject();
                jatp = new JObject();
                jatp.Add("GE", 1);
                jatp.Add("NE", 0);
                jatp.Add("CE", 0);
                jmdvr.Add("ATP", jatp);

                jparameter.Add("MDVR", jmdvr);

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"]), "开启ntp校时失败.错误码为{0}", Convert.ToInt32(jresp["ERRORCODE"]));

                Sleep(30000);

                jresp = session.SendCommand(Module.DEVEMM, Operation.GETCTRLUTC, null);

                Assert.IsNotNull(jresp["CURT"], "校时完毕指令下发后 重新获取UTC时间失败");
                Console.WriteLine("DateTime.UtcNow.ToUniversalTime = {0}", ExactTime.GetUTCTimeStamp());

                curt = Convert.ToInt64(jresp["CURT"].ToString());

                long utc = ExactTime.GetUTCTimeStamp();

                Assert.IsTrue(Math.Abs(utc - curt) <= 5, "校时失败 设备当前UTC时间 {0}  服务器UTC时间{1} 时间差值{2}", curt, utc, utc - curt);

                session.Logout();
            }
        }

        [Test]
        public void TimeManual()
        {
            foreach (string ip in IPList)
            {
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                //首先关闭自动校时
                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();

                JObject jatp = new JObject();
                jatp.Add("GE", 0);
                jatp.Add("NE", 0);
                jatp.Add("CE", 0);
                jmdvr.Add("ATP", jatp);

                jparameter.Add("MDVR", jmdvr);

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"]), "关闭所有校时机制[gps. ntp. center server].错误码为{0}", Convert.ToInt32(jresp["ERRORCODE"]));


                jparameter = new JObject();
                jmdvr = new JObject();
                JObject jtimep = new JObject();
                jtimep.Add("TIMEZ", "?");
                jmdvr.Add("TIMEP", jtimep);
                jparameter.Add("MDVR", jmdvr);

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);

                Assert.IsNotNull(jresp["MDVR"]["TIMEP"]["TIMEZ"], "获取设备时区失败");

                string timez = jresp["MDVR"]["TIMEP"]["TIMEZ"].ToString();

                int nTimez = Convert.ToInt32(timez.Substring(0, timez.Length - 1));


                Console.WriteLine("jresp = {0}", jresp.ToString());

                //unix时间戳1490234400 相当于北京时间:2017/03/23 10:00:00

                DateTime dtUTC = new DateTime(2017, 3,23,10,0,0, DateTimeKind.Utc);
                DateTime dtLocal = dtUTC.AddMinutes(nTimez);

                jparameter = new JObject();
                jparameter.Add("CURT", (int)(dtUTC - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);

                jresp = session.SendCommand(Module.DEVEMM, Operation.SETCTRLUTC, jparameter);

                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"]), "初始化时间设定(2017/03/23 10:00:00)失败.错误码为{0}", Convert.ToInt32(jresp["ERRORCODE"]));

                long time = 1490234400;

                Sleep(5000);

                jresp = session.SendCommand(Module.DEVEMM, Operation.GETCTRLUTC, null);
                long curt = Convert.ToInt64(jresp["CURT"].ToString());

                Console.WriteLine("时间差值为{0}", Math.Abs(curt - time));
                Assert.IsTrue(Math.Abs(curt - time) <= 10, "time = {0}. curt = {1}, timespan = {2}", time, curt, curt - time);

                session.Logout();
            }
        }
    }
}
