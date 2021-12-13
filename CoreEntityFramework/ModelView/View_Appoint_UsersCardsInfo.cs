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
    public class View_Appoint_UsersCardsInfo: DoorUsersCards
    {
        public string card_name { get; set; }
        public Enum_CardType card_type { get; set; }
    }
}
