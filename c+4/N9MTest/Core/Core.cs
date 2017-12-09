using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RM
{
    /// <summary>
    /// Version : 2017-03-17
    /// </summary>
    public class Core
    {
        public static Random random = GetRandom();
        public static string WorkPath = $"{AppDomain.CurrentDomain.BaseDirectory}\\";

        public static string Print(string s)
        {
            string log = $"[{DateTime.Now.ToString("HH:mm:ss")}] {s}";
            Console.WriteLine(log); return (log);
        }
        /// <summary>
        /// 获取一个随机数
        /// </summary>
        /// <returns></returns>
        private static Random GetRandom()
        {
            long tick = DateTime.Now.Ticks;
            return (new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32)));
        }
        /// <summary>
        /// 将数字格式为：100GB/100MB/100KB/100B这样的字符串
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string FormatSize(Int64 size)
        {
            if (size / 1024 / 1024 / 1024 > 0) return (Math.Round(size * 1D / 1024 / 1024 / 1024, 2) + "GB");
            else if (size / 1024 / 1024 > 0) return (Math.Round(size * 1D / 1024 / 1024, 2) + "MB");
            else if (size / 1024 > 0) return (Math.Round(size * 1D / 1024, 2) + "KB");
            else return (size + "B");
        }
        /// <summary>
        /// </summary>
        /// <param name="data"></param>
        /// <returns>return : 7E 01 00 00 2B 06 49 20</returns>
        public static string Bytes2HexString(byte[] data)
        {
            return (Bytes2HexString(data, 0, data.Length));
        }
        /// <summary>
        /// </summary>
        /// <param name="data"></param>
        /// <param name="start"></param>
        /// <param name="count">return : 7E 01 00 00 2B 06 49 20</param>
        /// <returns></returns>
        public static string Bytes2HexString(byte[] data, int start, int count)
        {
            string result = "";
            for (int i = start; i < count + start; i++)
            {
                if (result != "") result += " ";
                result += Convert.ToString(data[i], 16).PadLeft(2, '0');
            }
            return (result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="start"></param>
        /// <param name="IsLittleEndian">data字节数组是否是低位在前</param>
        /// <returns></returns>
        public static UInt16 Bytes2UInt16(byte[] data, int start, bool IsLittleEndian = true)
        {
            byte[] b = new byte[] { data[start], data[start + 1] };
            if (BitConverter.IsLittleEndian != IsLittleEndian) Array.Reverse(b);
            return (BitConverter.ToUInt16(b, 0));
        }

        public static byte[] UInt162Bytes(UInt16 v, bool IsLittleEndian = true)
        {
            byte[] b = BitConverter.GetBytes((UInt16)v);
            if (BitConverter.IsLittleEndian != IsLittleEndian) Array.Reverse(b);
            return (b);
        }

        public static Int32 Bytes2Int32(byte[] data, int start, bool IsLittleEndian = true)
        {
            byte[] b = new byte[] { data[start], data[start + 1], data[start + 2], data[start + 3] };
            if (BitConverter.IsLittleEndian != IsLittleEndian) Array.Reverse(b);
            return (BitConverter.ToInt32(b, 0));
        }

        public static byte[] Int322Bytes(Int32 v, bool IsLittleEndian = true)
        {
            byte[] b = BitConverter.GetBytes((Int32)v);
            if (BitConverter.IsLittleEndian != IsLittleEndian) Array.Reverse(b);
            return (b);
        }
        /// <summary>
        /// 将UInt16合并到data数组尾
        /// </summary>
        /// <param name="data"></param>
        /// <param name="v"></param>
        /// <param name="IsLittleEndian">data字节数组是否是低位在前</param>
        /// <returns></returns>
        public static byte[] MergeBytes(byte[] data, UInt16 v, bool IsLittleEndian = true)
        {
            byte[] bv = BitConverter.GetBytes(v);
            if (BitConverter.IsLittleEndian != IsLittleEndian) Array.Reverse(bv);
            return (_MergeBytes(data, bv));
        }
        /// <summary>
        /// 将(byte)v添加到数据data尾
        /// </summary>
        /// <param name="data"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static byte[] MergeByte(byte[] data, byte v)
        {
            return (_MergeBytes(data, new byte[] { v }));
        }
        /// <summary>
        /// 将字符串添加到data数组尾
        /// </summary>
        /// <param name="data"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static byte[] MergeBytes(byte[] data, string s)
        {
            byte[] bs = Encoding.Default.GetBytes(s);
            return (_MergeBytes(data, bs));
        }
        /// <summary>
        /// 将数组v添加到data数组尾
        /// </summary>
        /// <param name="data"></param>
        /// <param name="v"></param>
        /// <param name="IsLittleEndian">data字节数组是否是低位在前，如不在前将反转字节数组v</param>
        /// <returns></returns>
        public static byte[] MergeBytes(byte[] data, byte[] v, bool IsLittleEndian = true)
        {
            if (BitConverter.IsLittleEndian != IsLittleEndian) Array.Reverse(v);
            return (_MergeBytes(data, v));
        }

        private static byte[] _MergeBytes(byte[] source, byte[] destin)
        {
            if (source == null || source.Length == 0) return (destin);
            else
            {
                byte[] data = new byte[source.Length + destin.Length];
                Array.Copy(source, data, source.Length);
                Array.Copy(destin, 0, data, source.Length, destin.Length);
                return (data);
            }
        }

        /// <summary>
        /// 将读取的字节转换为结构值,例： Net_GPS _gps = (Net_GPS)Core.GetObject<Net_GPS>(bytes, pos, sz);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T GetObject<T>(byte[] data, int start, int len)
        {
            //int len = Marshal.SizeOf(typeof(T));
            //int len = data.Length;
            IntPtr pnt = Marshal.AllocHGlobal(len);
            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(data, start, pnt, len);
                return (T)Marshal.PtrToStructure(pnt, typeof(T));
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }
        }
        /// <summary>
        /// 将结构值转换为字节，以便可写入文件
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] GetObjectBytes(object obj)
        {
            var size = Marshal.SizeOf(obj.GetType());
            var data = new byte[size];
            IntPtr pnt = Marshal.AllocHGlobal(size);

            try
            {
                Marshal.StructureToPtr(obj, pnt, true);
                // Copy the array to unmanaged memory.
                Marshal.Copy(pnt, data, 0, size);
                return data;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }
        }
    }
}
