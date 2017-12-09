using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using N9MTest.SDK.net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace N9MTest.Record
{

    [TestFixture]
    class TestCase_DualStream: TestCase_Basecase
    {
        /// <summary>
        /// 单码流录像
        /// </summary>
        [Test]
        public void SingleStreamRecord()
        {
            int nDuration = Convert.ToInt32(this.CFG.gNode("Record/SingleStreamRecord/duration").InnerText);
            int nChannelBits = Convert.ToInt32(this.CFG.gNode("Record/SingleStreamRecord/channelbits").InnerText);

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
					if (((nChannelBits >> i) & 0x01) == 1)
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

                for (int i = 0; i < 32; i++)
				{
                    JObject jvec = new JObject();
                    jvec["VEN"] = 0;
                    jvecArray.Insert(i, jvec);
                }

                jar["VEC"] = jvecArray;

                jmdvr["AR"] = jar;

                //设定开机录像
                JObject jrp = new JObject();
                JObject jmp = new JObject();

                jparameter["MDVR"] = jmdvr;

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Sleep(10 * 1000);

                for (int i = 0; i < sizeof(UInt32); i++)
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

                Sleep(30 * 1000);

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
		/// 双码流录像
		/// </summary>
		
		[Test]
		public void DoubleStreamRecord()
		{
			int nDuration = Convert.ToInt32(this.CFG.gNode("Record/DoubleStreamRecord/duration").InnerText);
			int nChannelBits = Convert.ToInt32(this.CFG.gNode("Record/DoubleStreamRecord/channelbits").InnerText);

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

				//设定开启主码流录像 开启辅助码流（子码流）录像

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

                //开启辅助码流录像使能
                jar["EN"] = 1;

                //设置辅助码流为子码流录像
                jar["RM"] = (int)AssistRecordMode.Sub;

                //设置子码流录像的通道
                jar["SSC"] = nChannelBits;

                //设置为内置存储卡录像
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

                //设定开机录像
				JObject jrp = new JObject();
				JObject jmp = new JObject();

				jparameter["MDVR"] = jmdvr;

				jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Sleep(30 * 1000);

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

				Sleep(30 * 1000);

				string starttime = dtStartTime.ToString("yyyyMMddHHmmss");
				string endtime = dtEndTime.ToString("yyyyMMddHHmmss");

				//下载主码流录像
				do
				{
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

                        ulong lStartPts = 0;
                        ulong lEndPts = 0;

                        loader.GetTimeSpan(out lStartPts, out lEndPts);

                        Console.WriteLine("lStartPts = {0}, lEndPts = {1}", lStartPts, lEndPts);

                        Assert.GreaterOrEqual((lEndPts - lStartPts)/1000, 60, "录像文件时长小于60秒");
                    }
                } while (false);


				//下载子码流录像
				do
				{
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

                        ulong lStartPts = 0;
                        ulong lEndPts = 0;

                        loader.GetTimeSpan(out lStartPts, out lEndPts);

                        Console.WriteLine("lStartPts = {0}, lEndPts = {1}", lStartPts, lEndPts);

                        Assert.GreaterOrEqual((lEndPts - lStartPts) / 1000, 60, "录像文件时长小于60秒");
                    }
                } while (false);
			}
		}

		/// <summary>
		/// 自动调整录像时间
		/// </summary>
		/// 
		[Ignore("未完成，测试失败")]
		[Test]
		public void AutoAdjustRecordStreamTime()
		{
            uint nChannelBits = 0xffffffff;
			foreach (string ip in IPList)
			{
				Console.WriteLine("ip = {0}", ip);
				N9MSession session = new N9MSession(ip, port);
				Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

				JObject jparameter = new JObject();
				JObject jmdvr = new JObject();

				//首先获取主码流的参数
				jparameter = new JObject();
				jmdvr = new JObject();
				jmdvr.Add("AR", "?");
				jmdvr.Add("MAIN", "?");
				jparameter.Add("MDVR", jmdvr);

				JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);
                Assert.IsNotNull(jresp["MDVR"]);
                Assert.IsNotNull(jresp["MDVR"]["MAIN"]);
                Assert.IsNotNull(jresp["MDVR"]["AR"]);

                //设定开启主码流录像 开启辅助码流（子码流）录像

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

				//开启辅助码流录像使能
				jar["EN"] = 1;

				//设置辅助码流为子码流录像
				jar["RM"] = (int)AssistRecordMode.Sub;

                //设置子码流录像的通道
                jar["SSC"] = nChannelBits;

                //设置为内置存储卡录像
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

                //设定开机录像
                JObject jrp = new JObject();
                JObject jmp = new JObject();

                jparameter["MDVR"] = jmdvr;

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Sleep(30 * 1000);

                for (int i = 0; i < sizeof(UInt32); i++)
                {
                    if (((nChannelBits >> i) & 0x01) == 1)
                    {
                        Assert.IsTrue(session.isChannelRecording(StreamType.MAIN_STREAM, i), "主码流{0}通道没有正常录像", i);
                        Assert.IsTrue(session.isChannelRecording(StreamType.SUB_STREAM, i), "子码流{0}通道没有正常录像", i);
                        Assert.IsFalse(session.isChannelRecording(StreamType.MIRROR_STREAM, i), "镜像码流{0}通道没有停止录像", i);
                    }
                }

                //获取主码流所有时长
                TimeSpan MainTimeSpan = new TimeSpan(0);
                do
                {
                    int nStreamType = (int)StreamType.MAIN_STREAM;

                    //获取下所有月历
                    jparameter = new JObject();
                    jparameter["CALENDARTYPE"] = 2;
                    jparameter["STREAMTYPE"] = nStreamType;
                    jparameter["FILETYPE"] = 0xffffffff;
                    jparameter["CHANNEL"] = 0xffffffff;

                    jresp = session.SendCommand(Module.STORM, Operation.GETCALENDAR, jparameter);
                    Assert.IsNotNull(jresp);
                    Assert.IsNotNull(jresp["LIST"]);

                    List<CalendarData> callist = JsonConvert.DeserializeObject<List<CalendarData>>(jresp["LIST"].ToString());

                    callist.Sort(CalendarData.Sort);

                    foreach (CalendarData calendar in callist)
                    {
                        string starttime = new DateTime(calendar.nYear, calendar.nMonth, calendar.nDay, 0, 0, 0).ToString("yyyyMMddHHmmss");
                        string endtime = new DateTime(calendar.nYear, calendar.nMonth, calendar.nDay, 23, 59, 59).ToString("yyyyMMddHHmmss");

                        jparameter = new JObject();
                        jparameter.Add("STREAMTYPE", nStreamType);
                        jparameter.Add("FILETYPE", 0xffffffff);
                        jparameter.Add("CHANNEL", nChannelBits);
                        jparameter.Add("STARTTIME", starttime);
                        jparameter.Add("ENDTIME", endtime);

                        jresp = session.SendCommand(Module.STORM, Operation.QUERYFILELIST, jparameter);

                        Assert.IsNotNull(jresp);
                        Assert.IsNotNull(jresp["FILELIST"]);

                        List<RemoteFileInfo> RemoteFileList = JsonConvert.DeserializeObject<List<RemoteFileInfo>>(jresp["FILELIST"].ToString());

                        foreach (RemoteFileInfo info in RemoteFileList)
                        {
                            MainTimeSpan += info.GetTimeSpan();
                        }

                        Console.WriteLine("主码流 Total hours:{0}", MainTimeSpan.TotalHours);

                    }
                } while (false);

                //获取子码流所有时长
                TimeSpan SubTimeSpan = new TimeSpan(0);
                do
                {
                    int nStreamType = (int)StreamType.SUB_STREAM;

                    //获取下所有月历
                    jparameter = new JObject();
                    jparameter["CALENDARTYPE"] = 2;
                    jparameter["STREAMTYPE"] = nStreamType;
                    jparameter["FILETYPE"] = 0xffffffff;
                    jparameter["CHANNEL"] = 0xffffffff;

                    jresp = session.SendCommand(Module.STORM, Operation.GETCALENDAR, jparameter);
                    Assert.IsNotNull(jresp);
                    Assert.IsNotNull(jresp["LIST"]);

                    List<CalendarData> callist = JsonConvert.DeserializeObject<List<CalendarData>>(jresp["LIST"].ToString());

                    callist.Sort(CalendarData.Sort);

                    foreach (CalendarData calendar in callist)
                    {
                        string starttime = new DateTime(calendar.nYear, calendar.nMonth, calendar.nDay, 0, 0, 0).ToString("yyyyMMddHHmmss");
                        string endtime = new DateTime(calendar.nYear, calendar.nMonth, calendar.nDay, 23, 59, 59).ToString("yyyyMMddHHmmss");

                        jparameter = new JObject();
                        jparameter.Add("STREAMTYPE", nStreamType);
                        jparameter.Add("FILETYPE", 0xffffffff);
                        jparameter.Add("CHANNEL", nChannelBits);
                        jparameter.Add("STARTTIME", starttime);
                        jparameter.Add("ENDTIME", endtime);

                        jresp = session.SendCommand(Module.STORM, Operation.QUERYFILELIST, jparameter);

                        Assert.IsNotNull(jresp);
                        Assert.IsNotNull(jresp["FILELIST"]);

                        List<RemoteFileInfo> RemoteFileList = JsonConvert.DeserializeObject<List<RemoteFileInfo>>(jresp["FILELIST"].ToString());

                        foreach (RemoteFileInfo info in RemoteFileList)
                        {
                            SubTimeSpan += info.GetTimeSpan();
                        }

                        Console.WriteLine("子码流 Total hours:{0}", SubTimeSpan.TotalHours);
                    }
                } while (false);

                Assert.AreEqual(MainTimeSpan.TotalHours + 1, SubTimeSpan.TotalHours, "录像时长不对 主码流录像时长:{0} 子码流录像时长 {1}", MainTimeSpan.TotalHours, SubTimeSpan.TotalHours);
            }
        }
		[Ignore("1111")]
        /// <summary>
        /// 配置的双码流时间
        /// </summary>
        [Test]
        public void ManualAdjustRecordStreamTime()
        {
            uint nChannelBits = 0xffffffff;
            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();

                //首先获取主码流的参数
                jparameter = new JObject();
                jmdvr = new JObject();
                jmdvr.Add("AR", "?");
                jmdvr.Add("MAIN", "?");
                jparameter.Add("MDVR", jmdvr);

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);
                Assert.IsNotNull(jresp["MDVR"]);
                Assert.IsNotNull(jresp["MDVR"]["MAIN"]);
                Assert.IsNotNull(jresp["MDVR"]["AR"]);

                //设定开启主码流录像 开启辅助码流（子码流）录像

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

                //开启辅助码流录像使能
                jar["EN"] = 1;

                //设置辅助码流为子码流录像
                jar["RM"] = (int)AssistRecordMode.Sub;

                //设置子码流录像的通道
                jar["SSC"] = nChannelBits;

                //设置为内置存储卡录像
                jar["HID"] = 0;

                //设定按照录像时长 应该失败才对
                jar["ALH"] = 0;

                JArray jvecArray = new JArray();

                for (int i = 0; i < sizeof(UInt32); i++)
                {
                    JObject jvec = new JObject();
                    jvec["VEN"] = 1;
                    jvecArray.Insert(i, jvec);
                }

                jar["VEC"] = jvecArray;

				jmdvr["AR"] = jar;

				//设定开机录像
				JObject jrp = new JObject();
				JObject jmp = new JObject();

				jparameter["MDVR"] = jmdvr;

				jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

				Assert.IsNotNull(jresp);
				Assert.IsNotNull(jresp["ERRORCODE"]);
				Assert.AreNotEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));

				//设定开启主码流录像 开启辅助码流（子码流）录像

				jparameter = new JObject();
				jmdvr = new JObject();

				jmainArray = new JArray();

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

				//开启辅助码流录像使能
				jar["EN"] = 1;

				//设置辅助码流为子码流录像
				jar["RM"] = (int)AssistRecordMode.Sub;

				//设置子码流录像的通道
				jar["SSC"] = nChannelBits;

				//设置为内置存储卡录像
				jar["HID"] = 0;

				//设定按照录像时长 应该成功才对
				jar["ALH"] = 1;

				jvecArray = new JArray();

				for (int i = 0; i < sizeof(UInt32); i++)
				{
					JObject jvec = new JObject();
					jvec["VEN"] = 1;
					jvecArray.Insert(i, jvec);
				}

				jar["VEC"] = jvecArray;

				jmdvr["AR"] = jar;

				//设定开机录像
				jrp = new JObject();
				jmp = new JObject();

				jparameter["MDVR"] = jmdvr;

				jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

				Assert.IsNotNull(jresp);
				Assert.IsNotNull(jresp["ERRORCODE"]);
				Assert.AreNotEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));
			}
		}
	}
}
