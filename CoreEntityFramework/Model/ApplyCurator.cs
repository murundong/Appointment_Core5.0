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
    [Table("ApplyCurator")]
    public class ApplyCurator
    {
        [Key]
        public int id { get; set; }

        public string open_id { get; set; }

        public string apply_msg { get; set; }
        public Enum_ApplyCurator enum_status { get; set; }

        public DateTime create_time { get; set; } = DateTime.Now;

    }
}
