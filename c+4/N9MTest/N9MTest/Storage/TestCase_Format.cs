using N9MTest.commons;
using N9MTest.SDK.http;
using N9MTest.SDK.net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RM;
using System;
using System.Collections.Generic;

namespace N9MTest.Storage
{
    [TestFixture]
    class TestCase_Format: TestCase_Basecase
    {
        [Test]
        public void WebFormat()
        {
            foreach (string ip in IPList)
            {
                N9MHttpSession session = new N9MHttpSession(ip, webport);
                session.Login("/logincheck.rsp?type=1", username, password);

                string resp = session.Get("/device.rsp?opt=hdd&cmd=state");
                JObject jresp = JObject.Parse(resp);

                Assert.AreEqual(0, Convert.ToInt32(jresp["result"].ToString()));

                List<StorageInfo> list = JsonConvert.DeserializeObject<List<StorageInfo>>(jresp["data"].ToString());

                Console.WriteLine("list.count = {0}", list.Count);

                foreach (StorageInfo info in list)
                {
                    if (info.total == 0)
                    {
                        continue;
                    }

                    string url = String.Format("/device.rsp?opt=hdd&cmd=format&devmap={0}&acktype={1}&user={2}",
                        info.id, info.type, info.user);

                    resp = session.Get(url);
                    jresp = JObject.Parse(resp);
                }

                resp = session.Get("/device.rsp?opt=hdd&cmd=state");
                jresp = JObject.Parse(resp);

                Assert.AreEqual(0, Convert.ToInt32(jresp["result"].ToString()));

                list = JsonConvert.DeserializeObject<List<StorageInfo>>(jresp["data"].ToString());

                foreach (StorageInfo info in list)
                {
                    if (info.total == 0)
                    {
                        continue;
                    }

                    double rate = info.left  * 1.0f/ info.total;

                    Console.WriteLine("info.left = {0}, info.total = {1}, rate = {2}", info.left, info.total, rate);

                    Assert.IsTrue(rate >= 0.9);

                }
            }
        }

        [Test]
        public void ManualFormat()
        {
            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                JObject jresp = null;

                JObject jparameter = new JObject();

                jresp = session.SendCommand(Module.STORM,Operation.GETSTORAGEINFO, null);

                List<StorageInfoEx> StorageList = JsonConvert.DeserializeObject<List<StorageInfoEx>>(jresp["STORAGELIST"].ToString());

                foreach (StorageInfoEx info in StorageList)
                {
                    jparameter = new JObject();
                    jparameter["STORAGEINDEX"] = 1<< info.index;
                    jparameter["CMDTYPE"] = 2;

                    jresp = session.SendCommand(Module.STORM, Operation.SETCONTROLSTORAGE, jparameter);

                    Assert.IsNotNull(jresp);
                    Assert.IsNotNull(jresp["ERRORCODE"]);

                    int errorcode = Convert.ToInt32(jresp["ERRORCODE"].ToString());


                    if (info.status == (int)StorageStatus.STORAGE_STATE_NOEXISTS)
                    {
                        Assert.AreEqual(0X00000055, errorcode);
                    }
                    else
                    {
                        Assert.AreEqual(0, errorcode);
                    }
                }
            }
        }

        [Test]
        public void UdiskFormat()
        {
            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                JObject jresp = null;

                JObject jparameter = new JObject();

                jresp = session.SendCommand(Module.STORM, Operation.GETSTORAGEINFO, null);

                List<StorageInfoEx> StorageList = JsonConvert.DeserializeObject<List<StorageInfoEx>>(jresp["STORAGELIST"].ToString());

                int nUDiskIndex = -1;

                foreach (StorageInfoEx info in StorageList)
                {
                    if (info.type != (int)StorageType.STORAGE_INTERNAL_SDCARD)
                    {
                        continue;
                    }

                    if (info.status == (int)StorageStatus.STORAGE_STATE_NOEXISTS)
                    {
                        continue;
                    }

                    nUDiskIndex = info.index;

                    jparameter = new JObject();
                    jparameter["STORAGEINDEX"] = info.index << 1;
                    jparameter["CMDTYPE"] = 2;
                    jparameter["SYS"] = (int)FileSystem.FAT32;

                    jresp = session.SendCommand(Module.STORM, Operation.SETCONTROLSTORAGE, jparameter);

                    Assert.IsNotNull(jresp);
                    Assert.IsNotNull(jresp["ERRORCODE"]);

                    int errorcode = Convert.ToInt32(jresp["ERRORCODE"].ToString());
                    Assert.AreEqual(0, errorcode);
                }

                Assert.AreNotEqual(-1, nUDiskIndex);

                nUDiskIndex = -1;

                foreach (StorageInfoEx info in StorageList)
                {
                    if (info.type != (int)StorageType.STORAGE_INTERNAL_SDCARD)
                    {
                        continue;
                    }

                    if (info.status == (int)StorageStatus.STORAGE_STATE_NOEXISTS)
                    {
                        continue;
                    }

                    nUDiskIndex = info.index;

                    jparameter = new JObject();
                    jparameter["STORAGEINDEX"] = info.index << 1;
                    jparameter["CMDTYPE"] = 2;
                    jparameter["SYS"] = (int)FileSystem.N9M;

                    jresp = session.SendCommand(Module.STORM, Operation.SETCONTROLSTORAGE, jparameter);

                    Assert.IsNotNull(jresp);
                    Assert.IsNotNull(jresp["ERRORCODE"]);

                    int errorcode = Convert.ToInt32(jresp["ERRORCODE"].ToString());
                    Assert.AreEqual(0, errorcode);
                }

                Assert.AreNotEqual(-1, nUDiskIndex);

                foreach (StorageInfoEx info in StorageList)
                {
                    if (info.type != (int)StorageType.STORAGE_INTERNAL_SDCARD)
                    {
                        continue;
                    }

                    if (info.status == (int)StorageStatus.STORAGE_STATE_NOEXISTS)
                    {
                        continue;
                    }

                    nUDiskIndex = info.index;

                    jparameter = new JObject();
                    jparameter["STORAGEINDEX"] = info.index << 1;
                    jparameter["CMDTYPE"] = 2;
                    jparameter["SYS"] = (int)FileSystem.FAT32;

                    jresp = session.SendCommand(Module.STORM, Operation.SETCONTROLSTORAGE, jparameter);

                    Assert.IsNotNull(jresp);
                    Assert.IsNotNull(jresp["ERRORCODE"]);

                    int errorcode = Convert.ToInt32(jresp["ERRORCODE"].ToString());
                    Assert.AreEqual(0, errorcode);
                }

            }
        }
    }
}
