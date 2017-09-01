using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using N9MTest.SDK.net;
using Newtonsoft.Json.Linq;
using N9MTest.SDK.Tooling;
using static N9MTest.SDK.net.BlackBox;
using System.IO;

namespace N9MTest.Alarm
{
    class TestCase_SpeedAlarm:TestCase_Basecase
    {
		[Ignore("1111111")]
        [Test]
        public void GPSSpeed()
        {
            foreach (string ip in IPList)
            {
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                //获取设备时间
                DeviceTime devTime = session.GetDeviceTime();

                //首先获取一次导航设定的基本信息
                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                JObject jgsp = new JObject();

                jmdvr.Add("GSP", "?");
                jparameter.Add("MDVR", jmdvr);

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);
                Assert.IsNotNull(jresp);

                Sleep(10 * 1000);

                jparameter = new JObject();
                jmdvr = new JObject();
                jgsp = new JObject();

                jgsp["GM"] = (int)Navigation.GPS;
                jmdvr.Add("GSP", jgsp);
                jparameter.Add("MDVR", jmdvr);

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                ToolingSession toolingSession = new ToolingSession();
                toolingSession.StartSession(m_ToolingIP, m_ToolingPort);

                DateTime t1 = devTime.Now;

                for (int i = 0; i < 120; i++)
                {
                    toolingSession.SendMessage("");
                    Sleep(1000);
                }

                DateTime t2 = devTime.Now;

                DateTime dtStartTime = t2.AddSeconds(-90);
                DateTime dtEndTime = dtStartTime.AddSeconds(60);

                string starttime = dtStartTime.ToString("yyyyMMddHHmmss");
                string endtime = dtEndTime.ToString("yyyyMMddHHmmss");

                string path = Path.Combine(Environment.CurrentDirectory, "gps.data");

                int ret = session.DownloadData(DataType.BLACKBOX_DATATYPE_GPS, starttime, endtime, path);

                Assert.AreEqual(0, ret);

                BlackBoxLoader loader = new BlackBoxLoader(path);

                int nCount = 0;

                string lasttime = null;
                string currenttime = null;

                while (true)
                {
                    RMBDM_FRAMEHEADER header = loader.GetFrame();

                    Assert.IsNotNull(header, "解析GPS数据失败");

                    Assert.IsNotNull(header.list, "解析GPS附加数据失败");

                    for (int i = 0; i < header.list.Count; i++)
                    {
                        IBaseStruct bs = header.list[i];

                        if (typeof(RMBDM_DATETIME).IsAssignableFrom(bs.GetType()))
                        {
                            RMBDM_DATETIME date = (RMBDM_DATETIME)bs;

                            currenttime = date.time.GetDateTime().ToString("yyyyMMddHHmmss");

                            if (lasttime != null)
                            {
                                if (currenttime == lasttime)
                                {
                                    currenttime = date.time.GetDateTime().AddSeconds(1).ToString("yyyyMMddHHmmss");
                                }
                            }

                            lasttime = currenttime;

                        }
                        else if (typeof(RMBDM_GPS).IsAssignableFrom(bs.GetType()))
                        {
                            RMBDM_GPS gps = (RMBDM_GPS)bs;
                            Assert.IsTrue(gps.usSpeed >= 7, "GPS卫星颗数不足7颗， 实际为{0}颗", gps.cGpPlanetNum);
                        }
                    }

                    if (currenttime.Equals(endtime))
                    {
                        break;
                    }
                }
            }
        }
		[Ignore("1111111")]
		[Test]
        public void ManualAdjustPulseSpeed()
        {
            foreach (string ip in IPList)
            {
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                //首先获取一次速度报警参数
                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                JObject jsap = new JObject();

                jmdvr.Add("SAP", "?");
                jparameter.Add("MDVR", jmdvr);

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);
                Assert.IsNotNull(jresp);

                Sleep(10 * 1000);

                //设置当前播放的速度
                jparameter = new JObject();
                jmdvr = new JObject();
                jsap = new JObject();

                jsap["SF"] = (int)SpeedSource.Pluse;
                jmdvr.Add("SAP", jsap);
                jparameter.Add("MDVR", jmdvr);

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);
            }
        }
		[Ignore("1111111")]
		[Test]
        public void AutoAdjustPulseSpeed()
        {
            foreach (string ip in IPList)
            {
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                //首先获取一次速度报警参数
                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                JObject jsap = new JObject();

                jmdvr.Add("SAP", "?");
                jparameter.Add("MDVR", jmdvr);

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);
                Assert.IsNotNull(jresp);

                Sleep(10 * 1000);

                //
                jparameter = new JObject();
                jmdvr = new JObject();
                jsap = new JObject();

                jsap["SF"] = (int)SpeedSource.Pluse;
                jmdvr.Add("SAP", jsap);
                jparameter.Add("MDVR", jmdvr);

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);
            }
        }
		[Ignore("1111111")]
		[Test]
        public void OBDSpeed()
        {
            foreach (string ip in IPList)
            {
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                //首先获取一次速度报警参数
                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                JObject jsap = new JObject();

                jmdvr.Add("SAP", "?");
                jparameter.Add("MDVR", jmdvr);

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);
                Assert.IsNotNull(jresp);

                Sleep(10 * 1000);

                //
                jparameter = new JObject();
                jmdvr = new JObject();
                jsap = new JObject();

                jsap["SF"] = (int)SpeedSource.OBD;
                jmdvr.Add("SAP", jsap);
                jparameter.Add("MDVR", jmdvr);

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);
            }
        }
		[Ignore("1111111")]
		[Test]
        public void SpeedAlarmSensitivity()
        {
            foreach (string ip in IPList)
            {
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                //首先获取一次速度报警参数
                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                JObject jsap = new JObject();

                jmdvr.Add("SAP", "?");
                jparameter.Add("MDVR", jmdvr);

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);
                Assert.IsNotNull(jresp);

                Sleep(10 * 1000);

                //
                jparameter = new JObject();
                jmdvr = new JObject();
                jsap = new JObject();

                jsap["SF"] = (int)SpeedSource.Satellite;
                jmdvr.Add("SAP", jsap);
                jparameter.Add("MDVR", jmdvr);

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);
            }
        }
    }
}
