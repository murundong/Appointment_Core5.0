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
    [Table("DoorUsersQueueAppoints")]
    public class DoorUsersQueueAppoints
    {

        [Key]
        public int id { get; set; }
        public int du_id { get; set; }
        public int uid { get; set; }
        public int course_id { get; set; }
        public int? user_card_id { get; set; }
        public DateTime create_time { get; set; } = DateTime.Now;
    }
}
