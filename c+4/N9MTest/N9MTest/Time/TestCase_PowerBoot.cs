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

namespace N9MTest.Time
{
    [TestFixture]
    class TestCase_PowerBoot: TestCase_Basecase
    {
        [Test]
        public void TimeOff()
        {
            foreach (string ip in IPList)
            {
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

                jssp.Add("UPT", 1);
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
                    TimeSpan ts = info.GetDateTime() - time;

                    if (info.logContent.Equals("定时关机") && ts.TotalSeconds < 120)
                    {
                        dt = new DateTime[1];
                        dt[0] = new DateTime();
                        dt[0] = info.GetDateTime();
                        break;
                    }
                }

                Assert.AreNotEqual(null, dt);

                int nStreamType = (int)StreamType.MAIN_STREAM;
                uint nChannelBits = 0xffffff;
                string st = dt[0].AddMinutes(-1).ToString("yyyyMMddHHmmss");
                string et = dt[0].AddSeconds(5).ToString("yyyyMMddHHmmss");

                session = new N9MSession(ip, port);
                session.Login(username, password);

                JObject jparamater = new JObject();
                jparamater.Add("STREAMTYPE", nStreamType);
                jparamater.Add("FILETYPE", 0xffffffff);
                jparamater.Add("CHANNEL", nChannelBits);
                jparamater.Add("STARTTIME", st);
                jparamater.Add("ENDTIME", et);

                jresp = session.SendCommand(Module.STORM, Operation.QUERYFILELIST, jparamater);

                Assert.IsNotNull(jresp["FILELIST"]);

                List<RemoteFileInfo> filelist = JsonConvert.DeserializeObject<List<RemoteFileInfo>>(jresp["FILELIST"].ToString());


                //测试完毕以后 把开关机模式改为点火模式
                jparameter = new JObject();
                jmdvr = new JObject();
                jssp = new JObject();

                jssp.Add("UPT", 0);
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
                //下发重启指令
                N9MSession session = new N9MSession(ip, port);
                do
                {
                    Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                    JObject jparamater = new JObject();
                    jparamater["CMDTYPE"] = 0;

                    JObject jresp = session.SendCommand(Module.DEVEMM, Operation.SETCONTROLDEVCMD, jparamater);
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
    }
}
