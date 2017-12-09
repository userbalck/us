using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace N9MTest.SDK.http
{
    public class N9MHttpSession: IDisposable
    {
        //host地址
        private string mHost;

        //host端口
        private int mPort;

        //用户名
        private string mUsername;

        //密码
        private string mPassword;

        //cookie缓存
        CookieContainer mCookieContainer =  new CookieContainer();

        //登录代理
        private readonly string DefaultUserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";

        public N9MHttpSession(string host, int port)
        {
            mHost = host;
            mPort = port;
        }

        ~N9MHttpSession()
        {
            Dispose();
        }

        public void Dispose()
        {
            Logout();
        }

        public string GetUrl(string relativeUrl)
        {
            string url = string.Format("http://{0}:{1}{2}", mHost, mPort, relativeUrl);
            return url;
        }


        public int Login(string relativeUrl, string username, string password)
        {
            Console.WriteLine("url = {0}", GetUrl(relativeUrl));
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(GetUrl(relativeUrl));
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.AllowWriteStreamBuffering = false;//禁用数据缓存
            request.AllowAutoRedirect = false;
            request.CookieContainer = mCookieContainer;
            request.KeepAlive = true;
            request.UserAgent = DefaultUserAgent;

            string contentData = String.Format("username={0}&userpwd={1}&autologin=1", username, password);

            request.ContentLength = Encoding.UTF8.GetByteCount(contentData);
            Stream rs = request.GetRequestStream();
            rs.Write(Encoding.UTF8.GetBytes(contentData),0, Encoding.UTF8.GetByteCount(contentData));
            rs.Close();

            HttpWebResponse response = null;

            response = (HttpWebResponse)request.GetResponse();

            Console.WriteLine("response.Cookies.Count = {0}", response.Cookies.Count);

            for (int i = 0; i < response.Cookies.Count; i++)
            {
                Cookie cookie = response.Cookies[i];
                Console.WriteLine("[0]cookie.Name = {1}, cookie.Value= {2}", i, cookie.Name, cookie.Value);
            }

            mCookieContainer.Add(response.Cookies);

            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string resp = sr.ReadToEnd();
            sr.Close();
            response.Close();

            Console.WriteLine("resp = {0}", resp);

            if (response.Cookies.Count == 0)
            {
                return -1;
            }
            else
            {

                mUsername = username;
                mPassword = password;

                return 0;
            }
        }

        public int Logout(string relativeUrl = "/logincheck.rsp?method=logout")
        {
            Get(relativeUrl);
            mCookieContainer = new CookieContainer();
            return 0;
        }

        public string Get(string relativeUrl)
        {
            Console.WriteLine("url = {0}", GetUrl(relativeUrl));
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(GetUrl(relativeUrl));
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            request.AllowWriteStreamBuffering = false;//禁用数据缓存
            request.AllowAutoRedirect = false;
            request.CookieContainer = mCookieContainer;
            request.KeepAlive = true;
            request.UserAgent = DefaultUserAgent;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string resp = sr.ReadToEnd();
            sr.Close();
            response.Close();

            Console.WriteLine("resp = {0}", resp);

            return resp;
        }

        public string Post(string relativeUrl)
        {
            return null;
        }

        public string PostFile(string relativeUrl, string path)
        {
            string bound = "---------------" + DateTime.Now.Ticks.ToString("x");
            string endMark = "\r\n";

            Console.WriteLine("url = {0}", GetUrl(relativeUrl));
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(GetUrl(relativeUrl));
            request.Method = "POST";
            request.ContentType = "multipart/form-data; boundary=" + $"{bound}";
            request.AllowWriteStreamBuffering = false;//禁用数据缓存
            request.AllowAutoRedirect = false;
            request.CookieContainer = mCookieContainer;
            request.KeepAlive = true;
            request.UserAgent = DefaultUserAgent;

            Console.WriteLine("path = {0}", path);

            FileStream file = null;

            try
            {
                file = new FileStream(path, FileMode.Open);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("e = {0}", e.ToString());
            }
            catch (SecurityException e)
            {
                Console.WriteLine("e = {0}", e.ToString());
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("e = {0}", e.ToString());
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine("e = {0}", e.ToString());
            }
            catch (PathTooLongException e)
            {
                Console.WriteLine("e = {0}", e.ToString());
            }

            MemoryStream ms = new MemoryStream();

            string header = string.Format("--" + $"{bound}");
            header += endMark;

            header += string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"", "upgrade_file", Path.GetFileName(path));

            header += endMark;

            header += "Content-Type: application/octet-stream";
            header += endMark;
            header += endMark;

            byte[] byteHeader = Encoding.UTF8.GetBytes(header);
            ms.Write(byteHeader, 0, byteHeader.Length);


            BinaryReader reader = new BinaryReader(file);
            ms.Write(reader.ReadBytes((int)file.Length), 0, (int)file.Length);

            ms.Write(Encoding.UTF8.GetBytes(endMark), 0, Encoding.UTF8.GetBytes(endMark).Length);


            string tail = String.Format("--" + $"{bound}" + "--");

            ms.Write(Encoding.UTF8.GetBytes(tail), 0, Encoding.UTF8.GetBytes(tail).Length);

            reader.Close();
            file.Close(); 

            Console.WriteLine("ms length = {0}", ms.Length);

            request.ContentLength = ms.Length;

            Stream rs = request.GetRequestStream();

            rs.Write(ms.GetBuffer(), 0, (int)ms.Length);

            ms.Close();
            rs.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string resp = sr.ReadToEnd();
            sr.Close();
            response.Close();

            Console.WriteLine("resp = {0}", resp);

            return resp;
        }

        public bool GetFile(string relativeUrl, ref string path)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(GetUrl(relativeUrl));
            request.Method = "GET";
            request.AllowWriteStreamBuffering = false;//禁用数据缓存
            request.AllowAutoRedirect = true;
            request.CookieContainer = mCookieContainer;
            request.KeepAlive = true;
            request.UserAgent = DefaultUserAgent;
            
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Console.WriteLine("StatusCode = {0}", response.StatusCode);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return false;
            }

            Console.WriteLine("Content-Disposition = {0}", response.GetResponseHeader("Content-Disposition"));

            string contentDisposition = response.GetResponseHeader("Content-Disposition");

            bool IsAttach = contentDisposition.Split(';')[0].Equals("attachment");
            string filename = contentDisposition.Split(';')[1].Split('=')[1];

            Console.WriteLine("path = {0} filename = {1}", path, filename);

            path = Path.Combine(path, filename);
            path = Path.ChangeExtension(path, ".config");

            Console.WriteLine("path = {0}", path);

            FileStream fs = new FileStream(path, FileMode.Create);
            BinaryWriter binaryWriter = new BinaryWriter(fs);


            Stream rs = response.GetResponseStream();

            byte[] buffer = new byte[1024];

            int nReadLen = 0;

            while ((nReadLen = rs.Read(buffer, 0, 1024)) > 0)
            {
                binaryWriter.Write(buffer, 0, nReadLen);
            }
            
            rs.Close();
            fs.Close();
            binaryWriter.Close();
            response.Close();

            return true;
        }

        public string GetCookieValue(string name)
        {
            if (mCookieContainer == null)
            {
                return null;
            }

            List<Cookie> list = GetAllCookies(mCookieContainer);

            foreach (Cookie cookie in list)
            {
                if (cookie.Name.Equals(name))
                {
                    return cookie.Value;
                }
            }

            return "";
        }

        private static List<Cookie> GetAllCookies(CookieContainer cc)
        {
            List<Cookie> lstCookies = new List<Cookie>();
            Hashtable table = (Hashtable)cc.GetType().InvokeMember("m_domainTable",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField |
                System.Reflection.BindingFlags.Instance, null, cc, new object[] { });
            foreach (object pathList in table.Values)
            {
                SortedList lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField
                    | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { });
                foreach (CookieCollection colCookies in lstCookieCol.Values)
                    foreach (Cookie c in colCookies) lstCookies.Add(c);
            }
            return lstCookies;
        }
    }
}
