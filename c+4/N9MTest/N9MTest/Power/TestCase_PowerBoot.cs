using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RM;
using N9MTest.commons;
using N9MTest.SDK.net;
using Newtonsoft.Json.Linq;
using N9MTest.SDK.http;
using System.Threading;
using Newtonsoft.Json;
using Util;
using N9MTest.SDK.Tooling;
using System.Net.NetworkInformation;

namespace N9MTest.Power
{
    [TestFixture]
    class TestCase_PowerBoot: TestCase_Basecase
    {
        [Test]
        public void TimeOff()
        {
            foreach (string ip in IPList)
            {
                Console.WriteLine("ip ={0}", ip);

                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                //首先开启自动校时
                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();

                JObject jatp = new JObject();
                jatp.Add("GE", 1);
                jatp.Add("NE", 1);
                jatp.Add("CE", 1);
                jmdvr.Add("ATP", jatp);

                jparameter.Add("MDVR", jmdvr);

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"]), "开启所有校时机制[gps. ntp. center server].错误码为{0}", Convert.ToInt32(jresp["ERRORCODE"]));

                jparameter = new JObject();
                jmdvr = new JObject();
                JObject jtimep = new JObject();
                jtimep.Add("TIMEZ", "?");
                jmdvr.Add("TIMEP", jtimep);
                jparameter.Add("MDVR", jmdvr);

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);

                Assert.IsNotNull(jresp["MDVR"]["TIMEP"]["TIMEZ"], "获取设备时区失败");

                string timez = jresp["MDVR"]["TIMEP"]["TIMEZ"].ToString();

                int nTimez = Convert.ToInt32(timez.Substring(0, timez.Length - 1));


                Console.WriteLine("jresp = {0}", jresp.ToString());


                DateTime time = DateTime.UtcNow.AddMinutes(nTimez);

                DateTime starttime = time.AddMinutes(2);
                DateTime endtime = starttime.AddMinutes(120);

                Console.WriteLine("shutdowntime = {0}", endtime.ToString());
                Console.WriteLine("boottime = {0}", endtime.ToString());

                jparameter = new JObject();
                jmdvr = new JObject();
                JObject jssp = new JObject();

                jssp.Add("UPT", (int)BootupType.Timing);
                jssp.Add("UH", starttime.Hour);
                jssp.Add("UM", starttime.Minute);
                jssp.Add("US", starttime.Second);

                jssp.Add("DH", endtime.Hour);
                jssp.Add("DM", endtime.Minute);
                jssp.Add("DS", endtime.Second);

                jssp.Add("DR", 0);

                jssp.Add("DDS", 0);
                jmdvr.Add("SSP", jssp);
                jparameter.Add("MDVR", jmdvr);

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                session.Logout();

                Thread.Sleep(180 * 1000);

                N9MHttpSession httpSession = new N9MHttpSession(ip, 80);
                Assert.AreEqual(0, httpSession.Login("/logincheck.rsp?type=1", username, password));
                string url = string.Format("/device.rsp?opt=getLog&cmd=bypage&page=1&st={0}&et={1}&alarmType=0&logcount=500&logtype=2&mark=0",
                    time.ToString("yyyy-MM-dd HH:mm:ss"),
                    starttime.ToString("yyyy-MM-dd HH:mm:ss"));
                url = url.Replace(" ", "%20");

                Console.WriteLine("url = {0}", url);

                string resp = httpSession.Get(url);

                jresp = JObject.Parse(resp);

                Assert.IsNotNull(jresp, "日志查询回复为空");
                Assert.AreEqual(0, Convert.ToInt32(jresp["result"].ToString()),"日志查询结果返回失败");
                List<LogInfo> list = JsonConvert.DeserializeObject<List<LogInfo>>(jresp["data"].ToString());

                DateTime[] dt = null;

                foreach (LogInfo info in list)
                {
                    TimeSpan ts = info.GetStartTime() - time;

                    if (info.logContent.Equals("定时关机") && ts.TotalSeconds < 120)
                    {
                        dt = new DateTime[1];
                        dt[0] = new DateTime();
                        dt[0] = info.GetStartTime();
                        break;
                    }
                }

                Assert.AreNotEqual(null, dt);

                int nStreamType = (int)StreamType.MAIN_STREAM;
                uint nChannelBits = 0xffffff;
                string st = dt[0].AddMinutes(-1).ToString("yyyyMMddHHmmss");
                string et = dt[0].AddSeconds(5).ToString("yyyyMMddHHmmss");

                session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                jparameter = new JObject();
                jparameter.Add("STREAMTYPE", nStreamType);
                jparameter.Add("FILETYPE", 0xffffffff);
                jparameter.Add("CHANNEL", nChannelBits);
                jparameter.Add("STARTTIME", st);
                jparameter.Add("ENDTIME", et);

                jresp = session.SendCommand(Module.STORM, Operation.QUERYFILELIST, jparameter);

                Assert.IsNotNull(jresp["FILELIST"]);

                List<RemoteFileInfo> filelist = JsonConvert.DeserializeObject<List<RemoteFileInfo>>(jresp["FILELIST"].ToString());


                //测试完毕以后 把开关机模式改为点火模式
                jparameter = new JObject();
                jmdvr = new JObject();
                jssp = new JObject();

                jssp.Add("UPT", (int)BootupType.ACC);
                jmdvr.Add("SSP", jssp);
                jparameter.Add("MDVR", jmdvr);

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);
            }
        }

        [Test]
        public void PowerTime()
        {
            foreach (string ip in IPList)
            {
                Console.WriteLine("ip ={0}", ip);

                //下发重启指令
                N9MSession session = new N9MSession(ip, port);
                do
                {
                    Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                    JObject jparameter = new JObject();
                    jparameter["CMDTYPE"] = 0;

                    JObject jresp = session.SendCommand(Module.DEVEMM, Operation.SETCONTROLDEVCMD, jparameter);
                    //    Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));
                    session.Logout();
                } while (false);

                Sleep(40 * 1000);

                bool isSuccess = false;

                session = new N9MSession(ip, port);

                for (int i = 0; i < 10; i++)
                {
                    if (isSuccess)
                    {
                        break;
                    }

                    do
                    {
                        if (!session.isConnected())
                        {
                            if (session.Login(username, password) != 0)
                            {
                                break;
                            }
                        }
                        

                        if (session.isChannelRecording(StreamType.MAIN_STREAM, 0)
                            && session.isChannelRecording(StreamType.MAIN_STREAM, 1)
                            && session.isCMSConnected())
                        {
                            isSuccess = true;
                            break;
                        }
                    } while (false);
                    Sleep(5000);
                }

                Assert.IsTrue(isSuccess, "获取各个时间超时");
            }
        }

        [Test]
        public void IgnitonOff()
        {
            //点火关机延时的秒数
            int nDelayTime = Convert.ToInt32(this.CFG.gNode("Power/IgnitonOff/delaytime").InnerText);
            Console.WriteLine("nDelayTime = {0}", nDelayTime);

            foreach (string ip in IPList)
            {
                Console.WriteLine("ip ={0}", ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");
                DeviceTime devTime = session.GetDeviceTime();

                //首先设置点火类型为点火
                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                JObject jssp = new JObject();

                jssp["UPT"] = (int)BootupType.ACC;
                jssp["DDS"] = nDelayTime;

                jmdvr["SSP"] = jssp;
                jparameter["MDVR"] = jmdvr;

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Assert.IsNotNull(jresp);
                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));
                session.Logout();

                //记录当前的时间点T1
                DateTime t1 = devTime.Now;

                //关闭当前的ACC信号
                ToolingSession toolingSession = new ToolingSession();
                toolingSession.StartSession(m_ToolingIP, m_ToolingPort);
                toolingSession.SendMessage("$ACC,00*HH");

                //一直获取设备的时间 当获取失败的时候，记录时间为T2
                DateTime t2 = devTime.Now;

                Ping ping = new Ping();

                do
                {
                    PingReply pingReply = ping.Send(ip);
                    Console.WriteLine("[ping命令检测]关机延时 {0}， 现在过去时间 {1}", nDelayTime, (devTime.Now - t1).TotalSeconds);

                    if (pingReply.Status == IPStatus.TimedOut)
                    {
                        t2 = devTime.Now;
                        break;
                    }
                    else if (pingReply.Status == IPStatus.Success)
                    {
                        Thread.Sleep(1000);
                    }

                    if ((devTime.Now - t1).TotalSeconds >= nDelayTime + 30)
                    {
                        Console.WriteLine("关机等待时间太久 预计等待 {0} 实际等待 {1}", nDelayTime, (devTime.Now - t1).TotalSeconds);
                        Assert.IsTrue(Math.Abs((devTime.Now - t1).TotalSeconds - nDelayTime) < 30, "关机等待时间太久 预计等待 {0} 实际等待 {1}", nDelayTime, (devTime.Now - t1).TotalSeconds);
                    }

                } while (true);

                Console.WriteLine("[ping  指令检测]期望关机延时{0}, 实际关机延时{1}  实际关机时间t2 ={2}", nDelayTime, (t2 - t1).TotalSeconds, t2.ToString("yyyy-MM-dd HH:mm:ss"));

                Assert.IsTrue(Math.Abs((t2 - t1).TotalSeconds -  nDelayTime) < 30, "期望关机延时{0}, 实际关机延时{1}", nDelayTime, (t2 - t1).TotalSeconds);

                Sleep(60 * 1000);
                toolingSession.SendMessage("$ACC,01*HH");
                Sleep(90 * 1000);

                N9MHttpSession httpSession = new N9MHttpSession(ip, webport);

                Assert.AreEqual(0, httpSession.Login("/logincheck.rsp?type=1", username, password));
                string url = string.Format("/device.rsp?opt=getLog&cmd=bypage&page=1&st={0}&et={1}&alarmType=0&logcount=500&logtype=2&mark=0",
                    t1.ToString("yyyy-MM-dd HH:mm:ss"),
                    t2.ToString("yyyy-MM-dd HH:mm:ss"));
                url = url.Replace(" ", "%20");

                Console.WriteLine("url = {0}", url);
                string resp = httpSession.Get(url);
                jresp = JObject.Parse(resp);
                List<LogInfo> list = JsonConvert.DeserializeObject<List<LogInfo>>(jresp["data"].ToString());

                foreach (LogInfo info in list)
                {
                    if (info.logContent.Equals("车钥匙关机") || info.logContent.Equals("ACC off"))
                    {
                        t2 = info.GetStartTime();
                        Console.WriteLine("从日志中获取到的关机时间 t2 ={0}", t2.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                }


                //获取关机前的录像
                session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                int nStreamType = (int)StreamType.MAIN_STREAM;

                jparameter["STREAMTYPE"] = nStreamType;
                jparameter["FILETYPE"] = 0xffffffff;
                jparameter["CHANNEL"] = 0xffffffff;
                jparameter["STARTTIME"] = t1.ToString("yyyyMMddHHmmss");
                jparameter["ENDTIME"] = t2.ToString("yyyyMMddHHmmss");

                jresp = session.SendCommand(Module.STORM, Operation.QUERYFILELIST, jparameter);
                List<RemoteFileInfo> remoteFileInfoList = JsonConvert.DeserializeObject<List<RemoteFileInfo>>(jresp["FILELIST"].ToString());

                TimeSection section = new TimeSection();

                foreach (RemoteFileInfo info in remoteFileInfoList)
                {
                    section.merage(info.nChannel, info.starttime, info.endtime);
                }

                int nChannelCount = session.GetChannelCount();

                for (int i = 0; i < nChannelCount; i++)
                {
                    DateTime dtStartTime;
                    DateTime dtEndTime;
                    section.GetTimeSpan(i, out dtStartTime, out dtEndTime);

                    Console.WriteLine("[通道 {0}]dtStartTime = {1}, dtEndTime = {2}", i, dtStartTime, dtEndTime);

                    Console.WriteLine("日志记录关机时间 {0}，关机前最后录像时间{1} 误差为{2}",
                        t2.ToString("yyyy-MM-dd HH:mm:ss"),
                        dtEndTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        (dtEndTime - t2).TotalSeconds);

                    Assert.GreaterOrEqual(DateTime.ParseExact(dtEndTime.ToString("yyyyMMddHHmmss"), "yyyyMMddHHmmss", null),
                        DateTime.ParseExact(t2.ToString("yyyyMMddHHmmss"), "yyyyMMddHHmmss", null), 
                        "日志记录关机时间 {0}，关机前最后录像时间{1} 误差为{2}",
                        t2.ToString("yyyy-MM-dd HH:mm:ss"), 
                        dtEndTime.ToString("yyyy-MM-dd HH:mm:ss"), 
                        (dtEndTime - t2).TotalSeconds);
                }
            }
        }

        [Test]
        public void IgnitonOrTimerOff()
        {
            string starttime = "";
            string endtime = "";

            int nDelayTime = Convert.ToInt32(this.CFG.gNode("Power/IgnitonOrTimerOff/delaytime").InnerText);
            

            DateTime dtStartTime;
            DateTime dtEndTime;

            foreach (string ip in IPList)
            {
                Console.WriteLine("ip ={0}", ip);

                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");
                DeviceTime devTime = session.GetDeviceTime();

                dtStartTime = devTime.Now.AddMinutes(10);
                dtEndTime = dtStartTime.AddMinutes(10);

                //设置开关机类型为点火或定时，延时8分钟
                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                JObject jssp = new JObject();
                jssp["UPT"] = (int)BootupType.ACCOrTiming;
                jssp["DDS"] = nDelayTime;

                jssp["UH"] = dtStartTime.Hour;
                jssp["UM"] = dtStartTime.Minute;
                jssp["US"] = dtEndTime.Second;
                jssp["DH"] = dtEndTime.Hour;
                jssp["DM"] = dtEndTime.Minute;
                jssp["DS"] = dtEndTime.Second;

                jmdvr["SSP"] = jssp;
                jparameter["MDVR"] = jmdvr;

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);
                session.Logout();

                //记录当前的时间点T1
                DateTime t1 = devTime.Now;

                //关闭点火信号
                ToolingSession toolingSession = new ToolingSession();
                toolingSession.StartSession(m_ToolingIP, m_ToolingPort);
                toolingSession.SendMessage("$ACC,00*HH");
                Sleep(1000);
                toolingSession.StopSession();

                DateTime t2 = devTime.Now;

                Ping ping = new Ping();

                do
                {
                    PingReply pingReply = ping.Send(ip);
                    Console.WriteLine("[ping命令检测]关机延时 {0}， 现在过去时间 {1}", nDelayTime, (devTime.Now - t1).TotalSeconds);

                    if (pingReply.Status == IPStatus.TimedOut)
                    {
                        t2 = devTime.Now;
                        break;
                    }
                    else if (pingReply.Status == IPStatus.Success)
                    {
                        Thread.Sleep(1000);
                    }

                    if ((devTime.Now - t1).TotalSeconds >= nDelayTime + 30)
                    {
                        Console.WriteLine("关机等待时间太久 预计等待 {0} 实际等待 {1}", nDelayTime, (devTime.Now - t1).TotalSeconds);
                        Assert.IsTrue(Math.Abs((devTime.Now - t1).TotalSeconds - nDelayTime) < 30, "关机等待时间太久 预计等待 {0} 实际等待 {1}", nDelayTime, (devTime.Now - t1).TotalSeconds);
                    }

                } while (true);

                Assert.IsTrue(Math.Abs((t2 - t1).TotalSeconds - nDelayTime) < 20, "[ping指令检测]理论关机延时为{0}, 实际关机延时为{1}", nDelayTime, (t2 - t1).TotalSeconds);

                Sleep(300 * 1000);

                session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                toolingSession = new ToolingSession();
                toolingSession.StartSession(m_ToolingIP, m_ToolingPort);
                toolingSession.SendMessage("$ACC,01*HH");
                Sleep(1000);
                toolingSession.StopSession();

                DateTime t3 = devTime.Now;

                N9MHttpSession httpSession = new N9MHttpSession(ip, webport);

                Assert.AreEqual(0, httpSession.Login("/logincheck.rsp?type=1", username, password));
                string url = string.Format("/device.rsp?opt=getLog&cmd=bypage&page=1&st={0}&et={1}&alarmType=0&logcount=500&logtype=2&mark=0",
                    t2.ToString("yyyy-MM-dd HH:mm:ss"),
                    t3.ToString("yyyy-MM-dd HH:mm:ss"));
                url = url.Replace(" ", "%20");

                Console.WriteLine("url = {0}", url);
                string resp = httpSession.Get(url);
                jresp = JObject.Parse(resp);
                List<LogInfo> list = JsonConvert.DeserializeObject<List<LogInfo>>(jresp["data"].ToString());

                foreach (LogInfo info in list)
                {
                    if (info.logContent.Equals("开机") || info.logContent.Equals("Boot"))
                    {
                        t3 = info.GetStartTime();
                        Console.WriteLine("从日志中获取到的开机时间 t3 ={0}", t3.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                }

                Assert.IsTrue(Math.Abs((t3 - dtStartTime).TotalSeconds) <= 60, "设定的开机时间 {0}, 日志中记录的开始时间 {1}",
                    dtStartTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    t3.ToString("yyyy-MM-dd HH:mm:ss")
                    );
            }
        }
		[Ignore("111")]
        [Test]
        public void PowerOffRecordProtect()
        {
            foreach (string ip in IPList)
            {
                Console.WriteLine("ip ={0}", ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                //获取时间
                DeviceTime devTime = session.GetDeviceTime();

                DateTime dtStartTime = devTime.Now.AddMinutes(-6);
                DateTime dtEndTime = devTime.Now;

                //获取最新的开机时间
                N9MHttpSession httpSession = new N9MHttpSession(ip, 80);
                Assert.AreEqual(0, httpSession.Login("/logincheck.rsp?type=1", username, password));
                string url = string.Format("/device.rsp?opt=getLog&cmd=bypage&page=1&st={0}&et={1}&alarmType=0&logcount=500&logtype=2&mark=0",
                    dtStartTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    dtEndTime.ToString("yyyy-MM-dd HH:mm:ss"));
                url = url.Replace(" ", "%20");

                Console.WriteLine("url = {0}", url);

                string resp = httpSession.Get(url);

                JObject jresp = JObject.Parse(resp);

                Assert.IsNotNull(jresp, "日志查询回复为空");
                Assert.AreEqual(0, Convert.ToInt32(jresp["result"].ToString()), "日志查询结果返回失败");
                List<LogInfo> list = JsonConvert.DeserializeObject<List<LogInfo>>(jresp["data"].ToString());



                foreach (LogInfo info in list)
                {
                    if (info.logContent.Equals("车钥匙开机") || info.logContent.Equals("开机") || info.logContent.Equals("ACC ON") || info.logContent.Equals("Boot"))
                    {
                        DateTime dtBoot = info.GetStartTime();
                        Console.WriteLine("检测到日志记录开始时间为 {0}", dtBoot.ToString("yyyy-MM-dd HH:mm:ss"));

                        int nSeconds = 360 - (int)(devTime.Now - dtBoot).TotalSeconds;

                        if (nSeconds > 0)
                        {
                            Sleep(nSeconds * 1000);
                        }
                    }
                }


                //首先设置点火类型为点火
                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                JObject jssp = new JObject();

                jssp["UPT"] = (int)BootupType.ACC;
                jssp["DDS"] = 0;

                jmdvr["SSP"] = jssp;
                jparameter["MDVR"] = jmdvr;

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);
                Assert.IsNotNull(jresp["ERRORCODE"]);
                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));
                session.Logout();

                //关闭当前的ACC信号
                ToolingSession toolingSession = new ToolingSession();
                DateTime t1 = devTime.Now;
                toolingSession.StartSession(m_ToolingIP, m_ToolingPort);
                toolingSession.SendMessage("$ACC,00*HH");

                //一直获取设备的时间 当获取失败的时候，记录时间为T2
                DateTime t2 = devTime.Now;

                Ping ping = new Ping();

                do
                {
                    PingReply pingReply = ping.Send(ip);
                    Console.WriteLine("[ping命令检测]现在过去时间 {0}", (devTime.Now - t1).TotalSeconds);

                    if (pingReply.Status == IPStatus.TimedOut)
                    {
                        t2 = devTime.Now;
                        break;
                    }
                    else if (pingReply.Status == IPStatus.Success)
                    {
                        Thread.Sleep(1000);
                    }

                    if ((devTime.Now - t1).TotalSeconds >= 90)
                    {
                        Console.WriteLine("关机等待时间太久 实际等待 {0}",(devTime.Now - t1).TotalSeconds);
                        Assert.IsTrue(Math.Abs((devTime.Now - t1).TotalSeconds) <= 90, "关机等待时间太久 实际等待 {0}", (devTime.Now - t1).TotalSeconds);
                    }

                } while (true);

                Console.WriteLine("[ping指令检测]检测到关机时间 t2 = {0}", t2.ToString("yyyy-MM-dd HH:mm:ss"));

                Sleep(60 * 1000);
                toolingSession.SendMessage("$ACC,01*HH");

                Sleep(120 * 1000);
                //2分钟后获取设备的开机状态
                session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                httpSession = new N9MHttpSession(ip, webport);

                Assert.AreEqual(0, httpSession.Login("/logincheck.rsp?type=1", username, password));
                url = string.Format("/device.rsp?opt=getLog&cmd=bypage&page=1&st={0}&et={1}&alarmType=0&logcount=500&logtype=2&mark=0",
                    t1.ToString("yyyy-MM-dd HH:mm:ss"),
                    t2.ToString("yyyy-MM-dd HH:mm:ss"));
                url = url.Replace(" ", "%20");

                Console.WriteLine("url = {0}", url);
                resp = httpSession.Get(url);
                jresp = JObject.Parse(resp);
                list = JsonConvert.DeserializeObject<List<LogInfo>>(jresp["data"].ToString());

                foreach (LogInfo info in list)
                {
                    if (info.logContent.Equals("车钥匙关机") || info.logContent.Equals("ACC off"))
                    {
                        t2 = info.GetStartTime();
                        Console.WriteLine("从日志中获取到的关机时间 t2 ={0}", t2.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                }


                int nStreamType = (int)StreamType.MAIN_STREAM;

                jparameter["STREAMTYPE"] = nStreamType;
                jparameter["FILETYPE"] = 0xffffffff;
                jparameter["CHANNEL"] = 0xffffffff;
                jparameter["STARTTIME"] = t1.ToString("yyyyMMddHHmmss");
                jparameter["ENDTIME"] = t2.ToString("yyyyMMddHHmmss");

                jresp = session.SendCommand(Module.STORM, Operation.QUERYFILELIST, jparameter);
                List<RemoteFileInfo> remoteFileInfoList = JsonConvert.DeserializeObject<List<RemoteFileInfo>>(jresp["FILELIST"].ToString());

                TimeSection section = new TimeSection();

                foreach (RemoteFileInfo info in remoteFileInfoList)
                {
                    section.merage(info.nChannel, info.starttime, info.endtime);
                }

                int nChannelCount = session.GetChannelCount();

                for (int i = 0; i < nChannelCount; i++)
                {
                    section.GetTimeSpan(i, out dtStartTime, out dtEndTime);

                    Console.WriteLine("[通道 {0}]dtStartTime = {1}, dtEndTime = {2}", i, dtStartTime, dtEndTime);

                    Console.WriteLine("日志记录关机时间 {0}，关机前最后录像时间{1} 误差为{2}",
                        t2.ToString("yyyy-MM-dd HH:mm:ss"),
                        dtEndTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        (dtEndTime - t2).TotalSeconds);

                    Assert.GreaterOrEqual(
                        DateTime.ParseExact(dtEndTime.ToString("yyyyMMddHHmmss"), "yyyyMMddHHmmss", null),
                        DateTime.ParseExact(t2.ToString("yyyyMMddHHmmss"), "yyyyMMddHHmmss", null),
                        "日志记录关机时间 {0}，关机前最后录像时间{1} 误差为{2}",
                        t2.ToString("yyyy-MM-dd HH:mm:ss"),
                        dtEndTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        (dtEndTime - t2).TotalSeconds);
                }
            }
        }
		[Ignore("111111")]
        [Test]
        public void SuperCapRecordProtec()
        {
            foreach (string ip in IPList)
            {
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                //获取时间
                DeviceTime devTime = session.GetDeviceTime();

                DateTime dtStartTime = devTime.Now.AddMinutes(-6);
                DateTime dtEndTime = devTime.Now;

                //获取最新的开机时间
                N9MHttpSession httpSession = new N9MHttpSession(ip, 80);
                Assert.AreEqual(0, httpSession.Login("/logincheck.rsp?type=1", username, password));
                string url = string.Format("/device.rsp?opt=getLog&cmd=bypage&page=1&st={0}&et={1}&alarmType=0&logcount=500&logtype=2&mark=0",
                    dtStartTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    dtEndTime.ToString("yyyy-MM-dd HH:mm:ss"));
                url = url.Replace(" ", "%20");

                Console.WriteLine("url = {0}", url);

                string resp = httpSession.Get(url);

                JObject jresp = JObject.Parse(resp);

                Assert.IsNotNull(jresp, "日志查询回复为空");
                Assert.AreEqual(0, Convert.ToInt32(jresp["result"].ToString()), "日志查询结果返回失败");
                List<LogInfo> list = JsonConvert.DeserializeObject<List<LogInfo>>(jresp["data"].ToString());



                foreach (LogInfo info in list)
                {
                    if (info.logContent.Equals("车钥匙开机") || info.logContent.Equals("开机") || info.logContent.Equals("ACC ON") || info.logContent.Equals("Boot"))
                    {
                        DateTime dtBoot = info.GetStartTime();
                        Console.WriteLine("检测到日志记录开始时间为 {0}", dtBoot.ToString("yyyy-MM-dd HH:mm:ss"));

                        int nSeconds = 360 - (int)(devTime.Now - dtBoot).TotalSeconds;

                        if (nSeconds  > 0)
                        {
                            Sleep(nSeconds * 1000);
                        }
                    }
                }

                //首先设置点火类型为点火
                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                JObject jssp = new JObject();

                jssp["UPT"] = (int)BootupType.ACC;
                jssp["DDS"] = 0;

                jmdvr["SSP"] = jssp;
                jparameter["MDVR"] = jmdvr;

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);
                Assert.IsNotNull(jresp["ERRORCODE"]);
                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));
                session.Logout();

                //并记录系统时间1，同时关闭电源线信号
                DateTime t1 = devTime.Now;

                //关闭当前的ACC信号
                ToolingSession toolingSession = new ToolingSession();
                toolingSession.StartSession(m_ToolingIP, m_ToolingPort);
                toolingSession.SendMessage("$ACC,00*HH");

                //一直获取设备的时间 当获取失败的时候，记录时间为T2
                DateTime t2 = devTime.Now;

                Ping ping = new Ping();

                do
                {
                    PingReply pingReply = ping.Send(ip);
                    Console.WriteLine("[ping命令检测]现在过去时间 {0}", (devTime.Now - t1).TotalSeconds);

                    if (pingReply.Status == IPStatus.TimedOut)
                    {
                        t2 = devTime.Now;
                        break;
                    }
                    else if (pingReply.Status == IPStatus.Success)
                    {
                        Thread.Sleep(1000);
                    }

                    if ((devTime.Now - t1).TotalSeconds >= 90)
                    {
                        Console.WriteLine("关机等待时间太久 实际等待 {0}", (devTime.Now - t1).TotalSeconds);
                        Assert.IsTrue(Math.Abs((devTime.Now - t1).TotalSeconds) <= 90, "关机等待时间太久 实际等待 {0}", (devTime.Now - t1).TotalSeconds);
                    }

                } while (true);

                Console.WriteLine("[ping指令检测]检测到关机时间 t2 = {0}", t2.ToString("yyyy-MM-dd HH:mm:ss"));

                //休眠10秒 打开ACC信号
                Sleep(10 * 1000);
                toolingSession.SendMessage("$ACC,01*HH");

                //2分钟后获取开机状态
                Sleep(120 * 1000);

                httpSession = new N9MHttpSession(ip, webport);

                Assert.AreEqual(0, httpSession.Login("/logincheck.rsp?type=1", username, password));
                url = string.Format("/device.rsp?opt=getLog&cmd=bypage&page=1&st={0}&et={1}&alarmType=0&logcount=500&logtype=2&mark=0",
                    t1.ToString("yyyy-MM-dd HH:mm:ss"),
                    t2.ToString("yyyy-MM-dd HH:mm:ss"));
                url = url.Replace(" ", "%20");

                Console.WriteLine("url = {0}", url);
                resp = httpSession.Get(url);
                jresp = JObject.Parse(resp);
                list = JsonConvert.DeserializeObject<List<LogInfo>>(jresp["data"].ToString());

                foreach (LogInfo info in list)
                {
                    if (info.logContent.Equals("车钥匙关机") || info.logContent.Equals("ACC off"))
                    {
                        t2 = info.GetStartTime();
                        Console.WriteLine("从日志中获取到的关机时间 t2 ={0}", t2.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                }


                session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                //查询开机前录像的最后时间，等于时间点2，则OK
                int nStreamType = (int)StreamType.MAIN_STREAM;

                jparameter["STREAMTYPE"] = nStreamType;
                jparameter["FILETYPE"] = 0xffffffff;
                jparameter["CHANNEL"] = 0xffffffff;
                jparameter["STARTTIME"] = t1.ToString("yyyyMMddHHmmss");
                jparameter["ENDTIME"] = t2.ToString("yyyyMMddHHmmss");

                jresp = session.SendCommand(Module.STORM, Operation.QUERYFILELIST, jparameter);
                List<RemoteFileInfo> remoteFileInfoList = JsonConvert.DeserializeObject<List<RemoteFileInfo>>(jresp["FILELIST"].ToString());

                TimeSection section = new TimeSection();

                foreach (RemoteFileInfo info in remoteFileInfoList)
                {
                    section.merage(info.nChannel, info.starttime, info.endtime);
                }

                int nChannelCount = session.GetChannelCount();

                for (int i = 0; i < nChannelCount; i++)
                {
                    section.GetTimeSpan(i, out dtStartTime, out dtEndTime);

                    Console.WriteLine("[通道 {0}]dtStartTime = {1}, dtEndTime = {2}", i, dtStartTime, dtEndTime);

                    Console.WriteLine("日志记录关机时间 {0}，关机前最后录像时间{1} 误差为{2}",
                        t2.ToString("yyyy-MM-dd HH:mm:ss"),
                        dtEndTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        (dtEndTime - t2).TotalSeconds);

                    Assert.GreaterOrEqual(
                        DateTime.ParseExact(dtEndTime.ToString("yyyyMMddHHmmss"), "yyyyMMddHHmmss", null),
                        DateTime.ParseExact(t2.ToString("yyyyMMddHHmmss"), "yyyyMMddHHmmss", null),
                        "日志记录关机时间 {0}，关机前最后录像时间{1} 误差为{2}",
                        t2.ToString("yyyy-MM-dd HH:mm:ss"),
                        dtEndTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        (dtEndTime - t2).TotalSeconds);
                }

                //因为前面执行了关机 所以休眠6分钟
                Sleep(360 * 1000);

                //get并记录系统时间3，同时关闭电源线信号
                DateTime t3 = devTime.Now;

                //关闭当前的ACC信号
                toolingSession.SendMessage("$ACC,00*HH");

                DateTime t4 = devTime.Now;

                ping = new Ping();

                do
                {
                    PingReply pingReply = ping.Send(ip);
                    Console.WriteLine("[ping命令检测]现在过去时间 {0}", (devTime.Now - t3).TotalSeconds);

                    if (pingReply.Status == IPStatus.TimedOut)
                    {
                        t4 = devTime.Now;
                        break;
                    }
                    else if (pingReply.Status == IPStatus.Success)
                    {
                        Thread.Sleep(1000);
                    }

                    if ((devTime.Now - t3).TotalSeconds >= 90)
                    {
                        Console.WriteLine("关机等待时间太久 实际等待 {0}", (devTime.Now - t3).TotalSeconds);
                        Assert.IsTrue(Math.Abs((devTime.Now - t3).TotalSeconds) <= 90, "关机等待时间太久 实际等待 {0}", (devTime.Now - t3).TotalSeconds);
                    }

                } while (true);

                Console.WriteLine("[ping指令检测]检测到关机时间 t2 = {0}", t4.ToString("yyyy-MM-dd HH:mm:ss"));


                //5分钟后 打开电源灯信号
                Sleep(300 * 1000);
                toolingSession.SendMessage("$ACC,01*HH");

                //2分钟后获取设备的开机状态
                Sleep(120 * 1000);
                session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                httpSession = new N9MHttpSession(ip, webport);

                Assert.AreEqual(0, httpSession.Login("/logincheck.rsp?type=1", username, password));
                url = string.Format("/device.rsp?opt=getLog&cmd=bypage&page=1&st={0}&et={1}&alarmType=0&logcount=500&logtype=2&mark=0",
                    t3.ToString("yyyy-MM-dd HH:mm:ss"),
                    t4.ToString("yyyy-MM-dd HH:mm:ss"));
                url = url.Replace(" ", "%20");

                Console.WriteLine("url = {0}", url);
                resp = httpSession.Get(url);
                jresp = JObject.Parse(resp);
                list = JsonConvert.DeserializeObject<List<LogInfo>>(jresp["data"].ToString());

                foreach (LogInfo info in list)
                {
                    if (info.logContent.Equals("车钥匙关机") || info.logContent.Equals("ACC off"))
                    {
                        t4 = info.GetStartTime();
                        Console.WriteLine("从日志中获取到的关机时间 t4 ={0}", t4.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                }

                //查询开机前录像的最后时间，等于时间点2，则OK
                nStreamType = (int)StreamType.MAIN_STREAM;

                jparameter["STREAMTYPE"] = nStreamType;
                jparameter["FILETYPE"] = 0xffffffff;
                jparameter["CHANNEL"] = 0xffffffff;
                jparameter["STARTTIME"] = t3.ToString("yyyyMMddHHmmss");
                jparameter["ENDTIME"] = t4.ToString("yyyyMMddHHmmss");

                jresp = session.SendCommand(Module.STORM, Operation.QUERYFILELIST, jparameter);
                remoteFileInfoList = JsonConvert.DeserializeObject<List<RemoteFileInfo>>(jresp["FILELIST"].ToString());

                section = new TimeSection();

                foreach (RemoteFileInfo info in remoteFileInfoList)
                {
                    section.merage(info.nChannel, info.starttime, info.endtime);
                }

                nChannelCount = session.GetChannelCount();

                for (int i = 0; i < nChannelCount; i++)
                {
                    section.GetTimeSpan(i, out dtStartTime, out dtEndTime);

                    Console.WriteLine("[通道 {0}]dtStartTime = {1}, dtEndTime = {2}", i, dtStartTime, dtEndTime);

                    Console.WriteLine("日志记录关机时间 {0}，关机前最后录像时间{1} 误差为{2}",
                        t4.ToString("yyyy-MM-dd HH:mm:ss"),
                        dtEndTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        (dtEndTime - t4).TotalSeconds);

                    Assert.GreaterOrEqual(
                        DateTime.ParseExact(dtEndTime.ToString("yyyyMMddHHmmss"), "yyyyMMddHHmmss", null),
                        DateTime.ParseExact(t4.ToString("yyyyMMddHHmmss"), "yyyyMMddHHmmss", null),
                        "日志记录关机时间 {0}，关机前最后录像时间{1} 误差为{2}",
                        t4.ToString("yyyy-MM-dd HH:mm:ss"),
                        dtEndTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        (dtEndTime - t4).TotalSeconds);
                }
            }
        }
		[Ignore("111111")]
        [Test]
        public void VoltageAntiShake()
        {
            foreach (string ip in IPList)
            {
                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine("第 {0} 次执行", i);
                    N9MSession session = new N9MSession(ip, port);
                    Assert.AreEqual(0, session.Login(username, password), "登录设备失败");
                    session.Logout();

                    //关闭ACC信号
                    ToolingSession toolingSession = new ToolingSession();
                    toolingSession.StartSession(m_ToolingIP, m_ToolingPort);
                    toolingSession.SendMessage("DC,00*HH");

                    Sleep(500);
                    toolingSession.SendMessage("DC,01*HH");
                    Sleep(15 * 1000);
                    toolingSession.StopSession();

                    session = new N9MSession(ip, port);
                    Assert.AreEqual(0, session.Login(username, password), "登录设备失败");
                    session.Logout();
                }
            }
        }
    }
}
