using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperUWP.Lib.Web
{
    class Parameters
    {
        public String name { get;  set; }
        public String value { get;  set; }
        public KeyValuePair<String,String> keyValue
        {
            get
            {
                return new KeyValuePair<string, string>(name, value);
            }
        }
        public Parameters(String _name, String _value)
        {
            this.name = _name;
            this.value = _value;
        }

    }
}
