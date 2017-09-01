using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using N9MTest.SDK.http;
using N9MTest.SDK.Tooling;
using Newtonsoft.Json.Linq;
using N9MTest.SDK.net;
using Newtonsoft.Json;

namespace N9MTest.Alarm
{
    class TestCase_AlarmDelay:TestCase_Basecase
    {
		
		[Test]
        public void DurationAlarmTime()
        {
            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                DeviceTime devTime = session.GetDeviceTime();

                //1.设置IO1报警<打开使能/高电平/报警/联动通道录像>，报警有效时间为10秒
                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                JArray jiopArray = new JArray();

                for (int i = 0; i < 1; i++)
                {
                    JObject jiop = new JObject();
                    jiop["EN"] = 1;
                    jiop["EL"] = 1;
                    jiop["SDT"] = 10;
                    jiopArray.Insert(i, jiop);
                }
                jmdvr["IOP"] = jiopArray;
                jparameter["MDVR"] = jmdvr;


                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);
                session.Logout();
            

                ToolingSession toolingSession = new ToolingSession();
                toolingSession.StartSession(m_ToolingIP, m_ToolingPort);

                do
                {
                    DateTime dtStartTime = devTime.Now;

                    for (int i = 0; i < 3; i++)
                    {
                        toolingSession.SendMessage("$IO01,01*HH");
                        Console.WriteLine("[第{0}次IO报警产生][设备时间为{1}]", i, devTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        Sleep(10 * 1000);
                        toolingSession.SendMessage("$IO01,00*HH");
                        
                    }

                    DateTime dtEndTime = dtStartTime.AddSeconds(35);

                    N9MHttpSession httpSession = new N9MHttpSession(ip, 80);
                    Assert.AreEqual(0, httpSession.Login("/logincheck.rsp?type=1", username, password));
                    string url = string.Format("/device.rsp?opt=getLog&cmd=bypage&page=1&st={0}&et={1}&alarmType=255&logcount=500&logtype=1&mark=0",
                        dtStartTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        dtEndTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    url = url.Replace(" ", "%20");

                    Console.WriteLine("url = {0}", url);
                    string resp = httpSession.Get(url);
                    jresp = JObject.Parse(resp);

                    Assert.IsNotNull(jresp, "日志查询回复为空");
                    Assert.AreEqual(0, Convert.ToInt32(jresp["result"].ToString()), "日志查询结果返回失败");
                    List<LogInfo> list = JsonConvert.DeserializeObject<List<LogInfo>>(jresp["data"].ToString());
                    Assert.AreEqual(1, list.Count, "期望报警日志条数为1，实际报警日志条数为{0}", list.Count);
                } while (false);

                Sleep(15 * 1000);

                do
                {
                    DateTime dtStartTime = devTime.Now;

                    for (int i = 0; i < 3; i++)
                    {
                        toolingSession.SendMessage("$IO01,01*HH");
                        Sleep(1 * 1000);
                        toolingSession.SendMessage("$IO01,00*HH");
                        Sleep(15 * 1000);
                    }

                    DateTime dtEndTime = dtStartTime.AddSeconds(60);

                    N9MHttpSession httpSession = new N9MHttpSession(ip, 80);
                    Assert.AreEqual(0, httpSession.Login("/logincheck.rsp?type=1", username, password));
                    string url = string.Format("/device.rsp?opt=getLog&cmd=bypage&page=1&st={0}&et={1}&alarmType=255&logcount=500&logtype=1&mark=0",
                        dtStartTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        dtEndTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    url = url.Replace(" ", "%20");

                    Console.WriteLine("url = {0}", url);
                    string resp = httpSession.Get(url);
                    jresp = JObject.Parse(resp);

                    Assert.IsNotNull(jresp, "日志查询回复为空");
                    Assert.AreEqual(0, Convert.ToInt32(jresp["result"].ToString()), "日志查询结果返回失败");
                    List<LogInfo> list = JsonConvert.DeserializeObject<List<LogInfo>>(jresp["data"].ToString());
                    Assert.AreEqual(3, list.Count, "期望报警日志条数为1，实际报警日志条数为{0}", list.Count);
                } while (false);
            }
        }

		
		[Test]
        public void PostAlarmRecordTime()
        {
            int nRecordDelay = Convert.ToInt32(this.CFG.gNode("Alarm/PostAlarmRecordTime/record-delay").InnerText);
            foreach (string ip in IPList)
            {
                ToolingSession toolingSession = new ToolingSession();
                toolingSession.StartSession(m_ToolingIP, m_ToolingPort);

                //首先设定为IO低电平
                toolingSession.SendMessage("$IO01,00*HH");

                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");
                DeviceTime devTime = session.GetDeviceTime();

                //设置IO1报警(打开使能/高电平/报警/联动通道录像) 报警有效时间为10秒
                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                JArray jiopArray = new JArray();

                for (int i = 0; i < 8; i++)
                {
                    JObject jiop = new JObject();

                    if (i == 0)
                    {
                        jiop["EN"] = 1;
                        jiop["EL"] = 1;
                        jiop["AS"] = 1;
                        jiop["SDT"] = 10;

                        JObject japr = new JObject();
                        JObject jar = new JObject();
                        jar["CH"] = 0xffffffff;
                        jar["P"] = 0;
                        jar["D"] = nRecordDelay;
                        jar["L"] = 0;

                        japr["AR"] = jar;
                        jiop["APR"] = japr;
                    }
                    else
                    {
                        jiop["EN"] = 0;
                    }

                    jiopArray.Insert(i, jiop);
                }

                jmdvr["IOP"] = jiopArray;
                jparameter["MDVR"] = jmdvr;
                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);
                Assert.IsNotNull(jresp["ERRORCODE"]);
                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));

                DateTime t1 = devTime.Now;
                toolingSession.SendMessage("$IO01,01*HH");
                Sleep(1000);
                toolingSession.SendMessage("$IO01,00*HH");
                Sleep(90 * 1000);

                DateTime dtStartTime = t1;
                DateTime dtEndTime = devTime.Now;

                int nStreamType = (int)StreamType.MAIN_STREAM;
                uint nChannelBits = 0xffffffff;

                string starttime = dtStartTime.ToString("yyyyMMddHHmmss");
                string endtime = dtEndTime.ToString("yyyyMMddHHmmss");

                jparameter = new JObject();
                jparameter["STREAMTYPE"] = nStreamType;
                jparameter["FILETYPE"] = (int)FileType.CALENDAR_FILETYPE_ALARM;
                jparameter["CHANNEL"] = nChannelBits;
                jparameter["STARTTIME"] = starttime;
                jparameter["ENDTIME"] = endtime;

                jresp = session.SendCommand(Module.STORM, Operation.QUERYFILELIST, jparameter);

                List<RemoteFileInfo> list = JsonConvert.DeserializeObject<List<RemoteFileInfo>>(jresp["FILELIST"].ToString());
                Assert.IsNotNull(jresp["FILELIST"]);
                Assert.IsTrue(jresp["FILELIST"].HasValues, "获取到的文件列表为空");

                foreach (RemoteFileInfo info in list)
                {
                    Console.WriteLine("[通道 {0}]报警录像文件时间 {1}", info.nChannel,info.GetTimeSpan().TotalSeconds);
                    Assert.IsTrue(Math.Abs(info.GetTimeSpan().TotalSeconds - nRecordDelay - 10) <= 2, "报警录像文件的时间不约等于{0}秒 实际值为{1}", nRecordDelay + 10, info.GetTimeSpan().TotalSeconds);
                }

            }
        }

    }
}
