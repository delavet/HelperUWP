using HelperUWP.Lib;
using HelperUWP.Lib.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
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
    public sealed partial class SettingPage : Page
    {
        public SettingPage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            STRBDpopin.Begin();
            SLDradius.Value = Constants.BgBluredLevel;
            if (Constants.CourseUseElective) CMBBXcourse.SelectedIndex = 0;
            else CMBBXcourse.SelectedIndex = 1;
            if (Constants.CourseUseCustom) TGcustom.IsOn = true;
            else TGcustom.IsOn = false;
        }

        private async void BTNsaveRadius_Click(object sender, RoutedEventArgs e)
        {         
            if (SLDradius.Value == Constants.BgBluredLevel) return;
            Constants.BgBluredLevel = (int)SLDradius.Value;
            try
            {
                Editor.putInt(nameof(Constants.BgBluredLevel), Constants.BgBluredLevel);
            }
            catch { }
            WriteableBitmap Bg = new WriteableBitmap(1920, 1080);
            try
            {
                StorageFile tempFile = await Cache.getCacheFile("CachedBg.png");
                if (tempFile != null)
                {
                    using (IRandomAccessStream streamIn = await tempFile.OpenAsync(FileAccessMode.Read))
                    {
                        Bg.SetSource(streamIn);
                    }
                }
                else
                {
                    var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/DefaultBg.png"));
                    Bg.SetSource(await file.OpenAsync(FileAccessMode.Read));
                }
            }
            catch
            {
                var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/DefaultBg.png"));
                Bg.SetSource(await file.OpenAsync(FileAccessMode.Read));
            }
            finally
            {
                try
                {
                    var blured = await Util.Blur(Bg, Constants.BgBluredLevel);
                    await Cache.savePngFile(blured, "CachedBluredBg.png");
                    Constants.BoxPage.SetBg(Bg);
                    Constants.BoxPage.SetBluredBg(blured);
                }
                catch
                {
                    
                }
            }
        }

        private void CMBBXcourse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CMBBXcourse.SelectedIndex == 0 && Constants.CourseUseElective) return;
            if (CMBBXcourse.SelectedIndex == 1 && !Constants.CourseUseElective) return;
            switch (CMBBXcourse.SelectedIndex)
            {
                case 0:
                    {
                        Editor.putBoolean(nameof(Constants.CourseUseElective), true);
                        Constants.CourseUseElective = true;
                    }break;
                case 1:
                    {
                        Editor.putBoolean(nameof(Constants.CourseUseElective), false);
                        Constants.CourseUseElective = false;
                    }
                    break;
            }
            Constants.BoxPage.ShowMessage("刷新课程表生效");
        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (TGcustom.IsOn == Constants.CourseUseCustom) return;
            Editor.putBoolean(nameof(Constants.CourseUseCustom), TGcustom.IsOn);
            Constants.BoxPage.ShowMessage("刷新课程表生效");
        }

        private void SLDradius_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            //if (SLDradius.Value == Constants.BgBluredLevel) return;
            //Constants.BgBluredLevel = (int)SLDradius.Value;
            //BTNsaveRadius.IsEnabled = true;
        }
    }
}
