using HelperUWP.Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Provider;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace HelperUWP.Controls
{
    public sealed partial class ImageShow : UserControl
    {
        private WriteableBitmap bmp;
        private Boolean zoom = false;
        public Boolean BtnVisible
        {
            get
            {
                return (BTNcancel.Visibility == Visibility.Visible) && (BTNsave.Visibility == Visibility.Visible);
            }
            set
            {
                if (value)
                {
                    BTNsave.Visibility = Visibility.Visible;
                    BTNcancel.Visibility = Visibility.Visible;
                }
                else
                {
                    BTNsave.Visibility = Visibility.Collapsed;
                    BTNcancel.Visibility = Visibility.Collapsed;
                }
            }
        }
        public ImageShow()
        {
            this.InitializeComponent();
        }

        public void Show(WriteableBitmap src,Boolean btnVis=true)
        {
            this.Visibility = Visibility.Visible;
            
            bmp = src;
            IMG.Source = bmp;
            BtnVisible = btnVis;
            STRBDpopin.Begin();
        }
        
        private void IMG_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            zoom = !zoom;
            if (zoom)
            {
                STRBDzoom.Begin();
            }else
            {
                STRBDzoomBack.Begin();
            }
        }

        private void GRIDroot_Loaded(object sender, RoutedEventArgs e)
        {
            IMG.Width = GRIDroot.ActualWidth;
        }

        private async void BTNcancel_Click(object sender, RoutedEventArgs e)
        {
            STRBDpopout.Begin();
            await Task.Delay(100);
            this.Visibility = Visibility.Collapsed;
        }

        private void BTNsave_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        public async void Save()
        {
            FileUpdateStatus stat = await Util.SaveToPngImage(bmp, Windows.Storage.Pickers.PickerLocationId.PicturesLibrary, DateTime.Now.ToString("yyyyMMddhhmmss"));
            if (stat == FileUpdateStatus.Complete || stat == FileUpdateStatus.CompleteAndRenamed)
            {
                Constants.BoxPage.ShowMessage("保存成功！");
            }
            else
            {
                Constants.BoxPage.ShowMessage("未能保存");
            }
        }

        private void IMG_Loaded(object sender, RoutedEventArgs e)
        {
            SCALEimg.CenterX = IMG.ActualWidth / 2;
            SCALEimg.CenterY = IMG.ActualHeight / 2;
        }

        private async void GRIDroot_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!BtnVisible) return;
            if (e.OriginalSource != IMG)
            {
                STRBDpopout.Begin();
                await Task.Delay(100);
                this.Visibility = Visibility.Collapsed;
            }
        }
    }
}
