using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreEntityFramework.WeixData
{
    public class W_AUTH_RETURN
    {
        public string openid { get; set; }

        public string session_key { get; set; }

        public string unionid { get; set; }

        public int errcode { get; set; }
        public string errmsg { get; set; }

    }
}
