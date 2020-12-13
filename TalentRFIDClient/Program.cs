using System;
using System.Collections.Generic;
using System.Text;
using NLog;
using System.Diagnostics;
using System.Threading;

namespace TalentRFIDClient
{
    class Program
    {
        static void Main(string[] args)
        {
            NLog.Logger txtLogger = LogManager.GetLogger(StringUtility.LOGMODELNAME);
            MappedDiagnosticsContext.Set(StringUtility.LOGKEYINFO, "MAINPROCESS");
            bool isRun = false;
            Process[] pa = Process.GetProcesses();//获取当前进程数组。  
            foreach (Process PTest in pa)
            {
                if (PTest.ProcessName == Process.GetCurrentProcess().ProcessName)
                {
                    if (isRun)
                    {
                        //如果程序已经运行，则给出提示。并退出本进程。  
                        txtLogger.Info("Process had already start!!!!");
                        return;
                    }
                    else
                    {
                        isRun = true;
                    }
                }
            }

            txtLogger.Info("------------------------Start TalentRFIDClient -----------------------");

            XMLConfigParser.Instance().SetConfigName(StringUtility.CONFIGNAME);
            XMLConfigParser.Instance().XMLLoad();
            string url = "";
            string reconnect = "15";
            XMLConfigParser.Instance().GetParameter(StringUtility.SERVERINFO, StringUtility.SERVERURL, ref url);
            XMLConfigParser.Instance().GetParameter(StringUtility.SERVERINFO, StringUtility.RECONNECT, ref reconnect);

            WebSocketClient web_socket = new WebSocketClient(url, reconnect);

            /*
            string com_port = "";
            string baud_str = "";
            XMLConfigParser.Instance().GetParameter(StringUtility.UPSINFO, StringUtility.COMPORT, ref com_port);
            XMLConfigParser.Instance().GetParameter(StringUtility.UPSINFO, StringUtility.BAUDRATE, ref baud_str);
            UPSHandle ups_handle = new UPSHandle(com_port, baud_str);
            ups_handle.StartSerialPortMonitor();
            */
            web_socket.Start_WebSocket();
        }
    }
}
