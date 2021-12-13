using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreEntityFramework.WeixData
{
    public class W_SUBS_DATA_INPUT
    {
        public string access_token { get; set; }
        public string touser { get; set; }
        public string template_id { get; set; }
        public string page { get; set; }
        public object data { get; set; }
        //跳转小程序类型：developer为开发版；trial为体验版；formal为正式版；默认为正式版
        public string miniprogram_state { get; set; } = "formal";
        public string lang { get; set; } = "zh_CN";
    }
}
