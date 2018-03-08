using HelperUWP.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace HelperUWP.NCref
{
    class NoticeInfo
    {
        public BitmapImage SourceIcon { get; set; }
        public String SourceName { get; set; }
        public int Nid { get; set; }
        public String Title { get; set; }
        public int Sid { get; set; }
        public String Url { get; set; }
        public String Subscribe { get; set; }
        public String Time { get; set; }
        public NoticeInfo(int _nid, String _title, int _sid, String _url, String _subscribe, long timestamp)
        {
            Nid = _nid;
            Title = new String(_title.ToCharArray());
            Sid = _sid;
            Url = new String(_url.ToCharArray());
            Subscribe = new String(_subscribe.ToCharArray());
            if (Subscribe.Length > 30)
                Subscribe = Subscribe.Substring(0, 28) + "...";
            DateTime d = Util.TimestampToDateTime(timestamp);
            Time = d.ToString("yyyy年MM月dd日");
        }
    }
}
