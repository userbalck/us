using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using N9MTest.SDK.net;
using Newtonsoft.Json.Linq;

namespace N9MTest.Time
{
	
    [TestFixture]
    class TestCase_DateTime:TestCase_Basecase
    {
        [Test]
        public void TimeMax()
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

                DateTime dtUtc = new DateTime(2035, 12, 31, 23, 59, 55, DateTimeKind.Utc);
                DateTime dtLocal = dtUtc.AddMinutes(nTimez);

                jparameter = new JObject();
                jparameter.Add("CURT", (int)(dtUtc - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);

                jresp = session.SendCommand(Module.DEVEMM, Operation.SETCTRLUTC, jparameter);

                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"]), "初始化UTC时间设定(2082729595)失败.错误码为{0}", Convert.ToInt32(jresp["ERRORCODE"]));


                long time = 2082729595;

				Sleep(5000);

                for (int i = 0; i < 10; i++)
				{
					jresp = session.SendCommand(Module.DEVEMM, Operation.GETCTRLUTC, null);
                    long curt = Convert.ToInt64(jresp["CURT"].ToString());

					Console.WriteLine("第{0}次：时间差值为{1}", i + 1, Math.Abs(curt - time));
                    Assert.IsTrue(Math.Abs(curt - time) <= 10, "time = {0}. curt = {1}, timespan = {2}", time,curt, curt - time);
                    time = curt;
					Sleep(5000);
				}

                session.Logout();
            }
        }
    }
}
