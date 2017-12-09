using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace N9MTest.Record
{
    public enum RecordMode
    {
        //开机录像
        Boot = 0,

        //定时录像
        Timing = 1,

        //报警录像
        Alarm = 2,
    }
}
