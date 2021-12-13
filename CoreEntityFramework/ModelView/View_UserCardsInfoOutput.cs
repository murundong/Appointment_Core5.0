using CoreBaseClass;
using CoreEntityFramework.Enum;
using CoreEntityFramework.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreEntityFramework.ModelView
{
    public class View_UserCardsInfoOutput
    {
      
        public int id { get; set; }
        public int door_id { get; set; }
        public int uid { get; set; }

        public Enum_UserRole door_role { get; set; }
        public string door_remark { get; set; }

        public string open_id { get; set; }
        public string nick_name { get; set; }
        public string avatar { get; set; }
        public Enum_Gender gender { get; set; }
        public Enum_UserRole role { get; set; }
        public string tel { get; set; }
        public string initial { get; set; }

        public string real_name { get; set; }
        public DateTime? birthday { get; set; }

        public string str_birthday
        {
            get
            {
                return birthday?.ToString("yyyy-MM-dd");
            }
        }


        public int? cid { get; set; }
        public string card_name { get; set; }
        public string card_desc { get; set; }
        private string _door_img;
        public string door_img
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_door_img))
                {
                    return ConstConfig.ErrorImg;
                }
                return _door_img;
            }
            set
            {
                _door_img = value;
            }
        }
        public Enum_CardType? ctype { get; set; }

        public string str_ctype
        {
            get
            {
                return ctype?.ToString();
            }
        }

        public DateTime? card_sttime { get; set; }
        public string str_card_sttime
        {
            get
            {
                if (card_sttime.HasValue)
                {
                    return ((DateTime)card_sttime).ToString("yyyy-MM-dd");
                }
                return null;
            }
        }
        public DateTime? card_edtime { get; set; }
        public string str_card_edtime
        {
            get
            {
                if (card_edtime.HasValue)
                {
                    return ((DateTime)card_edtime).ToString("yyyy-MM-dd");
                }
                return null;
            }
        }
        //public int? stop_day { get; set; }
        //public int? extend_day { get; set; }
        public int? effective_time { get; set; }
        public int? limit_week_time { get; set; }
        public int? limit_day_time { get; set; }
        public bool? is_freeze { get; set; }
        public DateTime? freeze_edtime { get; set; }
        public string str_freeze_edtime
        {
            get
            {
                if (freeze_edtime.HasValue)
                {
                    return ((DateTime)freeze_edtime).ToString("yyyy-MM-dd");
                }
                return null;
            }
        }
    }
}
