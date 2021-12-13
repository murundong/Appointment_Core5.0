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
    [Table("Doors")]
    public class Doors
    {
        [Key]
        public int id { get; set; }
        public string door_name { get; set; }
        public string door_desc { get; set; }
        public string door_tel { get; set; }
        public string door_img { get; set; }
        public string door_banners { get; set; }
        public string door_address { get; set; }
        public bool only_allow_member { get; set; }
        public Enum_DoorStatus status { get; set; }
        public bool active { get; set; } = true;
        public string create_openid { get; set; }
        public DateTime create_time { get; set; } = DateTime.Now;

    }
}
