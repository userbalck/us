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

namespace N9MTest.Alarm
{
    [TestFixture]
    class TestCase_AlarmLock:TestCase_Basecase
    {
		
		[Test]
        public void LockAlarmRecord()
        {
            Sleep(60 * 1000);
            int nDuration = Convert.ToInt32(this.CFG.gNode("Alarm/LockAlarmRecord/duration").InnerText);
            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                //开启校时 校准时间
                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();

                JObject jatp = new JObject();
                jatp.Add("GE", 1);
                jatp.Add("NE", 0);
                jatp.Add("CE", 0);
                jmdvr.Add("ATP", jatp);

                jparameter.Add("MDVR", jmdvr);

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"]), "开启GPS校时机制[gps].错误码为{0}", Convert.ToInt32(jresp["ERRORCODE"]));

                Sleep(30 * 1000);

                //关闭校时
                jparameter = new JObject();
                jmdvr = new JObject();

                jatp = new JObject();
                jatp.Add("GE", 0);
                jatp.Add("NE", 0);
                jatp.Add("CE", 0);
                jmdvr.Add("ATP", jatp);

                jparameter.Add("MDVR", jmdvr);

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"]), "关闭所有校时机制[gps. ntp. center server].错误码为{0}", Convert.ToInt32(jresp["ERRORCODE"]));

                //获取设备时间
                DeviceTime devTime = session.GetDeviceTime();

                //设置录像模式为开机录像。设置录像保护时间为一天
                jparameter = new JObject();
                jmdvr = new JObject();
                JObject jrp = new JObject();
                JArray jmpArray = new JArray();

                JArray jrcpArray = new JArray();

                for (int i = 0; i < session.GetChannelCount(); i++)
                {
                    JObject jrcp = new JObject();
                    //设置为开机录像
                    jrcp["RM"] = (int)RecordMode.Boot;
                    jrcpArray.Insert(i, jrcp);
                }

                jrp["RCP"] = jrcpArray;

                JObject jmp = new JObject();
                //设置录像保护时间为1天
                jmp["OLD"] = nDuration;
                jmpArray.Insert((int)RecordMode.Boot, jmp);

                jrp["MP"] = jmpArray;

                //设置IO1高电平，联动通道1录像及加锁，延时录像时间1分钟
                JArray jiopArray = new JArray();

                for (int i = 0; i < 8; i++)
                {
                    JObject jiop = new JObject();

                    if (i == 0)
                    {
                        jiop["EN"] = 1;
                        jiop["EL"] = 1;
                        jiop["AS"] = 1;

                        JObject jap = new JObject();
                        JObject japr = new JObject();
                        JObject jar = new JObject();
                        jar["CH"] = 0xffffffff;
                        jar["P"] = 0;
                        jar["D"] = 60;
                        jar["L"] = 1;

                        japr["AR"] = jar;
                        jiop["APR"] = japr;

                    }
                    else
                    {
                        jiop["EN"] = 0;
                    }

                    jiopArray.Insert(i, jiop);
                }
                jmdvr["RP"] = jrp;
                jmdvr["IOP"] = jiopArray;
                jparameter["MDVR"] = jmdvr;

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);
                Assert.IsNotNull(jresp["ERRORCODE"]);
                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));

                //下发IO报警
                ToolingSession toolingSession = new ToolingSession();
                toolingSession.StartSession(m_ToolingIP, m_ToolingPort);

                DateTime t1 = devTime.Now;
                toolingSession.SendMessage("$IO01,01*HH");
                Sleep(1000);
                toolingSession.SendMessage("$IO01,00*HH");
                
                Sleep(60 * 1000);

                //检索是否形成加锁录像
                DateTime dtStartTime = t1;
                DateTime dtEndTime = t1.AddMinutes(1);

                int nStreamType = (int)StreamType.MAIN_STREAM;
                uint nChannelBits = 0xffffffff;

                string starttime = dtStartTime.ToString("yyyyMMddHHmmss");
                string endtime = dtEndTime.ToString("yyyyMMddHHmmss");

                jparameter = new JObject();
                jparameter["STREAMTYPE"] = nStreamType;
                jparameter["FILETYPE"] = 0xffffffff;
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
                    Assert.IsTrue(((info.nFileType >> 6) & 0x01) == 1, "[通道{0}] 没有被加锁");
                }

                DateTime tt = devTime.Now.AddDays(1);

                DateTime dtSetting = new DateTime(tt.Year, tt.Month, tt.Day, 23, 59, 50);
                jparameter = new JObject();
                jparameter.Add("CURT", (int)(dtSetting - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);

                jresp = session.SendCommand(Module.DEVEMM, Operation.SETCTRLUTC, jparameter);
                Sleep(90 * 1000);

                jparameter = new JObject();
                jparameter["STREAMTYPE"] = nStreamType;
                jparameter["FILETYPE"] = 0xffffffff;
                jparameter["CHANNEL"] = nChannelBits;
                jparameter["STARTTIME"] = starttime;
                jparameter["ENDTIME"] = endtime;

                jresp = session.SendCommand(Module.STORM, Operation.QUERYFILELIST, jparameter);

                 list = JsonConvert.DeserializeObject<List<RemoteFileInfo>>(jresp["FILELIST"].ToString());
                Assert.IsNotNull(jresp["FILELIST"]);
                Assert.IsTrue(jresp["FILELIST"].HasValues, "获取到的文件列表为空");

                foreach (RemoteFileInfo info in list)
                {
                    Console.WriteLine("[通道 {0}]报警录像文件时间 {1}", info.nChannel, info.GetTimeSpan().TotalSeconds);
                    Assert.IsTrue(((info.nFileType >> 6) & 0x01) == 0, "[通道{0}] 没有被解锁");
                }


                jparameter = new JObject();
                jmdvr = new JObject();

                jatp = new JObject();
                jatp.Add("GE", 1);
                jatp.Add("NE", 0);
                jatp.Add("CE", 0);
                jmdvr.Add("ATP", jatp);

                jparameter.Add("MDVR", jmdvr);

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"]), "开启GPS校时机制[gps].错误码为{0}", Convert.ToInt32(jresp["ERRORCODE"]));

            }
        }
    }
}
