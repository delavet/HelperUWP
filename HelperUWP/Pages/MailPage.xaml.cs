using HelperUWP.Lib;
using HelperUWP.MailRef;
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
    public sealed partial class MailPage : Page
    {
        public Boolean Sending
        {
            get
            {
                try
                {
                    Boolean ret = FRAMEdetail.Content is MailSendPage;
                    return ret;
                }
                catch
                {
                    return false;
                }
            }
        }

        private MailList inboxList = null, sentList = null, junkList = null, trashList = null;
        public MailPage()
        {
            this.InitializeComponent();
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {           
            STRBDpopin.Begin();
            MailUtil.mailPage = this;
            await MailUtil.Login();
            inboxList = new MailList();
            inboxList.DataLoading += InboxLoading;
            inboxList.DataLoaded += InboxLoaded;
            LSTVWinbox.ItemsSource = inboxList;
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {           
            MailUtil.mailPage = null;
            MailUtil.Logout();
        }
        public void InnerBackRequest()
        {
            if (VSGmail.CurrentState == narrow && COLdetail.Width.Value != 0)
            {
                STRBDdetailShrink.Begin();
            }
            else if (VSGmail.CurrentState == wide && !(FRAMEdetail.Content == null || (FRAMEdetail.Content is InfoBlankPage)))
            {
                STRBDdetailShrink.Begin();
            }
        }

        private void InboxLoading()
        {
            PRGRSinbox.ProgressStart();
        }
        private void InboxLoaded()
        {
            PRGRSinbox.ProgressEnd();
        }
        private void SentLoading()
        {
            PRGRSsent.ProgressStart();
        }
        private void SentLoaded()
        {
            PRGRSsent.ProgressEnd();
        }
        private void JunkLoading()
        {
            PRGRSjunk.ProgressStart();
        }
        private void JunkLoaded()
        {
            PRGRSjunk.ProgressEnd();
        }
        private void TrashLoading()
        {
            PRGRStrash.ProgressStart();
        }
        private void TrashLoaded()
        {
            PRGRStrash.ProgressEnd();
        }

        private void PVTmail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PVTEHmail.UpdateUI(PVTmail.SelectedIndex);
            switch (PVTmail.SelectedIndex)
            {
                case 0:
                    {
                        
                    }
                    break;
                case 1:
                    {
                        if (sentList != null) return;
                        sentList = new MailList(MailUtil.FolderType.SentItem);
                        sentList.DataLoading += SentLoading;
                        sentList.DataLoaded += SentLoaded;
                        LSTVWsent.ItemsSource = sentList;
                    }
                    break;
                case 2:
                    {
                        if (junkList != null) return;
                        junkList = new MailList(MailUtil.FolderType.JunkMail);
                        junkList.DataLoading += JunkLoading;
                        junkList.DataLoaded += JunkLoaded;
                        LSTVWjunk.ItemsSource = junkList;
                    }
                    break;
                case 3:
                    {
                        if(trashList!=null)return;
                        trashList = new MailList(MailUtil.FolderType.Trash);
                        trashList.DataLoading += TrashLoading;
                        trashList.DataLoaded += TrashLoaded;
                        LSTVWtrash.ItemsSource = trashList;
                    }
                    break;
                default:break;
            }
        }

        private void LSTVWinbox_ItemClick(object sender, ItemClickEventArgs e)
        {
            MailSummary clicked = e.ClickedItem as MailSummary;
            FRAMEdetail.Navigate(typeof(MailDetailPage), new MailInfo(MailUtil.FolderType.Inbox, clicked.Index));
            if (VSGmail.CurrentState == narrow) UpdateVisualState(narrow, null);
        }

        private void LSTVWsent_ItemClick(object sender, ItemClickEventArgs e)
        {
            MailSummary clicked = e.ClickedItem as MailSummary;
            FRAMEdetail.Navigate(typeof(MailDetailPage), new MailInfo(MailUtil.FolderType.SentItem, clicked.Index));
            if (VSGmail.CurrentState == narrow) UpdateVisualState(narrow, null);
        }

        private void LSTVWjunk_ItemClick(object sender, ItemClickEventArgs e)
        {
            MailSummary clicked = e.ClickedItem as MailSummary;
            FRAMEdetail.Navigate(typeof(MailDetailPage), new MailInfo(MailUtil.FolderType.JunkMail, clicked.Index));
            if (VSGmail.CurrentState == narrow) UpdateVisualState(narrow, null);
        }

        private void LSTVWtrash_ItemClick(object sender, ItemClickEventArgs e)
        {
            MailSummary clicked = e.ClickedItem as MailSummary;
            FRAMEdetail.Navigate(typeof(MailDetailPage), new MailInfo(MailUtil.FolderType.Trash, clicked.Index));
            if (VSGmail.CurrentState == narrow) UpdateVisualState(narrow, null);
        }

        private void STRBDdetailShrink_Completed(object sender, object e)
        {
            TRANSdetail.Y = 0;
            FRAMEdetail.Opacity = 1;
            if((FRAMEdetail.Content is MailSendPage)&& FRAMEdetail.CanGoBack)
            {
                FRAMEdetail.GoBack();
            }
            else FRAMEdetail.Navigate(typeof(InfoBlankPage));
            if (VSGmail.CurrentState == narrow) UpdateVisualState(narrow, null);
        }

        private void BTNsend_Click(object sender, RoutedEventArgs e)
        {
            FRAMEdetail.Navigate(typeof(MailSendPage), null);
            if (VSGmail.CurrentState == narrow) UpdateVisualState(narrow, null);
        }

        private void GRIDmailRoot_Loaded(object sender, RoutedEventArgs e)
        {
            double a = GRIDmailRoot.ActualWidth;
            if (a < 850) UpdateVisualState(narrow, null);
            else UpdateVisualState(wide, null);
        }

        private void VSGmail_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            UpdateVisualState(e.NewState, e.OldState);
        }
        private void UpdateVisualState(VisualState newState, VisualState oldState)
        {
            if (newState == null) return;
            if (newState == narrow)
            {
                if (!(FRAMEdetail.Content == null || (FRAMEdetail.Content is InfoBlankPage)))
                {
                    COLmaster.Width = new GridLength(0);
                    COLdetail.Width = new GridLength(1, GridUnitType.Star);
                    Constants.BoxPage.HamburgerVisible = false;
                }
                else
                {
                    COLmaster.Width = new GridLength(1, GridUnitType.Star);
                    COLdetail.Width = new GridLength(0);
                    Constants.BoxPage.HamburgerVisible = true;
                }
            }
            else
            {
                if (COLdetail.Width.Value > 0) return;
                COLmaster.Width = new GridLength(400);
                COLdetail.Width = new GridLength(1, GridUnitType.Star);
                Constants.BoxPage.HamburgerVisible = true;
            }

        }
        public Boolean TryFrameGoBack()
        {
            Boolean can = FRAMEdetail.CanGoBack;
            if (can)
            {
                FRAMEdetail.GoBack();
            }
            return can;
        }
    }
}
