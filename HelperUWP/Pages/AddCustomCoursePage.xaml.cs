using HelperUWP.CourseRef;
using HelperUWP.Lib;
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
    public sealed partial class AddCustomCoursePage : Page
    {
        private Boolean deleteChecked = false;
        
        public AddCustomCoursePage()
        {
            this.InitializeComponent();
            BTNdeleteCustomCourse.IsChecked = false;
            GetCustomCourses();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {                                
            STRBDpopin.Begin();            
        }

        private async void GetCustomCourses()
        {
            PRGRSadd.ProgressStart();
            CourseUtil.CustomCourses.Clear();
            Parameters parameter = await WebConnection.Connect(Constants.domain + "/services/pkuhelper/course.php?token=" + Constants.token+ "&?=" + DateTime.Now.Millisecond, null);
            if (!"200".Equals(parameter.name))
            {
                Util.DealWithDisconnect(parameter);
                PRGRSadd.ProgressEnd();
                return;
            }
            CourseUtil.ParseCourseJson(parameter.value, ref CourseUtil.CustomCourses);
            CourseUtil.SaveCourses("CustomCourses", CourseUtil.CustomCourses);
            CourseUtil.SimpleCustomInfo.Clear();
            foreach (var info in CourseUtil.CustomCourses)
            {
                CourseUtil.SimpleCustomInfo.Add(new SimpleCustomCourseInfo(info.Key));
            }
            LSTVWcustomCourse.ItemsSource = CourseUtil.SimpleCustomInfo;
            PRGRSadd.ProgressEnd();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            LSTVWcustomCourse.SelectionMode = ListViewSelectionMode.None;
            BTNrealDeleteCustomCourse.Visibility = Visibility.Collapsed;
            BTNdeleteCustomCourse.IsChecked = false;
        }
        
        private void BTNexitAddCustomCourse_Click(object sender, RoutedEventArgs e)
        {
            STRBDpopout.Begin();          
        }

        private void STRBDpopout_Completed(object sender, object e)
        {
            CourseUtil.InitCustom();
            this.Frame.Navigate(typeof(BlankPage));
        }

        private void GRIDroot_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (e.OriginalSource == GRIDroot) STRBDpopout.Begin();
        }

        private async void BTNsaveCustomCourse_Click(object sender, RoutedEventArgs e)
        {
            PRGRSadd.ProgressStart();
            List<Parameters> l = new List<Parameters>();
            l.Add(new Parameters("token", Constants.token));
            l.Add(new Parameters("operation", "set"));
            l.Add(new Parameters("content", CourseUtil.ToStandardJsonStr(CourseUtil.CustomCourses)));
            Parameters parameter = await WebConnection.Connect(Constants.domain + "/services/course.php", l);
            if (!parameter.name.Equals("200"))
            {
                Util.DealWithDisconnect(parameter);
                PRGRSadd.ProgressEnd();
                return;
            }
            try
            {
                JsonObject json = JsonObject.Parse(parameter.value);
                int code;
                try
                {
                    code = (int)json.GetNamedNumber("code");
                }
                catch
                {
                    code = int.Parse(json.GetNamedString("code"));
                }
                if (!(code == 0)) return;
                CourseUtil.SaveCourses("CustomCourses", CourseUtil.CustomCourses);
                CourseUtil.currentCoursePage.RefreshCourse();
                PRGRSadd.ProgressEnd();
                STRBDpopout.Begin();
            }
            catch(Exception ex)
            {
                Debug.WriteLine("fail to update custom courses! at" + ex.StackTrace);
            }
        }

        private async void BTNaddCustomCourse_Click(object sender, RoutedEventArgs e)
        {
            STRBDswitch.Begin();
            await Task.Delay(100);
            FindName(nameof(FRAMEedit));
            FRAMEedit.Navigate(typeof(CourseEditPage),null);     
        }

        private void BTNdeleteCustomCourse_Click(object sender, RoutedEventArgs e)
        {
            deleteChecked = !deleteChecked;
            if (deleteChecked)
            {
                LSTVWcustomCourse.SelectionMode = ListViewSelectionMode.Multiple;
                BTNrealDeleteCustomCourse.Visibility = Visibility.Visible;
            }
            else
            {
                LSTVWcustomCourse.SelectionMode = ListViewSelectionMode.None;
                BTNrealDeleteCustomCourse.Visibility = Visibility.Collapsed;

            }
        }

        private void LSTVWcustomCourse_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!(LSTVWcustomCourse.SelectionMode == ListViewSelectionMode.None)) return;
            SimpleCustomCourseInfo t = e.ClickedItem as SimpleCustomCourseInfo;
            FindName(nameof(FRAMEedit));
            FRAMEedit.Navigate(typeof(CourseEditPage), t.Name);
        }

        private void BTNrealDeleteCustomCourse_Click(object sender, RoutedEventArgs e)
        {
            List<String> deletes = new List<string>();
            foreach(var info in LSTVWcustomCourse.SelectedItems)
            {
                deletes.Add(((SimpleCustomCourseInfo)info).Name);
            }
            foreach(String name in deletes)
            {
                CourseUtil.SimpleCustomInfo.Delete(name);
                CourseUtil.CustomCourses.Remove(name);
            }
        }

        
    }
}
