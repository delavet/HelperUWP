using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace HelperUWP.CourseRef
{
    public class TimePoint {
        public int DayOfWeek, ClassOfDay;
        public week_kind WeekKind;
        public TimePoint(int _day,int _class,week_kind kind = week_kind.all)
        {
            DayOfWeek = _day;
            ClassOfDay = _class;
            WeekKind = kind;
        }
        public override bool Equals(object obj)
        {
            if (!(obj is TimePoint)) return false;
            TimePoint t = (TimePoint)obj;
            return t.ClassOfDay == ClassOfDay && t.DayOfWeek == DayOfWeek && t.WeekKind == WeekKind;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }


    public enum CourseKind
    {
        Elective,Dean,Custom
    }
    public class CourseInfo
    {
        public CourseInfo(String _course_name, String _location, String _exam,CourseKind kind,String _others="",String examPlace="")
        {
            CourseName = new String(_course_name.ToCharArray());
            Location = new String(_location.ToCharArray());
            Exam = new String(_exam.ToCharArray());
            Others = new string(_others.ToCharArray());
            ExamPlace = new string(examPlace.ToCharArray());
            CourseTimes = new List<CourseTime>();
            TimePoints = new List<TimePoint>();                       
        }
        public void Merge()
        {
            CourseTimes.Clear();
            TimePoints.Sort((a, b) =>
            {
                if (a.DayOfWeek != b.DayOfWeek) return Math.Sign(a.DayOfWeek - b.DayOfWeek);
                return Math.Sign(a.ClassOfDay - b.ClassOfDay);
            });
            ReMerge();
        }
        public void ReMerge()
        {
            CourseTimes.Clear();            
            int timePointCount = TimePoints.Count;
            int tempSpan = 0;
            TimePoint tempPoint = new TimePoint(-1, -1);
            for (int i = 0; i < timePointCount; i++)
            {
                if (TimePoints[i].DayOfWeek == tempPoint.DayOfWeek && TimePoints[i].ClassOfDay == (tempPoint.ClassOfDay + tempSpan) && TimePoints[i].WeekKind == tempPoint.WeekKind)
                {
                    tempSpan++;
                }
                else
                {
                    if (tempPoint.DayOfWeek != -1)
                        CourseTimes.Add(new CourseTime(tempPoint.DayOfWeek, tempPoint.ClassOfDay, tempSpan, tempPoint.WeekKind));
                    tempPoint = TimePoints[i];
                    tempSpan = 1;
                }
            }
            CourseTimes.Add(new CourseTime(tempPoint.DayOfWeek, tempPoint.ClassOfDay, tempSpan, tempPoint.WeekKind));
        }

        public void RemoveTimePoint(int day,int classNum)
        {
            int i = 0;
            for (; i < TimePoints.Count; i++)
            {
                if (TimePoints[i].DayOfWeek == day && TimePoints[i].ClassOfDay == classNum)
                {
                    TimePoints.RemoveAt(i);
                    break;
                }
            }
            ReMerge();
        }

        public void AddTime(CourseTime time)
        {
            CourseTimes.Add(time);
            for(int i = time.Start; i <= time.End; i++)
            {
                TimePoints.Add(new TimePoint(time.Day, i, time.Week));
            }
        }

        public CourseKind kind { get; set; }
        public String CourseName { get; set; }
        public String Location { get; set; }
        private String examStr;
        private DateTime examDate;
        public String Exam
        {
            get
            {
                String ret = "";
                try
                {
                    ret = examDate.GetDateTimeFormats('D')[0].ToString();
                    if (ret.StartsWith("0")) ret = examStr;
                }
                catch
                {
                    ret = examStr;
                }
                return ret;
            }
            set
            {
                examStr = value;
                try
                {
                    examDate = DateTime.ParseExact(value, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                }
                catch
                {
                    
                }
            }
        }
        public String ExamPlace { get; set; }
        public String Others { get; set; }
        public List<CourseTime> CourseTimes;
        public List<TimePoint> TimePoints;
    }
}
