using CoreEntityFramework.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreEntityFramework.ModelView
{
    public class View_ApplyUserInfo
    {
        public string open_id { get; set; }
        public string nick_name { get; set; }
        public string avatar { get; set; }
        public Enum_Gender? gender { get; set; }
        public string tel { get; set; }
        public string real_name { get; set; }
        public string birthday { get;set; }
        public string apply_msg { get; set; }

        public Enum_ApplyCurator? apply_status { get; set; }

    }
}
