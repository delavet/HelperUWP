using HelperUWP.CourseRef;
using HelperUWP.HoleRef;
using HelperUWP.Lib;
using HelperUWP.Lib.Web;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public sealed partial class HoleCommentPage : Page
    {
        private HoleInfo CurHoleInfo = null;
        private Boolean Attention = false;
        private ObservableCollection<CommentInfo> CommentInfos = null;
        public HoleCommentPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            STRBDpopin.Begin();
            Constants.BoxPage.HamburgerVisible = false;
            if (e.Parameter != null && e.Parameter is HoleInfo)
            {
                CurHoleInfo = e.Parameter as HoleInfo;
                content_panel.DataContext = CurHoleInfo;
            }
            if (!(CurHoleInfo.type == HoleInfo.TYPE_IMAGE)) IMGcontent.Visibility = Visibility.Collapsed;
            if (!(CurHoleInfo.type == HoleInfo.TYPE_AUDIO)) MEvoice.Visibility = Visibility.Collapsed;
            else MEvoice.AudioSource = new Uri(CurHoleInfo.url, UriKind.Absolute);
            
            GetComments();
        }

        private async void GetComments()
        {
            PRGRScomment.ProgressStart();
            Parameters result = await WebConnection.Connect(Constants.domain + "/services/pkuhole/api.php?action=getcomment&pid=" + CurHoleInfo.pid + "&token=" + Constants.token + "?="+DateTime.Now.Millisecond, null);
            if (result.name != "200")
            {
                Util.DealWithDisconnect(result);
                PRGRScomment.ProgressEnd();
                return;
            }
            else
            {
                handle_json(result.value);
                PRGRScomment.ProgressEnd();
                return;
            }
        }

        private void handle_json(String json_str)
        {
            JsonObject jsonObject = JsonObject.Parse(json_str);
            int code = (int)jsonObject.GetNamedNumber("code");
            if (code != 0)
            {
                Constants.BoxPage.ShowMessage("无效的返回！");
                return;
            }
            else
            {
                int at = (int)jsonObject.GetNamedNumber("attention");
                if (at != 0) Attention = true;
                if (Attention)
                {
                    BTNattention.IsChecked = true;
                }
                else
                {
                    BTNattention.IsChecked = false;
                }
                JsonArray data;
                try
                {
                    data = jsonObject.GetNamedArray("data");
                }
                catch
                {
                    return;
                }
                if (CommentInfos == null)
                {
                    CommentInfos = new ObservableCollection<CommentInfo>();
                }else
                {
                    CommentInfos.Clear();
                }
                foreach (var obj in data)
                {
                    try
                    {
                        JsonObject comment = obj.GetObject();
                        int tcid = int.Parse(comment.GetNamedString("cid"));
                        String ttext = comment.GetNamedString("text");
                        Boolean tislz = ((int)comment.GetNamedNumber("islz") != 0) ? true : false;
                        long ttimestamp = long.Parse(comment.GetNamedString("timestamp"));
                        CommentInfos.Add(new CommentInfo(tcid, ttext, tislz, ttimestamp));
                    }
                    catch
                    {

                    }
                }
                LSTVWcomment.ItemsSource = CommentInfos;
                return;
            }
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            Constants.BoxPage.HamburgerVisible = true;
        }

        private void BTNback_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack) this.Frame.GoBack();
        }

        private void LSTVWcomment_ItemClick(object sender, ItemClickEventArgs e)
        {
            String reply_to;
            if ((e.ClickedItem as CommentInfo).Islz) reply_to = "Reply to 洞主：";
            else reply_to = "Reply to #" + (e.ClickedItem as CommentInfo).Cid.ToString() + "：";
            FindName(nameof(RBcomment));
            RBcomment.Show(CurHoleInfo.pid, reply_to);
        }

        private void IMGcontent_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FindName(nameof(IMGshow));
            if (!(CurHoleInfo.type == HoleInfo.TYPE_IMAGE)) return;
            IMGshow.Show(CurHoleInfo.writable_bitmap);
        }

        private void BTNreply_Click(object sender, RoutedEventArgs e)
        {
            FindName(nameof(RBcomment));
            RBcomment.Show(CurHoleInfo.pid);
        }

        private void BTNattention_Click(object sender, RoutedEventArgs e)
        {
            SetAttention();
        }
        private async void SetAttention()
        {
            Attention = !Attention;
            BTNattention.IsChecked = Attention;
            List<Parameters> param = new List<Parameters>();
            param.Add(new Parameters("action", "attention"));
            param.Add(new Parameters("pid", CurHoleInfo.pid.ToString()));
            param.Add(new Parameters("switch", Attention ? "1" : "0"));
            param.Add(new Parameters("token", Constants.token));
            Parameters result = await WebConnection.Connect(Constants.domain + "/pkuhelper/../services/pkuhole/api.php", param);
            if (!result.name.Equals("200"))
            {
                Attention = !Attention;
                BTNattention.IsChecked = Attention;
                Constants.BoxPage.ShowMessage("未能连接到服务器");
                return;
            }
            try
            {
                JsonObject json = JsonObject.Parse(result.value);
                int code = int.Parse(json.GetNamedString("code"));
                if (code != 0)
                {
                    Attention = !Attention;
                    BTNattention.IsChecked = Attention;
                    Constants.BoxPage.ShowMessage("服务器不想理你");
                    return;
                }
            }
            catch
            {

            }
        }
    }
}
