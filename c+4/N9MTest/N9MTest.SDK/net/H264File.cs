using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;

namespace N9MTest.SDK.net
{
    public class H264File
    {
        public struct RMVIDEO_STHDR
        {
            public static readonly int SIZE = 8;

            byte[] struct_attribute;
            UInt32 struct_size;

            public static RMVIDEO_STHDR ToObject(byte[] data)
            {
                ByteBuffer buffer = ByteBuffer.allocate(SIZE);
                buffer.Put(data, SIZE);
                buffer.filp();

                RMVIDEO_STHDR rmvideo_sthdr = new RMVIDEO_STHDR();
                rmvideo_sthdr.struct_attribute = buffer.ReadBytes(4);
                rmvideo_sthdr.struct_size = buffer.ReadUInt32();

                return rmvideo_sthdr;
            }
        }

        public struct RMVIDEO_AUDIO
        {
            public static readonly int SIZE = 12;
            public byte chlCount;
            public byte sampleBits;
            public byte[] reserved1;
            public UInt32 bitRate;
            public UInt32 sampleRate;

            public static RMVIDEO_AUDIO ToObject(byte[] data)
            {
                ByteBuffer buffer = ByteBuffer.allocate(SIZE);
                buffer.Put(data, SIZE);
                buffer.filp();

                RMVIDEO_AUDIO rmvideo_audio = new RMVIDEO_AUDIO();
                rmvideo_audio.chlCount = buffer.ReadByte();
                rmvideo_audio.sampleBits = buffer.ReadByte();
                rmvideo_audio.reserved1 = buffer.ReadBytes(2);
                rmvideo_audio.bitRate = buffer.ReadUInt32();
                rmvideo_audio.sampleRate = buffer.ReadUInt32();

                return rmvideo_audio;
            }
        }

        public struct RMVIDEO_FILEHEADER_1
        {
            public static readonly int SIZE = RMVIDEO_STHDR.SIZE + RMVIDEO_AUDIO.SIZE + 72;

            public RMVIDEO_STHDR hdr;
            public byte[] verDev;
            public byte verFile; // 1
            public byte begYear;
            public byte begMonth;
            public byte begDay;
            public byte begHour;
            public byte begMinute;
            public byte begSecond;
            public byte endYear;
            public byte endMonth;
            public byte endDay;
            public byte endHour;
            public byte endMinute;
            public byte endSecond;
            public byte chlCount;
            public byte recordType;
            public byte fileProtected;
            public byte recordMode;
            public byte videoType;
            public byte[] reserved1;
            public RMVIDEO_AUDIO audio;
            public byte[] reserved2;
            public UInt16 usdevType;
            public byte[] reserved3;
            public byte[] szdevType;

            public static RMVIDEO_FILEHEADER_1 ToObject(byte[] data)
            {
                ByteBuffer buffer = ByteBuffer.allocate(SIZE);
                buffer.Put(data, SIZE);
                buffer.filp();

                RMVIDEO_FILEHEADER_1 header = new RMVIDEO_FILEHEADER_1();
                header.hdr = RMVIDEO_STHDR.ToObject(buffer.ReadBytes(RMVIDEO_STHDR.SIZE));
                header.verDev = buffer.ReadBytes(28);
                header.verFile = buffer.ReadByte();
                header.begYear = buffer.ReadByte();
                header.begMonth = buffer.ReadByte();
                header.begDay = buffer.ReadByte();
                header.begHour = buffer.ReadByte();
                header.begMinute = buffer.ReadByte();
                header.begSecond = buffer.ReadByte();
                header.endYear = buffer.ReadByte();
                header.endMonth = buffer.ReadByte();
                header.endDay = buffer.ReadByte();
                header.endHour = buffer.ReadByte();
                header.endMinute = buffer.ReadByte();
                header.endSecond = buffer.ReadByte();
                header.chlCount = buffer.ReadByte();
                header.recordType = buffer.ReadByte();
                header.fileProtected = buffer.ReadByte();
                header.recordMode = buffer.ReadByte();
                header.videoType = buffer.ReadByte();
                header.reserved1 = buffer.ReadBytes(2);
                header.audio = RMVIDEO_AUDIO.ToObject(buffer.ReadBytes(RMVIDEO_AUDIO.SIZE));
                header.reserved2 = buffer.ReadBytes(8);
                header.usdevType = buffer.ReadUInt16();
                header.reserved3 = buffer.ReadBytes(2);
                header.szdevType = buffer.ReadBytes(12);

                return header;

            }
        }

        public struct RMVIDEO_FILEHEADER_2
        {
            public static readonly int SIZE = RMVIDEO_STHDR.SIZE + RMVIDEO_AUDIO.SIZE + 72;

            RMVIDEO_STHDR hdr;
            public byte[] verDev;
            public byte verFile; // 2 或者3
            public byte begYear;
            public byte begMonth;
            public byte begDay;
            public byte begHour;
            public byte begMinute;
            public byte begSecond;
            public byte endYear;
            public byte endMonth;
            public byte endDay;
            public byte endHour;
            public byte endMinute;
            public byte endSecond;
            public byte chlCount;
            public byte recordType;
            public byte fileProtected;
            public byte recordMode;
            public byte videoType;
            public UInt16 devNo; // 占用了原来的reserved
            public RMVIDEO_AUDIO audio;
            public byte[] reserved2;
            public UInt16 usdevType;
            public byte reserved3;
            public byte dateShowFmt; // 从reserved3 中分出来一个
            public byte[] szdevType;

            public static RMVIDEO_FILEHEADER_2 ToObject(byte[] data)
            {
                ByteBuffer buffer = ByteBuffer.allocate(SIZE);
                buffer.Put(data, SIZE);
                buffer.filp();

                RMVIDEO_FILEHEADER_2 header = new RMVIDEO_FILEHEADER_2();
                header.hdr = RMVIDEO_STHDR.ToObject(buffer.ReadBytes(RMVIDEO_STHDR.SIZE));
                header.verDev = buffer.ReadBytes(28);
                header.verFile = buffer.ReadByte();
                header.begYear = buffer.ReadByte();
                header.begMonth = buffer.ReadByte();
                header.begDay = buffer.ReadByte();
                header.begHour = buffer.ReadByte();
                header.begMinute = buffer.ReadByte();
                header.begSecond = buffer.ReadByte();
                header.endYear = buffer.ReadByte();
                header.endMonth = buffer.ReadByte();
                header.endDay = buffer.ReadByte();
                header.endHour = buffer.ReadByte();
                header.endMinute = buffer.ReadByte();
                header.endSecond = buffer.ReadByte();
                header.chlCount = buffer.ReadByte();
                header.recordType = buffer.ReadByte();
                header.fileProtected = buffer.ReadByte();
                header.recordMode = buffer.ReadByte();
                header.videoType = buffer.ReadByte();
                header.devNo = buffer.ReadUInt16();
                header.audio = RMVIDEO_AUDIO.ToObject(buffer.ReadBytes(RMVIDEO_AUDIO.SIZE));
                header.reserved2 = buffer.ReadBytes(8);
                header.usdevType = buffer.ReadUInt16();
                header.reserved3 = buffer.ReadByte();
                header.dateShowFmt = buffer.ReadByte();
                header.szdevType = buffer.ReadBytes(12);

                return header;
            }
        }

        public struct RMVIDEO_FILEHEADER_4
        {
            public static readonly int SIZE = RMVIDEO_STHDR.SIZE + RMVIDEO_AUDIO.SIZE + 72;
            public  RMVIDEO_STHDR hdr;
            public byte[] verDev;
            public byte verFile; // 4 或者5 或者6
            public byte begYear;
            public byte begMonth;
            public byte begDay;
            public byte begHour;
            public byte begMinute;
            public byte begSecond;
            public byte endYear;
            public byte endMonth;
            public byte endDay;
            public byte endHour;
            public byte endMinute;
            public byte endSecond;
            public byte chlCount;
            public byte recordType;
            public byte fileProtected;
            public byte recordMode;
            public byte videoType;
            public UInt16 devNo;
            RMVIDEO_AUDIO audio;
            public UInt32 lIndexTableOffset;
            public UInt32 ulHeadLen;
            public UInt16 usdevType;
            public byte cDevBootMode;
            public byte cDevShutMode;
            public byte dateShowFmt;
            public byte[] szdevType;

            public static RMVIDEO_FILEHEADER_4 ToObject(byte[] data)
            {
                ByteBuffer buffer = ByteBuffer.allocate(SIZE);
                buffer.Put(data, SIZE);
                buffer.filp();

                RMVIDEO_FILEHEADER_4 header = new RMVIDEO_FILEHEADER_4();
                header.hdr = RMVIDEO_STHDR.ToObject(buffer.ReadBytes(RMVIDEO_STHDR.SIZE));
                header.verDev = buffer.ReadBytes(28);
                header.verFile = buffer.ReadByte();
                header.begYear = buffer.ReadByte();
                header.begMonth = buffer.ReadByte();
                header.begDay = buffer.ReadByte();
                header.begHour = buffer.ReadByte();
                header.begMinute = buffer.ReadByte();
                header.begSecond = buffer.ReadByte();
                header.endYear = buffer.ReadByte();
                header.endMonth = buffer.ReadByte();
                header.endDay = buffer.ReadByte();
                header.endHour = buffer.ReadByte();
                header.endMinute = buffer.ReadByte();
                header.endSecond = buffer.ReadByte();
                header.chlCount = buffer.ReadByte();
                header.recordType = buffer.ReadByte();
                header.fileProtected = buffer.ReadByte();
                header.recordMode = buffer.ReadByte();
                header.videoType = buffer.ReadByte();
                header.devNo = buffer.ReadUInt16();
                header.audio = RMVIDEO_AUDIO.ToObject(buffer.ReadBytes(RMVIDEO_AUDIO.SIZE));
                header.lIndexTableOffset = buffer.ReadUInt32();
                header.ulHeadLen = buffer.ReadUInt32();
                header.usdevType = buffer.ReadUInt16();

                byte flag = buffer.ReadByte();
                header.cDevBootMode = (byte)(flag & 0x0F);
                header.cDevShutMode = (byte)(flag & 0xF0);

                header.dateShowFmt = buffer.ReadByte();
                header.szdevType = buffer.ReadBytes(12);

                return header;
            }
        }

        public struct RMIT_HEAD
        {
            public static readonly int SIZE = 16;

            public byte[] szFlag;
            public UInt16 usIndexType;
            public byte[] reserved;
            public UInt16 usVersion;
            public UInt16 usItemCount;
            public UInt32 ulTableLength;

            public static RMIT_HEAD ToObject(byte[] data)
            {
                ByteBuffer buffer = ByteBuffer.allocate(SIZE);
                buffer.Put(data, SIZE);
                buffer.filp();

                RMIT_HEAD header = new RMIT_HEAD();
                header.szFlag = buffer.ReadBytes(4);
                header.usIndexType = buffer.ReadUInt16();
                header.reserved = buffer.ReadBytes(2);
                header.usVersion = buffer.ReadUInt16();
                header.usItemCount = buffer.ReadUInt16();
                header.ulTableLength = buffer.ReadUInt32();

                return header;
            }
        }

        public interface RMIT_FITEM
        {

        };

        public struct RMIT_FITEM_1: RMIT_FITEM
        {
            public static readonly int SIZE = 12;

            public UInt32 lFrameType;
            public UInt32 lFrameOffset;
            public byte byHour;
            public byte byMinute;
            public byte bySecond;
            public byte byMilliSecond;

            public static RMIT_FITEM_1 ToObject(byte[] data)
            {
                ByteBuffer buffer = ByteBuffer.allocate(SIZE);
                buffer.Put(data, SIZE);
                buffer.filp();

                RMIT_FITEM_1 item = new RMIT_FITEM_1();
                item.lFrameType = buffer.ReadUInt32();
                item.lFrameOffset = buffer.ReadUInt32();
                item.byHour = buffer.ReadByte();
                item.byMinute = buffer.ReadByte();
                item.bySecond = buffer.ReadByte();
                item.byMilliSecond = buffer.ReadByte();

                return item;
            }
        }

        public struct RMIT_FITEM_2: RMIT_FITEM
        {
            public static readonly int SIZE = 20;

            public UInt32 lFrameType;
            public UInt32 lFrameOffset;
            public byte byHour;
            public byte byMinute;
            public byte bySecond;
            public byte byMilliSecond;
            public UInt64 llTimeStampc;

            public static RMIT_FITEM_2 ToObject(byte[] data)
            {
                ByteBuffer buffer = ByteBuffer.allocate(SIZE);
                buffer.Put(data, SIZE);
                buffer.filp();

                RMIT_FITEM_2 item = new RMIT_FITEM_2();
                item.lFrameType = buffer.ReadUInt32();
                item.lFrameOffset = buffer.ReadUInt32();
                item.byHour = buffer.ReadByte();
                item.byMinute = buffer.ReadByte();
                item.bySecond = buffer.ReadByte();
                item.byMilliSecond = buffer.ReadByte();

                return item;
            }
        }

        public struct RMIT_INDEXTABLE
        {
            public RMIT_HEAD head;
            public RMIT_FITEM[] items;

            public static RMIT_INDEXTABLE ToObject(byte[] data, int nLen)
            {
                ByteBuffer buffer = ByteBuffer.allocate(nLen);
                buffer.Put(data, nLen);
                buffer.filp();

                RMIT_INDEXTABLE table = new RMIT_INDEXTABLE();
                table.head = RMIT_HEAD.ToObject(buffer.ReadBytes(RMIT_HEAD.SIZE));

                /*
                while(buffer.ReadBytes)

                if (table.head.usVersion == 1)
                {

                }
                else if (table.head.usVersion == 2)
                {

                }
                */

                return table;
            }

            public struct RMVIDEO_CHLHEADER
            {
                public static readonly int SIZE = RMVIDEO_STHDR.SIZE + 20;
                public RMVIDEO_STHDR hdr;
                public byte chlIndex;
                public byte frameRate;
                public byte frameRFixed;
                public byte frameRUnit;
                public byte frameRVer;
                public byte resolution;
                public byte reserved1;
                public byte[] chlName;

                public static RMVIDEO_CHLHEADER ToObject(byte[] data)
                {
                    ByteBuffer buffer = ByteBuffer.allocate(SIZE);
                    buffer.Put(data, SIZE);
                    buffer.filp();

                    RMVIDEO_CHLHEADER header = new RMVIDEO_CHLHEADER();
                    header.hdr = RMVIDEO_STHDR.ToObject(buffer.ReadBytes(RMVIDEO_STHDR.SIZE));
                    header.chlIndex = buffer.ReadByte();

                    byte flag = buffer.ReadByte();
                    header.frameRate = (byte)(flag & 0x1F);
                    header.frameRFixed = (byte)((flag >> 5)&0x01);
                    header.frameRUnit = (byte)((flag >> 6) & 0x01);
                    header.frameRVer = (byte)((flag >> 7) & 0x01);
                    header.resolution = buffer.ReadByte();
                    header.reserved1 = buffer.ReadByte();
                    header.chlName = buffer.ReadBytes(16);

                    return header;
                }
            }
        }
    }
}
