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
    public   class View_TearcherDoorOutput
    {
        
        private string _door_img;

        public int id { get; set; }
        public string door_name { get; set; }
        public string door_desc { get; set; }
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
        public string door_address { get; set; }
        public Enum_DoorStatus status { get; set; }
    }
}
