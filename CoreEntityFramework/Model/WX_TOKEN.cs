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
    [Table("WX_TOKEN")]
    public class WX_TOKEN
    {
        [Key]
        public int id { get; set; }
        public string appid { get; set; }
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public DateTime create_time { get; set; } = DateTime.Now;
    }
}
