using System;
using Util;
using static N9MTest.SDK.net.N9MMSG;

namespace N9MTest.SDK.net
{
    class StreamBuffer
    {
        //数据写入的位置
        private int m_WritePos;

        //内存的大小
        private int m_nBufferSize = 0;

        //缓存区
        private byte[] buffer;
      

        public void init(int size)
        {
            buffer = new byte[size];

            m_WritePos = 0;
            m_nBufferSize = size;
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
            if (m_WritePos + nLen > m_nBufferSize)
            {
                resize(m_WritePos + nLen);
            }

            Array.Copy(data, 0, buffer, 0, nLen);

            m_WritePos += nLen;

            return 0;
        }
        
        //读取指令
        public bool ReadOutCommand(byte[] data, int[] nLen)
        {
            if (m_WritePos < Protocol_Head.SIZE)
            {
                return false;
            }

            Protocol_Head head = Protocol_Head.ToObject(buffer);
            int nCount = Endian.SwapInt32((int)head.counts);

            if (m_WritePos < Protocol_Head.SIZE + nCount)
            {
                return false;
            }

            Array.Copy(data, 0, buffer, Protocol_Head.SIZE, nCount);
            Array.Copy(buffer, 0, buffer, Protocol_Head.SIZE + nCount, m_WritePos - Protocol_Head.SIZE - nCount);

            return true;
        }

    }
}
