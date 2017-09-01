using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N9MTest.SDK.net
{
    public enum Navigation
    {
        //GPS
        GPS = 0,

        //北斗导航系统
        BeiDou = 1,

        //伽利略卫星导航系统
        Galileo = 2,

        //格洛纳斯卫星导航系统
        Glonass = 3,

        //混合的
        Mixed = 4,
    }
}
