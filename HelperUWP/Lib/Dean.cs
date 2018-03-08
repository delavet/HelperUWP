using HelperUWP.Lib.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperUWP.Lib
{
    class Dean
    {
        public async static Task<String> get_session_id(String captcha)
        {
            try
            {
                if (captcha == null && captcha == "") return "";
                List<Parameters> list = new List<Parameters>();
                list.Add(new Parameters("sno", Constants.username));
                list.Add(new Parameters("password", Constants.password));
                list.Add(new Parameters("captcha", captcha));
                Parameters parameter = await WebConnection.Connect("http://dean.pku.edu.cn/student/authenticate.php", list);
                if (!"200".Equals(parameter.name))
                {
                    Util.DealWithDisconnect(parameter);
                }
                else
                {
                    int pos, pos2;
                    String str = parameter.value;
                    if (str.Contains("alert"))
                    {
                        pos = str.IndexOf("alert(");
                        str = str.Substring(pos);
                        pos = str.IndexOf("\"");
                        str = str.Substring(pos + 1);
                        pos2 = str.IndexOf("\"");
                        str = str.Substring(0, pos2);
                        Constants.BoxPage.ShowMessage(str, "alert");
                    }
                    String sessionId = "";
                    pos = str.IndexOf("PHPSESSID");
                    str = str.Substring(pos);
                    pos = str.IndexOf("=");
                    str = str.Substring(pos + 1);
                    pos2 = str.IndexOf("\"");
                    str = str.Substring(0, pos2);
                    sessionId = str;
                    Constants.phpsessid = sessionId;
                }
                return Constants.phpsessid;
            }
            catch
            {
                Constants.BoxPage.ShowMessage("登录dean错误！"); 
                return "";
            }
        }
    }
}
