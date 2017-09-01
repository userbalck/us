using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static N9MTest.SDK.net.FrameHeader;

namespace N9MTest.SDK.net
{
    public class H264FileLoader
    {
        private string mFileName;
        private FileStream stream;
        private BinaryReader reader;

        private static List<H264FileLoader> mlist = new List<H264FileLoader>();

        public  H264FileLoader(string filename)
        {
            mFileName = Path.Combine(Environment.CurrentDirectory, filename);
            this.stream = new FileStream(filename, FileMode.Open);
            this.reader = new BinaryReader(this.stream);

            mlist.Add(this);
        }

        ~H264FileLoader()
        {
            Dispose();
        }

        public void Dispose()
        {
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
        }

        public static void clear()
        {
        //    Console.WriteLine("mlist.count = {0}", mlist.Count);
            foreach (H264FileLoader loader in mlist)
            {
                if (loader != null)
                {
                    loader.Dispose();
                }
            }

            mlist.Clear();
        }

        private List<DateTime> mDateTimeList = new List<DateTime>();
        private List<UInt64> mPtsList = new List<UInt64>();


        public void Parse()
        {
            mDateTimeList.Clear();
            mPtsList.Clear();

            while (true)
            {
                //获取读取前的位置
                long postion = this.reader.BaseStream.Position;

                int nBufSize = 1024 * 1024;

                byte[] data = this.reader.ReadBytes(nBufSize);

                if (data == null || data.Length == 0)
                {
                    break;
                }

                int nSearchIndex = 0;
                int nFramePos = 0;
                FrameType enumFrameType = FrameType.FRAMETYPE_NONE;

                while (true)
                {
                    Boolean flag = SearchFrameHeader(data, nSearchIndex, data.Length - nSearchIndex, out nFramePos, out enumFrameType);

                    if (!flag)
                    {
                        if (this.reader.BaseStream.Position != this.reader.BaseStream.Length)
                        {
                            this.reader.BaseStream.Seek(-3, SeekOrigin.Current);
                        }

                        break;
                    }

                    if (nBufSize - nFramePos < RMSTREAM_HEADER.SIZE)
                    {
                        if (this.reader.BaseStream.Position != this.reader.BaseStream.Length)
                        {
                            this.reader.BaseStream.Seek(postion + nFramePos, SeekOrigin.Begin);
                        }
                        
                        break;
                    }

                    RMSTREAM_HEADER rmstream_header = RMSTREAM_HEADER.ToObject(data, nFramePos);

                    int nTotalLen = (int)(RMSTREAM_HEADER.SIZE + rmstream_header.lFrameLen + rmstream_header.lExtendLen);

                    if (postion + nFramePos > this.reader.BaseStream.Length)
                    {
                        break;
                    }

                    if (nBufSize - nFramePos < nTotalLen)
                    {
                        if (this.reader.BaseStream.Position != this.reader.BaseStream.Length)
                        {
                            this.reader.BaseStream.Seek(postion + nFramePos, SeekOrigin.Begin);
                        }

                        break;
                    }

                    int nExtendStart = nFramePos + RMSTREAM_HEADER.SIZE;

                    //完整的一帧
                    for (int i = 0; i < rmstream_header.lExtendCount; i++)
                    {
                        RMS_INFOTYPE infoType = RMS_INFOTYPE.ToObject(data, nExtendStart);

                        if (infoType.lInfoType == (int)ENUM_RMS_INFOTYPE.RMS_INFOTYPE_VIDEOINFO)
                        {
                            RMFI2_VIDEOINFO info = RMFI2_VIDEOINFO.ToObject(data, nExtendStart);
                        }
                        else if (infoType.lInfoType == (int)ENUM_RMS_INFOTYPE.RMS_INFOTYPE_RTCTIME)
                        {
                            RMFI2_RTCTIME time = RMFI2_RTCTIME.ToObject(data, nExtendStart);

                            DateTime dtTime = new DateTime(time.stuRtcTime.cYear + 2000,
                                time.stuRtcTime.cMonth,
                                time.stuRtcTime.cDay,
                                time.stuRtcTime.cHour,
                                time.stuRtcTime.cMinute,
                                time.stuRtcTime.cSecond);

                            mDateTimeList.Add(dtTime);

                            Console.WriteLine("[{0}]time:{1}", enumFrameType.ToString(), dtTime.ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                        else if (infoType.lInfoType == (int)ENUM_RMS_INFOTYPE.RMS_INFOTYPE_VIRTUALTIME)
                        {
                            RMFI2_VTIME virtualTime = RMFI2_VTIME.ToObject(data, nExtendStart);

                            if (enumFrameType == FrameType.FRAMETYPE_I || enumFrameType == FrameType.FRAMETYPE_P)
                            {
                                UInt64 lastpts = virtualTime.llVirtualTime;

                                if (mPtsList.Count > 0)
                                {
                                    lastpts = mPtsList.Last();
                                }
                                
                                mPtsList.Add(virtualTime.llVirtualTime);
                                Console.WriteLine("[{0}][{1:00}]pts = {2} timespan = {3} size = {4}", enumFrameType.ToString(), mPtsList.Count, virtualTime.llVirtualTime, virtualTime.llVirtualTime - lastpts,
                                    rmstream_header.lFrameLen);
                            }
                            
                        }
                        else if (infoType.lInfoType == (int)ENUM_RMS_INFOTYPE.RMS_INFOTYPE_AUDIOINFO)
                        {
                            RMFI2_AUDIOINFO info = RMFI2_AUDIOINFO.ToObject(data, nExtendStart);
                        }

                        nExtendStart += (int)infoType.lInfoLength;
                    }

                    //此处可以读取完整的一帧 如果不需要读取 直接移动帧尾
                    nSearchIndex = nFramePos + nTotalLen;
                }
            }

            Console.WriteLine("[文件解析完成]绝对时间数量:{0} 时间戳数量{1}", mDateTimeList.Count, mPtsList.Count);
        }

        public Boolean GetTimeDuration(out DateTime dtStartTime, out DateTime dtEndTime)
        {
            if (mDateTimeList == null || mDateTimeList.Count == 0)
            {
                dtStartTime = DateTime.Now;
                dtEndTime = DateTime.Now;
                return false;
            }

            dtStartTime = mDateTimeList[0];
            dtEndTime = mDateTimeList[mDateTimeList.Count - 1];

            return true;
        }

        public bool GetTimeSpan(out ulong lStartPts, out ulong lEndPts)
        {
            if (mPtsList == null || mPtsList.Count == 0)
            {
                lStartPts = lEndPts = 0;
                return false;
            }

            lStartPts = mPtsList[0];
            lEndPts = mPtsList[mPtsList.Count - 1];
            return true;
        }

        public int GetIFrameCount()
        {
            return mDateTimeList.Count;
        }

        public int GetVideoFrameCount()
        {
            return mPtsList.Count();
        }

        public List<UInt64> GetPts()
        {
            return mPtsList;
        }

        public Boolean GetPtsDuration(out UInt64 ullStartPts, out UInt64 ullEndPts)
        {
            if (mPtsList == null || mPtsList.Count == 0)
            {
                ullStartPts = 0;
                ullEndPts = 0;

                return false;
            }

            ullStartPts = mPtsList[0];
            ullEndPts = mPtsList[mPtsList.Count - 1];

            return true;
        }

        public Boolean SearchFrameHeader(byte[] data, int nSearchIndex, int nSize, out int nFramePos, out FrameType enumFrameType)
        {
            enumFrameType = FrameType.FRAMETYPE_NONE;
            nFramePos = 0;

            int nCount = 0;
            while (nCount + 4 <= nSize)
            {
                string tag = System.Text.Encoding.Default.GetString(data, nSearchIndex + nCount + 1, 3);
                if (String.Compare(tag, "2dc") == 0 || String.Compare(tag, "3dc") == 0 || String.Compare(tag, "4dc") == 0)
                {
                    nFramePos = nSearchIndex + nCount;

                    if (String.Compare(tag, "2dc") == 0)
                    {
                        enumFrameType = FrameType.FRAMETYPE_I;
                    }
                    else if (String.Compare(tag, "3dc") == 0)
                    {
                        enumFrameType = FrameType.FRAMETYPE_P;
                    }
                    else if (String.Compare(tag, "4dc") == 0)
                    {
                        enumFrameType = FrameType.FRAMETYPE_A;
                    }

                    return true;
                }
                else
                {
                    nCount++;
                }
            }

            nFramePos = 0;

            return false;
        }
    }
}
