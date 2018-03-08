using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace HelperUWP.ChatRef
{
    public class ChatListInfo : INotifyPropertyChanged
    {
        private String username;
        public String UserName
        {
            get
            {
                return username;
            }
            set
            {
                username = value;
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
        private bool hasnew;
        public bool HasNew
        {
            get
            {
                return hasnew;
            }
            set
            {
                hasnew = value;
                Name = "我了个去";
                OnPropertyChanged();
            }
        }
        private String name;
        public String Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }
        private int number;
        public int Number
        {
            get
            {
                return number;
            }
            set
            {
                number = value;
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
        private Color col;
        public Color color
        {
            get
            {
                return col;
            }
            set
            {
                col = value;
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
        public void SetNew()
        {
            HasNew = true;
        }
        public void ResetNew()
        {
            HasNew = false;
        }
        public ChatListInfo(String _user_name, String _name, long _timestamp, String _content, int _number, Boolean _has_new)
        {
            UserName = new String(_user_name.ToCharArray());
            Timestamp = _timestamp;
            Content = new String(_content.ToCharArray());
            Number = _number;
            HasNew = _has_new;
            if (_has_new) color = new Color() { A = 0xFF, R = 0x00, G = 0xA1, B = 0xF1 };
            else color = new Color() { A = 0, R = 0, G = 0, B = 0 };
            if ("".Equals(_name)) name = UserName;
            else Name = _name;
        }
        public override bool Equals(object obj)
        {
            if (!(obj is ChatListInfo)) return false;
            ChatListInfo info = obj as ChatListInfo;
            return (UserName.Equals(info.UserName)) &&
                (Content.Equals(info.Content)) &&
                (Name.Equals(info.Name)) &&
                (Number == info.Number);
        }
    }
}
