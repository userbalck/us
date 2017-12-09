using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using N9MTest.SDK.net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace N9MTest.DialupNetwork
{
	
    [TestFixture]
    class TestCase_Dialup: TestCase_Basecase
    {
        //联通网络 重启
        [Test]
        public void UnicomBootDial()
        {
            int nCount = Convert.ToInt32(this.CFG.gNode("DialupNetwork/UnicomBootDial/count").InnerText);
            int waittime = Convert.ToInt32(this.CFG.gNode("DialupNetwork/UnicomBootDial/waittime").InnerText);

            //中心服务器IP
            string csip = this.CFG.gNode("DialupNetwork/UnicomBootDial/servercsip").InnerText;
            int csport = Convert.ToInt32(this.CFG.gNode("DialupNetwork/UnicomBootDial/servercsport").InnerText);

            //首先设定中心服务器
            foreach (string ip in IPList)
            {
				Console.WriteLine("连接{0}",ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password));

                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                JObject jmcms = new JObject();
                JArray jspArray = new JArray();
                JObject jsp = new JObject();

                jsp["EN"] = 1;
                jsp["NWT"] = 0;
                jsp["CP"] = 0;
                jsp["CS"] = csip;
                jsp["CPORT"] = csport;

                jspArray.Add(jsp);
                jmcms["SP"] = jspArray;
                jmdvr["MCMS"] = jmcms;
                jparameter["MDVR"] = jmdvr;

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Assert.IsNotNull(jresp);
                Assert.IsNotNull(jresp["ERRORCODE"]);
                Assert.AreEqual(0, Convert.ToInt32((jresp["ERRORCODE"].ToString())));

                session.Logout();
            }

            for (int i = 0; i < nCount; i++)
            {
                Console.WriteLine("-----------第{0}次获取--------------------", i);
                foreach (string ip in IPList)
                {
                    N9MSession session = new N9MSession(ip, port);
                    Assert.AreEqual(0, session.Login(username, password));

                    JObject jparameter = new JObject();
                    jparameter["DATE"] = 0;
                    jparameter["TYPE"] = 1;
                    jparameter["INFO"] = "?";

                    JObject jresp = session.SendCommand(Module.DEVEMM, Operation.GETYUNWEIINFO, jparameter);

                    Assert.IsNotNull(jresp);
                    Assert.IsNotNull(jresp["ERRORCODE"]);
                    Assert.IsNotNull(jresp["INFO"]);

                    Assert.AreEqual((int)DialState.Success, Convert.ToInt32(jresp["INFO"]["3G"]["BS"].ToString()), "3G拨号状态检测失败");
           //         Assert.AreEqual((int)NetType.WCDMA, Convert.ToInt32(jresp["INFO"]["3G"]["NT"].ToString()), "非联通WCDMA模块");

                    jresp = session.SendCommand(Module.DEVEMM, Operation.GETCMSCONNECTSTATUS, jparameter);

                    List<CMSConnectInfo> cmsList = JsonConvert.DeserializeObject<List<CMSConnectInfo>>(jresp["LIST"].ToString());

                    Assert.AreEqual(1, cmsList[0].nCS, "服务器{0}未连接", cmsList[0].address);

                    jparameter = new JObject();
                    jparameter["CMDTYPE"] = 0;

                    jresp = session.SendCommand(Module.DEVEMM, Operation.SETCONTROLDEVCMD, jparameter);
                    Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));


                    session.Logout();
                }

                Sleep(waittime * 1000);
            }
        }

		//联通网络稳定性

		[Test]
        public void UnicomDialStability()
        {
            int nCount = Convert.ToInt32(this.CFG.gNode("DialupNetwork/UnicomDialStability/count").InnerText);

            //中心服务器IP
            string csip = this.CFG.gNode("DialupNetwork/UnicomDialStability/servercsip").InnerText;
            int csport = Convert.ToInt32(this.CFG.gNode("DialupNetwork/UnicomDialStability/servercsport").InnerText);

            //首先设定中心服务器
            foreach (string ip in IPList)
            {
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                JObject jmcms = new JObject();
                JArray jspArray = new JArray();
                JObject jsp = new JObject();

                jsp["EN"] = 1;
                jsp["NWT"] = 0;
                jsp["CP"] = 0;
                jsp["CS"] = csip;
                jsp["CPORT"] = csport;

                jspArray.Add(jsp);
                jmcms["SP"] = jspArray;
                jmdvr["MCMS"] = jmcms;
                jparameter["MDVR"] = jmdvr;

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Assert.IsNotNull(jresp);
                Assert.IsNotNull(jresp["ERRORCODE"]);
                Assert.AreEqual(0, Convert.ToInt32((jresp["ERRORCODE"].ToString())));

                session.Logout();
            }


            for (int i = 0; i < nCount; i++)
            {
                Console.WriteLine("-----------第{0}次获取--------------------", i);
                foreach (string ip in IPList)
                {
                    N9MSession session = new N9MSession(ip, port);
                    Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                    JObject jparameter = new JObject();
                    jparameter["DATE"] = 0;
                    jparameter["TYPE"] = 1;
                    jparameter["INFO"] = "?";

                    JObject jresp = session.SendCommand(Module.DEVEMM, Operation.GETYUNWEIINFO, jparameter);

                    Assert.IsNotNull(jresp);
                    Assert.IsNotNull(jresp["ERRORCODE"]);
                    Assert.IsNotNull(jresp["INFO"]);

                    Assert.AreEqual((int)DialState.Success, Convert.ToInt32(jresp["INFO"]["3G"]["BS"].ToString()), "3G拨号状态检测失败");
//                    Assert.AreEqual((int)NetType.WCDMA, Convert.ToInt32(jresp["INFO"]["3G"]["NT"].ToString()), "非联通WCDMA模块");

                    jresp = session.SendCommand(Module.DEVEMM, Operation.GETCMSCONNECTSTATUS, jparameter);

                    List<CMSConnectInfo> cmsList = JsonConvert.DeserializeObject<List<CMSConnectInfo>>(jresp["LIST"].ToString());

                    Assert.AreEqual(1, cmsList[0].nCS, "服务器{0}未连接", cmsList[0].address);


                    session.Logout();
                }

                Sleep(60 * 1000);
            }
        }


		[Test]
        public void TelecomBootDial()
        {
            int nCount = Convert.ToInt32(this.CFG.gNode("DialupNetwork/TelecomBootDial/count").InnerText);
            int waittime = Convert.ToInt32(this.CFG.gNode("DialupNetwork/TelecomBootDial/waittime").InnerText);

            //中心服务器IP
            string csip = this.CFG.gNode("DialupNetwork/TelecomBootDial/servercsip").InnerText;
            int csport = Convert.ToInt32(this.CFG.gNode("DialupNetwork/TelecomBootDial/servercsport").InnerText);

            //首先设定中心服务器
            foreach (string ip in IPList)
            {
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password));

                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                JObject jmcms = new JObject();
                JArray jspArray = new JArray();
                JObject jsp = new JObject();

                jsp["EN"] = 1;
                jsp["NWT"] = 0;
                jsp["CP"] = 0;
                jsp["CS"] = csip;
                jsp["CPORT"] = csport;

                jspArray.Add(jsp);
                jmcms["SP"] = jspArray;
                jmdvr["MCMS"] = jmcms;
                jparameter["MDVR"] = jmdvr;

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Assert.IsNotNull(jresp);
                Assert.IsNotNull(jresp["ERRORCODE"]);
                Assert.AreEqual(0, Convert.ToInt32((jresp["ERRORCODE"].ToString())));

                session.Logout();
            }

            for (int i = 0; i < nCount; i++)
            {
                Console.WriteLine("-----------第{0}次获取--------------------", i);
                foreach (string ip in IPList)
                {
                    N9MSession session = new N9MSession(ip, port);
                    Assert.AreEqual(0, session.Login(username, password));

                    JObject jparameter = new JObject();
                    jparameter["DATE"] = 0;
                    jparameter["TYPE"] = 1;
                    jparameter["INFO"] = "?";

                    JObject jresp = session.SendCommand(Module.DEVEMM, Operation.GETYUNWEIINFO, jparameter);

                    Assert.IsNotNull(jresp);
                    Assert.IsNotNull(jresp["ERRORCODE"]);
                    Assert.IsNotNull(jresp["INFO"]);

                    Assert.AreEqual((int)DialState.Success, Convert.ToInt32(jresp["INFO"]["3G"]["BS"].ToString()), "3G拨号状态检测失败");
  //                  Assert.AreEqual((int)NetType.WCDMA, Convert.ToInt32(jresp["INFO"]["3G"]["NT"].ToString()), "非电信TDCDMA模块");

                    jresp = session.SendCommand(Module.DEVEMM, Operation.GETCMSCONNECTSTATUS, jparameter);

                    List<CMSConnectInfo> cmsList = JsonConvert.DeserializeObject<List<CMSConnectInfo>>(jresp["LIST"].ToString());

                    Assert.AreEqual(1, cmsList[0].nCS, "服务器{0}未连接", cmsList[0].address);

                    jparameter = new JObject();
                    jparameter["CMDTYPE"] = 0;

                    jresp = session.SendCommand(Module.DEVEMM, Operation.SETCONTROLDEVCMD, jparameter);
                    Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));


                    session.Logout();
                }

                Sleep(waittime * 1000);
            }
        }


		[Test]
        public void TelecomDialStability()
        {
            int nCount = Convert.ToInt32(this.CFG.gNode("DialupNetwork/TelecomDialStability/count").InnerText);

            //中心服务器IP
            string csip = this.CFG.gNode("DialupNetwork/TelecomDialStability/servercsip").InnerText;
            int csport = Convert.ToInt32(this.CFG.gNode("DialupNetwork/TelecomDialStability/servercsport").InnerText);

            //首先设定中心服务器
            foreach (string ip in IPList)
            {
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                JObject jmcms = new JObject();
                JArray jspArray = new JArray();
                JObject jsp = new JObject();

                jsp["EN"] = 1;
                jsp["NWT"] = 0;
                jsp["CP"] = 0;
                jsp["CS"] = csip;
                jsp["CPORT"] = csport;

                jspArray.Add(jsp);
                jmcms["SP"] = jspArray;
                jmdvr["MCMS"] = jmcms;
                jparameter["MDVR"] = jmdvr;

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Assert.IsNotNull(jresp);
                Assert.IsNotNull(jresp["ERRORCODE"]);
                Assert.AreEqual(0, Convert.ToInt32((jresp["ERRORCODE"].ToString())));

                session.Logout();
            }


            for (int i = 0; i < nCount; i++)
            {
                Console.WriteLine("-----------第{0}次获取--------------------", i);
                foreach (string ip in IPList)
                {
                    N9MSession session = new N9MSession(ip, port);
                    Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                    JObject jparameter = new JObject();
                    jparameter["DATE"] = 0;
                    jparameter["TYPE"] = 1;
                    jparameter["INFO"] = "?";

                    JObject jresp = session.SendCommand(Module.DEVEMM, Operation.GETYUNWEIINFO, jparameter);

                    Assert.IsNotNull(jresp);
                    Assert.IsNotNull(jresp["ERRORCODE"]);
                    Assert.IsNotNull(jresp["INFO"]);

                    Assert.AreEqual((int)DialState.Success, Convert.ToInt32(jresp["INFO"]["3G"]["BS"].ToString()), "3G拨号状态检测失败");
     //               Assert.AreEqual((int)NetType.TDSCDMA, Convert.ToInt32(jresp["INFO"]["3G"]["NT"].ToString()), "非电信TDCDMA模块");

                    jresp = session.SendCommand(Module.DEVEMM, Operation.GETCMSCONNECTSTATUS, jparameter);

                    List<CMSConnectInfo> cmsList = JsonConvert.DeserializeObject<List<CMSConnectInfo>>(jresp["LIST"].ToString());

                    Assert.AreEqual(1, cmsList[0].nCS, "服务器{0}未连接", cmsList[0].address);


                    session.Logout();
                }

                Sleep(60 * 1000);
            }
        }
    }
}
