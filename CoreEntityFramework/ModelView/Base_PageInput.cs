using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreEntityFramework.ModelView
{
    public class Base_PageInput
    {
        public int page_index { get; set; } = 1;
        public int page_size { get; set; } = 10;
    }
}
