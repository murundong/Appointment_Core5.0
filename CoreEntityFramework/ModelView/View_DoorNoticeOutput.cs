using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreEntityFramework.ModelView
{
    public class View_DoorNoticeOutput
    {
        public int id { get; set; }
        public int door_id { get; set; }
        public int du_id { get; set; }
        public string title { get; set; }
        public string msg { get; set; }
        public DateTime create_time { get; set; }
        public bool active { get; set; }
        public string str_create_time
        {
            get
            {
                return create_time.ToString("yyyy/MM/dd");
            }
        }

        public int door_role { get; set; }
        public string nick_name { get; set; }
    }
}
