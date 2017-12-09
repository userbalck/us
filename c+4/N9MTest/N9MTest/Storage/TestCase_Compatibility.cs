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

namespace N9MTest.Storage
{
    [TestFixture]
    class TestCase_Compatibility:TestCase_Basecase
    {
        [Test]
        public void HDDModule()
        {
            int nCount = Convert.ToInt32(this.CFG.gNode("Storage/HDDModule/count").InnerText);
            int waittime = Convert.ToInt32(this.CFG.gNode("Storage/HDDModule/waittime").InnerText);

            for (int i = 0; i < nCount; i++)
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

                        if (info.type != (int)StorageType.STORAGE_DEFAULT_EXTERNAL)
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

                        if (info.type != (int)StorageType.STORAGE_DEFAULT_EXTERNAL)
                        {
                            continue;
                        }

                        double rate = info.left * 1.0f / info.total;

                        Console.WriteLine("info.left = {0}, info.total = {1}, rate = {2}", info.left, info.total, rate);

                        Assert.IsTrue(rate >= 0.9);
                    }
                }

                Sleep(waittime * 1000);
            }
        }

        [Test]
        public void SDModule()
        {
            int nCount = Convert.ToInt32(this.CFG.gNode("Storage/SDModule/count").InnerText);
            int waittime = Convert.ToInt32(this.CFG.gNode("Storage/SDModule/waittime").InnerText);

            for (int i = 0; i < nCount; i++)
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

                        if (info.type == (int)StorageType.STORAGE_DEFAULT_EXTERNAL)
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

                        if (info.type == (int)StorageType.STORAGE_DEFAULT_EXTERNAL)
                        {
                            continue;
                        }

                        double rate = info.left * 1.0f / info.total;

                        Console.WriteLine("info.left = {0}, info.total = {1}, rate = {2}", info.left, info.total, rate);

                        Assert.IsTrue(rate >= 0.9);
                    }
                }

                Sleep(waittime * 1000);
            }
        }

    }
}
