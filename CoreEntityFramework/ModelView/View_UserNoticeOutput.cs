using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreEntityFramework.ModelView
{
    public class View_UserNoticeOutput
    {
        public string img { get; set; }
        public string nick { get; set; }
        public string title { get; set; }
        public string msg { get; set; }
        public int is_system { get; set; }
        public DateTime create_time { get; set; }
        public string str_create_time
        {
            get
            {
                return create_time.ToString("yyyy/MM/dd HH:mm:ss");
            }
        }

    }
}
