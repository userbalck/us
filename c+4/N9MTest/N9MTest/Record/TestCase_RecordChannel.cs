using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using N9MTest.SDK.net;
using Newtonsoft.Json.Linq;

namespace N9MTest.Record
{
    [TestFixture]
    class TestCase_RecordChannel : TestCase_Basecase
    {
		[Ignore("111111")]
        [Test]
        public void RecordCodeRate()
        {
            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                //主码流使能关闭，此通道预览黑屏，主从都不录像

                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();


                JArray jmainArray = new JArray();

                for (int i = 0; i < session.GetChannelCount(); i++)
                {
                    JObject jmain = new JObject();
                    jmain.Add("VEN", 1);
                    jmainArray.Add(jmain);
                }

                JObject jsubstrnet = new JObject();
                JArray jnecArray = new JArray();
                for (int i = 0; i < session.GetChannelCount(); i++)
                {
                    JObject jnect = new JObject();
                    jnect.Add("VEN", 1);
                    jnecArray.Add(jnect);
                }

                jsubstrnet.Add("NEC", jnecArray);


                JObject jar = new JObject();
                jar.Add("EN", 1);
                jar.Add("RM", 0);

                jmdvr.Add("MAIN", jmainArray);
                jmdvr.Add("AR", jar);
                jmdvr.Add("SUBSTRNET", jsubstrnet);
                jparameter.Add("MDVR", jmdvr);


                session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Sleep(15 *1000);

                for (int i = 0; i < session.GetChannelCount(); i++)
                {
                    Console.WriteLine("{0}main = {1}, sub = {2} mirror = {3}",
                       i,
                       session.isChannelRecording(StreamType.MAIN_STREAM, i),
                       session.isChannelRecording(StreamType.SUB_STREAM, i),
                       session.isChannelRecording(StreamType.MIRROR_STREAM, i));

                    if (session.GetDeviceType() == DeviceType.X1)
                    {
                        Assert.IsTrue(session.isChannelRecording(StreamType.MAIN_STREAM, i));
                    }
                    else
                    {
                        Assert.IsTrue(session.isChannelRecording(StreamType.MAIN_STREAM, i));
                        Assert.IsTrue(session.isChannelRecording(StreamType.SUB_STREAM, i));
                    }
                }

                session.Logout();
            }
        }

        [Test]
        public void RecordEnable()
        {
            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                //主码流使能关闭，此通道预览黑屏，主从都不录像

                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();


                JArray jmainArray = new JArray();

                for (int i = 0; i < session.GetChannelCount(); i++)
                {
                    JObject jmain = new JObject();
                    jmain.Add("VEN", 0);
                    jmainArray.Add(jmain);
                }

                jmdvr.Add("MAIN", jmainArray);
                jparameter.Add("MDVR", jmdvr);

                session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Sleep(5000);

                for (int i = 0; i < session.GetChannelCount(); i++)
                {
                    Assert.IsFalse(session.isChannelRecording(StreamType.MAIN_STREAM, i));
                    Assert.IsFalse(session.isChannelRecording(StreamType.SUB_STREAM, i));
                    Assert.IsFalse(session.isChannelRecording(StreamType.MIRROR_STREAM, i));
                    Console.WriteLine("{0}main = {1}, sub = {2} mirror = {3}",
                        i,
                        session.isChannelRecording(StreamType.MAIN_STREAM, i),
                        session.isChannelRecording(StreamType.SUB_STREAM, i),
                        session.isChannelRecording(StreamType.MIRROR_STREAM, i));
                }

                //镜像通道使能关闭，主码流录像，无镜像录像

                jparameter = new JObject();
                jmdvr = new JObject();

                jmainArray = new JArray();

                for (int i = 0; i < session.GetChannelCount(); i++)
                {
                    JObject jmain = new JObject();
                    jmain.Add("VEN", 1);
                    jmainArray.Add(jmain);
                }

                jmdvr.Add("MAIN", jmainArray);

                JObject jar = new JObject();
                jar.Add("EN", 1);
                jar.Add("RM", 0);

                jmdvr.Add("AR", jar);
                jparameter.Add("MDVR", jmdvr);

                session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Sleep(5000);

                for (int i = 0; i < session.GetChannelCount(); i++)
                {
                    Assert.IsTrue(session.isChannelRecording(StreamType.MAIN_STREAM, i));
                    Assert.IsFalse(session.isChannelRecording(StreamType.MIRROR_STREAM, i));
                    Console.WriteLine("channel[{0}] main stream = {1} mirror = {2}",
                        i,
                        session.isChannelRecording(StreamType.MAIN_STREAM, i),
                        session.isChannelRecording(StreamType.MIRROR_STREAM, i));
                }

                session.Logout();
            }
        }
		[Ignore("111111")]
		[Test]
        public void SubRecordEnable()
        {
            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                //主码流使能关闭，此通道预览黑屏，主从都不录像

                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();


                JArray jmainArray = new JArray();

                for (int i = 0; i < session.GetChannelCount(); i++)
                {
                    JObject jmain = new JObject();
                    jmain.Add("VEN", 0);
                    jmainArray.Add(jmain);
                }

                jmdvr.Add("MAIN", jmainArray);
                jparameter.Add("MDVR", jmdvr);

                session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Sleep(5000);

                for (int i = 0; i < session.GetChannelCount(); i++)
                {
                    Assert.IsFalse(session.isChannelRecording(StreamType.MAIN_STREAM, i));
                    Assert.IsFalse(session.isChannelRecording(StreamType.SUB_STREAM, i));
                    Assert.IsFalse(session.isChannelRecording(StreamType.MIRROR_STREAM, i));
                    Console.WriteLine("{0}main = {1}, sub = {2} mirror = {3}",
                        i,
                        session.isChannelRecording(StreamType.MAIN_STREAM, i),
                        session.isChannelRecording(StreamType.SUB_STREAM, i),
                        session.isChannelRecording(StreamType.MIRROR_STREAM, i));
                }


                //子码流通道使能关闭，主码流录像，无子码流录像

                jparameter = new JObject();
                jmdvr = new JObject();

                jmainArray = new JArray();

                for (int i = 0; i < session.GetChannelCount(); i++)
                {
                    JObject jmain = new JObject();
                    jmain.Add("VEN", 1);
                    jmainArray.Add(jmain);
                }

                jmdvr.Add("MAIN", jmainArray);

                JObject jar = new JObject();
                JArray jvecArray = new JArray();

                for (int i = 0; i < session.GetChannelCount(); i++)
                {
                    JObject jvec = new JObject();
                    jvec["VEN"] = 0;
                    jvecArray.Add(jvec);
                }


                jar.Add("EN", 1);
                jar.Add("RM", 0);
                jar.Add("VEC", jvecArray);

                jmdvr.Add("AR", jar);
                jparameter.Add("MDVR", jmdvr);

                session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Sleep(5000);

                for (int i = 0; i < session.GetChannelCount(); i++)
                {
                    Assert.IsTrue(session.isChannelRecording(StreamType.MAIN_STREAM, i));
                    Assert.IsFalse(session.isChannelRecording(StreamType.SUB_STREAM, i));
                    Console.WriteLine("channel main stream[{0}] = {1}", i, session.isChannelRecording(StreamType.MAIN_STREAM, i));
                    Console.WriteLine("channel sub stream[{0}] = {1}", i, session.isChannelRecording(StreamType.SUB_STREAM, i));
                }

                session.Logout();
            }
        }
    }
}
