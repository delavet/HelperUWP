using HelperUWP.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace HelperUWP.LFref
{
    public sealed class TimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            long t_time = (long)value;
            String time_str = Util.TimestampToDateTime(t_time).ToString("yyyy年MM月dd日");
            return time_str;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
