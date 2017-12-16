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
            drive.Navigate().GoToUrl(weburl);       //s输入网址
            pl = new LocatYyaml(drive, "Xpath.txt");          //传递webdr对象,读取文件参数
            Console.WriteLine("访问地址URL：{0}", weburl);
           
        }
        [Test]
        public void Baidu() {
            pl.getElement("baidu").SendKeys("selenium hs");
            pl.getElement("su").Click();
            sl(10);
        }

        [TearDown]
        public void jsRepat()
        {
            Console.WriteLine("test结束运行TearDown");

        }
        public void sl(int s)
        {
            int ss = s * 1000;
            int t = ss / 1000;
            Console.WriteLine("等待{0}秒......",t);
            Thread.Sleep(ss);
        }

        [TestFixtureTearDown]
        public void js()
        {
            drive.Quit();
            Console.WriteLine("结束运行TestFixtureTearDown");

        }
    }

}
