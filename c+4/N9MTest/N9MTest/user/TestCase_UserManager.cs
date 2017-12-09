using System;
using System.Collections.Generic;
using NUnit.Framework;
using N9MTest.SDK.http;
using RM;
using N9MTest.commons;
using System.IO;
using N9MTest.SDK.net;
using Newtonsoft.Json.Linq;
using Util;

namespace N9MTest.User
{
    [TestFixture]
    class TestCase_UserManager: TestCase_Basecase
    {
        private enum UserRight
        {
            SUPPER = 1,//超级用户
            MANAGE = 2,//管理员
            COMMON = 3 //普通用户
        }

        private enum OptType
        {
            ADD = 0,//添加
            MOD = 1,//修改
            DEL = 2 //删除
        }
        private RMXML CFG;
        private FUN fun;

        [TestFixtureSetUp]
        public void Init()
        {
            this.fun = new FUN(); this.CFG = fun.xmlConfig;

            Console.WriteLine("版本{0} 发布日期:{1}",
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                File.GetLastWriteTime(this.GetType().Assembly.Location));
        }


        //判断用户是否存在
        private void IsExistUser(N9MSession session,string usrname)
        {
            JObject jmdvr = new JObject();
            JObject jresp = new JObject();

            jmdvr.Add("MDVR", new JObject());
            jresp = session.SendCommand(Module.DEVEMM, Operation.GETUSERRIGHTINFO, jmdvr);
            Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"]), "获取用户权限失败.错误码为{0}", Convert.ToInt32(jresp["ERRORCODE"]));
            JArray juseright = JArray.Parse(jresp["UIF"].ToString());
            int i = 0;
            for (i = 0; i < juseright.Count; i++)
            {
                if (jresp["UIF"][i]["UN"].ToString() == usrname)
                {
                    i = -1;
                    break;
                }
            }
            Assert.AreEqual(-1,i, usrname + "用户不存在！！！" + i);
        }

        private int SetUserCommon(string ip, int port, string username, string password, string setuser,string setpw,UserRight setuserright,OptType oprtype = OptType.MOD)
        {
            N9MSession session = new N9MSession(ip, port);
            Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

            JObject jresp = null;
            JObject jparameter = new JObject();
            JObject jmdvr = new JObject();

            long utctime = ExactTime.GetUTCTimeStamp(DateTime.UtcNow);
            switch (oprtype)
            {
                case OptType.ADD:
                    jparameter["MANAGECMD"] = 0;//添加
                    jparameter.Add("ADDTIME", utctime);
                    break;
                case OptType.MOD:
                    //先判断用户是否存在
                    IsExistUser(session, setuser);
                    jparameter["MANAGECMD"] = 1;//修改
                    jparameter.Add("EDITTIME", utctime);
                    break;
                case OptType.DEL:
                    jparameter["MANAGECMD"] = 2;//删除
                    break;
                default:
                    session.Logout();
                    Assert.IsFalse(true,"没有该操作！");
                    return -1;
            }
           
            jparameter.Add("USERNAME", setuser);

            //设置用户密码
            jmdvr["PW"] = setpw;
            jmdvr["UN"] = setuser;
            jmdvr["UR"] = (int)setuserright;
            jmdvr["UD"] = 1;
            jparameter.Add("UIF", jmdvr);

            jresp = session.SendCommand(Module.DEVEMM, Operation.MANAGEUSERCMD, jparameter);
            session.Logout();
            return Convert.ToInt32(jresp["ERRORCODE"]);
        }
		[Ignore("11111")]
		[Test]
        public void UserType()
        {
            foreach (string ip in IPList)
            {
                int ret = SetUserCommon(ip,port,"admin","120223", "user", "1", UserRight.COMMON);
                Assert.AreEqual(0, ret, "admin用户设置user用户密码失败.错误码为{0}", ret);

                ret = SetUserCommon(ip,port, "user", "1","admin", "2",UserRight.MANAGE);
                Assert.AreNotEqual(0, ret, "user用户设置admin用户密码成功 ！！！不符合测试期望！！！");

                ret = SetUserCommon(ip,port,"admin","120223", "admin", "1", UserRight.MANAGE);
                Assert.AreEqual(0, ret, "admin用户设置密码为1失败.错误码为{0}", ret);

                ret = SetUserCommon(ip,port, "admin", "1","admin", "2", UserRight.MANAGE);
                Assert.AreEqual(0, ret, "admin用户设置密码为2失败.错误码为{0}", ret);
            }
        }
		[Ignore("11111")]
		[Test]
        public void UserManage()
        {
            foreach (string ip in IPList)
            {
                int ret = SetUserCommon(ip, port, "admin", "120223", "user", "1", UserRight.COMMON);
                Assert.AreEqual(0, ret, "admin用户设置user用户密码失败.错误码为{0}", ret);

                ret = SetUserCommon(ip, port, "user", "1", "admin", "2", UserRight.MANAGE);
                Assert.AreNotEqual(0, ret, "user用户设置admin用户密码成功 ！！！不符合测试期望！！！");

                ret = SetUserCommon(ip, port, "admin", "120223", "admin", "1", UserRight.MANAGE);
                Assert.AreEqual(0, ret, "admin用户设置密码为1失败.错误码为{0}", ret);

                ret = SetUserCommon(ip, port, "admin", "1", "admin", "2", UserRight.MANAGE);
                Assert.AreEqual(0, ret, "admin用户设置密码为2失败.错误码为{0}", ret);

                ret = SetUserCommon(ip, port, "admin", "120223", "user1", "1", UserRight.COMMON, OptType.ADD);
                Assert.AreEqual(0, ret, "添加user1用户失败，请查看用户是否已经超过最大数量。错误码为{0}", ret);

                ret = SetUserCommon(ip, port, "admin", "120223", "user2", "1", UserRight.COMMON, OptType.ADD);
                Assert.AreNotEqual(0, ret, "添加user2用户成功，与测试预期不符！！！");

                ret = SetUserCommon(ip, port, "admin", "120223", "user1", "1", UserRight.COMMON, OptType.DEL);
                Assert.AreEqual(0, ret, "删除user1用户失败.错误码为{0}", ret);

                ret = SetUserCommon(ip, port, "admin", "120223", "user", "2", UserRight.COMMON, OptType.DEL);
                Assert.AreEqual(0, ret, "删除user用户失败.错误码为{0}", ret);

                ret = SetUserCommon(ip, port, "admin", "120223", "admin", "1", UserRight.MANAGE, OptType.DEL);
                Assert.AreNotEqual(0, ret, "删除admin用户成功，与测试预期不符！！！");
            }
        }
		[Ignore("11111")]
		[Test]
        public void UserPermission()
        {
            foreach (string ip in IPList)
            {
                int ret = SetUserCommon(ip, port, "admin", "120223", "admin", "120223", UserRight.MANAGE);
                Assert.AreEqual(0, ret, "admin用户设置密码失败.错误码为{0}", ret);

                ret = SetUserCommon(ip, port, "admin", "120223", "user", "120223", UserRight.COMMON);
                Assert.AreEqual(0, ret, "user用户设置密码120223失败.错误码为{0}", ret);

                N9MSession session = new N9MSession(ip, port);
                session.Login("user", "120223");
                session.Logout();

                ret = SetUserCommon(ip, port, "admin", "120223", "admin", "admin", UserRight.MANAGE);
                Assert.AreEqual(0, ret, "admin用户设置错误码为{0}", ret);

                ret = SetUserCommon(ip, port, "admin", "120223", "user", "admin", UserRight.MANAGE);
                Assert.AreEqual(0, ret, "user用户设置密码admin失败.错误码为{0}", ret);

                ret = SetUserCommon(ip, port, "user", "admin", "admin", "2", UserRight.MANAGE, OptType.DEL);
                Assert.AreNotEqual(0, ret, "user用户设置admin用户密码成功，与测试预期不符！！！");

            }
                
        }

        [Test]
        public void UserCount()
        {
            int count = 2;
            count = Convert.ToInt32(this.CFG.gNode("User/UserCount/count").InnerText);

            Console.WriteLine("count ={0}", count);

            List<N9MSession> list = new List<N9MSession>();

            foreach (string ip in IPList)
            {
                Console.WriteLine("ip ={0}", ip);

                for (int i = 0; i <= count; i++)
                {
                    Console.WriteLine("---------------------{0}----------------", i);

                    Console.WriteLine("ip ={0}", ip);

                    N9MSession session = new N9MSession(ip, port);

                    if (i == count)
                    {
                        Assert.AreEqual(24, session.Login(username, password));
                    }
                    else
                    {
                        Assert.AreEqual(0, session.Login(username, password), "登录设备失败");
                    }

                    list.Add(session);
                }
            }

            foreach(N9MSession session in list)
            {
                if (session != null)
                {
                    session.Logout();
                }
            }
        }
		[Ignore("11111")]
		[Test]
        public void UserRights()
        {
            foreach (string ip in IPList)
            {
                int ret = SetUserCommon(ip, port, "admin", "120223", "admin", "120223", UserRight.MANAGE);
                Assert.AreEqual(0, ret, "admin用户设置超级用户密码失败.错误码为{0}", ret);

                ret = SetUserCommon(ip, port, "admin", "120223", "user", "120223", UserRight.MANAGE);
                Assert.AreEqual(0, ret, "user用户设置超级用户密码失败 ！！！错误码为{0}", ret);

                do
                {
                    N9MSession session = new N9MSession(ip, port);
                    session.Login("user", "120223");
                    session.Logout();

                } while (false);

                ret = SetUserCommon(ip, port, "admin", "120223", "admin", "admin", UserRight.MANAGE);
                Assert.AreEqual(0, ret, "admin用户设置密码(admin)失败.错误码为{0}", ret);

                ret = SetUserCommon(ip, port, "admin", "120223", "user", "admin", UserRight.COMMON);
                Assert.AreEqual(0, ret, "user用户设置密码(admin)失败.错误码为{0}", ret);

                ret = SetUserCommon(ip, port, "user", "admin", "admin", "2", UserRight.MANAGE);
                Assert.AreEqual(0, ret, "user用户设置密码(admin)成功，不符合期望");
            }
        }
    }
}
