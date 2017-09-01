using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N9MTest.Record
{
    public enum OverwriteMode
    {
        //按天覆盖
        Days = 0,

        //按容量覆盖
        Capacity = 1,

        //永不覆盖
        Never = 2,
    }
}
