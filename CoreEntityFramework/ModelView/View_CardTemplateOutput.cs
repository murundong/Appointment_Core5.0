using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreEntityFramework.ModelView
{
    public class View_CardTemplateOutput
    {
        public int id { get; set; }
        public int door_id { get; set; }
        public string card_name { get; set; }
        public string card_type { get; set; }
        public string card_desc { get; set; }
    }

    public class View_CardTemplateOutputItem
    {
       
        private string _img { get; set; }
        public List<View_CardTemplateOutput> temps = new List<View_CardTemplateOutput>();
        public string img {
            get
            {
                if (string.IsNullOrWhiteSpace(_img))
                {
                     return ConstConfig.ErrorImg;
                }
                return _img;
            }
            set
            {
                _img = value;
            }
        }
    }
}
