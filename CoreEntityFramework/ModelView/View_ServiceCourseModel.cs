using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreEntityFramework.ModelView
{
   
    public class View_ServiceCourseModel
    {
       
        public int id { get; set; }
        public int uid { get; set; }
        public int appoint_id { get; set; }
        public string open_id { get; set; }
        public int door_id { get; set; }
        public string door_name { get; set; }
        public string course_date { get; set; }
        public string course_time { get; set; }
        public int subject_id { get; set; }

        public string subject_title { get; set; }

        /// <summary>
        /// 取消时间(离课程开始多长时间不能取消)
        /// </summary>
        public int? cancel_duration { get; set; }

    }
}
