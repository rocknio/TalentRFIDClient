using System;
using System.Collections.Generic;
using System.Text;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Threading;
using NLog;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Net.NetworkInformation;
using System.Globalization;

namespace TalentRFIDClient
{
    //websocket operate
    public class WebSocketClient
    {
        private string m_serverUrl;
        private string m_localIp;
        private int m_reconnect;

        private WebSocket m_webSsocket;

        private NLog.Logger txtLogger = LogManager.GetLogger(StringUtility.LOGMODELNAME);

        public WebSocketClient(string serverUrl, string reconnect)
        {
            m_serverUrl = serverUrl;
            m_reconnect = Convert.ToInt32(reconnect);
        }

        public void Start_WebSocket()
        {
            MappedDiagnosticsContext.Set(StringUtility.LOGKEYINFO, StringUtility.WEBSOCKETLOG);
            txtLogger.Info("Start to connect to the web server at [{0}]", m_serverUrl);
            try
            {
                bool loc_ip_re = GetLocalIP(ref m_localIp);

                if (!loc_ip_re)
                {
                    txtLogger.Error("Get local IP ERROR");
                    return;
                }

                m_webSsocket = new WebSocket(m_serverUrl);

                if (null == m_webSsocket)
                {
                    txtLogger.Error("New web socket fail ");
                    return;
                }

                m_webSsocket.WaitTime = TimeSpan.FromSeconds(m_reconnect);
            }
            catch (Exception ex)
            {
                txtLogger.Error("New web socket exception: {0}", ex.Message);
                return;
            }

            try
            { 
                m_webSsocket.EmitOnPing = true;  // 是否接收ping标志
                m_webSsocket.OnMessage += (sender, e) =>
                {
                    if (!e.IsPing)
                    {
                        MappedDiagnosticsContext.Set(StringUtility.LOGKEYINFO, StringUtility.WEBSOCKETLOG);
                        // if is not ping message, handle the message
                        try
                        {
                            if (e.IsText)
                            {
                                string writeBack = string.Empty;
                                txtLogger.Info(">>>>>>>>>> Receive message from rfid server: {0}", e.Data);

                                JObject jobj = (JObject)JsonConvert.DeserializeObject(e.Data);
                                if (jobj == null)
                                {
                                    txtLogger.Error("Convert the string to json fail: {0}", e.Data);

                                    m_webSsocket.Send("{\"result\":\"convert to json error\"}");
                                }
                                else
                                {
                                    //每一次收到数据都新申请对象，确保没有以前遗留数据
                                    ReaderParameter readerPara = new ReaderParameter();
                                    CommFunc commFunc = new CommFunc();

                                    //解析json数据到ReaderParameter对象
                                    commFunc.ParseJsonObj(jobj, ref readerPara);

                                    // TODO: 读写器操作
                                    writeBack = "盘点结果.......";
                                    m_webSsocket.Send(writeBack);
                                }
                            }
                            else
                            {
                                txtLogger.Error("Receive other type message: {0}", e.ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            txtLogger.Error("Receive message handle execption: {0}", ex.Message);
                        }
                    }
                    // else {going to add}
                };

                m_webSsocket.OnOpen += (sender, e) =>
                {
                    MappedDiagnosticsContext.Set(StringUtility.LOGKEYINFO, StringUtility.WEBSOCKETLOG);
                    txtLogger.Info("Connect to the server success");
                    //if need to add register info?

                    m_webSsocket.Send(m_localIp);
                    txtLogger.Info("Send register info, {0}", m_localIp);
                };

                m_webSsocket.OnClose += (sender, e) =>
                {
                    MappedDiagnosticsContext.Set(StringUtility.LOGKEYINFO, StringUtility.WEBSOCKETLOG);
                    txtLogger.Info("The websocket connect is closed, {0}", e.Reason);
                };

                m_webSsocket.OnError += (sender, e) =>
                {
                    MappedDiagnosticsContext.Set(StringUtility.LOGKEYINFO, StringUtility.WEBSOCKETLOG);
                    txtLogger.Error("The websocket have an error={0}", e.Message);
                };

                m_webSsocket.Connect();
            }
            catch (Exception ex)
            {
                txtLogger.Error("WebSocket handle exception = {0}", ex.Message);
                m_webSsocket.Close();
            }
            // 断线重连
            while (true)
            {
                if (!m_webSsocket.Ping())
                {
                    //if ping is not on, reconnect
                    try
                    {
                        m_webSsocket.Close();
                        MappedDiagnosticsContext.Set(StringUtility.LOGKEYINFO, StringUtility.WEBRECONNECT);
                        txtLogger.Info("Start to reconnect to the web server at [{0}],[{1}]", m_serverUrl, m_webSsocket.ReadyState);
                        m_webSsocket.Connect();
                    }
                    catch (Exception ex)
                    {
                        MappedDiagnosticsContext.Set(StringUtility.LOGKEYINFO, StringUtility.WEBRECONNECT);
                        txtLogger.Error("Reconnect to the web server exception: [{0}]", ex.Message);
                        m_webSsocket.Close();
                    }
                }
                // if ping is doing
                // m_webSsocket.Send(m_localIp);
                Thread.Sleep(m_reconnect * 1000);
            }
        }

        public bool GetLocalIP(ref string ipstr)
        {
            try
            {
                string bestIp = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault<IPAddress>(a => a.AddressFamily.ToString().Equals("InterNetwork")).ToString();

                JObject ip_obj = new JObject();
                ip_obj[StringUtility.MSG_TRANSID] = StringUtility.REGISTER;

                JArray jb_array = new JArray();
                jb_array.Add(bestIp);

                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface adapter in nics)
                {
                    if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    {
                        var mac = adapter.GetPhysicalAddress(); Console.WriteLine(mac);
                        IPInterfaceProperties ip = adapter.GetIPProperties();
                        UnicastIPAddressInformationCollection ipCollection = ip.UnicastAddresses;
                        foreach (UnicastIPAddressInformation ipadd in ipCollection)
                        {
                            //InterNetwork    IPV4地址      
                            //InterNetworkV6        IPV6地址 
                            if (ipadd.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                if (bestIp.Equals(ipadd.Address.ToString()))
                                {
                                    continue;
                                }
                                jb_array.Add(ipadd.Address.ToString());
                            }
                        }
                    }
                }
                
                ip_obj[StringUtility.LOCALIP] = jb_array;
                ipstr =  Newtonsoft.Json.JsonConvert.SerializeObject(ip_obj);
                return true;
            }
            catch (Exception ex)
            {
                txtLogger.Error("Get local ip exception fail, {0}", ex.Message);

                return false;
            }
        }
    }
}
