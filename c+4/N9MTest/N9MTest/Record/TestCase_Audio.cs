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
	[Ignore("11111")]
	[TestFixture]
    class TestCase_Audio:TestCase_Basecase
    {
		[Ignore("11111")]
		[Test]
        public void ADPCMEncode()
        {
            foreach (string ip in IPList)
            {
                Console.WriteLine("ip ={0}", ip);

                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");


                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                JObject jasp = new JObject();
                jmdvr["ASP"] = "?";
                jparameter["MDVR"] = jmdvr;

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);


                jparameter = new JObject();
                jmdvr = new JObject();
                jasp = new JObject();

                jasp["AT"] = (int)AudioEncodeFormat.ADPCM;
                jmdvr["ASP"] = jasp;
                jparameter["MDVR"] = jmdvr;

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);
            }
        }
		[Ignore("11111")]
		[Test]
        public void G711AEncode()
        {
            foreach (string ip in IPList)
            {
                Console.WriteLine("ip ={0}", ip);

                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");


                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                JObject jasp = new JObject();
                jmdvr["ASP"] = "?";
                jparameter["MDVR"] = jmdvr;

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);


                jparameter = new JObject();
                jmdvr = new JObject();
                jasp = new JObject();

                jasp["AT"] = (int)AudioEncodeFormat.G711A;
                jmdvr["ASP"] = jasp;
                jparameter["MDVR"] = jmdvr;

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);
            }
        }

        [Test]
        public void G711UEncode()
        {
            foreach (string ip in IPList)
            {
                Console.WriteLine("ip ={0}", ip);

                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                JObject jasp = new JObject();
                jmdvr["ASP"] = "?";
                jparameter["MDVR"] = jmdvr;

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);


                jparameter = new JObject();
                jmdvr = new JObject();
                jasp = new JObject();

                jasp["AT"] = (int)AudioEncodeFormat.G711U;
                jmdvr["ASP"] = jasp;
                jparameter["MDVR"] = jmdvr;

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);
            }
        }
    }
}
