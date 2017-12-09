using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using N9MTest.SDK.net;
using Newtonsoft.Json.Linq;
using N9MTest.SDK.Tooling;
using Newtonsoft.Json;
using Util;

namespace N9MTest.Alarm
{
    [TestFixture]
    class TestCase_AlarmPrerecord:TestCase_Basecase
    {
        [Test]
        public void IFrameRatePreRecordOnTimerMode()
        {
            int nPrerecordTime = Convert.ToInt32(this.CFG.gNode("Alarm/IFrameRatePreRecordOnTimerMode/prerecord-time").InnerText);
            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                DeviceTime devTime = session.GetDeviceTime();

                //设置录像模式为报警录像 勾选I帧录像 设置报警录像参数
                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                JArray jmainArray = new JArray();
                JObject jrp = new JObject();
                JArray jrcpArray = new JArray();

                DateTime dtNow = devTime.Now;

                DateTime dtStartTime = dtNow.AddMinutes(-10);
                DateTime dtEndTime = dtNow.AddMinutes(10);

                for (int i = 0; i < session.GetChannelCount(); i++)
                {
                    JObject jrcp = new JObject();
                    jrcp["RM"] = (int)RecordMode.Timer;

                    JArray jrsiArray = new JArray();

                    for (int weekday = 0; weekday < 7; weekday++)
                    {
                        jrsiArray.Insert(weekday, new JArray());
                        for (int n = 0; n < 1; n++)
                        {
                            JArray array = (JArray)jrsiArray[weekday];
                            array.Insert(n, new JObject());
                            jrsiArray[weekday][n]["S"] = dtStartTime.Hour * 3600 + dtStartTime.Minute * 60 + dtStartTime.Second;
                            jrsiArray[weekday][n]["E"] = dtEndTime.Hour * 3600 + dtEndTime.Minute * 60 + dtEndTime.Second;
                            jrsiArray[weekday][n]["T"] = 1;
                        }
                    }

                    jrcp["RSI"] = jrsiArray;
                    jrcpArray.Insert(i, jrcp);

                    JObject main = new JObject();
                    main["VEN"] = 1;
                    main["FT"] = (int)EncodeFrameType.IFrame;

                    jmainArray.Insert(i, main);
                }

                JArray jiopArray = new JArray();
                for (int i = 0; i < 8; i++)
                {
                    JObject jiop = new JObject();

                    if (i == 0)
                    {
                        jiop["EN"] = 1;
                        jiop["EL"] = 1;
                        jiop["AS"] = 1;

                        JObject japr = new JObject();
                        JObject jar = new JObject();

                        jar["CH"] = 1;
                        jar["P"] = nPrerecordTime;
                        jar["D"] = 60;
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

                jrp["PRE"] = 1;
                jrp["RCP"] = jrcpArray;

                jmdvr["IOP"] = jiopArray;
                jmdvr["RP"] = jrp;
                jmdvr["MAIN"] = jmainArray;

                jparameter["MDVR"] = jmdvr;

                Console.WriteLine("jparameter = {0}", jparameter.ToString());

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);
                Assert.IsNotNull(jresp["ERRORCODE"]);
                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));

                Sleep(nPrerecordTime * 2 * 1000);

                ToolingSession toolingSesstion = new ToolingSession();
                toolingSesstion.StartSession(m_ToolingIP, m_ToolingPort);
                toolingSesstion.SendMessage("$IO01,01*HH");
                DateTime t2 = devTime.Now;
                Sleep(1000);
                toolingSesstion.SendMessage("$IO01,00*HH");

                DateTime t1 = t2.AddSeconds(0 - nPrerecordTime);

                Sleep(5000);

                int nStreamType = (int)StreamType.MAIN_STREAM;
                int nChannelBits = 1 << 0;



                string starttime = t1.ToString("yyyyMMddHHmmss");
                string endtime = t2.ToString("yyyyMMddHHmmss");

                //查询报警录像
                jparameter = new JObject();
                jparameter["STREAMTYPE"] = nStreamType;
                jparameter["FILETYPE"] = 1 << 1;
                jparameter["CHANNEL"] = nChannelBits;
                jparameter["STARTTIME"] = starttime;
                jparameter["ENDTIME"] = endtime;

                jresp = session.SendCommand(Module.STORM, Operation.QUERYFILELIST, jparameter);



                List<RemoteFileInfo> list = JsonConvert.DeserializeObject<List<RemoteFileInfo>>(jresp["FILELIST"].ToString());
                Assert.IsNotNull(jresp["FILELIST"]);
                Assert.IsTrue(jresp["FILELIST"].HasValues, "获取到的文件列表为空");

               
                //设置录像模式为开机录像
                do
                {
                    jparameter = new JObject();
                    jmdvr = new JObject();
                    jrp = new JObject();
                    jmainArray = new JArray();
                    JArray jmpArray = new JArray();
                    jrcpArray = new JArray();

                    for (int i = 0; i < session.GetChannelCount(); i++)
                    {
                        JObject jrcp = new JObject();

                        //设置为开机录像
                        jrcp["RM"] = (int)RecordMode.Boot;
                        jrcpArray.Insert(i, jrcp);

                        JObject jmain = new JObject();
                        jmain["VEN"] = 1;
                        jmain["FT"] = (int)EncodeFrameType.Normal;

                        jmainArray.Insert(i, jmain);
                    }

                    jrp["RCP"] = jrcpArray;

                    jmdvr["RP"] = jrp;
                    jmdvr["MAIN"] = jmainArray;

                    jparameter["MDVR"] = jmdvr;

                    jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);
                    Assert.IsNotNull(jresp["ERRORCODE"]);
                    Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));

                } while (false);

                

                foreach (RemoteFileInfo info in list)
                {
                    Console.WriteLine("[通道 {0}]报警录像文件时间 {1}", info.nChannel, info.GetTimeSpan().TotalSeconds);
                    Console.WriteLine("报警录像预录开始时间应该为{0},实际报警录像预录时间开始为{1}", t1, info.GetStartTime());
                    Assert.AreEqual(info.GetStartTime().ToString("yyyyMMddHHmmss"), t1.ToString("yyyyMMddHHmmss"), "报警录像预录开始时间应该为{0},实际报警录像预录时间开始为{1}", t1, info.GetStartTime());

                    Console.WriteLine("报警录像预录结束时间应该为{0},实际报警录像预录时间结束为{1}", t2, info.GetEndTime());
                    Assert.AreEqual(info.GetEndTime().ToString("yyyyMMddHHmmss"), t2.ToString("yyyyMMddHHmmss"), "报警录像预录结束时间应该为{0},实际报警录像预录时间结束为{1}", t2, info.GetEndTime());
                }
            }
        }

        /// <summary>
        /// 报警模式下启用I帧时预录
        /// </summary>

        [Test]
        public void IFrameRatePreRecordOnAlarmMode()
        {
            int nPrerecordTime = Convert.ToInt32(this.CFG.gNode("Alarm/IFrameRatePreRecordOnAlarmMode/prerecord-time").InnerText);

            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                DeviceTime devTime = session.GetDeviceTime();

                //设置录像模式为报警录像 勾选I帧录像 设置报警录像参数
                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                JArray jmainArray = new JArray();
                JObject jrp = new JObject();
                JArray jrcpArray = new JArray();

                for (int i = 0; i < session.GetChannelCount(); i++)
                {
                    JObject jrcp = new JObject();
                    jrcp["RM"] = (int)RecordMode.Alarm;
                    jrcpArray.Insert(i, jrcp);

                    JObject jmain = new JObject();
                    jmain["VEN"] = 1;
                    jmain["FT"] = (int)EncodeFrameType.IFrame;

                    jmainArray.Insert(i, jmain);
                }

                JArray jiopArray = new JArray();
                for (int i = 0; i < 8; i++)
                {
                    JObject jiop = new JObject();

                    if (i == 0)
                    {
                        jiop["EN"] = 1;
                        jiop["EL"] = 1;
                        jiop["AS"] = 1;

                        JObject japr = new JObject();
                        JObject jar = new JObject();

                        jar["CH"] = 1;
                        jar["P"] = nPrerecordTime;
                        jar["D"] = 60;
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

                jrp["PRE"] = 1;
                jrp["RCP"] = jrcpArray;
                jmdvr["MAIN"] = jmainArray;
                jmdvr["IOP"] = jiopArray;
                jmdvr["RP"] = jrp;
                jparameter["MDVR"] = jmdvr;

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);
                Assert.IsNotNull(jresp["ERRORCODE"]);
                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));

                Sleep(nPrerecordTime * 2 * 1000);

                ToolingSession toolingSesstion = new ToolingSession();
                toolingSesstion.StartSession(m_ToolingIP, m_ToolingPort);

                
                toolingSesstion.SendMessage("$IO01,01*HH");
                DateTime t2 = devTime.Now;
                Sleep(1000);
                toolingSesstion.SendMessage("$IO01,00*HH");
                
                DateTime t1 = t2.AddSeconds(0 - nPrerecordTime);

                Sleep(5000);

                int nStreamType = (int)StreamType.MAIN_STREAM;
                int nChannelBits = 1 << 0;

                

                string starttime = t1.ToString("yyyyMMddHHmmss");
                string endtime = t2.ToString("yyyyMMddHHmmss");

                //查询报警录像
                jparameter = new JObject();
                jparameter["STREAMTYPE"] = nStreamType;
                jparameter["FILETYPE"] = 1<<1;
                jparameter["CHANNEL"] = nChannelBits;
                jparameter["STARTTIME"] = starttime;
                jparameter["ENDTIME"] = endtime;

                jresp = session.SendCommand(Module.STORM, Operation.QUERYFILELIST, jparameter);

                List<RemoteFileInfo> list = JsonConvert.DeserializeObject<List<RemoteFileInfo>>(jresp["FILELIST"].ToString());
                Assert.IsNotNull(jresp["FILELIST"]);
                Assert.IsTrue(jresp["FILELIST"].HasValues, "获取到的文件列表为空");

                foreach (RemoteFileInfo info in list)
                {
                    Console.WriteLine("[通道 {0}]报警录像文件时间 {1}", info.nChannel, info.GetTimeSpan().TotalSeconds);
                    Console.WriteLine("报警录像预录开始时间应该为{0},实际报警录像预录时间开始为{1}", t1, info.GetStartTime());
                    Assert.AreEqual(info.GetStartTime().ToString("yyyyMMddHHmmss"), t1.ToString("yyyyMMddHHmmss"), "报警录像预录开始时间应该为{0},实际报警录像预录时间开始为{1}", t1, info.GetStartTime());

                    Console.WriteLine("报警录像预录结束时间应该为{0},实际报警录像预录时间结束为{1}", t2, info.GetEndTime());
                    Assert.AreEqual(info.GetEndTime().ToString("yyyyMMddHHmmss"), t2.ToString("yyyyMMddHHmmss"), "报警录像预录结束时间应该为{0},实际报警录像预录时间结束为{1}", t2, info.GetEndTime());
                }
            }
        }
        /// <summary>
        /// 点火模式下非I帧录像时预录
        /// </summary>
		[Ignore("11111111")]
        [Test]
        public void NormalFrameRatePreRecordOnIgnitionMode()
        {
            int nPrerecordTime = Convert.ToInt32(this.CFG.gNode("Alarm/NormalFrameRatePreRecordOnIgnitionMode/prerecord-time").InnerText);

            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                DeviceTime devTime = session.GetDeviceTime();

                //设置录像模式为报警录像 勾选I帧录像 设置报警录像参数
                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                JArray jmainArray = new JArray();
                JObject jrp = new JObject();
                JArray jrcpArray = new JArray();

                for (int i = 0; i < session.GetChannelCount(); i++)
                {
                    JObject jrcp = new JObject();
                    jrcp["RM"] = (int)RecordMode.Boot;
                    jrcpArray.Insert(i, jrcp);

                    JObject jmain = new JObject();
                    jmain["VEN"] = 1;
                    jmain["FT"] = (int)EncodeFrameType.Normal;

                    jmainArray.Insert(i, jmain);
                }

                JArray jiopArray = new JArray();
                for (int i = 0; i < 8; i++)
                {
                    JObject jiop = new JObject();

                    if (i == 0)
                    {
                        jiop["EN"] = 1;
                        jiop["EL"] = 1;
                        jiop["AS"] = 1;

                        JObject japr = new JObject();
                        JObject jar = new JObject();

                        jar["CH"] = 1;
                        jar["P"] = nPrerecordTime;
                        jar["D"] = 60;
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

                jrp["PRE"] = 1;
                jrp["RCP"] = jrcpArray;
                jmdvr["MAIN"] = jmainArray;
                jmdvr["IOP"] = jiopArray;
                jmdvr["RP"] = jrp;
                jparameter["MDVR"] = jmdvr;

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);
                Assert.IsNotNull(jresp["ERRORCODE"]);
                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));

                Sleep(nPrerecordTime * 2 * 1000);

                ToolingSession toolingSesstion = new ToolingSession();
                toolingSesstion.StartSession(m_ToolingIP, m_ToolingPort);


                toolingSesstion.SendMessage("$IO01,01*HH");
                DateTime t2 = devTime.Now;
                Sleep(1000);
                toolingSesstion.SendMessage("$IO01,00*HH");

                DateTime t1 = t2.AddSeconds(0 - nPrerecordTime);

                Sleep(5000);

                int nStreamType = (int)StreamType.MAIN_STREAM;
                int nChannelBits = 1 << 0;



                string starttime = t1.ToString("yyyyMMddHHmmss");
                string endtime = t2.ToString("yyyyMMddHHmmss");

                //查询报警录像
                jparameter = new JObject();
                jparameter["STREAMTYPE"] = nStreamType;
                jparameter["FILETYPE"] = 1 << 1;
                jparameter["CHANNEL"] = nChannelBits;
                jparameter["STARTTIME"] = starttime;
                jparameter["ENDTIME"] = endtime;

                jresp = session.SendCommand(Module.STORM, Operation.QUERYFILELIST, jparameter);

                List<RemoteFileInfo> list = JsonConvert.DeserializeObject<List<RemoteFileInfo>>(jresp["FILELIST"].ToString());
                Assert.IsNotNull(jresp["FILELIST"]);
                Assert.IsTrue(jresp["FILELIST"].HasValues, "获取到的文件列表为空");

                foreach (RemoteFileInfo info in list)
                {
                    Console.WriteLine("[通道 {0}]报警录像文件时间 {1}", info.nChannel, info.GetTimeSpan().TotalSeconds);
                    Console.WriteLine("报警录像预录开始时间应该为{0},实际报警录像预录时间开始为{1}", t1, info.GetStartTime());
                    Assert.AreEqual(info.GetStartTime().ToString("yyyyMMddHHmmss"), t1.ToString("yyyyMMddHHmmss"), "报警录像预录开始时间应该为{0},实际报警录像预录时间开始为{1}", t1, info.GetStartTime());

                    Console.WriteLine("报警录像预录结束时间应该为{0},实际报警录像预录时间结束为{1}", t2, info.GetEndTime());
                    Assert.AreEqual(info.GetEndTime().ToString("yyyyMMddHHmmss"), t2.ToString("yyyyMMddHHmmss"), "报警录像预录结束时间应该为{0},实际报警录像预录时间结束为{1}", t2, info.GetEndTime());
                }
            }
        }

		/// <summary>
		/// 定时模式下非I帧录像时预录
		/// </summary>
		[Ignore("11111111")]
		[Test]
        public void NormalFrameRatePreRecordOnTimerMode()
        {
            int nPrerecordTime = Convert.ToInt32(this.CFG.gNode("Alarm/NormalFrameRatePreRecordOnTimerMode/prerecord-time").InnerText);
            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                DeviceTime devTime = session.GetDeviceTime();

                //设置录像模式为报警录像 勾选I帧录像 设置报警录像参数
                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                JArray jmainArray = new JArray();
                JObject jrp = new JObject();
                JArray jrcpArray = new JArray();

                DateTime dtNow = devTime.Now;

                DateTime dtStartTime = dtNow.AddMinutes(-10);
                DateTime dtEndTime = dtNow.AddMinutes(10);

                for (int i = 0; i < session.GetChannelCount(); i++)
                {
                    JObject jrcp = new JObject();
                    jrcp["RM"] = (int)RecordMode.Timer;

                    JArray jrsiArray = new JArray();

                    for (int weekday = 0; weekday < 7; weekday++)
                    {
                        jrsiArray.Insert(weekday, new JArray());
                        for (int n = 0; n < 1; n++)
                        {
                            JArray array = (JArray)jrsiArray[weekday];
                            array.Insert(n, new JObject());
                            jrsiArray[weekday][n]["S"] = dtStartTime.Hour * 3600 + dtStartTime.Minute * 60 + dtStartTime.Second;
                            jrsiArray[weekday][n]["E"] = dtEndTime.Hour * 3600 + dtEndTime.Minute * 60 + dtEndTime.Second;
                            jrsiArray[weekday][n]["T"] = 1;
                        }
                    }

                    jrcp["RSI"] = jrsiArray;
                    jrcpArray.Insert(i, jrcp);

                    JObject jmain = new JObject();
                    jmain["VEN"] = 1;
                    jmain["FT"] = (int)EncodeFrameType.Normal;

                    jmainArray.Insert(i, jmain);
                }

                JArray jiopArray = new JArray();
                for (int i = 0; i < 8; i++)
                {
                    JObject jiop = new JObject();

                    if (i == 0)
                    {
                        jiop["EN"] = 1;
                        jiop["EL"] = 1;
                        jiop["AS"] = 1;

                        JObject japr = new JObject();
                        JObject jar = new JObject();

                        jar["CH"] = 1;
                        jar["P"] = nPrerecordTime;
                        jar["D"] = 60;
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

                jrp["PRE"] = 1;
                jrp["RCP"] = jrcpArray;
                jmdvr["MAIN"] = jmainArray;
                jmdvr["IOP"] = jiopArray;
                jmdvr["RP"] = jrp;
                jparameter["MDVR"] = jmdvr;

                Console.WriteLine("jparameter = {0}", jparameter.ToString());

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);
                Assert.IsNotNull(jresp["ERRORCODE"]);
                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));

                Sleep(nPrerecordTime * 2 * 1000);

                ToolingSession toolingSesstion = new ToolingSession();
                toolingSesstion.StartSession(m_ToolingIP, m_ToolingPort);
                toolingSesstion.SendMessage("$IO01,01*HH");
                DateTime t2 = devTime.Now;
                Sleep(1000);
                toolingSesstion.SendMessage("$IO01,00*HH");

                DateTime t1 = t2.AddSeconds(0 - nPrerecordTime);

                Sleep(5000);

                int nStreamType = (int)StreamType.MAIN_STREAM;
                int nChannelBits = 1 << 0;



                string starttime = t1.ToString("yyyyMMddHHmmss");
                string endtime = t2.ToString("yyyyMMddHHmmss");

                //查询报警录像
                jparameter = new JObject();
                jparameter["STREAMTYPE"] = nStreamType;
                jparameter["FILETYPE"] = 1 << 1;
                jparameter["CHANNEL"] = nChannelBits;
                jparameter["STARTTIME"] = starttime;
                jparameter["ENDTIME"] = endtime;

                jresp = session.SendCommand(Module.STORM, Operation.QUERYFILELIST, jparameter);



                List<RemoteFileInfo> list = JsonConvert.DeserializeObject<List<RemoteFileInfo>>(jresp["FILELIST"].ToString());
                Assert.IsNotNull(jresp["FILELIST"]);
                Assert.IsTrue(jresp["FILELIST"].HasValues, "获取到的文件列表为空");


                //设置录像模式为开机录像
                do
                {
                    jparameter = new JObject();
                    jmdvr = new JObject();
                    jrp = new JObject();
                    jmainArray = new JArray();
                    JArray jmpArray = new JArray();
                    jrcpArray = new JArray();

                    for (int i = 0; i < session.GetChannelCount(); i++)
                    {
                        JObject jrcp = new JObject();

                        //设置为开机录像
                        jrcp["RM"] = (int)RecordMode.Boot;
                        jrcpArray.Insert(i, jrcp);

                        //设置主码流时间
                        JObject jmain = new JObject();
                        jmain["VEN"] = 1;
                        jmain["FT"] = (int)EncodeFrameType.Normal;
                        jmainArray.Insert(i, jmain);
                        
                    }

                    jrp["RCP"] = jrcpArray;
                    jmdvr["MAIN"] = jmainArray;
                    jmdvr["RP"] = jrp;
                    jparameter["MDVR"] = jmdvr;

                    jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);
                    Assert.IsNotNull(jresp["ERRORCODE"]);
                    Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));

                } while (false);


                foreach (RemoteFileInfo info in list)
                {
                    Console.WriteLine("[通道 {0}]报警录像文件时间 {1}", info.nChannel, info.GetTimeSpan().TotalSeconds);
                    Console.WriteLine("报警录像预录开始时间应该为{0},实际报警录像预录时间开始为{1}", t1, info.GetStartTime());
                    Assert.AreEqual(info.GetStartTime().ToString("yyyyMMddHHmmss"), t1.ToString("yyyyMMddHHmmss"), "报警录像预录开始时间应该为{0},实际报警录像预录时间开始为{1}", t1, info.GetStartTime());

                    Console.WriteLine("报警录像预录结束时间应该为{0},实际报警录像预录时间结束为{1}", t2, info.GetEndTime());
                    Assert.AreEqual(info.GetEndTime().ToString("yyyyMMddHHmmss"), t2.ToString("yyyyMMddHHmmss"), "报警录像预录结束时间应该为{0},实际报警录像预录时间结束为{1}", t2, info.GetEndTime());
                }
            }
        }

		/// <summary>
		/// 报警模式下非I帧录像时预录
		/// </summary>
		[Ignore("11111111")]
		[Test]
        public void NormalFrameRatePreRecordOnAlarmMode()
        {
            int nPrerecordTime = Convert.ToInt32(this.CFG.gNode("Alarm/NormalFrameRatePreRecordOnAlarmMode/prerecord-time").InnerText);

            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                DeviceTime devTime = session.GetDeviceTime();

                //设置录像模式为报警录像 常规录像模式 设置报警录像参数
                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                JArray jmainArray = new JArray();
                JObject jrp = new JObject();
                JArray jrcpArray = new JArray();

                for (int i = 0; i < session.GetChannelCount(); i++)
                {
                    JObject jrcp = new JObject();
                    jrcp["RM"] = (int)RecordMode.Alarm;
                    jrcpArray.Insert(i, jrcp);

                    JObject jmain = new JObject();
                    jmain["VEN"] = 1;
                    jmain["FT"] = (int)EncodeFrameType.Normal;

                    jmainArray.Insert(i, jmain);
                }

                JArray jiopArray = new JArray();
                for (int i = 0; i < 8; i++)
                {
                    JObject jiop = new JObject();

                    if (i == 0)
                    {
                        jiop["EN"] = 1;
                        jiop["EL"] = 1;
                        jiop["AS"] = 1;

                        JObject japr = new JObject();
                        JObject jar = new JObject();

                        jar["CH"] = 1;
                        jar["P"] = nPrerecordTime;
                        jar["D"] = 60;
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

                jrp["PRE"] = 1;
                jrp["RCP"] = jrcpArray;
                jmdvr["MAIN"] = jmainArray;
                jmdvr["IOP"] = jiopArray;
                jmdvr["RP"] = jrp;
                jparameter["MDVR"] = jmdvr;

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);
                Assert.IsNotNull(jresp["ERRORCODE"]);
                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));

                Sleep(nPrerecordTime * 2 * 1000);

                ToolingSession toolingSesstion = new ToolingSession();
                toolingSesstion.StartSession(m_ToolingIP, m_ToolingPort);


                toolingSesstion.SendMessage("$IO01,01*HH");
                DateTime t2 = devTime.Now;
                Sleep(1000);
                toolingSesstion.SendMessage("$IO01,00*HH");

                DateTime t1 = t2.AddSeconds(0 - nPrerecordTime);

                Sleep(5000);

                int nStreamType = (int)StreamType.MAIN_STREAM;
                int nChannelBits = 1 << 0;



                string starttime = t1.ToString("yyyyMMddHHmmss");
                string endtime = t2.ToString("yyyyMMddHHmmss");

                //查询报警录像
                jparameter = new JObject();
                jparameter["STREAMTYPE"] = nStreamType;
                jparameter["FILETYPE"] = 1 << 1;
                jparameter["CHANNEL"] = nChannelBits;
                jparameter["STARTTIME"] = starttime;
                jparameter["ENDTIME"] = endtime;

                jresp = session.SendCommand(Module.STORM, Operation.QUERYFILELIST, jparameter);

                List<RemoteFileInfo> list = JsonConvert.DeserializeObject<List<RemoteFileInfo>>(jresp["FILELIST"].ToString());
                Assert.IsNotNull(jresp["FILELIST"]);
                Assert.IsTrue(jresp["FILELIST"].HasValues, "获取到的文件列表为空");

                TimeSection section = new TimeSection();

                foreach (RemoteFileInfo info in list)
                {
                    section.merage(info.nChannel, info.starttime, info.endtime);
                }

                DateTime dtStartTime;
                DateTime dtEndTime;
                section.GetTimeSpan(0, out dtStartTime, out dtEndTime);

                Console.WriteLine("[通道 {0}]报警录像文件时间 {1}", 0, (dtEndTime - dtStartTime).TotalSeconds);
                Console.WriteLine("报警录像预录开始时间应该为{0},实际报警录像预录时间开始为{1}", t1, dtStartTime);
                Assert.AreEqual(dtStartTime.ToString("yyyyMMddHHmmss"), t1.ToString("yyyyMMddHHmmss"), "报警录像预录开始时间应该为{0},实际报警录像预录时间开始为{1}", t1, dtStartTime);

                Console.WriteLine("报警录像预录结束时间应该为{0},实际报警录像预录时间结束为{1}", t2, dtEndTime);
                Assert.AreEqual(dtEndTime.ToString("yyyyMMddHHmmss"), t2.ToString("yyyyMMddHHmmss"), "报警录像预录结束时间应该为{0},实际报警录像预录时间结束为{1}", t2, dtEndTime);
            }
        }

		/// <summary>
		/// 点火模式下启用I帧时预录
		/// </summary>
		[Ignore("11111111")]
		[Test]
        public void IFrameRatePreRecordOnIgnitionMode()
        {
            int nPrerecordTime = Convert.ToInt32(this.CFG.gNode("Alarm/IFrameRatePreRecordOnIgnitionMode/prerecord-time").InnerText);

            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                DeviceTime devTime = session.GetDeviceTime();

                //设置录像模式为报警录像 I帧录像模式 设置报警录像参数
                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                JArray jmainArray = new JArray();
                JObject jrp = new JObject();
                JArray jrcpArray = new JArray();

                for (int i = 0; i < session.GetChannelCount(); i++)
                {
                    JObject jrcp = new JObject();
                    jrcp["RM"] = (int)RecordMode.Boot;
                    jrcpArray.Insert(i, jrcp);

                    JObject jmain = new JObject();
                    jmain["VEN"] = 1;
                    jmain["FT"] = (int)EncodeFrameType.IFrame;

                    jmainArray.Insert(i, jmain);
                }

                JArray jiopArray = new JArray();
                for (int i = 0; i < 8; i++)
                {
                    JObject jiop = new JObject();

                    if (i == 0)
                    {
                        jiop["EN"] = 1;
                        jiop["EL"] = 1;
                        jiop["AS"] = 1;

                        JObject japr = new JObject();
                        JObject jar = new JObject();

                        jar["CH"] = 1;
                        jar["P"] = nPrerecordTime;
                        jar["D"] = 60;
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

                jrp["PRE"] = 1;
                jrp["RCP"] = jrcpArray;
                jmdvr["MAIN"] = jmainArray;
                jmdvr["IOP"] = jiopArray;
                jmdvr["RP"] = jrp;
                jparameter["MDVR"] = jmdvr;

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);
                Assert.IsNotNull(jresp["ERRORCODE"]);
                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));

                Sleep(nPrerecordTime * 2 * 1000);

                ToolingSession toolingSesstion = new ToolingSession();
                toolingSesstion.StartSession(m_ToolingIP, m_ToolingPort);


                toolingSesstion.SendMessage("$IO01,01*HH");
                DateTime t2 = devTime.Now;
                Sleep(1000);
                toolingSesstion.SendMessage("$IO01,00*HH");

                DateTime t1 = t2.AddSeconds(0 - nPrerecordTime);

                Sleep(5000);

                int nStreamType = (int)StreamType.MAIN_STREAM;
                int nChannelBits = 1 << 0;



                string starttime = t1.ToString("yyyyMMddHHmmss");
                string endtime = t2.ToString("yyyyMMddHHmmss");

                //查询报警录像
                jparameter = new JObject();
                jparameter["STREAMTYPE"] = nStreamType;
                jparameter["FILETYPE"] = 1 << 1;
                jparameter["CHANNEL"] = nChannelBits;
                jparameter["STARTTIME"] = starttime;
                jparameter["ENDTIME"] = endtime;

                jresp = session.SendCommand(Module.STORM, Operation.QUERYFILELIST, jparameter);

                List<RemoteFileInfo> list = JsonConvert.DeserializeObject<List<RemoteFileInfo>>(jresp["FILELIST"].ToString());
                Assert.IsNotNull(jresp["FILELIST"]);
                Assert.IsTrue(jresp["FILELIST"].HasValues, "获取到的文件列表为空");

                foreach (RemoteFileInfo info in list)
                {
                    Console.WriteLine("[通道 {0}]报警录像文件时间 {1}", info.nChannel, info.GetTimeSpan().TotalSeconds);
                    Console.WriteLine("报警录像预录开始时间应该为{0},实际报警录像预录时间开始为{1}", t1, info.GetStartTime());
                    Assert.AreEqual(info.GetStartTime().ToString("yyyyMMddHHmmss"), t1.ToString("yyyyMMddHHmmss"), "报警录像预录开始时间应该为{0},实际报警录像预录时间开始为{1}", t1, info.GetStartTime());

                    Console.WriteLine("报警录像预录结束时间应该为{0},实际报警录像预录时间结束为{1}", t2, info.GetEndTime());
                    Assert.AreEqual(info.GetEndTime().ToString("yyyyMMddHHmmss"), t2.ToString("yyyyMMddHHmmss"), "报警录像预录结束时间应该为{0},实际报警录像预录时间结束为{1}", t2, info.GetEndTime());
                }
            }
        }

    }
}
