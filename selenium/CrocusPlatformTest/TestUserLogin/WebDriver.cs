using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Firefox;

namespace Crocus.TestUI
{

	class WebDriver
	{
		static IWebDriver drvier;
		private RMXML xmlConfig;
		

		/// <summary>
		/// 封装浏览器启动
		/// </summary>
		/// <returns></returns>

		public IWebDriver getDriver() {

			return drvier;
		}
		public WebDriver()
		{
			this.isntatWebDrvier();

		}
		public void isntatWebDrvier()

		{
			this.xmlConfig = Core.GetRMXML("config.xml", true); //加载xml
			String Browser= xmlConfig.gNode("Platform/Browser").InnerText;
			Console.WriteLine("浏览器："+Browser);

			if ("google".Equals(Browser))
			{
				drvier = new ChromeDriver();
				Console.WriteLine("goole浏览器：" + Browser);
			}
			else if ("ie".Equals(Browser))
			{
                InternetExplorerOptions options = new InternetExplorerOptions();
                 options.IntroduceInstabilityByIgnoringProtectedModeSettings = true;
                drvier = new InternetExplorerDriver(options);
                Console.WriteLine("ie浏览器：" + Browser);
            }
            else if ("fox".Equals(Browser))
            {
               // drvier = new FirefoxDriver();
            }
			else {

				Console.WriteLine("浏览器配置错误：" + Browser);
			}
			


		}

		
	}
}
