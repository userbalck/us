using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N9MTest.SDK.net
{
    public enum StorageStatus
    {
        STORAGE_STATE_NOEXISTS = 0,
        STORAGE_STATE_UNFORMATTED = 1,
        STORAGE_STATE_FORMATTING = 2,
        STORAGE_STATE_UNMOUNT = 3,
        STORAGE_STATE_FULL = 4,
        STORAGE_STATE_NORMAL = 5,
        STORAGE_STATE_RECORDING = 6,
        STORAGE_STATE_RW_ERROR = 7,
        STORAGE_STATE_TESTING = 8,
        STORAGE_STATE_PARTITION_ERROR = 9,
    }
}
