using HelperUWP.HoleRef;
using HelperUWP.Lib;
using HelperUWP.Lib.Web;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperUWP.NCref
{
    class CourseNoticeList: ObservableCollection<NoticeInfo>
    {
        public event DataLoadingEventHandler DataLoading;
        public event DataLoadedEventHandler DataLoaded;
        public CourseNoticeList()
        {
            DoRefresh();
        }
        public async void DoRefresh()
        {
            //DataLoading();
            List<Parameters> param = new List<Parameters>();
            param.Add(new Parameters("user_id", Constants.username));
            param.Add(new Parameters("pwd",CourseJSTranslation.strEncode(Constants.password)));
            Parameters result = await WebConnection.Connect("http://course.pku.edu.cn/webapps/login/", param);
            if (!result.name.Equals("200"))
            {
                Util.DealWithDisconnect(result);
                return;
            }
            String reply1 = result.value;
            if(!reply1.Contains("Please Wait"))
            {
                Constants.BoxPage.ShowMessage("教学网登录失败");
                return;
            }
            String reply2 = await GetNotice();
            DecodeHtml(reply2);
            //DataLoaded();
        }
        private async Task<String> LoginCourse()
        {
            List<Parameters> param = new List<Parameters>();
            param.Add(new Parameters("user_id", Constants.username));
            param.Add(new Parameters("pwd", Constants.password+"asd"));
            Parameters result = await WebConnection.Connect("http://course.pku.edu.cn/webapps/login/", param);
            if (!result.name.Equals("200"))
            {
                Util.DealWithDisconnect(result);
                return "";
            }
            return result.value;
        }
        private async Task<String> GetNotice()
        {
            List<Parameters> arrayList = new List<Parameters>();
            arrayList.Add(new Parameters("action", "refreshAjaxModule"));
            arrayList.Add(new Parameters("modId", "_1_1"));
            arrayList.Add(new Parameters("tabId", "_1_1"));
            arrayList.Add(new Parameters("tab_tab_group_id", "_3_1"));
            Parameters result = await WebConnection.Connect("http://course.pku.edu.cn/webapps/portal/execute/tabs/tabAction", arrayList);
            if (!result.name.Equals("200"))
            {
                Util.DealWithDisconnect(result);
                return "";
            }
            return result.value;
        }
        private void DecodeHtml(String html)
        {
            html = html.Replace("<!--", "");
            html = html.Replace("-->", "");
            html = html.Replace("<![CDATA[", "");
            html = html.Replace("]]>", "");
            try
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                /*数据不足，未能实现*/
            }
            catch
            {
                Constants.BoxPage.ShowMessage("教学网通知解析失败！");
            }
        }
    }
}
