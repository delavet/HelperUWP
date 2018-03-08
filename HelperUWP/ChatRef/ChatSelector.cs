using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HelperUWP.ChatRef
{
    class ChatSelector : DataTemplateSelector
    {
        public DataTemplate FromTemplate { get; set; }
        public DataTemplate ToTemplate { get; set; }
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            ChatDetailInfo temp_info = item as ChatDetailInfo;
            if (temp_info != null)
            {
                if (temp_info.IsFrom) return FromTemplate;
                else return ToTemplate;
            }
            return null;
        }
    }
}
