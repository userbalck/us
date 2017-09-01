using System;
using NUnit.Framework;
using System.Collections.Generic;
using N9MTest.SDK.net;
using Newtonsoft.Json.Linq;
using System.IO;
using static N9MTest.SDK.net.BlackBox;

namespace N9MTest.GPS
{
	
	[TestFixture]
    public class TestCase_Navigation: TestCase_Basecase
    {
		
		[Test]
        public void GPSSensitivity()
        {
            int waittime = 45;

            if (this.CFG.gNode("GPS/GPSSensitivity/waittime") != null)
            {
                waittime = Convert.ToInt32(this.CFG.gNode("GPS/GPSSensitivity/waittime").InnerText);
            }
            

            foreach (string ip in IPList)
            {
                Console.WriteLine("ip ={0}", ip);

                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                //首先获取一次导航设定的基本信息
                JObject jparameter = new JObject();
                jparameter["CMDTYPE"] = 0;

                JObject jresp = session.SendCommand(Module.DEVEMM, Operation.SETCONTROLDEVCMD, jparameter);
                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));

                session.Logout();

                Sleep(waittime * 1000);

                DateTime dtNow = DateTime.Now;

                Console.WriteLine("(DateTime.Now - dtNow).TotalSeconds = {0}", (DateTime.Now - dtNow).TotalSeconds);

                while ((DateTime.Now - dtNow).TotalSeconds < 30)
                {
                    session = new N9MSession(ip, port);

                    if (session.Login(username, password) == 0)
                    {
                        break;
                    }
                    else
                    {
                        session.Logout();
                    }
                }

                Assert.IsTrue(session.isConnected());

                DateTime dtStartTime = DateTime.Now;
                DateTime dtEndTime = dtStartTime.AddSeconds(60);

                Sleep(90 * 1000);

                string starttime = dtStartTime.ToString("yyyyMMddHHmmss");
                string endtime = dtEndTime.ToString("yyyyMMddHHmmss");

                string path = Path.Combine(Environment.CurrentDirectory, "gps.data");

                int ret = session.DownloadData(DataType.BLACKBOX_DATATYPE_GPS, starttime, endtime, path);

                Assert.AreEqual(0, ret);

                BlackBoxLoader loader = new BlackBoxLoader(path);

                int nCount = 0;

                string lasttime = null;
                string currenttime = null;

                while (true)
                {
                    RMBDM_FRAMEHEADER header = loader.GetFrame();

                    if (header == null)
                    {
                        break;
                    }

                    Assert.IsNotNull(header, "解析GPS数据失败");

                    Assert.IsNotNull(header.list, "解析GPS附加数据失败");

                    for (int i = 0; i < header.list.Count; i++)
                    {
                        IBaseStruct bs = header.list[i];

                        if (typeof(RMBDM_DATETIME).IsAssignableFrom(bs.GetType()))
                        {
                            RMBDM_DATETIME date = (RMBDM_DATETIME)bs;

                            currenttime = date.time.GetDateTime().ToString("yyyyMMddHHmmss");

                            if (lasttime != null)
                            {
                                if (currenttime == lasttime)
                                {
                                    currenttime = date.time.GetDateTime().AddSeconds(1).ToString("yyyyMMddHHmmss");
                                }
                            }

                    //        Assert.AreEqual(currenttime, dtStartTime.AddSeconds(nCount - 60).ToString("yyyyMMddHHmmss"), "时间间隔比较失败");

                            lasttime = currenttime;

                        }
                        else if (typeof(RMBDM_GPS).IsAssignableFrom(bs.GetType()))
                        {
                            RMBDM_GPS gps = (RMBDM_GPS)bs;
                            Assert.IsTrue(gps.cBdPlanetNum >= 7, "卫星颗数应该大于等于7， 检测到的卫星颗数为{0}", gps.cBdPlanetNum);
                        }
                    }

                    if (currenttime.Equals(endtime))
                    {
                        break;
                    }

                    nCount++;

                }
            }
        }

        [Test]
        public void GPSMode()
        {
            //传入参数

            foreach (string ip in IPList)
            {
                Console.WriteLine("ip ={0}", ip);

                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                //首先获取一次导航设定的基本信息
                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                JObject jgsp = new JObject();

                jmdvr.Add("GSP", "?");
                jparameter.Add("MDVR", jmdvr);

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);
                Assert.IsNotNull(jresp);

                Sleep(90 * 1000);


                jparameter = new JObject();
                jmdvr = new JObject();
                jgsp = new JObject();

                jgsp["GM"] = (int)Navigation.GPS;
                jmdvr.Add("GSP", jgsp);
                jparameter.Add("MDVR", jmdvr);

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                DateTime dt = DateTime.Now;//.ToString("yyyyMMddHHmmss");

                string starttime = dt.AddSeconds(-60).ToString("yyyyMMddHHmmss");
                string endtime = dt.ToString("yyyyMMddHHmmss");

                string path = Environment.CurrentDirectory;
                path += "\\gps.data";

                session.DownloadData(DataType.BLACKBOX_DATATYPE_GPS, starttime, endtime, path);

                BlackBoxLoader loader = new BlackBoxLoader(path);

                int nCount = 0;

                string lasttime = null;
                string currenttime = null;

                while (true)
                {
                    RMBDM_FRAMEHEADER header = loader.GetFrame();

                    if (header == null)
                    {
                        break;
                    }

                    Assert.IsNotNull(header, "解析GPS数据失败");

                    Assert.IsNotNull(header.list, "解析GPS附加数据失败");

                    for (int i = 0; i < header.list.Count; i++)
                    {
                        IBaseStruct bs = header.list[i];

                        if (typeof(RMBDM_DATETIME).IsAssignableFrom(bs.GetType()))
                        {
                            RMBDM_DATETIME date = (RMBDM_DATETIME)bs;

                            currenttime = date.time.GetDateTime().ToString("yyyyMMddHHmmss");

                            Console.WriteLine("currenttime = {0}", currenttime);

                            if (lasttime != null)
                            {
                                if (currenttime == lasttime)
                                {
                                    currenttime = date.time.GetDateTime().AddSeconds(1).ToString("yyyyMMddHHmmss");
                                }
                            }

                     //       Assert.AreEqual(dt.AddSeconds(nCount - 60).ToString("yyyyMMddHHmmss"), currenttime, "时间间隔比较失败");                

                            lasttime = currenttime;

                        }
                        else if (typeof(RMBDM_GPS).IsAssignableFrom(bs.GetType()))
                        {
                            RMBDM_GPS gps = (RMBDM_GPS)bs;
                            Assert.IsTrue(gps.cBdPlanetNum >= 7, "卫星颗数应该大于等于7， 检测到的卫星颗数为{0}", gps.cBdPlanetNum);
                        }
                    }

                    if (currenttime.Equals(endtime))
                    {
                        break;
                    }

                    nCount++;

                }
            }
        }

		
		[Test]
        public void BeidouMode()
        {
            foreach (string ip in IPList)
            {
                Console.WriteLine("ip ={0}", ip);

                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                //首先获取一次导航设定的基本信息
                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                JObject jgsp = new JObject();

                jmdvr.Add("GSP", "?");
                jparameter.Add("MDVR", jmdvr);

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);


                jparameter = new JObject();
                jmdvr = new JObject();
                jgsp = new JObject();

                jgsp["GM"] = (int)Navigation.BeiDou;
                jmdvr.Add("GSP", jgsp);
                jparameter.Add("MDVR", jmdvr);

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Sleep(90 * 1000);

                DateTime dt = DateTime.Now;//.ToString("yyyyMMddHHmmss");

                string starttime = dt.AddSeconds(-60).ToString("yyyyMMddHHmmss");
                string endtime = dt.ToString("yyyyMMddHHmmss");

                string path = Environment.CurrentDirectory;
                path += "\\gps.data";

                session.DownloadData(DataType.BLACKBOX_DATATYPE_GPS, starttime, endtime, path);

                BlackBoxLoader loader = new BlackBoxLoader(path);

                int nCount = 0;

                string lasttime = null;
                string currenttime = null;

                while (true)
                {
                    RMBDM_FRAMEHEADER header = loader.GetFrame();

                    if (header == null)
                    {
                        break;
                    }

                    Assert.IsNotNull(header, "解析GPS数据失败");

                    Assert.IsNotNull(header.list, "解析GPS附加数据失败");

                    for (int i = 0; i < header.list.Count; i++)
                    {
                        IBaseStruct bs = header.list[i];

                        if (typeof(RMBDM_DATETIME).IsAssignableFrom(bs.GetType()))
                        {
                            RMBDM_DATETIME date = (RMBDM_DATETIME)bs;

                            currenttime = date.time.GetDateTime().ToString("yyyyMMddHHmmss");

                            if (lasttime != null)
                            {
                                if (currenttime == lasttime)
                                {
                                    currenttime = date.time.GetDateTime().AddSeconds(1).ToString("yyyyMMddHHmmss");
                                }
                            }

                     //       Assert.AreEqual(currenttime, dt.AddSeconds(nCount - 60).ToString("yyyyMMddHHmmss"), "时间间隔比较失败");

                            lasttime = currenttime;

                        }
                        else if (typeof(RMBDM_GPS).IsAssignableFrom(bs.GetType()))
                        {
                            RMBDM_GPS gps = (RMBDM_GPS)bs;
                            Assert.IsTrue(gps.cBdPlanetNum >= 7, "卫星颗数应该大于等于7， 检测到的卫星颗数为{0}", gps.cBdPlanetNum);
                        }
                    }

                    if (currenttime.Equals(endtime))
                    {
                        break;
                    }

                    nCount++;

                }
            }
        }

		[Ignore("11111")]
		[Test]
        public void GlonassMode()
        {
            foreach (string ip in IPList)
            {
                Console.WriteLine("ip ={0}", ip);

                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                //首先获取一次导航设定的基本信息
                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                JObject jgsp = new JObject();

                jmdvr.Add("GSP", "?");
                jparameter.Add("MDVR", jmdvr);

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);


                jparameter = new JObject();
                jmdvr = new JObject();
                jgsp = new JObject();

                jgsp["GM"] = (int)Navigation.Glonass;
                jmdvr.Add("GSP", jgsp);
                jparameter.Add("MDVR", jmdvr);

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                DateTime dt = DateTime.Now;//.ToString("yyyyMMddHHmmss");

                string starttime = dt.AddSeconds(-60).ToString("yyyyMMddHHmmss");
                string endtime = dt.ToString("yyyyMMddHHmmss");

                string path = Environment.CurrentDirectory;
                path += "\\gps.data";

                session.DownloadData(DataType.BLACKBOX_DATATYPE_GPS, starttime, endtime, path);

                BlackBoxLoader loader = new BlackBoxLoader(path);

                int nCount = 0;

                string lasttime = null;
                string currenttime = null;

                while (true)
                {
                    RMBDM_FRAMEHEADER header = loader.GetFrame();

                    if (header == null)
                    {
                        break;
                    }

                    Assert.IsNotNull(header, "解析GPS数据失败");

                    Assert.IsNotNull(header.list, "解析GPS附加数据失败");

                    for (int i = 0; i < header.list.Count; i++)
                    {
                        IBaseStruct bs = header.list[i];

                        if (typeof(RMBDM_DATETIME).IsAssignableFrom(bs.GetType()))
                        {
                            RMBDM_DATETIME date = (RMBDM_DATETIME)bs;

                            currenttime = date.time.GetDateTime().ToString("yyyyMMddHHmmss");

                            if (lasttime != null)
                            {
                                if (currenttime == lasttime)
                                {
                                    currenttime = date.time.GetDateTime().AddSeconds(1).ToString("yyyyMMddHHmmss");
                                }
                            }

                            Assert.AreEqual(currenttime, dt.AddSeconds(nCount - 60).ToString("yyyyMMddHHmmss"), "时间间隔比较失败");

                            lasttime = currenttime;

                        }
                        else if (typeof(RMBDM_GPS).IsAssignableFrom(bs.GetType()))
                        {
                            RMBDM_GPS gps = (RMBDM_GPS)bs;
                            Assert.IsTrue(gps.cGpPlanetNum >= 7);
                        }
                    }

                    if (currenttime.Equals(endtime))
                    {
                        break;
                    }

                    nCount++;

                }
            }
        }

		
		[Test]
        public void MixedMode()
        { 
            foreach (string ip in IPList)
            {
                Console.WriteLine("ip ={0}", ip);

                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                //首先获取一次导航设定的基本信息
                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                JObject jgsp = new JObject();

                jmdvr.Add("GSP", "?");
                jparameter.Add("MDVR", jmdvr);

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);


                jparameter = new JObject();
                jmdvr = new JObject();
                jgsp = new JObject();

                jgsp["GM"] = (int)Navigation.Mixed;
                jmdvr.Add("GSP", jgsp);
                jparameter.Add("MDVR", jmdvr);

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Sleep(90 * 1000);

                DateTime dt = DateTime.Now;//.ToString("yyyyMMddHHmmss");

                string starttime = dt.AddSeconds(-60).ToString("yyyyMMddHHmmss");
                string endtime = dt.ToString("yyyyMMddHHmmss");

                string path = Environment.CurrentDirectory;
                path += "\\gps.data";

                session.DownloadData(DataType.BLACKBOX_DATATYPE_GPS, starttime, endtime, path);

                BlackBoxLoader loader = new BlackBoxLoader(path);

                int nCount = 0;

                string lasttime = null;
                string currenttime = null;

                while (true)
                {
                    RMBDM_FRAMEHEADER header = loader.GetFrame();

					if (header == null)
					{
						break;
					}

					Assert.IsNotNull(header, "解析GPS数据失败");

                    Assert.IsNotNull(header.list, "解析GPS附加数据失败");

                    for (int i = 0; i < header.list.Count; i++)
                    {
                        IBaseStruct bs = header.list[i];

                        if (typeof(RMBDM_DATETIME).IsAssignableFrom(bs.GetType()))
                        {
                            RMBDM_DATETIME date = (RMBDM_DATETIME)bs;

                            currenttime = date.time.GetDateTime().ToString("yyyyMMddHHmmss");

                            if (lasttime != null)
                            {
                                if (currenttime == lasttime)
                                {
                                    currenttime = date.time.GetDateTime().AddSeconds(1).ToString("yyyyMMddHHmmss");
                                }
                            }

                      //      Assert.AreEqual(currenttime, dt.AddSeconds(nCount - 60).ToString("yyyyMMddHHmmss"), "时间间隔比较失败");

                            lasttime = currenttime;

                        }
                        else if (typeof(RMBDM_GPS).IsAssignableFrom(bs.GetType()))
                        {
                            RMBDM_GPS gps = (RMBDM_GPS)bs;
                            Assert.IsTrue(gps.cBdPlanetNum >= 7, "卫星颗数应该大于等于7， 检测到的卫星颗数为{0}", gps.cBdPlanetNum);
                        }
                    }

                    if (currenttime.Equals(endtime))
                    {
                        break;
                    }

                    nCount++;
                }
            }
        }

        [Test]
        public void GPSUninterruptibleRun()
        {
            string starttime = "20170405000000";
            string endtime = "20170407120000";

            starttime = this.CFG.gNode("GPS/GPSUninterruptibleRun/starttime").InnerText;
            endtime = this.CFG.gNode("GPS/GPSUninterruptibleRun/endtime").InnerText;

            Console.WriteLine("starttime = {0}", starttime);
            Console.WriteLine("endtime = {0}", endtime);

            foreach (string ip in IPList)
            {
                Console.WriteLine("ip ={0}", ip);

                Dictionary<String, String> dict = new Dictionary<String, String>();

                while (starttime.Substring(0, 8) != endtime.Substring(0, 8))
                {
                    DateTime dtStartTime;
                    DateTime dtEndTime;

                    dtStartTime = DateTime.ParseExact(starttime, "yyyyMMddHHmmss", null);
                    dtEndTime = DateTime.ParseExact(starttime.Substring(0, 8) + "235959", "yyyyMMddHHmmss", null);

                    Console.WriteLine("datetime = {0}", dtEndTime.Add(new TimeSpan(0, 0, 1)).ToString());

                    dict.Add(starttime, dtEndTime.ToString("yyyyMMddHHmmss"));

                    starttime = dtEndTime.AddSeconds(1).ToString("yyyyMMddHHmmss");
                }

                dict.Add(starttime, endtime);

                Console.WriteLine("dict = {0}", dict.Count);

                foreach (var item in dict)
                {
                    Console.WriteLine("starttime = {0} endtime = {1}", item.Key, item.Value);
                    N9MSession session = new N9MSession(ip, port);
                    Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                    string path = Path.Combine(Environment.CurrentDirectory, "gps.data");

                    DateTime dtStartTime = DateTime.ParseExact(starttime, "yyyyMMddHHmmss", null);
                    DateTime dtEndTime = DateTime.ParseExact(endtime, "yyyyMMddHHmmss", null);

                    int ret = session.DownloadData(DataType.BLACKBOX_DATATYPE_GPS, starttime, endtime, path);

                    Assert.AreEqual(0, ret);

                    BlackBoxLoader loader = new BlackBoxLoader(path);

                    int nCount = 0;

                    string lasttime = null;
                    string currenttime = null;

                    while (true)
                    {
                        RMBDM_FRAMEHEADER header = loader.GetFrame();

						if (header == null)
						{
							break;
						}

						Assert.IsNotNull(header, "解析GPS数据失败");

                        Assert.IsNotNull(header.list, "解析GPS附加数据失败");

                        for (int i = 0; i < header.list.Count; i++)
                        {
                            IBaseStruct bs = header.list[i];

                            if (typeof(RMBDM_DATETIME).IsAssignableFrom(bs.GetType()))
                            {
                                RMBDM_DATETIME date = (RMBDM_DATETIME)bs;

                                currenttime = date.time.GetDateTime().ToString("yyyyMMddHHmmss");

                                if (lasttime != null)
                                {
                                    if (currenttime == lasttime)
                                    {
                                        currenttime = date.time.GetDateTime().AddSeconds(1).ToString("yyyyMMddHHmmss");
                                    }
                                }

                     //           Assert.AreEqual(dtStartTime.AddSeconds(nCount).ToString("yyyyMMddHHmmss"), currenttime, "时间间隔比较失败");

                                lasttime = currenttime;

                            }
                            else if (typeof(RMBDM_GPS).IsAssignableFrom(bs.GetType()))
                            {
                                RMBDM_GPS gps = (RMBDM_GPS)bs;
                                Assert.IsTrue(gps.cBdPlanetNum >= 7, "卫星颗数应该大于等于7， 检测到的卫星颗数为{0}", gps.cBdPlanetNum);
                            }
                        }

                        if (currenttime.Equals(endtime))
                        {
                            break;
                        }

                        nCount++;
                    }
                }
            }
        }

		[Ignore("11111")]
		[Test]
        public void GPSInterruptibleRun()
        {
            int waittime = 45;

            if (this.CFG.gNode("GPS/GPSInterruptibleRun/waittime") != null)
            {
                waittime = Convert.ToInt32(this.CFG.gNode("GPS/GPSInterruptibleRun/waittime").InnerText);
            }

            foreach (string ip in IPList)
            {
                Console.WriteLine("ip ={0}", ip);

                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                //首先获取一次导航设定的基本信息
                JObject jparameter = new JObject();
                jparameter["CMDTYPE"] = 0;

                JObject jresp = session.SendCommand(Module.DEVEMM, Operation.SETCONTROLDEVCMD, jparameter);
                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));

                session.Logout();

                DateTime dtNow = DateTime.Now;

                Console.WriteLine("(DateTime.Now - dtNow).TotalSeconds = {0}", (DateTime.Now - dtNow).TotalSeconds);

                while ((DateTime.Now - dtNow).TotalSeconds < waittime)
                {
                    session = new N9MSession(ip, port);

                    if (session.Login(username, password) == 0)
                    {
                        break;
                    }
                    else
                    {
                        session.Logout();
                    }
                }

                Assert.IsTrue(session.isConnected());

                DateTime dtStartTime = DateTime.Now;
                DateTime dtEndTime = dtStartTime.AddSeconds(60);

                Sleep(60 * 1000);

                string starttime = dtStartTime.ToString("yyyyMMddHHmmss");
                string endtime = dtEndTime.ToString("yyyyMMddHHmmss");

                string path = Path.Combine(Environment.CurrentDirectory, "gps.data");

                int ret = session.DownloadData(DataType.BLACKBOX_DATATYPE_GPS, starttime, endtime, path);

                Assert.AreEqual(0, ret);

                BlackBoxLoader loader = new BlackBoxLoader(path);

                int nCount = 0;

                string lasttime = null;
                string currenttime = null;

                while (true)
                {
                    RMBDM_FRAMEHEADER header = loader.GetFrame();

                    Assert.IsNotNull(header, "解析GPS数据失败");

                    Assert.IsNotNull(header.list, "解析GPS附加数据失败");

                    for (int i = 0; i < header.list.Count; i++)
                    {
                        IBaseStruct bs = header.list[i];

                        if (typeof(RMBDM_DATETIME).IsAssignableFrom(bs.GetType()))
                        {
                            RMBDM_DATETIME date = (RMBDM_DATETIME)bs;

                            currenttime = date.time.GetDateTime().ToString("yyyyMMddHHmmss");

                            if (lasttime != null)
                            {
                                if (currenttime == lasttime)
                                {
                                    currenttime = date.time.GetDateTime().AddSeconds(1).ToString("yyyyMMddHHmmss");
                                }
                            }

               //             Assert.AreEqual(currenttime, dtStartTime.AddSeconds(nCount - 60).ToString("yyyyMMddHHmmss"), "时间间隔比较失败");

                            lasttime = currenttime;

                        }
                        else if (typeof(RMBDM_GPS).IsAssignableFrom(bs.GetType()))
                        {
                            RMBDM_GPS gps = (RMBDM_GPS)bs;
                            Assert.IsTrue(gps.cGpPlanetNum >= 7, "GPS卫星颗数不足7颗， 实际为{0}颗", gps.cGpPlanetNum);
                        }
                    }

                    if (currenttime.Equals(endtime))
                    {
                        break;
                    }

                    nCount++;
                }
            }
        }
    }
}
