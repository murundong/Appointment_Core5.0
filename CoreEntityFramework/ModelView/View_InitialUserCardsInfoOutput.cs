using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreEntityFramework.ModelView
{
   public  class View_InitialUserCardsInfoOutput
    {
        public List<string> initials = new List<string>();

        public List<View_InitialUserCardsInfoItemOutput> uinfos = new List<View_InitialUserCardsInfoItemOutput>();
    }

    public class View_InitialUserCardsInfoItemOutput
    {
        public string initial { get; set; }
        public List<View_UserCardsInfoOutput> uinfos = new List<View_UserCardsInfoOutput>();
    }

}
