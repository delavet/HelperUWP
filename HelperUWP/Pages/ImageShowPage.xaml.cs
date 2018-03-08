using HelperUWP.InfoRef;
using HelperUWP.Lib.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class ImageShowPage : Page
    {
        private WriteableBitmap bmp;
        private String title;
        public ImageShowPage()
        {
            this.InitializeComponent();
        }
        /// <summary>
        /// 这里e的parameter应该是一个Parameters类
        /// 其name是title的名字，value是要显示的图片的本地路径
        /// </summary>
        /// <param name="e"></param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            STRBDpopin.Begin();
            if (e == null) return;
            if (!(e.Parameter is Parameters)) return;
            var pair = e.Parameter as Parameters;
            title = pair.name;
            String path = pair.value;
            bmp = new WriteableBitmap(1920, 1080);
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(path));
            var file_stream = await file.OpenAsync(FileAccessMode.Read);
            bmp.SetSource(file_stream);
            ISimage.Show(bmp, false);
            TXTBLKimageTitle.Text = title;
        }

        private void BTNsave_Click(object sender, RoutedEventArgs e)
        {
            ISimage.Save();
        }

        private void BTNback_Click(object sender, RoutedEventArgs e)
        {
            InfoUtil.BackRequest();
        }
    }
}
