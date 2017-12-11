using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using OpenQA.Selenium.Interactions;

namespace Crocus.TestUI
{
    public class Core
    {
        public static Random random = GetRandom();
        public static string WorkPath = $"{AppDomain.CurrentDomain.BaseDirectory}";
        
        public enum eDriver
        {
            IE, Chrome
        }

        public static RMXML GetRMXML(string fileorxml, bool isfile)
        {
            if (isfile)
				return (new RMXML($"{Core.WorkPath}\\{fileorxml}", isfile));
            else return (new RMXML(fileorxml, isfile));
        }

        public static void Print(string s)
        {
            Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}] {s}");
        }

        /// <summary>
        /// 获取一个随机数
        /// </summary>
        /// <returns></returns>
        private static Random GetRandom()
        {
            long tick = DateTime.Now.Ticks;
            return (new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32)));
        }
        
        public static IWebDriver GetDriver(eDriver edriver)
        {
            if (edriver == eDriver.Chrome) return (new OpenQA.Selenium.Chrome.ChromeDriver());
            else if (edriver == eDriver.IE) return (new OpenQA.Selenium.IE.InternetExplorerDriver());
            return (null);
        }
        
    }

    public class XWebElements
    {
        private By _by;
        private OpenQA.Selenium.IWebDriver _dr;
        private XWebElement parentIframe;
        private TimeSpan tsWaitInterval = TimeSpan.FromMilliseconds(100);
        private TimeSpan tsWait = TimeSpan.FromSeconds(10);
        public ReadOnlyCollection<IWebElement> es
        {
            get
            {
                ReadOnlyCollection<IWebElement> _es = null;
                if (this.parentIframe == null) this._dr.SwitchTo().DefaultContent();
                else this._dr.SwitchTo().Frame(this.parentIframe.e);
                WebDriverWait wait = getWaitObject();
                wait.Until<bool>(dr => {
                    try
                    {
                        _es = this._dr.FindElements(this._by);
                        return (true);
                    }
                    catch (Exception ex)
                    {
                        Core.Print(ex.Message); return (false);
                    }
                });
                return (_es);
            }
        }

        public XWebElements(OpenQA.Selenium.IWebDriver dr, By by, XWebElement pIframe = null)
        {
            this._by = by; this._dr = dr; this.parentIframe = pIframe;
        }
        public XWebElements(OpenQA.Selenium.IWebDriver dr, string xpath, XWebElement pIframe = null) : this(dr, By.XPath(xpath), pIframe) { }

        private WebDriverWait getWaitObject()
        {
            WebDriverWait wait = new WebDriverWait(this._dr, tsWait);
            wait.PollingInterval = tsWaitInterval;
            wait.Message = $"Time out {tsWait.TotalSeconds}, xpath is : {_by.ToString()}";
            return (wait);
        }
        /// <summary>
        /// WaitIncludeText
        /// </summary>
        /// <returns></returns>
        public bool wt(int idx, string s)
        {
            WebDriverWait wait = getWaitObject();
            return (wait.Until(ExpectedConditions.TextToBePresentInElement(this.es[idx], s)));
        }
        /// <summary>
        /// WaitNotIncludeText
        /// </summary>
        /// <returns></returns>
        public bool wnt(int idx, string s)
        {
            WebDriverWait wait = getWaitObject();
            return (wait.Until(dr =>
            {
                try
                {
                    if (!this.es[idx].Text.Contains(s)) return (true);
                }
                catch (Exception ex)
                {
                    Core.Print($"wnt, {idx}, {this._by.ToString()}, {ex.Message}");
                }
                return (false);
            }));
        }
        
        /// <summary>
        /// WaitVisible
        /// </summary>
        /// <returns></returns>
        public bool wv(int idx)
        {
            WebDriverWait wait = getWaitObject();
            return (wait.Until(dr =>
            {
                if (this.es[idx].Displayed) return (true);
                else return (false);
            }));
        }

        /// <summary>
        /// WaitNotVisible
        /// </summary>
        /// <returns></returns>
        public bool wnv(int idx)
        {
            WebDriverWait wait = getWaitObject();
            return (wait.Until(dr =>
            {
                if (!this.es[idx].Displayed) return (true);
                else return (false);
            }));
        }

        public int Index(IWebElement ele)
        {
            return (this.es.IndexOf(ele));
        }

        public XWebElements click(int idx)
        {
            WebDriverWait wait = getWaitObject();
            wait.Until(dr =>
            {
                try { this.es[idx].Click(); return (true); }
                catch (Exception ex) { Core.Print($"wait click, {idx}, {ex.Message}"); return (false); }
            });
            return (this);
        }

        public XWebElements mmove(int idx)
        {
            WebDriverWait wait = getWaitObject();
            wait.Until(dr =>
            {
                try
                {
                    new Actions(this._dr).MoveToElement(this.es[idx], 5, 5).Build().Perform();
                    return (true);
                }
                catch (Exception ex) { Core.Print($"wait mmove, {idx}, {ex.Message}"); return (false); }
            });
            return (this);
        }
        
    }

    public class XWebElement
    {
        private By _by;
        private OpenQA.Selenium.IWebDriver _dr;
        private XWebElement parentIframe;
        public IWebElement e
        {
            get
            {
                IWebElement _e = null;
                if (this.parentIframe == null) this._dr.SwitchTo().DefaultContent();
                else try { this._dr.SwitchTo().Frame(this.parentIframe.e); }
                    catch (Exception) { }
                WebDriverWait wait = getWaitObject();
                wait.Until<bool>(dr => {
                    try
                    {
                        _e = this._dr.FindElement(this._by); return (true);
                    }
                    catch (Exception ex)
                    {
                        Core.Print(ex.Message); return (false);
                    }
                });
                return (_e);
            }
        }
        private TimeSpan tsWaitInterval = TimeSpan.FromMilliseconds(100);
        private TimeSpan tsWait = TimeSpan.FromSeconds(10);

        public XWebElement(OpenQA.Selenium.IWebDriver dr, By by, XWebElement pIframe = null)
        {
            this._by = by; this._dr = dr; this.parentIframe = pIframe;
        }
        public XWebElement(OpenQA.Selenium.IWebDriver dr, string xpath, XWebElement pIframe = null) : this(dr, By.XPath(xpath), pIframe) { }
        
        private WebDriverWait getWaitObject()
        {
            WebDriverWait wait = new WebDriverWait(this._dr, tsWait);
            wait.PollingInterval = tsWaitInterval;
            wait.Message = $"Time out {tsWait.TotalSeconds}, xpath is : {_by.ToString()}";
            return (wait);
        }
 
        /// <summary>
        /// WaitIncludeText
        /// </summary>
        /// <returns></returns>
        public bool wt(string s)
        {
            WebDriverWait wait = getWaitObject();
            return (wait.Until(ExpectedConditions.TextToBePresentInElement(this.e, s)));
        }

        /// <summary>
        /// WaitNotIncludeText
        /// </summary>
        /// <returns></returns>
        public bool wnt(string s)
        {
            WebDriverWait wait = getWaitObject();
            return (wait.Until(dr =>
            {
                try
                {
                    if (!this.e.Text.Contains(s)) return (true);
                }
                catch (Exception ex)
                {
                    Core.Print($"wnt, {this._by.ToString()}, {ex.Message}");
                }
                return (false);
            }));
        }

        /// <summary>
        /// WaitVisible
        /// </summary>
        /// <returns></returns>
        public bool wv()
        {
            WebDriverWait wait = getWaitObject();
            return (wait.Until(dr =>
            {
                if (this.e.Displayed) return (true);
                else return (false);
            }));
        }

        /// <summary>
        /// WaitNotVisible
        /// </summary>
        /// <returns></returns>
        public bool wnv()
        {
            WebDriverWait wait = getWaitObject();
            return (wait.Until(dr =>
            {
                if (!this.e.Displayed) return (true);
                else return (false);
            }));
        }

        public XWebElement click()
        {
            WebDriverWait wait = getWaitObject();
            wait.Until(dr =>
            {
                try { this.e.Click(); return (true); }
                catch (Exception ex) { Core.Print($"wait click, {ex.Message}"); return (false); }
            });
            return (this);
        }

        public XWebElement mmove()
        {
            WebDriverWait wait = getWaitObject();
            wait.Until(dr =>
            {
                try
                {
                    new Actions(this._dr).MoveToElement(this.e, 5, 5).Build().Perform();
                    return (true);
                }
                catch (Exception ex) { Core.Print($"wait mmove, {ex.Message}"); return (false); }
            });
            return (this);
        }

        //public static XWebElement click(XWebElement ele)
        //{
        //    WebDriverWait wait = ele.getWaitObject();
        //    wait.Until(dr =>
        //    {
        //        try
        //        {
        //            new Actions(ele._dr).MoveToElement(ele.e).Click().Build().Perform();
        //            return (true);
        //        }
        //        catch (Exception ex) { Core.Print($"wait mmove, {ex.Message}"); return (false); }
        //    });            
        //    return (ele);
        //}
    }
    

}
