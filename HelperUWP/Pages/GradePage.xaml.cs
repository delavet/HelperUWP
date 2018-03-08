using HelperUWP.InfoRef;
using HelperUWP.Lib;
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

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace HelperUWP.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class GradePage : Page
    {
        List<Course> all_courses = null;
        List<Semester> semesters = null;
        String total_weight, avg_gpa;
        public GradePage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            STRBDpopin.Begin();
            if (e.Parameter != null && e.Parameter is String)
            {
                handle_json(e.Parameter as String);
                show_grade();
            }
        }

        private void show_grade()
        {
            if (semesters == null) return;
            all_courses = new List<Course>();
            TXTBLKweight.Text = "共修学分：" + total_weight;
            TXTBLKgpa.Text = "平均gpa:" + avg_gpa;
            foreach (var temp_sem in semesters)
            {
                foreach (var temp_course in temp_sem.courses)
                {
                    all_courses.Add(temp_course);
                }
            }
            var groups = from n in all_courses
                         group n by n.semester;
            cvs.Source = groups;
        }
        private void handle_json(String json_str)
        {
            try
            {
                JsonObject grade_info = JsonObject.Parse(json_str);
                int code = (int)grade_info.GetNamedNumber("code");
                if (code != 0)
                {
                    Constants.BoxPage.ShowMessage("成绩解析失败！");
                    return;
                }
                total_weight = grade_info.GetNamedString("total", "unknown");
                avg_gpa = grade_info.GetNamedString("avggpa", "unknown");
                generate_semesters(grade_info.GetNamedArray("gpas", new JsonArray()), grade_info.GetNamedArray("courses", new JsonArray()));
            }
            catch (Exception e)
            {
                Constants.BoxPage.ShowMessage(e.StackTrace,"成绩解析失败！");
            }
        }

        private void generate_semesters(JsonArray gpas, JsonArray courses)
        {
            try
            {
                semesters = new List<Semester>();
                foreach (var temp_val in gpas)
                {
                    JsonObject term_obj = temp_val.GetObject();
                    String year = term_obj.GetNamedString("year", "unknown");
                    String term = term_obj.GetNamedString("term", "unknown");
                    String gpa = term_obj.GetNamedString("gpa", "unknown");
                    Semester temp_sem = new Semester(year, term, gpa, false);
                    semesters.Add(temp_sem);

                }
                foreach (var temp_course in courses)
                {
                    JsonObject course_obj = temp_course.GetObject();
                    String name = course_obj.GetNamedString("name", "unknown");
                    String fullName = course_obj.GetNamedString("fullName", "unknown");
                    String year = course_obj.GetNamedString("year", "unknown");
                    String term = course_obj.GetNamedString("term", "unknown");
                    String weight = course_obj.GetNamedString("weight", "unknown");
                    String grade = course_obj.GetNamedString("grade", "unknown");
                    String delta = course_obj.GetNamedString("delta", "unknown");
                    String gpa = course_obj.GetNamedString("gpa", "unknown");
                    String accurate = course_obj.GetNamedString("accurate", "unknown");
                    String type = course_obj.GetNamedString("type", "unknown");
                    foreach (var temp_sem in semesters)
                    {
                        if (temp_sem.isThisSemester(year, term))
                        {
                            temp_sem.addCourse(name, fullName,
                            type, weight, grade, delta, accurate, gpa);
                            break;
                        }
                    }
                }
                foreach (var sem in semesters)
                {
                    sem.calWeight();
                }
            }
            catch (Exception e)
            {
                Constants.BoxPage.ShowMessage(e.StackTrace,"解析成绩信息失败");
            }
        }

        private void BTNback_Click(object sender, RoutedEventArgs e)
        {
            InfoUtil.BackRequest();
        }

        private void grade_list_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!(e.ClickedItem is Course)) return;
            Course detail_course = e.ClickedItem as Course;
            String detail_str = "课程名称：" + detail_course.fullname
                + "\n学分：" + detail_course.weight
                + "\n成绩：" + detail_course.grade
                + "\n绩点：" + detail_course.gpa
                + "\n类型：" + detail_course.type;
            Constants.BoxPage.ShowMessagePopin(detail_str, "课程详细信息");
        }
    }
}
