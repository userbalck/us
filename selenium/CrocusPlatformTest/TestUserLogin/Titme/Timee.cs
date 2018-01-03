using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;
using System.Threading;

namespace Crocus.TestUI
{
	[TestFixture]
	class Timee
	{
		private RMXML xmlConfig;
		IWebDriver drive;
        LocatYyaml ple;
        string pwd;

       [TestFixtureSetUp]
		public void init()
		{
            //启动浏览器
            WebDriver seleiun = new WebDriver();
            drive = seleiun.getDriver();
            //读取config参数,读取ip
            this.xmlConfig = Core.GetRMXML("config.xml", true); //加载xml
            string weburl = xmlConfig.gNode("Platform/IP").InnerText; //读取xml参数
            drive.Navigate().GoToUrl(weburl);       //s输入网址
            ple = new LocatYyaml(drive, "Xpath.txt");          //传递webdr对象,读取文件参数
            Console.WriteLine("初始化：访问地址URL：{0}", weburl);
			// drive.Manage().Window.Maximize();   //窗口最大化
			Selp.sl(5);
			ple.getElement("user").Clear(); ple.getElement("pwd").Clear();//清楚用户密码
			ple.getElement("pwd").SendKeys(pwd);  //管理员密码amdin
			ple.getElement("login").Click();
			Selp.sl(3);
		}
		[SetUp]
        public void TestSetUp() {
             pwd = xmlConfig.gNode("Platform/pwd").InnerText;
            Console.WriteLine("TestSetUp");
        }
        [Test]
        public void Titmesz() {
			ple.getElement("config").Click();


        }

        [TearDown]
        public void jsRepat()
        {
            Console.WriteLine("test结束运行TearDown");

        }
      

        [TestFixtureTearDown]
        public void jsFixture()
        {
            try
            {
                drive.FindElement(By.XPath("//div[@onclick='LoginOut();']")).Click();
                Console.WriteLine("try注销");
            }
            catch (Exception)
            {
                Console.WriteLine("try注销失败");
               
            }

            drive.Quit();
            Console.WriteLine("结束运行TestFixtureTearDown");

        }

    }

}
