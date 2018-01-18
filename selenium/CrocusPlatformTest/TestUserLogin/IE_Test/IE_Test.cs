using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;
using System.Threading;
using System.Collections;

namespace Crocus.TestUI
{
	[TestFixture]
	class IE_Test
	{
		private RMXML xmlConfig;
		IWebDriver drive;
        LocatYyaml pl;

		[TestFixtureSetUp]
		public void init()
		{
            //启动浏览器
            WebDriver seleiun = new WebDriver();
            drive = seleiun.getDriver();
            //读取config参数,读取ip
            this.xmlConfig = Core.GetRMXML("config.xml", true); //加载xml
            string weburl = xmlConfig.gNode("Platform/IP").InnerText; //读取xml参数
			 weburl = "www.baidu.com";
            drive.Navigate().GoToUrl(weburl);       //s输入网址
            pl = new LocatYyaml(drive, "Xpath.txt");          //传递webdr对象,读取文件参数
            Console.WriteLine("访问地址URL：{0}", weburl);
			//drive.Manage().Window.Maximize();
			Selp.sl(5);
        }
        [Test]
        public void Baidu() {
			ArrayList Fanem = new ArrayList();
			string[] fa = new string[7];

			pl.getElement("baidu").SendKeys("selenium hs");
			Selp.sl(5);
			string st = pl.getElement("su").GetAttribute("Value");
			Fanem.Add(st);
			fa[0] = st;
			fa[1] = "123";
			fa[1] = "123";
			Console.WriteLine("获取Text：{0}", st);
			Console.WriteLine(st);
			Fanem.Add(st);
            pl.getElement("su").Click();
            Selp.sl(1);
			for (int i = 0; i < fa.Length; i++)
			{

			}
			foreach (var item in Fanem)
			{
				string ni = item.ToString();
				Console.WriteLine("fanme[]:", ni);
			}

        }

        [TearDown]
        public void jsRepat()
        {
            Console.WriteLine("test结束运行TearDown");

        }
      

        [TestFixtureTearDown]
        public void js()
        {
            drive.Quit();
            Console.WriteLine("结束运行TestFixtureTearDown");

        }
    }

}
