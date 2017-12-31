using System;
using Crocus.TestUI;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
namespace Andorid_Mdvr.DvrTest
{

    class DvrTest
	{
        AppiumDriver driver123;
		IWebDriver driver;
		LocatYyaml pl;
        string path;

		[TestFixtureSetUp]
		public void Fixture()
		{
			path = @"D:\Gitus\selenium\Vs\Andorid_Mdvr\Andorid_Mdvr\bin\Debug\MDVR.apk";
			Console.WriteLine("TearDown");
			DesiredCapabilities capabilities = new DesiredCapabilities();
			capabilities.SetCapability("platformName", "Android");
			capabilities.SetCapability("platformVersion", "4.4");//手机操作系统版本
			capabilities.SetCapability("automationName", "selendroid");  //你想使用的自动化测试引擎：Appium (默认) 或 Selendroid
			capabilities.SetCapability("deviceName", " Android Emulator");
			capabilities.SetCapability("app", path);
			driver = new RemoteWebDriver(new Uri("http://127.0.0.1:4723/wd/hub"), capabilities);
            //pl = new LocatYyaml(driver, "Testxpath.txt");          //传递webdr对象,读取文件参数
            Wait.sp(5);
            Console.WriteLine("初始化完成");
		}
        [Test]
        public void loginTest()
        {
            Console.WriteLine("loginTest");
            //登陆用例
          //driver.FindElementById("com.streamaxtech.mdvr.direct:id/device_ip");
            Console.WriteLine("login---el:");
            driver.FindElement(By.Id("com.streamaxtech.mdvr.direct:id/device_ip")).Clear();

            Wait.sp(5);
            Console.WriteLine("loginTest   ss:");
        }
        [Test]
		public void login()
		{
            Console.WriteLine("login");
            //登陆用例
           // driver.FindElementById("com.streamaxtech.mdvr.direct:id/device_ip");
            Console.WriteLine("login---el:" );
            driver.FindElement(By.Id("com.streamaxtech.mdvr.direct:id/device_ip")).Clear();

            Wait.sp(5);
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
