using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using N9MTest.SDK.net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml;

namespace N9MTest.Record
{
	[Ignore("1111")]
    [TestFixture]
    class TestCase_RecordTotalBitrate:TestCase_Basecase
    {
        [Test]
        public void RecordMainMaximum()
        {
            XmlNodeList ChannelInfoList = this.CFG.gNodes("Record/RecordMainMaximum/channel");
            UInt32 nChannelMask = Convert.ToUInt32(this.CFG.gNode("Record/RecordMainMaximum/channelmask").InnerText);

            Dictionary<int, NetStreamEncoderInfo> dict = new Dictionary<int, NetStreamEncoderInfo>();

            int nTotalCodeRate_setting = 0;

            nTotalCodeRate_setting = Convert.ToInt32(this.CFG.gNode("Record/RecordMainMaximum/total-coderate").InnerText);

            Console.WriteLine("nTotalCodeRate_setting = {0}", nTotalCodeRate_setting);

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

            do
            {
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

                        if (((nChannelMask >> channel) & 0x01) == 1)
                        {
                            jnecArray.Insert(channel, nec);
                        }

                    }

                    jmdvr.Add("MAIN", jnecArray);

                    jparameter.Add("MDVR", jmdvr);

                    jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                    Console.WriteLine("jresp = {0}", jresp);

                    //判定是否设置成功
                    Assert.IsNotNull(jresp["ERRORCODE"]);
                    Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));

                    jmdvr = new JObject();
                    jparameter = new JObject();

                    jmdvr.Add("MAIN", "?");
                    jparameter.Add("MDVR", jmdvr);

                    jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);
                    Assert.IsNotNull(jresp["MDVR"]);
                    Assert.IsNotNull(jresp["MDVR"]["MAIN"]);

                    list = JsonConvert.DeserializeObject<List<NetStreamEncoderInfo>>(jresp["MDVR"]["MAIN"].ToString());

                    foreach (var item in dict)
                    {
                        int channel = item.Key;
                        if (((nChannelMask >> channel) & 0x01) == 1)
                        {
                            NetStreamEncoderInfo info_setting = dict[channel];
                            NetStreamEncoderInfo info_getting = list[channel];

                            Assert.IsTrue(info_setting.isSettingSuccess(info_getting), "设定通道{0}不成功", channel);
                        }
                    }

                    session.Logout();
                }
            } while (false);

            Sleep(90 * 1000);

            do
            {
                foreach (string ip in IPList)
                {
                    Console.WriteLine("ip = {0}", ip);
                    N9MSession session = new N9MSession(ip, port);
                    Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                    DateTime dtEndTime = DateTime.Now;
                    DateTime dtStartTime = dtEndTime.AddMinutes(-1);

                    int nStreamType = (int)StreamType.MAIN_STREAM;
                    uint nChannelBits = nChannelMask;

                    string starttime = dtStartTime.ToString("yyyyMMddHHmmss");
                    string endtime = dtEndTime.ToString("yyyyMMddHHmmss");

                    JObject jparameter = new JObject();

                    //首先获取段内文件的大小
                    jparameter = new JObject();
                    jparameter["DATATYPE"] = 0;
                    jparameter["STREAMTYPE"] = nStreamType;
                    jparameter["RECORDTYPE"] = 3;
                    jparameter["STIME"] = starttime;
                    jparameter["ETIME"] = endtime;
                    jparameter["NEWSTREAMTYPE"] = 1 << nStreamType;
                    jparameter["RESTRAGE"] = 1 << (int)ReferStorage.REFER_STORAGE_HDD | (1 << (int)ReferStorage.REFER_STORAGE_SD);
                    jparameter["CHANNEL"] = nChannelBits;

                    JObject jresp = session.SendCommand(Module.STORM, Operation.GETFILESIZEBYTIME, jparameter);
                    Assert.IsNotNull(jresp);
                    Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));

                    int nTotalSize = Convert.ToInt32(jresp["TOTALSIZE"].ToString());

                    long nBitrate = (nTotalSize * 8) / (60 * 1024 * 1024);

                    Console.WriteLine("nTotalSize = {0} 计算总码率为{1}Mb", nTotalSize, nBitrate);

                    Assert.IsTrue(Math.Abs(nTotalCodeRate_setting - nBitrate) < 2, "码率比较失败,期望码率{0}Mb  实际码率{1}Mb", nTotalCodeRate_setting, nBitrate);
                }
            } while (false);
        }

        [Test]
        public void RecordSubMaximum()
        {
            XmlNodeList ChannelInfoList = this.CFG.gNodes("Record/RecordSubMaximum/channel");

            Dictionary<int, NetStreamEncoderInfo> dict = new Dictionary<int, NetStreamEncoderInfo>();

            int nTotalCodeRate_setting = 0;

            nTotalCodeRate_setting = Convert.ToInt32(this.CFG.gNode("Record/RecordSubMaximum/total-coderate").InnerText);
            UInt32 nChannelMask = Convert.ToUInt32(this.CFG.gNode("Record/RecordSubMaximum/channelmask").InnerText);

            Console.WriteLine("nTotalCodeRate_setting = {0}", nTotalCodeRate_setting);



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

            do
            {
                foreach (string ip in IPList)
                {
                    Console.WriteLine("ip = {0}", ip);
                    N9MSession session = new N9MSession(ip, port);
                    Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                    //首先获取主码流的参数
                    JObject jparameter = new JObject();
                    JObject jmdvr = new JObject();
                    jmdvr.Add("AR", "?");
                    jparameter.Add("MDVR", jmdvr);

                    JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);
                    Assert.IsNotNull(jresp["MDVR"]);
                    Assert.IsNotNull(jresp["MDVR"]["AR"]["VEC"]);

                    List<NetStreamEncoderInfo> list = JsonConvert.DeserializeObject<List<NetStreamEncoderInfo>>(jresp["MDVR"]["AR"]["VEC"].ToString());


                    //按照参数进行录像参数设定
                    jparameter = new JObject();
                    jmdvr = new JObject();
                    JObject jar = new JObject();

                    JArray jnecArray = new JArray();

                    UInt32 ulChannelBits = 0;

                    foreach (var item in dict)
                    {
                        JObject nec = new JObject();
                        int channel = item.Key;
                        NetStreamEncoderInfo info = dict[channel];
                        nec = JObject.Parse(JsonConvert.SerializeObject(info));
                        nec.Remove("BR");
                        jnecArray.Insert(channel, nec);

                        ulChannelBits |= (UInt32)(1 << channel);
                    }
                    jar.Add("VEC", jnecArray);
                    jar.Add("EN", 1);
                    jar.Add("RM", 0);
                    jar.Add("SSC", ulChannelBits);
                    jmdvr.Add("AR", jar);

                    jparameter.Add("MDVR", jmdvr);

                    jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);


                    //判定是否设置成功
                    Assert.IsNotNull(jresp["ERRORCODE"]);
                    Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));
                }

            } while (false);

            Sleep(90 * 1000);

            do
            {
                foreach (string ip in IPList)
                {
                    Console.WriteLine("ip = {0}", ip);
                    N9MSession session = new N9MSession(ip, port);
                    Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                    DateTime dtEndTime = DateTime.Now;
                    DateTime dtStartTime = dtEndTime.AddMinutes(-1);

                    int nStreamType = (int)StreamType.SUB_STREAM;
                    uint nChannelBits = nChannelMask;

                    string starttime = dtStartTime.ToString("yyyyMMddHHmmss");
                    string endtime = dtEndTime.ToString("yyyyMMddHHmmss");

                    JObject jparameter = new JObject();

                    //首先获取段内文件的大小
                    jparameter = new JObject();
                    jparameter["DATATYPE"] = 0;
                    jparameter["STREAMTYPE"] = nStreamType;
                    jparameter["RECORDTYPE"] = 3;
                    jparameter["STIME"] = starttime;
                    jparameter["ETIME"] = endtime;
                    jparameter["NEWSTREAMTYPE"] = 1 << nStreamType;
                    jparameter["RESTRAGE"] = 1 << (int)ReferStorage.REFER_STORAGE_HDD | (1 << (int)ReferStorage.REFER_STORAGE_SD);
                    jparameter["CHANNEL"] = nChannelBits;

                    JObject jresp = session.SendCommand(Module.STORM, Operation.GETFILESIZEBYTIME, jparameter);
                    Assert.IsNotNull(jresp);
                    Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));

                    int nTotalSize = Convert.ToInt32(jresp["TOTALSIZE"].ToString());

                    long nBitrate = (nTotalSize * 8) / (60 * 1024 * 1024);

                    Console.WriteLine("nTotalSize = {0} 计算总码率为{1}Mb", nTotalSize, nBitrate);

                    Assert.IsTrue(Math.Abs(nTotalCodeRate_setting - nBitrate) < 2, "码率比较失败,期望码率{0}Mb  实际码率{1}Mb", nTotalCodeRate_setting, nBitrate);
                }
            } while (false);
        }

        [Test]
        public void RecordMirrMaximum()
        {
            XmlNodeList ChannelInfoList = this.CFG.gNodes("Record/RecordMirrMaximum/channel");
            int nChannelMask = Convert.ToInt32(this.CFG.gNode("Record/RecordMirrMaximum/channelmask").InnerText);

            Dictionary<int, NetStreamEncoderInfo> dict = new Dictionary<int, NetStreamEncoderInfo>();

            int nTotalCodeRate_setting = 0;

            nTotalCodeRate_setting = Convert.ToInt32(this.CFG.gNode("Record/RecordMirrMaximum/total-coderate").InnerText);

            Console.WriteLine("nTotalCodeRate_setting = {0}", nTotalCodeRate_setting);



            for (int i = 0; i < ChannelInfoList.Count; i++)
            {
                if (((nChannelMask >> i) & 0x01) != 1)
                {
                    continue;
                }

                NetStreamEncoderInfo info = new NetStreamEncoderInfo();
                int channel = Convert.ToInt32(this.CFG.gNode(ChannelInfoList[i], "id").InnerText);
                info.resolution = (int)(Resolution)Enum.Parse(typeof(Resolution), "RES_" + this.CFG.gNode(ChannelInfoList[i], "resolution").InnerText.ToUpper());
                info.FrameRate = Convert.ToInt32(this.CFG.gNode(ChannelInfoList[i], "frame-rate").InnerText);
                info.quality = Convert.ToInt32(this.CFG.gNode(ChannelInfoList[i], "quality").InnerText);
                info.VideoEnable = Convert.ToInt32(this.CFG.gNode(ChannelInfoList[i], "enable").InnerText);
                info.BitrateMode = (int)Enum.Parse(typeof(BitrateMode), this.CFG.gNode(ChannelInfoList[i], "mode").InnerText);

                dict.Add(channel, info);
            }

            do
            {
                foreach (string ip in IPList)
                {
                    Console.WriteLine("ip = {0}", ip);
                    N9MSession session = new N9MSession(ip, port);
                    Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                    //首先获取主码流的参数
                    JObject jparameter = new JObject();
                    JObject jmdvr = new JObject();
                    jmdvr.Add("AR", "?");
                    jparameter.Add("MDVR", jmdvr);

                    JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);
                    Assert.IsNotNull(jresp["MDVR"]);
                    Assert.IsNotNull(jresp["MDVR"]["AR"]["VEC"]);

                List<NetStreamEncoderInfo> list = JsonConvert.DeserializeObject<List<NetStreamEncoderInfo>>(jresp["MDVR"]["AR"]["VEC"].ToString());


                //按照参数进行录像参数设定
                jparameter = new JObject();
                jmdvr = new JObject();
                JObject jar = new JObject();

                JArray jnecArray = new JArray();

                int ulChannelBits = nChannelMask;

                foreach (var item in dict)
                {
                    JObject nec = new JObject();
                    int channel = item.Key;

                    if (((nChannelMask >> channel) & 0x01) == 1)
                    {
                        NetStreamEncoderInfo info = dict[channel];
                        nec = JObject.Parse(JsonConvert.SerializeObject(info));
                        nec.Remove("BR");
                        jnecArray.Insert(channel, nec);
                    }

                }
                jar.Add("VEC", jnecArray);
                jar.Add("EN", 1);
                jar.Add("RM", 1);
                jar.Add("MC", ulChannelBits);
                jmdvr.Add("AR", jar);

                jparameter.Add("MDVR", jmdvr);

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);


                    //判定是否设置成功
                    Assert.IsNotNull(jresp["ERRORCODE"]);
                    Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));
                }
            } while (false);


            Sleep(90 * 1000);

            do
            {
                foreach (string ip in IPList)
                {
                    Console.WriteLine("ip = {0}", ip);
                    N9MSession session = new N9MSession(ip, port);
                    Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                    DateTime dtEndTime = DateTime.Now;
                    DateTime dtStartTime = dtEndTime.AddMinutes(-1);

                    int nStreamType = (int)StreamType.MIRROR_STREAM;
                    int nChannelBits = nChannelMask;

                    string starttime = dtStartTime.ToString("yyyyMMddHHmmss");
                    string endtime = dtEndTime.ToString("yyyyMMddHHmmss");

                    JObject jparameter = new JObject();

                    //首先获取段内文件的大小
                    jparameter = new JObject();
                    jparameter["DATATYPE"] = 0;
                    jparameter["STREAMTYPE"] = nStreamType;
                    jparameter["RECORDTYPE"] = 3;
                    jparameter["STIME"] = starttime;
                    jparameter["ETIME"] = endtime;
                    jparameter["NEWSTREAMTYPE"] = 1 << nStreamType;
                    jparameter["RESTRAGE"] = 1 << (int)ReferStorage.REFER_STORAGE_HDD | (1 << (int)ReferStorage.REFER_STORAGE_SD);
                    jparameter["CHANNEL"] = nChannelBits;

                    JObject jresp = session.SendCommand(Module.STORM, Operation.GETFILESIZEBYTIME, jparameter);
                    Assert.IsNotNull(jresp);
                    Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));

                    int nTotalSize = Convert.ToInt32(jresp["TOTALSIZE"].ToString());

                    long nBitrate = (nTotalSize * 8) / (60 * 1024 * 1024);

                    Console.WriteLine("nTotalSize = {0} 计算总码率为{1}Mb", nTotalSize, nBitrate);

                    Assert.IsTrue(Math.Abs(nTotalCodeRate_setting - nBitrate) < 2, "码率比较失败,期望码率{0}Mb  实际码率{1}Mb", nTotalCodeRate_setting, nBitrate);
                }
            } while (false);
        }

        [Test]
        public void RecordNetMaximum()
        {
            XmlNodeList ChannelInfoList = this.CFG.gNodes("Record/RecordNetMaximum/channel");
            int nChannelMask = Convert.ToInt32(this.CFG.gNode("Record/RecordNetMaximum/channelmask").InnerText);

            Dictionary<int, NetStreamEncoderInfo> dict = new Dictionary<int, NetStreamEncoderInfo>();

            int nTotalCodeRate_setting = 0;

            nTotalCodeRate_setting = Convert.ToInt32(this.CFG.gNode("Record/RecordNetMaximum/total-coderate").InnerText);

            Console.WriteLine("nTotalCodeRate_setting = {0}", nTotalCodeRate_setting);

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
                jmdvr.Add("SUBSTRNET", "?");
                jparameter.Add("MDVR", jmdvr);

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);
                Assert.IsNotNull(jresp["MDVR"]);
                Assert.IsNotNull(jresp["MDVR"]["SUBSTRNET"]["NEC"]);

                List<NetStreamEncoderInfo> list = JsonConvert.DeserializeObject<List<NetStreamEncoderInfo>>(jresp["MDVR"]["SUBSTRNET"]["NEC"].ToString());


                //按照参数进行录像参数设定
                jparameter = new JObject();
                jmdvr = new JObject();
                JObject jsubstrnet = new JObject();

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
                jsubstrnet.Add("NEC", jnecArray);
                jmdvr.Add("SUBSTRNET", jsubstrnet);

                jparameter.Add("MDVR", jmdvr);

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);


                //判定是否设置成功
                Assert.IsNotNull(jresp["ERRORCODE"]);
                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));

                jmdvr = new JObject();
                jparameter = new JObject();

                jmdvr.Add("SUBSTRNET", "?");
                jparameter.Add("MDVR", jmdvr);

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);
                Assert.IsNotNull(jresp["MDVR"]);
                Assert.IsNotNull(jresp["MDVR"]["SUBSTRNET"]["NEC"]);

                list = JsonConvert.DeserializeObject<List<NetStreamEncoderInfo>>(jresp["MDVR"]["SUBSTRNET"]["NEC"].ToString());

                foreach (var item in dict)
                {
                    int channel = item.Key;
                    NetStreamEncoderInfo info_setting = dict[channel];
                    NetStreamEncoderInfo info_getting = list[channel];

                    Assert.IsTrue(info_setting.isSettingSuccess(info_getting));
                }

                int nTotalCodeRate_getting = 0;
                foreach (NetStreamEncoderInfo info in list)
                {
                    nTotalCodeRate_getting += info.Bitrate;
                    Console.WriteLine("info.Bitrate ={0} nTotalCodeRate = {1}", info.Bitrate, nTotalCodeRate_getting);
                }

                Assert.IsTrue(Math.Abs(nTotalCodeRate_setting - nTotalCodeRate_getting * 8 / 1000) < 2, "码率比较失败");
            }
        }
    }
}
