using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace HelperUWP.CourseRef
{
    public class SimpleCustomCourseInfo: INotifyPropertyChanged
    {
        private String _name;
        public String Name
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
        private Visibility _boxVisible;
        public Visibility BoxVisible
        {
            get
            {
                return _boxVisible;
            }
            set
            {
                _boxVisible = value;
                OnPropertyChanged();
            }
        }
        private Boolean _selected;
        public Boolean Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                _selected = value;
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
        public SimpleCustomCourseInfo(String name,Visibility vis=Visibility.Collapsed,Boolean che = false)
        {
            Name = name;
            BoxVisible = vis;
            Selected = che;
        }
    }
}
