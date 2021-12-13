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
    [Table("UserInfos")]
    public class UserInfos
    {
        [Key]
        public int uid   { get; set; }
        public string nick_name { get; set; }
        public string avatar { get; set; }
        public Enum_Gender gender { get; set; }

        public string open_id { get; set; }
        public string tel { get; set; }
        public DateTime create_time { get; set; } = DateTime.Now;
        public string initial { get; set; }
        public Enum_UserRole role { get; set; }
        public string real_name { get; set; }
        public DateTime? birthday { get; set; }

        public string remark { get; set; }
    }
}
