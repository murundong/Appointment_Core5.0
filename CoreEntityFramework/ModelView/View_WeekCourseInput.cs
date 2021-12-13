using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreEntityFramework.ModelView
{
   public class View_WeekCourseInput
    {
        public int door_id { get; set; }
        /// <summary>
        /// 0:当前周，-1:上一周，1：下一周
        /// </summary>
        public int tp { get; set; }
    }
}
