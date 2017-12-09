using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static N9MTest.SDK.net.BlackBox;

namespace N9MTest.SDK.net
{
    public class BlackBoxLoader:IDisposable
    {
        private string mFileName;
        private FileStream stream;
        private BinaryReader reader;

        private BlackBoxLoader()
        {

        }

        ~BlackBoxLoader()
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

        public BlackBoxLoader(string filename)
        {
            mFileName = filename;
            this.stream = new FileStream(filename, FileMode.Open);
            this.reader = new BinaryReader(this.stream);
        }

        public RMBDM_FRAMEHEADER GetFrame()
        {
            if (this.stream.Position  + RMBDM_FRAMEHEADER.SIZE > this.stream.Length)
            {
                this.stream.Close();
                this.reader.Close();
                return null;
            }

            RMBDM_FRAMEHEADER header = RMBDM_FRAMEHEADER.ToObject(this.reader.ReadBytes(RMBDM_FRAMEHEADER.SIZE));

            if (header == null)
            {
                this.stream.Close();
                this.reader.Close();

                return null;
            }

            if (this.stream.Position + header.lFrameLen > this.stream.Length)
            {
                this.stream.Close();
                this.reader.Close();

                return null;
            }

            Console.WriteLine("header.ullPts = {0}", header.ullPts);

            if (header.list == null)
            {
                header.list = new List<IBaseStruct>();
            }

            byte[] data = this.reader.ReadBytes((int)header.lFrameLen);

            int nSourceIndex = 0;

            for (int i = 0; i < header.lDataCount; i++)
            {
                RMBDM_INFOTYPE type = RMBDM_INFOTYPE.ToObject(data, nSourceIndex);

            //    Console.WriteLine("type.lInfoType = {0}", type.lInfoType);

                if (type.lInfoType == (int)BBox.E_FILE_INFO_TYPE_TIME)
                {
                    RMBDM_DATETIME time = RMBDM_DATETIME.ToObject(data, nSourceIndex);
                    time.time.print();
                    header.list.Add(time);
                }
                else if (type.lInfoType == (int)BBox.E_FILE_INFO_TYPE_GPS)
                {
                    RMBDM_GPS gps = RMBDM_GPS.ToObject(data, nSourceIndex);
                    Console.WriteLine("gps.cGpPlanetNum = {0}", gps.cGpPlanetNum);
                    header.list.Add(gps);
                }
                else if (type.lInfoType == (int)BBox.E_FILE_INFO_TYPE_ALARM)
                {
                    RMBDM_ALARM alarm = RMBDM_ALARM.ToObject(data, nSourceIndex);
                    header.list.Add(alarm);
                }
                else if (type.lInfoType == (int)BBox.E_FILE_INFO_TYPE_VEHICLE)
                {
                    RMBDM_LONGLIST longlist = RMBDM_LONGLIST.ToObject(data, nSourceIndex);
                    header.list.Add(longlist);
                }
                else if (type.lInfoType == (int)BBox.E_FILE_INFO_TYPE_VEHICLE)
                {

                }

                nSourceIndex += (int)type.lInfoLength;
            }

            return header;
        }
    }
}
