using HelperUWP.HoleRef;
using HelperUWP.Lib;
using HelperUWP.Lib.Web;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace HelperUWP.ChatRef
{
    class ChatMasterList:ObservableCollection<ChatListInfo>
    {
        private Dictionary<String, int> records = new Dictionary<string, int>();
        public event DataLoadingEventHandler DataLoading;
        public event DataLoadedEventHandler DataLoaded;

        private static int my_compare(ChatListInfo a, ChatListInfo b)
        {
            if (a.Timestamp > b.Timestamp) return -1;
            else if (a.Timestamp < b.Timestamp) return 1;
            return 0;
        }
        private static int my_rev_compare(ChatListInfo a, ChatListInfo b)
        {
            if (a.Timestamp > b.Timestamp) return 1;
            else if (a.Timestamp < b.Timestamp) return -1;
            return 0;
        }

        public async Task<List<ChatListInfo>> GetList()
        {
            List<Parameters> param = new List<Parameters>();
            param.Add(new Parameters("token", Constants.token));
            param.Add(new Parameters("type", "getlist"));
            Parameters result = await WebConnection.Connect(Constants.domain + "/pkuhelper/../services/msg.php", param);
            if (result.name != "200")
            {
                Util.DealWithDisconnect(result);
                return null;
            }
            try
            {
                JsonObject json = JsonObject.Parse(result.value);
                int code = (int)json.GetNamedNumber("code");
                if (code != 0)
                {
                    Constants.BoxPage.ShowMessage("获取聊天列表失败");
                    return null;
                }
                JsonArray info_array = json.GetNamedArray("data");
                var temp_list = new List<ChatListInfo>();
                foreach (var temp in info_array)
                {
                    JsonObject info = temp.GetObject();
                    String tusername = info.GetNamedString("username");
                    long ttimestamp;
                    try
                    {
                        ttimestamp = long.Parse(info.GetNamedString("timestamp")) * 1000;
                    }
                    catch
                    {
                        ttimestamp = (long)info.GetNamedNumber("timestamp") * 1000;
                    }
                    String tcontent = info.GetNamedString("content");
                    int tnum;
                    try
                    {
                        tnum = (int)info.GetNamedNumber("number");
                    }
                    catch
                    {
                        tnum = int.Parse(info.GetNamedString("number"));
                    }
                    int thasnew = (int)info.GetNamedNumber("hasNew");
                    String tname = info.GetNamedString("name", tusername);
                    ChatListInfo temp_info = new ChatListInfo(tusername, tname, ttimestamp, tcontent, tnum, thasnew > 0);
                    temp_list.Add(temp_info);
                }
                return temp_list;
            }
            catch (Exception e)
            {
                Constants.BoxPage.ShowMessage("解析聊天列表失败" + e.StackTrace);
                return null;
            }
            
        }

        public bool RealNew(List<ChatListInfo> infos)
        {
            foreach(var info in infos)
            {
                if (!records.ContainsKey(info.UserName)) return true;
                if (!this[records[info.UserName]].Equals(info)) return true;
            }
            return false;
        }

        public void Replace(List<ChatListInfo> infos)
        {
            this.Clear();
            records.Clear();
            try
            {
                infos.Sort(my_compare);
                foreach (var x in infos)
                {
                    this.Insert(0, x);
                }
                for (int i = 0; i < this.Count; i++)
                {
                    records.Add(this[i].UserName, i);
                }
            }
            catch
            {

            }
        }

        public async Task refresh()
        {
            if (DataLoading != null) DataLoading();
            this.Clear();
            records.Clear();
            try
            {
                var temp_list = await GetList();
                temp_list.Sort(my_compare);
                foreach (var x in temp_list)
                {
                    this.Insert(0, x);
                }
                for(int i = 0; i < this.Count; i++)
                {
                    records.Add(this[i].UserName, i);
                }
            }
            catch
            {
                
            }
            finally
            {
                if (DataLoaded != null) DataLoaded();
            }
        }
    }

}
