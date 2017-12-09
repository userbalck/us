using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Util;
using static N9MTest.SDK.net.N9MMSG;

namespace N9MTest.SDK.net
{
    class CommandBuffer
    {
        private CommandBuffer()
        {

        }

        //数据写入的位置
        private int m_WritePos = 0;

        //内存的大小
        private int m_nBufferSize = 0;

        //缓存区
        private byte[] buffer= null;


        public static CommandBuffer init(int size)
        {
            CommandBuffer commandBuffer = new CommandBuffer();

            commandBuffer.buffer = new byte[size];

            commandBuffer.m_WritePos = 0;
            commandBuffer.m_nBufferSize = size;

            return commandBuffer;
        }

        public void reset()
        {
            m_WritePos = 0;
        }

        public void release()
        {
            buffer = null;
            m_WritePos = 0;
        }

        public void resize(int size)
        {
       //     Console.WriteLine("resize size ={0}", size);
            if (size <= m_nBufferSize)
            {
                return;
            }

            byte[] temp = new byte[size];
            Array.Copy(buffer, 0, temp, 0, m_WritePos);
            buffer = temp;
        }

        //写入数据
        public int WriteInData(byte[] data, int nLen)
        {
   //         Console.WriteLine("[WriteInData][before]nLen = {0},m_WritePos = {1}", nLen, m_WritePos);

 //           Console.WriteLine("string:{0}", Encoding.UTF8.GetString(data, 0, nLen));
 //           Console.WriteLine("data:{0}", BitConverter.ToString(data, 0, nLen));

            if (m_WritePos + nLen > m_nBufferSize)
            {
                resize(m_WritePos + nLen);
            }

            Array.Copy(data, 0, buffer, m_WritePos, nLen);

            m_WritePos += nLen;

//            Console.WriteLine("[WriteInData][after]nLen = {0},m_WritePos = {1}", nLen, m_WritePos);

            return 0;
        }

        //读取指令
        public bool ReadOutCommand(Protocol_Head[] head, byte[] data, int[] nLen)
        {
 //           Console.WriteLine("ReadOutCommand");
            nLen[0] = 0;

            if (m_WritePos < Protocol_Head.SIZE)
            {
//                Console.WriteLine("[ReadOutCommand]m_WritePos = {0}", m_WritePos);
                return false;
            }

            head[0] = Protocol_Head.ToObject(buffer);
            int nCount = Endian.SwapInt32((int)head[0].counts);

 //           Console.WriteLine("[ReadOutCommand]head[0].counts = {0} nCount = {1}, head[0].reserve = {2}", head[0].counts, nCount, head[0].reserve);

            if (m_WritePos < Protocol_Head.SIZE + nCount)
            {
//                Console.WriteLine("[ReadOutCommand]m_WritePos = {0} nCount = {1},", m_WritePos, nCount);
                return false;
            }

 //           Console.WriteLine("[ReadOutCommand][before]m_WritePos ={0}", m_WritePos);

            Array.Copy(buffer, Protocol_Head.SIZE, data, 0, nCount);
            Array.Copy(buffer, Protocol_Head.SIZE + nCount, buffer, 0, m_WritePos - Protocol_Head.SIZE - nCount);

            m_WritePos = m_WritePos - Protocol_Head.SIZE - nCount;
            nLen[0] = nCount;

 //           Console.WriteLine("[ReadOutCommand][after]m_WritePos ={0}", m_WritePos);

            if (m_WritePos > 12)
            {
 //               Console.WriteLine("[ReadOutCommand]string:{0}", Encoding.UTF8.GetString(buffer, Protocol_Head.SIZE, m_WritePos- Protocol_Head.SIZE));
 //               Console.WriteLine("[ReadOutCommand]data:{0}", BitConverter.ToString(buffer, 0, m_WritePos));
            }
            

            return true;
        }
    }
}
