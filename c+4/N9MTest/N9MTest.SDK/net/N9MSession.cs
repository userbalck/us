using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Util;

using static N9MTest.SDK.net.N9MMSG;

namespace N9MTest.SDK.net
{
    public class N9MSession: IDisposable
    {
        private const string VERSION_STRING = "1.0.0";
        //建立一个自身的管理链表
        private static List<N9MSession> mlist = new List<N9MSession>();
        //设备信息
        private DeviceInfo mDeviceInfo { get; set; }

        //设备录像信息
        private RecordStatus mRecordStatus;

        //服务器连接状态
        private List<CMSConnectInfo> mCMSConnectInfoList = new List<CMSConnectInfo>();

        //IO状态
        private List<IOInfo> mIOInfoList = new List<IOInfo>();

        //设备IP地址
        private string m_destIP = "";

        //设备端口
        private int m_destPort = 9006;

        //用户名
        private string mUsername = "admin";

        //密码
        private string mPassword = "";

        //连接的Session链路id
        private string m_SessionId = "";

        //是否退出链路
        private bool m_bExit= false;

        //心跳线程
        Thread m_tHeartBeatThread = null;

        //信令线程
        Thread m_tCommandThread = null;

        //下载文件线程
        Thread m_tDownloadDataThread = null;

        //下载录像线程
        DownloadVideoThread m_tDownloadVideoThread = null;

        //下载文件的大小
        long m_nVideoFileSize = 0;

        //申请实时视频线程
        Thread m_tRequestVideoThread = null;

        //退出实时视频申请
        bool m_bExitRequestVideo = false;

        //是否退出下载线程
        bool m_bExitDownloadData = false;

        //心跳时间
        long m_heartbeatTime = 0;

        //是否链接
        bool mIsConnected = false;

        //文件列表
        List<RemoteFileInfo> mFileList =  new List<RemoteFileInfo>();

        //存储器列表
        List<StorageInfoEx> mStorageList = new List<StorageInfoEx>();

        //连接TCP
        TcpClient tcpClient = null;

        private NetworkStream stream = null;
        private BinaryReader reader = null;
        private BinaryWriter writer = null;

        Dictionary<Operation, JObject> dic = new Dictionary<Operation, JObject>();

        public delegate void SendAlarmInfo(string parameter);

        public SendAlarmInfo onSendAlarmInfo;

        public N9MSession(string ip, int port)
        {
            m_destIP = ip;
            m_destPort = port;

            mlist.Add(this);
        }

        ~N9MSession()
        {
            Dispose();
        }

        public static void clear()
        {
        //    Console.WriteLine("mlist.count = {0}", mlist.Count);
            foreach (N9MSession session in mlist)
            {
                if (session != null)
                {
                    session.Logout();
                }
            }
        }

        public DeviceTime GetDeviceTime()
        {
            if (!mIsConnected)
            {
                Console.WriteLine("mIsConnected = {0}", mIsConnected);
                return null;
            }

            JObject jresp = SendCommand(Module.DEVEMM, Operation.GETCTRLUTC, null);

            long curt = Convert.ToInt64(jresp["CURT"].ToString());
            string timez = jresp["Z"].ToString();

            DeviceTime devTime = new DeviceTime(timez, curt);
            Console.WriteLine("dtDeviceTime = {0}", devTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            return devTime;
        }

        public void Dispose()
        {
            Logout();
        } 

        public int Login(string username, string password)
        {
            mUsername = username;
            mPassword = password;

            if (tcpClient != null)
            {
                tcpClient.Close();
                tcpClient = null;
            }

            tcpClient = new TcpClient();

            tcpClient.SendTimeout = 1000;
            tcpClient.ReceiveTimeout = 100;
            tcpClient.SendBufferSize = 65536;
            tcpClient.ReceiveBufferSize = 65536;
            tcpClient.NoDelay = true;

            /*
            uint dummy = 0;
            byte[] inOptionValues = new byte[Marshal.SizeOf(dummy) * 3];
            BitConverter.GetBytes((uint)1).CopyTo(inOptionValues, 0);//是否启用Keep-Alive
            BitConverter.GetBytes((uint)3000).CopyTo(inOptionValues, Marshal.SizeOf(dummy));//多长时间开始第一次探测
            BitConverter.GetBytes((uint)3000).CopyTo(inOptionValues, Marshal.SizeOf(dummy) * 2);//探测时间间隔
            m_CommandSocket.IOControl(IOControlCode.KeepAliveValues, inOptionValues, null);

    */

            tcpClient.Connect(m_destIP, m_destPort);

            if (!tcpClient.Connected)
            {
                Console.WriteLine("m_CommandSocket.Connected failed");
                return -1;
            }

            this.stream = tcpClient.GetStream();
            this.reader = new BinaryReader(stream);
            this.writer = new BinaryWriter(stream);


            m_tCommandThread = new Thread(new ThreadStart(CommandThreadFun));
            m_tCommandThread.Start();

            JObject jparameter = new JObject();
            jparameter.Add("MODE", 0);
            JObject jresp = SendCommand(Module.CERTIFICATE, Operation.CONNECT, jparameter);

            string so = "";

            if (jresp["S0"] != null)
            {
                so = jresp["S0"].ToString();
                Console.WriteLine("so = {0}", so);
            }

            jparameter = new JObject();
            jparameter["S0"] = MD5Util.MD5Encrypt(so, so).ToLower();

            jresp = SendCommand(Module.CERTIFICATE, Operation.VERIFY, jparameter);

            int errorcode = -1;

            if (jresp["ERRORCODE"] != null)
            {
                errorcode = Convert.ToInt32(jresp["ERRORCODE"].ToString());
                Console.WriteLine("errorcode = {0}", Convert.ToInt32(jresp["ERRORCODE"].ToString()));
            }

            if (errorcode != 0)
            {
                return errorcode;
            }

            jparameter = new JObject();
            jparameter["USER"] = mUsername;
            jparameter["PASSWD"] = MD5Util.MD5Encrypt(mPassword, "streaming").ToLower();

            jresp = SendCommand(Module.CERTIFICATE, Operation.LOGIN, jparameter);

            if (jresp == null)
            {
                return -1;
            }

            if (jresp["ERRORCODE"] != null)
            {
                errorcode = Convert.ToInt32(jresp["ERRORCODE"].ToString());
                Console.WriteLine("errorcode = {0}", Convert.ToInt32(jresp["ERRORCODE"].ToString()));
            }

            if (errorcode != 0)
            {
                return errorcode;
            }

            mDeviceInfo = JsonConvert.DeserializeObject<DeviceInfo>(jresp.ToString());

            mIsConnected = true;

            m_tHeartBeatThread = new Thread(new ThreadStart(HeartBeatThread));
            m_tHeartBeatThread.Start();

            return 0;
        }

        public int Logout()
        {
            if (m_bExit)
            {
                return 0;
            }

            mIsConnected = false;

            m_bExit = true;

            if (m_tHeartBeatThread != null)
            {
                Console.WriteLine("m_tHeartBeatThread warning!");
                m_tHeartBeatThread.Join();
                m_tHeartBeatThread = null;
                Console.WriteLine("m_tHeartBeatThread exit!");
            }

            if (m_tCommandThread != null)
            {
                Console.WriteLine("m_tCommandThread warning!");
                m_tCommandThread.Join();
                m_tCommandThread = null;
                Console.WriteLine("m_tCommandThread exit!");
            }

            if (this.stream != null)
            {
                this.stream.Close();
                this.stream = null;
            }

            if (this.reader != null)
            {
                this.reader.Close();
                this.reader = null;
            }

            if (this.writer != null)
            {
                this.writer.Close();
                this.writer = null;
            }

            if (tcpClient != null)
            {
                Console.WriteLine("tcpClient warning!");

                tcpClient.Close();
                tcpClient = null;
                Console.WriteLine("tcpClient closed");
            }

            return 0;
        }

        public void HeartBeatThread()
        {
            Console.WriteLine("HeartBeatThread start!!!!!");

     //       m_heartbeatTime = Time.GetExactTime();

            do
            {
                if (m_bExit)
                {
                    break;
                }

                if (ExactTime.GetExactTime() - m_heartbeatTime >= 10)
                {
                    JObject jresp = SendCommand(Module.CERTIFICATE, Operation.KEEPALIVE, null, 0);

                    if (jresp == null)
                    {
                    //    mIsConnected = false;
                    }

                    m_heartbeatTime = ExactTime.GetExactTime();
                    continue;
                }

                Thread.Sleep(500);
            } while (true);

            Console.WriteLine("HeartBeatThread exit!!!!!");
        }

        public bool isConnected()
        {
            return mIsConnected;
        }

        public IOInfo GetIOSensorInfo(int nSNO)
        {
            foreach (IOInfo info in mIOInfoList)
            {
                if (info.nSNO == nSNO)
                {
                    return info;
                }
            }

            return null;
        }

        public void CommandThreadFun()
        {
            Console.WriteLine("CommandThreadFun start");
            CommandBuffer commandBuffer = CommandBuffer.init(65536);

            while (true)
            {
                if (m_bExit)
                {
                    break;
                }

                if (tcpClient.Available == 0)
                {
                    Thread.Sleep(500);
                    continue;

                }

                byte[] buffer = new byte[655360];

                int numberOfBytesRead = 0;
                int totalNumberOfBytes = 0;

                while (tcpClient.Available > 0)
                {
                    numberOfBytesRead = reader.Read(buffer, totalNumberOfBytes, 655360 - totalNumberOfBytes);

                    if (numberOfBytesRead > 0)
                    {
                        totalNumberOfBytes += numberOfBytesRead;
                    }
                }

                if (totalNumberOfBytes == 0)
                {
                    continue;
                }

                commandBuffer.WriteInData(buffer, totalNumberOfBytes);

                Protocol_Head[] head = new Protocol_Head[1];
                byte[] buf = new byte[655360];
                int[] nLen = new int[1];

                while (true)
                {
                    if (m_bExit)
                    {
                        break;
                    }

                    bool flag = commandBuffer.ReadOutCommand(head, buf, nLen);

                    if (!flag)
                    {
                        break;
                    }

                    if (head[0].PT != (int)ENUM_PT_TYPE.PT_SIGNAL)
                    {
                        continue;
                    }

                    JObject jresp = JObject.Parse(Encoding.UTF8.GetString(buf, 0, nLen[0]));
              //      Console.WriteLine("jresp = {0}", jresp.ToString());

                    Module module = (Module)Enum.Parse(typeof(Module), jresp["MODULE"].ToString());
                    Operation operation = (Operation)Enum.Parse(typeof(Operation), jresp["OPERATION"].ToString());


                    if (module == Module.CERTIFICATE)
                    {
                        if (operation == Operation.CONNECT)
                        {
                            if (jresp["SESSION"] != null)
                            {
                                m_SessionId = jresp["SESSION"].ToString();
                            }
                        }
                        else if (operation == Operation.KEEPALIVE)
                        {
                            m_heartbeatTime = ExactTime.GetExactTime();
                            //    Console.WriteLine("[heartbeat:{0}]", DateTime.Now.ToString());
                            continue;
                        }
                    }
                    else if (module == Module.CONFIGMODEL)
                    {
                        if (operation == Operation.GET)
                        {
                            if (jresp["PARAMETER"] != null)
                            {
                                dic[operation] = (JObject)jresp["PARAMETER"];
                            }
                        }
                    }
                    else if (module == Module.STORM)
                    {
                        if (operation == Operation.QUERYFILELIST)
                        {
                            if (jresp["RESPONSE"] != null)
                            {
                                Console.WriteLine("jresp = {0}", jresp.ToString());
                                int nCount = Convert.ToInt32(jresp["RESPONSE"]["SENDFILECOUNT"].ToString());
                                int nSendTime = Convert.ToInt32(jresp["RESPONSE"]["SENDTIME"].ToString());

                                Console.WriteLine("nCount = {0} nSendTime = {1}", nCount, nSendTime);

                                if (nSendTime == 1)
                                {
                                    mFileList.Clear();
                                }

                                for (int i = 0; i < nCount; i++)
                                {
                                    RemoteFileInfo info = new RemoteFileInfo();

                                    //       Console.WriteLine("----------------{0}-------------", i);

                                    if (jresp["RESPONSE"]["RECORDSIZE"] != null && jresp["RESPONSE"]["RECORDSIZE"].HasValues)
                                    {
                                        info.dwFileSize = Convert.ToInt64(jresp["RESPONSE"]["RECORDSIZE"][i].ToString());
                                        //          Console.WriteLine("info.dwFileSize = {0}", info.dwFileSize);
                                    }

                                    if (jresp["RESPONSE"]["RECORDCHANNEL"] != null && jresp["RESPONSE"]["RECORDCHANNEL"].HasValues)
                                    {
                                        info.nChannel = Convert.ToInt32(jresp["RESPONSE"]["RECORDCHANNEL"][i].ToString());
                                        //            Console.WriteLine("info.nChannel = {0}", info.nChannel);
                                    }

                                    if (jresp["RESPONSE"]["FILETYPE"] != null && jresp["RESPONSE"]["FILETYPE"].HasValues)
                                    {
                                        info.nFileType = Convert.ToUInt32(jresp["RESPONSE"]["FILETYPE"][i].ToString());
                                        //             Console.WriteLine("info.nFileType = {0}", info.nFileType);
                                    }

                                    if (jresp["RESPONSE"]["RECORD"] != null && jresp["RESPONSE"]["RECORD"].HasValues)
                                    {
                                        info.szTime = jresp["RESPONSE"]["RECORD"][i].ToString();
                                        //               Console.WriteLine("info.szTime = {0}", info.szTime);
                                    }

                                    if (jresp["RESPONSE"]["RECORDID"] != null && jresp["RESPONSE"]["RECORDID"].HasValues)
                                    {
                                        info.recordID = jresp["RESPONSE"]["RECORDID"][i].ToString();
                                        //              Console.WriteLine("info.szFileName = {0}", info.szFileName);
                                    }

                                    if (jresp["RESPONSE"]["AT"] != null && jresp["RESPONSE"]["AT"].HasValues)
                                    {
                                        info.nAT = Convert.ToInt64(jresp["RESPONSE"]["AT"][i].ToString());
                                        //                Console.WriteLine("info.nAT = {0}", info.nAT);
                                    }

                                    if (jresp["RESPONSE"]["LOCK"] != null && jresp["RESPONSE"]["LOCK"].HasValues)
                                    {
                                        info.bLocked = Convert.ToUInt32(jresp["RESPONSE"]["LOCK"][i]);
                                        //              Console.WriteLine("info.bLocked = {0}", info.bLocked);
                                    }

                                    mFileList.Add(info);
                                    //                Console.WriteLine("File count = {0}", mFileList.Count);
                                }

                                if (jresp["RESPONSE"]["LASTRECORD"] != null)
                                {
                                    if (Convert.ToInt32(jresp["RESPONSE"]["LASTRECORD"].ToString()) == 1)
                                    {
                                        JObject jlist = new JObject();
                                        JArray array = new JArray();
                                        for (int i = 0; i < mFileList.Count; i++)
                                        {
                                            JObject jinfo = JObject.FromObject(mFileList[i]);
                                            array.Add(jinfo);
                                        }

                                        jlist.Add("FILELIST", array);

                                        dic[operation] = jlist;
                                    }
                                }

                                continue;
                            }
                        }
                        else if (operation == Operation.GETSTORAGEINFO)
                        {
                            if (jresp["RESPONSE"] != null)
                            {
                                Console.WriteLine("jresp = {0}", jresp.ToString());

                                mStorageList.Clear();

                                int nCount = Convert.ToInt32(jresp["RESPONSE"]["STORAGECOUNT"].ToString());

                                for (int i = 0; i < nCount; i++)
                                {
                                    StorageInfoEx storage = new StorageInfoEx();

                                    if (jresp["RESPONSE"]["STORAGEINDEX"] != null && jresp["RESPONSE"]["STORAGEINDEX"].HasValues)
                                    {
                                        storage.index = Convert.ToInt32(jresp["RESPONSE"]["STORAGEINDEX"][i].ToString());
                                    }

                                    if (jresp["RESPONSE"]["STORAGELASTSIZE"] != null && jresp["RESPONSE"]["STORAGELASTSIZE"].HasValues)
                                    {
                                        storage.lastsize = Convert.ToInt64(jresp["RESPONSE"]["STORAGELASTSIZE"][i].ToString());
                                    }

                                    if (jresp["RESPONSE"]["STORAGEPOSITION"] != null && jresp["RESPONSE"]["STORAGEPOSITION"].HasValues)
                                    {
                                        storage.postion = Convert.ToInt32(jresp["RESPONSE"]["STORAGEPOSITION"][i].ToString());
                                    }

                                    if (jresp["RESPONSE"]["STORAGESTATUS"] != null && jresp["RESPONSE"]["STORAGESTATUS"].HasValues)
                                    {
                                        storage.status = Convert.ToInt32(jresp["RESPONSE"]["STORAGESTATUS"][i].ToString());
                                    }

                                    if (jresp["RESPONSE"]["STORAGETOTALSIZE"] != null && jresp["RESPONSE"]["STORAGETOTALSIZE"][i].HasValues)
                                    {
                                        storage.totalsize = Convert.ToInt64(jresp["RESPONSE"]["STORAGETOTALSIZE"][i].ToString());
                                    }

                                    if (jresp["RESPONSE"]["STORAGETYPE"] != null && jresp["RESPONSE"]["STORAGETYPE"].HasValues)
                                    {
                                        storage.type = Convert.ToInt32(jresp["RESPONSE"]["STORAGETYPE"][i].ToString());
                                    }

                                    if (jresp["RESPONSE"]["STORAGEUNIT"] != null && jresp["RESPONSE"]["STORAGEUNIT"].HasValues)
                                    {
                                        storage.unit = Convert.ToInt32(jresp["RESPONSE"]["STORAGEUNIT"][i].ToString());
                                    }

                                    mStorageList.Add(storage);
                                }

                                JObject jlist = new JObject();
                                JArray array = new JArray();
                                for (int i = 0; i < mStorageList.Count; i++)
                                {
                                    JObject jinfo = JObject.FromObject(mStorageList[i]);
                                    array.Add(jinfo);
                                }

                                jlist.Add("STORAGELIST", array);

                                dic[operation] = jlist;

                                continue;
                            }
                        }
                        else if (operation == Operation.GETCALENDAR)
                        {
                            if(jresp["RESPONSE"]!= null)
                            {
                                if (jresp["RESPONSE"]["ERRORCODE"] != null)
                                {
                                    int nCode = Convert.ToInt32(jresp["RESPONSE"]["ERRORCODE"].ToString());

                                    if (nCode == 0)
                                    {
                                        List<CalendarData> list = new List<CalendarData>();
                                        int nCount = Convert.ToInt32(jresp["RESPONSE"]["COUNT"].ToString());

                                        for (int i = 0; i < nCount; i++)
                                        {
                                            CalendarData data = new CalendarData();

                                            string item = jresp["RESPONSE"]["CALENDER"][i].ToString();

                                            Console.WriteLine("item = {0}", item);
                                            data.nYear = Convert.ToInt32(item.Substring(0, 4));
                                            data.nMonth = Convert.ToInt32(item.Substring(4,2));
                                            data.nDay = Convert.ToInt32(item.Substring(6, 2));
                                            data.nProperty = Convert.ToInt32(item.Substring(8, 8), 16);

                                            list.Add(data);
                                        }

                                        JObject jlist = new JObject();
                                        JArray array = new JArray();
                                        for (int i = 0; i < list.Count; i++)
                                        {
                                            JObject jinfo = JObject.FromObject(list[i]);
                                            array.Add(jinfo);
                                        }

                                        jlist.Add("LIST", array);

                                        dic[operation] = jlist;

                                        continue;
                                    }
                                } 
                            }
                        }
                    }
                    else if (module == Module.EVEM)
                    {
                        if (operation == Operation.SENDRECORDSTATUS)
                        {
                            mRecordStatus = JsonConvert.DeserializeObject<RecordStatus>(jresp["PARAMETER"].ToString());
                            Console.WriteLine("[SENDRECORDSTATUS]jresp = {0}", jresp.ToString());
                        }
                        else if (operation == Operation.SENDALARMINFO)
                        {
                            Console.WriteLine("[SENDALARMINFO]jresp = {0}", jresp.ToString());
                            if (this.onSendAlarmInfo != null)
                            {
                                this.onSendAlarmInfo(jresp["PARAMETER"].ToString());
                            }
                        }
                    }
                    else if (module == Module.DEVEMM)
                    {
                        if (operation == Operation.SPI)
                        {
                            Console.WriteLine("[{0}]jresp = {1}", DateTime.Now.ToString(), jresp.ToString());
                        }
                        else if (operation == Operation.SENDCMSCONNECTSTATUS)
                        {
                            Console.WriteLine("[SENDCMSCONNECTSTATUS]jresp = {0}", jresp.ToString());
                            int nCMSN = 0;
                            mCMSConnectInfoList.Clear();

                            if (jresp["PARAMETER"]["CMSN"] != null)
                            {
                                nCMSN = Convert.ToInt32(jresp["PARAMETER"]["CMSN"].ToString());
                            }

                            for (int i = 0; i < nCMSN; i++)
                            {
                                CMSConnectInfo info = new CMSConnectInfo();

                                if (jresp["PARAMETER"]["CPT"] != null && jresp["PARAMETER"]["CPT"].HasValues)
                                {
                                    if (jresp["PARAMETER"]["CPT"][i] != null && jresp["PARAMETER"]["CPT"][i].ToString().Length > 0)
                                    {
                                        info.nCPT = Convert.ToInt32(jresp["PARAMETER"]["CPT"][i].ToString());
                                    }
                                }

                                if (jresp["PARAMETER"]["CS"] != null && jresp["PARAMETER"]["CS"].HasValues)
                                {
                                    if (jresp["PARAMETER"]["CS"][i] != null && jresp["PARAMETER"]["CS"][i].ToString().Length > 0)
                                    {
                                        info.nCS = Convert.ToInt32(jresp["PARAMETER"]["CS"][i].ToString());
                                    }

                                }

                                if (jresp["PARAMETER"]["ADD"] != null && jresp["PARAMETER"]["ADD"].HasValues)
                                {
                                    if (jresp["PARAMETER"]["ADD"][i] != null && jresp["PARAMETER"]["ADD"][i].ToString().Length > 0)
                                    {
                                        info.address = jresp["PARAMETER"]["ADD"][i].ToString();
                                    }
                                }

                                if (jresp["PARAMETER"]["M"] != null && jresp["PARAMETER"]["M"].HasValues)
                                {
                                    if (jresp["PARAMETER"]["M"][i] != null && jresp["PARAMETER"]["M"][i].ToString().Length > 0)
                                    {
                                        info.nM = Convert.ToInt32(jresp["PARAMETER"]["M"][i].ToString());
                                    }

                                }

                                if (jresp["PARAMETER"]["E"] != null && jresp["PARAMETER"]["E"].HasValues)
                                {
                                    if (jresp["PARAMETER"]["E"][i] != null && jresp["PARAMETER"]["E"][i].ToString().Length > 0)
                                    {
                                        info.nE = Convert.ToInt32(jresp["PARAMETER"]["E"][i].ToString());
                                    }
                                }

                                mCMSConnectInfoList.Add(info);
                            }

                            Console.WriteLine("[SENDCMSCONNECTSTATUS] end", jresp.ToString());
                        }
                        else if (operation == Operation.GETCMSCONNECTSTATUS)
                        {
                            Console.WriteLine("[GETCMSCONNECTSTATUS]jresp = {0}", jresp.ToString());

                            int nCMSN = 0;
                            mCMSConnectInfoList.Clear();

                            if (jresp["RESPONSE"]["CMSN"] != null)
                            {
                                nCMSN = Convert.ToInt32(jresp["RESPONSE"]["CMSN"].ToString());
                            }

                            for (int i = 0; i < nCMSN; i++)
                            {
                                CMSConnectInfo info = new CMSConnectInfo();

                                if (jresp["RESPONSE"]["CPT"] != null && jresp["RESPONSE"]["CPT"].HasValues)
                                {
                                    if (jresp["RESPONSE"]["CPT"][i] != null && jresp["RESPONSE"]["CPT"][i].ToString().Length > 0)
                                    {
                                        Console.WriteLine("cpt = {0}", jresp["RESPONSE"]["CPT"][i].ToString());
                                        info.nCPT = Convert.ToInt32(jresp["RESPONSE"]["CPT"][i].ToString());
                                    }

                                }

                                if (jresp["RESPONSE"]["CS"] != null && jresp["RESPONSE"]["CS"].HasValues)
                                {
                                    if (jresp["RESPONSE"]["CS"][i] != null && jresp["RESPONSE"]["CS"][i].ToString().Length > 0)
                                    {
                                        info.nCS = Convert.ToInt32(jresp["RESPONSE"]["CS"][i].ToString());
                                    }

                                }

                                if (jresp["RESPONSE"]["ADD"] != null && jresp["RESPONSE"]["ADD"].HasValues)
                                {
                                    if (jresp["RESPONSE"]["ADD"][i] != null && jresp["RESPONSE"]["ADD"][i].ToString().Length > 0)
                                    {
                                        info.address = jresp["RESPONSE"]["ADD"][i].ToString();
                                    }
                                }

                                if (jresp["RESPONSE"]["M"] != null && jresp["RESPONSE"]["M"].HasValues)
                                {
                                    if (jresp["RESPONSE"]["M"][i] != null && jresp["RESPONSE"]["M"][i].ToString().Length > 0)
                                    {
                                        info.nM = Convert.ToInt32(jresp["RESPONSE"]["M"][i].ToString());
                                    }

                                }

                                if (jresp["RESPONSE"]["E"] != null && jresp["RESPONSE"]["E"].HasValues)
                                {
                                    if (jresp["RESPONSE"]["E"][i] != null && jresp["RESPONSE"]["E"][i].ToString().Length > 0)
                                    {
                                        info.nE = Convert.ToInt32(jresp["RESPONSE"]["E"][i].ToString());
                                    }
                                }

                                mCMSConnectInfoList.Add(info);
                            }

                            JObject jlist = new JObject();
                            JArray array = new JArray();
                            for (int i = 0; i < mCMSConnectInfoList.Count; i++)
                            {
                                JObject jinfo = JObject.FromObject(mCMSConnectInfoList[i]);
                                array.Add(jinfo);
                            }

                            jlist.Add("LIST", array);

                            dic[operation] = jlist;

                            continue;
                        }
                        else if (operation == Operation.UPDATEIOSTATUS)
                        {
                            if (jresp["PARAMETER"]["IO"] != null && jresp["PARAMETER"]["IO"].HasValues)
                            {
                                mIOInfoList.Clear();
                                Console.WriteLine("io = {0}", jresp["PARAMETER"]["IO"].ToString());
                                mIOInfoList = JsonConvert.DeserializeObject<List<IOInfo>>(jresp["PARAMETER"]["IO"].ToString());
                            }
                        }
                    }

                    if (jresp["RESPONSE"] != null)
                    {
                        dic[operation] = (JObject)jresp["RESPONSE"];
                    }
                }
            }

            this.reader.Close();
            this.reader = null;

            Console.WriteLine("CommandThreadFun exit!!!!!");
        }

        private static object sign = new object();

        public JObject SendCommand(Module module, Operation operation, JObject jparam, int timeout = 3000)
        {
            lock(sign)
            {
                dic[operation] = null;
                JObject request = new JObject();
                request["MODULE"] = module.ToString();
                request["OPERATION"] = operation.ToString();
                request["SESSION"] = m_SessionId;

                if (jparam != null)
                {
                    request["PARAMETER"] = jparam;
                }

                Console.WriteLine("request = {0}", request.ToString());

                byte[] data = N9MMSG.Serialize(request);

                if (tcpClient.Connected)
                {
                    writer.Write(data, 0, data.Length);
                }
                int i = 0;
                while (dic[operation] == null && i++ < timeout / 10)
                {
                    Thread.Sleep(100);
                }

                if (operation == Operation.KEEPALIVE)
                {

                }
                else if (dic[operation] == null)
                {
                    Console.WriteLine("dic[{0}] == null", operation.ToString());
                    return null;
                }
                else
                {
                    Console.WriteLine("response = {0}", dic[operation].ToString());
                }

                return dic[operation];
            }
        }

        public int GetChannelCount()
        {
            return mDeviceInfo.Channel;
        }

        public DeviceInfo GetDeviceInfo()
        {
            return mDeviceInfo;
        }

        public DeviceType GetDeviceType()
        {
            return (DeviceType)Enum.Parse(typeof(DeviceType), mDeviceInfo.type);
        }

        public bool isChannelRecording(StreamType type, int nChannel)
        {
            if (mRecordStatus == null)
            {
                return false;
            }

            if (type == StreamType.MAIN_STREAM)
            {
                if (((mRecordStatus.nREV >> nChannel) & 0x01) == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (type == StreamType.SUB_STREAM)
            {
                if (((mRecordStatus.nRES >> nChannel) & 0x01) == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (type == StreamType.MIRROR_STREAM)
            {
                if (((mRecordStatus.nREMI >> nChannel) & 0x01) == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        public bool isAlarmRecording(int nChannel)
        {
            if (((mRecordStatus.nMT >> nChannel) & 0x01) == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool isCMSConnected()
        {
            if (mCMSConnectInfoList == null || mCMSConnectInfoList.Count == 0)
            {
                return false;
            }

            foreach (CMSConnectInfo info in mCMSConnectInfoList)
            {
                if (info.nE != 1)
                {
                    continue;
                }

                if (info.nCS != 1 && info.nCS != 2)
                {
                    return false;
                }
            }

            return true;
        }

        public int SetSensorUpload(bool upload)
        {
            JObject jparameter = new JObject();

            if (upload)
            {
                jparameter["SOR"] = 1;
            }
            else
            {
                jparameter["SOR"] = 0;
            }

            JObject jresp = SendCommand(Module.EVEM, Operation.SETUPLOADINFOMASK, jparameter);

            if(jresp == null)
            {
                return -1;
            }

            return Convert.ToInt32(jresp["ERRORCODE"].ToString());
            
        }

        public int RequestVideo(int nChannel, StreamType streamtype)
        {
            Console.WriteLine("[RequestVideo]nChannel = {0}, streamtype = {1}", nChannel, streamtype);

            AtomSocket atomSocket = RegisterMediaSocket();

            if (atomSocket == null)
            {
                Console.WriteLine("RegisterMediaSocket failed!!!");
                return -1;
            }

            JObject jparameter = new JObject();
            jparameter["STREAMNAME"] = atomSocket.GetSpecKeywords();
            jparameter["STREAMTYPE"] = (int)streamtype;
            jparameter["CHANNEL"] = 1 << nChannel;
            jparameter["AUDIOVALID"] = 1 << nChannel;

            JObject jresp = SendCommand(Module.MEDIASTREAMMODEL, Operation.REQUESTALIVEVIDEO, jparameter);

            if (Convert.ToInt32(jresp["ERRORCODE"].ToString()) != 0)
            {
                return -1;
            }

            m_tRequestVideoThread = new Thread(new ParameterizedThreadStart(RequestVideoThreadFun));
            m_tRequestVideoThread.Start(atomSocket);
            m_tRequestVideoThread.Join();

            Console.WriteLine("m_tRequestVideoThread.Join end");

            return 0;
        }

        public void RequestVideoThreadFun(Object socket)
        {
            Console.WriteLine("RequestVideoThreadFun start!!!");
            byte[] buffer = new byte[65536];

            AtomSocket atomSocket = (AtomSocket)socket;

            NetworkStream ns = atomSocket.GetStream();

            Console.WriteLine("ns = {0}", ns);

            BinaryReader br = atomSocket.GetReader();

            Console.WriteLine("br = {0}", br);

            string filepath = Path.Combine(Environment.CurrentDirectory, "gps.data");

            FileStream fs = new FileStream(filepath, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);

            CommandBuffer commandBuffer = CommandBuffer.init(65536);

            int nTotalLen = 0;

            while (true)
            {
                if (m_bExitRequestVideo)
                {
                    Console.WriteLine("[RequestVideoThreadFun]m_bExitRequestVideo exit");
                    break;
                }

                int numberOfBytesRead = 0;
                int totalNumberOfBytes = 0;

                //         Console.WriteLine("atomSocket.Available = {0}", atomSocket.Available);

                while (atomSocket.Available > 0)
                {
                    numberOfBytesRead = br.Read(buffer, totalNumberOfBytes, 65536 - totalNumberOfBytes);

                    if (numberOfBytesRead > 0)
                    {
                        totalNumberOfBytes += numberOfBytesRead;
                    }

                    if (totalNumberOfBytes == 65536)
                    {
                        break;
                    }
                }

                commandBuffer.WriteInData(buffer, totalNumberOfBytes);

                Protocol_Head[] head = new Protocol_Head[1];
                byte[] buf = new byte[65536];
                int[] nLen = new int[1];

                while (true)
                {
                    if (m_bExit)
                    {
                        break;
                    }

                    bool flag = commandBuffer.ReadOutCommand(head, buf, nLen);

                    if (!flag)
                    {
                        break;
                    }

                    if (head[0].PT == (int)ENUM_PT_TYPE.PT_SIGNAL)
                    {
                        JObject jresp = JObject.Parse(Encoding.UTF8.GetString(buf, 0, nLen[0]));

                        Console.WriteLine("jresp = {0}", jresp.ToString());

                        if (jresp["MODULE"].ToString() == Module.MEDIASTREAMMODEL.ToString()
                            && jresp["OPERATION"].ToString() == Operation.DOWNDATASTART.ToString())
                        {
                            nTotalLen = Convert.ToInt32(jresp["RESPONSE"]["TOTAL"].ToString());

                            Console.WriteLine("nTotalLen = {0}", nTotalLen);
                        }
                        else if (jresp["MODULE"].ToString() == Module.MEDIASTREAMMODEL.ToString()
                          && jresp["OPERATION"].ToString() == Operation.DOWNDATASTOP.ToString())
                        {
                            int errorcode = Convert.ToInt32(jresp["RESPONSE"]["ERRORCODE"].ToString());

                            if (errorcode == 0 && Convert.ToInt32(jresp["RESPONSE"]["LAST"].ToString()) == 1)
                            {
                                if (nTotalLen == fs.Length)
                                {
                                    Console.WriteLine("文件完整下载 nTotalLen = {0}", nTotalLen);
                                }
                                else
                                {
                                    Console.WriteLine("文件下载不完整 nTotalLen = {0}, fs.length = {1}", nTotalLen, fs.Length);
                                }

                                break;
                            }
                        }
                    }
                    else if (head[0].PT == (int)ENUM_PT_TYPE.PT_VIDEO_FILE)
                    {
                        bw.Write(buf, 0, nLen[0]);
                        Console.WriteLine("data.len = {0} fs.Length = {1}", nLen[0], fs.Length);
                    }
                    else
                    {
                        Console.WriteLine("head[0].PT = {0}", head[0].PT);
                    }
                }
            }

            fs.Close();
            bw.Close();

            atomSocket.Close();
            ns.Close();
            br.Close();
            bw.Close();
            fs.Close();

            Console.WriteLine("DownloadVideoThreadFun exit!!!");
        }

        public int DownloadVideo(StreamType streamtype, string recordID, int channel, string starttime, string endtime, string filename)
        {
            Console.WriteLine("[DownloadVideo]datatype = {0}, starttime = {1}, endtime = {2}", streamtype, starttime, endtime);

            AtomSocket atomSocket = RegisterMediaSocket();

            if (atomSocket == null)
            {
                Console.WriteLine("RegisterMediaSocket failed!!!");
                return -1;
            }

            JObject jparameter = new JObject();
            jparameter["PT"] = (int)Payload.PT_VIDEO_FILE;
            jparameter["SSRC"] = 1 << channel;
            jparameter["STREAMNAME"] = atomSocket.GetSpecKeywords();
            jparameter["STREAMTYPE"] = (int)streamtype;
            jparameter["RECORDID"] = recordID;
            jparameter["CHANNEL"] = 1 << channel;
            jparameter["STARTTIME"] = starttime;
            jparameter["ENDTIME"] = endtime;
            jparameter["OFFSETFLAG"] = 0;
            jparameter["OFFSET"] = 0;
            jparameter["DT"] = 0;

            JObject jresp = SendCommand(Module.MEDIASTREAMMODEL, Operation.REQUESTDOWNLOADVIDEO, jparameter);

            if (Convert.ToInt32(jresp["ERRORCODE"].ToString()) != 0)
            {
                return -1;
            }

            long nFileSize = Convert.ToInt64(jresp["FILESIZE"].ToString());

            Console.WriteLine("m_nVideoFileSize = {0}", m_nVideoFileSize);


            m_tDownloadVideoThread = new DownloadVideoThread(nFileSize, atomSocket);
            m_tDownloadVideoThread.Start();
            m_tDownloadVideoThread.Join();

            Console.WriteLine("m_tDownloadVideoThread.Join end");

            return 0;
        }

        public class DownloadVideoThread
        {
            private long nFileSize = 0;
            private AtomSocket socket = null;
            private Thread _thread;

            private bool m_bExitDownloadVideo = false;

            public DownloadVideoThread(long _nFileSize, AtomSocket _socket)
            {
                nFileSize = _nFileSize;
                socket = _socket;
            }

            public void Start()
            {
                _thread = new Thread(new ParameterizedThreadStart(DownloadVideoThreadFun));
                _thread.Start(socket);
                _thread.Join();
            }

            public void Interrupt()
            {

            }

            public void Join()
            {
                _thread.Join();
            }

            public void DownloadVideoThreadFun(Object socket)
            {
                Console.WriteLine("DownloadDataThreadFun start!!!");
                byte[] buffer = new byte[65536];

                AtomSocket atomSocket = (AtomSocket)socket;

                NetworkStream ns = atomSocket.GetStream();

                Console.WriteLine("ns = {0}", ns);

                BinaryReader br = atomSocket.GetReader();

                Console.WriteLine("br = {0}", br);

                string filepath = Path.Combine(Environment.CurrentDirectory, "video.data");

                FileStream fs = new FileStream(filepath, FileMode.Create);
                BinaryWriter bw = new BinaryWriter(fs);

                CommandBuffer commandBuffer = CommandBuffer.init(65536);

                int nTotalLen = 0;

                while (true)
                {
                    if (m_bExitDownloadVideo)
                    {
                        Console.WriteLine("[DownloadVideoThreadFun]m_bExitDownloadVideo exit");
                        break;
                    }

                    int numberOfBytesRead = 0;
                    int totalNumberOfBytes = 0;

                    while (atomSocket.Available > 0)
                    {
                        numberOfBytesRead = br.Read(buffer, totalNumberOfBytes, 65536 - totalNumberOfBytes);

                        if (numberOfBytesRead > 0)
                        {
                            totalNumberOfBytes += numberOfBytesRead;
                        }

                        if (totalNumberOfBytes == 65536)
                        {
                            break;
                        }
                    }

                    commandBuffer.WriteInData(buffer, totalNumberOfBytes);

                    Protocol_Head[] head = new Protocol_Head[1];
                    byte[] buf = new byte[65536];
                    int[] nLen = new int[1];

                    while (true)
                    {
                        if (m_bExitDownloadVideo)
                        {
                            break;
                        }

                        bool flag = commandBuffer.ReadOutCommand(head, buf, nLen);

                        if (!flag)
                        {
                            break;
                        }

                        if (head[0].PT == (int)ENUM_PT_TYPE.PT_SIGNAL)
                        {
                            JObject jresp = JObject.Parse(Encoding.UTF8.GetString(buf, 0, nLen[0]));

                            Console.WriteLine("jresp = {0}", jresp.ToString());
                        }
                        else if (head[0].PT == (int)ENUM_PT_TYPE.PT_VIDEO_FILE)
                        {
                            bw.Write(buf, 0, nLen[0]);
                            Console.WriteLine("data.len = {0} fs.Length = {1}", nLen[0], fs.Length);
                        }
                        else
                        {
                            Console.WriteLine("head[0].PT = {0}", head[0].PT);
                        }
                    }

                    if (fs.Length == nFileSize)
                    {
                        break;
                    }
                }

                fs.Close();
                bw.Close();

                atomSocket.Close();
                ns.Close();
                br.Close();
                bw.Close();
                fs.Close();

                Console.WriteLine("DownloadVideoThreadFun exit!!!");
            }
        }

        public int DownloadData(DataType datatype, string starttime, string endtime, string filename)
        {
            Console.WriteLine("[DownloadData]datatype = {0}, starttime = {1}, endtime = {2}", datatype, starttime, endtime);
            AtomSocket atomSocket = RegisterMediaSocket();

            if (atomSocket == null)
            {
                Console.WriteLine("RegisterMediaSocket failed!!!");
                return -1;
            }

            JObject jparameter = new JObject();
            jparameter["STREAMNAME"] = atomSocket.GetSpecKeywords();
            jparameter["DATATYPE"] = (int)datatype;
            jparameter["STARTT"] = starttime;
            jparameter["ENDT"] = endtime;

            JObject jresp = SendCommand(Module.MEDIASTREAMMODEL, Operation.DOWNLOADDATA, jparameter);

            if (Convert.ToInt32(jresp["ERRORCODE"].ToString()) != 0)
            {
                return -1;
            }

            m_tDownloadDataThread = new Thread(new ParameterizedThreadStart(DownloadDataThreadFun));
            m_tDownloadDataThread.Start(atomSocket);
            m_tDownloadDataThread.Join();

            Console.WriteLine("m_tDownloadThread.Join end");

            return 0;
        }

        public void DownloadDataThreadFun(Object socket)
        {
            Console.WriteLine("DownloadDataThreadFun start!!!");
            byte[] buffer = new byte[65536];

            AtomSocket atomSocket = (AtomSocket)socket;

            NetworkStream ns = atomSocket.GetStream();

            Console.WriteLine("ns = {0}", ns);

            BinaryReader br = atomSocket.GetReader();

            Console.WriteLine("br = {0}", br);

            string filepath = Path.Combine(Environment.CurrentDirectory, "gps.data");

            FileStream fs = new FileStream(filepath, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);

            CommandBuffer commandBuffer = CommandBuffer.init(65536);

            int nTotalLen = 0;

            while (true)
            {
                if (m_bExitDownloadData)
                {
                    Console.WriteLine("[DownloadDataThreadFun]m_bExitDownload exit");
                    break;
                }

                int numberOfBytesRead = 0;
                int totalNumberOfBytes = 0;

       //         Console.WriteLine("atomSocket.Available = {0}", atomSocket.Available);

                while (atomSocket.Available > 0)
                {
                    numberOfBytesRead = br.Read(buffer, totalNumberOfBytes, 65536 - totalNumberOfBytes);

                    if (numberOfBytesRead > 0)
                    {
                        totalNumberOfBytes += numberOfBytesRead;
                    }

                    if (totalNumberOfBytes == 65536)
                    {
                        break;
                    }
                }

                commandBuffer.WriteInData(buffer, totalNumberOfBytes);

                Protocol_Head[] head = new Protocol_Head[1];
                byte[] buf = new byte[65536];
                int[] nLen = new int[1];

                while (true)
                {
                    if (m_bExit)
                    {
                        break;
                    }

                    bool flag = commandBuffer.ReadOutCommand(head, buf, nLen);

                    if (!flag)
                    {
                        break;
                    }

                    if (head[0].PT == (int)ENUM_PT_TYPE.PT_SIGNAL)
                    {
                        JObject jresp = JObject.Parse(Encoding.UTF8.GetString(buf, 0, nLen[0]));

                        Console.WriteLine("jresp = {0}", jresp.ToString());

                        if (jresp["MODULE"].ToString() == Module.MEDIASTREAMMODEL.ToString()
                            && jresp["OPERATION"].ToString() == Operation.DOWNDATASTART.ToString())
                        {
                            nTotalLen = Convert.ToInt32(jresp["RESPONSE"]["TOTAL"].ToString());

                            Console.WriteLine("nTotalLen = {0}", nTotalLen);
                        }
                        else if (jresp["MODULE"].ToString() == Module.MEDIASTREAMMODEL.ToString()
                          && jresp["OPERATION"].ToString() == Operation.DOWNDATASTOP.ToString())
                        {
                            int errorcode = Convert.ToInt32(jresp["RESPONSE"]["ERRORCODE"].ToString());

                            if (errorcode == 0 && Convert.ToInt32(jresp["RESPONSE"]["LAST"].ToString()) == 1)
                            {
                                if (nTotalLen == fs.Length)
                                {
                                    Console.WriteLine("文件完整下载 nTotalLen = {0}", nTotalLen);
                                }
                                else
                                {
                                    Console.WriteLine("文件下载不完整 nTotalLen = {0}, fs.length = {1}", nTotalLen, fs.Length);
                                }

                                m_bExitDownloadData = true;

                                break;
                            }
                        }
                    }
                    else if (head[0].PT == (int)ENUM_PT_TYPE.PT_BLACKBOX)
                    {
                        bw.Write(buf, 0, nLen[0]);
                        Console.WriteLine("data.len = {0} fs.Length = {1}", nLen[0], fs.Length);
                    }
                    else
                    {
                        Console.WriteLine("head[0].PT = {0}", head[0].PT);
                    }
                }
            }

            fs.Close();
            bw.Close();

            atomSocket.Close();
            ns.Close();
            br.Close();
            bw.Close();
            fs.Close();

            Console.WriteLine("DownloadDataThreadFun exit!!!");
        }

        public AtomSocket RegisterMediaSocket()
        {
            Console.WriteLine("RegisterMediaSocket");
            AtomSocket tcp = new AtomSocket();

            tcp.SendTimeout = 1000;
            tcp.ReceiveTimeout = 100;
            tcp.SendBufferSize = 65536;
            tcp.ReceiveBufferSize = 65536;
            tcp.NoDelay = true;

  
              uint dummy = 0;
              byte[] inOptionValues = new byte[Marshal.SizeOf(dummy) * 3];
              BitConverter.GetBytes((uint)1).CopyTo(inOptionValues, 0);//是否启用Keep-Alive
              BitConverter.GetBytes((uint)3000).CopyTo(inOptionValues, Marshal.SizeOf(dummy));//多长时间开始第一次探测
              BitConverter.GetBytes((uint)3000).CopyTo(inOptionValues, Marshal.SizeOf(dummy) * 2);//探测时间间隔
              tcp.Client.IOControl(IOControlCode.KeepAliveValues, inOptionValues, null);

            tcp.Connect(m_destIP, m_destPort);

            if (!tcp.Connected)
            {
                Console.WriteLine("tcp.Connected failed");
                return null;
            }

            NetworkStream ns = tcp.GetStream();
            BinaryReader br = tcp.GetReader();
            BinaryWriter bw = tcp.GetWriter();

            JObject request = new JObject();
            request["SESSION"] = m_SessionId;
            request["MODULE"] = Module.CERTIFICATE.ToString();
            request["OPERATION"] = Operation.CREATESTREAM.ToString();

            JObject jparam = new JObject();
            jparam["VISION"] = VERSION_STRING;
            jparam["DEVTYPE"] = 1;
            jparam["STREAMNAME"] = tcp.GetSpecKeywords();
            request["PARAMETER"] = jparam;

            Console.WriteLine("request = {0}", request.ToString());

            byte[] data = N9MMSG.Serialize(request);

            bw.Write(data, 0, data.Length);

            CommandBuffer commandBuffer = CommandBuffer.init(65536);

            byte[] buffer = new byte[65536];

            do
            {
                int numberOfBytesRead = 0;
                int totalNumberOfBytes = 0;

                while (tcp.Available > 0)
                {
                    numberOfBytesRead = br.Read(buffer, totalNumberOfBytes, 65536 - totalNumberOfBytes);

           //         Console.WriteLine("numberOfBytesRead = {0}", numberOfBytesRead);

                    if (numberOfBytesRead > 0)
                    {
                        totalNumberOfBytes += numberOfBytesRead;
                    }
                }

                commandBuffer.WriteInData(buffer, totalNumberOfBytes);

                Protocol_Head[] head = new Protocol_Head[1];
                byte[] buf = new byte[65536];
                int[] nLen = new int[1];

                bool flag = commandBuffer.ReadOutCommand(head, buf, nLen);

                if (!flag)
                {
                    continue;
                }

                if (head[0].PT != (int)ENUM_PT_TYPE.PT_SIGNAL)
                {
                    continue;
                }

                Console.WriteLine("resp = {0}", Encoding.UTF8.GetString(buf, 0, nLen[0]));

                JObject jresp = JObject.Parse(Encoding.UTF8.GetString(buf, 0, nLen[0]));

                if (jresp["MODULE"].ToString() == Module.CERTIFICATE.ToString()
                     && jresp["OPERATION"].ToString() == Operation.CREATESTREAM.ToString())
                {
                    int errorcode = Convert.ToInt32(jresp["RESPONSE"]["ERRORCODE"].ToString());

                    Console.WriteLine("[RegisterMediaSocket]errorcode = {0}", errorcode);

                    if (errorcode == 0)
                    {
                        return tcp;
                    }
                    else
                    {
                        return null;
                    }
                }

            } while (true);

            return null;
        }
    }
}
