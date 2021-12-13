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
    [Table("DoorUsersCards")]
    public class DoorUsersCards
    {
        [Key]
        public int id { get; set; }
        public int door_id { get; set; }
        public int du_id { get; set; }
        public int uid { get; set; }
        public int? cid { get; set; }
        public Enum_CardType? ctype { get; set; }
        public DateTime? card_sttime { get; set; }
        public DateTime? card_edtime { get; set; }
    
        public int? effective_time { get; set; }
        public int? limit_week_time { get; set; }
        public int? limit_day_time { get; set; }
        public bool is_freeze { get; set; }
        public DateTime? freeze_edtime { get; set; }

        public bool is_delete { get; set; } = false;
        public DateTime create_time { get; set; } = DateTime.Now;

       

    }
}
