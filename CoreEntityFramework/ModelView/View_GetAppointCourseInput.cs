using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreEntityFramework.ModelView
{
    public class View_GetAppointCourseInput : Base_PageInput
    {
        public string date { get; set; }
        public string tag { get; set; }
        public int doorId { get; set; }
        public int uid { get; set; }
    }
}
