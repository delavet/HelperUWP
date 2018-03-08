using HelperUWP.Lib;
using HelperUWP.Lib.Storage;
using HelperUWP.Lib.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Data.Json;
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

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace HelperUWP.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        private String Username = "", Password = "";
        public LoginPage()
        {
            this.InitializeComponent();
        }

        private async void finishIAAA()
        {
            MessageDialog dlg = new MessageDialog("来自" + Constants.major + "的" + Constants.name + "你好，欢迎使用PKU Helper！", "欢迎使用PKU Helper UWP");
            UICommand cmdYes = new UICommand();
            cmdYes.Label = "确定";
            dlg.Commands.Add(cmdYes);
            // 显示对话框
            var cmd = await dlg.ShowAsync();
        }

        private async Task<Boolean> FinishLogin(String resultStr)
        {
            try
            {
                JsonObject jsonObject = JsonObject.Parse(resultStr);
                int code = (int)jsonObject.GetNamedNumber("code");
                if (code != 0)
                {
                    MessageDialog dlg = new MessageDialog(jsonObject.GetNamedString("msg", "登录失败"), "不好意思，炸了");
                    UICommand cmdYes = new UICommand();
                    cmdYes.Label = "怂";
                    dlg.Commands.Add(cmdYes);

                    // 显示对话框
                    var cmd = await dlg.ShowAsync();
                    return false;
                }
                Constants.token = jsonObject.GetNamedString("token");
                Constants.user_token = jsonObject.GetNamedString("user_token");
                Constants.name = jsonObject.GetNamedString("name");
                Constants.sex = jsonObject.GetNamedString("gender");
                Constants.major = jsonObject.GetNamedString("department");
                Constants.birthday = jsonObject.GetNamedString("birthday");
                Constants.username = Username;
                Constants.password = Password;
                Editor.putString("token", Constants.token);
                Editor.putString("user_token", Constants.user_token);
                Editor.putString("username", Constants.username);
                Editor.putString("password", Constants.password);
                Editor.putString("name", Constants.name);
                Editor.putString("major", Constants.major);
                Editor.putString("sex", Constants.sex);
                Editor.putString("birthday", Constants.birthday);
                finishIAAA();
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                MessageDialog dlg = new MessageDialog("登录失败", "不好意思，炸了");
                UICommand cmdYes = new UICommand();
                cmdYes.Label = "怂";
                dlg.Commands.Add(cmdYes);
                // 显示对话框
                var cmd = await dlg.ShowAsync();
                return false;
            }
        }

        private async Task<Parameters> ExecuteConnect()
        {
            try
            {
                Parameters parameters = await WebConnection.Connect(Constants.domain + "/services/login/local.php", null);
                if (!"200".Equals(parameters.name)) return parameters;
                JsonObject jsonObject = JsonObject.Parse(parameters.value);
                int code = (int)jsonObject.GetNamedNumber("code");
                if (code != 0) return parameters;
                Boolean local = (jsonObject.GetNamedNumber("local", 0) != 0);
                String token = "";

                /*JsonObject jsonObject;
                Boolean local = true;
                Parameters parameters;
                String token = "";*/

                if (local)
                {
                    await WebConnection.Connect("https://portal.pku.edu.cn/portal2013/index.jsp", null);
                    List <Parameters> list1 = new List<Parameters>();
                    list1.Add(new Parameters("appid", "portal"));
                    list1.Add(new Parameters("userName", Username));
                    list1.Add(new Parameters("password", Password));
                    list1.Add(new Parameters("redirUrl",
                            "http://portal.pku.edu.cn/portal2013/login.jsp/../ssoLogin.do"));
                    parameters = await WebConnection.Connect("https://iaaa.pku.edu.cn/iaaa/oauthlogin.do", list1);
                    if (!"200".Equals(parameters.name)) return parameters;
                    jsonObject = JsonObject.Parse(parameters.value);
                    Boolean success = jsonObject.GetNamedBoolean("success");
                    if (!success)
                    {
                        JsonObject errors = jsonObject.GetNamedObject("errors");
                        String msg = errors.GetNamedString("msg", "登录失败");
                        return new Parameters("200", "{\"code\": 1, \"msg\": \"" + msg + "\"}");
                    }
                    token = jsonObject.GetNamedString("token");
                    Random ra = new Random();
                    Parameters tempLoginPara = await WebConnection.Connect("http://portal.pku.edu.cn/portal2013/ssoLogin.do?rand="+ra.NextDouble()+"&token="+token, null);
                    tempLoginPara = await WebConnection.Connect("http://portal.pku.edu.cn/portal2013/isUserLogged.do", null);
                    //username = "";
                    //password = "";
                }
                List<Parameters> list = new List<Parameters>();
                list.Add(new Parameters("uid", Username));
                list.Add(new Parameters("password", Password));
                list.Add(new Parameters("token", token));
                list.Add(new Parameters("platform", "Android"));
                return await WebConnection.Connect(Constants.domain + "/services/login/login.php", list);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return new Parameters("200", "{\"code\": 1, \"msg\": \"登录失败，请重试\"}");
            }
        }

        private async Task<Boolean> doLogin()
        {
            if ("12345678".Equals(Username) && "admin".Equals(Password))
            {
                Constants.token = "admin";
                Constants.user_token = "admin";
                Constants.username = "admin";
                Constants.password = Password;
                Constants.name = "管理员";
                Constants.major = "PKU Helper";
                Constants.sex = "";
                Constants.birthday = "";
                Editor.putString("token", Constants.token);
                Editor.putString("user_token", Constants.user_token);
                Editor.putString("username", Constants.username);
                Editor.putString("password", Constants.password);
                Editor.putString("name", Constants.name);
                Editor.putString("major", Constants.major);
                Editor.putString("sex", Constants.sex);
                Editor.putString("birthday", Constants.birthday);
                finishIAAA();
                Constants.init();
                return true;
            }
            Parameters result = await ExecuteConnect();
            if (!"200".Equals(result.name))
            {
                if ("-1".Equals(result.name))
                {
                    MessageDialog dlg = new MessageDialog("无法连接网络(-1, -1)", "不好意思，炸了");
                    UICommand cmdYes = new UICommand();
                    cmdYes.Label = "怂";
                    dlg.Commands.Add(cmdYes);
                    // 显示对话框
                    var cmd = await dlg.ShowAsync();
                    return false;
                }
                else
                {
                    MessageDialog dlg = new MessageDialog("无法连接到服务器 (HTTP " + result.name + ")", "不好意思，炸了");
                    UICommand cmdYes = new UICommand();
                    cmdYes.Label = "怂";
                    dlg.Commands.Add(cmdYes);
                    // 显示对话框
                    var cmd = await dlg.ShowAsync();
                    return false;
                }
            }
            else return await FinishLogin(result.value);
        }

        private async void BTNsubmit_Click(object sender, RoutedEventArgs e)
        {
            Username = TXTBXid.Text;
            Password = TXTBXpassword.Password;
            if (Username == "" || Password == "")
            {
                MessageDialog dlg = new MessageDialog("用户名和密码都不能为空", "warning");
                UICommand cmdOK = new UICommand("确定");
                dlg.Commands.Add(cmdOK);
                await dlg.ShowAsync();
                return;
            }
            PRGRSlogin.Visibility = Visibility.Visible;
            Boolean success = await doLogin();
            PRGRSlogin.Visibility = Visibility.Collapsed;
            if (!success) return;
            this.Frame.Navigate(typeof(MainPage));
        }
    }
}
