using CoreBaseClass;
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
    public class View_MyAppointCompOutput
    {
        public string dt { get; set; }
        public int ct { get; set; }
        public List<View_MyAppointCompOutput_Detail> courses = new List<View_MyAppointCompOutput_Detail>();

        public string month
        {
            get
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(dt))
                    {
                        return $"{ Convert.ToInt32(dt.Split('-')[1])}月";
                    }
                }
                catch (Exception ex)
                {
                    
                }
                return "";
            }
        }

        public string year
        {
            get
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(dt))
                    {
                        return $"{dt.Split('-')[0]}";
                    }
                }
                catch (Exception ex)
                {
                }
                return "";
            }
        }
    }

    [NotMapped]
    public class View_MyAppointCompOutput_Detail : DoorUsersAppoints
    {
       
        public int judge { get; set; }
        public string dt { get; set; }
        public int door_id { get; set; }
        public string door_name { get; set; }
        public string course_date { get; set; }
        public string course_time { get; set; }
        public string subject_name { get; set; }
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

        public string st_time
        {
            get
            {
                return $"{course_date} {course_time}";
            }
        }

        public string ed_time
        {
            get
            {
                return $"{(Convert.ToDateTime($"{course_date} {course_time}").AddMinutes(subject_duration)).ToString("yyyy-MM-dd HH:mm:ss")}";
            }
        }


        public string str_time
        {
            get
            {
                try
                {
                    string res = $"{Convert.ToDateTime(course_date).ToString("MM/dd")}({Convert.ToDateTime(course_date).GetWeekOfTime()}){course_time}~{Convert.ToDateTime($"{course_date} {course_time}").AddMinutes(subject_duration).GetDateTimeWithoutDate()}";
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
