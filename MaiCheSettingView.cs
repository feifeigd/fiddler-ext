﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using fiddler_ext.Scripts;

using Fiddler;

// 这个类，不能有namespace
class MaiCheSettingView : IFiddlerExtension
{
    public void OnBeforeUnload()
    {
        //throw new NotImplementedException();
    }

    public void OnLoad()
    {
        var view = new TestView();
        view.Dock = DockStyle.Fill;
        var tab = new TabPage();
        tab.Text = "相关设置";
        tab.Controls.Add(view);
        FiddlerApplication.UI.tabsViews.TabPages.Add(tab);
    }
}
