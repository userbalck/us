using System;
using System.Collections.Generic;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Text;
using System.Threading;

namespace Crocus.TestUI
{
	class LocatYyaml
	{
		private RMXML xmlConfig;

        String type,value;
		private IWebDriver driver;
        string ph;

        public LocatYyaml(IWebDriver driver,String xpath)
		{
			this.driver = driver;
            //加载文件并读取
            Console.WriteLine("xpath:"+xpath);
            PathWrie(xpath);
        }
        public void PathWrie(String xpath) {

            String path = xpath;
			
            StreamReader srd = new StreamReader(path, Encoding.UTF8);
            ph = srd.ReadToEnd();
            srd.Close();
        }
		//封装读取元素内容type，value
        public void TestLoad(String key)
        {
            string[] phXpath = ph.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < phXpath.Length; i++)
            {
                string[] datas = phXpath[i].Split(new string[] { ":" }, StringSplitOptions.None);    
                if (datas[0].Equals(key))
                {
                    type = datas[1];
                    value = datas[2];

                }
                
            }
            if (type==null)
            {
                Console.WriteLine("Error未找到:{0}",key);
            }
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
        // 添加的map集合
        public IWebElement getElement(String key)
		{
         TestLoad(key);
        Console.WriteLine("获取的值=type:{0},value{1}", type, value);
           //return driver.FindElement(this.getBy(type, value));
		return this.waitForElement(this.getBy(type, value));    //隐式等待
		}

        //等待元素时间
        private IWebElement waitForElement(By by)
        {
            IWebElement myDynamicElement = null;
            xmlConfig = Core.GetRMXML("config.xml", true); //加载xml
            Double Time = Convert.ToInt32(xmlConfig.gNode("Platform/Time").InnerText);    //获取等待时间，强转
            try
            { 
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(Time));
                myDynamicElement = wait.Until<IWebElement>((d) =>
                {
                    return d.FindElement(by);  //定位到，并返回
                });
            }
            catch (Exception)
            {
                Console.WriteLine("超时{1} ，不存在：{0}", by.ToString(),Time);
            }
            return myDynamicElement;
        }
       
    }
}
