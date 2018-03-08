using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace HelperUWP.Controls
{
    public sealed partial class MessageShow : UserControl
    {
        public MessageShow()
        {
            this.InitializeComponent();
            grid_GG.Visibility = Visibility.Collapsed;
        }
        

        public void ShowMsg(String content, String title="")
        {
            TXTBLKtitle.Height = 25;
            if (title.Equals("")) TXTBLKtitle.Height = 0;
            grid_GG.Visibility = Visibility.Visible;
            TXTBLKtitle.Text = title;
            txt_GG.Text = content;
            BTN.Visibility = Visibility.Collapsed;
            BTN.IsEnabled = false;
            STRBDshow.Begin();
        }
        public void ShowMsgPopin(String content,String title = "")
        {
            TXTBLKtitle.Height = 25;
            if (title.Equals("")) TXTBLKtitle.Height = 0;
            grid_GG.Visibility = Visibility.Visible;
            TXTBLKtitle.Text = title;
            txt_GG.Text = content;
            BTN.Visibility = Visibility.Visible;
            BTN.IsEnabled = true;
            STRBDpopin.Begin();
        }

        private void STRBDshow_Completed(object sender, object e)
        {
            grid_GG.Visibility = Visibility.Collapsed;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            STRBDpopout.Begin();
        }
    }
}
