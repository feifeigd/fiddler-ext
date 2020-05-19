using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using fiddler_ext.Scripts;

using Fiddler;

namespace Scripts
{
    // Public classes in your assembly that implement the IFiddlerExtension interface will be loaded by Fiddler during startup.
    public class MaiCheSettingView : IFiddlerExtension
    {
        // Called when Fiddler is shutting down
        public void OnBeforeUnload()
        {
            //throw new NotImplementedException();
        }

        // Called when Fiddler User Interface is fully available
        public void OnLoad()
        {
            var view = new TestView();
            view.Dock = DockStyle.Fill;
            var tab = new TabPage();
            tab.Text = "自定义插件";
            tab.Controls.Add(view);
            FiddlerApplication.UI.tabsViews.TabPages.Add(tab);
        }
    }

}