using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using N9MTest.commons;
using RM;
using N9MTest.SDK.net;
using Newtonsoft.Json.Linq;
using System.Threading;
using Newtonsoft.Json;

namespace N9MTest.DialupNetwork
{
	[Ignore("11111")]
	[TestFixture]
    class TestCase_Server: TestCase_Basecase
    {
        string csip2 = "192.168.6.124";
        string msip2 = "192.168.6.124";
        int csport2 = 7300;
        int msport2 = 7400;
        string csip3 = "192.168.6.124";
        string msip3 = "192.168.6.124";
        int csport3 = 7300;
        int msport3 = 7400;
 
        [Test]
        public void ServerNetwork()
        {
            //中心服务器IP
            csip2 = this.CFG.gNode("DialupNetwork/ServerNetwork/server2csip").InnerText;
            csport2 = Convert.ToInt32(this.CFG.gNode("DialupNetwork/ServerNetwork/server2csport").InnerText);
            msip2 = this.CFG.gNode("DialupNetwork/ServerNetwork/server2msip").InnerText;
            msport2 = Convert.ToInt32(this.CFG.gNode("DialupNetwork/ServerNetwork/server2msport").InnerText);

            csip3 = this.CFG.gNode("DialupNetwork/ServerNetwork/server3csip").InnerText;
            csport3 = Convert.ToInt32(this.CFG.gNode("DialupNetwork/ServerNetwork/server3csport").InnerText);
            msip3 = this.CFG.gNode("DialupNetwork/ServerNetwork/server3msip").InnerText;
            msport3 = Convert.ToInt32(this.CFG.gNode("DialupNetwork/ServerNetwork/server3msport").InnerText);
            foreach (string ip in IPList)
            {
                N9MSession session = new N9MSession(ip, port);
                session.Login(username, password);

                JObject jresp = null;
                JObject jresprec = null;
                JObject jparameter = new JObject();
                JObject jmdvr = new JObject();

                JObject jwifieb = new JObject();
                JObject jwifi = new JObject();

                //查询wifi是否启用
                jwifi.Add("WIFI","?");
                jparameter.Add("MDVR", jwifi);
                jresprec = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);
                if(!(string.Equals(jresprec["MDVR"]["WIFI"]["ENABLE"].ToString(),"1")))
                {
                    Assert.Fail("请检查WIFI是否启用了？");
                }
                jwifi.RemoveAll();
                jparameter.RemoveAll();


                //首先获取中心服务器
                jmdvr.Add("MCMS","?");
                jparameter.Add("MDVR", jmdvr);
                jresp = session.SendCommand(Module.CONFIGMODEL, Operation.GET, jparameter);
                if (jresp == null)
                {
                    session.Logout();
                    Assert.Fail("获取中心服务器失败！");     
                }
                if(Convert.ToInt16(jresp["MDVR"]["MCMS"]["M"].ToString()) < 2)
                {
                    jresp["MDVR"]["MCMS"]["M"] = 4;
                }
                //设置其他服务器禁用
                jresp["MDVR"]["MCMS"]["SP"][0]["EN"] = 0;
                jresp["MDVR"]["MCMS"]["SP"][3]["EN"] = 0;
                jresp["MDVR"]["MCMS"]["SP"][4]["EN"] = 0;
                jresp["MDVR"]["MCMS"]["SP"][5]["EN"] = 0;

                //设置Server2为WIFI网络及N9M服务器
                jresp["MDVR"]["MCMS"]["SP"][1]["EN"] = 1;//是否开启服务器
                jresp["MDVR"]["MCMS"]["SP"][1]["NWT"] = 1; //网络类型； 0:有线网络；1:wifi； 2:手机网络
                jresp["MDVR"]["MCMS"]["SP"][1]["CP"] = 0;//协议类型[；0：默认N9，1:智达2: 808] 
                jresp["MDVR"]["MCMS"]["SP"][1]["CS"] = csip2;//中心信令服务器地址
                jresp["MDVR"]["MCMS"]["SP"][1]["MS"] = msip2;//媒体服务器
                jresp["MDVR"]["MCMS"]["SP"][1]["CPORT"] = csport2;//中心信令服务器端口
                jresp["MDVR"]["MCMS"]["SP"][1]["MPORT"] = msport2;//媒体服务器端口

                // 设置Server3为3G网络及N9M服务器
                jresp["MDVR"]["MCMS"]["SP"][2]["EN"] = 1;//是否开启服务器
                jresp["MDVR"]["MCMS"]["SP"][2]["NWT"] = 2; //网络类型； 0:有线网络；1:wifi； 2:手机网络
                jresp["MDVR"]["MCMS"]["SP"][2]["CP"] = 0;//协议类型[；0：默认N9，1:智达2: 808] 
                jresp["MDVR"]["MCMS"]["SP"][2]["CS"] = csip3;//中心信令服务器地址
                jresp["MDVR"]["MCMS"]["SP"][2]["MS"] = msip3;//媒体服务器
                jresp["MDVR"]["MCMS"]["SP"][2]["CPORT"] = csport3;//中心信令服务器端口
                jresp["MDVR"]["MCMS"]["SP"][2]["MPORT"] = msport3;//媒体服务器端口

                jresprec = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jresp);
                Assert.AreEqual(0, Convert.ToInt32(jresprec["ERRORCODE"]), "设置CenterServer失败.错误码为{0}", Convert.ToInt32(jresprec["ERRORCODE"]));

                Sleep(30000);

                //获取服务器连接状态
                jmdvr.RemoveAll();
                jparameter.RemoveAll();
                jmdvr.Add("SERIAL", "?");
                jparameter.Add("MDVR", jmdvr);
                jresprec = session.SendCommand(Module.DEVEMM, Operation.GETCMSCONNECTSTATUS, jparameter);

                List<CMSConnectInfo> cmsList = JsonConvert.DeserializeObject<List<CMSConnectInfo>>(jresp["LIST"].ToString());

                int servernum = 0;
                for(servernum = 0; servernum < cmsList.Count; servernum++)
                {
                    if(string.Equals(cmsList[servernum].address, csip2))
                    {
                        Assert.AreEqual(1, cmsList[1].nCS, "wifi连接服务器{0}失败！", cmsList[1].address);
                        break;
                    }
                }
                if(servernum == cmsList.Count)
                {
                    Assert.Fail("wifi连接服务器失败1！");
                }
                for (servernum = 0; servernum < cmsList.Count; servernum++)
                {
                    if (string.Equals(jresprec["ADD"][servernum], csip3))
                    {
                        Assert.AreEqual(1, cmsList[2].nCS, "3G连接服务器{1}失败！", cmsList[2].nCS);
                        break;
                    }
                }
                if (servernum == cmsList.Count)
                {
                    Assert.Fail("3G连接服务器失败@@！");
                }
                //只设置Server3为auto模式及N9M服务器
                jresp["MDVR"]["MCMS"]["SP"][1]["EN"] = 0;
                jresp["MDVR"]["MCMS"]["SP"][2]["NWT"] = 4;//自适应
                jresprec = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jresp);
                Assert.AreEqual(0, Convert.ToInt32(jresprec["ERRORCODE"]), "设置CenterServer失败.错误码为{0}", Convert.ToInt32(jresprec["ERRORCODE"]));

                //获取server3连接状态，及网络类型
                jresprec = session.SendCommand(Module.DEVEMM, Operation.GETCMSCONNECTSTATUS, jparameter);
                cmsList = JsonConvert.DeserializeObject<List<CMSConnectInfo>>(jresp["LIST"].ToString());

                for (servernum = 0; servernum < cmsList.Count; servernum++)
                {
                    if (string.Equals(cmsList[servernum].address, csip3) && (cmsList[servernum].nM == 1))
                    {
                        Assert.AreEqual(1, cmsList[2].nCS, "自适应WIFI连接服务器{0}失败！", cmsList[2].address);
                        break;
                    }
                }
                if (servernum == cmsList.Count)
                {
                    Assert.Fail("自适应WIFI连接服务器失败@@！");
                }

                //关闭wifi
                
                jmdvr.RemoveAll();
                jwifieb.Add("ENABLE", "0");
                jwifi.Add("WIFI", jwifieb);
                jmdvr.Add("MDVR", jwifi);
                jresprec = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jmdvr);
                Assert.AreEqual(0, Convert.ToInt32(jresprec["ERRORCODE"]), "禁用wifi失败.错误码为{0}", Convert.ToInt32(jresprec["ERRORCODE"]));

                Sleep(30000);

                //获取server3连接状态，及网络类型
                jresprec = session.SendCommand(Module.DEVEMM, Operation.GETCMSCONNECTSTATUS, jparameter);
                cmsList = JsonConvert.DeserializeObject<List<CMSConnectInfo>>(jresp["LIST"].ToString());

                for (servernum = 0; servernum < cmsList.Count; servernum++)
                {
                    if (string.Equals(cmsList[servernum].address, csip3) && (cmsList[servernum].nM == 2))
                    {
                        Assert.AreEqual(1, Convert.ToInt32(jresprec["CS"][2]), "自适应3G连接服务器失败！", Convert.ToInt32(jresprec["CS"][2]));
                        break;
                    }
                }
                if (servernum == cmsList.Count)
                {
                    Assert.Fail("自适应3G连接服务器失败！");
                }

                session.Logout();
            }
        }

       [Test]
        public void ServerType()
        {
            //中心服务器IP
            csip2 = this.CFG.gNode("G4/ServerType/server2csip").InnerText;
            csport2 = Convert.ToInt32(this.CFG.gNode("G4/ServerType/server2csport").InnerText);
            msip2 = this.CFG.gNode("G4/ServerType/server2msip").InnerText;
            msport2 = Convert.ToInt32(this.CFG.gNode("G4/ServerType/server2msport").InnerText);

            csip3 = this.CFG.gNode("G4/ServerType/server3csip").InnerText;
            csport3 = Convert.ToInt32(this.CFG.gNode("G4/ServerType/server3csport").InnerText);
            msip3 = this.CFG.gNode("G4/ServerType/server3msip").InnerText;
            msport3 = Convert.ToInt32(this.CFG.gNode("G4/ServerType/server3msport").InnerText);
            foreach (string ip in IPList)
            {
                N9MSession session = new N9MSession(ip, port);
                session.Login(username, password);

                JObject jresp = null;
                JObject jresprec = null;
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
                if (Convert.ToInt16(jresp["MDVR"]["MCMS"]["M"].ToString()) < 2)
                {
                    jresp["MDVR"]["MCMS"]["M"] = 4;
                }
                
                //设置Server1为WIFI网络及N9M服务器
                jresp["MDVR"]["MCMS"]["SP"][0]["EN"] = 1;//是否开启服务器
                jresp["MDVR"]["MCMS"]["SP"][0]["NWT"] = 1; //网络类型； 0:有线网络；1:wifi； 2:手机网络
                jresp["MDVR"]["MCMS"]["SP"][0]["CP"] = 0;//协议类型[；0：默认N9，1:智达 2: 808] 3 运维
                jresp["MDVR"]["MCMS"]["SP"][0]["CS"] = csip2;//中心信令服务器地址
                jresp["MDVR"]["MCMS"]["SP"][0]["MS"] = msip2;//媒体服务器
                jresp["MDVR"]["MCMS"]["SP"][0]["CPORT"] = csport2;//中心信令服务器端口
                jresp["MDVR"]["MCMS"]["SP"][0]["MPORT"] = msport2;//媒体服务器端口

                // 设置Server2为3G网络及N9M服务器
                jresp["MDVR"]["MCMS"]["SP"][1]["EN"] = 1;//是否开启服务器
                jresp["MDVR"]["MCMS"]["SP"][1]["NWT"] = 2; //网络类型； 0:有线网络；1:wifi； 2:手机网络
                jresp["MDVR"]["MCMS"]["SP"][1]["CP"] = 0;//协议类型[；0：默认N9，1:智达2: 808] 
                jresp["MDVR"]["MCMS"]["SP"][1]["CS"] = csip3;//中心信令服务器地址
                jresp["MDVR"]["MCMS"]["SP"][1]["MS"] = msip3;//媒体服务器
                jresp["MDVR"]["MCMS"]["SP"][1]["CPORT"] = csport3;//中心信令服务器端口
                jresp["MDVR"]["MCMS"]["SP"][1]["MPORT"] = msport3;//媒体服务器端口

                //设置其他服务器禁用
                jresp["MDVR"]["MCMS"]["SP"][2]["EN"] = 0;
                jresp["MDVR"]["MCMS"]["SP"][3]["EN"] = 0;
                jresp["MDVR"]["MCMS"]["SP"][4]["EN"] = 0;
                jresp["MDVR"]["MCMS"]["SP"][5]["EN"] = 0;

                jresprec = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jresp);
                Assert.AreEqual(0, Convert.ToInt32(jresprec["ERRORCODE"]), "设置CenterServer失败.错误码为{0}", Convert.ToInt32(jresprec["ERRORCODE"]));

                // 设置Server3为3G网络及N9M服务器
                jresp["MDVR"]["MCMS"]["SP"][0]["EN"] = 0;
                jresp["MDVR"]["MCMS"]["SP"][1]["EN"] = 0;
                jresp["MDVR"]["MCMS"]["SP"][2]["EN"] = 1;//是否开启服务器
                jresp["MDVR"]["MCMS"]["SP"][2]["NWT"] = 0; //网络类型； 0:有线网络；1:wifi； 2:手机网络
                jresp["MDVR"]["MCMS"]["SP"][2]["CP"] = 0;//协议类型[；0：默认N9，1:智达2: 808] 
                jresp["MDVR"]["MCMS"]["SP"][2]["CS"] = csip3;//中心信令服务器地址
                jresp["MDVR"]["MCMS"]["SP"][2]["MS"] = msip3;//媒体服务器
                jresp["MDVR"]["MCMS"]["SP"][2]["CPORT"] = csport3;//中心信令服务器端口
                jresp["MDVR"]["MCMS"]["SP"][2]["MPORT"] = msport3;//媒体服务器端口

                jresprec = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jresp);
                Assert.AreNotEqual(0, Convert.ToInt32(jresprec["ERRORCODE"]), "设置CenterServer失败.错误码为{0}", Convert.ToInt32(jresprec["ERRORCODE"]));

                //设置Server1为WIFI网络及运维服务器
                jresp["MDVR"]["MCMS"]["SP"][0]["CP"] = 3;
                jresp["MDVR"]["MCMS"]["SP"][2]["EN"] = 0;
                jresp["MDVR"]["MCMS"]["SP"][0]["EN"] = 1;
                jresprec = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jresp);
                Assert.AreEqual(0, Convert.ToInt32(jresprec["ERRORCODE"]), "设置CenterServer失败.错误码为{0}", Convert.ToInt32(jresprec["ERRORCODE"]));

                //设置Server2 Server3为3G网络及运维服务器
                jresp["MDVR"]["MCMS"]["SP"][1]["EN"] = 1;
                jresp["MDVR"]["MCMS"]["SP"][2]["EN"] = 0;
                jresp["MDVR"]["MCMS"]["SP"][1]["CP"] = 3;
                jresp["MDVR"]["MCMS"]["SP"][2]["CP"] = 3;
                jresprec = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jresp);
                Assert.AreNotEqual(0, Convert.ToInt32(jresprec["ERRORCODE"]), "设置CenterServer失败.错误码为{0}", Convert.ToInt32(jresprec["ERRORCODE"]));

                //设置Server1-6 为808服务器
                jresp["MDVR"]["MCMS"]["SP"][0]["EN"] = 1;
                jresp["MDVR"]["MCMS"]["SP"][2]["EN"] = 1;
                jresp["MDVR"]["MCMS"]["SP"][3]["EN"] = 1;
                jresp["MDVR"]["MCMS"]["SP"][4]["EN"] = 1;
                jresp["MDVR"]["MCMS"]["SP"][5]["EN"] = 1;

                jresp["MDVR"]["MCMS"]["SP"][0]["CP"] = 2;
                jresp["MDVR"]["MCMS"]["SP"][1]["CP"] = 2;
                jresp["MDVR"]["MCMS"]["SP"][2]["CP"] = 2;
                jresp["MDVR"]["MCMS"]["SP"][3]["CP"] = 2;
                jresp["MDVR"]["MCMS"]["SP"][4]["CP"] = 2;
                jresp["MDVR"]["MCMS"]["SP"][5]["CP"] = 2;
                jresprec = session.SendCommand(Module.CONFIGMODEL, Operation.SET, jresp);
                Assert.AreEqual(0, Convert.ToInt32(jresprec["ERRORCODE"]), "设置CenterServer失败.错误码为{0}", Convert.ToInt32(jresprec["ERRORCODE"]));

            }
        }
    }
}
