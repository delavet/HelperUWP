using HelperUWP.Lib;
using HelperUWP.Lib.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace HelperUWP.NCref
{
    class NcSourceInfo : INotifyPropertyChanged
    {
        public string Sid;
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }
        private BitmapImage _icon;
        public BitmapImage Icon
        {
            get
            {
                return _icon;
            }
            set
            {
                _icon = value;
                OnPropertyChanged();
            }
        }
        private string _desc;
        public string Desc
        {
            get
            {
                return _desc;
            }
            set
            {
                _desc = value;
                OnPropertyChanged();
            }
        }
        private bool _is_default;

        public bool Is_default
        {
            get
            {
                return _is_default;
            }
            set
            {
                _is_default = value;
                OnPropertyChanged();
            }
        }
        private bool _is_selected;
        public bool Is_selected
        {
            get
            {
                return _is_selected;
            }
            set
            {
                _is_selected = value;
                OnPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public NcSourceInfo(string tsid, string tname, string ticon_url, string tdesc, int tdefault, int tselected)
        {
            Sid = tsid;
            get_icon(ticon_url);
            Name = tname;
            Desc = tdesc;
            if (tdefault == 0) Is_default = false;
            else Is_default = true;
            if (tselected == 0) Is_selected = false;
            else Is_selected = true;
        }
        public async void get_icon(String url)
        {
            Icon = new BitmapImage();
            Stream stream = await WebConnection.Connect_for_stream(url);
            if (stream == null) return;
            var ran_stream = await Util.StreamToRandomAccessStream(stream);
            try
            {
                Icon.SetSource(ran_stream);
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
