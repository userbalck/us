using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Util;

//本段代码中需要新增加的命名空间


namespace N9MTest.SDK.Tooling
{
    public class ToolingSession : IDisposable
    {
        public static List<ToolingSession> mlist = new List<ToolingSession>();

        public ToolingSession()
        {
            mlist.Add(this);
        }

        ~ToolingSession()
        {
            Dispose();
        }

        public void Dispose()
        {
            StopSession();
        }

        public static void clear()
        {
            foreach (ToolingSession session in mlist)
            {
                session.Dispose();
            }

            mlist.Clear();
        }

        //设备IP地址
        private string m_destIP = "";

        //设备端口
        private int m_destPort = 7000;


        //是否退出链路
        private bool m_bExit = false;

        //信令线程
        Thread m_tCommandThread = null;

        public UdpClient udpClient = null;

        public int StartSession(string ip, int port)
        {
            Console.WriteLine("StartSession");

            if (udpClient != null)
            {
                udpClient.Close();
                udpClient = null;
            }

            m_destIP = ip;
            m_destPort = port;

            IPEndPoint localIpep = new IPEndPoint(IPAddress.Parse(pubFun.GetLocalIP()), 8848);

            udpClient = new UdpClient(localIpep);

            udpClient.Client.SendTimeout = 3000;
            udpClient.Client.ReceiveTimeout = 3000;

            m_tCommandThread = new Thread(new ThreadStart(CommandThreadFun));
            m_tCommandThread.Start();

            return 0;
        }

        public int StopSession()
        {
            m_bExit = true;

            if(m_tCommandThread != null)
            {
                m_tCommandThread.Join();
                m_tCommandThread = null;
            }

            if (this.udpClient != null)
            {
                this.udpClient.Close();
                this.udpClient = null;
            }

            return 0;
        }

        public void CommandThreadFun()
        {
            Console.WriteLine("工装接收链路启动......");
            while (true)
            {
                if (m_bExit)
                {
                    break;
                }

                if(udpClient.Available == 0)
                {
                    Thread.Sleep(100);
                    continue;
                }

                IPEndPoint remoteIpep = new IPEndPoint(IPAddress.Parse(m_destIP), m_destPort); // 本机IP和监听端口号

                while (udpClient.Available > 0)
                {
                    byte[] data = udpClient.Receive(ref remoteIpep);
                    Console.WriteLine("收到的数据是{0}\r\n", Encoding.ASCII.GetString(data));
                }
            }

            Console.WriteLine("工装接收链路退出......");
        }

        private static object sign = new object();

        public int SendMessage(string msg)
        {
            Console.WriteLine("工装发送指令 msg = {0}", msg);
            msg += "\r\n";
            IPEndPoint remoteIpep = new IPEndPoint(IPAddress.Parse(m_destIP), m_destPort); // 发送到的IP地址和端口号

            lock (sign)
            {
                byte[] data = Encoding.ASCII.GetBytes(msg);
                int nSendLen = udpClient.Send(data, data.Length, remoteIpep);

                if (nSendLen != data.Length)
                {
                    return -1;
                }

                return 0;
            }
        }
    }
}
