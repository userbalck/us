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
using Newtonsoft.Json;
using Util;

namespace N9MTest.DialupNetwork
{ 
    [TestFixture]
    class TestCase_RegisterInfo : TestCase_Basecase
    {
        public void SetSever(N9MSession session, string xmlnode)
        {
            JObject jresp = null;
            JObject jparameter = new JObject();
            JObject jmdvr = new JObject();

            //首先获取中心服务器
            jmdvr.Add("MCMS", "?");
            jparameter.Add("MDVR", jmdvr);
            jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);
            if (jresp == null)
            {
                session.Logout();
                Assert.Fail("获取中心服务器失败！");
            }

            //设置Server1为auto网络及N9M服务器
            string csip = "192.168.6.124";
            string msip = "192.168.6.124";
            int csport = 7300;
            int msport = 7400;

            if (this.CFG.gNode(xmlnode + "csip") != null)
            {
                Console.WriteLine("开始设定server xmlnode = {0}", xmlnode);
            }
            else
            {
                Console.WriteLine("为空");
            }

            csip = this.CFG.gNode(xmlnode + "csip").InnerText;

            Console.WriteLine("csip = {0}", csip);
            csport = Convert.ToInt32(this.CFG.gNode(xmlnode + "csport").InnerText);

            Console.WriteLine("csport = {0}", csport);

            msip = this.CFG.gNode(xmlnode + "msip").InnerText;
            Console.WriteLine("msip = {0}", msip);

            msport = Convert.ToInt32(this.CFG.gNode(xmlnode + "msport").InnerText);
            Console.WriteLine("msport = {0}", msport);

            jresp["MDVR"]["MCMS"]["SP"][0]["EN"] = 1;//是否开启服务器
            jresp["MDVR"]["MCMS"]["SP"][0]["NWT"] = 4;
            jresp["MDVR"]["MCMS"]["SP"][0]["CP"] = 0;//协议类型[；0：默认N9，1:智达 2: 808] 3 运维
            jresp["MDVR"]["MCMS"]["SP"][0]["CS"] = csip;//中心信令服务器地址
            jresp["MDVR"]["MCMS"]["SP"][0]["MS"] = msip;//媒体服务器
            jresp["MDVR"]["MCMS"]["SP"][0]["CPORT"] = csport;//中心信令服务器端口
            jresp["MDVR"]["MCMS"]["SP"][0]["MPORT"] = msport;//媒体服务器端口

            //设置其他服务器禁用
            jresp["MDVR"]["MCMS"]["SP"][1]["EN"] = 0;
            jresp["MDVR"]["MCMS"]["SP"][2]["EN"] = 0;
            jresp["MDVR"]["MCMS"]["SP"][3]["EN"] = 0;
            jresp["MDVR"]["MCMS"]["SP"][4]["EN"] = 0;
            jresp["MDVR"]["MCMS"]["SP"][5]["EN"] = 0;

            jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jresp);
            Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"]), "设置CenterServer失败.错误码为{0}", Convert.ToInt32(jresp["ERRORCODE"]));
        }
        public void PlaybackSearch(N9MSession session,string xmltype)
        {
           // Console.WriteLine("username ={0} password ={1} ip ={2} port ={3}", username, password,ip,port);

            int nStreamType = (int)StreamType.MAIN_STREAM;
            uint nChannelBits = 0xffffff;

            string starttime = DateTime.Now.ToString("yyyyMMddHH") + "0000";
            string endtime = DateTime.Now.ToString("yyyyMMddHH") + "5959";
            nStreamType = Convert.ToInt32(this.CFG.gNode(xmltype + "streamtype").InnerText);
            nChannelBits = Convert.ToUInt32(this.CFG.gNode(xmltype + "channel").InnerText);
            //starttime = this.CFG.gNode(xmltype + "starttime").InnerText;
            //endtime = this.CFG.gNode(xmltype + "endtime").InnerText;

            Console.WriteLine("nStreamType ={0}", nStreamType);
            Console.WriteLine("nChannelBits ={0}", nChannelBits);
            Console.WriteLine("starttime ={0}", starttime);
            Console.WriteLine("endtime ={0}", endtime);
            JObject jparamater = new JObject();
            jparamater.Add("STREAMTYPE", nStreamType);
            jparamater.Add("FILETYPE", 0xffffffff);
            jparamater.Add("CHANNEL", nChannelBits);
            jparamater.Add("STARTTIME", starttime);
            jparamater.Add("ENDTIME", endtime);

            JObject jresp = session.SendCommand(Module.STORM, Operation.QUERYFILELIST, jparamater);
        }

        public void CommonInfo(N9MSession session, string xmlnode,string jsonnode,string setval)
        {
            JObject jresp = null;
            JObject jparameter = new JObject();
            JObject jmdvr = new JObject();

            JObject jrip = new JObject();
            //jrip.Add("BN", "");
            //jmdvr.Add("RIP", jrip);
            jrip.Add(jsonnode, setval);
            jmdvr.Add("RIP", jrip);
            jparameter.Add("MDVR", jmdvr);
            jresp = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jparameter);
            Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"]), "设置车牌号为空失败.错误码为{0}", Convert.ToInt32(jresp["ERRORCODE"]));

            Sleep(10000);

            //检查服务器连接状态
            jmdvr.RemoveAll();
            jparameter.RemoveAll();
            jmdvr.Add("SERIAL", "?");
            jparameter.Add("MDVR", jmdvr);
            jresp = session.SendCommand(Module.DEVEMM, Operation.GETCMSCONNECTSTATUS, jparameter);
            List<CMSConnectInfo> cmsList = JsonConvert.DeserializeObject<List<CMSConnectInfo>>(jresp["LIST"].ToString());
            Assert.AreEqual(1, cmsList[0].nCS, "服务器{0}未连接", cmsList[0].address);

            //检索录像文件
  //          PlaybackSearch(session, xmlnode + "PlayBack/");


            //没办法判断文件保存路径
            //TODO

        }

        [Test]
        public void VehiclePlate()
        {
            foreach (string ip in IPList)
            {
                N9MSession session = new N9MSession(ip, port);
                session.Login(username, password);

                //设置服务器
                SetSever(session, "DialupNetwork/VehiclePlate/Server/");

                CommonInfo(session, "DialupNetwork/VehicleNum/", "BN", "");

                CommonInfo(session, "DialupNetwork/VehicleNum/", "BN", "~！@#￥%");

                session.Logout();
            }
        }

        [Test]
        public void DriverInfor()
        {
            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);
                N9MSession session = new N9MSession(ip, port);
                session.Login(username, password);

                //设置服务器
                SetSever(session, "DialupNetwork/DriverInfor/Server/");

                CommonInfo(session, "DialupNetwork/DriverInfor/", "DID", "~！@#￥%");

                session.Logout();
            }
        }

        [Test]
        public void VehicleNum()
        {
            foreach (string ip in IPList)
            {
                N9MSession session = new N9MSession(ip, port);
                session.Login(username, password);

                //设置服务器
                SetSever(session, "DialupNetwork/VehicleNum/Server/");

                CommonInfo(session, "DialupNetwork/VehicleNum/", "BID", "~！@#￥%");

                session.Logout();
            }
        }
        [Test]
        public void SerialNum()
        {
            foreach (string ip in IPList)
            {
                N9MSession session = new N9MSession(ip, port);
                session.Login(username, password);

                DeviceInfo info = session.GetDeviceInfo();

                Assert.IsNotNull(info.dsno);
                Assert.IsNotEmpty(info.dsno);
                session.Logout();
            }
        }
    }
}
