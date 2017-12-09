using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using Util;

namespace N9MTest.SDK.net
{
    public class N9MMSG
    {
        public static byte[] Serialize(JObject value)
        {
            return Serialize(ENUM_PT_TYPE.PT_SIGNAL,value);
        }

        public static byte[] Serialize(ENUM_PT_TYPE pt, JObject value)
        {
            string command = JsonConvert.SerializeObject(value);

            byte[] data = Encoding.UTF8.GetBytes(command);

 //           Console.WriteLine("command.length = {0}", command.Length);

            Protocol_Head head = new Protocol_Head();

            head.PT = (UInt32)pt;
            head.counts = (UInt32)Endian.SwapInt32((int)data.Length);
            head.reserve = (UInt32)'R';

            ByteBuffer buffer =  ByteBuffer.allocate(12 + data.Length);
            buffer.Put(head.ToBytes());
            buffer.PutString(command);

            return buffer.ToArray();
        }

        public static JObject Deserialize(byte[] data, int nLen)
        {
            Console.WriteLine("Deserialize");
            ByteBuffer buffer = ByteBuffer.allocate(nLen);
            buffer.Put(data, nLen);

            Protocol_Head head = Protocol_Head.ToObject(data);

            int count = Endian.SwapInt32((int)head.counts);

            Console.WriteLine("head.PT = {0}, head.counts = {1}", Enum.GetName(typeof(ENUM_PT_TYPE), head.PT),(UInt32)Endian.SwapInt32((int)head.counts));

            JObject resp = JObject.Parse(Encoding.UTF8.GetString(data, Protocol_Head.SIZE, count));
            return resp;
        }

        public enum ENUM_PT_TYPE
        {
            PT_SIGNAL = 0,
            PT_METADATA = 1,
            PT_H264 = 2,
            PT_VIDEO_FILE = 3,
            PT_REMOTEPLAYBACK = 4,
            PT_TALKBACK = 5,
            PT_JPEG = 6,
            PT_RAWFILE = 7,
            PT_UPGRADE = 8,
            PT_LOG = 9,
            PT_PARAM_IMPORT = 10,
            PT_PARAM_EXPORT = 11,
            PT_AUDIO = 12,
            PT_IE_PROXY = 13,
            PT_BLACKBOX = 17,
            PT_C6_SNAPSHOT = 19
        };

        public class Protocol_Head
        {
            public static readonly int SIZE = 12;

            public UInt32 V = 0;
            public UInt32 P = 0;
            public UInt32 M = 0;
            public UInt32 CSRC = 0;
            public UInt32 PT = 0;
            public UInt32 SSRC = 0;
            public UInt32 counts = 0;
            public UInt32 reserve = 'R';

            public byte[] ToBytes()
            {
                ByteBuffer buffer = ByteBuffer.allocate(12);
                UInt32 flag = 0;
                flag |= (this.V & 0x00000003);
                flag |= (this.P & 0x00000001) << 2;
                flag |= (this.M & 0x00000001) << 3;
                flag |= (this.CSRC & 0x0000000F) << 4;
                flag |= (this.PT & 0x000000FF) << 8;
                flag |= (this.SSRC & 0x000000FF) << 16;

                buffer.PutInt32((Int32)flag);
                buffer.PutInt32((Int32)this.counts);
                buffer.PutInt32((Int32)this.reserve);
                return buffer.ToArray();
            }

            public static Protocol_Head ToObject(byte[] data)
            {
   //             Console.WriteLine("[ToObject]data:{0}", BitConverter.ToString(data, 0, Protocol_Head.SIZE));

                Protocol_Head head = new Protocol_Head();
                ByteBuffer buffer = ByteBuffer.allocate(SIZE);
                buffer.Put(data, SIZE);
                buffer.filp();

                uint flag = buffer.ReadUInt32();
                head.V = flag & 0x03;
                head.P = (flag >> 2) & 0x01;
                head.M = (flag >> 3) & 0x01;
                head.CSRC = (flag>>4)&0x0F;
                head.PT = (flag >> 8) & 0xFF;
                head.SSRC = (flag >> 16) & 0xFFFF;
                head.counts = buffer.ReadUInt32();
                head.reserve = buffer.ReadUInt32();
                return head;
            }
        }
    }
}
