using HelperUWP.HoleRef;
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
    public sealed partial class PkuHolePage : Page
    {
        private Boolean useAttention = false;
        private HoleInfoList holeInfoList = null;
        public PkuHolePage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            holeInfoList = new HoleInfoList();
            holeInfoList.DataLoaded += HoleLoaded;
            holeInfoList.DataLoading += HoleLoading;
            LSTVWhole.ItemsSource = holeInfoList;
                
            soy();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            STRBDpopin.Begin();
            
        }
        private void JudgeWaterCol()
        {
            double w = GRIDholeRoot.ActualWidth;
            if (w < 600||Constants.IsMobile) HoleInfoList.WaterCol = 1;
            else if (w < 850) HoleInfoList.WaterCol = 2;
            else if (w < 1200) HoleInfoList.WaterCol = 3;
            else if (w < 1600) HoleInfoList.WaterCol = 4;
            else HoleInfoList.WaterCol = 5;
        }

        private async void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (SCRVWlist == null) return;
            if (SCRVWlist.VerticalOffset >= SCRVWlist.ScrollableHeight)
            {
                await holeInfoList.LoadMoreItemsAsync();
            }
        }

        private async void soy()
        {
            await holeInfoList.LoadMoreItemsAsync();
        }
        private void HoleLoading()
        {
            BTNloadMore.IsEnabled = false;
            PRGRShole.ProgressStart();
        }
        private void HoleLoaded()
        {
            PRGRShole.ProgressEnd();
            BTNloadMore.IsEnabled = true;            
        }

        private void LSTVWhole_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(HoleCommentPage), e.ClickedItem);
        }

        private void PRbox_RefreshInvoked(DependencyObject sender, object args)
        {
            refresh();
        }
        private void refresh()
        {
            PRGRShole.ProgressStart();
            holeInfoList.DoFresh();
        }

        private void LSTVWhole_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void VShole_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            VisualState s = e.NewState;
            if (s == narrown) HoleInfoList.WaterCol = 1;
            else if (s == superNarrow) HoleInfoList.WaterCol = 2;
            else if (s == widen) HoleInfoList.WaterCol = 3;
            else if (s == superWide) HoleInfoList.WaterCol = 4;
            else if (s == ultraWide) HoleInfoList.WaterCol = 5;
        }

        private void GRIDholeRoot_Loaded(object sender, RoutedEventArgs e)
        {
            JudgeWaterCol();
        }

        private async void BTNloadMore_Click(object sender, RoutedEventArgs e)
        {
            await holeInfoList.LoadMoreItemsAsync();
        }

        private void BTNwriteHole_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(HolePostPage));
        }

        private void BTNrefresh_Click(object sender, RoutedEventArgs e)
        {
            refresh();
        }

        private void BTNattention_Click(object sender, RoutedEventArgs e)
        {
            useAttention = !useAttention;
            if (useAttention) holeInfoList.SetHoleType(HoleListType.attention);
            else holeInfoList.SetHoleType(HoleListType.normal);
        }

        private void BTNsearch_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
