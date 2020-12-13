using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using nsAlienRFID2;

namespace TalentRFIDClient
{
    class AlienReader
    {
        private clsReader mReader;
        private ReaderInfo mReaderInfo;
        private ComInterface meReaderInterface = ComInterface.enumTCPIP;
		private string m_reader_ip;
		private int m_reader_port;

		private NLog.Logger txtLogger = LogManager.GetLogger(StringUtility.LOGMODELNAME);

		public AlienReader(string reader_ip, int reader_port)
        {
			m_reader_ip = reader_ip;
			m_reader_port = reader_port;

			mReader = new clsReader(true);
			mReaderInfo = mReader.ReaderSettings;
		}

		public void connect_reader(object sender, System.EventArgs e)
		{
			String result;

			try
			{
				if (meReaderInterface == ComInterface.enumTCPIP)
					mReader.InitOnNetwork("127.0.0.1", 22);
				else
					mReader.InitOnCom();

				txtLogger.Info("Start connect to reader: {}:{}", m_reader_ip, m_reader_port);
				result = mReader.Connect();
				if (mReader.IsConnected)
				{
					if (meReaderInterface == ComInterface.enumTCPIP)
					{
						if (!mReader.Login("alien", "password"))
						{
							txtLogger.Error("connect to reader: {}:{} failed with alien and password", m_reader_ip, m_reader_port);
							mReader.Disconnect();
							return;
						}
					}

					txtLogger.Info("connect to reader: {}:{}", m_reader_ip, m_reader_port);

					// to make it faster and not to lose any tag
					mReader.AutoMode = "On";

					// TODO: 其他参数设置
				}
			}
			catch (Exception ex)
			{
				txtLogger.Error("Exception in btnConnect_Click(): " + ex.Message);
			}
		}

		public String getTagList()
        {
			mReader.TagListCustomFormat = "%i, %a, %D %T, %p, %l";
			string result = null;
			try
			{
				result = mReader.TagList;
			}
			catch (Exception ex)
			{
				txtLogger.Error("Exception in btnConnect_Click(): " + ex.Message);
				return "";
			}

			if ((result != null) &&
				(result.Length > 0) &&
				(result.IndexOf("No Tags") == -1))
				return result;
			else
				return "";
		}
	}
}
