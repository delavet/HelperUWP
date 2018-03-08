using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HelperUWP.InfoRef
{
    class Semester : INotifyPropertyChanged
    {
        private String _year;
        public String year
        {
            get
            {
                return _year;
            }
            set
            {
                _year = value;
                OnPropertyChanged();
            }
        }
        private String _term;
        public String term
        {
            get
            {
                return _term;
            }
            set
            {
                _term = value;
                OnPropertyChanged();
            }
        }
        public List<Course> courses;
        private String _gpa;
        public String gpa
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
        private String _weight;
        public String weight
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
        public Boolean isDual;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public Semester(String _year, String _term, String _gpa, Boolean _isDual)
        {
            year = _year;
            term = _term;
            gpa = _gpa;
            weight = "";
            courses = new List<Course>();
            isDual = _isDual;
        }

        public void addCourse(String _name, String _fullname, String _type,
                String _weight, String _grade, String _delta, String _accurate, String _gpa)
        {
            Course course = new Course(this, _name, _fullname, _type, _weight,
                    _grade, _delta, _accurate, _gpa);
            courses.Add(course);
        }

        public Boolean isThisSemester(String _year, String _term)
        {
            return year.Equals(_year) && term.Equals(_term);
        }

        public void calWeight()
        {
            int w = 0;
            foreach (Course course in courses)
            {
                float sc = float.Parse(course.weight);
                w += (int)sc;
            }
            weight = w.ToString();
        }
    }
}
