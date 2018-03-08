using HelperUWP.Lib;
using HelperUWP.Lib.Storage;
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
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class UserInfoPage : Page
    {
        public UserInfoPage()
        {
            this.InitializeComponent();
            TXTBLKid.Text = Constants.username;
            TXTBLKname.Text = Constants.name;
            TXTBLKsex.Text = Constants.sex;
            TXTBLKbirthday.Text = Constants.birthday;
            TXTBLKmajor.Text = Constants.major;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            STRBDpopin.Begin();
        }

        private void BTNlogout_Click(object sender, RoutedEventArgs e)
        {
            Editor.clear();
            Frame FRAMEroot = Window.Current.Content as Frame;
            FRAMEroot.Navigate(typeof(LoginPage));
        }
    }
}
