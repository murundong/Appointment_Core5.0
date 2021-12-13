using CoreEntityFramework.Enum;
using CoreEntityFramework.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreEntityFramework.ModelView
{
    public class View_BannerOutput
    {
        public string img { get; set; }
        public Enum_ImgType img_type { get; set; }
        public string url { get; set; }

    }
}
