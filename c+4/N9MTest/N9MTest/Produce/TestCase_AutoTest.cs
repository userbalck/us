using N9MTest.SDK;
using N9MTest.SDK.http;
using N9MTest.SDK.net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static N9MTest.SDK.net.BeiDouInfo;
using static N9MTest.SDK.net.DialInfo;

namespace N9MTest.Produce
{
    [TestFixture]
    class TestCase_AutoTest:TestCase_Basecase
    {
        [Test]
        public void Produce()
        {
            //设备主版本号
            string mainVersion = this.CFG.gNode("Produce/Produce/version").InnerText;

            //设备将要设定的mac地址
            string settingMac = this.CFG.gNode("Produce/Produce/mac").InnerText;

            //获取WIFI的设置信息
            //加密方式
            int nEncryptType = 0;
            if (this.CFG.gNode("Produce/Produce/WIFI/ECRYPTTYPE") != null)
            {
                nEncryptType = Convert.ToInt32(this.CFG.gNode("Produce/Produce/WIFI/ECRYPTTYPE").InnerText);
            }

            string essid = "";

            if (this.CFG.gNode("Produce/Produce/WIFI/ESSID") != null)
            {
                essid = this.CFG.gNode("Produce/Produce/WIFI/ESSID").InnerText;
            }

            string pwd = "";

            if (this.CFG.gNode("Produce/Produce/WIFI/PWD") != null)
            {
                pwd = this.CFG.gNode("Produce/Produce/WIFI/PWD").InnerText;
            }
                

            //使能
            Boolean bStorage = Convert.ToBoolean(this.CFG.gNode("Produce/Produce/enabled/storage").InnerText);
            Boolean bDial = Convert.ToBoolean(this.CFG.gNode("Produce/Produce/enabled/dial").InnerText);
            Boolean bGPS = Convert.ToBoolean(this.CFG.gNode("Produce/Produce/enabled/gps").InnerText);
            Boolean bWIFI = Convert.ToBoolean(this.CFG.gNode("Produce/Produce/enabled/wifi").InnerText);

            Console.WriteLine("settingMac = {0}", settingMac);

            foreach (string ip in IPList)
            {
                Console.WriteLine("ip ={0}", ip);

                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                Console.WriteLine("加密芯片序列号:{0}", session.GetDeviceInfo().dsno);
                Assert.IsNotNull(session.GetDeviceInfo().dsno);

                JObject jresp = session.SendCommand(Module.DEVEMM, Operation.GETCTRLUTC, null);
                long curt = Convert.ToInt64(jresp["CURT"].ToString());

                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()), "获取时间失败， 错误码为{0}", Convert.ToInt32(jresp["ERRORCODE"].ToString()));

                DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                DateTime date = start.AddSeconds(curt).ToLocalTime();

                Console.WriteLine("系统当前的时间:unix time:{0} utc time:{1}", curt, date.ToString());

                JObject jparameter = new JObject();

                jresp = session.SendCommand(Module.DEVEMM, Operation.GETDEVVERSIONINFO, null);

                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()), "获取设备固件版本号失败， 错误码为{0}", Convert.ToInt32(jresp["ERRORCODE"].ToString()));

                Assert.IsNotNull(jresp);
                Assert.IsNotNull(jresp["ERRORCODE"]);
                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));
                Assert.IsNotNull(jresp["DEVINFO"]);

                DevInfo devinfo = JsonConvert.DeserializeObject<DevInfo>(jresp["DEVINFO"].ToString());

                Console.WriteLine("获取到的设备端版本号:{0}", devinfo.mainVersion);

                Assert.AreEqual(mainVersion, devinfo.mainVersion, "期望版本号:{0} 获取到的版本号:{1}", mainVersion, devinfo.mainVersion);

                //读取mac地址
                jparameter = new JObject();
                JObject jmdvr = new JObject();

                jmdvr["KEYS"] = "?";
                jparameter["MDVR"] = jmdvr;

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);

                Assert.IsNotNull(jresp["MDVR"]["KEYS"]["MAC"]);

                string mac = jresp["MDVR"]["KEYS"]["MAC"].ToString();

                Console.WriteLine("获取到设备端的mac地址为{0}", mac);

                //设置mac地址
                jparameter = new JObject();
                jmdvr = new JObject();
                JObject jkeys = new JObject();

                jkeys["MAC"] = settingMac;

                jmdvr["KEYS"] = jkeys;
                jparameter["MDVR"] = jmdvr;

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()), "设置mac地址返回值失败  错误码是{0}", Convert.ToInt32(jresp["ERRORCODE"].ToString()));

                Console.WriteLine("设置mac {0}地址完毕", settingMac);

                N9MHttpSession httpSession = new N9MHttpSession(ip, webport);
                httpSession.Login("/logincheck.rsp?type=1", username, password);


                List<StorageInfo> storageList = null;
                string resp = null;

                if (bStorage)
                {
                    //获取存储器大小
                  
                    resp = httpSession.Get("/device.rsp?opt=hdd&cmd=state");
                    jresp = JObject.Parse(resp);

                    Assert.AreEqual(0, Convert.ToInt32(jresp["result"].ToString()));

                    storageList = JsonConvert.DeserializeObject<List<StorageInfo>>(jresp["data"].ToString());

                    Assert.IsNotNull(storageList, "获取存储器列表为空");

                    Console.WriteLine("storageList.count = {0}", storageList.Count);

                    foreach (StorageInfo info in storageList)
                    {
                        info.Print();
                    }
                }


                if (bWIFI)
                {
                    //设置WIFI的账号信息
                    jparameter = new JObject();
                    jmdvr = new JObject();

                    JObject jwifi = new JObject();
                    jwifi["ENABLE"] = 1;
                    jwifi["ECRYPTTYPE"] = nEncryptType;
                    jwifi["IPMODE"] = 1;
                    jwifi["DNSMODE"] = 1;
                    jwifi["ESSID"] = essid;
                    jwifi["PWD"] = pwd;

                    jmdvr["WIFI"] = jwifi;
                    jparameter["MDVR"] = jmdvr;
                    jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                    Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()), "设置mac地址失败 错误码为{0}", Convert.ToInt32(jresp["ERRORCODE"].ToString()));

                    Sleep(15 * 1000);
                }

                //获取WIFI信息
                jparameter = new JObject();
                jparameter["DATE"] = 0;
                jparameter["TYPE"] = 1;
                jparameter["INFO"] = "?";

                jresp = session.SendCommand(Module.DEVEMM, Operation.GETYUNWEIINFO, jparameter);

                Assert.IsNotNull(jresp);
                Assert.IsNotNull(jresp["ERRORCODE"]);

                WIFIInfo wifiInfo = null;

                if (bWIFI)
                {
                    Assert.IsNotNull(jresp["INFO"]["WIFI"]);
                    wifiInfo = JsonConvert.DeserializeObject<WIFIInfo>(jresp["INFO"]["WIFI"].ToString());

                    wifiInfo.Print();
                }

                GPSInfo gpsInfo = null;
                BeiDouInfo beidouInfo = null;
                if (bGPS)
                {
                    if (jresp["INFO"]["GPS"] != null && jresp["INFO"]["GPS"].HasValues)
                    {
                        //GPS当前数据打印
                        gpsInfo = JsonConvert.DeserializeObject<GPSInfo>(jresp["INFO"]["GPS"]["G"].ToString());
                        gpsInfo.Print();
                    }

                    if (jresp["INFO"]["BD"] != null && jresp["INFO"]["BD"].HasValues)
                    {
                        beidouInfo = JsonConvert.DeserializeObject<BeiDouInfo>(jresp["INFO"]["BD"].ToString());
                    }

                    if (gpsInfo == null && beidouInfo == null)
                    {
                        Assert.IsTrue(false, "不存在北斗或者GPS定位模块");
                    }

                    if (gpsInfo != null)
                    {
                        Assert.AreNotEqual(0, Convert.ToInt32(jresp["INFO"]["GPS"]["S"].ToString()), "GPS 模块不存在");
                    }

                    if (beidouInfo != null)
                    {
                        Assert.AreNotEqual(BeiDouModuleState.NoExists, beidouInfo.state, "北斗模块不存在");
                    }
                  
                }

                //3g模块打印
                DialInfo dialInfo = null;

                if (bDial)
                {
                    if (jresp["INFO"]["3G"] != null)
                    {
                        dialInfo = JsonConvert.DeserializeObject<DialInfo>(jresp["INFO"]["3G"].ToString());
                        dialInfo.Print();
                    }
                    else if (jresp["INFO"]["4G"] != null)
                    {
                        dialInfo = JsonConvert.DeserializeObject<DialInfo>(jresp["INFO"]["4G"].ToString());
                        dialInfo.Print();
                    }

                    Assert.AreEqual(ModuleState.EXIST, dialInfo.moduleState, "拨号模块不存在");
                }
               

               

                //导出参数
                string path = Environment.CurrentDirectory;

                httpSession.GetFile("/download.rsp", ref path);

                Console.WriteLine("导出参数成功 路径为 path = {0}", path);

                resp = httpSession.PostFile("/upload.rsp?filetype=importparam", path);

                Console.WriteLine("导入参数成功 resp = {0}", resp);

                Console.WriteLine("\r\n\r\n");

                if (bStorage)
                {
                    foreach (StorageInfo info in storageList)
                    {
                        info.Print();
                    }
                }
               

                Console.WriteLine("加密芯片序列号:{0}", session.GetDeviceInfo().dsno);
                Console.WriteLine("系统当前的时间:unix time:{0} utc time:{1}", curt, date.ToString());
                Console.WriteLine("获取到的设备端版本号:{0}", devinfo.mainVersion);
                Console.WriteLine("获取到设备端的mac地址为{0}", mac);
                Console.WriteLine("设置mac {0}地址完毕", settingMac);

                if (bWIFI)
                {
                    if (wifiInfo != null)
                    {
                        wifiInfo.Print();
                    }
                    
                }


                if (bGPS)
                {
                    if (gpsInfo != null)
                    {
                        gpsInfo.Print();
                    }
                    
                }

                if (bDial)
                {
                    if (dialInfo != null)
                    {
                        dialInfo.Print();
                    }
                }
            }


        }
    }
}
