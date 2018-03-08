using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace HelperUWP.Lib.Web
{
    class WebConnection
    {
        public async static Task<Int32> CheckIfInSchool()//需要重写
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(new Uri(Constants.domain + "/services/pkuhelper/isIPInPKU.php"));
                if (response != null)
                {
                    String line = await response.Content.ReadAsStringAsync();
                    if (line.IndexOf('\n') != -1)
                    {
                        return Int32.Parse(line.Substring(0, line.IndexOf('\n')));
                    }
                    else return Int32.Parse(line);
                }
            }
            return -1;
        }

        private static int GetEncodingType(String url)
        {
            if (url.StartsWith("http://dean.pku.edu.cn/student/authenticate.php"))
                return 0;
            if (url.StartsWith("http://dean.pku.edu.cn/"))
                return 1;
            if (url.StartsWith("http://elective.pku.edu.cn"))
                return 0;
            if (url.StartsWith("https://iaaa.pku.edu.cn"))
                return 0;
            if (url.StartsWith("https://its.pku.edu.cn"))
                return 1;
            return -1;
        }

        private static Boolean WhetherToUseProxy(String url)
        {
            if (!Constants.isValidLogin()) return false;
            // 已经是校内：不用
            if (Constants.inSchool) return false;
            // 如果是树洞请求访问，使用
            if (url.StartsWith("http://pkuhole.sinaapp.com"))
                return true;

            // 默认不用
            return false;
        }

        private static void AddHeader(ref HttpRequestMessage httpRequestMsg, String url)
        {
            if (url.StartsWith("http://dean.pku.edu.cn/student/"))
            {
                httpRequestMsg.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/43.0.2357.81 Safari/537.36");
                httpRequestMsg.Headers.Add("Referer", "http://dean.pku.edu.cn/student/");
            }
            if (url.Contains("elective")){
                httpRequestMsg.Headers.Add("Referer", "http://elective.pku.edu.cn/elective2008/ssoLogin.do");
            }
            httpRequestMsg.Headers.Add("Platform", "Android");
            httpRequestMsg.Headers.Add("Version", Constants.version);
            httpRequestMsg.Headers.Add("User-token", Constants.user_token);


        }

        private async static Task<Parameters> ConnctWithGet(String url)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri(url));
                    Cookies.addCookie(ref request);
                    AddHeader(ref request, url);
                    HttpResponseMessage response = await httpClient.SendRequestAsync(request);
                    int return_code = (int)response.StatusCode;
                    int encodeingType = GetEncodingType(url);
                    Boolean isGbk = false;
                    if (encodeingType == -1)
                    {
                        String typeString = "";
                        response.Headers.TryGetValue("Content-type", out typeString);
                        if (typeString != null)
                        {
                            typeString = typeString.ToLower();
                            if (typeString.Contains("gbk") || typeString.Contains("gb2312")) ;
                            isGbk = true;
                        }
                    }//gbk有什么区别？以后在考虑吧
                    else if (encodeingType == 1) isGbk = true;
                    Cookies.setCookie(response, url);
                    using (Stream responseStream = (await response.Content.ReadAsInputStreamAsync()).AsStreamForRead())
                    {

                        Parameters parameters = new Parameters(return_code + "", "");
                        if (return_code == 200)
                        {
                            StreamReader sReader;

                            if (!isGbk) sReader = new StreamReader(responseStream);
                            else
                            {
                                EncodingProvider provider = CodePagesEncodingProvider.Instance;
                                Encoding.RegisterProvider(provider);
                                Encoding gb2312 = Encoding.GetEncoding("gb2312");
                                sReader = new StreamReader(responseStream, gb2312);
                            }
                            String str = "";
                            String line = sReader.ReadLine();
                            while (line != null)
                            {
                                str = str + line + "\n";
                                line = sReader.ReadLine();
                            }
                            parameters.value = str;
                        }
                        return parameters;
                    }

                }
            }
            catch (Exception e)
            {
                return new Parameters("-1", "");
            }
        }

        internal static Task<Parameters> connect(string url, List<Parameters> list)
        {
            throw new NotImplementedException();
        }

        /**
* 向服务器发送请求，并且接收到返回的数据
* @param url 请求地址
* @param params 请求参数；如果参数为空使用get请求，如果不为空使用post请求
* @return 一个 Parameters；name 存放 HTTP code（无法连接时为-1）；如果 code==200 那么 value
*  存放的是返回的网页内容
*/

        public async static Task<Parameters> Connect(String url, List<Parameters> paramList)
        {

            if (paramList == null || paramList.Count == 0)
            {
                return await ConnctWithGet(url);
            }
            try
            {
                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new Uri(url));

                url = url.Trim();
                Boolean useProxy = WhetherToUseProxy(url);
                if (useProxy)
                {
                    //以后加入
                }
                List<KeyValuePair<String, String>> t_params = new List<KeyValuePair<string, string>>();
                if (paramList != null)
                {
                    foreach (Parameters paramItem in paramList)
                    {
                        String str = paramItem.value;
                        if (str == null) continue;
                        t_params.Add(paramItem.keyValue);                       
                    }
                }
                HttpFormUrlEncodedContent content = new HttpFormUrlEncodedContent(t_params);
                Cookies.addCookie(ref request);

                if (t_params.Count != 0)
                    request.Content = content;

                AddHeader(ref request, url);
                /*String soy;
                if(!request.Headers.TryGetValue("Content-Type",out soy))
                    request.Headers.Add("Content-Type", "application/x-www-form-urlencoded");*/
                HttpResponseMessage response = await client.SendRequestAsync(request);

                int return_code = (int)response.StatusCode;
                int encodingType = GetEncodingType(url);
                Boolean isGbk = false;
                if (encodingType == -1)
                {
                    String typeString = "";
                    response.Headers.TryGetValue("Content-Type", out typeString);
                    if (typeString != null)
                    {
                        typeString = typeString.ToLower();
                        if (typeString.Contains("gbk") || typeString.Contains("gb2312")) ;
                        isGbk = true;
                    }
                }//gbk有什么区别？以后在考虑吧
                else if (encodingType == 1) isGbk = true;
                Cookies.setCookie(response, url);

                using (Stream responseStream = (await response.Content.ReadAsInputStreamAsync()).AsStreamForRead())
                {

                    Parameters parameters = new Parameters(return_code + "", "");
                    if (return_code == 200)
                    {
                        StreamReader sReader;
                        if (!isGbk) sReader = new StreamReader(responseStream);
                        else
                        {
                            EncodingProvider provider = CodePagesEncodingProvider.Instance;
                            Encoding.RegisterProvider(provider);
                            Encoding gb2312 = Encoding.GetEncoding("gb2312");
                            sReader = new StreamReader(responseStream, gb2312);
                        }
                        String str = "";
                        String line = sReader.ReadLine();
                        while (line != null)
                        {
                            str = str + line + "\n";
                            line = sReader.ReadLine();
                        }
                        str = str.Trim();
                        parameters.value = str;
                    }
                    return parameters;
                }


            }
            catch 
            {
                return new Parameters("-1", "");
            }
        }

        public async static Task<Stream> Connect_for_stream(String url)
        {

            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    url = url.Trim();
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri(url));
                    Cookies.addCookie(ref request);
                    AddHeader(ref request, url);
                    HttpResponseMessage response = await httpClient.SendRequestAsync(request);
                    Cookies.setCookie(response, url);
                    return (await response.Content.ReadAsInputStreamAsync()).AsStreamForRead();

                }
            }
            catch (Exception)
            {
                return null;
            }
        }


    }
}
