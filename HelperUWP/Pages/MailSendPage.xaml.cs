using HelperUWP.Lib;
using HelperUWP.Lib.Web;
using HelperUWP.MailRef;
using MailKit.Net.Smtp;
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
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace HelperUWP.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MailSendPage : Page
    {
        public MailSendPage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            STRBDpopin.Begin();
            if (e.Parameter == null) return;
            if (!(e.Parameter is Parameters)) return;
            Parameters p = e.Parameter as Parameters;
            TXTBXreceiver.Text = p.name;
            TXTBXsubject.Text = p.value;
        }

        private void BTNback_Click(object sender, RoutedEventArgs e)
        {
            MailUtil.BackRequest();
        }

        private void BTNsend_Click(object sender, RoutedEventArgs e)
        {
            PRGRSsend.ProgressStart();
            if (TXTBXreceiver.Text.Equals("") || TXTBXsubject.Text.Equals("") || TXTBXbody.Text.Equals("")) return;
            try
            {
                var message = new MimeMessage();
                message.To.Add(new MailboxAddress(TXTBXreceiver.Text, TXTBXreceiver.Text));
                message.From.Add(new MailboxAddress(Constants.name, Constants.username + "@pku.edu.cn"));
                message.Subject = TXTBXsubject.Text;
                message.Body = new TextPart("plain")
                {
                    Text = TXTBXbody.Text
                };
                using (var client = new SmtpClient())
                {
                    client.Connect("mail.pku.edu.cn", 465, true);

                    // Note: since we don't have an OAuth2 token, disable
                    // the XOAUTH2 authentication mechanism.
                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    // Note: only needed if the SMTP server requires authentication
                    client.Authenticate(Constants.username, Constants.password);

                    client.Send(message);
                    client.Disconnect(true);
                }
                Constants.BoxPage.ShowMessage("发送成功！");
                MailUtil.BackRequest();
            }
            catch
            {
                Constants.BoxPage.ShowMessage("发送失败，请检查您的邮件内容");
            }
            PRGRSsend.ProgressEnd();
        }
    }
}
