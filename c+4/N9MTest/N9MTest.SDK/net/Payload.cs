using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N9MTest.SDK.net
{
    public enum Payload
    {
        PT_SIGNAL = 0,
        PT_METADATA = 1,
        PT_H264 = 2,
        PT_VIDEO_FILE = 3,
        PT_REMOTEPLAYBACK = 4,
        PT_TALKBACK = 5,
        PT_JPEG = 6,
        PT_RAWFILE = 7,
        PT_UPGRADE = 8,
        PT_LOG = 9,
        PT_PARAM_IMPORT = 10,
        PT_PARAM_EXPORT = 11,
        PT_AUDIO = 12,
        PT_IE_PROXY = 13,
        PT_BLACKBOX = 17,
        PT_C6_SNAPSHOT = 19,
    }
}
