using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using N9MTest.SDK.net;
using Newtonsoft.Json.Linq;
using N9MTest.SDK.Tooling;
using N9MTest.DialupNetwork;

namespace N9MTest.Alarm
{
    [TestFixture]
    class TestCase_AlarmLinkage:TestCase_Basecase
    {
		
		[Test]
        public void LinkageAlarmRecord()
        {

            //录像延时为1分钟 大约为60秒
            int nDelay = 60;

            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();

                //首先获取所有的开关量设置
                jmdvr["IOP"] = "?";
                jparameter["MDVR"] = jmdvr;
                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);

                jparameter = new JObject();
                jmdvr = new JObject();

                JArray JIOPArray = new JArray();

                for (int i = 0; i < 4; i++)
                {
                    //设置IO报警开启 高电平触发
                    JObject jiop = new JObject();
                    jiop["EN"] = 1;
                    jiop["EL"] = 1;
                    jiop["AS"] = 1;
                    jiop["SDT"] = 0;

                    JObject japr = new JObject();

                    //设置对应的联动通道，延迟为30秒
                    JObject jar = new JObject();
                    jar["CH"] = 1 << i;
                    jar["P"] = 0;
                    jar["D"] = nDelay;
                    jar["L"] = 0;

                    japr["AR"] = jar;
                    jiop["APR"] = japr;
                    JIOPArray.Insert(i, jiop);
                }

                jmdvr["IOP"] = JIOPArray;
                jparameter["MDVR"] = jmdvr;

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);
                Assert.IsNotNull(jresp);
                Assert.IsNotNull(jresp["ERRORCODE"]);
                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));

                ToolingSession toolingSession = new ToolingSession();
                toolingSession.StartSession(m_ToolingIP, m_ToolingPort);

                //测试1通道报警
                toolingSession.SendMessage("$IO01,01*HH");
                Sleep(1000);
                toolingSession.SendMessage("$IO01,00*HH");
                Sleep(10 * 1000);
                for (int i = 0; i < session.GetChannelCount(); i++)
                {
                    if (i == 0)
                    {
                        Assert.IsTrue(session.isAlarmRecording(i), "通道{0}没有报警录像", i);
                    }
                    else
                    {
                        Assert.IsFalse(session.isAlarmRecording(i), "通道{0}不应该进行报警录像", i);
                    }
                }

                Sleep(90 * 1000);


                //测试2通道报警
                toolingSession.SendMessage("$IO02,01*HH");
                Sleep(1000);
                toolingSession.SendMessage("$IO02,00*HH");
                Sleep(10 * 1000);
                for (int i = 0; i < session.GetChannelCount(); i++)
                {
                    if (i == 1)
                    {
                        Assert.IsTrue(session.isAlarmRecording(i),"通道{0}没有报警录像", i);
                    }
                    else
                    {
                        Assert.IsFalse(session.isAlarmRecording(i), "通道{0}不应该进行报警录像", i);
                    }
                }

                Sleep(90 * 1000);

                //测试3通道报警
                toolingSession.SendMessage("$IO03,01*HH");
                Sleep(1000);
                toolingSession.SendMessage("$IO03,00*HH");
                Sleep(10 * 1000);
                for (int i = 0; i < session.GetChannelCount(); i++)
                {
                    if (i == 2)
                    {
                        Assert.IsTrue(session.isAlarmRecording(i), "通道{0}没有报警录像", i);
                    }
                    else
                    {
                        Assert.IsFalse(session.isAlarmRecording(i), "通道{0}不应该进行报警录像", i);
                    }
                }

                Sleep(90 * 1000);

                //测试4通道报警
                toolingSession.SendMessage("$IO04,01*HH");
                Sleep(1000);
                toolingSession.SendMessage("$IO04,00*HH");
                Sleep(10 * 1000);
                for (int i = 0; i < session.GetChannelCount(); i++)
                {
                    if (i == 3)
                    {
                        Assert.IsTrue(session.isAlarmRecording(i), "通道{0}没有报警录像", i);
                    }
                    else
                    {
                        Assert.IsFalse(session.isAlarmRecording(i), "通道{0}不应该进行报警录像", i);
                    }
                }
            }
        }

        /// <summary>
        /// 3G拨号
        /// </summary>
		[Ignore("111111")]
        public void LinkageDialup()
        {
            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
       
                jmdvr["IOP"] = "?";
                jparameter["MDVR"] = jmdvr;

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);

                //设定IO高电平报警联动拨号
                jparameter = new JObject();
                jmdvr = new JObject();
                JArray jiopArray = new JArray();

                for (int i = 0; i < 8; i++)
                {
                    if (i == 0)
                    {
                        JObject jiop = new JObject();
                        jiop["EN"] = 1;
                        jiop["EL"] = 1;

                        JObject japr = new JObject();
                        japr["DA"] = 1;
                        jiop["APR"] = japr;

                        jiopArray.Insert(0, jiop);
                    }
                    else
                    {
                        JObject jiop = new JObject();
                        jiop["EN"] = 0;

                        jiopArray.Insert(i, jiop);
                    }
                    

                }

                jmdvr["IOP"] = jiopArray;
                jparameter["MDVR"] = jmdvr;

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);
                Assert.IsNotNull(jresp);
                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));

                Sleep(10 * 1000);

                ToolingSession toolingSession = new ToolingSession();
                toolingSession.StartSession(m_ToolingIP, m_ToolingPort);

                do
                {
                    //发送IO1报警
                    toolingSession.SendMessage("$IO1, 01*HH");

                    Sleep(15 * 1000);

                    //获取运维信息 获取3G拨号状态
                    jparameter = new JObject();
                    jparameter["DATE"] = 0;
                    jparameter["TYPE"] = 1;
                    jparameter["INFO"] = "?";

                    jresp = session.SendCommand(Module.DEVEMM, Operation.GETYUNWEIINFO, jparameter);

                    Assert.IsNotNull(jresp);
                    Assert.IsNotNull(jresp["ERRORCODE"]);
                    Assert.IsNotNull(jresp["INFO"]);

                    DialState dialState = DialState.Unknown;

                    if (jresp["INFO"]["3G"] != null && jresp["INFO"]["3G"].HasValues)
                    {
                        dialState = (DialState)Convert.ToInt32(jresp["INFO"]["3G"]["BS"].ToString());

                        Console.WriteLine("[3G]dialState = {0}", dialState);

                        Assert.AreEqual(DialState.Success, dialState, "3G拨号状态检测失败");
                    }

                    if (jresp["INFO"]["4G"] != null && jresp["INFO"]["4G"].HasValues)
                    {
                        dialState = (DialState)Convert.ToInt32(jresp["INFO"]["4G"]["BS"].ToString());

                        Console.WriteLine("[4G]dialState = {0}", dialState);

                        Assert.AreEqual(DialState.Success, dialState, "4G拨号状态检测失败");
                    }

                    Assert.AreEqual(DialState.Success, dialState, "拨号状态检测失败");
                } while (false);

                do
                {
                    //解除IO1报警
                    toolingSession.SendMessage("$IO1, 00*HH");

                    Sleep(15 * 1000);

                    //获取运维信息 获取3G拨号状态
                    jparameter = new JObject();
                    jparameter["DATE"] = 0;
                    jparameter["TYPE"] = 1;
                    jparameter["INFO"] = "?";

                    jresp = session.SendCommand(Module.DEVEMM, Operation.GETYUNWEIINFO, jparameter);

                    Assert.IsNotNull(jresp);
                    Assert.IsNotNull(jresp["ERRORCODE"]);
                    Assert.IsNotNull(jresp["INFO"]);

                    DialState dialState = DialState.Unknown;

                    if (jresp["INFO"]["3G"] != null && jresp["INFO"]["3G"].HasValues)
                    {
                        dialState = (DialState)Convert.ToInt32(jresp["INFO"]["3G"]["BS"].ToString());

                        Console.WriteLine("[3G]dialState = {0}", dialState);

                        Assert.AreEqual(DialState.Success, dialState, "3G拨号状态检测失败");
                    }

                    if (jresp["INFO"]["4G"] != null && jresp["INFO"]["4G"].HasValues)
                    {
                        dialState = (DialState)Convert.ToInt32(jresp["INFO"]["4G"]["BS"].ToString());

                        Console.WriteLine("[4G]dialState = {0}", dialState);

                        Assert.AreEqual(DialState.Success, dialState, "4G拨号状态检测失败");
                    }

                    Assert.AreEqual(DialState.Success, dialState, "拨号状态检测失败");
                } while (false);
            }
        }

		/// <summary>
		/// 报警输出
		/// </summary>
		[Ignore("111111")]
		[Test]
        public void LinkageAlarmOutput()
        {
            foreach (string ip in IPList)
            {
                Console.WriteLine(" ip = {0}", ip);

                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                JArray jiopArray = new JArray();

                //设置IO1高电平，报警有效时间0秒，联动输出1，输出时间0秒
                do
                {
                    JObject jiop = new JObject();
                    jiop["EN"] = 1;
                    jiop["EL"] = 1;
                    jiop["AS"] = 1;

                } while (false);
            }
        }
    }
}
