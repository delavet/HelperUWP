using HelperUWP.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace HelperUWP.ChatRef
{
    public class TimeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                ChatDetailInfo info = value as ChatDetailInfo;
                if (!info.ShowTime) return "";
                long stamp = info.Timestamp;
                return Util.PassedTime(Util.GetTimeStamp(DateTime.Now), stamp);
            }
            catch
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
