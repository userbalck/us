using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using N9MTest.SDK.net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace N9MTest.Playback
{
    [TestFixture]
    class TestCase_PlaybackBackup:TestCase_Basecase
    {
        [Test]
        public void PlaybackBackup()
        {
            int nStreamType = (int)StreamType.MAIN_STREAM;
            uint nChannelBits = 0xffffff;
            string starttime = "20170405000000";
            string endtime = "20170407120000";

            nStreamType = Convert.ToInt32(this.CFG.gNode("Playback/PlaybackBackup/streamtype").InnerText);
            nChannelBits = Convert.ToUInt32(this.CFG.gNode("Playback/PlaybackBackup/channel").InnerText);
            starttime = this.CFG.gNode("Playback/PlaybackBackup/starttime").InnerText;
            endtime = this.CFG.gNode("Playback/PlaybackBackup/endtime").InnerText;

            Console.WriteLine("nStreamType ={0}", nStreamType);
            Console.WriteLine("nChannelBits ={0}", nChannelBits);
            Console.WriteLine("starttime ={0}", starttime);
            Console.WriteLine("endtime ={0}", endtime);

            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);

                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password));

                JObject jparameter = new JObject();
                jparameter.Add("STREAMTYPE", nStreamType);
                jparameter.Add("FILETYPE", 0xffffffff);
                jparameter.Add("CHANNEL", nChannelBits);
                jparameter.Add("STARTTIME", starttime);
                jparameter.Add("ENDTIME", endtime);

                JObject jresp = session.SendCommand(Module.STORM, Operation.QUERYFILELIST, jparameter);

                Assert.IsNotNull(jresp["FILELIST"]);

                List<RemoteFileInfo> list = JsonConvert.DeserializeObject<List<RemoteFileInfo>>(jresp["FILELIST"].ToString());

                foreach (RemoteFileInfo info in list)
                {
                    //首先获取段内文件的大小
                    jparameter = new JObject();
                    jparameter["DATATYPE"] = 0;
                    jparameter["STREAMTYPE"] = nStreamType;
                    jparameter["RECORDTYPE"] = 3;
                    jparameter["STIME"] = starttime;
                    jparameter["ETIME"] = endtime;
                    jparameter["CHANNEL"] = 1<<info.nChannel;

                    jresp = session.SendCommand(Module.STORM, Operation.GETFILESIZEBYTIME, jparameter);
                    Assert.IsNotNull(jresp);
                    Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));

                    int nTotalSize = Convert.ToInt32(jresp["TOTALSIZE"].ToString());

                    session.DownloadVideo(StreamType.MAIN_STREAM, info.recordID, info.nChannel, info.starttime, info.endtime, "video.data");

                    System.IO.FileInfo fileInfo = new System.IO.FileInfo("video.data");

                    double rate = fileInfo.Length * 1.0f / nTotalSize;

                    Assert.IsTrue(rate >= 0.9);
                }
            }
        }
		
        [Test]
        public void BackH264File()
        {
            int nStreamType = (int)StreamType.MAIN_STREAM;
            uint nChannelBits = 0xffffff;
            string starttime = "";
            string endtime = "";
            int framerate = 0;

            nStreamType = Convert.ToInt32(this.CFG.gNode("Playback/BackH264File/streamtype").InnerText);
            nChannelBits = Convert.ToUInt32(this.CFG.gNode("Playback/BackH264File/channel").InnerText);

            bool bAuto = false;

            if (this.CFG.gNode("Playback/BackH264File/starttime") != null)
            {
                starttime = this.CFG.gNode("Playback/BackH264File/starttime").InnerText;
            }
            else
            {
                bAuto = true;
            }

            if (this.CFG.gNode("Playback/BackH264File/endtime") != null)
            {
                endtime = this.CFG.gNode("Playback/BackH264File/endtime").InnerText;
            }
            else
            {
                bAuto = true;
            }

            if (this.CFG.gNode("Playback/BackH264File/framerate") != null)
            {
                framerate = Convert.ToInt32(this.CFG.gNode("Playback/BackH264File/framerate").InnerText);
            }
            else
            {
                bAuto = true;
            }


            DateTime dtStartTime;
            DateTime dtEndTime;

            if (starttime == null || starttime.Length == 0)
            {
                dtStartTime = DateTime.Now.AddSeconds(-90);
                starttime = dtStartTime.ToString("yyyyMMddHHmmss");
            }
            else
            {
                dtStartTime = DateTime.ParseExact(starttime, "yyyyMMddHHmmss", null);
            }

            if (endtime == null || endtime.Length == 0)
            {
                dtEndTime = dtStartTime.AddSeconds(60);
                endtime = dtEndTime.ToString("yyyyMMddHHmmss");
            }
            else
            {
                dtEndTime = DateTime.ParseExact(endtime, "yyyyMMddHHmmss", null);
            }

            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                //首先获取当前的录像帧率

                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();

                jmdvr["MAIN"] = "?";
                jparameter["MDVR"] = jmdvr;
                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);
                Assert.IsNotNull(jresp);
                Assert.IsNotNull(jresp["MDVR"]["MAIN"]);
                List<NetStreamEncoderInfo> netStreamEncoderInfoList = JsonConvert.DeserializeObject<List<NetStreamEncoderInfo>>(jresp["MDVR"]["MAIN"].ToString());


                Console.WriteLine("nStreamType ={0}", nStreamType);
                Console.WriteLine("nChannelBits ={0}", nChannelBits);
                Console.WriteLine("starttime ={0}", starttime);
                Console.WriteLine("endtime ={0}", endtime);

                jparameter = new JObject();
                jparameter["STREAMTYPE"] = nStreamType;
                jparameter["FILETYPE"] = 0xffffffff;
                jparameter["CHANNEL"] = nChannelBits;
                jparameter["STARTTIME"] = starttime;
                jparameter["ENDTIME"] = endtime;

                jresp = session.SendCommand(Module.STORM, Operation.QUERYFILELIST, jparameter);

                List<RemoteFileInfo> list = JsonConvert.DeserializeObject<List<RemoteFileInfo>>(jresp["FILELIST"].ToString());
                Assert.IsNotNull(jresp["FILELIST"]);

                foreach (RemoteFileInfo info in list)
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

                    DateTime dtFileStartTime;
                    DateTime dtFileEndTime;

                    loader.GetTimeDuration(out dtFileStartTime, out dtFileEndTime);

                    loader.Dispose();

                    Assert.GreaterOrEqual((dtFileEndTime - dtFileStartTime).TotalSeconds, (dtEndTime - dtStartTime).TotalSeconds, 
                        "录像时间不够完整 实际请求间隔{0}-{1} 下载文件间隔{2}-{3}", 
                        starttime, 
                        endtime , 
                        dtFileEndTime.ToString("yyyyMMddHHmmss"),
                        dtFileEndTime.ToString("yyyyMMddHHmmss"));

                    int nAverageFrameRate = loader.GetPts().Count() / ((int)(dtEndTime - dtStartTime).TotalSeconds);

                    Console.WriteLine("期望帧率 {0}, 实际帧率 {1}",
                        netStreamEncoderInfoList[info.nChannel].FrameRate, nAverageFrameRate);

                    Assert.IsTrue(Math.Abs(nAverageFrameRate - netStreamEncoderInfoList[info.nChannel].FrameRate) <= 2, "期望帧率 {0}, 实际帧率 {1}",
                        netStreamEncoderInfoList[info.nChannel].FrameRate, nAverageFrameRate);
                }
            }
        }
		[Ignore("11111")]
        [Test]
        public void BackAVIFile()
        {
            string starttime = "20170516093000";
            string endtime = "20170516100000";

            int nStreamType = (int)StreamType.MAIN_STREAM;
            uint nChannelBits = 15;

            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                session.Login(username, password);

                Console.WriteLine("nStreamType ={0}", nStreamType);
                Console.WriteLine("nChannelBits ={0}", nChannelBits);
                Console.WriteLine("starttime ={0}", starttime);
                Console.WriteLine("endtime ={0}", endtime);
                JObject jparameter = new JObject();
                jparameter.Add("STREAMTYPE", nStreamType);
                jparameter.Add("FILETYPE", 0xffffffff);
                jparameter.Add("CHANNEL", nChannelBits);
                jparameter.Add("STARTTIME", starttime);
                jparameter.Add("ENDTIME", endtime);

                JObject jresp = session.SendCommand(Module.STORM, Operation.QUERYFILELIST, jparameter);

                List<RemoteFileInfo> list = JsonConvert.DeserializeObject<List<RemoteFileInfo>>(jresp["FILELIST"].ToString());


                foreach (RemoteFileInfo info in list)
                {
                    Assert.AreEqual(starttime, info.szTime.Split('-')[0]);
                    Assert.AreEqual(endtime, info.szTime.Split('-')[1]);

                    session.DownloadVideo(StreamType.MAIN_STREAM, info.recordID, info.nChannel, starttime, endtime, "video.data");

                    break;
                }
            }
        }
    }
}
