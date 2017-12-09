using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Xml;
using Util;
using N9MTest.SDK.http;
using Newtonsoft.Json.Linq;
using N9MTest.SDK.net;
using Newtonsoft.Json;

namespace N9MTest.Performance
{

	[TestFixture]
    class TestCase_Performance:TestCase_Basecase
    {
	
        [Test]
        public void PerformanceUpgrade()
        {
            int waittime = 300;
            List<string> swList = new List<string>();

            //用例C:\\Users\\Administrator\\Desktop\\X7A_V222_T170109.02_C0010
            XmlNodeList list = this.CFG.gNodes("Upgrade/PerformanceUpgrade/firmware");

            if (this.CFG.gNode("Upgrade/PerformanceUpgrade/waittime") != null)
            {
                waittime = Convert.ToInt32(this.CFG.gNode("Upgrade/PerformanceUpgrade/waittime").InnerText);
            }

            Console.WriteLine("waittime = {0}", waittime);


            for (int i = 0; i < list.Count; i++)
            {
                swList.Add(list[i].InnerText);
            }

            int interval = 5;
            int loop = 5;

            interval = Convert.ToInt32(this.CFG.gNode("Upgrade/PerformanceUpgrade/interval").InnerText);
            loop = Convert.ToInt32(this.CFG.gNode("Upgrade/PerformanceUpgrade/loop").InnerText);

            Console.WriteLine("interval = {0}, loop = {1}", interval, loop);

            for (int i = 0; i < loop; i++)
            {
                //当前要升级的版本路径
                string upgradeFilePath = swList[i % swList.Count];
                string filename = Versions.GetFirmwareVersion(upgradeFilePath);

                long time = ExactTime.GetExactTime();

                foreach (string ip in IPList)
                {
                    N9MHttpSession session = new N9MHttpSession(ip, webport);
                    session.Login("/logincheck.rsp?type=1", "admin", "");

                    string resp = session.Get("/device.rsp?opt=sys&cmd=version");
                    JObject jresp = JObject.Parse(resp);
                    Assert.IsNotNull(jresp);
                    Assert.IsNotNull(jresp["result"]);
                    Assert.AreEqual(0, Convert.ToInt32(jresp["result"].ToString()));
                    SysVersInfo sysInfo = JsonConvert.DeserializeObject<SysVersInfo>(jresp.ToString());
                    Assert.IsNotNull(sysInfo.software);

                    Console.WriteLine("filename = {0}", filename);
                    Assert.AreNotEqual(sysInfo, filename);
                    Console.WriteLine("发送升级文件{0}...", filename);
                    resp = session.PostFile("/upload.rsp?filetype=upgrade", upgradeFilePath);
                    Console.WriteLine("resp = {0}", resp.Substring(0, resp.IndexOf("}") + 1));

                    jresp = JObject.Parse(resp.Substring(0, resp.IndexOf("}") + 1));
                    Assert.IsNotNull(jresp, "传递升级文件回复格式无效");
                    Assert.IsNotNull(jresp["result"], "传递升级文件结果未知");
                    Assert.AreEqual(0, Convert.ToInt32(jresp["result"].ToString()), "升级文件校验失败");

                }

                Sleep(waittime * 1000);

                foreach (string ip in IPList)
                {
                    N9MHttpSession session = new N9MHttpSession(ip, webport);

                    session.Login("/logincheck.rsp?type=1", "admin", "");
                    string resp = session.Get("/device.rsp?opt=sys&cmd=version");
                    JObject jresp = JObject.Parse(resp);
                    SysVersInfo sysInfo = JsonConvert.DeserializeObject<SysVersInfo>(jresp.ToString());
                    Assert.AreEqual(sysInfo.software, filename, "升级后版本号对比失败");
                }

                Sleep(interval * 1000);
            }
        }
    }
}
