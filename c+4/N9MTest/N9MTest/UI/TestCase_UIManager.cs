using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using N9MTest.SDK.http;
using Newtonsoft.Json.Linq;
using N9MTest.SDK.net;
using Util;
using Newtonsoft.Json;
using System.Threading;

namespace N9MTest.UI
{
    [TestFixture]
    class TestCase_UIManager:TestCase_Basecase
    {
		[Ignore("1111")]
        [Test]
        public void UIconformity()
        {
            Assert.IsTrue(false, "当前N9M协议不支持导入导出语言包");
        }

        [Test]
        public void OneLanguageUpgrade()
        {
            int waittime = 300;

            if (this.CFG.gNode("UI/OneLanguageUpgrade/waittime") != null)
            {
                waittime = Convert.ToInt32(this.CFG.gNode("UI/OneLanguageUpgrade/waittime").InnerText);
            }

            //用例C:\\Users\\Administrator\\Desktop\\X7A_V222_T170109.02_C0010
            string upgradeFilePath = this.CFG.gNode("UI/OneLanguageUpgrade/chs-firmware").InnerText;

            string filename = Versions.GetFirmwareVersion(upgradeFilePath);

            foreach (string ip in IPList)
            {
                Console.WriteLine("ip ={0}", ip);

                LANT initLant = LANT.undefined;

                //获取当前系统语言
                do
                {
                    N9MSession session = new N9MSession(ip, port);
                    Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                    JObject jparameter = new JObject();
                    JObject jgsp = new JObject();
                    JObject jmdvr = new JObject();

                    jgsp["LANT"] = "?";
                    jmdvr["GSP"] = jgsp;
                    jparameter["MDVR"] = jmdvr;

                    JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);

                    Assert.IsNotNull(jresp);
                    Assert.IsNotNull(jresp["MDVR"]["GSP"]["LANT"]);

                    initLant = (LANT)Convert.ToInt32(jresp["MDVR"]["GSP"]["LANT"].ToString());

                    session.Logout();

                } while (false);

                //升级中文版软件
                LANT firmwareLant = LANT.zh_CN;

                do
                {
                    N9MHttpSession session = new N9MHttpSession(ip, webport);
                    session.Login("/logincheck.rsp?type=1", username, password);

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

                } while (false);

                Sleep(waittime * 1000);

                //判定升级是否成功
                do
                {
                    N9MHttpSession session = new N9MHttpSession(ip, webport);

                    session.Login("/logincheck.rsp?type=1", username, password);
                    string resp = session.Get("/device.rsp?opt=sys&cmd=version");
                    JObject jresp = JObject.Parse(resp);
                    SysVersInfo sysInfo = JsonConvert.DeserializeObject<SysVersInfo>(jresp.ToString());
                    Assert.AreEqual(sysInfo.software, filename, "升级后版本号对比失败");

                } while (false);


                //获取升级后的设备的系统语言
                do
                {
                    N9MSession session = new N9MSession(ip, port);
                    Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                    JObject jparameter = new JObject();
                    JObject jgsp = new JObject();
                    JObject jmdvr = new JObject();

                    jgsp["LANT"] = "?";
                    jmdvr["GSP"] = jgsp;
                    jparameter["MDVR"] = jmdvr;

                    JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);

                    Assert.IsNotNull(jresp);
                    Assert.IsNotNull(jresp["MDVR"]["GSP"]["LANT"]);

                    LANT lant = (LANT)Convert.ToInt32(jresp["MDVR"]["GSP"]["LANT"].ToString());

                    Assert.AreEqual(firmwareLant, lant);

                    session.Logout();

                } while (false);

                upgradeFilePath = this.CFG.gNode("UI/OneLanguageUpgrade/eng-firmware").InnerText;

                filename = Versions.GetFirmwareVersion(upgradeFilePath);

                firmwareLant = LANT.en_US;

                //升级英文版软件

                do
                {
                    N9MHttpSession session = new N9MHttpSession(ip, webport);
                    session.Login("/logincheck.rsp?type=1", username, password);

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

                } while (false);

                Sleep(waittime * 1000);

                //判定升级是否成功
                do
                {
                    N9MHttpSession session = new N9MHttpSession(ip, webport);

                    session.Login("/logincheck.rsp?type=1", username, password);
                    string resp = session.Get("/device.rsp?opt=sys&cmd=version");
                    JObject jresp = JObject.Parse(resp);
                    SysVersInfo sysInfo = JsonConvert.DeserializeObject<SysVersInfo>(jresp.ToString());
                    Assert.AreEqual(sysInfo.software, filename, "升级后版本号对比失败");

                } while (false);


                //获取当前的语言
                do
                {
                    N9MSession session = new N9MSession(ip, port);
                    Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                    JObject jparameter = new JObject();
                    JObject jgsp = new JObject();
                    JObject jmdvr = new JObject();

                    jgsp["LANT"] = "?";
                    jmdvr["GSP"] = jgsp;
                    jparameter["MDVR"] = jmdvr;

                    JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);

                    Assert.IsNotNull(jresp);
                    Assert.IsNotNull(jresp["MDVR"]["GSP"]["LANT"]);

                    LANT lant = (LANT)Convert.ToInt32(jresp["MDVR"]["GSP"]["LANT"].ToString());

                    Assert.AreEqual(firmwareLant, lant);

                    session.Logout();

                } while (false);
            }
        }
		[Ignore("1111")]
		[Test]
        public void SwitchLanguage()
        {
            Assert.IsTrue(false, "当前N9M协议不支持导入导出语言包");
        }
		[Ignore("1111")]
		[Test]
        public void UserExperience()
        {
            Assert.IsTrue(false, "当前N9M协议不支持导入导出语言包");
        }
		[Ignore("1111")]
		[Test]
        public void LanguageSwitch()
        {
            string language = this.CFG.gNode("UI/LanguageSwitch/language").InnerText;

            Console.WriteLine("language = {0}", language);

            LANT lant = (LANT)Enum.Parse(typeof(LANT), language);

            foreach (string ip in IPList)
            {
                //获取并切换语言，恢复出厂设置，再次获取当前语言
                do
                {

                    //首先获取当前语言
                    N9MSession session = new N9MSession(ip, port);
                    Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                    JObject jparameter = new JObject();
                    JObject jgsp = new JObject();
                    JObject jmdvr = new JObject();

                    jgsp["LANT"] = "?";
                    jmdvr["GSP"] = jgsp;
                    jparameter["MDVR"] = jmdvr;

                    JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);

                    Assert.IsNotNull(jresp);
                    Assert.IsNotNull(jresp["MDVR"]["GSP"]["LANT"]);

                    LANT lant_before = (LANT)Convert.ToInt32(jresp["MDVR"]["GSP"]["LANT"].ToString());

                    jparameter = new JObject();
                    jparameter["PARAMMASK"] = 0xFFFFFFFFFFFFFFFF;
                    jresp = session.SendCommand(Module.DEVEMM, Operation.SETRESTOREDEFAULT, jparameter);

                    Assert.IsNotNull(jresp);
                    Assert.IsNotNull(jresp["ERRORCODE"]);
                    Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));

                    jparameter = new JObject();
                    jgsp = new JObject();
                    jmdvr = new JObject();

                    jgsp["LANT"] = "?";
                    jmdvr["GSP"] = jgsp;
                    jparameter["MDVR"] = jmdvr;

                    jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);

                    Assert.IsNotNull(jresp);
                    Assert.IsNotNull(jresp["MDVR"]["GSP"]["LANT"]);

                    LANT lant_after = (LANT)Convert.ToInt32(jresp["MDVR"]["GSP"]["LANT"].ToString());

                    Assert.AreEqual(lant_before, lant_after);

                } while (false);

                //获取语言，导出配置文件 切换语言，导入配置文件，再次获取当前语言
                do
                {
                    N9MSession session = new N9MSession(ip, port);
                    session.Login(username, password);

                    JObject jparameter = new JObject();
                    JObject jgsp = new JObject();
                    JObject jmdvr = new JObject();

                    jgsp["LANT"] = "?";
                    jmdvr["GSP"] = jgsp;
                    jparameter["MDVR"] = jmdvr;

                    JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);

                    Assert.IsNotNull(jresp);
                    Assert.IsNotNull(jresp["MDVR"]["GSP"]["LANT"]);

                    LANT lant_before = (LANT)Convert.ToInt32(jresp["MDVR"]["GSP"]["LANT"].ToString());

                    N9MHttpSession httpSession = new N9MHttpSession(ip, webport);
                    httpSession.Login("/logincheck.rsp?type=1", username, password);

                    string path = Environment.CurrentDirectory;

                    httpSession.GetFile("/download.rsp", ref path);

                    Console.WriteLine("ref path = {0}", path);


                    jparameter = new JObject();
                    jgsp = new JObject();
                    jmdvr = new JObject();

                    jgsp["LANT"] = (int)LANT.es_ES;
                    jmdvr["GSP"] = jgsp;
                    jparameter["MDVR"] = jmdvr;
                    jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);

                    Assert.IsNotNull(jresp);
                    Assert.IsNotNull(jresp["MDVR"]["GSP"]["LANT"]);

                    string resp = httpSession.PostFile("/upload.rsp?filetype=importparam", path);

                    Console.WriteLine("ref resp = {0}", resp);

                    jparameter = new JObject();
                    jgsp = new JObject();
                    jmdvr = new JObject();

                    jgsp["LANT"] = "?";
                    jmdvr["GSP"] = jgsp;
                    jparameter["MDVR"] = jmdvr;

                    jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);

                    Assert.IsNotNull(jresp);
                    Assert.IsNotNull(jresp["MDVR"]["GSP"]["LANT"]);

                    LANT lant_after = (LANT)Convert.ToInt32(jresp["MDVR"]["GSP"]["LANT"].ToString());

                    Assert.AreEqual(lant_before, lant_after);

                } while (false);

                //重启设备，2分钟后再次当前语言，重启前后的语言一样则OK
                do
                {
                    //获取当前语言
                    N9MSession session = new N9MSession(ip, port);
                    session.Login(username, password);

                    JObject jparameter = new JObject();
                    JObject jgsp = new JObject();
                    JObject jmdvr = new JObject();

                    jgsp["LANT"] = "?";
                    jmdvr["GSP"] = jgsp;
                    jparameter["MDVR"] = jmdvr;

                    JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);

                    Assert.IsNotNull(jresp);
                    Assert.IsNotNull(jresp["MDVR"]["GSP"]["LANT"]);

                    LANT lant_before = (LANT)Convert.ToInt32(jresp["MDVR"]["GSP"]["LANT"].ToString());

                    //发送重启指令
                    JObject jparamater = new JObject();
                    jparamater["CMDTYPE"] = 0;

                    jresp = session.SendCommand(Module.DEVEMM, Operation.SETCONTROLDEVCMD, jparamater);
                    Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));

                    session.Logout();

                    Sleep(120 * 1000);

                    session = new N9MSession(ip, port);
                    session.Login(username, password);

                    jparameter = new JObject();
                    jgsp = new JObject();
                    jmdvr = new JObject();

                    jgsp["LANT"] = "?";
                    jmdvr["GSP"] = jgsp;
                    jparameter["MDVR"] = jmdvr;

                    jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);

                    Assert.IsNotNull(jresp);
                    Assert.IsNotNull(jresp["MDVR"]["GSP"]["LANT"]);

                    LANT lant_after = (LANT)Convert.ToInt32(jresp["MDVR"]["GSP"]["LANT"].ToString());

                    Assert.AreEqual(lant_before, lant_after);

                    session.Logout();

                } while (false);
            }
        }
		[Ignore("1111")]
		[Test]
        public void MultiLanguageUpgrade()
        {
            int waittime = 300;

            if (this.CFG.gNode("UI/MultiLanguageUpgrade/waittime") != null)
            {
                waittime = Convert.ToInt32(this.CFG.gNode("UI/MultiLanguageUpgrade/waittime").InnerText);
            }

            string upgradeFilePath = this.CFG.gNode("UI/MultiLanguageUpgrade/multi-firmware").InnerText;
            string filename = Versions.GetFirmwareVersion(upgradeFilePath);

            foreach (string ip in IPList)
            {
                //首先判定语言是否葡萄牙语
                do
                {
                    N9MSession session = new N9MSession(ip, port);
                    Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                    JObject jparameter = new JObject();
                    JObject jgsp = new JObject();
                    JObject jmdvr = new JObject();

                    jgsp["LANT"] = "?";
                    jmdvr["GSP"] = jgsp;
                    jparameter["MDVR"] = jmdvr;

                    JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);

                    Assert.IsNotNull(jresp);
                    Assert.IsNotNull(jresp["MDVR"]["GSP"]["LANT"]);

                    LANT lant = (LANT)Convert.ToInt32(jresp["MDVR"]["GSP"]["LANT"].ToString());

                    Assert.AreEqual(LANT.pt_PT, lant);

                    session.Logout();

                } while (false);

                //升级多国语版本(包括葡西英俄土<默认英语>)
                do
                {
                    N9MHttpSession session = new N9MHttpSession(ip, webport);
                    session.Login("/logincheck.rsp?type=1", username, password);

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

                    session.Logout();

                } while (false);

                Sleep(waittime * 1000);

                //判定升级是否成功
                do
                {
                    N9MHttpSession session = new N9MHttpSession(ip, webport);

                    session.Login("/logincheck.rsp?type=1", username, password);
                    string resp = session.Get("/device.rsp?opt=sys&cmd=version");
                    JObject jresp = JObject.Parse(resp);
                    SysVersInfo sysInfo = JsonConvert.DeserializeObject<SysVersInfo>(jresp.ToString());
                    Assert.AreEqual(sysInfo.software, filename, "升级后版本号对比失败");

                } while (false);

                //获取语言，判定当前是否为葡萄牙语
                do
                {
                    N9MSession session = new N9MSession(ip, port);
                    Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                    JObject jparameter = new JObject();
                    JObject jgsp = new JObject();
                    JObject jmdvr = new JObject();

                    jgsp["LANT"] = "?";
                    jmdvr["GSP"] = jgsp;
                    jparameter["MDVR"] = jmdvr;

                    JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);

                    Assert.IsNotNull(jresp);
                    Assert.IsNotNull(jresp["MDVR"]["GSP"]["LANT"]);

                    LANT lant = (LANT)Convert.ToInt32(jresp["MDVR"]["GSP"]["LANT"].ToString());

                    Assert.AreEqual(LANT.pt_PT, lant);

                    session.Logout();

                } while (false);

                upgradeFilePath = this.CFG.gNode("UI/MultiLanguageUpgrade/es-firmware").InnerText;
                filename = Versions.GetFirmwareVersion(upgradeFilePath);

                //升级西班牙语版本 包括西语、英文<默认西语>
                do
                {
                    N9MHttpSession session = new N9MHttpSession(ip, webport);
                    session.Login("/logincheck.rsp?type=1", username, password);

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

                    session.Logout();

                } while (false);

                Sleep(waittime * 1000);

                //判定升级是否成功
                do
                {
                    N9MHttpSession session = new N9MHttpSession(ip, webport);

                    session.Login("/logincheck.rsp?type=1", username, password);
                    string resp = session.Get("/device.rsp?opt=sys&cmd=version");
                    JObject jresp = JObject.Parse(resp);
                    SysVersInfo sysInfo = JsonConvert.DeserializeObject<SysVersInfo>(jresp.ToString());
                    Assert.AreEqual(sysInfo.software, filename, "升级后版本号对比失败");

                } while (false);

                //获取语言，判定当前是否为西班牙语
                do
                {
                    N9MSession session = new N9MSession(ip, port);
                    session.Login(username, password);

                    JObject jparameter = new JObject();
                    JObject jgsp = new JObject();
                    JObject jmdvr = new JObject();

                    jgsp["LANT"] = "?";
                    jmdvr["GSP"] = jgsp;
                    jparameter["MDVR"] = jmdvr;

                    JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);

                    Assert.IsNotNull(jresp);
                    Assert.IsNotNull(jresp["MDVR"]["GSP"]["LANT"]);

                    LANT lant = (LANT)Convert.ToInt32(jresp["MDVR"]["GSP"]["LANT"].ToString());

                    Assert.AreEqual(LANT.es_ES, lant);

                    session.Logout();

                } while (false);
            }
        }
    }
}
