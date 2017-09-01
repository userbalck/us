using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using N9MTest.SDK.net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Util;
using Newtonsoft.Json;

namespace N9MTest.Record
{
	[Ignore("1111")]
    [TestFixture]
    class TestCase_RecordFile:TestCase_Basecase
    {
		[Test]
		public void RecordFileLength()
		{
            int nStreamType = Convert.ToInt32(this.CFG.gNode("Record/RecordFileLength/streamtype").InnerText);
            int nChannelBits = Convert.ToInt32(this.CFG.gNode("Record/RecordFileLength/channelbits").InnerText);
            string starttime = this.CFG.gNode("Record/RecordFileLength/starttime").InnerText;
            string endtime = this.CFG.gNode("Record/RecordFileLength/endtime").InnerText;

            Console.WriteLine("nStreamType ={0}", nStreamType);
            Console.WriteLine("nChannelBits ={0}", nChannelBits);
            Console.WriteLine("starttime ={0}", starttime);
            Console.WriteLine("endtime ={0}", endtime);

			foreach (string ip in IPList)
			{
				Console.WriteLine("ip = {0}", ip);

				N9MSession session = new N9MSession(ip, port);
				Assert.AreEqual(0, session.Login(username, password));

				JObject jparameter = new JObject();
				jparameter.Add("STREAMTYPE", nStreamType);
				jparameter.Add("FILETYPE", 0xffffffff);
				jparameter.Add("CHANNEL", nChannelBits);
				jparameter.Add("STARTTIME", starttime);
				jparameter.Add("ENDTIME", endtime);

				JObject jresp = session.SendCommand(Module.STORM, Operation.QUERYFILELIST, jparameter);

				Assert.IsNotNull(jresp["FILELIST"]);

				List<RemoteFileInfo> list = JsonConvert.DeserializeObject<List<RemoteFileInfo>>(jresp["FILELIST"].ToString());

				TimeSection timeSection = new TimeSection();

				foreach (RemoteFileInfo info in list)
				{
					if (info == null)
					{
						continue;
					}

					timeSection.merage(info.nChannel, info.szTime.Split('-')[0], info.szTime.Split('-')[1]);
				}

				for (int i = 0; i < sizeof(UInt32); i++)
				{
					if (((nChannelBits >> i) & 0x01) == 1)
					{
						Assert.IsTrue(timeSection.IsContinuous(i, starttime, endtime));
					}
				}

				foreach (RemoteFileInfo info in list)
				{
					//首先获取段内文件的大小
					jparameter = new JObject();
                    jparameter["DATATYPE"] = 0;
                    jparameter["STREAMTYPE"] = nStreamType;
                    jparameter["RECORDTYPE"] = 3;
                    jparameter["STIME"] = starttime;
                    jparameter["ETIME"] = endtime;
                    jparameter["CHANNEL"] = 1 << info.nChannel;

                    jresp = session.SendCommand(Module.STORM, Operation.GETFILESIZEBYTIME, jparameter);
                    Assert.IsNotNull(jresp);
                    Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));

                    int nTotalSize = Convert.ToInt32(jresp["TOTALSIZE"].ToString());

                    session.DownloadVideo((StreamType)nStreamType, info.recordID, info.nChannel, starttime, endtime, "video.data");

                    System.IO.FileInfo fileInfo = new System.IO.FileInfo("video.data");

                    double rate = fileInfo.Length * 1.0f / nTotalSize;

                    Assert.IsTrue(rate >= 0.9, "时间段{0}-{1}文件预计大小为{2} 实际下载获取到的文件大小为{3}, 差距不符合期望", info.starttime, info.endtime, nTotalSize, fileInfo.Length);
                }
            }
		}
	}
}
