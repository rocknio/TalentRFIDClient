using System;
using System.Collections.Generic;
using System.Text;

namespace TalentRFIDClient
{
    //常量
    static public class StringUtility
    {
        //about log config
        static public string LOGKEYINFO = "logkey";   //log action key, config in NLog.config
        static public string LOGMODELNAME = "rfidclient";   //log model name

        //about config
        static public string CONFIGNAME = "client_config.xml";
        static public string SERVERINFO = "Server";
        static public string SERVERURL = "url";
        static public string RECONNECT = "reconnect";
        static public string READERINFO = "Reader";
        static public string READERFREQ = "freq";
        static public string UPSINFO = "UPS";
        static public string COMPORT = "com_port";
        static public string BAUDRATE = "baudrate";

        //about websocket
        static public string WEBSOCKETLOG = "WebSocket";
        static public string WEBRECONNECT = "ReConnect";

        //about register
        static public string REGISTER = "register";
        static public string LOCALIP = "local_ip";

        //about message info
        static public string MSG_TRANSID        = "trans_id";
        static public string MSG_CASE_ID        = "case_id";

        //event number
        static public int EVENT_READ_TAG_SUC              = 3000;   //读取标签成功
        static public int EVENT_CONNECT_READER_FAIL       = 3001;   //读写器连接失败
        static public int EVENT_READ_NO_TAG               = 3002;   //读标签失败，没有读取到标签
        static public int EVENT_READ_MULTI_TAG            = 3003;   //读标签失败，读到多个标签
        static public int EVENT_READ_ERROR_TAG            = 3004;   //读标签失败，读取到其他标签
        static public int EVENT_READ_PARSE_ERROR          = 3005;   //读标签失败，解析失败
        static public int EVENT_WRITE_TAG_SUC             = 4000;   //写标签成功
        static public int EVENT_WRITE_LOCK_ERR            = 4001;   //写标签失败，锁定标签失败
        static public int EVENT_WRITE_TAG_FAIL            = 4002;   //写标签失败，写入数据失败
        static public int EVENT_WRITE_DATA_ERR            = 4003;   //写标签失败，写入数据错误
        static public int EVENT_CLOSE_PC_SUC              = 9000;   //关机成功

        static public int EVENT_OTHER_ERROR               = 999;   //其他错误
    }
}
