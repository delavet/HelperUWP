using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperUWP.MailRef
{
    class MailSummary
    {    
        public int Index { get; set; }
        public String Subject { get; set; }
        public String TimeString { get; set; }
        public String From { get; set; }
        public MailSummary(int _index,String _subject,DateTimeOffset _time,InternetAddressList from)
        {
            Index = _index;
            Subject = _subject;
            TimeString = _time.ToString("yyyy-MM-dd");
            From = "";
            foreach(var adr in from)
            {
                From = From + adr.Name;
            }
            if (From.Equals(""))
            {
                foreach (var adr in from)
                {
                    From += adr.ToString();
                }
            }
        }
    }
}
