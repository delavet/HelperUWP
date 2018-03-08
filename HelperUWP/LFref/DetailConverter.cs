using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace HelperUWP.LFref
{
    public sealed class DetailConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            String d = (String)value;
            String ret = null;
            if (d.Length > 20) ret = d.Substring(0, 20) + "...";
            else ret = d;
            return ret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
