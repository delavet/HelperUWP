using HelperUWP.LFref;
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
    public sealed partial class LFmasterPage : Page
    {
        private LostFoundInfoList infos;
        public LFmasterPage()
        {
            this.InitializeComponent();
            infos = new LostFoundInfoList();
            infos.DataLoading += DataLoading;
            infos.DataLoaded += DataLoaded;
            GRDVWLF.ItemsSource = infos;
            CMBBXLFType.SelectedIndex = 0;
            CMBBXLFType2.SelectedIndex = 0;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            STRBDpopin.Begin();
        }
        private void DataLoading()
        {
            PRGRSLF.ProgressStart();
        }
        private void DataLoaded()
        {
            PRGRSLF.ProgressEnd();
        }

        private void BTNsend_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(LFsendPage));
        }

        private void CMBBXLFType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (CMBBXLFType.SelectedIndex == CMBBXLFType2.SelectedIndex) return;
                
            }
            catch
            {

            }
            ComboBox temp = sender as ComboBox;
            switch (temp.SelectedIndex)
            {
                case 0:
                    {
                        infos.Type = LFstatus.Lost;
                    }break;
                case 1:
                    {
                        infos.Type = LFstatus.Found;
                    }break;
                case 2:
                    {
                        infos.Type = LFstatus.My;
                    }break;
            }
            if (sender == CMBBXLFType) CMBBXLFType2.SelectedIndex = CMBBXLFType.SelectedIndex;
            else CMBBXLFType.SelectedIndex = CMBBXLFType2.SelectedIndex;
        }

        private void GRDVWLF_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(LFDetailPage), e.ClickedItem);
        }

        private void GRIDLFRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double w = GRIDLFRoot.ActualWidth;
            w -= 20;
            ItemsWrapGrid wrap = Util.GetItemsWrapGrid(GRDVWLF);
            try
            {
                if (w < 650) wrap.ItemWidth = w;
                else if (w < 850) wrap.ItemWidth = w / 2;
                else if (w < 1150) wrap.ItemWidth = w / 3;
                else if (w < 1600) wrap.ItemWidth = w / 4;
                else wrap.ItemWidth = w / 5;
            }
            catch
            {

            }
        }
    }
}
