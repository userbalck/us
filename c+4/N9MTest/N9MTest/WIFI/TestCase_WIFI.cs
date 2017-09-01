using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Xml;
using N9MTest.SDK.net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Util;

namespace N9MTest.WIFI
{
	
    [TestFixture]
    class TestCase_WIFI:TestCase_Basecase
    {
        private void JustWifiConnect(N9MSession session)
        {
            //获取wifi信息
            JObject jmdvr = new JObject();
            JObject jparameter = new JObject();
            JObject jresp = new JObject();

            jmdvr.Add("SERIAL", "?");
            jparameter.Add("MDVR", jmdvr);
            jresp = session.SendCommand(Module.DEVEMM, Operation.GETDEVINFOSTATUS, jparameter);
            Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"]), "查询wifi参数失败.错误码为{0}", Convert.ToInt32(jresp["ERRORCODE"]));
            Assert.AreNotEqual(0, Convert.ToInt32(jresp["S"]["WS"]), "wifi未连接.错误码为{0}", Convert.ToInt32(jresp["S"]["WS"]));
        }
		[Ignore("111111")]
		[Test]
        public void AutoWIFIIP()
        {
            string wipmode = this.CFG.gNode("WIFI/AutoWIFIIP/IPMODE").InnerText;
            string wecryttype = this.CFG.gNode("WIFI/AutoWIFIIP/ECRYPTTYPE").InnerText;
            string wessid = this.CFG.gNode("WIFI/AutoWIFIIP/ESSID").InnerText;
            string wpwd = this.CFG.gNode("WIFI/AutoWIFIIP/PWD").InnerText;

            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                JObject jresp = null;
                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                JObject jwifi = new JObject();

                jwifi.Add("ENABLE", "1");//1： 开启
                jwifi.Add("ECRYPTTYPE", wecryttype);
                jwifi.Add("IPMODE", wipmode);
                jwifi.Add("ESSID", wessid);
                jwifi.Add("PWD", wpwd);

                jmdvr.Add("WIFI", jwifi);
                jparameter.Add("MDVR", jmdvr);
                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);
                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"]), "设置wifi状态失败.错误码为{0}", Convert.ToInt32(jresp["ERRORCODE"]));

                Sleep(20000);

                JustWifiConnect(session);

                session.Logout();
            }
        }
		[Ignore("111111")]
		[Test]
        public void WirelessSecurity()
        {
            string wipmode = this.CFG.gNode("WIFI/WirelessSecurity/IPMODE").InnerText;
            string wecryttype = this.CFG.gNode("WIFI/WirelessSecurity/ECRYPTTYPE").InnerText;
            string wessid = this.CFG.gNode("WIFI/WirelessSecurity/ESSID").InnerText;
            string wpwd = this.CFG.gNode("WIFI/WirelessSecurity/PWD").InnerText;

            foreach (string ip in IPList)
            {
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                JObject jresp = null;
                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                JObject jwifi = new JObject();

                jwifi.Add("ENABLE", "1");//1： 开启
                jwifi.Add("ECRYPTTYPE", wecryttype);
                jwifi.Add("IPMODE", wipmode);
                jwifi.Add("ESSID", wessid);
                jwifi.Add("PWD", wpwd);

                jmdvr.Add("WIFI", jwifi);
                jparameter.Add("MDVR", jmdvr);
                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);
                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"]), "设置wifi状态失败.错误码为{0}", Convert.ToInt32(jresp["ERRORCODE"]));

                Sleep(20000);

                JustWifiConnect(session);

                //设置wifi加密类型为NONE
                jwifi.RemoveAll();
                jmdvr.RemoveAll();
                jparameter.RemoveAll();

                jwifi.Add("ECRYPTTYPE", "0");
                jmdvr.Add("WIFI", jwifi);
                jparameter.Add("MDVR", jmdvr);
                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);
                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"]), "设置wifi状态失败.错误码为{0}", Convert.ToInt32(jresp["ERRORCODE"]));

                Sleep(20000);

                JustWifiConnect(session);
            }
        }
		[Ignore("111111")]
		[Test]
        public void SSIDBroadcast()
        {
            string wipmode = this.CFG.gNode("WIFI/SSIDBroadcast/IPMODE").InnerText;
            string wecryttype = this.CFG.gNode("WIFI/SSIDBroadcast/ECRYPTTYPE").InnerText;
            string wessid = this.CFG.gNode("WIFI/SSIDBroadcast/ESSID").InnerText;
            string wpwd = this.CFG.gNode("WIFI/SSIDBroadcast/PWD").InnerText;

            foreach (string ip in IPList)
            {
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                JObject jresp = null;
                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                JObject jwifi = new JObject();

                jwifi.Add("ENABLE", "1");//1： 开启
                jwifi.Add("ECRYPTTYPE", wecryttype);
                jwifi.Add("IPMODE", wipmode);
                jwifi.Add("ESSID", wessid);
                jwifi.Add("PWD", wpwd);

                jmdvr.Add("WIFI", jwifi);
                jparameter.Add("MDVR", jmdvr);
                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);
                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"]), "设置wifi状态失败.错误码为{0}", Convert.ToInt32(jresp["ERRORCODE"]));

                Sleep(20000);

                JustWifiConnect(session);

                session.Logout();
            }
        }
		[Ignore("111111")]
		[Test]
        public void BootToReconnect()
        {

        }
		[Ignore("111111")]

		[Test]
        public void WIFIDownloadLowSpeed()
        {
            XmlNodeList ChannelInfoList = this.CFG.gNodes("Local/WIFIDownloadLowSpeed/channel");
            int duration = Convert.ToInt32(this.CFG.gNode("Local/WIFIDownloadLowSpeed/duration").InnerText);
            int count = Convert.ToInt32(this.CFG.gNode("Local/WIFIDownloadLowSpeed/count").InnerText);

            Dictionary<int, NetStreamEncoderInfo> dict = new Dictionary<int, NetStreamEncoderInfo>();

            for (int i = 0; i < ChannelInfoList.Count; i++)
            {
                NetStreamEncoderInfo info = new NetStreamEncoderInfo();
                int channel = Convert.ToInt32(this.CFG.gNode(ChannelInfoList[i], "id").InnerText);
                info.resolution = (int)(Resolution)Enum.Parse(typeof(Resolution), "RES_" + this.CFG.gNode(ChannelInfoList[i], "resolution").InnerText.ToUpper());
                info.FrameRate = Convert.ToInt32(this.CFG.gNode(ChannelInfoList[i], "frame-rate").InnerText);
                info.quality = Convert.ToInt32(this.CFG.gNode(ChannelInfoList[i], "quality").InnerText);
                info.VideoEnable = Convert.ToInt32(this.CFG.gNode(ChannelInfoList[i], "enable").InnerText);
                info.BitrateMode = (int)Enum.Parse(typeof(BitrateMode), this.CFG.gNode(ChannelInfoList[i], "mode").InnerText);

                dict.Add(channel, info);
            }

            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                //首先获取主码流的参数
                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                jmdvr.Add("MAIN", "?");
                jparameter.Add("MDVR", jmdvr);

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);
                Assert.IsNotNull(jresp["MDVR"]);
                Assert.IsNotNull(jresp["MDVR"]["MAIN"]);

                List<NetStreamEncoderInfo> list = JsonConvert.DeserializeObject<List<NetStreamEncoderInfo>>(jresp["MDVR"]["MAIN"].ToString());


                //按照参数进行录像参数设定
                jparameter = new JObject();
                jmdvr = new JObject();

                JArray jnecArray = new JArray();

                foreach (var item in dict)
                {
                    JObject nec = new JObject();
                    int channel = item.Key;
                    NetStreamEncoderInfo info = dict[channel];
                    nec = JObject.Parse(JsonConvert.SerializeObject(info));
                    nec.Remove("BR");
                    jnecArray.Insert(channel, nec);
                }

                jmdvr.Add("MAIN", jnecArray);

                jparameter.Add("MDVR", jmdvr);

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Console.WriteLine("jresp = {0}", jresp);

                Sleep(150 * 1000);

                DateTime now = DateTime.Now;

                int nStreamType = (int)StreamType.MAIN_STREAM;

                string starttime = now.AddSeconds(-120).ToString("yyyyMMddHHmmss");
                string endtime = DateTime.Now.ToString("yyyyMMddHHmmss");


                jparameter = new JObject();
                jparameter.Add("STREAMTYPE", nStreamType);
                jparameter.Add("FILETYPE", 0xffffffff);
                jparameter.Add("CHANNEL", 0xF);
                jparameter.Add("STARTTIME", starttime);
                jparameter.Add("ENDTIME", endtime);

                jresp = session.SendCommand(Module.STORM, Operation.QUERYFILELIST, jparameter);

                Assert.IsNotNull(jresp["FILELIST"]);

                List<RemoteFileInfo> RemoteFileList = JsonConvert.DeserializeObject<List<RemoteFileInfo>>(jresp["FILELIST"].ToString());

                Assert.AreEqual(4, list.Count);

                for (int i = 0; i < count; i++)
                {
                    foreach (RemoteFileInfo info in RemoteFileList)
                    {
                        //首先获取段内文件的大小
                        JObject jparamater = new JObject();
                        jparamater["DATATYPE"] = 0;
                        jparamater["STREAMTYPE"] = nStreamType;
                        jparamater["RECORDTYPE"] = 3;
                        jparamater["STIME"] = starttime;
                        jparamater["ETIME"] = endtime;
                        jparamater["CHANNEL"] = 1 << info.nChannel;

                        jresp = session.SendCommand(Module.STORM, Operation.GETFILESIZEBYTIME, jparamater);
                        Assert.IsNotNull(jresp);
                        Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));

                        int nTotalSize = Convert.ToInt32(jresp["TOTALSIZE"].ToString());

                        long before = ExactTime.GetExactTime();

                        session.DownloadVideo((StreamType)nStreamType, info.recordID, info.nChannel, starttime, endtime, "video.264");

                        long after = ExactTime.GetExactTime();

                        double rate = (nTotalSize * 1.0) / (after - before);

                        Assert.IsTrue(rate * 8 > 27);
                    }
                }
            }
        }
		[Ignore("11111111")]
        [Test]
        public void WIFIDownloadHighSpeed()
        {
            XmlNodeList ChannelInfoList = this.CFG.gNodes("Local/WIFIDownloadHighSpeed/channel");
            int duration = Convert.ToInt32(this.CFG.gNode("Local/WIFIDownloadHighSpeed/duration").InnerText);
            int count = Convert.ToInt32(this.CFG.gNode("Local/WIFIDownloadHighSpeed/count").InnerText);

            Dictionary<int, NetStreamEncoderInfo> dict = new Dictionary<int, NetStreamEncoderInfo>();

            for (int i = 0; i < ChannelInfoList.Count; i++)
            {
                NetStreamEncoderInfo info = new NetStreamEncoderInfo();
                int channel = Convert.ToInt32(this.CFG.gNode(ChannelInfoList[i], "id").InnerText);
                info.resolution = (int)(Resolution)Enum.Parse(typeof(Resolution), "RES_" + this.CFG.gNode(ChannelInfoList[i], "resolution").InnerText.ToUpper());
                info.FrameRate = Convert.ToInt32(this.CFG.gNode(ChannelInfoList[i], "frame-rate").InnerText);
                info.quality = Convert.ToInt32(this.CFG.gNode(ChannelInfoList[i], "quality").InnerText);
                info.VideoEnable = Convert.ToInt32(this.CFG.gNode(ChannelInfoList[i], "enable").InnerText);
                info.BitrateMode = (int)Enum.Parse(typeof(BitrateMode), this.CFG.gNode(ChannelInfoList[i], "mode").InnerText);

                dict.Add(channel, info);
            }

            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                //首先获取主码流的参数
                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                jmdvr.Add("MAIN", "?");
                jparameter.Add("MDVR", jmdvr);

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);
                Assert.IsNotNull(jresp["MDVR"]);
                Assert.IsNotNull(jresp["MDVR"]["MAIN"]);

                List<NetStreamEncoderInfo> list = JsonConvert.DeserializeObject<List<NetStreamEncoderInfo>>(jresp["MDVR"]["MAIN"].ToString());


                //按照参数进行录像参数设定
                jparameter = new JObject();
                jmdvr = new JObject();

                JArray jnecArray = new JArray();

                foreach (var item in dict)
                {
                    JObject nec = new JObject();
                    int channel = item.Key;
                    NetStreamEncoderInfo info = dict[channel];
                    nec = JObject.Parse(JsonConvert.SerializeObject(info));
                    nec.Remove("BR");
                    jnecArray.Insert(channel, nec);
                }

                jmdvr.Add("MAIN", jnecArray);

                jparameter.Add("MDVR", jmdvr);

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Console.WriteLine("jresp = {0}", jresp);

                Sleep(150 * 1000);

                DateTime now = DateTime.Now;

                int nStreamType = (int)StreamType.MAIN_STREAM;

                string starttime = now.AddSeconds(-120).ToString("yyyyMMddHHmmss");
                string endtime = DateTime.Now.ToString("yyyyMMddHHmmss");


                jparameter = new JObject();
                jparameter.Add("STREAMTYPE", nStreamType);
                jparameter.Add("FILETYPE", 0xffffffff);
                jparameter.Add("CHANNEL", 0xF);
                jparameter.Add("STARTTIME", starttime);
                jparameter.Add("ENDTIME", endtime);

                jresp = session.SendCommand(Module.STORM, Operation.QUERYFILELIST, jparameter);

                Assert.IsNotNull(jresp["FILELIST"]);

                List<RemoteFileInfo> RemoteFileList = JsonConvert.DeserializeObject<List<RemoteFileInfo>>(jresp["FILELIST"].ToString());

                Assert.AreEqual(4, list.Count);

                for (int i = 0; i < count; i++)
                {
                    foreach (RemoteFileInfo info in RemoteFileList)
                    {
                        //首先获取段内文件的大小
                        JObject jparamater = new JObject();
                        jparamater["DATATYPE"] = 0;
                        jparamater["STREAMTYPE"] = nStreamType;
                        jparamater["RECORDTYPE"] = 3;
                        jparamater["STIME"] = starttime;
                        jparamater["ETIME"] = endtime;
                        jparamater["CHANNEL"] = 1 << info.nChannel;

                        jresp = session.SendCommand(Module.STORM, Operation.GETFILESIZEBYTIME, jparamater);
                        Assert.IsNotNull(jresp);
                        Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));

                        int nTotalSize = Convert.ToInt32(jresp["TOTALSIZE"].ToString());

                        long before = ExactTime.GetExactTime();

                        session.DownloadVideo((StreamType)nStreamType, info.recordID, info.nChannel, starttime, endtime, "video.264");

                        long after = ExactTime.GetExactTime();

                        double rate = (nTotalSize * 1.0) / (after - before);

                        Assert.IsTrue(rate * 8 > 27);
                    }
                }
            }
        }
    }
}
