using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperUWP.CourseRef
{
    class SimpleCourseList: ObservableCollection<SimpleCustomCourseInfo>
    {
        public void Delete(String courseName)
        {
            foreach(var info in this)
            {
                if (info.Name.Equals(courseName))
                {
                    this.Remove(info);
                    break;
                }
                
            }
        }
    }
}
