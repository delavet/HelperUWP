using HelperUWP.CourseRef;
using HelperUWP.Lib;
using HelperUWP.Lib.Storage;
using HelperUWP.Lib.Web;
using HelperUWP.Pages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace HelperUWP
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// 作为整个app的UI的大容器之用
    /// 提供：汉堡菜单和基础的信息通知控件
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // SplitView 控件模板中，Pane部分的 Grid
        private Grid PaneRoot;
        private Boolean hamVis = true;
        public Boolean HamburgerVisible
        {
            get
            {
                return hamVis;
            }
            set
            {
                hamVis = value;
                if (value)
                {
                    CHKBXhamburger.Visibility = Visibility.Visible;
                }
                else
                {
                    if(visualState.CurrentState==narrow)
                        CHKBXhamburger.Visibility = Visibility.Collapsed;
                }
            }
        }

        //  引用 SplitView 控件中， 保存从 Pane “关闭” 到“打开”的 VisualTransition
        //  也就是 <VisualTransition From="Closed" To="OpenOverlayLeft"> 这个 
        private VisualTransition from_ClosedToOpenOverlayLeft_Transition;

        private WriteableBitmap Bg = null, BluredBg = null;

        public MainPage()
        {
            this.InitializeComponent();
            Constants.BoxPage = this;
            FRAMEcontent.CacheSize = 4;
            TXTBLKuserFirstName.Text = Constants.name.ElementAt(0).ToString();
            TXTBLKuserName.Text = Constants.name;
            TXTBLKid.Text = Constants.username;
            //LoadBg();
            //LoadDefaultImg();          
        }

        /*private async void LoadDefaultImg()
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/DefaultImg.png"));
            Constants.DefaultImg.SetSource(await file.OpenReadAsync());
        }*/

        public void SetBg(WriteableBitmap bg)
        {
            /*Bg = bg;
            BRSHbg.ImageSource = Bg;
            GRIDroot.Background = BRSHbg;*/
        }

        public void SetBluredBg(WriteableBitmap bbg)
        {
            //BluredBg = bbg;
            //BRSHbluredBg.ImageSource = BluredBg;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {   //透明化手机状态栏
                var statusBar = StatusBar.GetForCurrentView();
                statusBar.BackgroundOpacity = 0;
                statusBar.ForegroundColor = Colors.Black;
                this.Frame.Margin = new Thickness(0, -1, 0, 0);
                Constants.IsMobile = true;
                TXTBLKtitle.Visibility = Visibility.Collapsed;
                GRIDtitle.Height = Constants.ContentMargin;
                
            }
            else
            {   //透明化电脑端标题栏
                var coreTitleBar = Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().TitleBar;
                var appTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;             
                coreTitleBar.ExtendViewIntoTitleBar = true;
                appTitleBar.ButtonBackgroundColor = Colors.Transparent;
                Constants.IsMobile = false;
                Window.Current.SetTitleBar(GRIDtitle);
                Constants.ContentMargin = GRIDtitle.Height;
            }
            HEIGHTtitle.Height = new GridLength(Constants.ContentMargin);
            FRAMEcontent.Margin = new Thickness(0, Constants.ContentMargin, 0, 0);
            CHKBXhamburger.Margin = new Thickness(0, Constants.ContentMargin, 0, 0);
            //GRIDblur.Opacity = 0;
            FRAMEcontent.Navigate(typeof(IPGW2Page));
            //STRBDbgChangeBack.Begin();
            GetWeekInfo();
            
        }

        private async void GetWeekInfo()
        {
            try
            {
                Parameters result = await WebConnection.Connect(Constants.domain + "/pkuhelper/../services/info.php?device=iPhone6%2C2&platform=iOS&sysver=9.3.4&uid=" + Constants.username + "&version=2.1.1", null);
                JsonObject resJson = JsonObject.Parse(result.value);
                int week = (int)resJson.GetNamedNumber("week");
                Constants.Week = week;
                CourseUtil.InitCourses();
            }
            catch
            {

            }
        }

        /*private async void LoadBg()
        {
            try
            {
                StorageFile tempFile = await Cache.getCacheFile("CachedBg.png");
                if (tempFile != null)
                {
                    using(IRandomAccessStream streamIn=await tempFile.OpenAsync(FileAccessMode.Read))
                    {
                        Bg = new WriteableBitmap(1920, 1080);
                        Bg.SetSource(streamIn);
                    }
                    
                }
                else
                {
                    Bg = new WriteableBitmap(1920, 1080);
                    var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/DefaultBg.png"));
                    Bg.SetSource(await file.OpenAsync(FileAccessMode.Read));
                }
            }
            catch(Exception e)
            {
                Bg = new WriteableBitmap(1920, 1080);
                var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/DefaultBg.png"));
                Bg.SetSource(await file.OpenAsync(FileAccessMode.Read));
                Debug.WriteLine(e.StackTrace + "\r\n");
            }
            finally
            {
                BRSHbg.ImageSource = Bg;
                GRIDroot.Background = BRSHbg;
            }
            try
            {
                StorageFile tempFile = await Cache.getCacheFile("CachedBluredBg.png");
                using (IRandomAccessStream streamIn2 = await tempFile.OpenAsync(FileAccessMode.Read))
                {
                    BluredBg = new WriteableBitmap(1920, 1080);
                    BluredBg.SetSource(streamIn2);
                }
            }
            catch(Exception e)
            {
                try
                {
                    BluredBg = await Util.Blur(Bg,Constants.BgBluredLevel);
                    await Cache.savePngFile(BluredBg, "CachedBluredBg.png");
                }
                catch
                {
                    BluredBg = new WriteableBitmap(1920, 1080);
                    Debug.WriteLine(e.StackTrace + "\r\n");
                }               
            }
            finally
            {
                BRSHbluredBg.ImageSource = BluredBg;
            }
        }*/
        
        private void LSTBXmenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (LSTBXmenu.SelectedIndex)
            {
                case 0:
                    {
                        //if (FRAMEcontent.Content is IPGW2Page) STRBDbgChange.Begin();
                        FRAMEcontent.Navigate(typeof(UserInfoPage));
                    }
                    break;
                case 1:
                    {
                        //STRBDbgChangeBack.Begin();
                        FRAMEcontent.Navigate(typeof(IPGW2Page));
                    }break;
                case 2:
                    {
                        //if(FRAMEcontent.Content is IPGW2Page) STRBDbgChange.Begin();
                        FRAMEcontent.Navigate(typeof(CoursePage));
                    }break;
                case 3:
                    {
                        //if (FRAMEcontent.Content is IPGW2Page) STRBDbgChange.Begin();
                        FRAMEcontent.Navigate(typeof(InfoPage));
                    }
                    break;
                case 4:
                    {
                        //if (FRAMEcontent.Content is IPGW2Page) STRBDbgChange.Begin();
                        FRAMEcontent.Navigate(typeof(NCPage));
                    }
                    break;
                case 5:
                    {
                        //if (FRAMEcontent.Content is IPGW2Page) STRBDbgChange.Begin();
                        FRAMEcontent.Navigate(typeof(MailPage));
                    }
                    break;
                case 6:
                    {
                        //if (FRAMEcontent.Content is IPGW2Page) STRBDbgChange.Begin();
                        FRAMEcontent.Navigate(typeof(ChatPage));
                    }
                    break;
                case 7:
                    {
                        //if (FRAMEcontent.Content is IPGW2Page) STRBDbgChange.Begin();
                        FRAMEcontent.Navigate(typeof(PkuHolePage));
                    }break;
                case 8:
                    {
                        //if (FRAMEcontent.Content is IPGW2Page) STRBDbgChange.Begin();
                        FRAMEcontent.Navigate(typeof(LFmasterPage));
                    }break;
                case 9:
                    {
                        //if (FRAMEcontent.Content is IPGW2Page) STRBDbgChange.Begin();
                        FRAMEcontent.Navigate(typeof(SettingPage));
                    }
                    break;
                default:break;
            }
        }

        private void BDswip_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            e.Handled = true;

            // 仅当 SplitView 处于 Overlay 模式时（窗口宽度最小时）
            if (SPVmenu.DisplayMode == SplitViewDisplayMode.Overlay)
            {
                if (PaneRoot == null)
                {
                    // 找到 SplitView 控件中，模板的父容器
                    Grid grid = Util.FindVisualChild<Grid>(SPVmenu);

                    PaneRoot = grid.FindName("PaneRoot") as Grid;

                    if (from_ClosedToOpenOverlayLeft_Transition == null)
                    {
                        // 获取 SplitView 模板中“视觉状态集合”
                        IList<VisualStateGroup> stateGroup = VisualStateManager.GetVisualStateGroups(grid);

                        //  获取 VisualTransition 对象的集合。
                        IList<VisualTransition> transitions = stateGroup[0].Transitions;

                        // 找到 SplitView.IsPaneOpen 设置为 true 时，播放的 transition
                        from_ClosedToOpenOverlayLeft_Transition = transitions?.Where(train => train.From == "Closed" && train.To == "OpenOverlayLeft").First();
                       
                    }
                }
                // 默认为 Collapsed，所以先显示它
                PaneRoot.Visibility = Visibility.Visible;

                // 当在 Border 上向右滑动，并且滑动的总距离需要小于 Panel 的默认宽度。否则会脱离左侧窗口，继续向右拖动
                if (e.Cumulative.Translation.X >= 0 && e.Cumulative.Translation.X < SPVmenu.OpenPaneLength)
                {
                    CompositeTransform ct = PaneRoot.RenderTransform as CompositeTransform;
                    ct.TranslateX = (e.Cumulative.Translation.X - SPVmenu.OpenPaneLength);                    
                }
            }
        }

        private void STRBDbgChangeBack_Completed(object sender, object e)
        {
            
        }

        private void BDswip_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            e.Handled = true;

            // 仅当 SplitView 处于 Overlay 模式时（窗口宽度最小时）
            if (SPVmenu.DisplayMode == SplitViewDisplayMode.Overlay && PaneRoot != null)
            {
                // 因为当 IsPaneOpen 为 true 时，会通过 VisualStateManager 把 PaneRoot.Visibility  设置为
                // Visibility.Visible，所以这里把它改为 Visibility.Collapsed，以回到初始状态
                PaneRoot.Visibility = Visibility.Collapsed;

                // 恢复初始状态 
                CompositeTransform ct = PaneRoot.RenderTransform as CompositeTransform;


                // 如果大于 SPVmenu.OpenPaneLength 宽度的 1/2 ，则显示，否则隐藏
                if ((SPVmenu.OpenPaneLength + ct.TranslateX) > SPVmenu.OpenPaneLength / 2)
                {
                    SPVmenu.IsPaneOpen = true;

                    // 因为上面设置 IsPaneOpen = true 会再次播放向右滑动的动画，所以这里使用 SkipToFill()
                    // 方法，直接跳到动画结束状态
                    from_ClosedToOpenOverlayLeft_Transition?.Storyboard?.SkipToFill();
                }
                
                ct.TranslateX = 0;
                
            }
        }

        public void ShowMessage(String content,String title="")
        {
            Canvas.SetZIndex(SHOWmsg, 999);
            SHOWmsg.ShowMsg(content, title);
        }

        
        private void STRBDbgChange_Completed(object sender, object e)
        {
            
        }

        private void visualState_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            CHKBXhamburger.Visibility = Visibility.Visible;
            if (hamVis) return;
            if (e.NewState != narrow) return;
            CHKBXhamburger.Visibility = Visibility.Collapsed;
        }

        private void CHKBXhamburger_Click(object sender, RoutedEventArgs e)
        {
            SPVmenu.IsPaneOpen = !SPVmenu.IsPaneOpen;
            if (SPVmenu.DisplayMode != SplitViewDisplayMode.CompactInline) return;
            if (SPVmenu.IsPaneOpen)
            {
                STKPNuserLarge.Visibility = Visibility.Visible;
                STKPNuserSmall.Visibility = Visibility.Collapsed;
            }
            else
            {
                STKPNuserLarge.Visibility = Visibility.Collapsed;
                STKPNuserSmall.Visibility = Visibility.Visible;
            }
        }

        private void SPVmenu_PaneClosed(SplitView sender, object args)
        {

        }

        public void ShowMessagePopin(String content, String title = "")
        {
            Canvas.SetZIndex(SHOWmsg, 999);
            SHOWmsg.ShowMsgPopin(content, title);
        }
    }
}
