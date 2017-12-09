using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using N9MTest.SDK.http;
using Newtonsoft.Json.Linq;
using N9MTest.SDK.net;
using Newtonsoft.Json;
using Util;

namespace N9MTest.Upgrade
{
    [TestFixture]
    class TestCase_UpgradeProtect:TestCase_Basecase
    {
        [Test]
        public void UpgradeCheck()
        {
            int waittime = 300;

            if (this.CFG.gNode("Upgrade/UpgradeCheck/waittime") != null)
            {
                waittime = Convert.ToInt32(this.CFG.gNode("Upgrade/UpgradeCheck/waittime").InnerText);
            }

            foreach (string ip in IPList)
            {
                //首先升级空内容的文件
                do
                {
                    Console.WriteLine("升级空内容的文件");
                    string upgradeFilePath = this.CFG.gNode("Upgrade/UpgradeCheck/empty-firmware").InnerText;
                    N9MHttpSession session = new N9MHttpSession(ip, webport);
                    session.Login("/logincheck.rsp?type=1", "admin", "");

                    string resp = session.Get("/device.rsp?opt=sys&cmd=version");
                    JObject jresp = JObject.Parse(resp);
                    Assert.IsNotNull(jresp);
                    Assert.IsNotNull(jresp["result"]);
                    Assert.AreEqual(0, Convert.ToInt32(jresp["result"].ToString()));
                    SysVersInfo sysInfo = JsonConvert.DeserializeObject<SysVersInfo>(jresp.ToString());
                    Assert.IsNotNull(sysInfo.software);

                    string filename = Versions.GetFirmwareVersion(upgradeFilePath);
                    Console.WriteLine("filename = {0}", filename);
                    Assert.AreNotEqual(sysInfo, filename);
                    Console.WriteLine("发送升级文件{0}...", filename);
                    resp = session.PostFile("/upload.rsp?filetype=upgrade", upgradeFilePath);
                    Console.WriteLine("resp = {0}", resp.Substring(0, resp.IndexOf("}") + 1));

                    jresp = JObject.Parse(resp.Substring(0, resp.IndexOf("}") + 1));
                    Assert.IsNotNull(jresp, "传递升级文件回复格式无效");
                    Assert.IsNotNull(jresp["result"], "传递升级文件结果未知");
                    Assert.AreEqual(3, Convert.ToInt32(jresp["result"].ToString()), "升级文件校验失败");

                } while (false);

                Sleep(3000);

                //升级同样的文件
                do
                {
                    Console.WriteLine("升级同样的文件");
                    string upgradeFilePath = this.CFG.gNode("Upgrade/UpgradeCheck/same-firmware").InnerText;
                    N9MHttpSession session = new N9MHttpSession(ip, webport);
                    session.Login("/logincheck.rsp?type=1", "admin", "");

                    string resp = session.Get("/device.rsp?opt=sys&cmd=version");
                    JObject jresp = JObject.Parse(resp);
                    Assert.IsNotNull(jresp);
                    Assert.IsNotNull(jresp["result"]);
                    Assert.AreEqual(0, Convert.ToInt32(jresp["result"].ToString()));
                    SysVersInfo sysInfo = JsonConvert.DeserializeObject<SysVersInfo>(jresp.ToString());
                    Assert.IsNotNull(sysInfo.software);

                    string filename = Versions.GetFirmwareVersion(upgradeFilePath);
                    Console.WriteLine("filename = {0}", filename);
                    Assert.AreNotEqual(sysInfo, filename);
                    Console.WriteLine("发送升级文件{0}...", filename);
                    resp = session.PostFile("/upload.rsp?filetype=upgrade", upgradeFilePath);
                    Console.WriteLine("resp = {0}", resp.Substring(0, resp.IndexOf("}") + 1));

                    jresp = JObject.Parse(resp.Substring(0, resp.IndexOf("}") + 1));
                    Assert.IsNotNull(jresp, "传递升级文件回复格式无效");
                    Assert.IsNotNull(jresp["result"], "传递升级文件结果未知");
                    Assert.AreEqual(2, Convert.ToInt32(jresp["result"].ToString()), "升级文件校验失败");

                } while (false);

                Sleep(3000);


                //升级无效的升级文件
                do
                {
                    Console.WriteLine("升级无效的文件");
                    string upgradeFilePath = this.CFG.gNode("Upgrade/UpgradeCheck/invalid-firmware").InnerText;
                    N9MHttpSession session = new N9MHttpSession(ip, webport);
                    session.Login("/logincheck.rsp?type=1", "admin", "");

                    string resp = session.Get("/device.rsp?opt=sys&cmd=version");
                    JObject jresp = JObject.Parse(resp);
                    Assert.IsNotNull(jresp);
                    Assert.IsNotNull(jresp["result"]);
                    Assert.AreEqual(0, Convert.ToInt32(jresp["result"].ToString()));
                    SysVersInfo sysInfo = JsonConvert.DeserializeObject<SysVersInfo>(jresp.ToString());
                    Assert.IsNotNull(sysInfo.software);

                    string filename = Versions.GetFirmwareVersion(upgradeFilePath);
                    Console.WriteLine("filename = {0}", filename);
                    Assert.AreNotEqual(sysInfo, filename);
                    Console.WriteLine("发送升级文件{0}...", filename);
                    resp = session.PostFile("/upload.rsp?filetype=upgrade", upgradeFilePath);
                    Console.WriteLine("resp = {0}", resp.Substring(0, resp.IndexOf("}") + 1));

                    jresp = JObject.Parse(resp.Substring(0, resp.IndexOf("}") + 1));
                    Assert.IsNotNull(jresp, "传递升级文件回复格式无效");
                    Assert.IsNotNull(jresp["result"], "传递升级文件结果未知");
                    Assert.AreEqual(3, Convert.ToInt32(jresp["result"].ToString()), "升级文件校验失败");

                } while (false);
            }
        }
    }
}
