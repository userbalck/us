using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;


namespace Crocus.TestUI
{
	[TestFixture]
	class IE_Test
	{
		private RMXML xmlConfig;
		private string PlatformUrl = "";
		private OpenQA.Selenium.IWebDriver driver;
		private TimeSpan tsNav;
		IWebDriver drive;


		//[TestFixtureSetUp]
		public void init()
		{

			
			this.xmlConfig = Core.GetRMXML("config.xml", true);
			this.PlatformUrl = $"http://{this.xmlConfig.gNode("Platform/IP").InnerText}" +
				$":{this.xmlConfig.gNode("Platform/Port").InnerText}";
			Console.WriteLine(PlatformUrl + "===123");
			this.tsNav = TimeSpan.FromMilliseconds(Convert.ToInt32(this.xmlConfig.gNode("Platform/TimeSpanNav").InnerText));
			driver = Core.GetDriver(Core.eDriver.Chrome);
		
			driver.Url = this.PlatformUrl;
			Console.WriteLine("111111111111"+ driver.Url);
			
		}
		[Test]
		public void tc_UserLogin()
		{
			string weburl = "http://www.baidu.com";
			WebDriver seleiun = new WebDriver();
			drive=seleiun.getDriver();
			this.xmlConfig = Core.GetRMXML("config.xml", true); //加载xml
			 weburl = xmlConfig.gNode("Platform/IP").InnerText; //读取xml参数
			Console.WriteLine("drvce访问网站:" + drive);
			drive.Navigate().GoToUrl(weburl);  //访问网站
			drive.FindElement(By.Id("kw")).SendKeys("heishou");
            drive.FindElement(By.XPath(".//*[@id='username']")).Click();
			
		}
		[Test]
		public void TestLoad()
		{
		
			String path = "Xpath.txt";
			StreamReader srd = new StreamReader(path, Encoding.UTF8);
			string ph = srd.ReadToEnd();
			srd.Close();
			string[] phXpath = ph.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			
			for (int i = 0; i<phXpath.Length; i++)
			{
				
				Console.WriteLine("-------------"+i);
				Console.WriteLine(i + phXpath[i]);
				string[] datas = phXpath[i].Split(new string[] { ":" }, StringSplitOptions.None);
				String  user= "密码";
				if (datas[0].Equals(user))
				{
					Console.WriteLine(datas[1]);
					Console.WriteLine(datas[2]);
				}
			}
		}
	}

}
