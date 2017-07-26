using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RM
{
    public class RMHttp
    {
        private Encoding _encoding = Encoding.UTF8;
        private int _timeout = 30;
        private string _baseurl = "http://1.1.1.1";
        public delegate void DOutMsg(string s);
        public event DOutMsg OnOutMsg = delegate (string s)
        {
            Core.Print(s);
        };
        private string _ip; private int _port = 80;

        public RMHttp(string iporurl, bool isIP = true)
        {
            if (isIP)
            {
                this._baseurl = $"http://{iporurl}:{_port}"; this._ip = iporurl;
            }
            else
            {
                this._baseurl = $"http://{iporurl}";
            }
        }
        public RMHttp(string ip, int port)
        {
            this._baseurl = $"http://{ip}:" + port; this._ip = ip; this._port = port;
        }
        private string Url(string url)
        {
            return ($"{this._baseurl}/{url}");
        }
        public RMXML GetResultRMXML(string url)
        {
            return new RMXML(this.Get(url), false);
        }
        /// <summary>
        /// Full url : $"http://{ip}/time/current"
        /// </summary>
        /// <param name="url">sample : "time/current"</param>
        /// <returns></returns>
        public string Get(string url)
        {
            string _url = this.Url(url);
            if (this.OnOutMsg != null) { this.OnOutMsg($"REIHttp.Url[GET] : {_url}"); }
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(_url);
            myRequest.Timeout = this._timeout * 1000;
            //获取web请求的响应的内容
            HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
            //通过响应流构造一个StreamReader
            StreamReader reader = new StreamReader(myResponse.GetResponseStream(), this._encoding);
            //string ReturnXml = HttpUtility.UrlDecode(reader.ReadToEnd());
            string result = reader.ReadToEnd();
            reader.Close(); myResponse.Close();
            if (this.OnOutMsg != null) { this.OnOutMsg($"REIHttp[GET], Result = {result}"); }
            return result;
        }
        public struct GetStreamResult
        {
            public string file; public long totalsize; public double timelen;
        }
        /// <summary>
        /// Full url : $"http://{ip}/time/current"
        /// </summary>
        /// <param name="url">sample : "time/current"</param>
        /// <param name="ShowProgress">是否打印进度，会影响下载速度</param>
        /// <param name="file">为""时，不保存文件</param>
        /// <returns></returns>
        public GetStreamResult GetStream(string url, bool ShowProgress, string file)
        {
            string _url = this.Url(url); GetStreamResult result = new GetStreamResult();
            if (this.OnOutMsg != null) this.OnOutMsg("REIHttp.Url[GET,Stream] : " + _url);
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(_url);
            myRequest.Timeout = 1000 * 3600 * 10;
            //获取web请求的响应的内容
            HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
            result.totalsize = myResponse.ContentLength;
            Core.Print("    Stream len : " + result.totalsize);
            Stream st = myResponse.GetResponseStream(); st.ReadTimeout = 1000 * 3600 * 10;
            DateTime dtstart = DateTime.Now; bool isReadFirst = false;

            FileStream fs = null;
            if (file != "") fs = new FileStream(file, FileMode.CreateNew);
            byte[] buffer = new byte[1024 * 1024 * 20];
            int readed = st.Read(buffer, 0, buffer.Length); Int64 totalreaded = 0; int per = 0;
            if (!isReadFirst)
            {
                isReadFirst = true; dtstart = DateTime.Now;  //if readed first, begin timekeeping
            }
            while (readed > 0)
            {
                if (file != "") fs.Write(buffer, 0, readed);
                if (ShowProgress)
                {
                    totalreaded += readed;
                    int newper = (int)(totalreaded * 100 / result.totalsize);
                    if (newper > per)
                    {
                        per = newper;
                        Core.Print("    Download progress is : " + per + ", "
                            + Core.FormatSize(totalreaded) + "/" + Core.FormatSize(result.totalsize)
                            + ", Speed : " + Core.FormatSize((long)(totalreaded / ((TimeSpan)(DateTime.Now - dtstart)).TotalSeconds)));
                    }
                }
                readed = st.Read(buffer, 0, buffer.Length);
            }

            result.timelen = ((TimeSpan)(DateTime.Now - dtstart)).TotalSeconds;
            long speed = (long)(result.totalsize / result.timelen);

            Core.Print("    Download progress is finished, "
                + result.totalsize + "[" + Core.FormatSize(result.totalsize) + "], Time : " + result.timelen
                + "s, Speed : " + Core.FormatSize(speed) + " (" + Core.FormatSize(speed * 8) + ")");
            if (file != "")
            {
                Core.Print("    Save file : " + file); fs.Close();
            }

            st.Close(); myResponse.Close(); return (result);
        }
        /// <summary>
        /// Full url : $"http://{ip}/time/current"
        /// </summary>
        /// <param name="data">put data</param>
        /// <param name="url">sample : "time/current"</param>
        /// <returns></returns>
        public string Put(string data, string url)
        {
            return (this._Post(data, url, "PUT"));
        }
        /// <summary>
        /// Full url : $"http://{ip}/time/current"
        /// </summary>
        /// <param name="data">put data</param>
        /// <param name="url">sample : "time/current"</param>
        /// <returns></returns>
        public string Post(string data, string url)
        {
            return (this._Post(data, url, "POST"));
        }
        /// <summary>
        /// Full url : $"http://{ip}/time/current"
        /// </summary>
        /// <param name="data">put data</param>
        /// <param name="url">sample : "time/current"</param>
        /// <returns></returns>
        public string Delete(string data, string url)
        {
            return (this._Post(data, url, "DELETE"));
        }
        /// <summary>
        /// Full url : $"http://{ip}/time/current"
        /// </summary>
        /// <param name="data"post data</param>
        /// <param name="url">sample : "time/current"</param>
        /// <returns></returns>
        public RMXML PostResultRXML(string data, string url)
        {
            string rs = this.Post(data, url); return (new RMXML(rs, false));
        }
        /// <summary>
        /// Full url : $"http://{ip}/time/current"
        /// </summary>
        /// <param name="url">sample : "time/current"</param>
        /// <returns></returns>
        public string Delete(string url)
        {
            string _url = this.Url(url);
            if (this.OnOutMsg != null) this.OnOutMsg("REIHttp.Url[DELETE] : " + _url);
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(_url);
            myRequest.Timeout = this._timeout * 1000;
            myRequest.Method = "DELETE";
            // 获得接口返回值68
            HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
            StreamReader reader = new StreamReader(myResponse.GetResponseStream(), this._encoding);
            string result = reader.ReadToEnd();
            reader.Close(); myResponse.Close();
            return result;
        }
        private string _Post(string data, string url, string type)
        {
            string _url = this.Url(url);
            if (this.OnOutMsg != null)
            {
                this.OnOutMsg("REIHttp.Url[" + type + "] : " + _url + ", data.Length = " + data.Length);
                this.OnOutMsg("REIHttp.Url[" + type + "], Data = " + data);
            }
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(_url);
            myRequest.Timeout = this._timeout * 1000 * 2;
            //转成网络流
            byte[] buf = this._encoding.GetBytes(data);
            //设置
            myRequest.Method = type;
            myRequest.ContentLength = buf.Length;
            myRequest.ContentType = "application/json";
            myRequest.MaximumAutomaticRedirections = 1;
            myRequest.AllowAutoRedirect = true;
            // 发送请求
            Stream newStream = myRequest.GetRequestStream();
            newStream.Write(buf, 0, buf.Length);
            newStream.Close();
            // 获得接口返回值
            HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
            //直到request.GetResponse()程序才开始向目标网页发送Post请求
            StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
            string result = reader.ReadToEnd();
            reader.Close(); myResponse.Close();
            if (this.OnOutMsg != null && result != "") this.OnOutMsg("REIHttp[" + type + "], Result = " + result);
            return result;
        }
        /// <summary>
        /// Full url : $"http://{ip}/time/current"
        /// </summary>
        /// <param name="file">upload file full path</param>
        /// <param name="url">sample : "time/current"</param>
        public void PostFile(string file, string url)
        {
            TcpClient client = new TcpClient(this._ip, 80);
            FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);

            StringBuilder sb = new StringBuilder();
            sb.Append("POST /" + url + " HTTP/1.1\r\n");
            sb.Append("Host: " + this._ip + "\r\n");
            sb.AppendFormat("Content-Length: {0}\r\n", new FileInfo(file).Length);
            sb.Append("Content-Type: application/octet-stream\r\n");
            sb.Append("Connection: keep-alive\r\n");
            sb.Append("User-Agent: REI\r\n");
            sb.Append("\r\n");
            byte[] buffer = Encoding.UTF8.GetBytes(sb.ToString());
            client.Client.Send(buffer);

            long totalreaded = 0; int unit = 1024 * 1024; int readed = 0; int sended = 0;
            while (true)
            {
                if (fs.Length == totalreaded) break;
                if (fs.Length - totalreaded < unit)
                {
                    unit = (int)(fs.Length - totalreaded);
                    if (unit > 64 * 1024) unit = 64 * 1024;
                    else
                    {
                        if (unit % (4 * 1024) > 0) unit = (unit / (4 * 1024) + 1) * (4 * 1024);
                    }
                }
                buffer = new byte[unit];
                readed = fs.Read(buffer, 0, buffer.Length);
                sended = client.Client.Send(buffer);
                totalreaded += readed;
                Core.Print("Sended : fs.Length = " + fs.Length + "unit = " + unit + " , Post = " + (totalreaded * 100 / fs.Length) + " %");
            }
            //buffer = Encoding.UTF8.GetBytes("aaaaaaaaaa");
            Core.Print("Send is finished!");
            fs.Close();

            buffer = new byte[1024];
            client.Client.Receive(buffer);
            Core.Print("Send is finished : " + Encoding.UTF8.GetString(buffer));
            client.Close();

        }
    }
}
