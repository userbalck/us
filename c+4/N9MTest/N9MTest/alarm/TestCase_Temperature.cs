using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using N9MTest.SDK.net;
using Newtonsoft.Json.Linq;
using Util;

namespace N9MTest.Alarm
{
    [TestFixture]
    class TestCase_Temperature:TestCase_Basecase
    {
		
        [Test]
        public void HDDTemperature()
        {
            int interval = Convert.ToInt32(this.CFG.gNode("Alarm/HDDTemperature/interval").InnerText);
            int max = Convert.ToInt32(this.CFG.gNode("Alarm/HDDTemperature/max").InnerText);
            int min = Convert.ToInt32(this.CFG.gNode("Alarm/HDDTemperature/min").InnerText);

            Console.WriteLine("温度范围是{0}-{1}", min, max);
		


            foreach (string ip in IPList)
            {
                 Console.WriteLine("ip = {0}", ip);
                long starttime = ExactTime.GetExactTime();

                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                do
                {
                    JObject jparameter = new JObject();
                    jparameter["DATE"] = 0;
                    jparameter["TYPE"] = 1;
                    jparameter["INFO"] = "?";

                    JObject jresp = session.SendCommand(Module.DEVEMM, Operation.GETYUNWEIINFO, jparameter);

                    Assert.IsNotNull(jresp);
                    Assert.IsNotNull(jresp["ERRORCODE"]);
                    Assert.IsNotNull(jresp["INFO"]["T"]["I"]);

                    int nInnerTemp = Convert.ToInt32(jresp["INFO"]["T"]["I"].ToString()) / 100;

                    Console.WriteLine("当前温度是{0}", nInnerTemp);

                    Assert.GreaterOrEqual(nInnerTemp, min, "当前温度 {0} 低于下限值{1}", nInnerTemp, min);
                    Assert.LessOrEqual(nInnerTemp, max, "当前温度 {0} 高于下限值{1}", nInnerTemp, max);

                    Sleep(interval * 1000);
                } while (ExactTime.GetExactTime() - starttime <= 60);

                session.Logout();
            }
        }
    }
}
