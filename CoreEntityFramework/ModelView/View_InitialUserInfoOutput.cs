using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreEntityFramework.ModelView
{
    public class View_InitialUserInfoItemOutput
    {
        public string initial { get; set; }
        public List<View_UinfoOutput> uinfos = new List<View_UinfoOutput>();
    }
                 
    public class View_InitialUserInfoOutput
    {
        public List<View_InitialUserInfoItemOutput> uinfos = new List<View_InitialUserInfoItemOutput>();
        public List<string> initials = new List<string>();
    }
}
