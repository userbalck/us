using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using N9MTest.SDK.net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using N9MTest.SDK.http;

namespace N9MTest.Record
{
    [TestFixture]
    class TestCase_SingleStream:TestCase_Basecase
    {
        /// <summary>
        /// 主码流录像
        /// </summary>
        [Test]
        public void MainRecord()
        {
            int nDuration = Convert.ToInt32(this.CFG.gNode("Record/MainRecord/duration").InnerText);
            int nChannelBits = Convert.ToInt32(this.CFG.gNode("Record/MainRecord/channelbits").InnerText);

            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();

                //首先获取主码流的参数 辅助码流的参数
                jparameter = new JObject();
                jmdvr = new JObject();
                jmdvr.Add("AR", "?");
                jmdvr.Add("RP", "?");
                jmdvr.Add("MAIN", "?");
                jparameter.Add("MDVR", jmdvr);

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);
                Assert.IsNotNull(jresp["MDVR"]);
                Assert.IsNotNull(jresp["MDVR"]["MAIN"]);
                Assert.IsNotNull(jresp["MDVR"]["AR"]);

                //设定开启主码流录像 关闭辅助码流录像

                jparameter = new JObject();
                jmdvr = new JObject();

                JArray jmainArray = new JArray();

                for (int i = 0; i < sizeof(UInt32); i++)
                {
                    if(((nChannelBits >> i) & 0x01) == 1)
                    {
                        JObject jmain = new JObject();
                        jmain["VEN"] = 1;
                        jmainArray.Insert(i, jmain);
                    }
                }

                jmdvr["MAIN"] = jmainArray;

                JObject jar = new JObject();
                jar["EN"] = 0;

                JArray jvecArray = new JArray();

                for (int i = 0; i < sizeof(UInt32); i++)
                {
                    JObject jvec = new JObject();
                    jvec["VEN"] = 0;
                    jvecArray.Insert(i, jvec);
                }

                jar["VEC"] = jvecArray;
    
                jmdvr["AR"] = jar;

                jparameter["MDVR"] = jmdvr;

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Sleep(10 * 1000);

                for(int i = 0; i < sizeof(UInt32); i++)
                {
                    if (((nChannelBits >> i) & 0x01) == 1)
                    {
                        Assert.IsTrue(session.isChannelRecording(StreamType.MAIN_STREAM, i), "主码流{0}通道没有正常录像", i);
                        Assert.IsFalse(session.isChannelRecording(StreamType.SUB_STREAM, i), "子码流{0}通道没有停止录像", i);
                        Assert.IsFalse(session.isChannelRecording(StreamType.MIRROR_STREAM, i), "镜像码流{0}通道没有停止录像", i);
                    }
                }
               

                Sleep((nDuration + 30) * 1000);

                DateTime dtEndTime = DateTime.Now;
                DateTime dtStartTime = dtEndTime.AddMinutes(-1);

                string starttime = dtStartTime.ToString("yyyyMMddHHmmss");
                string endtime = dtEndTime.ToString("yyyyMMddHHmmss");

                Sleep(30 * 1000);

                int nStreamType = (int)StreamType.MAIN_STREAM;

                jparameter = new JObject();
                jparameter.Add("STREAMTYPE", nStreamType);
                jparameter.Add("FILETYPE", 0xffffffff);
                jparameter.Add("CHANNEL", nChannelBits);
                jparameter.Add("STARTTIME", starttime);
                jparameter.Add("ENDTIME", endtime);

                jresp = session.SendCommand(Module.STORM, Operation.QUERYFILELIST, jparameter);

                Assert.IsNotNull(jresp["FILELIST"]);

                List<RemoteFileInfo> RemoteFileList = JsonConvert.DeserializeObject<List<RemoteFileInfo>>(jresp["FILELIST"].ToString());

                foreach (RemoteFileInfo info in RemoteFileList)
                {
                    //首先获取段内文件的大小
                    jparameter = new JObject();
                    jparameter["DATATYPE"] = 0;
                    jparameter["STREAMTYPE"] = nStreamType;
                    jparameter["RECORDTYPE"] = 3;
                    jparameter["STIME"] = starttime;
                    jparameter["ETIME"] = endtime;
                    jparameter["CHANNEL"] = 1 << info.nChannel;

                    jresp = session.SendCommand(Module.STORM, Operation.GETFILESIZEBYTIME, jparameter);
                    Assert.IsNotNull(jresp);
                    Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));

                    int nTotalSize = Convert.ToInt32(jresp["TOTALSIZE"].ToString());

                    session.DownloadVideo(StreamType.MAIN_STREAM, info.recordID, info.nChannel, info.starttime, info.endtime, "video.data");

                    H264FileLoader loader = new H264FileLoader("video.data");
                    loader.Parse();
                    loader.Dispose();

                    DateTime dtFileStartTime;
                    DateTime dtFileEndTime;

                    loader.GetTimeDuration(out dtFileStartTime, out dtFileEndTime);
                    Assert.GreaterOrEqual((dtFileEndTime - dtFileStartTime).TotalSeconds, 60, "录像文件时长小于60秒");
                }
            }
        }

        /// <summary>
        /// 内置镜像录像
        /// </summary>
		[Ignore("1111111")]
        [Test]
        public void InternalMirrorRecord()
        {
            int nDuration = Convert.ToInt32(this.CFG.gNode("Record/InternalMirrorRecord/duration").InnerText);
            int nChannelBits = Convert.ToInt32(this.CFG.gNode("Record/InternalMirrorRecord/channelbits").InnerText);

            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();

                //首先获取主码流的参数 辅助码流的参数
                jparameter = new JObject();
                jmdvr = new JObject();
                jmdvr.Add("AR", "?");
                jmdvr.Add("RP", "?");
                jmdvr.Add("MAIN", "?");
                jparameter.Add("MDVR", jmdvr);

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);
                Assert.IsNotNull(jresp["MDVR"]);
                Assert.IsNotNull(jresp["MDVR"]["MAIN"]);
                Assert.IsNotNull(jresp["MDVR"]["AR"]);

                //设定开启主码流录像 设置辅助码流录像为镜像码流录像

                jparameter = new JObject();
                jmdvr = new JObject();

                JArray jmainArray = new JArray();

                for (int i = 0; i < sizeof(UInt32); i++)
                {
                    if (((nChannelBits >> i) & 0x01) == 1)
                    {
                        JObject jmain = new JObject();
                        jmain["VEN"] = 1;
                        jmainArray.Insert(i, jmain);
                    }
                }

                jmdvr["MAIN"] = jmainArray;

                JObject jar = new JObject();
                jar["EN"] = 1;

                //设置内置SD卡录像
                jar["HID"] = 0;

                //设定当前码流录像为镜像码流录像
                jar["RM"] = (int)AssistRecordMode.Mirror;

                //设置镜像录像录像对应通道
                jar["MC"] = nChannelBits;

                JArray jvecArray = new JArray();

                for (int i = 0; i < sizeof(UInt32); i++)
                {
                    JObject jvec = new JObject();
                    jvec["VEN"] = 1;
                    jvecArray.Insert(i, jvec);
                }

                jar["VEC"] = jvecArray;

                jmdvr["AR"] = jar;

                jparameter["MDVR"] = jmdvr;

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Sleep(30 * 1000);

                for (int i = 0; i < sizeof(UInt32); i++)
                {
                    if (((nChannelBits >> i) & 0x01) == 1)
                    {
                        Assert.IsTrue(session.isChannelRecording(StreamType.MAIN_STREAM, i), "主码流{0}通道没有正常录像", i);
                        Assert.IsFalse(session.isChannelRecording(StreamType.SUB_STREAM, i), "子码流{0}通道没有停止录像", i);
                        Assert.IsTrue(session.isChannelRecording(StreamType.MIRROR_STREAM, i), "镜像码流{0}通道没有正常录像", i);
                    }
                }


                Sleep((nDuration + 30) * 1000);

                DateTime dtEndTime = DateTime.Now;
                DateTime dtStartTime = dtEndTime.AddMinutes(-1);

                string starttime = dtStartTime.ToString("yyyyMMddHHmmss");
                string endtime = dtEndTime.ToString("yyyyMMddHHmmss");

                Sleep(30 * 1000);

                int nStreamType = (int)StreamType.MIRROR_STREAM;

                jparameter = new JObject();
                jparameter.Add("STREAMTYPE", nStreamType);
                jparameter.Add("FILETYPE", 0xffffffff);
                jparameter.Add("CHANNEL", nChannelBits);
                jparameter.Add("STARTTIME", starttime);
                jparameter.Add("ENDTIME", endtime);

                jresp = session.SendCommand(Module.STORM, Operation.QUERYFILELIST, jparameter);

                Assert.IsNotNull(jresp["FILELIST"]);

                List<RemoteFileInfo> RemoteFileList = JsonConvert.DeserializeObject<List<RemoteFileInfo>>(jresp["FILELIST"].ToString());

                foreach (RemoteFileInfo info in RemoteFileList)
                {
                    //首先获取段内文件的大小
                    jparameter = new JObject();
                    jparameter["DATATYPE"] = 0;
                    jparameter["STREAMTYPE"] = nStreamType;
                    jparameter["RECORDTYPE"] = 3;
                    jparameter["STIME"] = starttime;
                    jparameter["ETIME"] = endtime;
                    jparameter["CHANNEL"] = 1 << info.nChannel;

                    jresp = session.SendCommand(Module.STORM, Operation.GETFILESIZEBYTIME, jparameter);
                    Assert.IsNotNull(jresp);
                    Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));

                    int nTotalSize = Convert.ToInt32(jresp["TOTALSIZE"].ToString());

                    session.DownloadVideo(StreamType.MAIN_STREAM, info.recordID, info.nChannel, info.starttime, info.endtime, "video.data");

                    H264FileLoader loader = new H264FileLoader("video.data");
                    loader.Parse();
                    loader.Dispose();

                    DateTime dtFileStartTime;
                    DateTime dtFileEndTime;

                    loader.GetTimeDuration(out dtFileStartTime, out dtFileEndTime);

                    

                    Assert.GreaterOrEqual((dtFileEndTime - dtFileStartTime).TotalSeconds, 60, "录像文件时长小于60秒");
                }
            }
        }

		/// <summary>
		/// 外置镜像录像 HID 为1
		/// </summary>
		[Ignore("1111111")]
		[Test]
        public void ExternalMirrorRecord()
        {
            int nDuration = Convert.ToInt32(this.CFG.gNode("Record/ExternalMirrorRecord/duration").InnerText);
            int nChannelBits = Convert.ToInt32(this.CFG.gNode("Record/ExternalMirrorRecord/channelbits").InnerText);

            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();

                //首先获取主码流的参数 辅助码流的参数
                jparameter = new JObject();
                jmdvr = new JObject();
                jmdvr.Add("AR", "?");
                jmdvr.Add("RP", "?");
                jmdvr.Add("MAIN", "?");
                jparameter.Add("MDVR", jmdvr);

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);
                Assert.IsNotNull(jresp["MDVR"]);
                Assert.IsNotNull(jresp["MDVR"]["MAIN"]);
                Assert.IsNotNull(jresp["MDVR"]["AR"]);

                //设定开启主码流录像 设置辅助码流录像为镜像码流录像

                jparameter = new JObject();
                jmdvr = new JObject();

                JArray jmainArray = new JArray();

                for (int i = 0; i < sizeof(UInt32); i++)
                {
                    if (((nChannelBits >> i) & 0x01) == 1)
                    {
                        JObject jmain = new JObject();
                        jmain["VEN"] = 1;
                        jmainArray.Insert(i, jmain);
                    }
                }

                jmdvr["MAIN"] = jmainArray;

                JObject jar = new JObject();

                jar["EN"] = 1;

                //设置外置SD卡录像
                jar["HID"] = 1;

                //设定当前码流录像为镜像码流录像
                jar["RM"] = (int)AssistRecordMode.Mirror;

                //设置镜像码流的录像通道
                jar["MC"] = nChannelBits;

                JArray jvecArray = new JArray();

                for (int i = 0; i < sizeof(UInt32); i++)
                {
                    JObject jvec = new JObject();
                    jvec["VEN"] = 1;
                    jvecArray.Insert(i, jvec);
                }

                jar["VEC"] = jvecArray;

                jmdvr["AR"] = jar;

                jparameter["MDVR"] = jmdvr;

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Sleep(10 * 1000);

                for (int i = 0; i < sizeof(UInt32); i++)
                {
                    if (((nChannelBits >> i) & 0x01) == 1)
                    {
                        Assert.IsTrue(session.isChannelRecording(StreamType.MAIN_STREAM, i), "主码流{0}通道没有正常录像", i);
                    //    Assert.IsFalse(session.isChannelRecording(StreamType.SUB_STREAM, i), "子码流{0}通道没有停止录像", i);
                        Assert.IsTrue(session.isChannelRecording(StreamType.MIRROR_STREAM, i), "镜像码流{0}通道没有正常录像", i);
                    }
                }


                Sleep((nDuration + 30) * 1000);

                DateTime dtEndTime = DateTime.Now;
                DateTime dtStartTime = dtEndTime.AddMinutes(-1);

                string starttime = dtStartTime.ToString("yyyyMMddHHmmss");
                string endtime = dtEndTime.ToString("yyyyMMddHHmmss");

                Sleep(30 * 1000);

                int nStreamType = (int)StreamType.MIRROR_STREAM;

                jparameter = new JObject();
                jparameter.Add("STREAMTYPE", nStreamType);
                jparameter.Add("FILETYPE", 0xffffffff);
                jparameter.Add("CHANNEL", nChannelBits);
                jparameter.Add("STARTTIME", starttime);
                jparameter.Add("ENDTIME", endtime);

                jresp = session.SendCommand(Module.STORM, Operation.QUERYFILELIST, jparameter);

                Assert.IsNotNull(jresp["FILELIST"]);

                List<RemoteFileInfo> RemoteFileList = JsonConvert.DeserializeObject<List<RemoteFileInfo>>(jresp["FILELIST"].ToString());

                foreach (RemoteFileInfo info in RemoteFileList)
                {
                    //首先获取段内文件的大小
                    jparameter = new JObject();
                    jparameter["DATATYPE"] = 0;
                    jparameter["STREAMTYPE"] = nStreamType;
                    jparameter["RECORDTYPE"] = 3;
                    jparameter["STIME"] = starttime;
                    jparameter["ETIME"] = endtime;
                    jparameter["CHANNEL"] = 1 << info.nChannel;

                    jresp = session.SendCommand(Module.STORM, Operation.GETFILESIZEBYTIME, jparameter);
                    Assert.IsNotNull(jresp);
                    Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));

                    int nTotalSize = Convert.ToInt32(jresp["TOTALSIZE"].ToString());

                    session.DownloadVideo(StreamType.MAIN_STREAM, info.recordID, info.nChannel, info.starttime, info.endtime, "video.data");

                    H264FileLoader loader = new H264FileLoader("video.data");
                    loader.Parse();
                    loader.Dispose();

                    DateTime dtFileStartTime;
                    DateTime dtFileEndTime;

                    loader.GetTimeDuration(out dtFileStartTime, out dtFileEndTime);

                    Assert.GreaterOrEqual((dtFileEndTime - dtFileStartTime).TotalSeconds, 60, "录像文件时长小于60秒");
                }
            }
        }

        /// <summary>
        /// 内置子码流录像 存储类型HID 为 0
        /// </summary>

        [Test]
        public void InternalSubRecord()
        {
            int nDuration = Convert.ToInt32(this.CFG.gNode("Record/InternalSubRecord/duration").InnerText);
            int nChannelBits = Convert.ToInt32(this.CFG.gNode("Record/InternalSubRecord/channelbits").InnerText);

            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();

                //首先获取主码流的参数 辅助码流的参数
                jparameter = new JObject();
                jmdvr = new JObject();
                jmdvr.Add("AR", "?");
                jmdvr.Add("RP", "?");
                jmdvr.Add("MAIN", "?");
                jparameter.Add("MDVR", jmdvr);

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);
                Assert.IsNotNull(jresp["MDVR"]);
                Assert.IsNotNull(jresp["MDVR"]["MAIN"]);
                Assert.IsNotNull(jresp["MDVR"]["AR"]);

                //设定开启主码流录像 设置辅助码流录像为子码流录像

                jparameter = new JObject();
                jmdvr = new JObject();

                JArray jmainArray = new JArray();

                for (int i = 0; i < sizeof(UInt32); i++)
                {
                    if (((nChannelBits >> i) & 0x01) == 1)
                    {
                        JObject jmain = new JObject();
                        jmain["VEN"] = 1;
                        jmainArray.Insert(i, jmain);
                    }
                }

                jmdvr["MAIN"] = jmainArray;

                JObject jar = new JObject();
                jar["EN"] = 1;

                //设定当前码流录像为子码流录像
                jar["RM"] = (int)AssistRecordMode.Sub;

                //设置子码流的录像通道
                jar["SSC"] = nChannelBits;

                //设置内置SD卡录像
                jar["HID"] = 0;

                //设置子码流录像通道
                jar["SSC"] = nChannelBits;

                JArray jvecArray = new JArray();

                for (int i = 0; i < sizeof(UInt32); i++)
                {
                    JObject jvec = new JObject();
                    jvec["VEN"] = 1;
                    jvecArray.Insert(i, jvec);
                }

                jar["VEC"] = jvecArray;

                jmdvr["AR"] = jar;

                jparameter["MDVR"] = jmdvr;

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Sleep(10 * 1000);

                for (int i = 0; i < sizeof(UInt32); i++)
                {
                    if (((nChannelBits >> i) & 0x01) == 1)
                    {
                        Assert.IsTrue(session.isChannelRecording(StreamType.MAIN_STREAM, i), "主码流{0}通道没有正常录像", i);
                        Assert.IsTrue(session.isChannelRecording(StreamType.SUB_STREAM, i), "子码流{0}通道没有正常录像", i);
                        Assert.IsFalse(session.isChannelRecording(StreamType.MIRROR_STREAM, i), "镜像码流{0}通道没有停止录像", i);
                    }
                }


                Sleep((nDuration + 30) * 1000);

                DateTime dtEndTime = DateTime.Now;
                DateTime dtStartTime = dtEndTime.AddMinutes(-1);

                string starttime = dtStartTime.ToString("yyyyMMddHHmmss");
                string endtime = dtEndTime.ToString("yyyyMMddHHmmss");

                Sleep(30 * 1000);

                int nStreamType = (int)StreamType.SUB_STREAM;

                jparameter = new JObject();
                jparameter.Add("STREAMTYPE", nStreamType);
                jparameter.Add("FILETYPE", 0xffffffff);
                jparameter.Add("CHANNEL", nChannelBits);
                jparameter.Add("STARTTIME", starttime);
                jparameter.Add("ENDTIME", endtime);

                jresp = session.SendCommand(Module.STORM, Operation.QUERYFILELIST, jparameter);

                Assert.IsNotNull(jresp["FILELIST"]);

                List<RemoteFileInfo> RemoteFileList = JsonConvert.DeserializeObject<List<RemoteFileInfo>>(jresp["FILELIST"].ToString());

                foreach (RemoteFileInfo info in RemoteFileList)
                {
                    //首先获取段内文件的大小
                    jparameter = new JObject();
                    jparameter["DATATYPE"] = 0;
                    jparameter["STREAMTYPE"] = nStreamType;
                    jparameter["RECORDTYPE"] = 3;
                    jparameter["STIME"] = starttime;
                    jparameter["ETIME"] = endtime;
                    jparameter["CHANNEL"] = 1 << info.nChannel;

                    jresp = session.SendCommand(Module.STORM, Operation.GETFILESIZEBYTIME, jparameter);
                    Assert.IsNotNull(jresp);
                    Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));

                    int nTotalSize = Convert.ToInt32(jresp["TOTALSIZE"].ToString());

                    session.DownloadVideo(StreamType.MAIN_STREAM, info.recordID, info.nChannel, info.starttime, info.endtime, "video.data");

                    H264FileLoader loader = new H264FileLoader("video.data");
                    loader.Parse();
                    loader.Dispose();

                    DateTime dtFileStartTime;
                    DateTime dtFileEndTime;

                    loader.GetTimeDuration(out dtFileStartTime, out dtFileEndTime);

                    

                    Assert.GreaterOrEqual((dtFileEndTime - dtFileStartTime).TotalSeconds, 60, "录像文件时长小于60秒");
                }
            }
        }

		/// <summary>
		/// 外置子码流录像
		/// </summary>
	
		[Test]
        public void ExternalSubRecord()
        {
            int nDuration = Convert.ToInt32(this.CFG.gNode("Record/ExternalSubRecord/duration").InnerText);
            int nChannelBits = Convert.ToInt32(this.CFG.gNode("Record/ExternalSubRecord/channelbits").InnerText);

            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");


                //把外置SD卡格式化为N9M格式
                JObject jparameter = new JObject();

                JObject jresp = session.SendCommand(Module.STORM, Operation.GETSTORAGEINFO, null);

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


                Sleep(60 * 1000);

                jparameter = new JObject();
                JObject jmdvr = new JObject();

                //首先获取主码流的参数 辅助码流的参数
                jparameter = new JObject();
                jmdvr = new JObject();
                jmdvr.Add("AR", "?");
                jmdvr.Add("RP", "?");
                jmdvr.Add("MAIN", "?");
                jparameter.Add("MDVR", jmdvr);

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);
                Assert.IsNotNull(jresp["MDVR"]);
                Assert.IsNotNull(jresp["MDVR"]["MAIN"]);
                Assert.IsNotNull(jresp["MDVR"]["AR"]);

                //设定开启主码流录像 设置辅助码流录像为子码流录像

                jparameter = new JObject();
                jmdvr = new JObject();

                JArray jmainArray = new JArray();

                for (int i = 0; i < sizeof(UInt32); i++)
                {
                    if (((nChannelBits >> i) & 0x01) == 1)
                    {
                        JObject jmain = new JObject();
                        jmain["VEN"] = 1;
                        jmainArray.Insert(i, jmain);
                    }
                }

                jmdvr["MAIN"] = jmainArray;

                JObject jar = new JObject();
                jar["EN"] = 1;

                //设定当前码流录像为子码流录像
                jar["RM"] = (int)AssistRecordMode.Sub;

                //设置外置SD卡录像
                jar["HID"] = 1;

                //设置子码流录像通道
                jar["SSC"] = nChannelBits;

                JArray jvecArray = new JArray();

                for (int i = 0; i < sizeof(UInt32); i++)
                {
                    JObject jvec = new JObject();
                    jvec["VEN"] = 1;
                    jvecArray.Insert(i, jvec);
                }

                jar["VEC"] = jvecArray;

                jmdvr["AR"] = jar;

                jparameter["MDVR"] = jmdvr;

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Sleep(10 * 1000);

                for (int i = 0; i < sizeof(UInt32); i++)
                {
                    if (((nChannelBits >> i) & 0x01) == 1)
                    {
                        Assert.IsTrue(session.isChannelRecording(StreamType.MAIN_STREAM, i), "主码流{0}通道没有正常录像", i);
                        Assert.IsTrue(session.isChannelRecording(StreamType.SUB_STREAM, i), "子码流{0}通道没有正常录像", i);
                    }
                }


                Sleep((nDuration + 30) * 1000);

                DateTime dtEndTime = DateTime.Now;
                DateTime dtStartTime = dtEndTime.AddMinutes(-1);

                string starttime = dtStartTime.ToString("yyyyMMddHHmmss");
                string endtime = dtEndTime.ToString("yyyyMMddHHmmss");

                Sleep(30 * 1000);

                int nStreamType = (int)StreamType.SUB_STREAM;

                jparameter = new JObject();
                jparameter.Add("STREAMTYPE", nStreamType);
                jparameter.Add("FILETYPE", 0xffffffff);
                jparameter.Add("CHANNEL", nChannelBits);
                jparameter.Add("STARTTIME", starttime);
                jparameter.Add("ENDTIME", endtime);

                jresp = session.SendCommand(Module.STORM, Operation.QUERYFILELIST, jparameter);

                Assert.IsNotNull(jresp["FILELIST"]);

                List<RemoteFileInfo> RemoteFileList = JsonConvert.DeserializeObject<List<RemoteFileInfo>>(jresp["FILELIST"].ToString());

                foreach (RemoteFileInfo info in RemoteFileList)
                {
                    //首先获取段内文件的大小
                    jparameter = new JObject();
                    jparameter["DATATYPE"] = 0;
                    jparameter["STREAMTYPE"] = nStreamType;
                    jparameter["RECORDTYPE"] = 3;
                    jparameter["STIME"] = starttime;
                    jparameter["ETIME"] = endtime;
                    jparameter["CHANNEL"] = 1 << info.nChannel;

                    jresp = session.SendCommand(Module.STORM, Operation.GETFILESIZEBYTIME, jparameter);
                    Assert.IsNotNull(jresp);
                    Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));

                    int nTotalSize = Convert.ToInt32(jresp["TOTALSIZE"].ToString());

                    session.DownloadVideo(StreamType.MAIN_STREAM, info.recordID, info.nChannel, info.starttime, info.endtime, "video.data");

                    H264FileLoader loader = new H264FileLoader("video.data");
                    loader.Parse();
                    loader.Dispose();

                    DateTime dtFileStartTime;
                    DateTime dtFileEndTime;

                    loader.GetTimeDuration(out dtFileStartTime, out dtFileEndTime);

                    Assert.GreaterOrEqual((dtFileEndTime - dtFileStartTime).TotalSeconds, 60, "录像文件时长小于60秒");
                }
            }
        }

		/// <summary>
		/// 内置报警备份录像
		/// </summary>
		[Ignore("1111111")]
		[Test]
        public void InternalAlarmRecord()
        {
            int nChannelBits = Convert.ToInt32(this.CFG.gNode("Record/InternalAlarmRecord/channelbits").InnerText);

            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备{0}失败", ip);

                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();

                //首先获取主码流的参数 辅助码流的参数
                jparameter = new JObject();
                jmdvr = new JObject();
                jmdvr.Add("AR", "?");
                jmdvr.Add("RP", "?");
                jmdvr.Add("IOP", "?");
                jmdvr.Add("MAIN", "?");
                jparameter.Add("MDVR", jmdvr);

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);
                Assert.IsNotNull(jresp["MDVR"]);
                Assert.IsNotNull(jresp["MDVR"]["MAIN"]);
                Assert.IsNotNull(jresp["MDVR"]["AR"]);
                Assert.IsNotNull(jresp["MDVR"]["IOP"]);


                //首先设定IO1报警为高电平报警 联通录像通道录像
                jparameter = new JObject();
                jmdvr = new JObject();

                JArray jiopArray = new JArray();

                JObject jiop = new JObject();
                jiop["EN"] = 1;
                jiop["EL"] = 1;
                jiop["AS"] = 1;

                JObject japr = new JObject();

                JObject jar = new JObject();
                jar["CH"] = nChannelBits;

                japr["AR"] = jar;
                jiop["APR"] = japr;
                jiopArray.Insert(0, jiop);

                jmdvr["IOP"] = jiopArray;
                jparameter["MDVR"] = jmdvr;

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);


                //设置所有录像为开机录像 主码流录像+辅助码流(报警录像) 存储在内置SD卡
                jparameter = new JObject();
                jmdvr = new JObject();

                //设置为开机录像
                JObject jrp = new JObject();
                JArray jrcpArray = new JArray();

                for (int i = 0; i < sizeof(UInt32); i++)
                {
                    if (((nChannelBits >> i) & 0x01) == 1)
                    {
                        JObject jrcp = new JObject();
                        jrcp["RM"] = (int)RecordMode.Boot;
                        jrcpArray.Insert(i, jrcp);
                    }
                }

                jrp["RCP"] = jrcpArray;

                JArray jmainArray = new JArray();

                for (int i = 0; i < sizeof(UInt32); i++)
                {
                    if (((nChannelBits >> i) & 0x01) == 1)
                    {
                        JObject jmain = new JObject();
                        jmain["VEN"] = 1;
                        jmainArray.Insert(i, jmain);
                    }
                }

                jmdvr["MAIN"] = jmainArray;

                jar = new JObject();
                jar["EN"] = 1;

                //设定当前码流录像为报警录像
                jar["RM"] = (int)AssistRecordMode.Alarm;


                //设定镜像录像的报警通道
                jar["MAC"] = nChannelBits;

                //设置内置SD卡录像
                jar["HID"] = 0;

                JArray jvecArray = new JArray();

                for (int i = 0; i < sizeof(UInt32); i++)
                {
                    JObject jvec = new JObject();
                    jvec["VEN"] = 1;
                    jvecArray.Insert(i, jvec);
                }

                jar["VEC"] = jvecArray;

                jmdvr["AR"] = jar;

                jparameter["MDVR"] = jmdvr;

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Sleep(10 * 1000);

                for (int i = 0; i < sizeof(UInt32); i++)
                {
                    if (((nChannelBits >> i) & 0x01) == 1)
                    {
                        Assert.IsTrue(session.isChannelRecording(StreamType.MAIN_STREAM, i), "主码流{0}通道没有正常录像", i);
                    }
                }

                //首先设定IO1报警为低电平报警 联通录像通道录像
                jparameter = new JObject();
                jmdvr = new JObject();

                jiopArray = new JArray();

                jiop = new JObject();
                jiop["EN"] = 1;
                jiop["EL"] = 0;
                jiop["AS"] = 1;

                japr = new JObject();

                jar = new JObject();
                jar["CH"] = nChannelBits;

                japr["AR"] = jar;
                jiop["APR"] = japr;
                jiopArray.Insert(0, jiop);

                jmdvr["IOP"] = jiopArray;
                jparameter["MDVR"] = jmdvr;

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

               
                Sleep(60 * 1000);

                for (int i = 0; i < sizeof(UInt32); i++)
                {
                    if (((nChannelBits >> i) & 0x01) == 1)
                    {
                        Assert.IsTrue(session.isChannelRecording(StreamType.MAIN_STREAM, i), "主码流{0}通道没有正常录像", i);
                    }
                }

                //恢复设定IO1报警为高电平报警 联通录像通道录像
                jparameter = new JObject();
                jmdvr = new JObject();

                jiopArray = new JArray();

                jiop = new JObject();
                jiop["EN"] = 1;
                jiop["EL"] = 1;
                jiop["AS"] = 1;

                japr = new JObject();

                jar = new JObject();
                jar["CH"] = nChannelBits;

                japr["AR"] = jar;
                jiop["APR"] = japr;
                jiopArray.Insert(0, jiop);

                jmdvr["IOP"] = jiopArray;
                jparameter["MDVR"] = jmdvr;

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                jparameter = new JObject();

                DateTime dtEndTime = DateTime.Now;
                DateTime dtStartTime = dtEndTime.AddMinutes(-1);

                int nStreamType = (int)StreamType.MIRROR_STREAM;

                string starttime = dtStartTime.ToString("yyyyMMddHHmmss");
                string endtime = dtEndTime.ToString("yyyyMMddHHmmss");

                jparameter.Add("STREAMTYPE", nStreamType);
                jparameter.Add("FILETYPE", 0xffffffff);
                jparameter.Add("CHANNEL", nChannelBits);
                jparameter.Add("STARTTIME", starttime);
                jparameter.Add("ENDTIME", endtime);

                jresp = session.SendCommand(Module.STORM, Operation.QUERYFILELIST, jparameter);

                Assert.IsNotNull(jresp["FILELIST"]);

                List<RemoteFileInfo> filelist = JsonConvert.DeserializeObject<List<RemoteFileInfo>>(jresp["FILELIST"].ToString());

                for (int i = 0; i < sizeof(UInt32); i++)
                {
                    if (((nChannelBits >> i) & 0x01) == 1)
                    {
                        bool bGet = false;

                        foreach (RemoteFileInfo info in filelist)
                        {
                            if (info.nChannel == i)
                            {
                                bGet = true;
                            }
                        }

                        Assert.IsTrue(bGet, "通道 {0} 没有进行报警录像", i);
                    }
                    else
                    {
                        bool bGet = false;
                        foreach (RemoteFileInfo info in filelist)
                        {
                            if (info.nChannel == i)
                            {
                                bGet = true;
                            }
                        }

                        Assert.IsFalse(bGet, "通道 {0} 不应该进行报警录像", i);
                    }
                }
               
            }
        }

		/// <summary>
		/// 外置报警备份录像
		/// </summary>
	
        [Test]
        public void ExternalAlarmRecord()
        {
            int nChannelBits = Convert.ToInt32(this.CFG.gNode("Record/ExternalAlarmRecord/channelbits").InnerText);

            foreach (string ip in IPList)
            {
                //首先判定是否有外部存储
                N9MHttpSession httpSession = new N9MHttpSession(ip, webport);
                Assert.AreEqual(0, httpSession.Login("/logincheck.rsp?type=1", username, password));

                string resp = httpSession.Get("/device.rsp?opt=hdd&cmd=state");
                JObject jresp = JObject.Parse(resp);

                Assert.AreEqual(0, Convert.ToInt32(jresp["result"].ToString()));

                List<StorageInfo> storageList = JsonConvert.DeserializeObject<List<StorageInfo>>(jresp["data"].ToString());

                Console.WriteLine("storageList.count = {0}", storageList.Count);

                StorageInfo _info = null;

                foreach (StorageInfo info in storageList)
                {
                    if (info.type == (int)StorageType.STORAGE_EXTERNAL_SDCARD && info.left > 0)
                    {
                        _info = info;
                    }
                }

                Assert.IsNotNull(_info, "没有获取到外置SD卡的信息");

                Assert.IsTrue(_info.left > 0, "外置SD卡存储[存储空间大小:{0}]不大于0", _info.left);

                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备{0}失败", ip);

                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();

                //首先获取主码流的参数 辅助码流的参数
                jparameter = new JObject();
                jmdvr = new JObject();
                jmdvr.Add("AR", "?");
                jmdvr.Add("RP", "?");
                jmdvr.Add("IOP", "?");
                jmdvr.Add("MAIN", "?");
                jparameter.Add("MDVR", jmdvr);

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);
                Assert.IsNotNull(jresp["MDVR"]);
                Assert.IsNotNull(jresp["MDVR"]["MAIN"]);
                Assert.IsNotNull(jresp["MDVR"]["AR"]);
                Assert.IsNotNull(jresp["MDVR"]["IOP"]);


                //首先设定IO1报警为高电平报警 联通录像通道录像
                jparameter = new JObject();
                jmdvr = new JObject();

                JArray jiopArray = new JArray();

                JObject jiop = new JObject();
                jiop["EN"] = 1;
                jiop["EL"] = 1;
                jiop["AS"] = 1;

                JObject japr = new JObject();

                JObject jar = new JObject();
                jar["CH"] = nChannelBits;

                japr["AR"] = jar;
                jiop["APR"] = japr;
                jiopArray.Insert(0, jiop);

                jmdvr["IOP"] = jiopArray;
                jparameter["MDVR"] = jmdvr;

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);


                //设置所有录像为开机录像 主码流录像+辅助码流(报警录像) 存储在内置SD卡
                jparameter = new JObject();
                jmdvr = new JObject();

                //设置为开机录像
                JObject jrp = new JObject();
                JArray jrcpArray = new JArray();

                for (int i = 0; i < sizeof(UInt32); i++)
                {
                    if (((nChannelBits >> i) & 0x01) == 1)
                    {
                        JObject jrcp = new JObject();
                        jrcp["RM"] = (int)RecordMode.Boot;
                        jrcpArray.Insert(i, jrcp);
                    }
                }

                jrp["RCP"] = jrcpArray;

                JArray jmainArray = new JArray();

                for (int i = 0; i < sizeof(UInt32); i++)
                {
                    if (((nChannelBits >> i) & 0x01) == 1)
                    {
                        JObject jmain = new JObject();
                        jmain["VEN"] = 1;
                        jmainArray.Insert(i, jmain);
                    }
                }

                jmdvr["MAIN"] = jmainArray;

                jar = new JObject();
                jar["EN"] = 1;

                //设定当前码流录像为报警录像
                jar["RM"] = (int)AssistRecordMode.Alarm;


                //设定镜像录像的报警通道
                jar["MAC"] = nChannelBits;

                //设置外置SD卡录像
                jar["HID"] = 1;

                JArray jvecArray = new JArray();

                for (int i = 0; i < sizeof(UInt32); i++)
                {
                    JObject jvec = new JObject();
                    jvec["VEN"] = 1;
                    jvecArray.Insert(i, jvec);
                }

                jar["VEC"] = jvecArray;

                jmdvr["AR"] = jar;

                jparameter["MDVR"] = jmdvr;

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Sleep(10 * 1000);

                for (int i = 0; i < sizeof(UInt32); i++)
                {
                    if (((nChannelBits >> i) & 0x01) == 1)
                    {
                        Assert.IsTrue(session.isChannelRecording(StreamType.MAIN_STREAM, i), "主码流{0}通道没有正常录像", i);
                    }
                }

                //首先设定IO1报警为低电平报警 联通录像通道录像
                jparameter = new JObject();
                jmdvr = new JObject();

                jiopArray = new JArray();

                jiop = new JObject();
                jiop["EN"] = 1;
                jiop["EL"] = 0;
                jiop["AS"] = 1;

                japr = new JObject();

                jar = new JObject();
                jar["CH"] = nChannelBits;

                japr["AR"] = jar;
                jiop["APR"] = japr;
                jiopArray.Insert(0, jiop);

                jmdvr["IOP"] = jiopArray;
                jparameter["MDVR"] = jmdvr;

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Sleep(30 * 1000);

                for (int i = 0; i < sizeof(UInt32); i++)
                {
                    if (((nChannelBits >> i) & 0x01) == 1)
                    {
                        Assert.IsTrue(session.isChannelRecording(StreamType.MAIN_STREAM, i), "主码流{0}通道没有正常录像", i);

                    }
                }

                //首先设定IO1报警为高电平报警 联通录像通道录像
                jparameter = new JObject();
                jmdvr = new JObject();

                jiopArray = new JArray();

                jiop = new JObject();
                jiop["EN"] = 1;
                jiop["EL"] = 1;
                jiop["AS"] = 1;

                japr = new JObject();

                jar = new JObject();
                jar["CH"] = nChannelBits;

                japr["AR"] = jar;
                jiop["APR"] = japr;
                jiopArray.Insert(0, jiop);

                jmdvr["IOP"] = jiopArray;
                jparameter["MDVR"] = jmdvr;

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                jparameter = new JObject();

                DateTime dtEndTime = DateTime.Now;
                DateTime dtStartTime = dtEndTime.AddMinutes(-1);

                int nStreamType = (int)StreamType.MIRROR_STREAM;

                string starttime = dtStartTime.ToString("yyyyMMddHHmmss");
                string endtime = dtEndTime.ToString("yyyyMMddHHmmss");

                jparameter.Add("STREAMTYPE", nStreamType);
                jparameter.Add("FILETYPE", 0xffffffff);
                jparameter.Add("CHANNEL", nChannelBits);
                jparameter.Add("STARTTIME", starttime);
                jparameter.Add("ENDTIME", endtime);

                jresp = session.SendCommand(Module.STORM, Operation.QUERYFILELIST, jparameter);

                Assert.IsNotNull(jresp["FILELIST"]);

                List<RemoteFileInfo> filelist = JsonConvert.DeserializeObject<List<RemoteFileInfo>>(jresp["FILELIST"].ToString());

                for (int i = 0; i < sizeof(UInt32); i++)
                {
                    if (((nChannelBits >> i) & 0x01) == 1)
                    {
                        bool bGet = false;

                        foreach (RemoteFileInfo info in filelist)
                        {
                            if (info.nChannel == i)
                            {
                                bGet = true;
                            }
                        }

                        Assert.IsTrue(bGet, "通道 {0} 没有进行报警录像", i);
                    }
                    else
                    {
                        bool bGet = false;
                        foreach (RemoteFileInfo info in filelist)
                        {
                            if (info.nChannel == i)
                            {
                                bGet = true;
                            }
                        }

                        Assert.IsFalse(bGet, "通道 {0} 不应该进行报警录像", i);
                    }
                }
            }
        }
    }
}
