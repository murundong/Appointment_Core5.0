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
    [Table("Notice")]
    public class Notice
    {
        [Key]
        public int id { get; set; }
        public int uid { get; set; }
        public string title { get; set; }
        public string msg { get; set; }
        public DateTime create_time { get; set; } = DateTime.Now;
    }
}
