using NUnit.Framework;
using N9MTest.SDK.net;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Util;
using RM;
using N9MTest.commons;

namespace N9MTest.Playback
{
    [TestFixture]
    public class TestCase_PlaybackSearch:TestCase_Basecase
    {
        [Test]
        public void HDDPlaybackSearch()
        {
            int nStreamType = (int)StreamType.MAIN_STREAM;
            uint nChannelBits = 0xffffff;
            string starttime = "20170405000000";
            string endtime = "20170407120000";

            nStreamType = Convert.ToInt32(this.CFG.gNode("Playback/HDDPlaybackSearch/streamtype").InnerText);
            nChannelBits = Convert.ToUInt32(this.CFG.gNode("Playback/HDDPlaybackSearch/channel").InnerText);
            starttime = this.CFG.gNode("Playback/HDDPlaybackSearch/starttime").InnerText;
            endtime = this.CFG.gNode("Playback/HDDPlaybackSearch/endtime").InnerText;

            Console.WriteLine("nStreamType ={0}", nStreamType);
            Console.WriteLine("nChannelBits ={0}", nChannelBits);
            Console.WriteLine("starttime ={0}", starttime);
            Console.WriteLine("endtime ={0}", endtime);


            List<string> resultList = new List<string>();
            foreach (string ip in IPList)
            {
                Console.WriteLine("ip ={0}", ip);

                Dictionary<String, String> dict = new Dictionary<String, String>();

                while (starttime.Substring(0, 8) != endtime.Substring(0, 8))
                {
                    DateTime dtStartTime;
                    DateTime dtEndTime;

                    dtStartTime = DateTime.ParseExact(starttime, "yyyyMMddHHmmss", null);
                    dtEndTime = DateTime.ParseExact(starttime.Substring(0, 8) + "235959", "yyyyMMddHHmmss", null);

                    Console.WriteLine("datetime = {0}", dtEndTime.Add(new TimeSpan(0, 0, 1)).ToString());

                    dict.Add(starttime, dtEndTime.ToString("yyyyMMddHHmmss"));

                    starttime = dtEndTime.AddSeconds(1).ToString("yyyyMMddHHmmss");
                }

                dict.Add(starttime, endtime);

                Console.WriteLine("dict = {0}", dict.Count);

                foreach (var item in dict)
                {
                    Console.WriteLine("starttime = {0} endtime = {1}", item.Key, item.Value);
                    N9MSession session = new N9MSession(ip, port);
                    session.Login(username, password);

                    JObject jparameter = new JObject();
                    jparameter["STREAMTYPE"] = nStreamType;
                    jparameter["FILETYPE"] = 0xffffffff;
                    jparameter["CHANNEL"] = nChannelBits;
                    jparameter["STARTTIME"] = item.Key;
                    jparameter["ENDTIME"] = item.Value;
                    jparameter["NEWSTREAMTYPE"] = 1<< nStreamType;
                    jparameter["RFSTORAGE"] = (int)ReferStorage.REFER_STORAGE_HDD;

                    JObject jresp = session.SendCommand(Module.STORM, Operation.QUERYFILELIST, jparameter);

                    session.Logout();


                    if (jresp["FILELIST"] == null)
                    {
                        Console.WriteLine("设备IP:{0} 在时间段{1}-{2}文件列表为空", ip, item.Key, item.Value);
                    }

                    Assert.IsNotNull(jresp["FILELIST"]);

                    List<RemoteFileInfo> list = JsonConvert.DeserializeObject<List<RemoteFileInfo>>(jresp["FILELIST"].ToString());

                    list.Sort(RemoteFileInfo.comparison);

                    TimeSection timeSection = new TimeSection();

                    foreach (RemoteFileInfo info in list)
                    {
                        if (info == null)
                        {
                            continue;
                        }

                        timeSection.merage(info.nChannel, info.szTime.Split('-')[0], info.szTime.Split('-')[1]);
                    }

                    for (int i = 0; i < sizeof(UInt32) * 8; i++)
                    {
                        if (((nChannelBits >> i) & 0x01) == 1)
                        {
                            if (false == timeSection.IsContinuous(i, item.Key, item.Value))
                            {
                                resultList.Add(string.Format("设备IP:{0} 通道{1} 没有连续", ip, i));
                            }
                        }
                    }
                }
            }

            if (resultList != null && resultList.Count > 0)
            {
                string info = "";
                foreach (string result in resultList)
                {
                    info += result;
                    info += "\n";
                }

                Assert.IsTrue(false, info);
            }
        }

        [Test]
        public void SDPlaybackSearch()
        {
            int nStreamType = (int)StreamType.MAIN_STREAM;
            uint nChannelBits = 0xffffff;
            string starttime = "20170405000000";
            string endtime = "20170407120000";

            nStreamType = Convert.ToInt32(this.CFG.gNode("Playback/SDPlaybackSearch/streamtype").InnerText);
            nChannelBits = Convert.ToUInt32(this.CFG.gNode("Playback/SDPlaybackSearch/channel").InnerText);
            starttime = this.CFG.gNode("Playback/SDPlaybackSearch/starttime").InnerText;
            endtime = this.CFG.gNode("Playback/SDPlaybackSearch/endtime").InnerText;

            Console.WriteLine("nStreamType ={0}", nStreamType);
            Console.WriteLine("nChannelBits ={0}", nChannelBits);
            Console.WriteLine("starttime ={0}", starttime);
            Console.WriteLine("endtime ={0}", endtime);

            List<string> resultList = new List<string>();


            foreach (string ip in IPList)
            {
                Console.WriteLine("ip ={0}", ip);

                Dictionary<String, String> dict = new Dictionary<String, String>();

                while (starttime.Substring(0, 8) != endtime.Substring(0, 8))
                {
                    DateTime dtStartTime;
                    DateTime dtEndTime;

                    dtStartTime = DateTime.ParseExact(starttime, "yyyyMMddHHmmss", null);
                    dtEndTime = DateTime.ParseExact(starttime.Substring(0, 8) + "235959", "yyyyMMddHHmmss", null);

                    Console.WriteLine("datetime = {0}", dtEndTime.Add(new TimeSpan(0, 0, 1)).ToString());

                    dict.Add(starttime, dtEndTime.ToString("yyyyMMddHHmmss"));

                    starttime = dtEndTime.AddSeconds(1).ToString("yyyyMMddHHmmss");
                }

                dict.Add(starttime, endtime);

                Console.WriteLine("dict = {0}", dict.Count);

                foreach (var item in dict)
                {
                    Console.WriteLine("starttime = {0} endtime = {1}", item.Key, item.Value);
                    N9MSession session = new N9MSession(ip, port);
                    session.Login(username, password);

                    JObject jparameter = new JObject();
                    jparameter["STREAMTYPE"] = nStreamType;
                    jparameter["FILETYPE"] = 0xffffffff;
                    jparameter["CHANNEL"] = nChannelBits;
                    jparameter["STARTTIME"] = item.Key;
                    jparameter["ENDTIME"] = item.Value;
                    jparameter["NEWSTREAMTYPE"] = 1<< nStreamType;
                    jparameter["RFSTORAGE"] = (int)ReferStorage.REFER_STORAGE_SD;

                    JObject jresp = session.SendCommand(Module.STORM, Operation.QUERYFILELIST, jparameter);

                    Assert.IsNotNull(jresp["FILELIST"]);

                    List<RemoteFileInfo> list = JsonConvert.DeserializeObject<List<RemoteFileInfo>>(jresp["FILELIST"].ToString());

                    TimeSection timeSection = new TimeSection();

                    foreach (RemoteFileInfo info in list)
                    {
                        if (info == null)
                        {
                            continue;
                        }

                        timeSection.merage(info.nChannel, info.szTime.Split('-')[0], info.szTime.Split('-')[1]);
                    }

                    for (int i = 0; i < sizeof(UInt32) * 8; i++)
                    {
                        if (((nChannelBits >> i) & 0x01) == 1)
                        {
                            if (false == timeSection.IsContinuous(i, item.Key, item.Value))
                            {
                                resultList.Add(string.Format("设备IP:{0} 通道{1} 没有连续", ip, i));
                            }
                        }
                    }
                }
            }

            if (resultList != null && resultList.Count > 0)
            {
                string info = "";
                foreach (string result in resultList)
                {
                    info += result;
                    info += "\n";
                }

                Assert.IsTrue(false, info);
            }
        }
		[Ignore("11111")]
		[Test]
        public void PerformanceRecord()
        {
            int nStreamType = (int)StreamType.MAIN_STREAM;
            uint nChannelBits = 0xffffff;
            string starttime = "20170405000000";
            string endtime = "20170407120000";

            nStreamType = Convert.ToInt32(this.CFG.gNode("Playback/PerformanceRecord/streamtype").InnerText);
            nChannelBits = Convert.ToUInt32(this.CFG.gNode("Playback/PerformanceRecord/channel").InnerText);
            starttime = this.CFG.gNode("Playback/PerformanceRecord/starttime").InnerText;
            endtime = this.CFG.gNode("Playback/PerformanceRecord/endtime").InnerText;

            Console.WriteLine("nStreamType ={0}", nStreamType);
            Console.WriteLine("nChannelBits ={0}", nChannelBits);
            Console.WriteLine("starttime ={0}", starttime);
            Console.WriteLine("endtime ={0}", endtime);


            foreach (string ip in IPList)
            {
                Dictionary<String, String> dict = new Dictionary<String, String>();

                while (starttime.Substring(0, 8) != endtime.Substring(0, 8))
                {
                    DateTime dtStartTime;
                    DateTime dtEndTime;

                    dtStartTime = DateTime.ParseExact(starttime, "yyyyMMddHHmmss", null);
                    dtEndTime = DateTime.ParseExact(starttime.Substring(0, 8) + "235959", "yyyyMMddHHmmss", null);

                    Console.WriteLine("datetime = {0}", dtEndTime.Add(new TimeSpan(0, 0, 1)).ToString());

                    dict.Add(starttime, dtEndTime.ToString("yyyyMMddHHmmss"));

                    starttime = dtEndTime.Add(new TimeSpan(0, 0, 1)).ToString("yyyyMMddHHmmss");
                }

                dict.Add(starttime, endtime);

                Console.WriteLine("dict = {0}", dict.Count);

                foreach (var item in dict)
                {
                    N9MSession session = new N9MSession(ip, port);
                    session.Login(username, password);

                    JObject jparameter = new JObject();
                    jparameter.Add("STREAMTYPE", nStreamType);
                    jparameter.Add("FILETYPE", 0xffffffff);
                    jparameter.Add("CHANNEL", nChannelBits);
                    jparameter.Add("STARTTIME", item.Key);
                    jparameter.Add("ENDTIME", item.Value);

                    JObject jresp = session.SendCommand(Module.STORM, Operation.QUERYFILELIST, jparameter);

                    Assert.IsNotNull(jresp["FILELIST"]);

                    List<RemoteFileInfo> list = JsonConvert.DeserializeObject<List<RemoteFileInfo>>(jresp["FILELIST"].ToString());

                    TimeSection timeSection = new TimeSection();

                    foreach (RemoteFileInfo info in list)
                    {
                        if (info == null)
                        {
                            continue;
                        }

                        timeSection.merage(info.nChannel, info.szTime.Split('-')[0], info.szTime.Split('-')[1]);
                    }

                    for (int i = 0; i < sizeof(UInt32); i++)
                    {
                        if (((nChannelBits >> i) & 0x01) == 1)
                        {
                            Assert.IsTrue(timeSection.IsContinuous(i, item.Key, item.Value));
                        }
                    }

                    session.Logout();
                }
            }
        }

		[Ignore("11111")]
		[Test]
        public void AlarmLogLinkRecord()
        {
            Assert.IsTrue(false, "工装暂不支持IO报警");
        }

		[Ignore("11111")]
		[Test]
        public void AlarmLogLinkBack()
        {
            Assert.IsTrue(false, "工装暂不支持IO报警");
        }
    }
}
