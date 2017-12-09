using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N9MTest.SDK.DSM
{
    public enum DSMAlarmType
    {
        //司机疲劳
        DRIVER_TIRED = 0,

        //无驾驶员
        NO_DRIVER = 1,

        //驾驶员打电话
        DRIVER_CALLUP = 2,

        //驾驶员抽烟
        DRIVER_SMOKING = 3,

        //驾驶员分心
        DRIVER_DISTRACTION = 4,

        //车道偏离
        LANE_DEPARTURE = 5,

        //前车碰撞 Vehicle collision
        VEHICLE_COLLISION = 6,

        //超速预警
        OVERSPEED_WARNING = 7,

        //车牌识别Vehicle License Plate Recognition
        VEHICLE_LICENSE_PLATE_RECOGNIATION = 8,
    }
}
