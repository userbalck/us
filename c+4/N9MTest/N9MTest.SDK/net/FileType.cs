using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N9MTest.SDK.net
{
    public enum FileType
    {
        CALENDAR_FILETYPE_NORMAL = 1 << 0,
        CALENDAR_FILETYPE_ALARM = 1 << 1,
        CALENDAR_FILETYPE_ALARM_INSPECT = 1 << 2,
        CALENDAR_FILETYPE_WARNING_INSPECT = 1 << 3,
        CALENDAR_FILETYPE_ALARM_VIOLATION = 1 << 4,
        CALENDAR_FILETYPE_WARNING_VIOLATION = 1 << 5,
        CALENDAR_FILETYPE_LOCK = 1 << 6,
    }
}
