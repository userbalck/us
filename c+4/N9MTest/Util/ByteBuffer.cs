using System;
using System.Collections.Generic;
using System.Text;

namespace Util
{
    public class ByteBuffer : IDisposable
    {
        private byte[] data;//数据
        public int index;//当前指针
        public int cur;//永远指向数据最后一次读取的地方，平时随index移动，flip时index归零，cur不变
        public int limit;//极限容纳能力
        private ByteOrder byteOrder;//字节顺序

        private ByteBuffer()
        {

        }
        public void Dispose()
        {
            data = null;
        }
        public void filp()
        {
            index = 0;
        }
        public void reset()
        {
            index = cur = 0;
        }
        public void forward(int size)
        {
            index += size;
            cur += size;
        }

        public void setByteOrder(ByteOrder bo)
        {
            this.byteOrder = bo;
        }
        public void Close()
        {
            data = null;
        }
        /***********数据读取部分*************/
        /// <summary>
        /// 绑定数据，构造实例
        /// </summary>
        /// <param name="data">二进制数据</param>
        /// <returns>ByteBuffer实例</returns>
        public static ByteBuffer wrapData(byte[] data, int startIndex, int endIndex)
        {
            if (data == null || data.Length == 0)
            {
                throw new OverflowException();
            }
            ByteBuffer instance = new ByteBuffer();
            instance.data = data;
            instance.byteOrder = ByteOrder.LITTLE;
            instance.index = instance.index = startIndex;
            instance.limit = endIndex;// data.Length;
            return instance;
        }
        public static ByteBuffer wrapData(byte[] data)
        {
            return wrapData(data, 0, data.Length);
        }
        public byte ReadByte()
        {
            byte b = data[index];
            index++;
            cur++;
            return b;
        }
        public byte[] ReadBytes(int count)
        {
            byte[] bs = new byte[count];
            System.Array.Copy(data, index, bs, 0, count);
            index += count;
            cur += count;
            return bs;
        }
        public float ReadSingle()
        {
            float val;
            if (byteOrder == ByteOrder.LITTLE)
            {
                val = BitConverter.ToSingle(data, index);

            }
            else
            {
                byte[] b = new byte[4];
                {
                    System.Array.Copy(data, index, b, 0, 4);
                    Array.Reverse(b, 0, b.Length);
                    val = BitConverter.ToSingle(b, 0);
                }
            }
            index += 4;
            cur += 4;
            return val;
        }
        public double ReadDouble()
        {
            double val;
            if (byteOrder == ByteOrder.LITTLE)
            {
                val = BitConverter.ToDouble(data, index);

            }
            else
            {
                byte[] b = new byte[8];
                {
                    System.Array.Copy(data, index, b, 0, 8);
                    Array.Reverse(b, 0, b.Length);
                    val = BitConverter.ToDouble(b, 0);
                }
            }
            index += 8;
            cur += 8;
            return val;
        }
        public short ReadInt16()
        {
            short val;
            if (byteOrder == ByteOrder.LITTLE)
            {
                val = BitConverter.ToInt16(data, index);

            }
            else
            {
                byte[] b = new byte[2];

                System.Array.Copy(data, index, b, 0, 2);
                Array.Reverse(b, 0, b.Length);
                val = BitConverter.ToInt16(b, 0);

            }
            index += 2;
            cur += 2;
            return val;
        }
        public ushort ReadUInt16()
        {
            ushort val;
            if (byteOrder == ByteOrder.LITTLE)
            {
                val = BitConverter.ToUInt16(data, index);
            }
            else
            {
                byte[] b = new byte[2];
                System.Array.Copy(data, index, b, 0, 2);
                Array.Reverse(b, 0, b.Length);
                val = BitConverter.ToUInt16(b, 0);
            }
            index += 2;
            cur += 2;
            return val;
        }

        public int ReadInt32()
        {
            int val;
            if (byteOrder == ByteOrder.LITTLE)
            {
                val = BitConverter.ToInt32(data, index);
            }
            else
            {
                byte[] b = new byte[4];

                System.Array.Copy(data, index, b, 0, b.Length);
                Array.Reverse(b, 0, b.Length);
                val = BitConverter.ToInt32(b, 0);

            }
            index += 4;
            cur += 4;
            return val;
        }
        public uint ReadUInt32()
        {
            uint val;
            if (byteOrder == ByteOrder.LITTLE)
            {
                val = BitConverter.ToUInt32(data, index);
            }
            else
            {
                byte[] b = new byte[4];
                System.Array.Copy(data, index, b, 0, b.Length);
                Array.Reverse(b, 0, b.Length);
                val = BitConverter.ToUInt32(b, 0);
            }
            index += 4;
            cur += 4;
            return val;
        }

        public long ReadInt64()
        {
            long val;
            if (byteOrder == ByteOrder.LITTLE)
            {
                val = BitConverter.ToInt64(data, index);
            }
            else
            {
                byte[] b = new byte[8];

                System.Array.Copy(data, index, b, 0, b.Length);
                Array.Reverse(b, 0, b.Length);
                val = BitConverter.ToInt64(b, 0);

            }
            index += 8;
            cur += 8;
            return val;
        }

        public UInt64 ReadUInt64()
        {
            UInt64 val;
            if (byteOrder == ByteOrder.LITTLE)
            {
                val = BitConverter.ToUInt64(data, index);
            }
            else
            {
                byte[] b = new byte[8];

                System.Array.Copy(data, index, b, 0, b.Length);
                Array.Reverse(b, 0, b.Length);
                val = BitConverter.ToUInt64(b, 0);

            }
            index += 8;
            cur += 8;
            return val;
        }
        public sbyte ReadSByte()
        {
            sbyte val = (sbyte)data[index];
            index++;
            cur++;
            return val;
        }

        public bool ReadBoolean()
        {
            bool val = data[index] == 0 ? false : true;
            index++;
            cur++;
            return val;
        }
        public char ReadChar()
        {
            char val = BitConverter.ToChar(data, index);
            index++;
            cur++;
            return val;
        }
        public char[] ReadChars(int count)
        {
            char[] val = new char[count];
            for (int c = 0; c < count; c++)
            {

                val[c] = BitConverter.ToChar(data, index);
                index++;
                cur++;
            }
            /*if (byteOrder != ByteOrder.LITTLE)
            {
                System.Array.Reverse(val);  
            } */
            return val;
        }
        public String ReadString(int count)
        {
            String val = Encoding.UTF8.GetString(data, index, count);
            index += count;
            cur += count;
            return val;
        }

        /**********数据写入部分*********/
        public static ByteBuffer allocate(int size)
        {
            if (size <= 0)
            {
                throw new OverflowException();
            }
            ByteBuffer instance = new ByteBuffer();
            instance.data = new byte[size];
            instance.byteOrder = ByteOrder.LITTLE;
            instance.index = instance.index = 0;
            instance.limit = size;
            return instance;
        }

        public void Put(byte b)
        {
            data[index] = b;
            index++;
            cur++;
        }
        public void Put(byte[] b)
        {
            System.Array.Copy(b, 0, data, index, b.Length);
            index += b.Length;
            cur += b.Length;
        }

        public void Put(byte[] b, int sourceIndex, int length)
        {
            System.Array.Copy(b, sourceIndex, data, index, length);
            index += length;
            cur += length;
        }

        public void Put(byte[] b, int length)
        {
            System.Array.Copy(b, 0, data, index, length);
            index += length;
            cur += length;
        }

        public void PutShort(short s)
        {
            byte[] temp = BitConverter.GetBytes(s);
            if (this.byteOrder == ByteOrder.BIG)
                Array.Reverse(temp, 0, temp.Length);
            this.Put(temp);
        }

        public void PutInt32(Int32 num)
        {
            byte[] temp = BitConverter.GetBytes(num);
            if (this.byteOrder == ByteOrder.BIG)
                Array.Reverse(temp, 0, temp.Length);
            this.Put(temp);
        }

        public void PutInt64(Int64 num)
        {
            byte[] temp = BitConverter.GetBytes(num);
            if (this.byteOrder == ByteOrder.BIG)
                Array.Reverse(temp, 0, temp.Length);
            this.Put(temp);
        }

        //string double 不分大小端
        public void PutString(string str)
        {
            byte[] temp = System.Text.Encoding.UTF8.GetBytes(str);
            //if (this.byteOrder == ByteOrder.BIG)
            //Array.Reverse(temp, 0, temp.Length);
            this.Put(temp);
        }

        public void PutDouble(double d)
        {
            byte[] temp = BitConverter.GetBytes(d);
            //if (this.byteOrder == ByteOrder.BIG)
            //Array.Reverse(temp, 0, temp.Length);
            this.Put(temp);
        }

        public byte[] ToArray()
        {
            byte[] result = new byte[index];
            Array.Copy(data, 0, result, 0, index);
            return result;
        }
    }
}
