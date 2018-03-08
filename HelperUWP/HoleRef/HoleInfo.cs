using HelperUWP.Lib;
using HelperUWP.Lib.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace HelperUWP.HoleRef
{
    class HoleInfo
    {
        public static int TYPE_UNKNOWN
        {
            get
            {
                return 0;
            }
        }
        public static int TYPE_TEXT
        {
            get
            {
                return 1;
            }
        }
        public static int TYPE_IMAGE
        {
            get
            {
                return 2;
            }
        }
        public static int TYPE_AUDIO
        {
            get
            {
                return 3;
            }
        }

        public int pid { get; set; }
        public String text { get; set; }
        public long timestamp { get; set; }
        public int type { get; set; }
        public int reply { get; set; }
        public int like { get; set; }
        public int extra { get; set; }
        public String url { get; set; }
        public WriteableBitmap writable_bitmap { get; set; }
        public Visibility audio_vis { get; set; }
        public String TimeStr
        {
            get
            {
                return Util.PassedTime(Util.GetTimeStamp(DateTime.Now), timestamp);
            }
        }

        public HoleInfo(int _pid, String _text, long _timestamp, String _type,
            int _reply, int _like, int _extra, String _url)
        {
            audio_vis = Visibility.Collapsed;
            pid = _pid;
            text = new String(_text.ToCharArray());
            timestamp = _timestamp * 1000;
            if ("image".Equals(_type)) type = TYPE_IMAGE;
            else if ("audio".Equals(_type)) type = TYPE_AUDIO;
            else if ("text".Equals(_type)) type = TYPE_TEXT;
            else
            {
                type = TYPE_TEXT;
                text = "（这是一条不支持的消息，请更新到最新版PKU Helper进行查看。）";
            }

            reply = _reply;
            like = _like;
            extra = _extra;
            url = new String(_url.ToCharArray());
            if (type == TYPE_IMAGE)
            {
                writable_bitmap = Constants.DefaultImg;
                url = Constants.domain + "/services/pkuhole/images/" + url;
            }
            else if(type==TYPE_AUDIO)
            {
                audio_vis = Visibility.Visible;
                url = Constants.domain + "/services/pkuhole/audios/" + url;
            }
            other_init();
        }

        private async void other_init()
        {
            if (!((type == TYPE_IMAGE || type == TYPE_AUDIO) && !"".Equals(url))) return;
            if (type == TYPE_IMAGE)
            {
                writable_bitmap = new WriteableBitmap(100, 200);
                Stream stream = await WebConnection.Connect_for_stream(url);
                if (stream == null) return;
                var ran_stream = await Util.StreamToRandomAccessStream(stream);
                try
                {
                    writable_bitmap.SetSource(ran_stream);
                }
                catch (Exception e)//这里未能明白什么造成了异常
                {
                    Debug.WriteLine("fail to get the hole image, at" + e.StackTrace);
                    return;
                }
            }else
            {
                /*try
                {
                    writable_bitmap = new WriteableBitmap(100, 200);
                    StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/audio_start.png"));
                    writable_bitmap.SetSource(await file.OpenReadAsync());
                }
                catch(Exception e)
                {
                    Debug.WriteLine("fail to set the audio image, at" + e.StackTrace);
                    return;
                }*/
            }
        }


    }
}
