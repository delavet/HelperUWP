using HelperUWP.Lib;
using HelperUWP.Lib.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Popups;

namespace HelperUWP.NCref
{
    class NcSourceQuery
    {
        public static Dictionary<String, NcSourceInfo> nc_sources = null;
        public static void partionLists(ref List<NcSourceInfo> src_list)
        {
            src_list = new List<NcSourceInfo>();
            foreach (var temp_src_info in nc_sources)
            {
                src_list.Add(temp_src_info.Value);
            }
        }
        public async static void getSources()
        {
            Parameters result = await WebConnection.Connect(Constants.domain + "/pkuhelper/nc/source.php?token=" + Constants.token, new List<Parameters>());
            if (result.name != "200")
            {
                Constants.BoxPage.ShowMessage("获取订阅源失败！");
                return;
            }
            JsonArray sources;
            try
            {
                sources = JsonArray.Parse(result.value);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return;
            }
            nc_sources = new Dictionary<string, NcSourceInfo>();
            foreach (var source in sources)
            {
                try
                {
                    var obj = source.GetObject();
                    string tsid = obj.GetNamedString("sid");
                    string tname = obj.GetNamedString("name");
                    string turl = obj.GetNamedString("icon");
                    string tdesc = obj.GetNamedString("desc");
                    int tdefault = int.Parse(obj.GetNamedString("default"));
                    int tselected = (int)obj.GetNamedNumber("selected");
                    NcSourceInfo temp_src_info = new NcSourceInfo(tsid, tname, turl, tdesc, tdefault, tselected);
                    nc_sources.Add(tsid, temp_src_info);

                }
                catch
                {
                    Constants.BoxPage.ShowMessage("解析订阅源失败！");
                }

            }
        }
    }
}
