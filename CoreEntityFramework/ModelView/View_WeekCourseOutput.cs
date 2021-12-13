using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreEntityFramework.ModelView
{
    public class View_WeekCourseOutput
    {
        public string date { get; set; }
        public string week { get; set; }
        public List<View_CoursesOutput> Courses = new List<View_CoursesOutput>();
    }
}
