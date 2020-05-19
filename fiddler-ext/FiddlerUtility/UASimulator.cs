using Fiddler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fiddler_ext.FiddlerUtility
{
    public class UASimulator
    {
        string m_sUAString;
        public UASimulator(string uaString)
        {
            m_sUAString = uaString;
        }

        // 修改请求的 User-Agent
        public bool OverwriteUA(Session oSession)
        {
            oSession.oRequest["User-Agent"] = m_sUAString;
            return true;
        }
    }
}
