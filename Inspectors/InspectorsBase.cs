using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Fiddler;

namespace Inspectors
{
    public class InspectorsBase : Inspector2
    {
        CusUserControl cusUserControl { get; set; }

        byte[] _requestBody;
        bool _bReadOnly;


        public byte[] body { get => _requestBody; set
            {
                _requestBody = value;
                cusUserControl.RTxtBoxBody.Text = Encoding.UTF8.GetString(value); 
            }
        }

        public bool bDirty => true;

        public bool bReadOnly { get => true; set => _bReadOnly = value; }

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
