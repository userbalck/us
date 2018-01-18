using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crocus.TestUI.PulicWeb;
using NUnit.Framework;
using OpenQA.Selenium;

namespace Crocus.TestUI.Set_info
{
	class Set_info
	{

		private RMXML xmlConfig;
		IWebDriver drive;
		LocatYyaml ple;
		string pwd="120223";

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
			ple.getElement("user").Clear(); ple.getElement("pwd").Clear();//清楚用户密码
			pwd = xmlConfig.gNode("Platform/pwd").InnerText;  //获取密码
			ple.getElement("pwd").SendKeys(pwd);  //管理员密码amdin
			ple.getElement("login").Click();
			Selp.sl(3);
		}
		[SetUp]
		public void TestSetUp()
		{
			string factory = xmlConfig.gNode("Assertion/factory").InnerText;
			Console.WriteLine("获取的序列号:[0]",factory);
			ple.getElement("config").Click();
			Selp.sl(3);
			Console.WriteLine("TestSetUp-每次调用");
		}
		//[Test]
		public void Titmesz()
		{
			ple.getElement("config_font_size").Click();
			string con = ple.getElement("序列号").Text;
			Console.WriteLine("CON:",con);
			Assertion.verifyEquals(1,1);
			


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
