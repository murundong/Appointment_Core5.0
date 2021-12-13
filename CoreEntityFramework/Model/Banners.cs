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
    [Table("Banners")]
    public class Banners
    {
        [Key]
        public int id { get; set; }
        public Enum_ImgType img_type { get; set; }
        public string img { get; set; }
        public string url { get; set; }
        public bool active { get; set; } = true;
        public int sort { get; set; }
        public DateTime create_time { get; set; } = DateTime.Now;

    }
}
