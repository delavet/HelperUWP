using HelperUWP.ChatRef;
using HelperUWP.Lib;
using HelperUWP.Lib.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public sealed partial class ChatPage : Page
    {
        private bool flag;
        private object o = new object();
        private ChatMasterList ChatList = new ChatMasterList();
        public ChatPage()
        {
            this.InitializeComponent();
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            lock (o)
            {
                flag = true;
            }
            STRBDpopin.Begin();
            ChatUtil.chatPage = this;
            ChatList.DataLoading += ChatList_DataLoading;
            ChatList.DataLoaded += ChatList_DataLoaded;
            await ChatList.refresh();
            LSTVWchat.ItemsSource = ChatList;
            ContinuumRefresh();
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            ChatUtil.chatPage = null;
            lock (o)
            {
                flag = false;
            }
        }

        private void ContinuumRefresh()
        {
            Task.Factory.StartNew(async () =>
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, async () =>
                {
                    while (true)
                    {
                        bool temp = false;
                        lock (o)
                        {
                            temp = flag;
                        }
                        if (!temp) return;
                        try
                        {
                            Parameters fresh = await WebConnection.Connect(Constants.domain + "/pkuhelper/../services/hasnew.php?to=&uid=" + Constants.username+"&?="+DateTime.Now.Millisecond, null);
                            if (!fresh.name.Equals("200")) continue;
                            if (!fresh.value.Contains("1")) continue;
                            List<ChatListInfo> infos = await ChatList.GetList();
                            if (ChatList.RealNew(infos))
                                ChatList.Replace(infos);
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                        finally
                        {
                            await Task.Delay(4000);
                        }
                    }
                });
            });
        }
        private void PopoutComplete()
        {
            if (VSGchat.CurrentState == narrow) UpdateVisualState(narrow, null);
        }

        private void ChatList_DataLoaded()
        {
            PRGRSchatMaster.ProgressEnd();
        }

        private void ChatList_DataLoading()
        {
            PRGRSchatMaster.ProgressStart();
        }

        private void GRIDchatRoot_Loaded(object sender, RoutedEventArgs e)
        {
            double a = GRIDchatRoot.ActualWidth;
            if (a < 850) UpdateVisualState(narrow, null);
            else UpdateVisualState(wide, null);
        }

        private async void BTNadd_Click(object sender, RoutedEventArgs e)
        {
            ContentDialogResult res = await DLGchat.ShowAsync();
            if (res != ContentDialogResult.Primary) return;
            if (chatTo == "") return;
            CHATdetail.Show(chatTo);
            if (VSGchat.CurrentState == narrow) UpdateVisualState(narrow, null);
        }

        private void LSTVWchat_ItemClick(object sender, ItemClickEventArgs e)
        {
            CHATdetail.Show(e.ClickedItem as ChatListInfo);
            if (VSGchat.CurrentState == narrow) UpdateVisualState(narrow, null);
        }

        private void VSGchat_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            UpdateVisualState(e.NewState, e.OldState);
        }

        private void UpdateVisualState(VisualState newState, VisualState oldState)
        {
            if (newState == null) return;
            if (newState == narrow)
            {
                if (!CHATdetail.IsBlank)
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

        private void DLGchat_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            TXTBXchatChoose.Text = "";
        }

        private String chatTo = "";

        private void DLGchat_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            chatTo = TXTBXchatChoose.Text;
        }
    }
}
