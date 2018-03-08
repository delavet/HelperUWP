using HelperUWP.InfoRef;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace HelperUWP.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。'
    /// 本来是打算只用在info页面上的，结果用在所有的master-detail布局上了
    /// 所以请不要在意这货的Info前缀
    /// </summary>
    public sealed partial class InfoBlankPage : Page
    {
        public InfoBlankPage()
        {
            this.InitializeComponent();
        }
        
    }
}
