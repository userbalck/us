using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using N9MTest.SDK.net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace N9MTest.Snap
{
    [TestFixture]
    class TestCase_SnapShot:TestCase_Basecase
    {
		[Ignore("11111")]
		[Test]
        public void TimeSnap()
        {

            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);

                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                jmdvr["PSNAP"] = "?";

                jparameter["MDVR"] = jmdvr;

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);

                JArray array = JArray.Parse(jresp["MDVR"]["PSNAP"]["TSP"][0]["CSP"].ToString());

                Console.WriteLine("array = {0}", array.Count);

                DateTime snapStartTime = DateTime.Now.AddMinutes(2);
                DateTime snapEndTime = snapStartTime.AddSeconds(5);

                jparameter = new JObject();

                JArray jcspArray = new JArray();

                for (int i = 0; i < session.GetChannelCount(); i++)
                {
                    JObject jcsp = new JObject();
                    jcsp["E"] = 1;
                    jcsp["I"] = 5;
                    jcsp["N"] = 3;
                    jcsp["Q"] = 1;
                    jcsp["R"] = 0;
                    jcsp["U"] = 1;

                    jcspArray.Add(jcsp);
                }

                JObject jtsp = new JObject();
                jtsp["CSP"] = jcspArray;
                jtsp["SH"] = snapStartTime.Hour;
                jtsp["SM"] = snapStartTime.Minute;
                jtsp["SS"] = snapStartTime.Second;
                jtsp["EH"] = 23;
                jtsp["EM"] = 59;
                jtsp["ES"] = 59;
                jtsp["I"] = 24 * 3600;

                JArray jtspArray = new JArray();
                jtspArray.Add(jtsp);

                JObject jpsnap = new JObject();
                jpsnap["TSP"] = jtspArray;
                jpsnap["NE"] = 1;

                jmdvr = new JObject();

                jmdvr["PSNAP"] = jpsnap;
                jparameter["MDVR"] = jmdvr;

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Sleep(150 * 1000);

                jparameter = new JObject();
                jparameter["STREAMTYPE"] = (int)StreamType.IMAGE_STREAM;
                jparameter["PICMTYPE"] = (int)PicMainType.PIC_MAIN_TIMER;
                jparameter["CHANNEL"] = 0xffffffff;
                jparameter["STARTTIME"] = snapStartTime.AddMinutes(-1).ToString("yyyyMMddHHmmss");
                jparameter["ENDTIME"] = snapEndTime.AddMinutes(1).ToString("yyyyMMddHHmmss");

                jresp = session.SendCommand(Module.STORM, Operation.QUERYFILELIST, jparameter);

                Assert.IsNotNull(jresp);
                Assert.IsNotNull(jresp["FILELIST"]);

                List<RemoteFileInfo> RemoteFileList = JsonConvert.DeserializeObject<List<RemoteFileInfo>>(jresp["FILELIST"].ToString());

                Assert.AreEqual(session.GetChannelCount() * 3, RemoteFileList.Count);

            }
        }

		[Ignore("11111")]
		[Test]
        public void AlarmSnap()
        {
            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);

                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                jmdvr["PSNAP"] = "?";

                jparameter["MDVR"] = jmdvr;

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);

                JArray array = JArray.Parse(jresp["MDVR"]["PSNAP"]["TSP"][0]["CSP"].ToString());

                Console.WriteLine("array = {0}", array.Count);

                DateTime snapStartTime = DateTime.Now.AddMinutes(2);
                DateTime snapEndTime = snapStartTime.AddSeconds(10);

                Sleep(120 * 1000);

                jparameter = new JObject();

                JArray jcspArray = new JArray();

                for (int i = 0; i < session.GetChannelCount(); i++)
                {
                    JObject jcsp = new JObject();
                    jcsp["E"] = 1;
                    jcsp["I"] = 5;
                    jcsp["N"] = 3;
                    jcsp["Q"] = 1;
                    jcsp["R"] = 0;
                    jcsp["U"] = 1;

                    jcspArray.Add(jcsp);
                }

                JObject jmsp = new JObject();
                jmsp["CSP"] = jcspArray;


                JObject jpsnap = new JObject();
                jpsnap["MSP"] = jmsp;

                jmdvr = new JObject();

                jmdvr["PSNAP"] = jpsnap;
                jparameter["MDVR"] = jmdvr;

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Sleep(10 * 1000);

                jparameter = new JObject();
                jparameter["STREAMTYPE"] = (int)StreamType.IMAGE_STREAM;
                jparameter["PICMTYPE"] = (int)PicMainType.PIC_MAIN_ALARM;
                jparameter["CHANNEL"] = 0xffffffff;
                jparameter["STARTTIME"] = snapStartTime.ToString("yyyyMMddHHmmss");
                jparameter["ENDTIME"] = snapEndTime.ToString("yyyyMMddHHmmss");

                jresp = session.SendCommand(Module.STORM, Operation.QUERYFILELIST, jparameter);
                Assert.IsNotNull(jresp);
                Assert.IsNotNull(jresp["FILELIST"]);
            }
        }

        [Test]
        public void ManualSnap()
        {
            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);

                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                jmdvr["PSNAP"] = "?";

                jparameter["MDVR"] = jmdvr;

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);

                JArray array = JArray.Parse(jresp["MDVR"]["PSNAP"]["TSP"][0]["CSP"].ToString());

                Console.WriteLine("array = {0}", array.Count);

                DateTime snapStartTime = DateTime.Now.AddMinutes(2);
                DateTime snapEndTime = snapStartTime.AddSeconds(10);

                Sleep(120 * 1000);

                jparameter = new JObject();

                JArray jcspArray = new JArray();

                for (int i = 0; i < session.GetChannelCount(); i++)
                {
                    JObject jcsp = new JObject();
                    jcsp["E"] = 1;
                    jcsp["I"] = 5;
                    jcsp["N"] = 3;
                    jcsp["Q"] = 1;
                    jcsp["R"] = 0;
                    jcsp["U"] = 1;

                    jcspArray.Add(jcsp);
                }

                JObject jmsp = new JObject();
                jmsp["CSP"] = jcspArray;


                JObject jpsnap = new JObject();
                jpsnap["MSP"] = jmsp;

                jmdvr = new JObject();

                jmdvr["PSNAP"] = jpsnap;
                jparameter["MDVR"] = jmdvr;

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Sleep(10 * 1000);

                jparameter = new JObject();
                jparameter["STREAMTYPE"] = (int)StreamType.IMAGE_STREAM;
                jparameter["PICMTYPE"] = (int)PicMainType.PIC_MAIN_MANUAL;
                jparameter["CHANNEL"] = 0xffffffff;
                jparameter["STARTTIME"] = snapStartTime.ToString("yyyyMMddHHmmss");
                jparameter["ENDTIME"] = snapEndTime.ToString("yyyyMMddHHmmss");

                jresp = session.SendCommand(Module.STORM, Operation.QUERYFILELIST, jparameter);
                Assert.IsNotNull(jresp);
                Assert.IsNotNull(jresp["FILELIST"]);
            }
        }
    }
}
