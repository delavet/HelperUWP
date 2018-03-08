using HelperUWP.InfoRef;
using HelperUWP.Lib;
using HelperUWP.Lib.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Calls;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace HelperUWP.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class InfoPage : Page
    {
        private String verifyCode = "";
        private Boolean DLGshowing = false;
        public InfoPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Disabled;
            
            LSTVWinfo.ItemsSource = new List<UsualItemData> {
                new UsualItemData { ImageSource="ms-appx:///Assets/info_map.png",ItemName="北大地图" ,ID=0},
                new UsualItemData { ImageSource="ms-appx:///Assets/info_subway.png",ItemName="北京地铁图",ID=1 },
                new UsualItemData { ImageSource="ms-appx:///Assets/info_card.png",ItemName="查询校园卡余额",ID=2 },
                new UsualItemData { ImageSource="ms-appx:///Assets/cjcx.png",ItemName="成绩查询",ID=3 },
                new UsualItemData {ImageSource="ms-appx:///Assets/info_calendar.png",ItemName="北大校历",ID=4 },
                new UsualItemData {ImageSource="ms-appx:///Assets/info_phone.png",ItemName="常用电话",ID=5 }
            };
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            InfoUtil.infoPage = this;
            STRBDpopin.Begin();
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            InfoUtil.infoPage = null;
        }
        public void InnerBackRequest()
        {
            if (VSGinfo.CurrentState == narrow && COLdetail.Width.Value != 0)
            {
                STRBDdetailShrink.Begin();
            }else if(VSGinfo.CurrentState==wide&& !(FRAMEdetail.Content == null || (FRAMEdetail.Content is InfoBlankPage)))
            {
                STRBDdetailShrink.Begin();
            }
        }

        private void STRBDdetailShrink_Completed(object sender, object e)
        {
            FRAMEdetail.Navigate(typeof(InfoBlankPage));
            TRANSdetail.Y = 0;
            FRAMEdetail.Opacity = 1;
            if (VSGinfo.CurrentState == narrow) UpdateVisualState(narrow, null);
        }

        private void GRIDinfoRoot_Loaded(object sender, RoutedEventArgs e)
        {
            double a = GRIDinfoRoot.ActualWidth;
            if (a < 850) UpdateVisualState(narrow, null);
            else UpdateVisualState(wide, null);
        }

        private void VSGadapt_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            UpdateVisualState(e.NewState, e.OldState);
        }

        private void UpdateVisualState(VisualState newState,VisualState oldState)
        {
            if (newState == null) return;
            if (newState == narrow)
            {
                if(!(FRAMEdetail.Content==null||(FRAMEdetail.Content is InfoBlankPage)))
                {
                    COLmaster.Width = new GridLength(0);
                    COLdetail.Width = new GridLength(1, GridUnitType.Star);
                    Constants.BoxPage.HamburgerVisible = false;
                }
                else
                {
                    COLmaster.Width = new GridLength(1, GridUnitType.Star);
                    COLdetail.Width = new GridLength(0);
                    Constants.BoxPage.HamburgerVisible = true;
                }
            }else
            {
                if (COLdetail.Width.Value > 0) return;
                COLmaster.Width = new GridLength(400);
                COLdetail.Width = new GridLength(1,GridUnitType.Star);
                Constants.BoxPage.HamburgerVisible = true;
            }
            
        }

        private async void LSTVWinfo_ItemClick(object sender, ItemClickEventArgs e)
        {
            switch ((e.ClickedItem as UsualItemData).ID)
            {
                case 0:
                    {
                        FRAMEdetail.Navigate(typeof(ImageShowPage), new Parameters("北大地图", "ms-appx:///Assets/pkumap.jpg"));
                        if (VSGinfo.CurrentState == narrow) UpdateVisualState(narrow, null);
                    }break;
                case 1:
                    {
                        FRAMEdetail.Navigate(typeof(ImageShowPage), new Parameters("北京地铁图", "ms-appx:///Assets/subwaymap.jpg"));
                        if (VSGinfo.CurrentState == narrow) UpdateVisualState(narrow, null);
                    }break;
                case 2:
                    {
                        PRGRSinfo.ProgressStart();
                        await InfoUtil.GetCardAmount();
                        PRGRSinfo.ProgressEnd();
                    }break;
                case 3:
                    {
                 first: BitmapImage bmp = new BitmapImage();
                        Stream stream = await WebConnection.Connect_for_stream("http://dean.pku.edu.cn/student/yanzheng.php?act=init");
                        if (stream == null)
                        {
                            Constants.BoxPage.ShowMessage("获取验证码失败！");
                            return;
                        }
                        var ran_stream = await Util.StreamToRandomAccessStream(stream);
                        bmp.SetSource(ran_stream);

                        IMGverify.Source = bmp;
                        if (DLGshowing) return;
                        ContentDialogResult res = await DLGverify.ShowAsync();
                        if (res == ContentDialogResult.Primary)
                        {
                            String phpsessid = await Dean.get_session_id(verifyCode);
                            if (phpsessid == "") goto first;
                            PRGRSinfo.ProgressStart();
                            Parameters parameters = await WebConnection.Connect(Constants.domain + "/services/pkuhelper/allGrade.php?phpsessid=" + phpsessid, null);
                            if (parameters.name != "200")
                            {
                                Util.DealWithDisconnect(parameters);
                                PRGRSinfo.ProgressEnd();
                            }
                            else
                            {
                                PRGRSinfo.ProgressEnd();
                                FRAMEdetail.Navigate(typeof(GradePage), parameters.value);
                                if (VSGinfo.CurrentState == narrow) UpdateVisualState(narrow, null);
                            }
                        }
                    }break;
                case 4:
                    {
                        FRAMEdetail.Navigate(typeof(SchoolCalendarPage));
                        if (VSGinfo.CurrentState == narrow) UpdateVisualState(narrow, null);
                    }
                    break;
                case 5:
                    {
                        FRAMEdetail.Navigate(typeof(PhoneList));
                        if (VSGinfo.CurrentState == narrow) UpdateVisualState(narrow, null);
                    }
                    break;
            }
        }

        private void DLGverify_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            DLGshowing = false;
            verifyCode = TXTBXinput.Text.Trim();
        }

        private void DLGverify_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            DLGshowing = true;
            TXTBXinput.ClearValue(TextBox.TextProperty);
        }
    }
}
