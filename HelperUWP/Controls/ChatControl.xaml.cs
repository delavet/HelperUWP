using HelperUWP.ChatRef;
using HelperUWP.Lib;
using HelperUWP.Lib.Web;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
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
    public sealed partial class ChatControl : UserControl
    {
        private ChatListInfo MasterInfo;
        private ChatDetailList chatList;
        private object o = new object();
        private object newJudgeLock = new object();
        private bool newLoad = false;
        private bool flag = false;
        private double historyHeight = 0;
        public delegate void SthCompleted();
        public event SthCompleted PopoutCompleted;
        StorageFile imgFile = null;
        public bool IsBlank
        {
            get
            {
                return this.Visibility == Visibility.Collapsed;
            }
        }
        public ChatControl()
        {
            this.InitializeComponent();
            this.Visibility = Visibility.Collapsed;
        }
        private void DataLoading()
        {
            PRGRSchat.ProgressStart();
            
        }
        private void DataLoaded()
        {
            lock (newJudgeLock)
            {
                newLoad = false;
            }
            PRGRSchat.ProgressEnd();            
        }
        private void NewDataLoaded()
        {
            lock (newJudgeLock)
            {
                newLoad = true;
            }
            PRGRSchat.ProgressEnd();
        }

        public void Show(String user)
        {
            Show(new ChatListInfo(user, user, Util.GetTimeStamp(DateTime.Now), "", 0, false));
        }

        public async void Show(ChatListInfo masterInfo)
        {
            historyHeight = 0;
            lock (o)
            {
                MasterInfo = masterInfo;
                flag = false;
            }
            this.Visibility = Visibility.Visible;
            STRBDpopin.Begin();
            
            lock (o)
            {
                chatList = new ChatDetailList(masterInfo.UserName);
                chatList.DataLoading += DataLoading;
                chatList.DataLoaded += DataLoaded;
                chatList.NewLoaded += NewDataLoaded; 
                flag = true;
            }
            await chatList.LoadMoreHistory();
            LSTVWchat.ItemsSource = chatList;
            ContinuumRefresh();
        }
        private void ContinuumRefresh()
        {
            Task.Factory.StartNew(async () =>
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, async () =>
                {
                    String myUid;
                    lock (o)
                    {
                        myUid = MasterInfo != null ? MasterInfo.UserName : "";
                    }
                    if (myUid.Equals("")) return;
                    while (true)
                    {
                        bool quit = false;
                        lock (o)
                        {
                            if (!flag) quit = true;
                            if (!myUid.Equals(MasterInfo.UserName)) quit = true;
                        }
                        if (quit) break;
                        try
                        {
                            if(await chatList.HasNew())
                            {
                                await chatList.LoadNew();
                            }
                        }
                        catch
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

        private void BTNback_Click(object sender, RoutedEventArgs e)
        {
            flag = false;
            STRBDpopout.Begin();
            if (chatList != null) chatList.Clear();    
        }

        private void STRBDpopout_Completed(object sender, object e)
        {
            this.Visibility = Visibility.Collapsed;
            LSTVWchat.ItemsSource = null;
            PopoutCompleted?.Invoke();
        }

        private void LSTVWchat_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            
        }

        private void LSTVWchat_Loaded(object sender, RoutedEventArgs e)
        {
           //SCRVWchat.ChangeView(0, SCRVWchat.ScrollableHeight, 1, true);
        }

        private async void SCRVWchat_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (SCRVWchat == null) return;
            if (SCRVWchat.VerticalOffset <= 3)
            {
                await chatList.LoadMoreHistory();
            }
        }

        private void LSTVWchat_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            lock (newJudgeLock)
            {
                if (newLoad)
                {
                    SCRVWchat.ChangeView(0, SCRVWchat.ScrollableHeight, 1, true);
                    return;
                }
            }
            if (Math.Abs(SCRVWchat.ScrollableHeight - historyHeight) < 10) return;
            SCRVWchat.ChangeView(0, SCRVWchat.ScrollableHeight - historyHeight, 1, true);
            historyHeight = SCRVWchat.ScrollableHeight;
        }

        private void LSTVWchat_ItemClick(object sender, ItemClickEventArgs e)
        {
            ChatDetailInfo info = e.ClickedItem as ChatDetailInfo;
            if (info.Mime != ChatInfoType.Image) return;
            IMGchat.Show((e.ClickedItem as ChatDetailInfo).bmpsrc);
        }

        private void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (TXTBXchat.Text == "" || (TXTBXchat.Text == null)) return;
            if (e.Key == VirtualKey.Enter)
            {
                String send = TXTBXchat.Text;
                TXTBXchat.Text = "";
                SendMessage(send);
            }
        }
        private async void SendMessage(String msg)
        {
            DataLoading();
            List<Parameters> param = new List<Parameters>();
            param.Add(new Parameters("content", msg));
            param.Add(new Parameters("to", MasterInfo.UserName));
            param.Add(new Parameters("token", Constants.token));
            param.Add(new Parameters("type", "sendmsg"));
            Parameters result = await WebConnection.Connect(Constants.domain + "/pkuhelper/../services/msg.php", param);
            if (!result.name.Equals("200"))
            {
                DataLoaded();
                Util.DealWithDisconnect(result);
                return;
            }           
            try
            {
                JsonObject obj = JsonObject.Parse(result.value);
                int code = (int)obj.GetNamedNumber("code");
                if (code != 0) throw new Exception("code is not zero");
                String m = obj.GetNamedString("msg");
                if(!m.Equals("ok")) throw new Exception("msg is not ok");
                chatList.Add(new ChatDetailInfo(-1, msg, "text/plain", Util.GetTimeStamp(DateTime.Now), "to"));
                NewDataLoaded();
            }
            catch
            {
                DataLoaded();
                return;
            }
        }

        private void BTNimgSend_Click(object sender, RoutedEventArgs e)
        {
            SendImage();
        }

        private async void SendImage()
        {
            DataLoading();       
            try
            {
                imgFile = await Util.ChooseFile(".png", ".jpg");
                List<Parameters> param = new List<Parameters>();
                var a = await imgFile.OpenAsync(FileAccessMode.ReadWrite);
                Stream stream = a.AsStream();
                byte[] bts = Util.StreamToBytes(stream);
                String bmp_str = Convert.ToBase64String(bts);
                param.Add(new Parameters("uid", Constants.username));
                param.Add(new Parameters("type", "image"));
                param.Add(new Parameters("token", Constants.token));
                param.Add(new Parameters("to", MasterInfo.UserName));
                param.Add(new Parameters("data", bmp_str));
                Parameters result = await WebConnection.Connect(Constants.domain + "/pkuhelper/../services/message/postDataMessage.php", param);
                if (!result.name.Equals("200"))
                {
                    DataLoaded();
                    Util.DealWithDisconnect(result);
                    return;
                }
                JsonObject obj = JsonObject.Parse(result.value);
                int code = (int)obj.GetNamedNumber("code");
                if (code != 0) throw new Exception("code is not zero");
                String m = obj.GetNamedString("msg");
                if (!m.Equals("ok")) throw new Exception("msg is not ok");
                WriteableBitmap bmp = new WriteableBitmap(100, 200);
                bmp.SetSource(a);
                ChatDetailInfo temp_info = new ChatDetailInfo(bmp);
                chatList.Add(temp_info);
                NewDataLoaded();
            }
            catch
            {
                DataLoaded();
                return;
            }
        }

        private void TXTBXchat_GotFocus(object sender, RoutedEventArgs e)
        {
            SCRVWchat.ChangeView(0, SCRVWchat.ScrollableHeight, 1, true);
        }
    }
}
