using HelperUWP.IPGWref;
using HelperUWP.Lib;
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
    public sealed partial class IPGW2Page : Page
    {
        private IPGW_type ConnectType = IPGW_type.None;
        private IPGWstatus PageStatus = IPGWstatus.Nothing;
        public IPGW2Page()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            STRBDpopin.Begin();
        }

        private void IPGWmain_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            if (!(PageStatus == IPGWstatus.Beginning)) return;
            PageStatus = IPGWstatus.Multing;
        }

        private void BDred_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (!(PageStatus == IPGWstatus.Multing)) return;
            PageStatus = IPGWstatus.Choosing;
            ConnectType = IPGW_type.ConnectNofree;
            TXTBLKred.Text = "连接收费";
            STRBDredInc.Begin();
        }

        private void BDred_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (!(PageStatus == IPGWstatus.Choosing)) return;
            PageStatus = IPGWstatus.Multing;
            ConnectType = IPGW_type.None;
            TXTBLKred.Text = "";
            STRBDredDec.Begin();
        }

        
        private async void GRIDipgwMain_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (!((PageStatus == IPGWstatus.Choosing) || (PageStatus == IPGWstatus.Multing))) return;
            IPGWmain.Opacity = 0.5;
            if (!(PageStatus == IPGWstatus.Choosing))
            {
                PageStatus = IPGWstatus.Nothing;
                STRBDconnectEnd.Begin();
                return;
            }
            PageStatus = IPGWstatus.Connecting;
            STRBDconnecting.Begin();
            TXTBLKblue.Text = "";
            TXTBLKgreen.Text = "";
            TXTBLKred.Text = "";
            TXTBLKyellow.Text = "";
            IPGWMsg msg = await IPGWUtil.DoConnection(ConnectType);
            try
            {
                Constants.BoxPage.ShowMessage(msg.Content, msg.Title);
            }
            catch
            {
                Constants.BoxPage.ShowMessage("连接网关失败！");
            }
            PageStatus = IPGWstatus.Nothing;
            STRBDconnecting.Stop();
            STRBDconnectEnd.Begin();
        }

        private void BDgreen_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (!(PageStatus == IPGWstatus.Multing)) return;
            PageStatus = IPGWstatus.Choosing;
            ConnectType = IPGW_type.Disconnect;
            TXTBLKgreen.Text = "断开连接";
            STRBDgreenInc.Begin();
        }

        private void BDgreen_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (!(PageStatus == IPGWstatus.Choosing)) return;
            PageStatus = IPGWstatus.Multing;
            ConnectType = IPGW_type.None;
            TXTBLKgreen.Text = "";
            STRBDgreenDec.Begin();
        }

        private void BDblue_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (!(PageStatus == IPGWstatus.Multing)) return;
            PageStatus = IPGWstatus.Choosing;
            ConnectType = IPGW_type.ConnectFree;
            TXTBLKblue.Text = "连接免费";
            STRBDblueInc.Begin();
        }

        private void BDblue_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (!(PageStatus == IPGWstatus.Choosing)) return;
            PageStatus = IPGWstatus.Multing;
            ConnectType = IPGW_type.None;
            TXTBLKblue.Text = "";
            STRBDblueDec.Begin();
        }

        private void IPGWmain_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            PageStatus = IPGWstatus.Beginning;
            IPGWmain.Opacity = 1;
            STRBDlinkShow.Begin();
        }

        private void IPGWmain_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (!(PageStatus == IPGWstatus.Beginning)) return;
            PageStatus = IPGWstatus.Nothing;
            IPGWmain.Opacity = 0.5;
            STRBDconnectEnd.Begin();
        }

        private void BDyellow_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (!(PageStatus == IPGWstatus.Multing)) return;
            PageStatus = IPGWstatus.Choosing;
            ConnectType = IPGW_type.DisconnectAll;
            TXTBLKyellow.Text = "断开所有连接";
            STRBDyellowInc.Begin();
        }

        private void BDyellow_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (!(PageStatus == IPGWstatus.Choosing)) return;
            PageStatus = IPGWstatus.Multing;
            ConnectType = IPGW_type.None;
            TXTBLKyellow.Text = "";
            STRBDyellowDec.Begin();
        }
    }
}
