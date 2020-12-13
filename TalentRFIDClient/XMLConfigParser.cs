using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using NLog;

namespace TalentRFIDClient
{
    public class XMLConfigParser
    {
        //only need one this instance in all of this application, use singleton model
        private static XMLConfigParser m_xmlParser = new XMLConfigParser();    //init one instance obj

        private XMLConfigParser()
        {    ///private constructor
        }

        public static XMLConfigParser Instance()
        {   
            return m_xmlParser;
        }

        private XmlDocument m_xmlConfig;
        private string m_configName;
        private Logger txtLogger = LogManager.GetLogger(StringUtility.LOGMODELNAME);
        
        public void SetConfigName(string config_name)
        {
            m_configName = config_name;

        }

        //load xml file
        public bool XMLLoad()
        {
            // set the log info as CONFIG            
            MappedDiagnosticsContext.Set(StringUtility.LOGKEYINFO, "CONFIG");
            try
            {
                m_xmlConfig = new XmlDocument();
                m_xmlConfig.Load(System.AppDomain.CurrentDomain.BaseDirectory + m_configName);
                txtLogger.Info("Load config [{0}] success", m_configName);
            }
            catch (Exception ex)
            {
                txtLogger.Error("Load config fail, " + ex.Message);
                return false;
            }

            return true;
        }

        /*********************************************************************
        *函数名称：GetParameter()
        *函数功能：获取单个配置文件参数值
        *参    数:nodeName--xml节点名；paraKey--需要获取值的key；
        *         paraValue--取得的值，传出
        *返 回 值：true--成功， false--失败  
        *创 建 者：Ennis
        *创建日期：2016-11-15
        *修改记录：无  
        **********************************************************************/
        public bool GetParameter(string nodeName, string paraKey, ref string paraValue)
        {
            try
            {
                XmlNodeList xmlNode = m_xmlConfig.GetElementsByTagName(nodeName);
                paraValue = xmlNode[0].Attributes[paraKey].Value;

                txtLogger.Info("Get value=[{0}] by key=[{1}] in nodeList=[{2}]", paraValue, paraKey, nodeName);
            }
            catch (Exception ex)
            {
                txtLogger.Error("Get parameter fail, " + ex.Message);

                return false;
            }
            return true;
        }

        /*********************************************************************
        *函数名称：GetParameter()
        *函数功能：获取多个配置文件参数值
        *参    数:nodeName--xml节点名；paraKeyList--需要获取值的key的list；
        *         paraValueDic--取得的值，与传入的key组成Dictionary格式，传出
        *返 回 值：true--成功， false--失败  
        *创 建 者：Ennis
        *创建日期：2016-11-15
        *修改记录：无  
        **********************************************************************/
        public bool GetParameter(string nodeName, List<string> paraKeyList, ref Dictionary<string, string> paraValueDic)
        {
            //wait add
            return true;
        }
    }
}
