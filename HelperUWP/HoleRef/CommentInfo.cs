using HelperUWP.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperUWP.HoleRef
{
    class CommentInfo
    {
        public int Cid { get; set; }
        public String Text { get; set; }
        public Boolean Islz { get; set; }
        public long Timestamp { get; set; }
        public String TimeStr
        {
            get
            {
                return Util.PassedTime(Util.GetTimeStamp(DateTime.Now), Timestamp);
            }
        }

        public CommentInfo(int _cid, String _text, Boolean _islz, long _timestamp)
        {
            Cid = _cid;
            Text = new String(_text.ToCharArray());
            Islz = _islz;
            Timestamp = _timestamp * 1000;
        }
        
    }
}
