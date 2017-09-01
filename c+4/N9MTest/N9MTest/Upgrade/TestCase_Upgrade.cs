using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RM;
using N9MTest.commons;
using N9MTest.SDK.http;
using Newtonsoft.Json.Linq;
using N9MTest.SDK.net;
using Newtonsoft.Json;
using System.Threading;
using Util;
using System.Xml;
using N9MTest.SDK;

namespace N9MTest.Upgrade
{
    [TestFixture]
    class TestCase_Upgrade: TestCase_Basecase
    {
        private static int mTaskId = 0;

        [Test]
        public void UpgradeWeb()
        {
            int waittime = 300;
            //用例C:\\Users\\Administrator\\Desktop\\X7A_V222_T170109.02_C0010
            string upgradeFilePath = this.CFG.gNode("Upgrade/UpgradeWeb/firmware").InnerText;

            if (this.CFG.gNode("Upgrade/UpgradeWeb/waittime") != null)
            {
                waittime = Convert.ToInt32(this.CFG.gNode("Upgrade/UpgradeWeb/waittime").InnerText);
            }

            Console.WriteLine("waittime = {0}", waittime);
            

            string filename = Versions.GetFirmwareVersion(upgradeFilePath);

            foreach (string ip in IPList)
            {
                Console.WriteLine("ip ={0}", ip);
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

            //    session.Logout();
            }

            Sleep(waittime * 1000);

            foreach (string ip in IPList)
            {
                N9MHttpSession session = new N9MHttpSession(ip, webport);
                
                session.Login("/logincheck.rsp?type=1", username, password);
                string resp = session.Get("/device.rsp?opt=sys&cmd=version");
                JObject jresp = JObject.Parse(resp);
                SysVersInfo sysInfo = JsonConvert.DeserializeObject<SysVersInfo>(jresp.ToString());
                Assert.AreEqual(sysInfo.software, filename, "升级后版本号对比失败");
            }
        }
		
        [Test]
        public void UpgradeBatch()
        {
            int waittime = 300;
            //用例C:\\Users\\Administrator\\Desktop\\X7A_V222_T170109.02_C0010
            string upgradeFilePath = this.CFG.gNode("Upgrade/UpgradeBatch/firmware").InnerText;

            if (this.CFG.gNode("Upgrade/UpgradeBatch/waittime") != null)
            {
                waittime = Convert.ToInt32(this.CFG.gNode("Upgrade/UpgradeBatch/waittime").InnerText);
            }

            Console.WriteLine("waittime = {0}", waittime);

            string filename = Versions.GetFirmwareVersion(upgradeFilePath);

            foreach (string ip in IPList)
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

           //     session.Logout();
            }

            Sleep(waittime * 1000);

            foreach (string ip in IPList)
            {
                N9MHttpSession session = new N9MHttpSession(ip, webport);

                session.Login("/logincheck.rsp?type=1", username, password);
                string resp = session.Get("/device.rsp?opt=sys&cmd=version");
                JObject jresp = JObject.Parse(resp);
                SysVersInfo sysInfo = JsonConvert.DeserializeObject<SysVersInfo>(jresp.ToString());
                Assert.AreEqual(sysInfo.software, filename, "升级后版本号对比失败");
            }
        }

        [Test]
        public void UpgradeFW()
        {
            int waittime = 300;

            if (this.CFG.gNode("Upgrade/UpgradeFW/waittime") != null)
            {
                waittime = Convert.ToInt32(this.CFG.gNode("Upgrade/UpgradeFW/waittime").InnerText);
            }

            Console.WriteLine("waittime = {0}", waittime);

            List<string> swList = new List<string>();

            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);

                N9MSession session = new N9MSession(ip, port);
                session.Login(username, password);

                JObject jparameter = new JObject();

                JObject jresp = session.SendCommand(Module.DEVEMM, Operation.GETDEVVERSIONINFO, null);

                Assert.IsNotNull(jresp);
                Assert.IsNotNull(jresp["ERRORCODE"]);
                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));
                Assert.IsNotNull(jresp["DEVINFO"]);

                DevInfo devinfo = JsonConvert.DeserializeObject<DevInfo>(jresp["DEVINFO"].ToString());

                string mainVersion_before = devinfo.mainVersion;

                jparameter = new JObject();

                jparameter["REQUESTTYPE"] = 0;
                jparameter["CMD"] = 1;
                jparameter["STORAGE"] = (int)StorageType.STORAGE_INTERNAL_SDCARD;
                jparameter["UPEVENTTYPE"] = (int)UploadEventType.UPLOAD_EVENTTYPE_UPGRADE;
                jparameter["SSRC"] = mTaskId++;

                jresp = session.SendCommand(Module.DEVEMM, Operation.REQUESTCTRLEVENT, jparameter);

                Assert.IsNotNull(jresp);
                Assert.IsNotNull(jresp["ERRORCODE"]);
                Assert.AreEqual(0, Convert.ToUInt32(jresp["ERRORCODE"].ToString()));

                session.Logout();

                Sleep(waittime * 1000);

                session = new N9MSession(ip, port);
                session.Login(username, password);

                jparameter = new JObject();

                jresp = session.SendCommand(Module.DEVEMM, Operation.GETDEVVERSIONINFO, null);

                Assert.IsNotNull(jresp);
                Assert.IsNotNull(jresp["ERRORCODE"]);
                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));
                Assert.IsNotNull(jresp["DEVINFO"]);

                devinfo = JsonConvert.DeserializeObject<DevInfo>(jresp["DEVINFO"].ToString());

                string mainVersion_after = devinfo.mainVersion;

                Assert.AreEqual(mainVersion_before, mainVersion_after);

                session.Logout();

            }
        }
		[Ignore("11111")]
		[Test]
        public void UpgradeCP4()
        {
            int waittime = 300;

            if (this.CFG.gNode("Upgrade/UpgradeCP4/waittime") != null)
            {
                waittime = Convert.ToInt32(this.CFG.gNode("Upgrade/UpgradeCP4/waittime").InnerText);
            }

            Console.WriteLine("waittime = {0}", waittime);

            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);

                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                JObject jparameter = new JObject();

                JObject jresp = session.SendCommand(Module.DEVEMM, Operation.GETVERBYUSED, null);

                Assert.IsNotNull(jresp);
                Assert.IsNotNull(jresp["ERRORCODE"]);
                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));
                Assert.IsNotNull(jresp["VERS"]);

                string cp4Version_before = jresp["VERS"].ToString();



                jresp = session.SendCommand(Module.DEVEMM, Operation.GETDEVVERSIONINFO, null);

                Assert.IsNotNull(jresp);
                Assert.IsNotNull(jresp["ERRORCODE"]);
                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));
                Assert.IsNotNull(jresp["DEVINFO"]);

                DevInfo devinfo = JsonConvert.DeserializeObject<DevInfo>(jresp["DEVINFO"].ToString());

                string temp = devinfo.cp4.Substring(devinfo.cp4.Length - 6);

                Assert.AreNotEqual(cp4Version_before.Substring(cp4Version_before.Length - 6), devinfo.cp4.Substring(devinfo.cp4.Length - 6), "升级版本相同");

                jparameter = new JObject();

                jparameter["REQUESTTYPE"] = 0;
                jparameter["CMD"] = 1;
                jparameter["STORAGE"] = (int)StorageType.STORAGE_INTERNAL_SDCARD;
                jparameter["UPEVENTTYPE"] = (int)UploadEventType.UPLOAD_EVENTTYPE_UPGRADECP4;
                jparameter["SSRC"] = mTaskId++;

                jresp = session.SendCommand(Module.DEVEMM, Operation.REQUESTCTRLEVENT, jparameter);

                Assert.IsNotNull(jresp);
                Assert.IsNotNull(jresp["ERRORCODE"]);
                Assert.AreEqual(0, Convert.ToUInt32(jresp["ERRORCODE"].ToString()));

                Sleep(waittime * 1000);

                jparameter = new JObject();

                jresp = session.SendCommand(Module.DEVEMM, Operation.GETDEVVERSIONINFO, null);

                Assert.IsNotNull(jresp);
                Assert.IsNotNull(jresp["ERRORCODE"]);
                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));
                Assert.IsNotNull(jresp["DEVINFO"]);

                devinfo = JsonConvert.DeserializeObject<DevInfo>(jresp["DEVINFO"].ToString());

                string cp4Version_after = devinfo.cp4;

                Assert.AreEqual(cp4Version_before.Substring(cp4Version_before.Length - 6), cp4Version_after.Substring(cp4Version_after.Length - 6));
            }
        }
		[Ignore("11111")]
		[Test]
        public void UpgradeIPC()
        {
            int waittime = 300;

            if (this.CFG.gNode("Upgrade/UpgradeIPC/waittime") != null)
            {
                waittime = Convert.ToInt32(this.CFG.gNode("Upgrade/UpgradeIPC/waittime").InnerText);
            }

            Console.WriteLine("waittime = {0}", waittime);

            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);

                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                JObject jparameter = new JObject();

                JObject jresp = session.SendCommand(Module.DEVEMM, Operation.GETIPCVERS, null);

                Assert.IsNotNull(jresp);
                Assert.IsNotNull(jresp["ERRORCODE"]);
                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));
                Assert.IsNotNull(jresp["VERS"]);

                List<IPCVers> list = JsonConvert.DeserializeObject<List<IPCVers>>(jresp["VERS"].ToString());

                for (int i = 0; i < list.Count; i++)
                {
                    IPCVers ipcVers = list[i];

                    jparameter = new JObject();

                    jparameter["REQUESTTYPE"] = 0;
                    jparameter["CMD"] = 1;
                    jparameter["STORAGE"] = (int)StorageType.STORAGE_INTERNAL_SDCARD;
                    jparameter["UPEVENTTYPE"] = (int)UploadEventType.UPLOAD_EVENTTYPE_UPGRADEIPC;
                    jparameter["SSRC"] = mTaskId++;
                    jparameter["INDEX"] = ipcVers.index;

                    jresp = session.SendCommand(Module.DEVEMM, Operation.GETIPCVERS, jparameter);

                    Assert.IsNotNull(jresp);
                    Assert.IsNotNull(jresp["ERRORCODE"]);
                    Assert.AreEqual(0, Convert.ToUInt32(jresp["ERRORCODE"].ToString()));
                }

                Sleep(waittime * 1000);

                jparameter = new JObject();

                jresp = session.SendCommand(Module.DEVEMM, Operation.GETIPCVERS, null);

                Assert.IsNotNull(jresp);
                Assert.IsNotNull(jresp["ERRORCODE"]);
                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));
                Assert.IsNotNull(jresp["VERS"]);

                List<IPCVers> list_copy = JsonConvert.DeserializeObject<List<IPCVers>>(jresp["VERS"].ToString());


                Assert.AreEqual(list.Count, list_copy.Count);

                for (int i = 0; i < list.Count; i++)
                {
                    Assert.AreEqual(list[i].index, list_copy[i].index);
                    Assert.AreNotEqual(list[i].version, list_copy[i].version);
                }

            }
        }
		[Ignore("11111")]
		[Test]
        public void UpgradeGPS()
        {

        }
		
		[Test]
        public void UpgradeFast()
        {
            int waittime = 300;

            if (this.CFG.gNode("Upgrade/UpgradeFast/waittime") != null)
            {
                waittime = Convert.ToInt32(this.CFG.gNode("Upgrade/UpgradeFast/waittime").InnerText);
            }

            Console.WriteLine("waittime = {0}", waittime);

            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);

                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                JObject jparameter = new JObject();

                JObject jresp = session.SendCommand(Module.DEVEMM, Operation.GETVERSINFOBYSW, null);

                Assert.IsNotNull(jresp);
                Assert.IsNotNull(jresp["ERRORCODE"]);
                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));
                Assert.IsNotNull(jresp["DEVINFO"]);

                DevInfo devinfo = JsonConvert.DeserializeObject<DevInfo>(jresp["DEVINFO"].ToString());

                string mainVersion_before = devinfo.mainVersion;

                jparameter = new JObject();

                jparameter["REQUESTTYPE"] = 0;
                jparameter["CMD"] = 1;
                jparameter["STORAGE"] = (int)StorageType.STORAGE_INTERNAL_SDCARD;
                jparameter["UPEVENTTYPE"] = (int)UploadEventType.UPLOAD_EVENTTYPE_UPGRADE;
                jparameter["SSRC"] = mTaskId++;

                jresp = session.SendCommand(Module.DEVEMM, Operation.REQUESTCTRLEVENT, jparameter);

                Assert.IsNotNull(jresp);
                Assert.IsNotNull(jresp["ERRORCODE"]);
                Assert.AreEqual(0, Convert.ToUInt32(jresp["ERRORCODE"].ToString()));

                session.Logout();

                Sleep(waittime * 1000);

				session = new N9MSession(ip, port);
				Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

				jparameter = new JObject();

                jresp = session.SendCommand(Module.DEVEMM, Operation.GETDEVVERSIONINFO, null);

                Assert.IsNotNull(jresp);
                Assert.IsNotNull(jresp["ERRORCODE"]);
                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));
                Assert.IsNotNull(jresp["DEVINFO"]);

                devinfo = JsonConvert.DeserializeObject<DevInfo>(jresp["DEVINFO"].ToString());

                string mainVersion_after = devinfo.mainVersion;

                Assert.AreEqual(mainVersion_before, mainVersion_after);
            }
        }
    }
}
