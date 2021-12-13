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
    [NotMapped]
    public  class View_DoorUserInfoOutput: DoorUsers
    {
        public string avatar { get; set; }
        public string nick_name { get; set; }
    }
}
