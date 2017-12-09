using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;

namespace N9MTest.SDK.net
{
    class FrameHeader
    {
        public enum ENUM_RMS_INFOTYPE
        {
            RMS_INFOTYPE_VIDEOINFO = 1,
            RMS_INFOTYPE_RTCTIME = 2,
            RMS_INFOTYPE_VIRTUALTIME = 3,
            RMS_INFOTYPE_AUDIOINFO = 4,
        }

        public class RMSTREAM_HEADER
        {
            public static readonly int SIZE = 12;
            public UInt32 lFrameType;
            public UInt32 lFrameLen;
            public UInt32 lStreamExam;
            public UInt32 lExtendLen;
            public UInt32 lExtendCount;

            public static RMSTREAM_HEADER ToObject(byte[] data, int nSourceIndex)
            {
                RMSTREAM_HEADER header = new RMSTREAM_HEADER();
                ByteBuffer buffer = ByteBuffer.allocate(SIZE);

                buffer.Put(data, nSourceIndex, SIZE);
                buffer.filp();

                header.lFrameType = buffer.ReadUInt32();

                UInt32 flag = buffer.ReadUInt32();
                header.lFrameLen = flag & 0xFFFFFF;
                header.lStreamExam = (flag >> 24) & 0xFF;

                flag = buffer.ReadUInt32();
                header.lExtendLen = flag & 0xFFFFFF;
                header.lExtendCount = (flag >> 24) & 0xFF;

                return header;
            }
        }

        public class RMS_INFOTYPE
        {
            public static readonly int SIZE = 4;
            public UInt32 lInfoType;
            public UInt32 lInfoLength;


            public static RMS_INFOTYPE ToObject(byte[] data, int nSourceIndex)
            {
                RMS_INFOTYPE infoType = new RMS_INFOTYPE();
                ByteBuffer buffer = ByteBuffer.allocate(SIZE);

                buffer.Put(data, nSourceIndex, SIZE);
                buffer.filp();

                UInt32 flag = buffer.ReadUInt32();

                infoType.lInfoType = flag & 0xFF;
                infoType.lInfoLength = (flag >> 8) & 0xFFFFFF;

                return infoType;
            }
        }

        public struct RMS_DATETIME
        {
            public static readonly int SIZE = 8;

            public byte cYear;
            public byte cMonth;
            public byte cDay;
            public byte cHour;
            public byte cMinute;
            public byte cSecond;
            public UInt16 usMilliSecond;
            public UInt16 usWeek;
            public UInt16 usReserved;
            public UInt16 usMilliValidate;

            public static RMS_DATETIME ToObject(byte[] data, int nSourceIndex)
            {
                RMS_DATETIME time = new RMS_DATETIME();

                ByteBuffer buffer = ByteBuffer.allocate(SIZE);
                buffer.Put(data, nSourceIndex, SIZE);
                buffer.filp();

                time.cYear = buffer.ReadByte();
                time.cMonth = buffer.ReadByte();
                time.cDay = buffer.ReadByte();
                time.cHour = buffer.ReadByte();
                time.cMinute = buffer.ReadByte();
                time.cSecond = buffer.ReadByte();

                UInt16 flag = buffer.ReadUInt16();

                time.usMilliSecond = (UInt16)(flag & 0x03FF);
                time.usWeek = (UInt16)((flag >> 10) & 0x07);
                time.usReserved = (UInt16)((flag >> 13) & 0x03);
                time.usMilliValidate = (UInt16)((flag >> 15) & 0x01);

                return time;
            }
        }

        public class RMFI2_RTCTIME
        {
            public static readonly int SIZE = RMS_INFOTYPE.SIZE + RMS_DATETIME.SIZE;
            public RMS_INFOTYPE stuInfoType;
            public RMS_DATETIME stuRtcTime;

            public static RMFI2_RTCTIME ToObject(byte[] data, int nSourceIndex)
            {
                RMFI2_RTCTIME rtctime = new RMFI2_RTCTIME();

                rtctime.stuInfoType = RMS_INFOTYPE.ToObject(data, nSourceIndex);
                rtctime.stuRtcTime = RMS_DATETIME.ToObject(data, nSourceIndex + RMS_INFOTYPE.SIZE);

                return rtctime;
            }
        }

        public class RMFI2_VTIME
        {
            public static readonly int SIZE = RMS_INFOTYPE.SIZE + 8;
            public RMS_INFOTYPE stuInfoType;
            public UInt64 llVirtualTime;

            public static RMFI2_VTIME ToObject(byte[] data, int nSourceIndex)
            {
                RMFI2_VTIME vtime = new RMFI2_VTIME();
                vtime.stuInfoType = RMS_INFOTYPE.ToObject(data, nSourceIndex);

                ByteBuffer buffer = ByteBuffer.allocate(8);
                buffer.Put(data, nSourceIndex + RMS_INFOTYPE.SIZE, 8);
                buffer.filp();

                vtime.llVirtualTime = buffer.ReadUInt64();

                return vtime;
            }
        }

        public class RMFI2_AUDIOINFO
        {
            public static readonly int SIZE = RMS_INFOTYPE.SIZE + 4;
            public RMS_INFOTYPE stuInfoType;
            public UInt32 lPayloadType;
            public UInt32 lSoundMode;
            public UInt32 lPlayAudio;
            public UInt32 lBitWidth;
            public UInt32 lSampleRate;
            public UInt32 lReserve;

            public static RMFI2_AUDIOINFO ToObject(byte[] data, int nSourceIndex)
            {
                RMFI2_AUDIOINFO info = new RMFI2_AUDIOINFO();

                info.stuInfoType = RMS_INFOTYPE.ToObject(data, nSourceIndex);

                ByteBuffer buffer = ByteBuffer.allocate(4);
                buffer.Put(data, nSourceIndex + RMS_INFOTYPE.SIZE, 4);
                buffer.filp();

                UInt32 flag = buffer.ReadUInt32();

                flag = buffer.ReadUInt32();
                info.lPayloadType = flag & 0x0F;
                info.lSoundMode = (flag >> 4) & 0x01;
                info.lPlayAudio = (flag >> 5) & 0x01;
                info.lBitWidth = (flag >> 6) & 0x3F;
                info.lSampleRate = (flag >> 12) & 0x3FFFFF;
                info.lReserve = (flag >> 30) & 0x03;

                return info;
            }
        }

        public class RMFI2_VIDEOINFO
        {
            public static readonly int SIZE = RMS_INFOTYPE.SIZE + 4;
            public RMS_INFOTYPE stuInfoType;
            public UInt32 lWidth;
            public UInt32 lHeight;
            public UInt32 lFPS;

            public static RMFI2_VIDEOINFO ToObject(byte[] data, int nSourceIndex)
            {
                RMFI2_VIDEOINFO info = new RMFI2_VIDEOINFO();

                info.stuInfoType = RMS_INFOTYPE.ToObject(data, nSourceIndex);

                ByteBuffer buffer = ByteBuffer.allocate(4);
                buffer.Put(data, nSourceIndex + RMS_INFOTYPE.SIZE, 4);
                buffer.filp();

                UInt32 flag = buffer.ReadUInt32();
                info.lWidth = flag & 0x0FFF;
                info.lHeight = (flag >> 12) & 0x0FFF;
                info.lFPS = (flag >> 24) & 0x0F;

                return info;
            }

        }
    }
}
