using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;

namespace N9MTest.SDK.net
{
    public static class BlackBox
    {
        public enum BBox
        {
            E_FILE_INFO_TYPE_NO = 0x00,
            E_FILE_INFO_TYPE_TIME = 0x01,   /*时间信息*/
            E_FILE_INFO_TYPE_GPS = 0x02,    /*GPS数据*/
            E_FILE_INFO_TYPE_ALARM = 0x03,  /*报警信息*/
            E_FILE_INFO_TYPE_ACC = 0x04,    /*ACC数据*/
            E_FILE_INFO_TYPE_VEHICLE = 0x05,    /*车辆状态基本信息*/
            E_FILE_INFO_TYPE_OFFSET = 0x06, /*文件偏移量*/
            E_FILE_INFO_TYPE_VCAN = 0x07,   /*CAN数据*/
            E_FILE_INFO_TYPE_GDS = 0x08,    /*GDS数据*/
            E_FILE_INFO_TYPE_TAX2 = 0x09,   /*TAX2数据*/
            E_FILE_INFO_TYPE_SWLKJ = 0x0A,  /*思维LKJ数据*/
            E_FILE_INFO_TYPE_GPS_DATA = 0x0B,   /*GPS原始数据*/
            E_FILE_INFO_TYPE_CUSTOM = 0x0C, /*用户自定义数据*/
            E_FILE_INFO_TYPE_TIRE = 0x0D,           /*胎压数据*/
            E_FILE_INFO_TYPE_RSR = 0x0E,        /*停录像原因数据*/
            E_FILE_INFO_TYPE_REI_CAN = 0x0F,       /*REI can Data*/
            E_FILE_INFO_TYPE_REI_ALERT = 0x10,       /*REI alert Data*/
            E_FILE_INFO_TYPE_FLIGHT_ATTITUDE = 0x11,        /*航姿数据*/
            E_FILE_INFO_TYPE_T2_FIRE_BOX = 0x12, /*T2防火盒子数据*/
            E_FILE_INFO_TYPE_MAINT = 0x13, /*运维黑匣子数据*/
            E_FILE_INFO_TYPE_RECORD_EVENT = 0x14, /*录像启停写黑匣子*/
                                                  //...
            E_FILE_INFO_TYPE_COMPLETE = 0xFF,
        }

        public interface IBaseStruct
        {

        }

        public class  RMBDM_FRAMEHEADER
        {
            public static readonly int SIZE = 16;

            public UInt32 lFrameFlag;
            public UInt32 lFrameLen;
            public UInt32 lDataCount;
            public UInt64 ullPts;
            public List<IBaseStruct> list;

            public static RMBDM_FRAMEHEADER ToObject(byte[] data, int nSourceIndex = 0)
            {
                RMBDM_FRAMEHEADER header = new RMBDM_FRAMEHEADER();

                ByteBuffer buffer = ByteBuffer.allocate(SIZE);
                buffer.Put(data, nSourceIndex, SIZE);
                buffer.filp();

                header.lFrameFlag = buffer.ReadUInt32();

                UInt32 flag = buffer.ReadUInt32();

                header.lFrameLen = flag & 0xFFFFFF;
                header.lDataCount = (flag >> 24) & 0xFF;
                header.ullPts = buffer.ReadUInt64();

                return header;
            }
        }

        public class RMBDM_INFOTYPE: IBaseStruct
        {
            public static readonly int SIZE = 4;

            public UInt32 lInfoType;
            public UInt32 lInfoLength;

            public static RMBDM_INFOTYPE ToObject(byte[] data, int nSourceIndex = 0)
            {
                RMBDM_INFOTYPE type = new RMBDM_INFOTYPE();

                ByteBuffer buffer = ByteBuffer.allocate(SIZE);
                buffer.Put(data, nSourceIndex, SIZE);
                buffer.filp();

                UInt32 flag = buffer.ReadUInt32();

                type.lInfoType = flag & 0xFF;
                type.lInfoLength = flag >> 8;

                return type;
            }
        }

        public class  RMBDM_DATETIME: IBaseStruct
        {
            public static readonly int SIZE = RMBDM_INFOTYPE.SIZE + RMS_DATETIME.SIZE;

            public RMBDM_INFOTYPE type;
            public RMS_DATETIME time;

            public static RMBDM_DATETIME ToObject(byte[] data, int nSourceIndex = 0)
            {
                RMBDM_DATETIME datetime = new RMBDM_DATETIME();
                datetime.type = RMBDM_INFOTYPE.ToObject(data, nSourceIndex);
                datetime.time = RMS_DATETIME.ToObject(data, nSourceIndex + RMBDM_INFOTYPE.SIZE);

                return datetime;
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
                time.usWeek = (UInt16)((flag>>10) & 0x07);
                time.usReserved = (UInt16)((flag >> 13) & 0x03);
                time.usMilliValidate = (UInt16)((flag >> 15) & 0x01);

                return time;
            }

            public DateTime GetDateTime()
            {
                DateTime dt = new DateTime(cYear + 2000, cMonth, cDay, cHour, cMinute, cSecond);
                return dt;
            }

            public void print()
            {
                DateTime dt = new DateTime(cYear + 2000, cMonth, cDay, cHour, cMinute, cSecond);
                Console.WriteLine("time = {0}", dt.ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }

        public class RMBDM_GPS: IBaseStruct
        {
            public static readonly int SIZE = RMBDM_INFOTYPE.SIZE + 28;

            public RMBDM_INFOTYPE type;
            public byte cVersion;
            public byte cGpsSource;
            public byte reserved1;
            public byte cGpsStatus;
            public byte cSpeedUnit;
            public UInt16 usSpeed;
            public byte cDirectionLatitude;
            public byte cDirectionLongitude;
            public byte cLatitudeDegree;
            public byte cLatitudeCent;
            public byte cLongitudeDegree;
            public byte cLongitudeCent;
            public UInt32 lLatitudeSec;
            public UInt32 lLongitudeSec;
            public UInt16 usGpsAngle;
            public byte cGpPlanetNum;
            public byte cBdPlanetNum;
  //          public byte cGlPlanetNum;
  //          public byte cGaPlanetNum;
            public byte cSignalStrength;
            public byte reserved;
            public Int16 sAltitude;

            public static RMBDM_GPS ToObject(byte[] data, int nSourceIndex = 0)
            {
                RMBDM_GPS gps = new RMBDM_GPS();
                gps.type = RMBDM_INFOTYPE.ToObject(data, nSourceIndex);

                ByteBuffer buffer = ByteBuffer.allocate(28);
                buffer.Put(data, nSourceIndex + RMBDM_INFOTYPE.SIZE, 28);
                buffer.filp();

                byte flag = buffer.ReadByte();

                gps.cVersion = (byte)(flag & 0x0F);
                gps.cGpsSource = (byte)(flag >> 4);
                gps.cGpsStatus = buffer.ReadByte();
                gps.reserved1 = buffer.ReadByte();
                gps.cSpeedUnit = buffer.ReadByte();
                gps.usSpeed = buffer.ReadUInt16();
                gps.cDirectionLatitude = buffer.ReadByte();
                gps.cDirectionLongitude = buffer.ReadByte();
                gps.cLatitudeDegree = buffer.ReadByte();
                gps.cLatitudeCent = buffer.ReadByte();
                gps.cLongitudeDegree = buffer.ReadByte();
                gps.cLongitudeCent = buffer.ReadByte();
                gps.lLatitudeSec = buffer.ReadUInt32();
                gps.lLongitudeSec = buffer.ReadUInt32();
                gps.usGpsAngle = buffer.ReadUInt16();
                gps.cGpPlanetNum = buffer.ReadByte();
                gps.cBdPlanetNum = buffer.ReadByte();

                gps.cSignalStrength = buffer.ReadByte();
                gps.reserved = buffer.ReadByte();
                gps.sAltitude = buffer.ReadInt16();

                return gps;
            }
        }

        public struct RMBDM_TIME
        {
            public static readonly int SIZE = 4;

            public byte cHour;
            public byte cMinute;
            public byte cSecond;
            public byte reserved;

            public static RMBDM_TIME ToObject(byte[] data, int nSourceIndex = 0)
            {
                RMBDM_TIME time = new RMBDM_TIME();

                ByteBuffer buffer = ByteBuffer.allocate(SIZE);
                buffer.Put(data, nSourceIndex, SIZE);
                buffer.filp();

                time.cHour = buffer.ReadByte();
                time.cMinute = buffer.ReadByte();
                time.cSecond = buffer.ReadByte();
                time.reserved = buffer.ReadByte();

                return time;
            }
        }

        public class RMBDM_ALARM : IBaseStruct
        {
            public static readonly int SIZE = RMBDM_INFOTYPE.SIZE + 160 + RMS_DATETIME.SIZE * 2;

            public RMBDM_INFOTYPE type;
            public byte cVersion;
            public byte reserved;
            public byte[] reserved0;
            public byte cAlarmType;
            public byte cAlarmStep;
            public UInt32 lSubType;
            public byte[] cOsdName;
            public byte[] cInstruction;
            public byte cPerRecord;
            public byte cDelayRecord;
            public byte[] reserved1;
            public UInt32 iChlMask;
            public RMS_DATETIME starttime;
            public RMS_DATETIME endtime;

            public static RMBDM_ALARM ToObject(byte[] data, int nSourceIndex)
            {
                RMBDM_ALARM alarm = new RMBDM_ALARM();

                alarm.type = RMBDM_INFOTYPE.ToObject(data, nSourceIndex);

                ByteBuffer buffer = ByteBuffer.allocate(160);
                buffer.Put(data, nSourceIndex + RMBDM_INFOTYPE.SIZE, 160);
                buffer.filp();

                byte flag = buffer.ReadByte();
                alarm.cVersion = (byte)(flag & 0x0F);
                alarm.reserved = (byte)(flag >> 4);
                alarm.reserved0 = buffer.ReadBytes(1);
                alarm.cAlarmType = buffer.ReadByte();
                alarm.cAlarmStep = buffer.ReadByte();
                alarm.lSubType = buffer.ReadUInt32();
                alarm.cOsdName = buffer.ReadBytes(16);
                alarm.cInstruction = buffer.ReadBytes(128);
                alarm.cPerRecord = buffer.ReadByte();
                alarm.cDelayRecord = buffer.ReadByte();
                alarm.reserved1 = buffer.ReadBytes(2);
                alarm.iChlMask = buffer.ReadUInt32();

                alarm.starttime = RMS_DATETIME.ToObject(data, nSourceIndex + RMBDM_INFOTYPE.SIZE + 160);
                alarm.endtime = RMS_DATETIME.ToObject(data, nSourceIndex + RMBDM_INFOTYPE.SIZE + 160 + RMS_DATETIME.SIZE);

                return alarm;
            }
        }

        public class RMBDM_MACCEL: IBaseStruct
        {
            public static readonly int SIZE = RMBDM_INFOTYPE.SIZE + 20;

            public RMBDM_INFOTYPE type;
            public byte cVersion;
            public byte reserved;
            public byte ucAccSource;
            public UInt16 shUnit;
            public UInt16 shAngleUnit;
            public byte[] reserved0;
            public UInt16 usAccelerateX;
            public UInt16 usZeroX;
            public UInt16 usAccelerateY;
            public UInt16 usZeroY;
            public UInt16 usAccelerateZ;
            public UInt16 usZeroZ;
            public UInt16 usAccelerateAngleX;
            public UInt16 usAngleZeroX;
            public UInt16 usAccelerateAngleY;
            public UInt16 usAngleZeroY;
            public UInt16 usAccelerateAngleZ;
            public UInt16 usAngleZeroZ;

            public static RMBDM_MACCEL ToObject(byte[] data, int nSourceIndex)
            {
                RMBDM_MACCEL accel = new RMBDM_MACCEL();

                ByteBuffer buffer = ByteBuffer.allocate(160);
                buffer.Put(data, nSourceIndex + RMBDM_INFOTYPE.SIZE, 20);
                buffer.filp();

                byte flag = buffer.ReadByte();
                accel.cVersion = (byte)(flag & 0x0F);
                accel.reserved = (byte)(flag & 0xF0);
                accel.ucAccSource = buffer.ReadByte();
                accel.shUnit = buffer.ReadUInt16();
                accel.shAngleUnit = buffer.ReadUInt16();
                accel.reserved0 = buffer.ReadBytes(2);

                UInt16 x = buffer.ReadUInt16();
                accel.usAccelerateX = (UInt16)(x & 0x7FFF);
                accel.usZeroX = (UInt16)(x >> 15);

                UInt16 y = buffer.ReadUInt16();
                accel.usAccelerateY = (UInt16)(y & 0x7FFF);
                accel.usZeroY = (UInt16)(y >> 15);

                UInt16 z = buffer.ReadUInt16();
                accel.usAccelerateZ = (UInt16)(z & 0x7FFF);
                accel.usZeroZ = (UInt16)(z >> 15);

                UInt16 angleX = buffer.ReadUInt16();
                accel.usAccelerateAngleX = (UInt16)(angleX & 0x7FFF);
                accel.usAngleZeroX = (UInt16)(angleX >> 15);

                UInt16 angleY = buffer.ReadUInt16();
                accel.usAccelerateAngleY = (UInt16)(angleY & 0x7FFF);
                accel.usAngleZeroY = (UInt16)(angleY >> 15);

                UInt16 angleZ = buffer.ReadUInt16();
                accel.usAccelerateAngleZ = (UInt16)(angleZ & 0x7FFF);
                accel.usAngleZeroZ = (UInt16)(angleZ >> 15);

                return accel;
            }
        }

        public struct RMBDM_LONGINFO
        {
            public static readonly int SIZE = 8;
            public byte cType;
            public byte[] reserved;
            public byte reserved2;
            public byte cNegative;
            public byte cUnit;
            public UInt32 lValue;

            public static RMBDM_LONGINFO ToObject(byte[] data, int nSourceIndex)
            {
                RMBDM_LONGINFO info = new RMBDM_LONGINFO();

                ByteBuffer buffer = ByteBuffer.allocate(SIZE);
                buffer.Put(data, nSourceIndex, SIZE);
                buffer.filp();

                info.cType = buffer.ReadByte();
                info.reserved = buffer.ReadBytes(2);

                byte flag = buffer.ReadByte();
                info.reserved2 = (byte)(flag & 0x07);
                info.cNegative = (byte)((flag >> 3) & 0x01);
                info.cUnit = (byte)((flag >> 4) & 0x0F);
                info.lValue = buffer.ReadUInt32();

                return info;
            }
        }

        public class RMBDM_LONGLIST: IBaseStruct
        {
            public static readonly int SIZE = RMBDM_INFOTYPE.SIZE + 4;
            public RMBDM_INFOTYPE type;
            public byte cItemCount;
            public byte[] reserved;
            public List<RMBDM_LONGINFO> list;

            public static RMBDM_LONGLIST ToObject(byte[] data, int nSourceIndex)
            {
                RMBDM_LONGLIST longlist = new RMBDM_LONGLIST();

                longlist.type = RMBDM_INFOTYPE.ToObject(data);

                ByteBuffer buffer = ByteBuffer.allocate(160);
                buffer.Put(data, nSourceIndex + RMBDM_INFOTYPE.SIZE, 4);
                buffer.filp();

                longlist.cItemCount = buffer.ReadByte();
                longlist.reserved = buffer.ReadBytes(3);

                return longlist;
            }
        }
    }
}
