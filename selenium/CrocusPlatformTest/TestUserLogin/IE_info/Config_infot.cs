using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;
using System.Threading;
using Crocus.TestUI.PulicWeb;
using System.Collections;

namespace Crocus.TestUI
{
	[TestFixture]
	class Config_infot
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
			drive.Navigate().GoToUrl(weburl);       //输入网址
			ple = new LocatYyaml(drive, "Xpath.txt");          //传递webdr对象,读取文件参数
			Console.WriteLine("初始化：访问地址URL：{0}", weburl);
			// drive.Manage().Window.Maximize();   //窗口最大化
			Selp.sl(3);
			Console.WriteLine("user password");
			ple.getElement("user").Clear(); ple.getElement("pwd").Clear();//清楚用户密码
			pwd = xmlConfig.gNode("Platform/pwd").InnerText;  //获取密码
			ple.getElement("pwd").SendKeys(pwd);  //管理员密码amdin
			ple.getElement("login").Click();
			Selp.sl(3);
			
		}
		[SetUp]
		public void TestSetUp()
		{
			string ti = "View MDVR";
			string titi = drive.Title;
			Console.WriteLine("title:{0}", titi);
			Assertion.verifyEquals(ti,titi,"登陆成功或失败");			//判断登陆成功
			ple.getElement("config").Click();           //进入config页面
			Selp.sl(3);
			Console.WriteLine("Test：SetUp");
		}
		[Test]
		public void UserSet()
		{
			string factory, devid, bn, bid, ln, did, dname;//预期序列号、自编号、车牌号、自编号、线路号、司机工号、司机姓名
			ArrayList Fanem = new ArrayList();    //实际获取的值
			factory = xmlConfig.gNode("Assertion/factory").InnerText;
			Console.WriteLine("获取的序列号:{0}", factory);
			ple.getElement("config_font_size").Click();
			Selp.sl(5);
			string Fact = ple.getElement("序列号").Text;
			Fanem.Add(Fact) ;
			Console.WriteLine("实际序列号：{0}", Fact);
			Assert.AreEqual(factory, Fact);
			string fa4 = Fact.Substring(0,4);     //截取边序列
			

			ple.getElement("车牌号").Clear();//清空内容
			ple.getElement("车辆自编号").Clear();
			ple.getElement("线路号").Clear();
			ple.getElement("工号").Clear();
			ple.getElement("dname").Clear();

			Console.WriteLine("输入自编号/车牌号:{0}", fa4);
			ple.getElement("序列号").SendKeys(fa4);
			ple.getElement("车牌号").SendKeys(fa4);
			ple.getElement("车辆自编号").SendKeys("fa4");
			ple.getElement("线路号").SendKeys("fa4");
			ple.getElement("工号").SendKeys("fa4");
			ple.getElement("dname").SendKeys("fa4");
			Selp.sl(10);
		}

		[TearDown]
        public void jsRepat()
        {
            Console.WriteLine("运行TearDown");

        }
      

        [TestFixtureTearDown]
        public void js()
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
