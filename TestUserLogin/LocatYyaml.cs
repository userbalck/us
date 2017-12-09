using System;
using System.Collections.Generic;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;


namespace Crocus.TestUI
{
	class LocatYyaml
	{
		private RMXML xmlConfig;

		private Dictionary<string, Dictionary<string, string>> ml = new Dictionary<string, Dictionary<string, string>>();  //集合
		private String yamlfile;
		private IWebDriver driver;
		public LocatYyaml(IWebDriver driver)
		{
			yamlfile = "Testxpath.yaml"; //文件赋值
			this.getYamlFile();
			this.driver = driver;
		}

		private void getYamlFile()
		{
			var input = new StringReader(yamlfile);
		//	var yaml = new YamlStream();

			//TextReader textReader = new StreamReader("c:/file.xml");
			//ParseStream parser = new Yaml.ParseStream(textReader);
			throw new NotImplementedException();
		}
            

		// 添加的map集合
		public IWebElement getElement(String key)
		{
		String type = ml[key]["type"];
		String value = ml[key]["value"];
			//返回值
			
			return driver.FindElement(this.getBy(type, value));
			//return this.waitForElement(this.getBy(type, value));
		}

		private By getBy(String type, String value)
		{

			By by = null;
			if (type.Equals("id"))
			{
				by = By.Id(value);

			}
			if (type.Equals("xpath"))
			{
				by = By.XPath(value);
			}

			return by;

		}
		//调试隐式方法2
		public static void waitForPageLoa(string ID, IWebDriver driver)
		{
			WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
			IWebElement myDynamicElement = wait.Until<IWebElement>((d) =>
			{
				return d.FindElement(By.Id(ID));
			});
		}
		//调试隐式方法
		public static void waitForPageLoad(string ID, IWebDriver driver)
		{
			WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
			IWebElement myDynamicElement = wait.Until<IWebElement>((d) =>
			{
				return d.FindElement(By.Id(ID));
			});
		}

		//等待元素时间
		private IWebElement waitForElement(object p)
		{
			IWebElement element = null;
			xmlConfig = Core.GetRMXML("config.xml", true); //加载xml
			string waitTim = xmlConfig.gNode("Platform/time").InnerText;
			string waitTime = waitTim;
			double timeout = 10;

			return null;
		}


	}
}
