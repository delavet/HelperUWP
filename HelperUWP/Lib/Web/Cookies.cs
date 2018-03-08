using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace HelperUWP.Lib.Web
{
    class Cookies
    {
        private static Dictionary<String, Dictionary<String, String>> cookies = new Dictionary<string, Dictionary<string, string>>();
        private static List<String> tempCookieList = new List<String>();

        //从url字符串中解析出域名字符串
        private static String getDomain(String Url)
        {
            try
            {
                char[] tempArray = Url.ToCharArray();
                String domain = new String(tempArray);
                int pos1 = domain.IndexOf("://") + 3;
                domain = Url.Substring(pos1);
                int pos2 = domain.IndexOf("/");
                if (pos2 == -1) return domain;
                return domain.Substring(0, pos2).Trim();
            }
            catch (Exception e)
            {
                return "";
            }
        }

        private static Boolean whetherToSendCookies(String url)
        {
            if (url.StartsWith("http://course.pku.edu.cn/webapps/login/"))
                return false;
            if (url.StartsWith("http://www.bdwm.net/"))
                return false;
            return true;
        }

        private static Boolean whetherToSetCookie(String url)
        {
            if (url.StartsWith("http://www.bdwm.net/"))
                return false;
            return true;
        }

        public static void setCookie(HttpResponseMessage response, String url)
        {
            String domain = getDomain(url);
            if (domain.Equals("")) return;
            Dictionary<String, String> dictionary;
            if (!cookies.TryGetValue(domain, out dictionary)) dictionary = new Dictionary<string, string>();
            if (!whetherToSetCookie(url)) return;
            String cookie = "";
            tempCookieList.Clear();
            tempCookieList.Add(cookie);
            while (response.Headers.TryGetValue("Set-cookie", out cookie))
            {
                if (tempCookieList.IndexOf(cookie) != -1) break;
                tempCookieList.Add(cookie);
                String[] strings = cookie.Split(';');
                String[] strings2 = strings[0].Split('=');
                String key = strings2[0].Trim();
                String value = strings2.Length > 1 ? strings2[1].Trim() : "";
                if (!"".Equals(key))
                {
                    dictionary[key] = value;
                }
            }
            cookies[domain] = dictionary;
        }

        public static void addCookie(ref HttpRequestMessage httpRequestMsg)
        {
            String domain = getDomain(httpRequestMsg.RequestUri.ToString());

            if (domain.Equals("")) return;
            if (!whetherToSendCookies(httpRequestMsg.RequestUri.ToString())) return;
            Dictionary<String, String> temp_dic;
            if (cookies.TryGetValue(domain, out temp_dic))
            {
                String cookieString = "";
                foreach (KeyValuePair<String, String> keyValPr in temp_dic)
                {
                    String key = keyValPr.Key;
                    String value = keyValPr.Value;
                    cookieString = cookieString + key + "=" + value + ";";
                }
                if (cookieString != "") httpRequestMsg.Headers.Add(new KeyValuePair<String, String>("cookie", cookieString));
            }
        }
    }
}
