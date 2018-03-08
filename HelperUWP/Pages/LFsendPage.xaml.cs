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
using Windows.Storage;
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
    public sealed partial class LFsendPage : Page
    {
        private BitmapImage bitmap = null;
        StorageFile file = null;
        public LFsendPage()
        {
            this.InitializeComponent();
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            STRBDpopin.Begin();
            Constants.BoxPage.HamburgerVisible = false;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            Constants.BoxPage.HamburgerVisible = true;
        }

        private void BTNback_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
                this.Frame.GoBack();
        }

        private async void BTNsend_Click(object sender, RoutedEventArgs e)
        {
            PRGRSLFsend.ProgressStart();
            String name = name_box.Text;
            if ("".Equals(name))
            {
                Constants.BoxPage.ShowMessage("物品名字不能为空");
                return;
            }
            String phone = phone_box.Text;
            if ("".Equals(phone))
            {
                Constants.BoxPage.ShowMessage("电话号码不能为空");
                return;
            }
            String detail = detail_box.Text;
            String type = "other";
            switch (type_box.SelectedIndex)
            {
                case 0:
                    {
                        type = "card";
                    }
                    break;
                case 1:
                    {
                        type = "book";
                    }
                    break;
                case 2:
                    {
                        type = "device";
                    }
                    break;
                case 3:
                    {
                        type = "other";
                    }
                    break;
                default: break;
            }
            String lost_or_found = "lost";
            switch (lost_or_found_box.SelectedIndex)
            {
                case 0:
                    {
                        lost_or_found = "lost";
                    }
                    break;
                case 1:
                    {
                        lost_or_found = "found";
                    }
                    break;
                default: break;
            }
            String action_time_str = ((Util.GetTimeStamp(action_date_chooser.Date.Date) + action_time_chooser.Time.Milliseconds)/1000).ToString();
            List<Parameters> param = new List<Parameters>();
            param.Add(new Parameters("name", name));
            param.Add(new Parameters("type", type));
            param.Add(new Parameters("detail", detail));
            param.Add(new Parameters("action_time", action_time_str));
            param.Add(new Parameters("poster_phone", phone));
            param.Add(new Parameters("lost_or_found", lost_or_found));
            param.Add(new Parameters("token", Constants.token));
            String img_str = null;
            if (file != null)
            {
                var a = await file.OpenAsync(FileAccessMode.ReadWrite);
                Stream stream = a.AsStream();
                byte[] bts = Util.StreamToBytes(stream);
                img_str = Convert.ToBase64String(bts);
                param.Add(new Parameters("imgData", img_str));
            }
            Parameters result = await WebConnection.connect(Constants.domain + "/services/LFpost.php", param);
            PRGRSLFsend.ProgressEnd();
            try
            {
                if (result.name != "200")
                {
                    Util.DealWithDisconnect(result);
                    return;
                }
                JsonObject jsonObject = JsonObject.Parse(result.value);
                int success = (int)jsonObject.GetNamedNumber("success");
                if (success == 0)
                {
                    Constants.BoxPage.ShowMessage(jsonObject.GetNamedString("reason", "nothing to know"));
                    return;
                }
                return;
            }
            catch (Exception)
            {
                return;
            }
        }

        private async void choose_img_btn_Click(object sender, RoutedEventArgs e)
        {
            file = await Util.ChooseFile(".png", ".jpg");

            if (file != null)
            {
                bitmap = new BitmapImage();
                bitmap.SetSource(await file.OpenAsync(FileAccessMode.ReadWrite));
                preview_img.Source = bitmap;
            }else
            {
                Constants.BoxPage.ShowMessage("未能成功选取图片");
            }
        }

        private void delete_img_btn_Click(object sender, RoutedEventArgs e)
        {
            file = null;
            bitmap = null;
            preview_img.Source = null;
        }
    }
}
