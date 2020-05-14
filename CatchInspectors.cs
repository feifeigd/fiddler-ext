using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Fiddler;

namespace CustomInspectors
{
    public class CatchInspectors : Inspector2, IRequestInspector2, IResponseInspector2
    {
        HTTPRequestHeaders _requestHeaders;
        HTTPResponseHeaders _responseHeaders;
        CusUserControl cusUserControl { get; set; }

        byte[] _requestBody;
        bool _bReadOnly;

        HTTPRequestHeaders IRequestInspector2.headers { get => _requestHeaders; set => _requestHeaders = value; }

        public byte[] body { get => _requestBody; set
            {
                _requestBody = value;
                cusUserControl.RTxtBoxBody.Text = Encoding.Default.GetString(value);
            }
        }

        public bool bDirty => true;

        public bool bReadOnly { get => true; set => _bReadOnly = value; }
        HTTPResponseHeaders IResponseInspector2.headers { get => _responseHeaders; set => _responseHeaders = value; }

        public override void AddToTab(TabPage o)
        {
            o.Text = "自定义插件";

            cusUserControl = new CusUserControl();
            cusUserControl.Dock = DockStyle.Fill;
            o.Controls.Add(cusUserControl);
        }

        public void Clear()
        {
            cusUserControl.RTxtBoxBody.Text = string.Empty;
        }

        public override int GetOrder()
        {
            return 100;
        }
    }
}
