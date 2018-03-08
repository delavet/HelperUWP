using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace HelperUWP.ChatRef
{
    class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool hasnew = (bool)value;
            if (hasnew) return new SolidColorBrush(new Color() { A = 0xFF, R = 0x00, G = 0xA1, B = 0xF1 });
            else return new SolidColorBrush(new Color() { A = 0, R = 0, G = 0, B = 0 });
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
