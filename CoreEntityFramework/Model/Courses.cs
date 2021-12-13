using CoreEntityFramework.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreEntityFramework.Model
{
    [Table("Courses")]
    public class Courses
    {
        [Key]
        public int id { get; set; }
        public int door_id { get; set; }
        public string course_desc { get; set; }
        public string course_date { get; set; }
        public string course_time { get; set; }
        public int subject_id { get; set; }
        public int max_allow { get; set; }
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
        /// 需要会员卡
        /// </summary>
        public string need_cards { get; set; }

        /// <summary>
        /// 预约时间限制（离课程开始多长时间不能预约）
        /// </summary>
        public int? limit_appoint_duration { get; set; }

        public bool active { get; set; } = true;

        /// <summary>
        /// 临时替换老师
        /// </summary>
        public string temp_teacher { get; set; }
        public string create_openid { get; set; }
        public DateTime create_time { get; set; } = DateTime.Now;

    }
}
