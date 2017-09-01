using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Xml;
using N9MTest.SDK.net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Util;

namespace N9MTest.Local
{
    [TestFixture]
    class TestCase_WiredNetwork:TestCase_Basecase
    {
        public bool PingIPisLosspage(string ip, int seconds, int package)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口
            p.Start();//启动程序

            //向cmd窗口发送输入信息
            //p.StandardInput.WriteLine("ping " + ip + "&exit");
            p.StandardInput.WriteLine("ping " + ip + " -l " + package + " -w 1000 -n " + seconds + " &exit"); //一秒钟ping一次，一分钟则ping 60次
            p.StandardInput.AutoFlush = true;
            //p.StandardInput.WriteLine("exit");
            string output = p.StandardOutput.ReadToEnd();

            p.WaitForExit();//等待程序执行完退出进程
            p.Close();

            Console.WriteLine(output);

            if (output.IndexOf("丢失 = 0") == -1)
            {
                return true;
            }
            return false;
        }

       [Test]
        public void StaticLocalIP()
        {
            string setip = this.CFG.gNode("Local/StaticLocalIP/LocalIP").InnerText;
            string setdns = this.CFG.gNode("Local/StaticLocalIP/LocalDNS").InnerText;
            int package = Convert.ToInt32(this.CFG.gNode("Local/StaticLocalIP/package").InnerText);

            foreach (string ip in IPList)
            {
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password));

                JObject jresp = null;
                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                JObject jehternet = new JObject();
                JObject jpip = new JObject();

                //设置静态IP 和 自动DSN
                jpip.Add("IPADDR", setip);
                jmdvr.Add("PIP", jpip);
                jmdvr.Add("DNSMODE", "1");
                jehternet.Add("ETHERNET", jmdvr);
                jparameter.Add("MDVR", jehternet);
                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);
                //ip修改后不会收到回应
                //Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"]), "设置静态ip及动态DSN失败.错误码为{0}", Convert.ToInt32(jresp["ERRORCODE"]));
                session.Logout();

                N9MSession sessionaftrer = new N9MSession(setip, port);
                sessionaftrer.Login(username, password);
                Assert.IsFalse(PingIPisLosspage(setip, 60, package), "静态IP自动DSN,1分钟内ping出现丢包！");

                //设置静态DSN
                jmdvr.RemoveAll();
                jehternet.RemoveAll();
                jparameter.RemoveAll();

                JObject jdns = new JObject();
                jdns.Add("PDNS", setdns);
                jdns.Add("ADNS", setdns);
                jmdvr.Add("DNS", jdns);
                jmdvr.Add("DNSMODE", "0");
                jehternet.Add("ETHERNET", jmdvr);
                jparameter.Add("MDVR", jehternet);
                jresp = sessionaftrer.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);
                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"]), "设置静态DNS失败.错误码为{0}", Convert.ToInt32(jresp["ERRORCODE"]));

                Assert.IsFalse(PingIPisLosspage(setip, 60, package), "静态DNS,1分钟内ping出现丢包！");
                sessionaftrer.Logout();
            }

        }

		[Ignore("11111")]
		[Test]
        public void AutoLocalIP()
        {
            string setip = this.CFG.gNode("Local/AutoLocalIP/LocalIP").InnerText;
            string setdns = this.CFG.gNode("Local/AutoLocalIP/LocalDNS").InnerText;
            int package = Convert.ToInt32(this.CFG.gNode("Local/AutoLocalIP/package").InnerText);

            foreach (string ip in IPList)
            {
                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password));

                JObject jresp = null;
                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                JObject jehternet = new JObject();

                //设置动态IP 和 动态DSN
                jmdvr.Add("IPMODE", "1");
                jmdvr.Add("DNSMODE", "1");
                jehternet.Add("ETHERNET", jmdvr);
                jparameter.Add("MDVR", jehternet);
                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);
                //ip修改后不会收到回应
                session.Logout();

                //动态ip需要等待分配ip
                Sleep(30000);

                //动态ip， n9m没办法得到ip，测试无法进行.
                //TODO

            }
        }

		
		[Test]
        public void WANDownloadSpeed()
        {
            XmlNodeList ChannelInfoList = this.CFG.gNodes("Local/WANDownloadSpeed/channel");
            int duration = Convert.ToInt32(this.CFG.gNode("Local/WANDownloadSpeed/duration").InnerText);
            int count = Convert.ToInt32(this.CFG.gNode("Local/WANDownloadSpeed/count").InnerText);
            int nSpeed = Convert.ToInt32(this.CFG.gNode("Local/WANDownloadSpeed/download-speed").InnerText);

            Dictionary<int, NetStreamEncoderInfo> dict = new Dictionary<int, NetStreamEncoderInfo>();

            int nChannelBits = 0;

            for (int i = 0; i < ChannelInfoList.Count; i++)
            {
                NetStreamEncoderInfo info = new NetStreamEncoderInfo();
                int channel = Convert.ToInt32(this.CFG.gNode(ChannelInfoList[i], "id").InnerText);
                info.resolution = (int)(Resolution)Enum.Parse(typeof(Resolution), "RES_" + this.CFG.gNode(ChannelInfoList[i], "resolution").InnerText.ToUpper());
                info.FrameRate = Convert.ToInt32(this.CFG.gNode(ChannelInfoList[i], "frame-rate").InnerText);
                info.quality = Convert.ToInt32(this.CFG.gNode(ChannelInfoList[i], "quality").InnerText);
                info.VideoEnable = Convert.ToInt32(this.CFG.gNode(ChannelInfoList[i], "enable").InnerText);
                info.BitrateMode = (int)Enum.Parse(typeof(BitrateMode), this.CFG.gNode(ChannelInfoList[i], "mode").InnerText);

                nChannelBits |= (1 << channel);

                dict.Add(channel, info);
            }

            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                session.Login(username, password);

                //首先获取主码流的参数
                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();
                jmdvr.Add("MAIN", "?");
                jparameter.Add("MDVR", jmdvr);

                JObject jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);
                Assert.IsNotNull(jresp["MDVR"]);
                Assert.IsNotNull(jresp["MDVR"]["MAIN"]);

                List<NetStreamEncoderInfo> list = JsonConvert.DeserializeObject<List<NetStreamEncoderInfo>>(jresp["MDVR"]["MAIN"].ToString());


                //按照参数进行录像参数设定
                jparameter = new JObject();
                jmdvr = new JObject();

                JArray jnecArray = new JArray();

                foreach (var item in dict)
                {
                    JObject nec = new JObject();
                    int channel = item.Key;
                    NetStreamEncoderInfo info = dict[channel];
                    nec = JObject.Parse(JsonConvert.SerializeObject(info));
                    nec.Remove("BR");
                    jnecArray.Insert(channel, nec);
                }

                jmdvr.Add("MAIN", jnecArray);

                jparameter.Add("MDVR", jmdvr);

                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);

                Console.WriteLine("jresp = {0}", jresp);

                Sleep(90 * 1000);

                DateTime now = DateTime.Now;

                int nStreamType = (int)StreamType.MAIN_STREAM;

                string starttime = now.AddSeconds(0 - duration).ToString("yyyyMMddHHmmss");
                string endtime = DateTime.Now.ToString("yyyyMMddHHmmss");


                jparameter = new JObject();
                jparameter.Add("STREAMTYPE", nStreamType);
                jparameter.Add("FILETYPE", 0xffffffff);
                jparameter.Add("CHANNEL", nChannelBits);
                jparameter.Add("STARTTIME", starttime);
                jparameter.Add("ENDTIME", endtime);

                jresp = session.SendCommand(Module.STORM, Operation.QUERYFILELIST, jparameter);

                Assert.IsNotNull(jresp["FILELIST"]);

                List<RemoteFileInfo> RemoteFileList = JsonConvert.DeserializeObject<List<RemoteFileInfo>>(jresp["FILELIST"].ToString());


                for (int i = 0; i < count; i++)
                {
                    foreach (RemoteFileInfo info in RemoteFileList)
                    {
                        //首先获取段内文件的大小
                        JObject jparamater = new JObject();
                        jparamater["DATATYPE"] = 0;
                        jparamater["STREAMTYPE"] = nStreamType;
                        jparamater["RECORDTYPE"] = 3;
                        jparamater["STIME"] = starttime;
                        jparamater["ETIME"] = endtime;
                        jparamater["CHANNEL"] = 1 << info.nChannel;

                        jresp = session.SendCommand(Module.STORM, Operation.GETFILESIZEBYTIME, jparamater);
                        Assert.IsNotNull(jresp);
                        Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));

                        int nTotalSize = Convert.ToInt32(jresp["TOTALSIZE"].ToString());

                        long before = ExactTime.GetExactTime();

                        session.DownloadVideo((StreamType)nStreamType, info.recordID, info.nChannel, starttime, endtime, "video.264");

                        long after = ExactTime.GetExactTime();

                        double rate = (nTotalSize * 1.0) / (after - before);

                        Console.WriteLine("当前文件下载速度为 {0} byte/s", rate);

                        Assert.IsTrue(rate * 8/(1024 * 1024) > 27, "理想预计的下载速度是 {0} Mb/s 实际下载速度为 {1} Mb/s", nSpeed, (int)(rate * 8)/(1024 * 1024));
                    }
                }
            }
        }
    }
}
