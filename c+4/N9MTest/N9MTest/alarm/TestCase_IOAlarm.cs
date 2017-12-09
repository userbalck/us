using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using N9MTest.SDK.net;
using N9MTest.SDK.Tooling;
using Newtonsoft.Json.Linq;

namespace N9MTest.Alarm
{
    [TestFixture]
    public class TestCase_IO: TestCase_Basecase
    {
		[Ignore("1111")]
        [Test]
        public void HLTAlarmInput()
        {
            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");
                session.SetSensorUpload(true);

                //设置IO报警电平为高

                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                JArray jiopArray = new JArray();

                for (int i = 0; i < 1; i++)
                {
                    JObject jiop = new JObject();
                    jiop["EN"] = 1;
                    jiop["EL"] = 1;

                    jiopArray.Insert(i, jiop);
                }

                jmdvr["IOP"] = jiopArray;
                jparameter["MDVR"] = jmdvr;

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);
                Assert.IsNotNull(jresp);
                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));

                ToolingSession toolingSession = new ToolingSession();
                toolingSession.StartSession(m_ToolingIP, m_ToolingPort);
                toolingSession.SendMessage("$IO01,01*HH");
                Sleep(3 * 1000);
                Assert.AreEqual(1, session.GetIOSensorInfo(0).nS, "IO报警设置失败");
                toolingSession.SendMessage("$IO01,00*HH");
            }
        }
		[Ignore("1111")]
		[Test]
        public void LLTAlarmInput()
        {
            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");
                session.SetSensorUpload(true);

                //设置IO报警电平为高

                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                JArray jiopArray = new JArray();

                for (int i = 0; i < 1; i++)
                {
                    JObject jiop = new JObject();
                    jiop["EN"] = 1;
                    jiop["EL"] = 0;

                    jiopArray.Insert(i, jiop);
                }

                jmdvr["IOP"] = jiopArray;
                jparameter["MDVR"] = jmdvr;

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);
                Assert.IsNotNull(jresp);
                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));

                ToolingSession toolingSession = new ToolingSession();
                toolingSession.StartSession(m_ToolingIP, m_ToolingPort);
                toolingSession.SendMessage("$IO01,00*HH");
                Sleep(3 * 1000);
                Assert.AreEqual(1, session.GetIOSensorInfo(0).nS, "IO报警设置失败");
            }
        }
    }
}
