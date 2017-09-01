using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N9MTest.SDK.net
{
    public enum AlarmType
    {
        //视频丢失
        VideoLoss = 0,

        //视频遮挡
        VideoShelt = 1,

        //存储器异常
        Storage = 2,

        //用户自定义报警
        Customized = 3,

        //ACC报警
        ACC = 18,

        //DSM报警数据
        DSM = 68,
    }
}
