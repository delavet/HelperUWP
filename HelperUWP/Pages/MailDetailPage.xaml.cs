using HelperUWP.Lib.Web;
using HelperUWP.MailRef;
using MimeKit;
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
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace HelperUWP.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MailDetailPage : Page
    {
        private MimeMessage message = null;
        public MailDetailPage()
        {
            this.InitializeComponent();
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            STRBDpopin.Begin();
            if (e.Parameter == null) return;
            if (!(e.Parameter is MailInfo)) return;
            PRGRSmail.ProgressStart();
            var info = e.Parameter as MailInfo;
            message = await MailUtil.GetMessageAsync(info.MailFolderType, info.Index);
            try
            {
                TXTBLKmailTitle.Text = message.Subject;
            }
            catch
            {
                
            }
            try
            {
                WBVWmailBody.NavigateToString(message.HtmlBody);
            }
            catch
            {
                FindName(nameof(TXTBLKmailBody));
                TXTBLKmailBody.Text = message.TextBody;
            }
            PRGRSmail.ProgressEnd();
        }

        private void BTNback_Click(object sender, RoutedEventArgs e)
        {
            MailUtil.BackRequest();
        }

        private void BTNreply_Click(object sender, RoutedEventArgs e)
        {
            
            this.Frame.Navigate(typeof(MailSendPage), new Parameters(TryGetAddress(), "Re:" + message.Subject));
        }
        private String TryGetAddress()
        {
            try
            {
                String ret = message.From[0].ToString();
                if (ret.Contains("<")) ret = ret.Substring(ret.IndexOf('<') + 1, ret.Length - ret.IndexOf('<') - 2);
                return ret;
            }
            catch
            {
                return "";
            }
        }
    }
}
