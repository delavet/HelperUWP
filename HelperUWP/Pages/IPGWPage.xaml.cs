using HelperUWP.Lib;
using HelperUWP.Lib.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using HelperUWP.Lib.Storage;
using Windows.UI;
using Windows.Data.Json;
using System.Diagnostics;
using Windows.UI.Xaml.Media.Imaging;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace HelperUWP.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class IPGWPage : Page
    {
        private enum IPGW_type
        {   //指示网关的连接类型
            ConnectFree, ConnectNofree, Disconnect, DisconnectAll, None
        }
        private enum IPGWstatus
        {
            Nothing,Beginning,Multing,Choosing,Connecting
        }
        private IPGW_type ConnectType = IPGW_type.None;
        private IPGWstatus PageStatus = IPGWstatus.Nothing;
        private double HistoryX=0, HistoryY=0;

        public IPGWPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            STRBDpopin.Begin();
            await GetBingToday();//用于在校外环境时调试每日必应
        }

        //此函数留作以后的参考，无实际作用
        /*public void soy()
        {
            Task.Factory.StartNew(async () =>
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    
                    await GetBingToday();
                });
            });
        }*/

        private async Task GetBingToday()
        {
            try
            {
                String bing_url = "http://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1&mkt=zh-cn";
                Parameters result1 = await WebConnection.Connect(bing_url, null);
                if (result1.name != "200") return;
                JsonObject info_json = JsonObject.Parse(result1.value);
                JsonArray img_array = info_json.GetNamedArray("images", new JsonArray());
                JsonObject img_info = img_array.GetObjectAt(0);
                String img_url = img_info.GetNamedString("url", "");
                if (img_url.Equals(""))
                {
                    img_url = "http://s.cn.bing.net" + img_info.GetNamedString("urlbase", "");
                }
                if (img_url.Equals("http://s.cn.bing.net")) return;              
                Stream img_stream = await WebConnection.Connect_for_stream(img_url);
                var img_acstr = await Util.StreamToRandomAccessStream(img_stream);

                try
                {
                    var bing_bmp = new WriteableBitmap(1920, 1080);
                    bing_bmp.SetSource(img_acstr);
                    await SaveCacheBmp(bing_bmp);
                    Editor.putString("bing_history", DateTime.Now.ToString());
                }
                catch(Exception e)
                {
                    Debug.WriteLine(e.StackTrace);
                }

            }
            catch(Exception e)
            {
                Debug.WriteLine(e.StackTrace);
            }           
        }

        private async Task SaveCacheBmp(WriteableBitmap bmp)
        {
            try
            {
                await Cache.savePngFile(bmp, "CachedBg.png");
                var blured = await Util.Blur(bmp, Constants.BgBluredLevel);
                await Cache.savePngFile(blured, "CachedBluredBg.png");
                Constants.BoxPage.SetBg(bmp);
                Constants.BoxPage.SetBluredBg(blured);
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.StackTrace);
            }          
        }

        private Dictionary<String, String> GetReturnMsg(String str)
        {
            Dictionary<String, String> map = new Dictionary<string, string>();
            int a = str.Length;
            int pos1 = str.IndexOf("SUCCESS=");
            int pos2 = str.IndexOf("IPGWCLIENT_END-->");
            String msg = str.Substring(pos1, pos2 - pos1 - 1);
            String[] strs = msg.Split(' ');
            foreach (String tempStr in strs)
            {
                tempStr.Trim();
                if (!str.Contains("=")) continue;
                String[] strs2 = tempStr.Split('=');
                if (strs2.Length != 1)
                    map.Add(strs2[0], strs2[1]);
                else map.Add(strs2[0], "");
            }
            return map;
        }

        private async void DoConnection(IPGW_type connectType, String hintString)
        {
            String type = "connect";
            String free = "2";
            
            if (connectType == IPGW_type.ConnectNofree)
            {
                free = "1";
            }
            else if (connectType == IPGW_type.Disconnect)
                type = "disconnect";
            else if (connectType == IPGW_type.DisconnectAll)
                type = "disconnectall";
            //if (!Constants.isLogin()) ;//日后加入登录内容
            List<Parameters> paramList = new List<Parameters>();

            /**
             * 这两句仅用于测试！！！
             * 必须改掉！
             */
            //Constants.username = "1400012944";
            //Constants.password = "Delavet0719";



            paramList.Add(new Parameters("uid", Constants.username));
            paramList.Add(new Parameters("password", Constants.password));
            paramList.Add(new Parameters("operation", type));
            paramList.Add(new Parameters("range", free));
            paramList.Add(new Parameters("timeout", "-1"));
            Reset();
            PageStatus = IPGWstatus.Connecting;
            STRBDconnectEnd.Begin();


            Parameters parameter = await WebConnection.Connect("https://its.pku.edu.cn:5428/ipgatewayofpku", paramList);

            if (!"200".Equals(parameter.name))
            {
                if ("-1".Equals(parameter.name))
                {
                    Constants.BoxPage.ShowMessage("无法连接网络");
                }
                else
                {
                    Constants.BoxPage.ShowMessage("无法连接到服务器 (HTTP " + parameter.name + ")");                   
                }

            }
            else
            {
                Dictionary<String, String> map = GetReturnMsg(parameter.value);
                if (!map.ContainsKey("SUCCESS"))
                {
                    Constants.BoxPage.ShowMessage("网关连接失败，请重试");
                    STRBDconnecting.Stop();
                    return;
                }
                String successMsg = map["SUCCESS"];
                Boolean success = "YES".Equals(successMsg);

                if (connectType == IPGW_type.ConnectFree || connectType == IPGW_type.ConnectNofree)
                {
                    if (success)
                    {
                        String scope = "";
                        if ("f".Equals(map["SCOPE"].Trim())) scope = "免费地址";
                        else if ("international".Equals(map["SCOPE"].Trim())) scope = "收费地址";
                        hintString = scope + "\r\n"
                            + "IP: " + map["IP"] + "\r\n当前连接数：" + map["CONNECTIONS"] + "\r\n"
                            + "已用时长： " + map["FR_TIME"] + "\r\n" + "账户余额：" + map["BALANCE"];                                             
                        if (free.Equals("1"))
                        {
                            Boolean whether_get_bing = false;
                            String his_str = Editor.getString("bing_history", "");
                            try
                            {
                                if (!his_str.Equals(""))
                                {
                                    DateTime history = DateTime.Parse(his_str);
                                    int span = DateTime.Now.Day - history.Day;
                                    if (span >= 1 || span < 0) whether_get_bing = true;
                                }
                                else whether_get_bing = true;
                            }
                            catch { }
                            if (whether_get_bing)
                            {
                                await GetBingToday();
                            }
                        }
                        // 显示对话框
                        //两个Lib的函数？仍然没看懂
                        Constants.BoxPage.ShowMessage(hintString,"连接状态：已连接");
                    }
                    else
                    {                        
                        Constants.BoxPage.ShowMessage(map["REASON"], "连接失败");
                        //一个Lib的函数,未实现
                    }
                    STRBDconnecting.Stop();
                    return;
                }
                if (connectType == IPGW_type.Disconnect)
                {
                    if (success)
                    {
                        Constants.BoxPage.ShowMessage("", "断开连接成功");
                    }
                    else
                    {
                        Constants.BoxPage.ShowMessage(map["REASON"], "断开连接失败");
                        STRBDconnecting.Stop();
                        return;
                    }
                }
                if (connectType == IPGW_type.DisconnectAll)
                {
                    if (success)
                    {
                        Constants.BoxPage.ShowMessage("", "断开全部连接成功");
                    }
                    else
                    {
                        Constants.BoxPage.ShowMessage(map["REASON"], "断开全部连接失败");
                        STRBDconnecting.Stop();
                        return;
                    }
                }
            }
            
        }

        private void GRIDipgwMain_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (PageStatus != IPGWstatus.Multing && PageStatus != IPGWstatus.Choosing) return;
            Rect re = (PATHslid.Data as RectangleGeometry).Rect;
            re.X += e.Delta.Translation.X;
            re.Y += e.Delta.Translation.Y;
            (PATHslid.Data as RectangleGeometry).Rect = re;
        }

        private async void GRIDipgwMain_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            switch (PageStatus)
            {
                case IPGWstatus.Multing:
                    {
                        Reset();  
                                            
                    }break;
                case IPGWstatus.Choosing:
                    {
                        STRBDgreenRoll.Stop();
                        STRBDredRoll.Stop();
                        STRBDyellowRoll.Stop();
                        STRBDblueRoll.Stop();
                        PageStatus = IPGWstatus.Connecting;
                        DoConnection(ConnectType, "");
                        await Task.Delay(1000);
                        STRBDconnecting.SkipToFill();
                        STRBDconnecting.Stop();
                        PageStatus = IPGWstatus.Nothing;
                        ConnectType = IPGW_type.None;
                        STRBDcircle1.Begin();
                    }
                    break;
                default:
                    {
                        
                    }break;
            }
        }

        private void ELPSin_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            PageStatus = IPGWstatus.Beginning;
            STRBDconnectStart.Begin();
        }

        private void ELPSin_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            PageStatus = IPGWstatus.Nothing;
            STRBDconnectEnd.Begin();
        }

        private void ELPSin_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            PageStatus = IPGWstatus.Multing;
            var tempRect = PATHslid.Data as RectangleGeometry;
            HistoryX = tempRect.Rect.X;
            HistoryY = tempRect.Rect.Y;
        }

        private void PATHred_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (PageStatus != IPGWstatus.Multing) return;
            PageStatus = IPGWstatus.Choosing;
            ConnectType = IPGW_type.DisconnectAll;
            SetTitle("断开所有");
            STRBDredRoll.Begin();
        }

        private void PATHgreen_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (PageStatus != IPGWstatus.Multing) return;
            PageStatus = IPGWstatus.Choosing;
            ConnectType = IPGW_type.Disconnect;
            SetTitle("断开连接");
            STRBDgreenRoll.Begin();
        }

        private void PATHred_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (PageStatus != IPGWstatus.Choosing) return;
            PageStatus = IPGWstatus.Multing;
            SetTitle("北大网关");
            ConnectType = IPGW_type.None;
            STRBDredRoll.Stop();
            PROJred.RotationY = 0;
        }

        private void PATHgreen_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (PageStatus != IPGWstatus.Choosing) return;
            PageStatus = IPGWstatus.Multing;
            SetTitle("北大网关");
            ConnectType = IPGW_type.None;
            STRBDgreenRoll.Stop();
            PROJgreen.RotationY = 0;
        }

        private void PATHblue_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (PageStatus != IPGWstatus.Multing) return;
            PageStatus = IPGWstatus.Choosing;
            ConnectType = IPGW_type.ConnectFree;
            SetTitle("连接免费地址");
            STRBDblueRoll.Begin();
        }

        private void PATHblue_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (PageStatus != IPGWstatus.Choosing) return;
            PageStatus = IPGWstatus.Multing;
            SetTitle("北大网关");
            ConnectType = IPGW_type.None;
            STRBDblueRoll.Stop();
            PROJblue.RotationY = 0;
        }

        private void PATHyellow_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (PageStatus != IPGWstatus.Multing) return;
            PageStatus = IPGWstatus.Choosing;
            ConnectType = IPGW_type.ConnectNofree;
            SetTitle("连接收费地址");
            STRBDyellowRoll.Begin();
        }

        private void PATHyellow_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (PageStatus != IPGWstatus.Choosing) return;
            PageStatus = IPGWstatus.Multing;
            SetTitle("北大网关");
            ConnectType = IPGW_type.None;
            STRBDyellowRoll.Stop();
            PROJyellow.RotationY = 0;
        }

        private void Reset()
        {
            SetTitle("北大网关");
            RectangleGeometry tempRec = PATHslid.Data as RectangleGeometry;
            Rect re = tempRec.Rect;
            re.X = HistoryX;
            re.Y = HistoryY;
            tempRec.Rect = re;
            PATHslid.Data = tempRec;
            PageStatus = IPGWstatus.Nothing;
            ConnectType = IPGW_type.None;
            STRBDconnectEnd.Begin();
        }

        private void SetTitle(String str)
        {
            TXTBLKipgwTitle.Text = str;
        }

        private void STRBDconnectEnd_Completed(object sender, object e)
        {
            if (PageStatus != IPGWstatus.Connecting) STRBDcircle1.Begin();
            else STRBDconnecting.Begin();
            
        }

        private void STRBDpopin_Completed(object sender, object e)
        {
            STRBDcircle1.Begin();
        }

        private async void STRBDcircle_Completed(object sender, object e)
        {
            await Task.Delay(1000);
            if (PageStatus != IPGWstatus.Nothing) return;
            Random ran = new Random(DateTime.Now.Millisecond);
            int ranNum = ran.Next(5);
            switch (ranNum)
            {
                case 0:
                    {
                        STRBDcircle1.Begin();
                    }break;
                case 1:
                    {
                        STRBDcircle2.Begin();
                    }break;
                case 2:
                    {
                        STRBDcircle3.Begin();
                    }break;
                case 3:
                    {
                        STRBDcircle4.Begin();
                    }break;
                case 4:
                    {
                        STRBDcircle5.Begin();
                    }break;
                default:
                    {
                        STRBDcircle1.Begin();
                    }break;
            }
        }
    }
}
