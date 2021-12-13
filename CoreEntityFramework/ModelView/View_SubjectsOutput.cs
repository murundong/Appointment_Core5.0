using CoreBaseClass;
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
    public class View_SubjectsOutput
    {
        private string _subject_img { get; set; }
        public int id { get; set; }
        public string subject_name { get; set; }
        public string subject_tag { get; set; }

        public List<string> lst_subject_tag
        {
            get
            {
                List<string> res = new List<string>();
                if (!string.IsNullOrWhiteSpace(subject_tag))
                {
                    res.AddRange(subject_tag.Split(','));
                }
                return res;
            }
        }

        public string subject_teacher { get; set; }
        public int subject_duration { get; set; }
        public int? subject_price { get; set; }
        public string subject_img
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_subject_img))
                {
                    return ConstConfig.ErrorImg;
                }
                return _subject_img;
            }
            set
            {
                _subject_img = value;
            }
        }
        
        public string subject_desc { get; set; }
        public string need_cards { get; set; }
        public string create_time { get; set; }
    }
}
