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
    [Table("Subjects")]
    public class Subjects
    {
        [Key]
        public int id { get; set; }

        public int door_id { get; set; }
        public string subject_name { get; set; }
        public string subject_tag { get; set; }
        public string subject_teacher { get; set; }
        public int subject_duration { get; set; }
        public int? subject_price { get; set; }
        public string subject_img { get; set; }
        public string subject_desc { get; set; }
        public string need_cards { get; set; }
        public string create_openid { get; set; }
        public bool active { get; set; } = true;
        public DateTime create_time { get; set; } = DateTime.Now;


    }
}
