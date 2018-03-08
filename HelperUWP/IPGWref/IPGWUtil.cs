using HelperUWP.Lib;
using HelperUWP.Lib.Storage;
using HelperUWP.Lib.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Xaml.Media.Imaging;

namespace HelperUWP.IPGWref
{

    public enum IPGW_type
    {   //指示网关的连接类型
        ConnectFree, ConnectNofree, Disconnect, DisconnectAll, None
    }
    public enum IPGWstatus
    {
        Nothing, Beginning, Multing, Choosing, Connecting
    }
    class IPGWUtil
    {
        public static async Task GetBingToday()
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
                catch (Exception e)
                {
                    Debug.WriteLine(e.StackTrace);
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
            }
        }

        public static async Task SaveCacheBmp(WriteableBitmap bmp)
        {
            try
            {
                await Cache.savePngFile(bmp, "CachedBg.png");
                var blured = await Util.Blur(bmp, Constants.BgBluredLevel);
                await Cache.savePngFile(blured, "CachedBluredBg.png");
                Constants.BoxPage.SetBg(bmp);
                Constants.BoxPage.SetBluredBg(blured);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
            }
        }

        private static Dictionary<String, String> GetReturnMsg(String str)
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
        public static async Task<IPGWMsg> DoConnection(IPGW_type connectType, String hintString="")
        {
            if (connectType == IPGW_type.None) return null;
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
            List<Parameters> paramList = new List<Parameters>();
            paramList.Add(new Parameters("uid", Constants.username));
            paramList.Add(new Parameters("password", Constants.password));
            paramList.Add(new Parameters("operation", type));
            paramList.Add(new Parameters("range", free));
            paramList.Add(new Parameters("timeout", "-1"));
            Parameters result = await WebConnection.Connect("https://its.pku.edu.cn:5428/ipgatewayofpku", paramList);
            if (!"200".Equals(result.name))
            {
                Util.DealWithDisconnect(result);
                return null;
            }
            Dictionary<String, String> map = GetReturnMsg(result.value);
            if (!map.ContainsKey("SUCCESS")) return null;
            String successMsg = map["SUCCESS"];
            bool success = "YES".Equals(successMsg);
            String title = "", content = "";
            switch (connectType)
            {
                case IPGW_type.ConnectFree:
                    {
                        if (success) title = "连接免费地址成功";
                        else title = map["REASON"];
                        String scope = "";
                        if ("f".Equals(map["SCOPE"].Trim())) scope = "免费地址";
                        else if ("international".Equals(map["SCOPE"].Trim())) scope = "收费地址";
                        content = scope + "\n"
                            + "IP: " + map["IP"] + "\n当前连接数：" + map["CONNECTIONS"] + "\n"
                            + "已用时长： " + map["FR_TIME"] + "\n" + "账户余额：" + map["BALANCE"];
                    }
                    break;
                case IPGW_type.ConnectNofree:
                    {
                        if (success) title = "连接收费地址成功";
                        else title = map["REASON"];
                        String scope = "";
                        if ("f".Equals(map["SCOPE"].Trim())) scope = "免费地址";
                        else if ("international".Equals(map["SCOPE"].Trim())) scope = "收费地址";
                        content = scope + "\n"
                            + "IP: " + map["IP"] + "\n当前连接数：" + map["CONNECTIONS"] + "\n"
                            + "已用时长： " + map["FR_TIME"] + "\n" + "账户余额：" + map["BALANCE"];
                        /*if (free.Equals("1"))
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
                        }*/
                    }
                    break;
                case IPGW_type.Disconnect:
                    {
                        if (success) title = "断开连接成功";
                        else
                        {
                            title= "断开连接失败";
                            content = map["REASON"];
                        }
                    }break;
                case IPGW_type.DisconnectAll:
                    {
                        if (success) title = "断开全部连接成功";
                        else
                        {
                            title = "断开全部连接失败";
                            content = map["REASON"];
                        }
                    }
                    break;
            }
            return new IPGWMsg(success, title, content, connectType);
        }

    }
}
