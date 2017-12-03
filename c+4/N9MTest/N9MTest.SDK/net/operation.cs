﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N9MTest.SDK.net
{
    public enum Operation
    {
        Invalid = -1,
        SETVIDEOIMAGE,
        //设备发现
        SETNETWORKOPTION, REBOOT, DEFAULTPARAM, LISTDEVICE, LISTSERVER,
        //连接服务
        CONNECT, VERIFY, LOGIN, DESCRIBE, KEEPALIVE, CREATESTREAM, CONTROLUPDATEPARAM, CONTROLDOWNPARAM,
        //流媒体通讯
        REQUESTSTREAM, CONTROLSTREAM, MEDIAREGISTEFAILACK, MEDIATASKSTART, MEDIATASKSTOP,
        REQUESTALIVEVIDEO, CTRLSINGLEORMUL, REQUESTDOWNLOADVIDEO, CONTROLDOWNLOADVIDEO,
        REQUESTDOWNLOADEXPAND, CONTROLDOWNLOADEXPAND, REQUESTREMOTEPLAYBACK, CONTROLREMOTEPLAYBACK,
        REMOTEPLAYBACKSTART, REMOTEPLAYBACKSTOP, REMOTECHANNELSTATUS, REQUESTTALK, CONTROLTALK,
        REQUESTUPGRADE, REQUESTCATCHPIC, CONTROLCATCHPICTURE, REQUESTLOG, CONTROLDOWNLOADLOG,
        DOWNLOGSTART, DOWNLOGSTOP, REQUESTUPDATEPARAM, REQUESTDOWNPARAM, REQUESTAUDIO, CONTROLAUDIO,
        REQUESTTRANS, CONTROLTRANSLINK, REQUESTNEWALIVEVIDEO, CONTROLNEWSTREAM, REQUESTMOBILEALIVEVIDEO,
        CONTROLMOBILESTREAM, DOWNLOADDATA, CONTROLDOWNLOADDATA, DOWNDATASTART, DOWNDATASTOP, UPLOADSTAT,
        UPLOADSTOP, REQUESTID,
        //报警事件
        GETALARMSTATUSINFO, SENDALARMSTATUSINFO, SENDALARMINFO, GALARMING, SALARMING, TERMINATEALARM,
        ISSUEDALARMINFO, SENDRECORDSTATUS, CTRLRECORD, SETGPSINFO, UPLOADGPSINFO, SETUPLOADINFOMASK,
        SENDCHNCONFIGSTATUS,
        //存储模块
        GETSTORAGEINFO, SETCONTROLSTORAGE, GETCALENDAR, QUERYFILELIST, SETLOCK, GETCALENDARLOG,
        GETFILESIZEBYTIME, GETTIMESHIFTINFO, TIMESHIFTCTRL, UPTIMESHIFTRES,
        //录像参数
        GETIFRAME, SETVIDEOPARAM, SETOVERLAY, SETAUTOENCODE, GETSUPPORTFRAME, UPDATELEVEL,
        //网络参数
        TESTFUN, GETWIFIAPLIST, GETAPCONFIG, CONFIGAP, GETCONSTATEORINFO, SENDCONSTATEORINFO, UPLOADDEVINFO,
        UPALARMINFO, REGISTER, HEART, CTRLTRANSMIT, CTRLPUSH, UPINFOS,
        //设备注册
        RQTCOND, RQTCONS, CONSEQ, CONACK,
        //设备管理
        CONTROLPTZ, SETCONTROLDEVCMD, MANAGEONLINE, NOTICEUSERINFO, GETUSERRIGHTINFO, MANAGEUSERCMD,
        GETUSERINFO, GETDEVVERSIONINFO, GETCTRLUTC, SETCTRLUTC, CHECKTIME, SWITCHSTREAM, SENDTRANPORT,
        UPTRANPORT, UPDATEUSERMANAGE, SETRESTOREDEFAULT, GETDEVTYPE, GETDEVALLVERSIONS, GETDEVALLRIGHT,
        GETSEHMOELIST, UPDATESEHMOELIST, CTRLSEHOME, SENDPOWEROFF, SETPOSMONITORING, GETPOS, SPI,
        DEVUPGRADE, UPUPGRADESTATUS, GETDEVINFOSTATUS, REQUESTCTRLEVENT, CTRLEVENTSTATUS, GETYUNWEIINFO,
        UPDATEYWINFO, UPEVENTSTATUS, GETVERSINFOBYSW, SENDNETCHANGE, DISPATHERPROXYMSG, CALLBACK,
        GETIPCVERS, GETVERBYUSED, SENDCMSCONNECTSTATUS, GETCMSCONNECTSTATUS, EDITAREA, STOPCALL, NOTICECLL,
        NOTICESYNPARAM, GETAREA, UPLOADMOBILEWIFIINFO, INTELLIGENTCONTROL, UPLOADSROAD, GETNETTYPE, UPDATENETTYPE,
        UPDATEIOSTATUS, GETUPDATEIOSTATUS, SIGNINQUERY, SIGNIN, SIGNOUT, LUNCHHOUR, SMS, RFIDSTATE, RFIDDATA,
        RFIDINIT, RFIDANTENNA, UPLOADPUNCKCARD, CTRLIOOUTPUT, GETIOOUTPUT, UPPSTATISTICS, UPMSTATISTICS,
        CTRLCMDFTPDATA, UPCMDFTPRESULT, NVRCTRLSTATUS, UPLOADEXDEVSTATUS, ADJUSTSIXAXIS, GETMAINTAINTASK,
        SETLICENSEINFO, DEVNOTICEEVENT, SETALIVESWITCH, GETFILEDOWNCMSINFO, SETFILEDOWNCMSINFO, SETTASKSTATUS,
        DEVINFOCHANGEUPLOAD, DEVFAULTUPLOAD, ADJUSTEXTERNALSIXAXIS,
        //登陆视频墙
        LOGOUT, GETMULTIMONITOR, SETMULTIMONITOR, SETSHOWPARAM, GETSHOWPARAM, SETMONITORMAX, SWAPMONITOR,
        OPENVIDEO, CLOSEVIDEO, CLOSEWALLVIDEO, CLOSEALLVIDEO, LOGOUTDEV, GETDEVMULTIMONITOR, SETDEVMULTIMONITOR,
        SETDEVSHOWPARAM, GETDEVSHOWPARAM, SWAPDEVMONITOR, CTRDEV, CTRLAUDIO, SETRES,
        //参数设置
        GET, SET
    }
}