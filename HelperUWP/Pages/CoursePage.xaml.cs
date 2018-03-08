using HelperUWP.Lib;
using HelperUWP.Lib.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using HtmlAgilityPack;
using System.Threading.Tasks;
using HelperUWP.CourseRef;
using System.Diagnostics;
using HelperUWP.Controls;
using Windows.UI.Xaml.Media.Imaging;
using System.Reflection.Metadata;
using Windows.UI;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace HelperUWP.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class CoursePage : Page
    {
        private List<CourseInfoControl> controls = new List<CourseInfoControl>();
        private String VerifyCode = "";
        private Style FlyoutStyle = new Style(typeof(FlyoutPresenter));
        public CoursePage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            CourseUtil.currentCoursePage = this;
            Setter tSetter = new Setter(FlyoutPresenter.BackgroundProperty, new SolidColorBrush(Color.FromArgb(0x9F, 0xFF, 0xFF, 0xFF)));
            FlyoutStyle.Setters.Add(tSetter);
            TXTBLKcourseTitle.Text = "第" + Constants.Week + "周";
            ShowCourse(); 
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            STRBDpopin.Begin();

        }
               
        private void BTNcourseRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshCourse();
        }
        public void RefreshCourse()
        {           
            PRGRScourse.ProgressStart();
            //this.IsEnabled = false;
            if (Constants.CourseUseElective) GetCourseByElective();
            else GetCourseByDean();
        }

        private async void GetCourseByDean()
        {            
            BitmapImage bmp = new BitmapImage();
            Stream stream = await WebConnection.Connect_for_stream("http://dean.pku.edu.cn/student/yanzheng.php?act=init");
            if (stream == null)
            {
                Constants.BoxPage.ShowMessage("获取验证码失败！");
                return;
            }
            var ran_stream = await Util.StreamToRandomAccessStream(stream);
            bmp.SetSource(ran_stream);
            IMGverify.Source = bmp;
            PRGRScourse.ProgressStart();
            ContentDialogResult res = await DLGverify.ShowAsync();
            if (res != ContentDialogResult.Primary)
            {
                this.IsEnabled = true;
                PRGRScourse.ProgressEnd();
                return;
            }
            String str = await Dean.get_session_id(VerifyCode);
            if (str == "") return;            
            /**
             * 下面这段注释的代码可能是用来获取自选课程的，不是dean
             */
            //List<Parameters> list = new List<Parameters>();
            //list.Add(new Parameters("token", Constants.token));
            //list.Add(new Parameters("phpsessid", str));
            //Parameters parameter = await WebConnection.Connect(Constants.domain + "/services/pkuhelper/course.php", list);
            Parameters parameter = await WebConnection.Connect("http://dean.pku.edu.cn/student/newXkInfo_1105.php?PHPSESSID=" + str, null);

            if (parameter != null && parameter.name == "200")
            {
                CourseUtil.DecodeDeanHtml(parameter.value);
                CourseUtil.SaveCourses("DeanCourses", CourseUtil.DeanCourses);
                if (Constants.CourseUseCustom) await CourseUtil.GetCustomCourses(str);
                ShowCourse();
                this.IsEnabled = true;
                PRGRScourse.ProgressEnd();
            }
            else
            {
                Util.DealWithDisconnect(parameter);
                this.IsEnabled = true;
                PRGRScourse.ProgressEnd();
            }
        }

        private async void GetCourseByElective()
        {
            Parameters parameter;
            List<Parameters> list = new List<Parameters>();
            list.Add(new Parameters("appid", "syllabus"));
            list.Add(new Parameters("userName", Constants.username));
            list.Add(new Parameters("password", Constants.password));
            list.Add(new Parameters("randCode", "0"));
            list.Add(new Parameters("redirUrl",
                    "http://elective.pku.edu.cn:80/elective2008/agent4Iaaa.jsp/../ssoLogin.do"));
            parameter = await WebConnection.Connect("https://iaaa.pku.edu.cn/iaaa/oauthlogin.do", list);
            if (!"200".Equals(parameter.name))
            {
                Util.DealWithDisconnect(parameter);
                this.IsEnabled = true;
                PRGRScourse.ProgressEnd();
                return;
            }
            else
            {
                JsonObject jsonObject = JsonObject.Parse(parameter.value);
                Boolean success = jsonObject.GetNamedBoolean("success");

                if (success)
                {
                    String token = jsonObject.GetNamedString("token");
                    GettingCookie(token);
                    return;
                }
                else
                {
                    Constants.BoxPage.ShowMessage("用户名或密码错误");
                }
            }
        }

        private async void GettingCookie(string token)
        {
            Parameters parameter = await WebConnection.Connect("http://elective.pku.edu.cn/elective2008/ssoLogin.do?token=" + token, null);
            if (!"200".Equals(parameter.name))
            {
                Util.DealWithDisconnect(parameter);
                this.IsEnabled = true;
                PRGRScourse.ProgressEnd();
                return;
            }
            else ConnectCourse();
        }

        private async void ConnectCourse()
        {
            Parameters parameter = await WebConnection.Connect("http://elective.pku.edu.cn/elective2008/edu/pku/stu/elective/controller/electiveWork/showResults.do", null);
            if (!"200".Equals(parameter.name))
            {
                Util.DealWithDisconnect(parameter);
                this.IsEnabled = true;
                PRGRScourse.ProgressEnd();
                return;
            }
            try
            {
                CourseUtil.DecodeElectiveHtml(parameter.value);
                if (Constants.CourseUseCustom) await CourseUtil.GetCustomCourses("(null)");
            }
            catch(Exception e)
            {
                Debug.WriteLine("fail to decode elective HTML! at" + e.StackTrace);
            }
            ShowCourse();
        }
        private void ShowCourse()
        {
            try
            {
                if (Constants.CourseUseElective)
                {
                    if(Constants.CourseUseCustom) CourseUtil.CourseCover(CourseUtil.ElectiveCourses);
                    ShowCourse(CourseUtil.ElectiveCourses);                    
                }
                else
                {
                    if (Constants.CourseUseCustom) CourseUtil.CourseCover(CourseUtil.DeanCourses);
                    ShowCourse(CourseUtil.DeanCourses);
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine("Show course error! at" + e.StackTrace);
            }
        }

        private void ShowCourse(Dictionary<String,CourseInfo> dic)
        {
            foreach(var ctrl in controls)
            {
                GRIDcourseTable.Children.Remove(ctrl);
            }
            controls.Clear();
            AddCourse(dic);
        }

        private void CourseTapped(object sender,TappedRoutedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element != null)
            {
                Flyout.ShowAttachedFlyout(element);
            }
        }
        private async void AddCourse(Dictionary<String,CourseInfo> dic)
        {
            if (dic.Count <= 0) return;
            foreach (var pair in dic)
            {
                var courseInfo = pair.Value;
                foreach (var courseTime in courseInfo.CourseTimes)
                {
                    try
                    {
                        if (!courseTime.WeekFit(Constants.Week)) continue;
                        CourseInfoControl tempControl = new CourseInfoControl(courseInfo);
                        
                        Grid.SetColumn(tempControl, courseTime.Day);
                        Grid.SetRow(tempControl, courseTime.Start);
                        Grid.SetRowSpan(tempControl, courseTime.TimeSpan);
                        controls.Add(tempControl);
                        GRIDcourseTable.Children.Add(tempControl);
                        tempControl.SetScale0();
                        Flyout fly = new Flyout();
                        fly.FlyoutPresenterStyle = FlyoutStyle;
                        
                        StackPanel tempPanel = new StackPanel();
                        TextBlock tempTitle = new TextBlock(), tempContent = new TextBlock();
                        tempTitle.FontSize = 20;
                        tempContent.FontSize = 15;
                        tempTitle.Text = "课程详细信息";
                        tempContent.Text = "课程名称：" + courseInfo.CourseName +
                                            "\n上课地点：" + courseInfo.Location +
                                            "\n考试时间：" + courseInfo.Exam +
                                            "\n考试地点：" + courseInfo.ExamPlace +
                                            "\n其它信息：" + courseInfo.Others;
                        
                        tempPanel.Children.Add(tempTitle);
                        tempPanel.Children.Add(tempContent);
                        fly.Content = tempPanel;
                        FlyoutBase.SetAttachedFlyout(tempControl, fly);
                        tempControl.Tapped += CourseTapped;
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("fail to show a course! at" + e.StackTrace+"\nthe course's name is" + courseInfo.CourseName);
                    }
                }
            }
            
            
            if (Constants.CourseUseCustom)
            {
                try
                {
                    foreach (var pair in CourseUtil.CustomCourses)
                    {
                        var courseInfo = pair.Value;
                        foreach (var courseTime in courseInfo.CourseTimes)
                        {
                            if (!courseTime.WeekFit(Constants.Week)) continue;
                            CourseInfoControl tempControl = new CourseInfoControl(courseInfo);
                            controls.Add(tempControl);
                            GRIDcourseTable.Children.Add(tempControl);
                            Grid.SetColumn(tempControl, courseTime.Day);
                            Grid.SetRow(tempControl, courseTime.Start);
                            Grid.SetRowSpan(tempControl, courseTime.TimeSpan);
                            tempControl.SetScale0();
                            Flyout fly = new Flyout();
                            fly.FlyoutPresenterStyle = FlyoutStyle;
                            StackPanel tempPanel = new StackPanel();
                            TextBlock tempTitle = new TextBlock(), tempContent = new TextBlock();
                            tempTitle.FontSize = 20;
                            tempContent.FontSize = 15;
                            tempTitle.Text = "课程详细信息";
                            tempContent.Text = "课程名称：" + courseInfo.CourseName +
                                                "\n上课地点：" + courseInfo.Location +
                                                "\n考试时间：" + courseInfo.Exam +
                                                "\n考试地点：" + courseInfo.ExamPlace +
                                                "\n其它信息：" + courseInfo.Others;
                            
                            tempPanel.Children.Add(tempTitle);
                            tempPanel.Children.Add(tempContent);
                            fly.Content = tempPanel;
                            FlyoutBase.SetAttachedFlyout(tempControl, fly);
                            tempControl.Tapped += CourseTapped;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("fail to show a course! at" + e.StackTrace);
                }
            }
            this.IsEnabled = true;
            PRGRScourse.ProgressEnd();
            foreach (var ctrl in controls)
            {
                ctrl.CoursePopin();
                await Task.Delay(50);
            }
        }

        private void DLGverify_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            VerifyCode = TXTBXinput.Text.Trim();
        }

        private void DLGverify_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            TXTBXinput.ClearValue(TextBox.TextProperty);
        }

        private void ITEMaddCustomCourse_Click(object sender, RoutedEventArgs e)
        {
            FindName(nameof(FRAMEcourseInner));
            FRAMEcourseInner.Visibility = Visibility.Visible;
            FRAMEcourseInner.Navigate(typeof(AddCustomCoursePage));
        }

        
    }
}
