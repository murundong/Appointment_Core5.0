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
    [Table("DoorUsers")]
    public class DoorUsers
    {
        [Key]
        public int id { get; set; }
        public int door_id { get; set; }
        public int uid { get; set; }
        public Enum_UserRole role { get; set; }
        public string remark { get; set; }
        public DateTime create_time { get; set; } = DateTime.Now;

    }
}
