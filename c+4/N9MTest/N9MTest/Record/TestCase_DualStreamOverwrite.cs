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
using Util;

namespace N9MTest.Record
{
    [TestFixture]
    class TestCase_DualStreamOverwrite:TestCase_Basecase
    {
		//【硬盘双录像】使能关闭，设置覆盖模式
		[Ignore("111")]
		[Test]
		public void HDDSingleStreamCapacityOverwrite()
		{
			//查看录像覆盖次数
			int nCount = Convert.ToInt32(this.CFG.gNode("Record/HDDSingleStreamCapacityOverwrite/count").InnerText);

            //获取超时时间
            int nTimeOut = Convert.ToInt32(this.CFG.gNode("Record/HDDSingleStreamCapacityOverwrite/timeout").InnerText);


            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                //首先获取主码流的参数
                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                jmdvr.Add("MAIN", "?");
                jparameter.Add("MDVR", jmdvr);

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);
                Assert.IsNotNull(jresp["MDVR"]);
                Assert.IsNotNull(jresp["MDVR"]["MAIN"]);

                List<NetStreamEncoderInfo> list = JsonConvert.DeserializeObject<List<NetStreamEncoderInfo>>(jresp["MDVR"]["MAIN"].ToString());

                //开启所有通道主码流 关闭所有的辅助码流
                jparameter = new JObject();
                jmdvr = new JObject();

                JArray jnecArray = new JArray();

                for(int i = 0; i < session.GetChannelCount(); i++)
                {
                    JObject jnec = new JObject();
                    jnec["VEN"] = 1;

                    jnecArray.Add(jnec);
                }

                JArray jvecArray = new JArray();
                JObject jar = new JObject();

                for (int i = 0; i < session.GetChannelCount(); i++)
                {
                    JObject jvec = new JObject();
                    jvec["VEN"] = 0;

                    jvecArray.Add(jvec);
                }

                jar.Add("VEC", jvecArray);
                jmdvr.Add("AR", jar);
                jmdvr.Add("MAIN", jnecArray);

                jparameter.Add("MDVR", jmdvr);

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                //打开录像覆盖开关
                jmdvr = new JObject();
                jparameter = new JObject();
                JObject jrp = new JObject();
                JArray jmpArray = new JArray();

                JObject jmp = new JObject();
                jmp["OT"] = (int)OverwriteMode.Capacity;
                jmpArray.Insert((int)RecordMode.Boot, jmp);
                jrp["MP"] = jmpArray;
                jmdvr["RP"] = jrp;
                jparameter["MDVR"] = jmdvr;

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                session.Logout();

                N9MHttpSession httpSession = new N9MHttpSession(ip, webport);
                httpSession.Login("/logincheck.rsp?type=1", username, password);

                int nTrigger = 0;
                long lLastLeftSize = -1;

                long starttime = ExactTime.GetExactTime();

                do
                {
                    Assert.IsFalse(ExactTime.GetExactTime() - starttime >= nTimeOut, "等待时间超时，设定等待时间为{0}, 实际等待时间为{1}", nTimeOut, ExactTime.GetExactTime() - starttime);

                    string resp = httpSession.Get("/device.rsp?opt=hdd&cmd=state");
                    jresp = JObject.Parse(resp);

                    Assert.AreEqual(0, Convert.ToInt32(jresp["result"].ToString()));

                    List<StorageInfo> storageList = JsonConvert.DeserializeObject<List<StorageInfo>>(jresp["data"].ToString());

                    Console.WriteLine("storageList.count = {0}", storageList.Count);

                    long lLeftSize = 0;

                    foreach (StorageInfo info in storageList)
                    {
                        if (info.type != (int)StorageType.STORAGE_DEFAULT_EXTERNAL)
                        {
                            continue;
                        }

                        lLeftSize += info.left;
                    }

                    Assert.Less(lLeftSize, (long)10 * 1024 * 1024 * 1024, "剩余空间{0}大于10G 按照判定条件，需要判定为失败", lLeftSize);

                    if (lLastLeftSize == -1)
                    {
                        lLastLeftSize = lLeftSize;
                    }
                    else
                    {
                        if (lLastLeftSize < lLeftSize)
                        {
                            lLastLeftSize = lLeftSize;
                            nTrigger++;

                            Console.WriteLine("[0]发现第{1}次HDD容量的自动覆盖", DateTime.Now.ToString(), nTrigger);

                            if (nTrigger >= nCount)
                            {
                                break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("[0]当前检测不符合HDD自动覆盖的条件lLastLeftSize = {0}, lLeftSize = {1}", DateTime.Now.ToString(), lLastLeftSize, lLeftSize);
                        }
                    }

                    Sleep(30 * 1000);

                } while (true);
            }
        }

        //【硬盘双录像】使能关闭，设置覆盖模式
		[Ignore("1111111111")]
        [Test]
        public void HDDSingleStreamDayOverwrite()
        {
            //查看录像覆盖次数
            int nCount = Convert.ToInt32(this.CFG.gNode("Record/HDDSingleStreamDayOverwrite/count").InnerText);

            //超时时间
            int nTimeOut = Convert.ToInt32(this.CFG.gNode("Record/HDDSingleStreamDayOverwrite/timeout").InnerText);

            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                //首先获取主码流的参数
                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                jmdvr.Add("MAIN", "?");
                jparameter.Add("MDVR", jmdvr);

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);
                Assert.IsNotNull(jresp["MDVR"]);
                Assert.IsNotNull(jresp["MDVR"]["MAIN"]);

                List<NetStreamEncoderInfo> list = JsonConvert.DeserializeObject<List<NetStreamEncoderInfo>>(jresp["MDVR"]["MAIN"].ToString());

                //开启所有通道主码流 关闭所有的辅助码流
                jparameter = new JObject();
                jmdvr = new JObject();

                JArray jnecArray = new JArray();

                for (int i = 0; i < session.GetChannelCount(); i++)
                {
                    JObject jnec = new JObject();
                    jnec["VEN"] = 1;

                    jnecArray.Add(jnec);
                }

                JArray jvecArray = new JArray();
                JObject jar = new JObject();

                for (int i = 0; i < session.GetChannelCount(); i++)
                {
                    JObject jvec = new JObject();
                    jvec["VEN"] = 0;

                    jvecArray.Add(jvec);
                }

                jar.Add("VEC", jvecArray);
                jmdvr.Add("AR", jar);
                jmdvr.Add("MAIN", jnecArray);

                jparameter.Add("MDVR", jmdvr);

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                //打开录像覆盖开关
                jmdvr = new JObject();
                jparameter = new JObject();
                JObject jrp = new JObject();
                JArray jmpArray = new JArray();

                JObject jmp = new JObject();
                jmp["OT"] = (int)OverwriteMode.Days;
                jmpArray.Insert((int)RecordMode.Boot, jmp);
                jrp["MP"] = jmpArray;
                jmdvr["RP"] = jrp;
                jparameter["MDVR"] = jmdvr;

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                N9MHttpSession httpSession = new N9MHttpSession(ip, webport);
                httpSession.Login("/logincheck.rsp?type=1", username, password);

                int nTrigger = 0;

                List<CalendarData> CalendarList = null;

                long starttime = ExactTime.GetExactTime();

                //每隔30秒检测下录像的月历时间
                do
                {
                    Assert.IsFalse(ExactTime.GetExactTime() - starttime >= nTimeOut, "等待时间超时，设定等待时间为{0}, 实际等待时间为{1}", nTimeOut, ExactTime.GetExactTime() - starttime);

                    string resp = httpSession.Get("/device.rsp?opt=hdd&cmd=state");
                    jresp = JObject.Parse(resp);

                    Assert.AreEqual(0, Convert.ToInt32(jresp["result"].ToString()));

                    List<StorageInfo> storageList = JsonConvert.DeserializeObject<List<StorageInfo>>(jresp["data"].ToString());

                    Console.WriteLine("storageList.count = {0}", storageList.Count);

                    long lLeftSize = 0;

                    foreach (StorageInfo info in storageList)
                    {
                        if (info.type != (int)StorageType.STORAGE_DEFAULT_EXTERNAL)
                        {
                            continue;
                        }

                        lLeftSize += info.left;
                    }

                    Assert.Less(lLeftSize, (long)10 * 1024 * 1024 * 1024, "剩余空间{0}大于10G 按照判定条件，需要判定为失败", lLeftSize);

                    //获取下所有月历
                    jparameter = new JObject();
                    jparameter["CALENDARTYPE"] = 2;
                    jparameter["STREAMTYPE"] = (int)StreamType.MAIN_STREAM;
                    jparameter["FILETYPE"] = 0xffffffff;
                    jparameter["CHANNEL"] = 0xffffffff;

                    jresp = session.SendCommand(Module.STORM, Operation.GETCALENDAR, jparameter);
                    Assert.IsNotNull(jresp);
                    Assert.IsNotNull(jresp["LIST"]);

                    List<CalendarData> callist = JsonConvert.DeserializeObject<List<CalendarData>>(jresp["LIST"].ToString());

                    callist.Sort(CalendarData.Sort);

                    if (CalendarList == null)
                    {
                        CalendarList = callist;
                    }
                    else
                    {
                        if (false == CalendarList[0].isEqualCalendar(callist[0]))
                        {
                            nTrigger++;

                            DateTime dt = new DateTime(CalendarList[0].nYear, CalendarList[0].nMonth, CalendarList[0].nDay);

                            Console.WriteLine("[0]发现第{1}次按天自动覆盖{2}", DateTime.Now.ToString(), nTrigger, dt.ToString());

                            if (nTrigger == nCount)
                            {
                                break;
                            }
                            else
                            {
                                CalendarList = callist;
                            }
                        }
                        else
                        {
                            DateTime dt = new DateTime(CalendarList[0].nYear, CalendarList[0].nMonth, CalendarList[0].nDay);
                            Console.WriteLine("{0}当前未检测到按天覆盖{1}", DateTime.Now.ToString(), dt.ToString());
                        }
                    }

                    Sleep(30 * 1000);

                } while (true);

            }
        }

		//【硬盘双录像】使能关闭，设置覆盖模式
		[Ignore("11111111")]
		[Test]
        public void HDDSingleStreamNoOverwrite()
        {
            //超时时间
            int nTimeOut = Convert.ToInt32(this.CFG.gNode("Record/HDDSingleStreamNoOverwrite/timeout").InnerText);


            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                //首先获取主码流的参数
                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                jmdvr.Add("MAIN", "?");
                jparameter.Add("MDVR", jmdvr);

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);
                Assert.IsNotNull(jresp["MDVR"]);
                Assert.IsNotNull(jresp["MDVR"]["MAIN"]);

                List<NetStreamEncoderInfo> list = JsonConvert.DeserializeObject<List<NetStreamEncoderInfo>>(jresp["MDVR"]["MAIN"].ToString());

                //开启所有通道主码流 关闭所有的辅助码流
                jparameter = new JObject();
                jmdvr = new JObject();

                JArray jnecArray = new JArray();

                for (int i = 0; i < session.GetChannelCount(); i++)
                {
                    JObject jnec = new JObject();
                    jnec["VEN"] = 1;

                    jnecArray.Add(jnec);
                }

                JArray jvecArray = new JArray();
                JObject jar = new JObject();

                for (int i = 0; i < session.GetChannelCount(); i++)
                {
                    JObject jvec = new JObject();
                    jvec["VEN"] = 0;

                    jvecArray.Add(jvec);
                }

                jar.Add("VEC", jvecArray);
                jmdvr.Add("AR", jar);
                jmdvr.Add("MAIN", jnecArray);

                jparameter.Add("MDVR", jmdvr);

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                //打开录像覆盖开关
                jmdvr = new JObject();
                jparameter = new JObject();
                JObject jrp = new JObject();
                JArray jmpArray = new JArray();

                JObject jmp = new JObject();
                jmp["OT"] = (int)OverwriteMode.Never;
                jmpArray.Insert((int)RecordMode.Boot, jmp);
                jrp["MP"] = jmpArray;
                jmdvr["RP"] = jrp;
                jparameter["MDVR"] = jmdvr;

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                session.Logout();

                N9MHttpSession httpSession = new N9MHttpSession(ip, webport);
                httpSession.Login("/logincheck.rsp?type=1", username, password);

                long starttime = ExactTime.GetExactTime();

                do
                {
                    Assert.IsFalse(ExactTime.GetExactTime() - starttime >= nTimeOut, "等待时间超时，设定等待时间为{0}, 实际等待时间为{1}", nTimeOut, ExactTime.GetExactTime() - starttime);

                    string resp = httpSession.Get("/device.rsp?opt=hdd&cmd=state");
                    jresp = JObject.Parse(resp);

                    Assert.AreEqual(0, Convert.ToInt32(jresp["result"].ToString()));

                    List<StorageInfo> storageList = JsonConvert.DeserializeObject<List<StorageInfo>>(jresp["data"].ToString());

                    Console.WriteLine("storageList.count = {0}", storageList.Count);

                    long lLeftSize = 0;

                    foreach (StorageInfo info in storageList)
                    {
                        if (info.type != (int)StorageType.STORAGE_DEFAULT_EXTERNAL)
                        {
                            continue;
                        }

                        lLeftSize += info.left;
                    }

                    Assert.Less(lLeftSize, (long)10 * 1024 * 1024 * 1024, "剩余空间{0}大于10G 按照判定条件，需要判定为失败", lLeftSize);

                    if (lLeftSize == 0)
                    {
                        for (int i = 0; i < session.GetChannelCount(); i++)
                        {
                            Assert.IsFalse(session.isChannelRecording(StreamType.MAIN_STREAM, i));
                        }
                    }

                    Sleep(30 * 1000);

                } while (true);
            }
        }

        //【硬盘双录像】使能打开，超过主码流时长后就开始覆盖
		[Ignore("11111")]
        [Test]
        public void HDDMainStreamOverwrite()
        {
            //主码流录像时长
            int nDuration = Convert.ToInt32(this.CFG.gNode("Record/HDDMainStreamOverwrite/duration").InnerText);


            //需要复现的次数
            int nCount = Convert.ToInt32(this.CFG.gNode("Record/HDDMainStreamOverwrite/count").InnerText);

            //超时时间
            int nTimeOut = Convert.ToInt32(this.CFG.gNode("Record/HDDMainStreamOverwrite/timeout").InnerText);

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
                jmdvr.Add("RP", "?");
                jmdvr.Add("MAIN", "?");
                jparameter.Add("MDVR", jmdvr);

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);
                Assert.IsNotNull(jresp["MDVR"]);
                Assert.IsNotNull(jresp["MDVR"]["MAIN"]);
                Assert.IsNotNull(jresp["MDVR"]["AR"]);

                jparameter = new JObject();
                jmdvr = new JObject();
                JObject jar = new JObject();
                jar["ALH"] = nDuration;
                jmdvr["AR"] = jar;
                jparameter["MDVR"] = jmdvr;

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                session.Logout();

                N9MHttpSession httpSession = new N9MHttpSession(ip, webport);
                httpSession.Login("/logincheck.rsp?type=1", username, password);

                int nTrigger = 0;
                long lLastLeftSize = -1;

                do
                {
                    string resp = httpSession.Get("/device.rsp?opt=hdd&cmd=state");
                    jresp = JObject.Parse(resp);

                    Assert.AreEqual(0, Convert.ToInt32(jresp["result"].ToString()));

                    List<StorageInfo> storageList = JsonConvert.DeserializeObject<List<StorageInfo>>(jresp["data"].ToString());

                    Console.WriteLine("storageList.count = {0}", storageList.Count);

                    long lLeftSize = 0;

                    foreach (StorageInfo info in storageList)
                    {
                        if (info.type != (int)StorageType.STORAGE_DEFAULT_EXTERNAL)
                        {
                            continue;
                        }

                        lLeftSize += info.left;
                    }

                    Assert.Less(lLeftSize, (long)10 * 1024 * 1024 * 1024, "剩余空间{0}大于10G 按照判定条件，需要判定为失败", lLeftSize);

                    if (lLastLeftSize == -1)
                    {
                        lLastLeftSize = lLeftSize;
                    }
                    else
                    {
                        if (lLastLeftSize < lLeftSize)
                        {
                            lLastLeftSize = lLeftSize;
                            nTrigger++;

                            Console.WriteLine("[0]发现第{1}次按照容量的自动覆盖", DateTime.Now.ToString(), nTrigger);

                            if (nTrigger >= nCount)
                            {
                                break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("[0]当前检测不符合自动覆盖的条件lLastLeftSize = {0}, lLeftSize = {1}", DateTime.Now.ToString(), lLastLeftSize, lLeftSize);
                        }
                    }

                    Sleep(30 * 1000);

                } while (true);
            }
        }
		[Ignore("11111")]
		//【硬盘双录像】使能打开，超过子码流时长后就开始覆盖
		[Test]
        public void HDDSubStreamOverwrite()
        {
            //主码流录像的时长
            int nDuration = Convert.ToInt32(this.CFG.gNode("Record/HDDSubStreamOverwrite/duration").InnerText);

            //需要检测到覆盖的次数
            int nCount = Convert.ToInt32(this.CFG.gNode("Record/HDDSubStreamOverwrite/count").InnerText);
            //超时时间
            int nTimeOut = Convert.ToInt32(this.CFG.gNode("Record/HDDSubStreamOverwrite/timeout").InnerText);

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

                jparameter = new JObject();
                jmdvr = new JObject();
                JObject jar = new JObject();
                jar["ALH"] = nDuration;
                jmdvr["AR"] = jar;
                jparameter["MDVR"] = jmdvr;

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                session.Logout();

                N9MHttpSession httpSession = new N9MHttpSession(ip, webport);
                httpSession.Login("/logincheck.rsp?type=1", username, password);

                int nTrigger = 0;
                long lLastLeftSize = -1;

                do
                {
                    string resp = httpSession.Get("/device.rsp?opt=hdd&cmd=state");
                    jresp = JObject.Parse(resp);

                    Assert.AreEqual(0, Convert.ToInt32(jresp["result"].ToString()));

                    List<StorageInfo> storageList = JsonConvert.DeserializeObject<List<StorageInfo>>(jresp["data"].ToString());

                    Console.WriteLine("storageList.count = {0}", storageList.Count);

                    long lLeftSize = 0;

                    foreach (StorageInfo info in storageList)
                    {
                        if (info.type != (int)StorageType.STORAGE_DEFAULT_EXTERNAL)
                        {
                            continue;
                        }

                        lLeftSize += info.left;
                    }

                    Assert.Less(lLeftSize, (long)10 * 1024 * 1024 * 1024, "剩余空间{0}大于10G 按照判定条件，需要判定为失败", lLeftSize);

                    if (lLastLeftSize == -1)
                    {
                        lLastLeftSize = lLeftSize;
                    }
                    else
                    {
                        if (lLastLeftSize < lLeftSize)
                        {
                            lLastLeftSize = lLeftSize;
                            nTrigger++;

                            Console.WriteLine("[0]发现第{1}次按照容量的自动覆盖", DateTime.Now.ToString(), nTrigger);

                            if (nTrigger >= nCount)
                            {
                                break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("[0]当前检测不符合自动覆盖的条件lLastLeftSize = {0}, lLeftSize = {1}", DateTime.Now.ToString(), lLastLeftSize, lLeftSize);
                        }
                    }

                    Sleep(30 * 1000);

                } while (true);
            }
        }
		[Ignore("11111")]
		//【硬盘双录像】使能打开，加锁录像
		[Test]
        public void HDDSubStreamLockedOverwrite()
        {
            int nDuration = Convert.ToInt32(this.CFG.gNode("Record/HDDSubStreamLockedOverwrite/lock-duration").InnerText);
            int nCount = Convert.ToInt32(this.CFG.gNode("Record/HDDSubStreamLockedOverwrite/count").InnerText);

            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();

                jmdvr["IOP"] = "?";
                jmdvr["RP"] = "?";
                jparameter["MDVR"] = jmdvr;

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);

                jparameter = new JObject();
                jmdvr = new JObject();
                JArray jiopArray = new JArray();

                JObject jiop = new JObject();
                jiop["EN"] = 1;
                jiop["EL"] = 0;

                JObject japr = new JObject();
                JObject jar = new JObject();
                jar["P"] = 0;
                jar["CH"] = 0xffffffff;
                jar["D"] = 3600 * 24;
                jar["L"] = 1;
                japr["AR"] = jar;
                jiop["APR"] = japr;

                jiopArray.Insert(0, jiop);
                jmdvr["IOP"] = jiopArray;

                JObject jrp = new JObject();
                JArray jmpArray = new JArray();
                JObject jmp = new JObject();

                jmp["OLD"] = nDuration;
                jmpArray.Insert((int)RecordMode.Boot, jmp);
                jrp["MP"] = jmpArray;
                jmdvr["RP"] = jrp;
                jparameter["MDVR"] = jmdvr;

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                N9MHttpSession httpSession = new N9MHttpSession(ip, webport);
                httpSession.Login("/logincheck.rsp?type=1", username, password);

                int nTrigger = 0;

                List<CalendarData> CalendarList = null;

                //每隔30秒检测下录像的月历时间
                do
                {
                    string resp = httpSession.Get("/device.rsp?opt=hdd&cmd=state");
                    jresp = JObject.Parse(resp);

                    Assert.AreEqual(0, Convert.ToInt32(jresp["result"].ToString()));

                    List<StorageInfo> storageList = JsonConvert.DeserializeObject<List<StorageInfo>>(jresp["data"].ToString());

                    Console.WriteLine("storageList.count = {0}", storageList.Count);

                    long lLeftSize = 0;

                    foreach (StorageInfo info in storageList)
                    {
                        if (info.type != (int)StorageType.STORAGE_DEFAULT_EXTERNAL)
                        {
                            continue;
                        }

                        lLeftSize += info.left;
                    }

                    Assert.Less(lLeftSize, (long)10 * 1024 * 1024 * 1024, "剩余空间{0}大于10G 按照判定条件，需要判定为失败", lLeftSize);

                    //获取下所有月历
                    jparameter = new JObject();
                    jparameter["CALENDARTYPE"] = 2;
                    jparameter["STREAMTYPE"] = (int)StreamType.MAIN_STREAM;
                    jparameter["FILETYPE"] = (int)FileType.CALENDAR_FILETYPE_LOCK;
                    jparameter["CHANNEL"] = 0xffffffff;
                    jparameter["RFSTORAGE"] = (int)ReferStorage.REFER_STORAGE_HDD;

                    jresp = session.SendCommand(Module.STORM, Operation.GETCALENDAR, jparameter);
                    Assert.IsNotNull(jresp);
                    Assert.IsNotNull(jresp["LIST"]);

                    List<CalendarData> callist = JsonConvert.DeserializeObject<List<CalendarData>>(jresp["LIST"].ToString());

                    callist.Sort(CalendarData.Sort);

                    for (int i = callist.Count() - 1; i >= 0; i--)
                    {
                        int property = callist[i].nProperty;

                        if ((property & (int)FileType.CALENDAR_FILETYPE_LOCK) == 0)
                        {
                            callist.RemoveAt(i);
                        }
                    }

                    if (CalendarList == null)
                    {
                        CalendarList = callist;
                    }
                    else
                    {
                        if (false == CalendarList[0].isEqualCalendar(callist[0]))
                        {
                            nTrigger++;

                            DateTime dt = new DateTime(CalendarList[0].nYear, CalendarList[0].nMonth, CalendarList[0].nDay);

                            Console.WriteLine("[0]发现第{1}次HDD解锁{2}", DateTime.Now.ToString(), nTrigger, dt.ToString());

                            if (nTrigger == nCount)
                            {
                                break;
                            }
                            else
                            {
                                CalendarList = callist;
                            }
                        }
                        else
                        {
                            DateTime dt = new DateTime(CalendarList[0].nYear, CalendarList[0].nMonth, CalendarList[0].nDay);
                            Console.WriteLine("{0}当前未检测到HDD解锁{1}", DateTime.Now.ToString(), dt.ToString());
                        }
                    }

                    Sleep(30 * 1000);

                } while (true);
            }
        }
		[Ignore("11111")]
		//开启主码流或子码流录像后，检查覆盖方式
		[Test]
        public void SDSingleStreamCapacityOverwrite()
        {
            //查看录像覆盖次数
            int nCount = Convert.ToInt32(this.CFG.gNode("Record/HDDSingleStreamCapacityOverwrite/count").InnerText);

            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                //首先获取主码流的参数
                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                jmdvr.Add("AR", "?");
                jmdvr.Add("MAIN", "?");
                jparameter.Add("MDVR", jmdvr);

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);
                Assert.IsNotNull(jresp["MDVR"]);
                Assert.IsNotNull(jresp["MDVR"]["MAIN"]);
                Assert.IsNotNull(jresp["MDVR"]["AR"]);

                List<NetStreamEncoderInfo> list = JsonConvert.DeserializeObject<List<NetStreamEncoderInfo>>(jresp["MDVR"]["MAIN"].ToString());

                //开启所有通道主码流 并且开启子码流录像开关
                jparameter = new JObject();
                jmdvr = new JObject();

                JArray jnecArray = new JArray();

                for (int i = 0; i < session.GetChannelCount(); i++)
                {
                    JObject jnec = new JObject();
                    jnec["VEN"] = 1;

                    jnecArray.Add(jnec);
                }

                JArray jvecArray = new JArray();
                JObject jar = new JObject();

                jar["EN"] = 1;

                for (int i = 0; i < session.GetChannelCount(); i++)
                {
                    JObject jvec = new JObject();
                    jvec["VEN"] = 1;

                    jvecArray.Add(jvec);
                }

                jar.Add("VEC", jvecArray);
                jmdvr.Add("AR", jar);
                jmdvr.Add("MAIN", jnecArray);

                jparameter.Add("MDVR", jmdvr);

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                //打开录像覆盖开关
                jmdvr = new JObject();
                jparameter = new JObject();
                JObject jrp = new JObject();
                JArray jmpArray = new JArray();

                JObject jmp = new JObject();
                jmp["OT"] = (int)OverwriteMode.Capacity;
                jmpArray.Insert((int)RecordMode.Boot, jmp);
                jrp["MP"] = jmpArray;
                jmdvr["RP"] = jrp;
                jparameter["MDVR"] = jmdvr;

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                session.Logout();

                N9MHttpSession httpSession = new N9MHttpSession(ip, webport);
                httpSession.Login("/logincheck.rsp?type=1", username, password);

                int nTrigger = 0;
                long lLastLeftSize = -1;

                do
                {
                    string resp = httpSession.Get("/device.rsp?opt=hdd&cmd=state");
                    jresp = JObject.Parse(resp);

                    Assert.AreEqual(0, Convert.ToInt32(jresp["result"].ToString()));

                    List<StorageInfo> storageList = JsonConvert.DeserializeObject<List<StorageInfo>>(jresp["data"].ToString());

                    Console.WriteLine("storageList.count = {0}", storageList.Count);

                    long lLeftSize = 0;

                    foreach (StorageInfo info in storageList)
                    {
                        if (info.type != (int)StorageType.STORAGE_INTERNAL_SDCARD)
                        {
                            continue;
                        }

                        lLeftSize += info.left;
                    }

                    Assert.Less(lLeftSize, (long)10 * 1024 * 1024 * 1024, "剩余空间{0}大于10G 按照判定条件，需要判定为失败", lLeftSize);

                    if (lLastLeftSize == -1)
                    {
                        lLastLeftSize = lLeftSize;
                    }
                    else
                    {
                        if (lLastLeftSize < lLeftSize)
                        {
                            lLastLeftSize = lLeftSize;
                            nTrigger++;

                            Console.WriteLine("[0]发现第{1}次按照SD卡容量的自动覆盖", DateTime.Now.ToString(), nTrigger);

                            if (nTrigger >= nCount)
                            {
                                break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("[0]当前检测不符合SD卡自动覆盖的条件lLastLeftSize = {0}, lLeftSize = {1}", DateTime.Now.ToString(), lLastLeftSize, lLeftSize);
                        }
                    }

                    Sleep(30 * 1000);

                } while (true);
            }
        }
		[Ignore("11111")]
		//开启主码流或子码流录像后，检查覆盖方式
		[Test]
        public void SDSingleStreamLockedOverwrite()
        {
            int nDuration = Convert.ToInt32(this.CFG.gNode("Record/SDSingleStreamLockedOverwrite/lock-duration").InnerText);
            int nCount = Convert.ToInt32(this.CFG.gNode("Record/SDSingleStreamLockedOverwrite/count").InnerText);

            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();

                jmdvr["IOP"] = "?";
                jmdvr["RP"] = "?";
                jparameter["MDVR"] = jmdvr;

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);

                jparameter = new JObject();
                jmdvr = new JObject();
                JArray jiopArray = new JArray();

                JObject jiop = new JObject();
                jiop["EN"] = 1;
                jiop["EL"] = 0;

                JObject japr = new JObject();
                JObject jar = new JObject();
                jar["P"] = 0;
                jar["CH"] = 0xffffffff;
                jar["D"] = 3600 * 24;
                jar["L"] = 1;
                japr["AR"] = jar;
                jiop["APR"] = japr;

                jiopArray.Insert(0, jiop);
                jmdvr["IOP"] = jiopArray;

                JObject jrp = new JObject();
                JArray jmpArray = new JArray();
                JObject jmp = new JObject();

                jmp["OLD"] = nDuration;
                jmpArray.Insert((int)RecordMode.Boot, jmp);
                jrp["MP"] = jmpArray;
                jmdvr["RP"] = jrp;
                jparameter["MDVR"] = jmdvr;

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                N9MHttpSession httpSession = new N9MHttpSession(ip, webport);
                httpSession.Login("/logincheck.rsp?type=1", username, password);

                int nTrigger = 0;

                List<CalendarData> CalendarList = null;

                //每隔30秒检测下录像的月历时间
                do
                {
                    string resp = httpSession.Get("/device.rsp?opt=hdd&cmd=state");
                    jresp = JObject.Parse(resp);

                    Assert.AreEqual(0, Convert.ToInt32(jresp["result"].ToString()));

                    List<StorageInfo> storageList = JsonConvert.DeserializeObject<List<StorageInfo>>(jresp["data"].ToString());

                    Console.WriteLine("storageList.count = {0}", storageList.Count);

                    long lLeftSize = 0;

                    foreach (StorageInfo info in storageList)
                    {
                        if (info.type == (int)StorageType.STORAGE_DEFAULT_EXTERNAL)
                        {
                            continue;
                        }

                        lLeftSize += info.left;
                    }

                    Assert.Less(lLeftSize, (long)10 * 1024 * 1024 * 1024, "剩余空间{0}大于10G 按照判定条件，需要判定为失败", lLeftSize);

                    //获取下所有月历
                    jparameter = new JObject();
                    jparameter["CALENDARTYPE"] = 2;
                    jparameter["STREAMTYPE"] = (int)StreamType.SUB_STREAM;
                    jparameter["FILETYPE"] = (int)FileType.CALENDAR_FILETYPE_LOCK;
                    jparameter["CHANNEL"] = 0xffffffff;
                    jparameter["RFSTORAGE"] = (int)ReferStorage.REFER_STORAGE_SD;

                    jresp = session.SendCommand(Module.STORM, Operation.GETCALENDAR, jparameter);
                    Assert.IsNotNull(jresp);
                    Assert.IsNotNull(jresp["LIST"]);

                    List<CalendarData> callist = JsonConvert.DeserializeObject<List<CalendarData>>(jresp["LIST"].ToString());

                    callist.Sort(CalendarData.Sort);

                    for (int i = callist.Count() - 1; i >= 0; i--)
                    {
                        int property = callist[i].nProperty;

                        if ((property & (int)FileType.CALENDAR_FILETYPE_LOCK) == 0)
                        {
                            callist.RemoveAt(i);
                        }
                    }

                    if (CalendarList == null)
                    {
                        CalendarList = callist;
                    }
                    else
                    {
                        if (false == CalendarList[0].isEqualCalendar(callist[0]))
                        {
                            nTrigger++;

                            DateTime dt = new DateTime(CalendarList[0].nYear, CalendarList[0].nMonth, CalendarList[0].nDay);

                            Console.WriteLine("[0]发现第{1}次SD解锁{2}", DateTime.Now.ToString(), nTrigger, dt.ToString());

                            if (nTrigger == nCount)
                            {
                                break;
                            }
                            else
                            {
                                CalendarList = callist;
                            }
                        }
                        else
                        {
                            DateTime dt = new DateTime(CalendarList[0].nYear, CalendarList[0].nMonth, CalendarList[0].nDay);
                            Console.WriteLine("{0}当前未检测到SD解锁{1}", DateTime.Now.ToString(), dt.ToString());
                        }
                    }

                    Sleep(30 * 1000);

                } while (true);
            }
        }
    }
}
