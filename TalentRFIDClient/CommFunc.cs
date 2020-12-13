using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TalentRFIDClient
{
    public class CommFunc
    {
        private Dictionary<int, string> m_ErrorCodeValue;

        public CommFunc()
        {
            m_ErrorCodeValue = new Dictionary<int, string>();

            m_ErrorCodeValue.Add(1, "reader firmware return no tag");
            m_ErrorCodeValue.Add(2, "reader firmware return password error");
            m_ErrorCodeValue.Add(3, "reader firmware return write operate fail");
            m_ErrorCodeValue.Add(5, "reader is not connected");
            m_ErrorCodeValue.Add(6, "write operate do not receive answer from reader");
            m_ErrorCodeValue.Add(7, "lock data length too long");
            m_ErrorCodeValue.Add(8, "write data have other character");
            m_ErrorCodeValue.Add(9, "write operate catch excption");
            m_ErrorCodeValue.Add(10, "before write operate read fail");
        }

        public string GetErrorCodeValue(int errocode)
        {
            if (m_ErrorCodeValue.ContainsKey(errocode))
            {
                return m_ErrorCodeValue[errocode];
            }
            else
            {
                return "not define errorcode";
            }
        }

        public string ReturnJsonStr(ReaderParameter readerPara, int eventno)
        {
            try
            {
                JObject return_jobj = new JObject();
                return_jobj[StringUtility.MSG_TRANSID] = readerPara.transid;
                
                // TODO: 生成相应消息

                string retrun_str = JsonConvert.SerializeObject(return_jobj);
                return retrun_str;
            }
            catch (Exception ex)
            {
                return "{\"result\":\"json exception error, " + ex.Message + "\"}";
            }
        }

        //从json对象中取出对应的值，存到ReaderParameter的对象中
        public void ParseJsonObj(JObject jobj, ref ReaderParameter readerPara)
        {
            if (jobj.Property(StringUtility.MSG_TRANSID) != null)
            {
                readerPara.transid = jobj[StringUtility.MSG_TRANSID].ToString();
            }

            if (jobj.Property(StringUtility.MSG_CASE_ID) != null)
            {
                readerPara.case_id = jobj[StringUtility.MSG_CASE_ID].ToString();
            }
        }
    }
}
