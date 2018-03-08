using HelperUWP.Lib;
using HelperUWP.Lib.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Xaml.Media.Imaging;

namespace HelperUWP.ChatRef
{
    public enum ChatInfoType { Text,Image }
    public class ChatDetailInfo : INotifyPropertyChanged
    {
        public WriteableBitmap bmpsrc;
        public WriteableBitmap BmpSrc
        {
            get
            {
                return bmpsrc;
            }
            set
            {
                bmpsrc = value;
                OnPropertyChanged();
            }
        }
        private int id;
        public int ID
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
                OnPropertyChanged();
            }
        }
        private String content;
        public String Content
        {
            get
            {
                return content;
            }
            set
            {
                content = value;
                OnPropertyChanged();
            }
        }
        private long timestamp;
        public long Timestamp
        {
            get
            {
                return timestamp;
            }
            set
            {
                timestamp = value;
                OnPropertyChanged();
            }
        }
        public ChatInfoType Mime;
        public bool IsFrom;
        public bool ShowTime = false;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public ChatDetailInfo(int _id,String _content,String _mime,long _timestamp,String _type)
        {
            ID = _id;
            Content = new String(_content.ToCharArray());   
            Timestamp = _timestamp * 1000;
            if (_mime.Contains("text")) Mime = ChatInfoType.Text;
            else
            {
                Content = "";
                Mime = ChatInfoType.Image;
                JsonObject json = JsonObject.Parse(_content);
                String type = json.GetNamedString("type", "unknown");
                if (!type.Equals("image")) return;
                JsonObject param = json.GetNamedObject("parameter", null);
                if (param == null) return;
                String filename = param.GetNamedString("filename", "");
                LoadImage(filename);
            }
            if (_type.Equals("from")) IsFrom = true;
            else IsFrom = false;
        }
        public ChatDetailInfo(WriteableBitmap bmp)
        {
            Content = "";
            Mime = ChatInfoType.Image;
            ID = -1;
            IsFrom = false;
            Timestamp = Util.GetTimeStamp(DateTime.Now);
            BmpSrc = bmp;
        }
        private async void LoadImage(String imgName)
        {
            if (imgName.Equals("")) return;
            BmpSrc = new WriteableBitmap(100, 200);
            Stream stream = await WebConnection.Connect_for_stream(Constants.domain + "/pkuhelper/../static/message/image/" + imgName);         
            try
            {
                var ran_stream = await Util.StreamToRandomAccessStream(stream);
                BmpSrc.SetSource(ran_stream);
            }
            catch(Exception e)
            {
                Debug.WriteLine("fail to get the chat image," + e.StackTrace);
                return;
            }
        }
    }
}
