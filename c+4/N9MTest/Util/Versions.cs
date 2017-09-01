using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    public class Versions
    {
        public static string GetFirmwareVersion(string fullpath)
        {
            string version = "";
            FileStream stream = new FileStream(fullpath, FileMode.Open);
            BinaryReader reader = new BinaryReader(stream);
            stream.Seek(64, SeekOrigin.Begin);
            version = Encoding.UTF8.GetString(reader.ReadBytes(32));
            version = version.Replace("\0", "");

            stream.Close();
            reader.Close();
            return version;
        }
    }
}
