using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N9MTest.SDK.net
{

    //0:增加新的电子围栏，只要以下电子围栏的唯一编号不同即可，否侧则进行覆盖
    //1:更新，即把旧的电子围栏全部删除，只放放新的电子围栏数据
    //2:修改，即根据电子围栏的唯一编号，来把旧的电子围栏的参数进行修改
    //3:删除制定的电子围栏,SID 有效
    //4:删除所有的电子围栏，后面不需要参数
    public enum EleFenceCmd
    {
        ADD = 0,
        UPDATE = 1,
        MODIFY = 2,
        DELETE = 3,
    }

    public class Point
    {
        [JsonProperty(PropertyName = "X", NullValueHandling = NullValueHandling.Ignore)]
        public int X;

        [JsonProperty(PropertyName = "Y", NullValueHandling = NullValueHandling.Ignore)]
        public int Y;
    }

    public class Circle
    {
        [JsonProperty(PropertyName = "ATTR", NullValueHandling = NullValueHandling.Ignore)]
        public int attitude;

        [JsonProperty(PropertyName = "ID", NullValueHandling = NullValueHandling.Ignore)]
        public int id;

        [JsonProperty(PropertyName = "NAME", NullValueHandling = NullValueHandling.Ignore)]
        public string name;

        [JsonProperty(PropertyName = "RADIUS", NullValueHandling = NullValueHandling.Ignore)]
        public int radius;

        [JsonProperty(PropertyName = "POINT", NullValueHandling = NullValueHandling.Ignore)]
        public List<Point> list;
    }

    public class Rectangle
    {
        [JsonProperty(PropertyName = "ATTR", NullValueHandling = NullValueHandling.Ignore)]
        public int attitude;

        [JsonProperty(PropertyName = "ID", NullValueHandling = NullValueHandling.Ignore)]
        public int id;

        [JsonProperty(PropertyName = "NAME", NullValueHandling = NullValueHandling.Ignore)]
        public string name;

        [JsonProperty(PropertyName = "POINT", NullValueHandling = NullValueHandling.Ignore)]
        public List<Point> list;
    }

    public class Polygon
    {
        [JsonProperty(PropertyName = "ATTR", NullValueHandling = NullValueHandling.Ignore)]
        public int attitude;

        [JsonProperty(PropertyName = "ID", NullValueHandling = NullValueHandling.Ignore)]
        public int id;

        [JsonProperty(PropertyName = "NAME", NullValueHandling = NullValueHandling.Ignore)]
        public string name;

        [JsonProperty(PropertyName = "POINT", NullValueHandling = NullValueHandling.Ignore)]
        public List<Point> list;
    }

    public class EleFenceLoader:IDisposable
    {
        private string mFileName;
        private FileStream stream;
        private BinaryReader reader;

        public EleFenceLoader(string filename)
        {
            mFileName = filename;
            this.stream = new FileStream(filename, FileMode.Open);
            this.reader = new BinaryReader(this.stream);
        }

        public JObject LoadData()
        {
            long length = this.stream.Length;

            if (length == 0)
            {
                return null;
            }

            byte[] data = this.reader.ReadBytes((int)length);

            JObject jresp = JObject.Parse(Encoding.UTF8.GetString(data));

            return jresp;

        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }

                if (this.stream != null)
                {
                    this.stream.Close();
                    this.stream = null;
                }

                if (this.reader != null)
                {
                    this.reader.Close();
                    this.reader = null;
                }

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~EleFenceLoader() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
