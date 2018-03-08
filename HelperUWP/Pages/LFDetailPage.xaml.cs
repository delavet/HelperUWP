using HelperUWP.LFref;
using HelperUWP.Lib;
using HelperUWP.Lib.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Json;
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
    public sealed partial class LFDetailPage : Page
    {
        private List<Parameters> details;
        private LostFoundInfo info;
        private bool lost;
        public LFDetailPage()
        {
            this.InitializeComponent();
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            STRBDpopin.Begin();
            Constants.BoxPage.HamburgerVisible = false;
            if (e.Parameter != null && e.Parameter is LostFoundInfo)
            {
                info = e.Parameter as LostFoundInfo;
                if (info.mine)
                {
                    BTNdelete.Visibility = Visibility.Visible;
                }
                else
                {
                    BTNdelete.Visibility = Visibility.Collapsed;
                }
                Show();
            }
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
             Constants.BoxPage.HamburgerVisible = true;
        }

        private void Show()
        {
            if (info == null) return;
            lost = info.lost_or_found;
            IMGthing.Source = info.real_img;
            details = new List<Parameters>();
            details.Add(new Parameters("物品名称", info.name));
            details.Add(new Parameters("详细信息", info.detail));
            details.Add(new Parameters("物品大类", TransThingType(info.type)));
            if (lost == LostFoundInfo.LOST)
                details.Add(new Parameters("丢失时间", Util.TimestampToDateTime(info.action_time).ToString()));
            else details.Add(new Parameters("拾得时间", Util.TimestampToDateTime(info.action_time).ToString()));
            details.Add(new Parameters("发布人", info.poster_name));
            details.Add(new Parameters("发布人学院", info.poster_college));
            details.Add(new Parameters("发布人学号", info.poster_uid));
            details.Add(new Parameters("联系电话", info.poster_phone));
            details.Add(new Parameters("发布时间", Util.TimestampToDateTime(info.post_time).ToString()));
            LSTVWLFdetail.ItemsSource = details;
        }

        String TransThingType(int type)
        {
            switch (type)
            {
                case 3:
                    {
                        return "钱包与卡片";
                    }

                case 4:
                    {
                        return "书籍与笔记本";
                    }
                case 5:
                    {
                        return "电子设备";
                    }
                default: return "其他物品";

            }
        }

        private void BTNback_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
                this.Frame.GoBack();
        }

        private async void BTNdelete_Click(object sender, RoutedEventArgs e)
        {
            List<Parameters> param = new List<Parameters>();
            param.Add(new Parameters("posttime", info.post_time.ToString()));
            param.Add(new Parameters("token", Constants.token));
            param.Add(new Parameters("imageName", info.img_name));
            Parameters result = await WebConnection.Connect(Constants.domain + "/services/LFDelete.php", param);
            if (result.name != "200")
            {
                Util.DealWithDisconnect(result);
            }
            else
            {
                try
                {
                    JsonObject json = JsonObject.Parse(result.value);
                    int success = (int)json.GetNamedNumber("success");
                    if (success == 0)
                    {
                        Constants.BoxPage.ShowMessage(json.GetNamedString("reason", "未知原因"), "删除失败！");
                    }
                    else
                    {
                        Constants.BoxPage.ShowMessage("删除成功！");
                    }
                }
                catch
                {
                    if (result.value == "0") Constants.BoxPage.ShowMessage("删除成功！");
                    return;
                }
            }
        }

        private void IMGthing_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FindName(nameof(IMGSdetail));
            IMGSdetail.Show(info.real_img);
        }
    }
}
