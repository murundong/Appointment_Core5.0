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
    [Table("DoorNotice")]
    public class DoorNotice
    {
        [Key]
        public int id { get; set; }
        public int door_id { get; set; }
        public int du_id { get; set; }
        public string title { get; set; }
        public string msg { get; set; }
        public bool active { get; set; } = true;
        public DateTime create_time { get; set; } = DateTime.Now;

    }
}
