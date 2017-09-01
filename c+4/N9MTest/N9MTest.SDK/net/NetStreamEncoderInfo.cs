using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N9MTest.SDK.net
{
    public class NetStreamEncoderInfo
    {
        //码率 单位KBPS
        [JsonProperty(PropertyName = "BR", NullValueHandling = NullValueHandling.Ignore)]
        public int Bitrate;

        //CBR VBR： 0-CBR;1- VBR
        [JsonProperty(PropertyName = "BRM", NullValueHandling = NullValueHandling.Ignore)]
        public int BitrateMode;

        //帧率
        [JsonProperty(PropertyName = "FR", NullValueHandling = NullValueHandling.Ignore)]
        public int FrameRate;
        
       //码率 单位KBPS
       [JsonProperty(PropertyName = "QLT", NullValueHandling = NullValueHandling.Ignore)]
       public int quality;

       
       //分辨率
       [JsonProperty(PropertyName = "RST", NullValueHandling = NullValueHandling.Ignore)]
       public int resolution;

       //视频使能
       [JsonProperty(PropertyName = "VEN", NullValueHandling = NullValueHandling.Ignore)]
       public int VideoEnable;


        public bool isSettingSuccess(NetStreamEncoderInfo _info)
        {
            if (this.FrameRate != _info.FrameRate)
            {
                return false;
            }

            if (this.quality != _info.quality)
            {
                return false;
            }

            if (this.resolution != _info.resolution)
            {
                return false;
            }

            if (this.VideoEnable != _info.VideoEnable)
            {
                return false;
            }

            return true;
        }
    }
}
