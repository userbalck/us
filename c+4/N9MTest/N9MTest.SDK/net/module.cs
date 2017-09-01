using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N9MTest.SDK.net
{
    public enum Module
    {
        Invalid = -1,
        //设备发现
        DISCOVERY,
        //连接服务
        CERTIFICATE,
        //流媒体通讯
        MEDIASTREAMMODEL,
        //报警事件
        EVEM,
        //存储模块
        STORM,
        //录像参数
        AVSM,
        //网络参数
        NWSM,
        //设备注册
        NAT,
        //设备管理
        DEVEMM,
        //登陆视频墙
        VW,
        //参数设置
        CONFIGMODEL
    }
}
