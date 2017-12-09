using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N9MTest.SDK
{
    public enum UploadEventType
    {
        UPLOAD_EVENTTYPE_IMPORTPARAMETER = 1 << 0,
        UPLOAD_EVENTTYPE_STATIONREPORT = 1 << 1,
        UPLOAD_EVENTTYPE_UPGRADE = 1 << 2,
        UPLOAD_EVENTTYPE_ElLECTRONICFENCE = 1 << 3,
        UPLOAD_EVENTTYPE_UPGRADEIPC = 1 << 4,
        UPLOAD_EVENTTYPE_UPGRADECP4 = 1 << 5,
    }
}
