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
	[Ignore("111111")]
	[TestFixture]
    class TestCase_DST:TestCase_Basecase
    {
        [Test]
        public void TimeDST()
        {
            //输入模式[0-日期模式 1-星期模式]
            int mode = 1;

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

                //获取当前下夏令时的状态
                jparameter = new JObject();
                jmdvr = new JObject();
                JObject jtimep = new JObject();
                JObject jdst = new JObject();

                jtimep.Add("DST", "?");
                jmdvr.Add("TIMEP", jtimep);
                jparameter.Add("MDVR", jmdvr);

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);

                Assert.AreEqual(2, 1);

                jparameter = new JObject();
                jmdvr = new JObject();
                jtimep = new JObject();
                jdst = new JObject();

                //打开夏令时使能 设定夏令时时段

                if (mode == 0)
                {
                    jdst.Add("SW", 1);
                    jdst.Add("DSTM", 2);
                    jdst.Add("DSTS", 1);
                    jdst.Add("STARTTIME", "1489113035");
                    jdst.Add("ENDTIME", "1489116695");
                }
                else if (mode == 1)
                {
                    jdst.Add("SW", 1);
                    jdst.Add("DSTM", 1);
                    jdst.Add("DSTS", 1);
                    jdst.Add("SMON", 2);
                    jdst.Add("SWEEK", 1);
                    jdst.Add("SWIND", 1);
                    jdst.Add("SH", 2);
                    jdst.Add("SM", 30);
                    jdst.Add("SS", 35);

                    jdst.Add("EMON", 2);
                    jdst.Add("EWEEK", 1);
                    jdst.Add("EWIND", 1);

                    jdst.Add("EH", 3);
                    jdst.Add("EM", 31);
                    jdst.Add("ES", 35);
                }

                jtimep.Add("DST", jdst);
                jmdvr.Add("TIMEP", jtimep);
                jparameter.Add("MDVR", jmdvr);

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Assert.IsNotNull(jresp, "[日期模式]设置夏令时起始日期失败");

                jparameter = new JObject();

                if (mode == 0)
                {
                    //[UTC时间 2017/03/10 02:30:30->1489113030]
                    jparameter.Add("CURT", 1489113030);
                }
                else
                {
                    //[UTC时间 2017/03/06 10:30:30->1488767430]
                    jparameter.Add("CURT", 1488767430);
                }

                jparameter.Add("Z", "0A");

                jresp = session.SendCommand(Module.DEVEMM, Operation.SETCTRLUTC, jparameter);

                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"]), "初始化UTC时间设定(1029632400)失败.错误码为{0}", Convert.ToInt32(jresp["ERRORCODE"]));

                //获取跳转前的时间
                jresp = session.SendCommand(Module.DEVEMM, Operation.GETCTRLUTC, null);

                Assert.IsNotNull(jresp["CURT"]);

                long curt = Convert.ToInt64(jresp["CURT"].ToString());

                Sleep(10000);

                jresp = session.SendCommand(Module.DEVEMM, Operation.GETCTRLUTC, jparameter);

                Assert.IsNotNull(jresp["CURT"]);

                double timespan = System.Math.Abs((double)Convert.ToUInt64(jresp["CURT"].ToString()) - (double)curt);

                Assert.IsTrue(timespan >= 3600 && timespan < 3605, "[日期模式]夏令时跳转失败timespan = {0}", timespan);

                session.Logout();
            }
        }
    }
}
