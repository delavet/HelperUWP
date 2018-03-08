using HelperUWP.Lib;
using HelperUWP.Lib.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace HelperUWP.LFref
{
    public class LostFoundInfo : INotifyPropertyChanged
    {
        public static bool LOST
        {
            get
            {
                return true;
            }
        }
        public static bool FOUND
        {
            get
            {
                return false;
            }
        }
        public static int TYPE_CARD
        {
            get
            {
                return 3;
            }
        }
        public static int TYPE_BOOK
        {
            get
            {
                return 4;
            }
        }
        public static int TYPE_DEVICE
        {
            get
            {
                return 5;

            }
        }
        public static int TYPE_OTHERS
        {
            get
            {
                return 6;
            }
        }
        public int id;
        public String name;
        public bool lost_or_found;
        public int type;
        public String detail;
        public long post_time;
        public long action_time;
        public bool mine = false;
        public String img_url;
        public String thumb_img_url;
        public String img_name;
        public String poster_uid;
        public String poster_phone;
        public String poster_name;
        public String poster_college;
        private BitmapImage _thumb_img;
        public BitmapImage thumb_img
        {
            get
            {
                return _thumb_img;
            }
            set
            {
                _thumb_img = value;
                OnPropertyChanged();
            }
        }
        public WriteableBitmap real_img;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public LostFoundInfo(int _id, String _name, String _lost_or_found, String _type,
            String _detail, long _posttime, long _actiontime, String _image,
            String _posterUid, String _posterPhone, String _posterName, String _posterCollege)
        {
            id = _id;
            name = _name;
            if ("lost".Equals(_lost_or_found)) lost_or_found = LOST;
            else lost_or_found = FOUND;
            if ("card".Equals(_type)) type = TYPE_CARD;
            else if ("book".Equals(_type)) type = TYPE_BOOK;
            else if ("device".Equals(_type)) type = TYPE_DEVICE;
            else type = TYPE_OTHERS;
            detail = _detail;
            post_time = _posttime;
            action_time = _actiontime;
            poster_uid = _posterUid;
            poster_phone = _posterPhone;
            poster_name = _posterName;
            poster_college = _posterCollege;
            get_image(_image);
        }
        private async void get_image(String _image)
        {
            if (_image != null && !"".Equals(_image))
            {
                try
                {
                    img_name = _image;
                    img_url = Constants.domain + "/services/image/" + _image + ".jpg";
                    thumb_img_url = Constants.domain + "/services/image/" + _image + "_thumb.jpg";
                    var stream1 = await WebConnection.Connect_for_stream(img_url);
                    real_img = new WriteableBitmap(100, 100);
                    real_img.SetSource(await Util.StreamToRandomAccessStream(stream1));
                    var stream2 = await WebConnection.Connect_for_stream(thumb_img_url);
                    thumb_img = new BitmapImage();
                    thumb_img.SetSource(await Util.StreamToRandomAccessStream(stream2));
                }
                catch(Exception e)
                {
                    Debug.WriteLine("fail to get the LF image" + e.StackTrace);
                }
            }
            else
            {
                img_url = "";
                thumb_img_url = "";
                img_name = "";
                StorageFile img_file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/hero_pku.png"));
                IRandomAccessStream input = await img_file.OpenReadAsync();
                thumb_img = new BitmapImage();
                thumb_img.SetSource(input);
                StorageFile img_file1 = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/hero_pku.png"));
                IRandomAccessStream input1 = await img_file.OpenReadAsync();
                real_img = new WriteableBitmap(150, 180);
                real_img.SetSource(input1);
            }
        }
    }
}
