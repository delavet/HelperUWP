using HelperUWP.Lib;
using HelperUWP.Lib.Storage;
using HelperUWP.Lib.Web;
using HelperUWP.Pages;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace HelperUWP.CourseRef
{
    class CourseUtil
    {
        public static Dictionary<String, CourseInfo> ElectiveCourses = new Dictionary<string, CourseInfo>();
        public static Dictionary<String, CourseInfo> CustomCourses = new Dictionary<string, CourseInfo>();
        public static Dictionary<String, CourseInfo> DeanCourses = new Dictionary<string, CourseInfo>();
        public static SimpleCourseList SimpleCustomInfo = new SimpleCourseList();
        public static CoursePage currentCoursePage = null;
        public static void InitCourses()
        {
           
            try
            {
                if (Constants.CourseUseCustom) InitCustom();
                if (Constants.CourseUseElective) InitElective();
                else InitDean();
            }
            catch(Exception e)
            {
                Debug.WriteLine("fail to load courses:" + e.StackTrace);
            }
        }
        public static void InitCustom()
        {
            CustomCourses.Clear();
            String jsonStr = Editor.getString("CustomCourses");
            ParseCourseJson(jsonStr, ref CustomCourses);
        }

        public static async Task GetCustomCourses(String phpid)
        {
            CustomCourses.Clear(); 
            Parameters parameter = await WebConnection.Connect(Constants.domain + "/services/pkuhelper/course.php?token=" + Constants.token+ "&phpsessid="+phpid + "&?=" + DateTime.Now.Millisecond, null);
            if (!"200".Equals(parameter.name)) Util.DealWithDisconnect(parameter);
            CourseUtil.ParseCourseJson(parameter.value, ref CourseUtil.CustomCourses);
            CourseUtil.SaveCourses("CustomCourses", CourseUtil.CustomCourses);
        }

        private static void InitDean()
        {
            DeanCourses.Clear();
            String jsonStr = Editor.getString("DeanCourses");
            ParseCourseJson(jsonStr, ref DeanCourses);
        }

        private static void InitElective()
        {
            ElectiveCourses.Clear();
            String jsonStr = Editor.getString("ElectiveCourses");
            ParseCourseJson(jsonStr, ref ElectiveCourses);
        }

        public static void ParseCourseJson(String jsonStr,ref Dictionary<String,CourseInfo> dic)
        {
            if (dic == null) dic = new Dictionary<string, CourseInfo>();
            try
            {
                JsonObject json = JsonObject.Parse(jsonStr);
                try
                {
                    String returnCode = json.GetNamedString("code");
                    if (returnCode != "0") return;
                }
                catch
                {
                    int returnCodeNum = (int)json.GetNamedNumber("code");
                    if (returnCodeNum != 0) return;
                }
                JsonArray courses = json.GetNamedArray("courses", null);
                if (courses == null) return;
                dic.Clear();
                JsonArray temp_times;
                String temp_coursename;
                String temp_exam;
                String temp_location;
                String temp_startend;
                String temp_week_kind;
                String temp_other;
                String temp_examPlace;
                int temp_day;
                foreach (var temp_value in courses)
                {
                    try
                    {
                        JsonObject temp_json = temp_value.GetObject();
                        temp_coursename = temp_json.GetNamedString("courseName", "unknown");
                        temp_exam = temp_json.GetNamedString("exam", "unknown exam information");
                        temp_location = temp_json.GetNamedString("location", "unknown location");
                        temp_times = temp_json.GetNamedArray("times", null);
                        temp_other = temp_json.GetNamedString("other", "");
                        temp_examPlace = temp_json.GetNamedString("examPlace", "");
                        dic.Add(temp_coursename, new CourseInfo(temp_coursename, temp_location, temp_exam, CourseKind.Elective, temp_other)); ;
                        foreach (var temp_time in temp_times)
                        {
                            JsonObject t = temp_time.GetObject();
                            try
                            {
                                temp_day = (int)t.GetNamedNumber("day");
                            }
                            catch
                            {
                                temp_day = int.Parse(t.GetNamedString("day"));
                            }
                            temp_startend = t.GetNamedString("num");
                            temp_week_kind = t.GetNamedString("week");
                            dic[temp_coursename].AddTime(new CourseTime(temp_day, temp_startend, temp_week_kind));
                        }
                    }
                    catch
                    {
                        
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("fail to parse json! " + e.StackTrace);
            }
        }
        public static void DecodeElectiveHtml(String html)
        {
            ElectiveCourses.Clear();
            try
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                HtmlNode table = doc.GetElementbyId("classAssignment");
                HtmlNodeCollection trs = table.ChildNodes;
                int row = -1;
                foreach (var tr in trs)
                {
                    if (!tr.OuterHtml.Contains("</tr>")) continue;
                    ++row;
                    if (row == 0) continue;
                    int col = -1;
                    HtmlNodeCollection tds = tr.ChildNodes;
                    foreach (var td in tds)
                    {
                        if (!td.OuterHtml.Contains("</td>")) continue;
                        ++col;
                        if (col == 0) continue;
                        var attributes = td.Attributes;
                        if (attributes.AttributesWithName("style").Count() < 1) continue;
                        HtmlNode span = td.Element("span");
                        String innerString = span.InnerHtml;
                        week_kind tempKind = week_kind.all;
                        if (innerString.Contains("单周"))
                        {
                            tempKind = week_kind.odd;
                            innerString = innerString.Remove(innerString.IndexOf("单周"), 2);
                        }
                        else if (innerString.Contains("双周"))
                        {
                            tempKind = week_kind.even;
                            innerString = innerString.Remove(innerString.IndexOf("双周"), 2);
                        }
                        else if (innerString.Contains("每周"))
                        {
                            innerString = innerString.Remove(innerString.IndexOf("每周"), 2);
                        }
                        String[] strings = innerString.Split(new String[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries);
                        int k = 0;
                        while (k < strings.Length && strings[k].Replace(" ","").Length == 0) ++k;
                        if (k == strings.Length) continue;
                        String tempCourseName = strings[k].Replace(" ","");
                        if (ElectiveCourses.ContainsKey(tempCourseName))
                        {
                            ElectiveCourses[tempCourseName].TimePoints.Add(new TimePoint(col, row, tempKind));
                            continue;
                        }
                        ++k;
                        while (k < strings.Length && strings[k].Replace(" ","").Length == 0) ++k;
                        if (k == strings.Length) continue;
                        String where = strings[k].Replace(" ","");
                        String tempLocation = "未知地点", tempOther = "", tempExam = "";
                        int pos = where.IndexOf(')');
                        if (pos != -1)
                        {
                            tempLocation = where.Substring(1, pos - 1);
                            if (tempLocation.Contains("备注"))
                            {
                                tempOther = tempLocation;
                                tempLocation = "未知地点";
                            }
                        }
                        ++k;
                        while (k < strings.Length)
                        {
                            if (strings[k].Contains("考试时间："))
                            {
                                int examPos = strings[k].IndexOf('：');
                                if (examPos != -1)
                                {
                                    tempExam = strings[k].Substring(examPos + 1, 8);
                                }
                            }
                            else
                            {
                                tempOther += "\n" + strings[k];
                            }
                            ++k;
                        }
                        ElectiveCourses.Add(tempCourseName, new CourseInfo(tempCourseName, tempLocation, tempExam, CourseKind.Elective, tempOther));
                        ElectiveCourses[tempCourseName].TimePoints.Add(new TimePoint(col, row, tempKind));
                    }
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine("fail to decode elective's HTML! at" + e.StackTrace);
            }
            foreach(var course in ElectiveCourses)
            {
                course.Value.Merge();
            }
            SaveCourses("ElectiveCourses", ElectiveCourses);            
        }
        private static HtmlNode FindDeanTable(HtmlNode OuterTable)
        {
            if (OuterTable.OuterHtml.StartsWith("<table")) return OuterTable;
            foreach(HtmlNode node in OuterTable.ChildNodes)
            {
                if (node.OuterHtml.Contains("</table>")) return FindDeanTable(node); 
            }
            return null;
        }

        public static void DecodeDeanHtml(String html)
        {
            html = html.Trim();
            DeanCourses.Clear();
            try
            {               
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                HtmlNode table = null;

                foreach (var node in doc.DocumentNode.ChildNodes)
                {
                    String a = node.OuterHtml;
                    if (node.OuterHtml.Contains("</table>"))
                    {
                        table = FindDeanTable(node);
                        break;
                    }
                }
                
                
                foreach(var courseNode in table.ChildNodes)
                {
                    String a = courseNode.OuterHtml;
                    try
                    {
                        if (!(courseNode.OuterHtml.EndsWith("</tr>"))) continue;
                        CourseInfo tempInfo = new CourseInfo("", "", "", CourseKind.Dean);
                        int col = 0;
                        foreach (var detail in courseNode.ChildNodes)
                        {
                            if (!detail.OuterHtml.Contains("</td>")) continue;                            
                            String innerString = detail.InnerHtml.Replace(" ","");
                            while (innerString.Contains("&nbsp;")) innerString = innerString.Remove(innerString.IndexOf("&nbsp;"), 6);
                            col++;
                            switch (col)
                            {
                                case 2:
                                    {
                                        tempInfo.CourseName = innerString;
                                    }
                                    break;
                                case 9:
                                    {
                                        String[] whereAndWhen = innerString.Split(new String[] { "<br>" },StringSplitOptions.RemoveEmptyEntries);
                                        foreach(String str in whereAndWhen)
                                        {
                                            if (str.StartsWith("时间"))
                                            {
                                                String when = str.Remove(0, 3);
                                                String[] whens = when.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                                foreach(String timeStr in whens)
                                                {
                                                    if (!timeStr.StartsWith("周")) continue;
                                                    String dayOfWeek = timeStr.Substring(0, 2);
                                                    String classStr = timeStr.Substring(2);
                                                    String weekTypeStr = "";
                                                    if (classStr.EndsWith(")"))
                                                    {
                                                        weekTypeStr = classStr.Substring(classStr.Length - 3);
                                                        classStr = classStr.Remove(classStr.Length - 3);
                                                    }
                                                    tempInfo.AddTime(new CourseTime(DayOfWeekConvert(dayOfWeek), classStr,WeekKindConvert(weekTypeStr)));
                                                }
                                                
                                            }else if (str.StartsWith("地点"))
                                            {
                                                tempInfo.Location = str.Substring(3);
                                            }
                                        }
                                    }break;
                                case 10:
                                    {
                                        String[] whereAndWhen = innerString.Split(new String[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries);
                                        foreach(String str in whereAndWhen)
                                        {
                                            if (str.StartsWith("时间"))
                                            {
                                                try
                                                {
                                                    String timeStr = str.Substring(3, 8);
                                                    tempInfo.Exam = timeStr;
                                                }
                                                catch
                                                {
                                                    tempInfo.Exam = "";
                                                }
                                            } else if (str.StartsWith("地点"))
                                            {
                                                try
                                                {
                                                    String placeStr = str.Substring(3);
                                                    tempInfo.ExamPlace = placeStr;
                                                }
                                                catch
                                                {
                                                    tempInfo.ExamPlace = "";
                                                }
                                            }
                                        }
                                    }
                                    break;
                                case 14:
                                    {
                                        tempInfo.Others = innerString;
                                    }break;
                                default:break;
                            }
                        }
                        DeanCourses.Add(tempInfo.CourseName, tempInfo);
                    }
                    catch
                    {
                        Debug.WriteLine("fail to parse a course, it's HTML is\n" + courseNode.OuterHtml);
                        continue;
                    }
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine("fail to decode Dean's HTML! at" + e.StackTrace);
            }
            SaveCourses("DeanCourses", DeanCourses);
        }

        public static String ToStandardJsonStr(Dictionary<String, CourseInfo> infos)
        {
            JsonArray courseArray = new JsonArray();
            foreach (var a in infos)
            {
                CourseInfo info = a.Value;
                JsonObject obj = new JsonObject();
                obj.Add("courseName", JsonValue.CreateStringValue(info.CourseName));
                
                JsonArray timeArray = new JsonArray();
                foreach (var time in info.CourseTimes)
                {
                    JsonObject t = new JsonObject();
                    t.Add("day", JsonValue.CreateStringValue(time.Day.ToString()));
                    t.Add("num", JsonValue.CreateStringValue(time.Start + "-" + time.End));
                    t.Add("week", JsonValue.CreateStringValue(time.WeekWord));
                    timeArray.Add(t);
                }
                obj.Add("times", timeArray);
                obj.Add("type", JsonValue.CreateStringValue("custom"));
                obj.Add("location", JsonValue.CreateStringValue(info.Location));
                courseArray.Add(obj);
            }
            String str = courseArray.Stringify();
            return str;
        }

        public static void SaveCourses(String storageName,Dictionary<String,CourseInfo> infos)
        {
            JsonObject jsonRoot = new JsonObject();
            jsonRoot.Add("code", JsonValue.CreateStringValue("0"));
            JsonArray courseArray = new JsonArray();
            foreach(var a in infos)
            {
                CourseInfo info = a.Value;
                JsonObject obj = new JsonObject();
                obj.Add("courseName", JsonValue.CreateStringValue(info.CourseName));
                obj.Add("exam", JsonValue.CreateStringValue(info.Exam));
                obj.Add("location", JsonValue.CreateStringValue(info.Location));
                obj.Add("other", JsonValue.CreateStringValue(info.Others));
                obj.Add("examPlace", JsonValue.CreateStringValue(info.ExamPlace));
                JsonArray timeArray = new JsonArray();
                foreach(var time in info.CourseTimes)
                {
                    JsonObject t = new JsonObject();
                    t.Add("day", JsonValue.CreateNumberValue(time.Day));
                    t.Add("num", JsonValue.CreateStringValue(time.Start + "-" + time.End));
                    t.Add("week", JsonValue.CreateStringValue(time.WeekWord));
                    timeArray.Add(t);
                }
                obj.Add("times", timeArray);
                courseArray.Add(obj);
            }
            jsonRoot.Add("courses", courseArray);
            String str = jsonRoot.Stringify();
            Editor.putString(storageName, str);
        }
        /// <summary>
        /// 用自选课程将dean或者elective的课表信息覆盖
        /// 不冲突则不覆盖
        /// </summary>
        /// <param name="dic">被覆盖的课表，elective或者dean</param>
        public static void CourseCover(Dictionary<String,CourseInfo> dic)
        {
            KeyValuePair<String,week_kind>[,] classAssignment = new KeyValuePair<String, week_kind>[8, 13];
            //课程哈希表，方便查找
            for(int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 13; j++)
                {
                    classAssignment[i, j] = new KeyValuePair<string, week_kind>("", week_kind.all);
                }
            }
            foreach(var course in dic)
            {
                foreach(var point in course.Value.TimePoints)
                {
                    classAssignment[point.DayOfWeek, point.ClassOfDay] = new KeyValuePair<string, week_kind>(course.Key, point.WeekKind);
                }
            }
            foreach(var customCourse in CustomCourses)
            {
                foreach(var customPoint in customCourse.Value.TimePoints)
                {
                    if (classAssignment[customPoint.DayOfWeek, customPoint.ClassOfDay].Key == "") continue;
                    if (!WeekKindInterrupt(customPoint.WeekKind, classAssignment[customPoint.DayOfWeek, customPoint.ClassOfDay].Value)) continue;
                    dic[classAssignment[customPoint.DayOfWeek, customPoint.ClassOfDay].Key].RemoveTimePoint(customPoint.DayOfWeek, customPoint.ClassOfDay);
                }
            }
        }

        private static bool WeekKindInterrupt(week_kind w1,week_kind w2)
        {
            if (w1 == week_kind.all || w2 == week_kind.all) return true;
            if (w1 == w2) return true;
            return false;
        }

        public static week_kind WeekKindConvert(String typeStr)
        {
            if (typeStr.Contains("单")) return week_kind.odd;
            else if (typeStr.Contains("双")) return week_kind.even;
            else return week_kind.all;
        }

        public static int DayOfWeekConvert(String day)
        {
            if (day.Contains("周一")) return 1;
            else if (day.Contains("周二")) return 2;
            else if (day.Contains("周三")) return 3;
            else if (day.Contains("周四")) return 4;
            else if (day.Contains("周五")) return 5;
            else if (day.Contains("周六")) return 6;
            else if (day.Contains("周日")) return 7;
            else return 7;
        }
    }
}
