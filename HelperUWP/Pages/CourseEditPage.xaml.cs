using HelperUWP.Controls;
using HelperUWP.CourseRef;
using HelperUWP.Lib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public sealed partial class CourseEditPage : Page
    {
        private enum EditMode
        {
            add,edit
        }
        private EditMode PageMode;
        private CourseChooseBlock[,] blocks = new CourseChooseBlock[8, 13];
        private String defaultName = "";
        public CourseEditPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Disabled;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter == null||!(e.Parameter is String)||((e.Parameter is String)&&((e.Parameter as String)==""))) PageMode = EditMode.add;
            else
            {
                String str = e.Parameter as String;
                defaultName = str;
                PageMode = EditMode.edit;
                TXTBXname.Text = str;
                TXTBXlocation.Text = CourseUtil.CustomCourses[str].Location;
                TXTBXname.IsEnabled = false;
                AddCustomCourseHint.Text = "编辑自选课程";
            }
            InitBlock();
            STRBDpopin.Begin();
        }
        private void InitBlock()
        {
            for(int i = 1; i <= 7; i++)
            {
                for(int j = 1; j <= 12; j++)
                {
                    var block = new CourseChooseBlock(i, j);
                    GRIDcourseTable.Children.Add(block);
                    Grid.SetColumn(block, i);
                    Grid.SetRow(block, j);
                    blocks[i, j] = block;
                }
            }
            if (PageMode == EditMode.edit)
            {
                foreach(TimePoint point in CourseUtil.CustomCourses[defaultName].TimePoints)
                {
                    try
                    {
                        blocks[point.DayOfWeek, point.ClassOfDay].Kind = point.WeekKind;
                    }
                    catch
                    {
                        Debug.WriteLine("fail to change a course block!");
                    }
                }
            }
        }

        private void BTNback_Click(object sender, RoutedEventArgs e)
        {
            STRBDpopout.Begin();           
        }

        private void STRBDpopout_Completed(object sender, object e)
        {
            this.Frame.Navigate(typeof(BlankPage));
        }

        private void BTNconfirm_Click(object sender, RoutedEventArgs e)
        {
            if (PageMode == EditMode.edit)
            {
                try
                {
                    CourseUtil.CustomCourses[defaultName].Location = TXTBXlocation.Text;
                    CourseUtil.CustomCourses[defaultName].TimePoints.Clear();
                    foreach (var block in blocks)
                    {
                        if (block == null) continue;
                        if (block.Kind == week_kind.none) continue;
                        CourseUtil.CustomCourses[defaultName].TimePoints.Add(new TimePoint(block.Day, block.ClassOfDay, block.Kind));
                    }
                    CourseUtil.CustomCourses[defaultName].Merge();
                }
                catch {
                    Debug.WriteLine("fail to edit a custom course!"+defaultName);
                }

            }else
            {
                if (TXTBXname.Text == null || TXTBXname.Text == "")
                {
                    Constants.BoxPage.ShowMessage("课程名不能为空！");
                    return;
                }
                defaultName = TXTBXname.Text;
                String locat = TXTBXlocation.Text;
                CourseInfo info = new CourseInfo(defaultName, locat, "", CourseKind.Custom);
                foreach (var block in blocks)
                {
                    if (block == null) continue;
                    if (block.Kind == week_kind.none) continue;
                    info.TimePoints.Add(new TimePoint(block.Day, block.ClassOfDay, block.Kind));
                }
                info.Merge();
                CourseUtil.SimpleCustomInfo.Add(new SimpleCustomCourseInfo(defaultName));
                CourseUtil.CustomCourses.Add(defaultName,info);
            }
            STRBDpopout.Begin();
        }

        private void GRIDroot_Tapped(object sender, TappedRoutedEventArgs e)
        {       
            if(e.OriginalSource==GRIDroot) STRBDpopout.Begin();
        }
    }
}
