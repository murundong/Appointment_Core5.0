using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreEntityFramework.ModelView
{
    public class View_LessonDoorInfoOutput
    {
        public List<string> banners { get; set; }
        public string door_name { get; set; }
        public string door_desc { get; set; }
        public string door_manager { get; set; }
        public string door_manager_img { get; set; }
        public string door_tel { get; set; }
        public string door_address { get; set; }
    }
}
