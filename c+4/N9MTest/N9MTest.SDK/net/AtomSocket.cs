using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Util;

namespace N9MTest.SDK.net
{
    public class AtomSocket:TcpClient
    {
        private NetworkStream ns = null;
        private BinaryReader br { get; set; }
        private BinaryWriter bw { get; set; }
        private string streamname;

        public string GetSpecKeywords()
        {
            if (streamname == null)
            {
                streamname = Guid.NewGuid().ToString();
            }

            return streamname;
        }

        public NetworkStream GetStream()
        {
            Console.WriteLine("GetStream");
            if (ns == null)
            {
                ns = base.GetStream();
                br = new BinaryReader(ns);
                bw = new BinaryWriter(ns);
            }

            return ns;
        }

        public BinaryReader GetReader()
        {
            return this.br;
        }

        public BinaryWriter GetWriter()
        {
            return this.bw;
        }
    }
}
