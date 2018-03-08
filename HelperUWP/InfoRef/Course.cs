using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace HelperUWP.InfoRef
{
    class Course : INotifyPropertyChanged
    {
        private string _name;
        public string name
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
        private string _fullname;
        public string fullname
        {
            get
            {
                return _fullname;
            }
            set
            {
                _fullname = value;
                OnPropertyChanged();
            }
        }
        private Semester _semester;
        public Semester semester
        {
            get
            {
                return _semester;
            }
            set
            {
                _semester = value;
                OnPropertyChanged();
            }
        }
        private string _weight;
        public string weight
        {
            get
            {
                return _weight;
            }
            set
            {
                _weight = value;
                OnPropertyChanged();
            }
        }
        private string _grade;
        public string grade
        {
            get
            {
                return _grade;
            }
            set
            {
                _grade = value;
                OnPropertyChanged();
            }
        }
        private string _delta;
        public string delta
        {
            get
            {
                return _delta;
            }
            set
            {
                _delta = value;
                OnPropertyChanged();
            }
        }
        private string _accurate;
        public string accurate
        {
            get
            {
                return _accurate;
            }
            set
            {
                _accurate = value;
                OnPropertyChanged();
            }
        }
        private string _gpa;
        public string gpa
        {
            get
            {
                return _gpa;
            }
            set
            {
                _gpa = value;
                OnPropertyChanged();
            }
        }
        public string _type;
        public string type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
                OnPropertyChanged();
            }
        }
        private SolidColorBrush _course_color;
        public SolidColorBrush course_color
        {
            get
            {
                return _course_color;
            }
            set
            {
                _course_color = value;
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

        public Course(Semester _semester, String _name, String _fullname, String _type,
                String _weight, String _grade, String _delta, String _accurate, String _gpa)
        {
            name = _name.Trim();
            fullname = _fullname.Trim();
            type = _type.Trim();
            semester = _semester;
            weight = _weight.Trim();
            grade = _grade.Trim();
            delta = _delta.Trim();
            accurate = _accurate.Trim();
            gpa = _gpa.Trim();
            byte _R = (byte)(0xFF - (int)(double.Parse(gpa) > 1.00 ? (85 * (double.Parse(gpa) - 1)) : 0));
            byte _G = (byte)(0 + (int)(double.Parse(gpa) > 1.00 ? (85 * (double.Parse(gpa) - 1)) : 0));
            Color t_color = new Color() { A = 0xFF, R = _R, G = _G, B = 0 };
            course_color = new SolidColorBrush(t_color);
            if ("0".Equals(accurate))
            {
                grade = grade + "±" + delta;
            }
        }
    }
}
