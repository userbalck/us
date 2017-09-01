using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using N9MTest.SDK.net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Util;

namespace N9MTest.Record
{
	[TestFixture]
	class TestCase_RecordMode : TestCase_Basecase
	{
		[Test]
		public void IgnitionRecord()
		{
			foreach (string ip in IPList)
			{
				Console.WriteLine("ip = {0}", ip);
				N9MSession session = new N9MSession(ip, port);
				Assert.AreEqual(0, session.Login(username, password), "登录设备失败");


				JObject jparameter = new JObject();
				JObject jmdvr = new JObject();
				JObject jrp = new JObject();
				JObject jrcp = new JObject();

				//关闭所有的报警联动
				JArray jiopArray = new JArray();

				for (int i = 0; i < session.GetChannelCount(); i++)
				{
					JObject jiop = new JObject();
					jiop["EN"] = 0;
					jiopArray.Insert(i, jiop);
				}

				jmdvr["IOP"] = jiopArray;
				jparameter["MDVR"] = jmdvr;

				JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);

				jparameter = new JObject();
				jmdvr = new JObject();
				jrp = new JObject();
				jrcp = new JObject();

				jrp["RCP"] = "?";
				jmdvr["RP"] = jrp;
				jparameter["MDVR"] = jmdvr;

				jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);

				jparameter = new JObject();
				jmdvr = new JObject();
				jrp = new JObject();
                jrcp = new JObject();

                JArray array = new JArray();

                jrcp = new JObject();
                jrcp["RM"] = (int)RecordMode.Boot;

                array.Insert(0, jrcp);

                jrcp = new JObject();
                jrcp["RM"] = (int)RecordMode.Alarm;

                array.Insert(1, jrcp);

                jrp["RCP"] = array;
                jmdvr["RP"] = jrp;
                jparameter["MDVR"] = jmdvr;

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);
			}
		}
		[Ignore("1111")]
		[Test]
		public void TimerRecord()
        {
            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");


                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                JObject jrp = new JObject();
                JObject jrcp = new JObject();

                //关闭所有的报警联动
                JArray jiopArray = new JArray();

                for (int i = 0; i < session.GetChannelCount(); i++)
                {
                    JObject jiop = new JObject();
                    jiop["EN"] = 0;
                    jiopArray.Insert(i, jiop);
                }

                jmdvr["IOP"] = jiopArray;
                jparameter["MDVR"] = jmdvr;

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);

                jparameter = new JObject();
                jmdvr = new JObject();
                jrp = new JObject();
                jrcp = new JObject();

                jrp["RCP"] = "?";
                jmdvr["RP"] = jrp;
                jparameter["MDVR"] = jmdvr;

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);

                jparameter = new JObject();
                jmdvr = new JObject();
                jrp = new JObject();
                jrcp = new JObject();

                JArray array = new JArray();

                for (int i = 0; i < session.GetChannelCount(); i++)
                {
                    jrcp = new JObject();
                    jrcp["RM"] = (int)RecordMode.Timing;

                    array.Insert(i, jrcp);
                }

                jrp["RCP"] = array;
                jmdvr["RP"] = jrp;
                jparameter["MDVR"] = jmdvr;

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);
            }
        }
		[Ignore("1111")]
		[Test]
		public void AlarmRecord()
		{
			foreach (string ip in IPList)
			{
				Console.WriteLine("ip ={0}", ip);

				Console.WriteLine("ip = {0}", ip);
				N9MSession session = new N9MSession(ip, port);
				Assert.AreEqual(0, session.Login(username, password), "登录设备失败");
			}
		}

		[Test]
		public void NormalFrameRate()
		{
			foreach (string ip in IPList)
			{
				Console.WriteLine("ip = {0}", ip);
				N9MSession session = new N9MSession(ip, port);
				Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

				JObject jparameter = new JObject();
				JObject jmdvr = new JObject();
				JObject jrp = new JObject();
				JArray jmainArray = new JArray();
				JArray jrcpArray = new JArray();

				//设置1通道开机录像 帧率为1
				do
				{
					JObject main = new JObject();
					main["VEN"] = 1;
					main["FT"] = (int)EncodeFrameType.Normal;
					main["FR"] = 1;

					jmainArray.Insert(0, main);

					JObject rcp = new JObject();
					rcp["RM"] = (int)RecordMode.Boot;
					jrcpArray.Insert(0, rcp);

				} while (false);

				//通道2开机录像，帧率为2
				do
				{
					JObject main = new JObject();
					main["VEN"] = 1;
					main["FT"] = (int)EncodeFrameType.Normal;
					main["FR"] = 2;

					jmainArray.Insert(1, main);

					JObject rcp = new JObject();
					rcp["RM"] = (int)RecordMode.Boot;
					jrcpArray.Insert(1, rcp);

				} while (false);

				// 通道3开机录像，帧率为15
				do
				{
					JObject main = new JObject();
					main["VEN"] = 1;
					main["FT"] = (int)EncodeFrameType.Normal;
					main["FR"] = 15;

					jmainArray.Insert(2, main);

					JObject rcp = new JObject();
					rcp["RM"] = (int)RecordMode.Boot;
					jrcpArray.Insert(2, rcp);

				} while (false);

				//通道4开机录像，帧率为21

				do
				{
					JObject main = new JObject();
					main["VEN"] = 1;
					main["FT"] = (int)EncodeFrameType.Normal;
					main["FR"] = 21;

					jmainArray.Insert(3, main);

					JObject rcp = new JObject();
					rcp["RM"] = (int)RecordMode.Boot;
					jrcpArray.Insert(3, rcp);

				} while (false);

				jmdvr["MAIN"] = jmainArray;
				jrp["RCP"] = jrcpArray;
				jmdvr["RP"] = jrp;
				jparameter["MDVR"] = jmdvr;

				JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

				Assert.IsNotNull(jresp);
				Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));

				Sleep(120 * 1000);

				DateTime dtEndTime = DateTime.Now.AddSeconds(-10);
				DateTime dtStartTime = dtEndTime.AddSeconds(-90);

				string starttime = dtStartTime.ToString("yyyyMMddHHmmss");
				string endtime = dtEndTime.ToString("yyyyMMddHHmmss");

				int nStreamType = (int)StreamType.MAIN_STREAM;


				jparameter = new JObject();
				jparameter.Add("STREAMTYPE", nStreamType);
				jparameter.Add("FILETYPE", 0xffffffff);
				jparameter.Add("CHANNEL", 0xF);
				jparameter.Add("STARTTIME", starttime);
				jparameter.Add("ENDTIME", endtime);

				jresp = session.SendCommand(Module.STORM, Operation.QUERYFILELIST, jparameter);

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

				for (int i = 0; i < 4; i++)
				{
					Assert.IsTrue(timeSection.IsContinuous(i, starttime, endtime));
				}

				foreach (RemoteFileInfo info in list)
				{
					if (info == null)
					{
						continue;
					}

					session.DownloadVideo((StreamType)nStreamType, info.recordID, info.nChannel, info.starttime, info.endtime, "video.data");

					H264FileLoader loader = new H264FileLoader("video.data");
					loader.Parse();
					loader.Dispose();

					int nAverageFrameRate = loader.GetPts().Count / (int)(dtEndTime - dtStartTime).TotalSeconds;

					//0通道 帧率为1
					if (info.nChannel == 0)
					{
						Console.WriteLine("[通道0]：期望帧率是1, 实际帧率是 {0}", nAverageFrameRate);
						Assert.AreEqual(1, nAverageFrameRate, "期望帧率是1, 实际帧率是 {0}", nAverageFrameRate);
					}
					else if (info.nChannel == 1)
					{
						Console.WriteLine("[通道1]：期望帧率是2, 实际帧率是 {0}", nAverageFrameRate);
						Assert.AreEqual(2, nAverageFrameRate, "期望帧率是2, 实际帧率是 {0}", nAverageFrameRate);
					}
					else if (info.nChannel == 2)
					{
						Console.WriteLine("[通道2]:期望帧率是15, 实际帧率是 {0}", nAverageFrameRate);
						Assert.AreEqual(15, nAverageFrameRate, "期望帧率是15, 实际帧率是 {0}", nAverageFrameRate);
					}
					else if (info.nChannel == 3)
					{
						Console.WriteLine("[通道3]:期望帧率是21, 实际帧率是 {0}", nAverageFrameRate);
						Assert.AreEqual(21, nAverageFrameRate, "期望帧率是21, 实际帧率是 {0}", nAverageFrameRate);
					}
				}
			}
		}

		[Test]
		public void IFrameRate()
		{
			foreach (string ip in IPList)
			{
				Console.WriteLine("ip = {0}", ip);
				N9MSession session = new N9MSession(ip, port);
				Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

				JObject jparameter = new JObject();
				JObject jmdvr = new JObject();
				JObject jrp = new JObject();
				JArray jmainArray = new JArray();
				JArray jrcpArray = new JArray();

				//设置1通道开机录像 帧率为1
				do
				{
					JObject main = new JObject();
					main["VEN"] = 1;
					main["FT"] = (int)EncodeFrameType.IFrame;
					main["FR"] = 1;

					jmainArray.Insert(0, main);

					JObject rcp = new JObject();
					rcp["RM"] = (int)RecordMode.Boot;
					jrcpArray.Insert(0, rcp);

				} while (false);

				//通道2开机录像，帧率为2
				do
				{
					JObject main = new JObject();
					main["VEN"] = 1;
					main["FT"] = (int)EncodeFrameType.IFrame;
					main["FR"] = 2;

					jmainArray.Insert(1, main);

					JObject rcp = new JObject();
					rcp["RM"] = (int)RecordMode.Boot;
					jrcpArray.Insert(1, rcp);

				} while (false);

				// 通道3开机录像，帧率为15
				do
				{
					JObject main = new JObject();
					main["VEN"] = 1;
					main["FT"] = (int)EncodeFrameType.IFrame;
					main["FR"] = 15;

					jmainArray.Insert(2, main);

					JObject rcp = new JObject();
					rcp["RM"] = (int)RecordMode.Boot;
					jrcpArray.Insert(2, rcp);

				} while (false);

				//通道4开机录像，帧率为21

				do
				{
					JObject main = new JObject();
					main["VEN"] = 1;
					main["FT"] = (int)EncodeFrameType.IFrame;
					main["FR"] = 21;

					jmainArray.Insert(3, main);

					JObject rcp = new JObject();
					rcp["RM"] = (int)RecordMode.Boot;
					jrcpArray.Insert(3, rcp);

				} while (false);

				jmdvr["MAIN"] = jmainArray;
				jrp["RCP"] = jrcpArray;
				jmdvr["RP"] = jrp;
				jparameter["MDVR"] = jmdvr;

				JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

				Assert.IsNotNull(jresp);
				Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));

				Sleep(120 * 1000);

				DateTime dtEndTime = DateTime.Now.AddSeconds(-10);
				DateTime dtStartTime = dtEndTime.AddSeconds(-90);


				string starttime = dtStartTime.ToString("yyyyMMddHHmmss");
				string endtime = dtEndTime.ToString("yyyyMMddHHmmss");

				int nStreamType = (int)StreamType.MAIN_STREAM;

				jparameter = new JObject();
				jparameter.Add("STREAMTYPE", nStreamType);
				jparameter.Add("FILETYPE", 0xffffffff);
				jparameter.Add("CHANNEL", 0xF);
				jparameter.Add("STARTTIME", starttime);
				jparameter.Add("ENDTIME", endtime);

				jresp = session.SendCommand(Module.STORM, Operation.QUERYFILELIST, jparameter);

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

                for (int i = 0; i < 4; i++)
                {
                    Assert.IsTrue(timeSection.IsContinuous(i, starttime, endtime));
                }

                foreach (RemoteFileInfo info in list)
                {
                    if (info == null)
                    {
                        continue;
                    }

                    session.DownloadVideo((StreamType)nStreamType, info.recordID, info.nChannel, info.starttime, info.endtime, "video.data");

                    H264FileLoader loader = new H264FileLoader("video.data");
					loader.Parse();
					loader.Dispose();


					Console.WriteLine("[通道{0}]：总视频帧数 {1}, 其中I帧帧数 {2}", info.nChannel, loader.GetVideoFrameCount(), loader.GetIFrameCount());
					Assert.AreEqual(loader.GetIFrameCount(), loader.GetVideoFrameCount(), "[通道{0}]：总视频帧数 {1}, 其中I帧帧数 {2}", info.nChannel, loader.GetVideoFrameCount(), loader.GetIFrameCount());

				}
			}
		}
		[Ignore("1111")]
        [Test]
        public void ScheduleRecord()
        {

        }
    }
}
