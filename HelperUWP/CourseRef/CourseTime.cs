using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperUWP.CourseRef
{
    public enum week_kind
    {
        all, odd, even ,none
    }
    public class CourseTime
    {
        public CourseTime(int _day, String start_end, String _week_kind)
        {
            Day = _day;
            int split = start_end.IndexOf('-');
            String str_start = start_end.Substring(0, split);
            String str_end = start_end.Substring(split + 1);
            try
            {
                Start = int.Parse(str_start);
                End = int.Parse(str_end);
            }
            catch
            {
                Start = -2;
                End = -1;
            }
            TimeSpan = End - Start + 1;
            if (_week_kind.Equals("all")) Week = week_kind.all;
            else if (_week_kind.Equals("odd")) Week = week_kind.odd;
            else Week = week_kind.even;
        }
        public CourseTime(int _day, String start_end, week_kind _week_kind)
        {
            Day = _day;
            int split = start_end.IndexOf('-');
            String str_start = start_end.Substring(0, split);
            String str_end = start_end.Substring(split + 1);
            try
            {
                Start = int.Parse(str_start);
                End = int.Parse(str_end);
            }
            catch
            {
                Start = 2;
                End = 1;
            }
            TimeSpan = End - Start + 1;
            Week = _week_kind;
        }
        public CourseTime(int _day,int _start,int _timeSpan,week_kind _week_kind)
        {
            Day = _day;
            Start = _start;
            TimeSpan = _timeSpan;
            End = Start + TimeSpan - 1;
            Week = _week_kind;
        }
        public String GetWeekWord()
        {
            switch (Week)
            {
                case week_kind.all:
                    {
                        return "每周";
                    }

                case week_kind.odd:
                    {
                        return "单周";
                    }
                case week_kind.even:
                    {
                        return "双周";
                    }

                default:
                    return "每周";
            }
        }
        public String WeekWord
        {
            get
            {
                switch (Week)
                {
                    case week_kind.all:
                        {
                            return "all";
                        }

                    case week_kind.odd:
                        {
                            return "odd";
                        }
                    case week_kind.even:
                        {
                            return "even";
                        }

                    default:
                        return "all";
                }
            }
        }
        public bool WeekFit(int week)
        {
            if (Week == week_kind.all) return true;
            else if (Week == week_kind.odd && week % 2 != 0) return true;
            else if (Week == week_kind.even && week % 2 == 0) return true;
            else return false;
        }
        public int Day { get; set; }
        public int Start { get; set; }
        public int End { set; get; }
        public int TimeSpan { get; set; }
        public week_kind Week { get; set; }
    }
}
