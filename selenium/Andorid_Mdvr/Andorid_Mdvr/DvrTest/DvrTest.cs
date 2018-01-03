using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Crocus.TestUI;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.AppiumDriver;

namespace Andorid_Mdvr.DvrTest
{

	class DvrTest
	{

		RemoteWebDriver driver;
		LocatYyaml pl;

		string path;
		[TestFixtureSetUp]
		public void Fixture()
		{
			path = @"D:\giT\us\selenium\Appium\apps\MDVR.apk";
			Console.WriteLine("TearDown");
			DesiredCapabilities capabilities = new DesiredCapabilities();
			capabilities.SetCapability("platformName", "Android");
			capabilities.SetCapability("platformVersion", "4.4");//手机操作系统版本
			capabilities.SetCapability("automationName", "selendroid");  //你想使用的自动化测试引擎：Appium (默认) 或 Selendroid
			capabilities.SetCapability("deviceName", " Android Emulator");
			capabilities.SetCapability("app", path);
			driver = new RemoteWebDriver(new Uri("http://127.0.0.1:4723/wd/hub"), capabilities);
			pl = new LocatYyaml(driver, "Testxpath.txt");          //传递webdr对象,读取文件参数
			Selp.sl(5);
			Console.WriteLine("初始化完成");
		}

		[Test]
		public void login()
		{
			//登陆用例
			driver.FindElement(By.Id("com.streamaxtech.mdvr.direct:id/device_ip")).Clear();
			Selp.sl(5);
			//String sip = pl.getElement("ip").ToString();
			Console.WriteLine("初始化完成IP");
			pl.getElement("ip").Clear();
			pl.getElement("ip").SendKeys("192.168.20.44");
			pl.getElement("pwd").Clear();
			string slogin = pl.getElement("login").ToString();
			pl.getElement("login").Click();
			Console.WriteLine("slogin:" + slogin);
		}
		[TearDown]
		public void jsRepat()
		{
			Console.WriteLine("test结束运行TearDown");

		}
		[TestFixtureTearDown]
		public void js()
		{
			driver.Quit();
			Console.WriteLine("结束运行TestFixtureTearDown");

		}
	}
}
