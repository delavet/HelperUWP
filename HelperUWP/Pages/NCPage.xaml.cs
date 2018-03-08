using HelperUWP.NCref;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class NCPage : Page
    {
        private int PreIndex=0;
        private CourseNoticeList courseNotice;
        public NCPage()
        {
            this.InitializeComponent();
            courseNotice = new CourseNoticeList();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            STRBDpopin.Begin();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PVTnc.SelectedIndex= Convert.ToInt32((sender as Button).Tag);
        }

        private void UpdateUI()
        {
            switch (PVTnc.SelectedIndex)
            {
                case 0:
                    {
                        BTNcourse.Foreground = new SolidColorBrush(Color.FromArgb(0XFF, 0, 0, 0));
                        BTNall.Foreground = new SolidColorBrush(Color.FromArgb(0XFF, 0X77, 0X77, 0X77));
                        BTNone.Foreground = new SolidColorBrush(Color.FromArgb(0XFF, 0X77, 0X77, 0X77));
                    }break;
                case 1:
                    {
                        BTNcourse.Foreground = new SolidColorBrush(Color.FromArgb(0XFF, 0X77, 0X77, 0X77));
                        BTNall.Foreground = new SolidColorBrush(Color.FromArgb(0XFF, 0, 0, 0));
                        BTNone.Foreground = new SolidColorBrush(Color.FromArgb(0XFF, 0X77, 0X77, 0X77));
                    }
                    break;
                case 2:
                    {
                        BTNcourse.Foreground = new SolidColorBrush(Color.FromArgb(0XFF, 0X77, 0X77, 0X77));
                        BTNall.Foreground = new SolidColorBrush(Color.FromArgb(0XFF, 0X77, 0X77, 0X77));
                        BTNone.Foreground = new SolidColorBrush(Color.FromArgb(0XFF, 0, 0, 0));
                    }
                    break;
            }
            ANIncBg.From = (Color)this.Resources[PreIndex.ToString()];
            PreIndex = PVTnc.SelectedIndex;
            ANIncBg.To= (Color)this.Resources[PreIndex.ToString()];
            STRBDbgChange.Begin();
        }

        private void PVTnc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateUI();
        }
    }
}
