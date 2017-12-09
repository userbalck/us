using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    class Section
    {
        public DateTime dtStartTime;
        public DateTime dtEndTime;

        public string starttime;
        public string endtime;

        public Section()
        {

        }

        public Section(DateTime start, DateTime end)
        {
            dtStartTime = start;
            dtEndTime = end;

            starttime = dtStartTime.ToString("yyyyMMddHHmmss");
            endtime = dtEndTime.ToString("yyyyMMddHHmmss");
        }

        public static Comparison<Section> comparison = new Comparison<Section>
             ((Section x, Section y) =>
             {
                 if (x.dtStartTime < y.dtStartTime)
                 {
                     return -1;
                 }
                 else if (x.dtStartTime == y.dtStartTime)
                 {
                     return 0;
                 }
                 return 1;
             });

        public void Print()
        {
            Console.WriteLine("时间段  开始时间：{0}   结束时间：{1}", dtStartTime.ToString("yyyy-MM-dd HH:mm:ss"), dtEndTime.ToString("yyyy-MM-dd HH:mm:ss"));
        }
        
    }

    public class TimeSection
    {
        private Dictionary<int, List<Section>> dict = new Dictionary<int, List<Section>>();

        public void merage(int channel, string starttime, string endtime)
        {
            DateTime dtStartTime = DateTime.ParseExact(starttime, "yyyyMMddHHmmss", null);
            DateTime dtEndTime = DateTime.ParseExact(endtime, "yyyyMMddHHmmss", null);

            Console.WriteLine("[{0}]dtStartTime = {1}, dtEndTime = {2}", channel,dtStartTime.ToString(), dtEndTime.ToString());

            List<Section> list = null;

            if (!dict.ContainsKey(channel))
            {
                dict[channel] = new List<Section>();
            }

            list = dict[channel];

            List<Section> tmplist = new List<Section>();

            byte[] data = new byte[24 * 3600];

            Array.Clear(data, 0, 24 * 3600);

            //首先把所有的时间都填充进去
            foreach (Section section in list)
            {
                int nStartSeconds = section.dtStartTime.Hour * 3600 + section.dtStartTime.Minute * 60 + section.dtStartTime.Second;
                int nEndSeconds = section.dtEndTime.Hour * 3600 + section.dtEndTime.Minute * 60 + section.dtEndTime.Second;

                for (int second = nStartSeconds; second <= nEndSeconds; second++)
                {
                    data[second] = 1;
                }
            }

            int nStartSecond = dtStartTime.Hour * 3600 + dtStartTime.Minute * 60 + dtStartTime.Second;
            int nEndSecond = dtEndTime.Hour * 3600 + dtEndTime.Minute * 60 + dtEndTime.Second;

            for (int second = nStartSecond; second <= nEndSecond; second++)
            {
                data[second] = 1;
            }

            bool bFind = false;
            DateTime dtTmpStartTime = new DateTime(dtStartTime.Year, dtStartTime.Month, dtStartTime.Day);
            DateTime dtTmpEndTime = new DateTime(dtEndTime.Year, dtEndTime.Month, dtEndTime.Day);

            for (int i = 0; i < data.Length; i++)
            {
                if (i == 0 && data[i] == 1)
                {
                    dtTmpStartTime = new DateTime(dtStartTime.Year, dtStartTime.Month, dtStartTime.Day, 0, 0, 0);
                    bFind = true;
                    continue;
                }

                if (i == data.Length - 1)
                {
                    if (bFind)
                    {
                        dtTmpEndTime = new DateTime(dtStartTime.Year, dtStartTime.Month, dtStartTime.Day, 23, 59, 59);
                        tmplist.Add(new Section(dtTmpStartTime, dtTmpEndTime));
                        bFind = false;
                        continue;
                    }
                }

                if (bFind)
                {
                    if (data[i] == 0)
                    {
                        int hour = (i - 1) / 3600;
                        int minute = ((i - 1) - hour * 3600) / 60;
                        int second = (i - 1) % 60;

                        dtTmpEndTime = new DateTime(dtStartTime.Year, dtStartTime.Month, dtStartTime.Day, hour, minute, second);
                        tmplist.Add(new Section(dtTmpStartTime, dtTmpEndTime));
                        bFind = false;
                        continue;
                    }
                    
                }
                else
                {
                    if (data[i] == 1)
                    {
                        int hour = i / 3600;
                        int minute = (i - hour * 3600) / 60;
                        int second = i % 60;

                        dtTmpStartTime = new DateTime(dtStartTime.Year, dtStartTime.Month, dtStartTime.Day, hour, minute, second);
                        bFind = true;
                        continue;
                    }
                }
            }

            dict[channel] = tmplist;
        }

        public bool isOverFlow(int channel, string starttime, string endtime)
        {
            DateTime dtStartTime = DateTime.ParseExact(starttime, "yyyyMMddHHmmss", null);
            DateTime dtEndTime = DateTime.ParseExact(endtime, "yyyyMMddHHmmss", null);

            List<Section> list = dict[channel];

            list.Sort(Section.comparison);

            return false;
        }

        public bool IsContinuous(int channel, string starttime, string endtime)
        {
            Console.WriteLine("\r\n");

            Console.WriteLine("[IsContinuous] 理论上需要判定的条件 channel = {0} starttime ={1} endtime = {2}", channel, starttime, endtime);

            DateTime dtStartTime = DateTime.ParseExact(starttime, "yyyyMMddHHmmss", null);
            DateTime dtEndTime = DateTime.ParseExact(endtime, "yyyyMMddHHmmss", null);

            if (!dict.ContainsKey(channel))
            {
                Console.WriteLine("dict.ContainsKey error channel = {0}", channel);
                return false;
            }

            List<Section> list = dict[channel];

            if (list.Count == 1)
            {
                Section section = list[0];
                if (section.dtStartTime <= dtStartTime && section.dtEndTime >= dtEndTime)
                {
                    Console.WriteLine("[IsContinuous] 实际上的通道及开始结束时间 channel = {0} starttime ={1} endtime = {2} 通道连续", channel, section.starttime, section.endtime);
                    return true;
                }
                else
                {
                    Console.WriteLine("[IsContinuous] 实际上的通道及开始结束时间 channel = {0} starttime ={1} endtime = {2} 通道录像段边界判定不通过", channel, section.starttime, section.endtime);
                }
            }
            else
            {
                Console.WriteLine("[IsContinuous]通道{0}分为{1}段", channel, list.Count);

                foreach (Section section in list)
                {
                    section.Print();
                }

                Console.WriteLine("\r\n");
            }

            return false;
        }

        public bool GetTimeSpan(int channel,out DateTime stStartTime, out DateTime stEndTime)
        {
            stStartTime = DateTime.Now;
            stEndTime = DateTime.Now;

            if (!dict.ContainsKey(channel))
            {
                return false;
            }

            List<Section> list = dict[channel];

            if (list.Count != 1)
            {
                return false;
            }

            Section section = list[0];
            stStartTime = section.dtStartTime;
            stEndTime = section.dtEndTime;

            return true;
        }

        public void print()
        {
            foreach (var item in dict)
            {
                int channel = item.Key;
                List<Section> list = dict[channel];

                Console.WriteLine("-------------------channel:{0}----------------", channel);

                foreach (Section section in list)
                {
                    Console.WriteLine("starttime:{0} endtime:{1}", section.dtStartTime.ToString(),section.dtEndTime.ToString());
                }

            }
        }
    }
}
