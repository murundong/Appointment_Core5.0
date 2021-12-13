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
    [Table("DoorUsersSubsMsg")]
    public class DoorUsersSubsMsg
    {
        [Key]
        public int id { get; set; }
        public int door_id { get; set; }
        public int uid { get; set; }
        public int course_id { get; set; }
        public bool is_queen { get; set; }
        public bool is_cancel { get; set; }
        public bool is_notice { get; set; }
    }
}
