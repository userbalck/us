using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using N9MTest.SDK.net;
using System.Threading;
using RM;
using N9MTest.commons;
using System.Reflection;
using N9MTest.SDK.Tooling;

namespace N9MTest
{
    [Author("wutaihua", "thwu@streamax.com")]
    public class TestCase_Basecase
    {
        protected TestCase_Basecase()
        {
            this.fun = new FUN();
			this.CFG = fun.XMlConfig;
        }

        protected TModule tModule;
        protected RMXML CFG;
        protected FUN fun;

        //传入参数
        protected List<string> IPList = new List<string>();
        protected int webport = 80;
        protected int port = 9006;
        protected string username = "admin";
        protected string password = "120223";

        //工装的IP和端口
        protected string m_ToolingIP = "192.168.50.255";
        protected int m_ToolingPort = 7000;

        public void XmlHeaderInit()
        {
            Console.WriteLine("[XmlHeaderInit]");
            if (this.CFG.getIPs("pre/IPs") != null)
            {
                IPList.AddRange(this.CFG.getIPs("pre/IPs"));
                Console.WriteLine("[XmlHeaderInit]IPList 获取成功");
            }


            if (this.CFG.gNode("pre/port") != null)
            {
                port = Convert.ToInt32(this.CFG.gNode("pre/port").InnerText);
                Console.WriteLine("[XmlHeaderInit]媒体端口获取成功");
            }

            if (this.CFG.gNode("pre/webport") != null)
            {
                webport = Convert.ToInt32(this.CFG.gNode("pre/webport").InnerText);
                Console.WriteLine("[XmlHeaderInit]web端口获取成功");
            }

            if (this.CFG.gNode("pre/username") != null)
            {
                username = this.CFG.gNode("pre/username").InnerText;
            }

            if (this.CFG.gNode("pre/password") != null)
            {
                password = this.CFG.gNode("pre/password").InnerText;
            }

            if (this.CFG.gNode("pre/Tooling/ToolingIP") != null)
            {
                m_ToolingIP = this.CFG.gNode("pre/Tooling/ToolingIP").InnerText;
            }

            if (this.CFG.gNode("pre/Tooling/ToolingPort") != null)
            {
                m_ToolingPort = Convert.ToInt32(this.CFG.gNode("pre/Tooling/ToolingPort").InnerText);
            }

            Console.WriteLine("[xml header]IPList.count ={0}", IPList.Count);
            Console.WriteLine("[xml header]port ={0}", port);
            Console.WriteLine("[xml header]webport ={0}", webport);
            Console.WriteLine("[xml header]username ={0}", username);
            Console.WriteLine("[xml header]password ={0}", password);

            Console.WriteLine("[xml header]m_ToolingIP = {0}", m_ToolingIP);
            Console.WriteLine("[xml header]m_ToolingPort = {0}", m_ToolingPort);
        }


        [TestFixtureSetUp]
        public void Init()
        {
            Console.WriteLine("[TestFixtureSetUp]");

            XmlHeaderInit();

            this.tModule = (TModule)Enum.Parse(typeof(TModule), this.GetType().Namespace.Split('.')[1]);

            Console.WriteLine("版本{0} 发布日期:{1}",
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                System.IO.File.GetLastWriteTime(this.GetType().Assembly.Location));

            if (this.CFG.getIPs(this.tModule.ToString() + "/pre/IPs") != null && this.CFG.getIPs(this.tModule.ToString() + "/pre/IPs").Length > 0)
            {
                IPList.Clear();
                IPList.AddRange(this.CFG.getIPs(this.tModule.ToString() + "/pre/IPs"));
                Console.WriteLine("ip 获取成功");
            }

            
            if(this.CFG.gNode(this.tModule.ToString() + "/pre/port") != null)
            {
                if (this.CFG.gNode(this.tModule.ToString() + "/pre/port").InnerText.Length > 0)
                {
                    port = Convert.ToInt32(this.CFG.gNode(this.tModule.ToString() + "/pre/port").InnerText);
                    Console.WriteLine("媒体端口获取成功");
                }
            }
            

            

            if (this.CFG.gNode(this.tModule.ToString() + "/pre/webport") != null)
            {
                if (this.CFG.gNode(this.tModule.ToString() + "/pre/webport").InnerText.Length > 0)
                {
                    webport = Convert.ToInt32(this.CFG.gNode(this.tModule.ToString() + "/pre/webport").InnerText);
                    Console.WriteLine("web端口获取成功");
                }
            }

            if (this.CFG.gNode(this.tModule.ToString() + "/pre/username") != null && this.CFG.gNode(this.tModule.ToString() + "/pre/username").InnerText.Length > 0)
            {
                username = this.CFG.gNode(this.tModule.ToString() + "/pre/username").InnerText;
            }

            if (this.CFG.gNode(this.tModule.ToString() + "/pre/password") != null && this.CFG.gNode(this.tModule.ToString() + "/pre/password").InnerText.Length > 0)
            {
                password = this.CFG.gNode(this.tModule.ToString() + "/pre/password").InnerText;
            }

            Console.WriteLine("[module header]IPList.count ={0}", IPList.Count);
            Console.WriteLine("[module header]port ={0}", port);
            Console.WriteLine("[xml header]webport ={0}", webport);
            Console.WriteLine("[module header]username ={0}", username);
            Console.WriteLine("[module header]password ={0}", password);

            Console.WriteLine("[module header]m_ToolingIP = {0}", m_ToolingIP);
            Console.WriteLine("[module header]m_ToolingPort = {0}", m_ToolingPort);
        }

        [SetUp]
        public void BeforeMethod()
        {
            string name = MethodBase.GetCurrentMethod().DeclaringType.Namespace;
            Console.WriteLine("[BeforeMethod]");
        }

        [TearDown]
        public void AfterMethod()
        {
            Console.WriteLine("[AfterMethod]");
            N9MSession.clear();
            H264FileLoader.clear();
            ToolingSession.clear();
        }

        [TestFixtureTearDown]
        public void AfterClass()
        {
            Console.WriteLine("[TestFixtureTearDown]");
        }

        public void Sleep(int millisecondsTimeout)
        {
            Console.WriteLine("[{0}]进入{1}秒等待", DateTime.Now.ToString(), millisecondsTimeout/1000);
            Thread.Sleep(millisecondsTimeout);
            Console.WriteLine("[{0}]结束{1}秒等待", DateTime.Now.ToString(), millisecondsTimeout / 1000);
        }
    }
}
