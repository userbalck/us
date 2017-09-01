using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N9MTest.SDK.net
{
    public enum DataType
    {
        BLACKBOX_DATATYPE_GPS = (1 << 0),
        BLACKBOX_DATATYPE_ALARMLOG = (1 << 1),
        BLACKBOX_DATATYPE_ACC = (1 << 2),
        BLACKBOX_DATATYPE_DEVSTATUS = (1 << 3),
        BLACKBOX_DATATYPE_USERLOG = (1<<4),
        BLACKBOX_DATATYPE_CANDATA = (1<<5),
        BLACKBOX_DATATYPE_DIALLOG = (1<<6),
    }
}
