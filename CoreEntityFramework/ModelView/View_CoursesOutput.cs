using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreEntityFramework.ModelView
{
    public class View_CoursesOutput
    {
        public int id { get; set; }
        public string course_desc { get; set; }
        public string course_date { get; set; }
        public string course_time { get; set; }
        public int subject_id { get; set; }
        public View_SubjectsOutput Subject { get; set; } = new View_SubjectsOutput();
        public List<View_CourseShowOutput_AppointUser> AppointUsers = new List<View_CourseShowOutput_AppointUser>();
        public List<View_CourseShowOutput_AppointUser> QueueAppointUsers = new List<View_CourseShowOutput_AppointUser>();
        public int max_allow { get; set; }
        public int? door_id { get; set; }
        public int min_allow { get; set; }
        /// <summary>
        /// 取消时间(离课程开始多长时间不能取消)
        /// </summary>
        public int? cancel_duration { get; set; }
        /// <summary>
        /// 允许排队
        /// </summary>
        public bool allow_queue { get; set; }

        /// <summary>
        /// 只允许当天预约
        /// </summary>
        public bool only_today_appoint { get; set; }
        /// <summary>
        /// 需要的会员卡
        /// </summary>
        public string need_cards { get; set; }

        /// <summary>
        /// 预约时间限制（离课程开始多长时间不能预约）
        /// </summary>
        public int? limit_appoint_duration { get; set; }

        public string temp_teacher { get; set; }

        public bool active { get; set; }
    }
}
