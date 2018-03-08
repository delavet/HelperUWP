using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperUWP.IPGWref
{
    class IPGWMsg
    {
        public bool Success;
        public String Title;
        public String Content;
        public IPGW_type Type;
        public IPGWMsg(bool s,String t,String c,IPGW_type a)
        {
            Success = s;
            Title = t;
            Content = c;
            Type = a;
        }
        public IPGWMsg() { }
    }
}
