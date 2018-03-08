using HelperUWP.HoleRef;
using HelperUWP.Lib;
using HelperUWP.Lib.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Data.Json;
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
    public sealed partial class HolePostPage : Page
    {
        private bool recording = false;
        private object o = new object();
        private int HoleType = HoleInfo.TYPE_TEXT;
        private BitmapImage bitmap = null;
        private AudioRecorder recorder = null;
        private int Length = 0;
        IRandomAccessStream audioStream = null;
        StorageFile file = null;
        public HolePostPage()
        {
            this.InitializeComponent();
            bitmap = new BitmapImage();
            CMBBXpostType.SelectedIndex = 0;
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

        private void CMBBXpostType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (CMBBXpostType.SelectedIndex)
            {
                case 0:
                    {
                        HoleType = HoleInfo.TYPE_TEXT;
                        STKPNaudio.Visibility = Visibility.Collapsed;
                        STKPNimage.Visibility = Visibility.Collapsed;
                    }break;
                case 1:
                    {
                        HoleType = HoleInfo.TYPE_IMAGE;
                        STKPNimage.Visibility = Visibility.Visible;
                        STKPNaudio.Visibility = Visibility.Collapsed;
                    }break;
                case 2:
                    {
                        HoleType = HoleInfo.TYPE_AUDIO;
                        STKPNimage.Visibility = Visibility.Collapsed;
                        STKPNaudio.Visibility = Visibility.Visible;
                        recorder = new AudioRecorder();
                    }break;
                default:break;
            }
        }

        private async void BTNpost_Click(object sender, RoutedEventArgs e)
        {
            PRGRSpost.ProgressStart();
            String content = TXTBLKpost.Text;
            if (content.Length > 1000)
            {
                Constants.BoxPage.ShowMessage("树洞内容不要太长");
                return;
            }
            if ("".Equals(content) && HoleType==HoleInfo.TYPE_TEXT)
            {
                Constants.BoxPage.ShowMessage("不能没有树洞内容");
                return;
            }
            List<Parameters> param = new List<Parameters>();
            String type = "text";
            switch (HoleType)
            {
                case 2:
                    {
                        type = "image";
                    }  
                    break;
                case 3:
                    {
                        type = "audio";
                    }
                    break;
            }
            param.Add(new Parameters("action", "dopost"));
            
            switch (HoleType)
            {
                case 2:
                    {
                        type = "image";
                        try
                        {
                            var a = await file.OpenAsync(FileAccessMode.ReadWrite);
                            Stream stream = a.AsStream();
                            byte[] bts = Util.StreamToBytes(stream);
                            String bmp_str = Convert.ToBase64String(bts);
                            param.Add(new Parameters("data", bmp_str));
                        }
                        catch
                        {
                            Constants.BoxPage.ShowMessage("添加图像内容失败");
                            PRGRSpost.ProgressEnd();
                            return;
                        }
                    }break;
                case 3:
                    {
                        type = "audio";
                        try
                        {
                            Stream stream = audioStream.AsStream();
                            byte[] bts = Util.StreamToBytes(stream);
                            String bmp_str = Convert.ToBase64String(bts);
                            param.Add(new Parameters("data", bmp_str));
                            param.Add(new Parameters("length", Length + ""));
                        }
                        catch
                        {
                            Constants.BoxPage.ShowMessage("添加音频内容失败");
                            PRGRSpost.ProgressEnd();
                            return;
                        }
                    }break;
            }
            param.Add(new Parameters("text", content));
            param.Add(new Parameters("token", Constants.token));
            param.Add(new Parameters("type", type));
            
            Parameters result = await WebConnection.Connect(Constants.domain + "/pkuhelper/../services/pkuhole/api.php", param);
            try
            {
                if (result.name != "200")
                {
                    Constants.BoxPage.ShowMessage("发布失败");
                    PRGRSpost.ProgressEnd();
                    return;
                }
                JsonObject jsonObject = JsonObject.Parse(result.value);
                int code;
                try
                {
                    code = (int)jsonObject.GetNamedNumber("code");
                }
                catch
                {
                    code = int.Parse(jsonObject.GetNamedString("code"));
                }
                if (code != 0)
                {
                    Constants.BoxPage.ShowMessage("发布失败");
                    PRGRSpost.ProgressEnd();
                    return;
                }
                else
                {
                    Constants.BoxPage.ShowMessage("发布成功！");
                    PRGRSpost.ProgressEnd();
                    BTNback_Click(null, null);

                    //这里应该加入以后导航到发表的页面
                    return;
                }
            }
            catch (Exception)
            {
                Constants.BoxPage.ShowMessage("发布失败");
                PRGRSpost.ProgressEnd();
                return;
            }
        }

        private void BTNback_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
                this.Frame.GoBack();
        }

        private async void BTNimageChoose_Click(object sender, RoutedEventArgs e)
        {
            file = await Util.ChooseFile(".png", ".jpg");

            if (file != null)
            {
                var r = await file.OpenAsync(FileAccessMode.ReadWrite);
                bitmap.SetSource(r);
                IMGshow.Source = bitmap;
            }
        }

        private async void ELPSaudio_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            lock (o)
            {
                recording = true;
            }
            Length = 0;
            Counter();
            GRIDrecord.Opacity = 0.6;
            STRBDrecord.Begin();
            await recorder.Record();
            /*try
            {
                await recorder.Record();
            }
            catch
            {
                Constants.BoxPage.ShowMessage("录音失败！");
                return;
            }*/
        }
        private async void Counter()
        {
            while (true)
            {
                lock (o)
                {
                    if (!recording) return;
                }
                await Task.Delay(1000);
                Length += 1;
                
            }
        }

        private async void ELPSaudio_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            lock (o)
            {
                recording = false;
            }
            GRIDrecord.Opacity = 1;
            STRBDrecord.Stop();
            SCALEaudio.ScaleX = SCALEaudio.ScaleY = 1;
            try
            {
                audioStream = await recorder.StopRecording();
                APaudio.SetSource(audioStream.CloneStream());
                //Length = APaudio.TotalLength;
            }catch{
                Constants.BoxPage.ShowMessage("录制失效！");
            }
        }

    }
}
