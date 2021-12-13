using CoreBaseClass;
using CoreEntityFramework.Enum;
using CoreEntityFramework.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreEntityFramework.ModelView
{
    [NotMapped]
    public class View_JudgeCourseOutput: Courses
    {
        public string subject_name { get; set; }
        public string subject_teacher { get; set; }
        public int subject_duration { get; set; }
        private string _subject_img { get; set; }
        public string subject_img
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_subject_img))
                {
                    return ConstConfig.ErrorImg;
                }
                return _subject_img;
            }
            set
            {
                _subject_img = value;
            }
        }

        public string str_time
        {
            get
            {
                try
                {
                    string res = $"{course_date}({Convert.ToDateTime(course_date).GetWeekOfTime()}){course_time}~{Convert.ToDateTime($"{course_date} {course_time}").AddMinutes(subject_duration).GetDateTimeWithoutDate()}";
                    return res;
                }
                catch (Exception ex)
                {
                    return string.Empty;
                }
            }
        }
    }
}
