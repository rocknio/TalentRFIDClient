using System;
using System.Collections.Generic;
using System.Text;

namespace TalentRFIDClient
{
    public class ReaderParameter
    {
        public ReaderParameter()
        { }

        public string transid = string.Empty;

        public bool ReaderConStatus = false;

        public string ErrMsg = string.Empty;

        public string tid = string.Empty;

        public string epc = string.Empty;

        public string case_id = string.Empty;
    }
}
