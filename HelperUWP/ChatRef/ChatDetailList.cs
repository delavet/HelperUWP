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
    public delegate void NewDataLoaded();
    class ChatDetailList : ObservableCollection<ChatDetailInfo>
    {
        public event DataLoadingEventHandler DataLoading;
        public event DataLoadedEventHandler DataLoaded;
        public event NewDataLoaded NewLoaded;
        private int CurrentPage = 0;
        private String ToUid;
        private bool hasmore=true;
        private bool loading=false;
        private object o = new object();
        private HashSet<int> idSet;
        public bool HasMore
        {
            get
            {
                lock (o)
                {
                    if (loading) return false;
                    else return hasmore;
                }
            }
        }
        public ChatDetailList(String uid)
        {
            ToUid = uid;
        }
        
        public async Task<bool> HasNew()
        {
            Parameters result = await WebConnection.Connect(Constants.domain + "/pkuhelper/../services/hasnew.php?to=" + ToUid + "&uid=" + Constants.username+"&?="+DateTime.Now.Millisecond,null);
            if (!result.name.Equals("200")) return false;
            if (result.value.Contains("1")) return true;
            return false;
        }
        public async Task LoadNew()
        {
            List<Parameters> param = new List<Parameters>();
            param.Add(new Parameters("newonly", "1"));
            param.Add(new Parameters("page", "1"));
            param.Add(new Parameters("to", ToUid));
            param.Add(new Parameters("token", Constants.token));
            param.Add(new Parameters("type", "getdetail"));
            Parameters result = await WebConnection.Connect(Constants.domain + "/pkuhelper/../services/msg.php?api_version=2", param);
            if (result.name != "200")
            {
                Util.DealWithDisconnect(result);
            }
            else
            {
                JsonObject temp_json = JsonObject.Parse(result.value);
                int code = (int)temp_json.GetNamedNumber("code");
                if (code != 0) return;
                else await AddNew(result.value);
                
            }                      
        }

        private async Task AddNew(string jsonStr)
        {
            try
            {
                JsonObject temp_json = JsonObject.Parse(jsonStr);
                JsonArray info_array = temp_json.GetNamedArray("data");
                List<ChatDetailInfo> temp_list = new List<ChatDetailInfo>();
                foreach (var temp in info_array)
                {
                    JsonObject info = temp.GetObject();
                    int _id = int.Parse(info.GetNamedString("id"));
                    //if (idSet.Contains(_id)) continue;
                    //idSet.Add(_id);
                    String _content = info.GetNamedString("content");
                    String _mime = info.GetNamedString("mime");
                    long _timestamp = long.Parse(info.GetNamedString("timestamp"));
                    String _type = info.GetNamedString("type");
                    ChatDetailInfo temp_info = new ChatDetailInfo(_id, _content, _mime, _timestamp, _type);
                    temp_list.Add(temp_info);
                }
                temp_list.Sort(reverse_cmp);
                foreach (var x in temp_list)
                {
                    this.Add(x);
                }
                await Task.Delay(200);
                NewLoaded?.Invoke();
            }
            catch
            {
                return;
            }
        }

        public async Task LoadMoreHistory()
        {
            if (!HasMore) return;
            lock (o)
            {
                loading = true;
            }
            DataLoading?.Invoke();
            CurrentPage++;
            List<Parameters> param = new List<Parameters>();
            param.Add(new Parameters("to", ToUid));
            param.Add(new Parameters("type", "getdetail"));
            param.Add(new Parameters("page", CurrentPage.ToString()));
            param.Add(new Parameters("token", Constants.token));
            Parameters result = await WebConnection.Connect(Constants.domain + "/pkuhelper/../services/msg.php?api_version=2", param);
            if (result.name != "200")
            {
                Util.DealWithDisconnect(result);
            }
            else
            {
                JsonObject temp_json = JsonObject.Parse(result.value);
                int code = (int)temp_json.GetNamedNumber("code");
                if (code != 0) Constants.BoxPage.ShowMessage("获取聊天记录失败");
                else AddHistory(result.value);
            }
            DataLoaded?.Invoke();
            lock (o)
            {
                loading = false;
            }
        }
        private int cmp(ChatDetailInfo a, ChatDetailInfo b)
        {
            if (a.Timestamp > b.Timestamp) return -1;
            else if (a.Timestamp < b.Timestamp) return 1;
            return 0;
        }

        private int reverse_cmp(ChatDetailInfo a, ChatDetailInfo b)
        {
            if (a.Timestamp > b.Timestamp) return 1;
            else if (a.Timestamp < b.Timestamp) return -1;
            return 0;
        }

        private bool JudgeShowTime(ChatDetailInfo newone,ChatDetailInfo oldone)
        {
            
            return Math.Abs(newone.Timestamp - oldone.Timestamp) > 72000000;
        }

        private void AddHistory(String jsonStr)
        {
            try
            {
                JsonObject temp_json = JsonObject.Parse(jsonStr);
                JsonArray info_array = temp_json.GetNamedArray("data");
                List<ChatDetailInfo> temp_list = new List<ChatDetailInfo>();
                foreach (var temp in info_array)
                {
                    JsonObject info = temp.GetObject();
                    int _id = 0;
                    try
                    {
                        _id = (int)info.GetNamedNumber("id");
                    }
                    catch
                    {
                        _id = int.Parse(info.GetNamedString("id"));
                    }
                    //if (idSet.Contains(_id)) continue;
                    //idSet.Add(_id);
                    String _content = info.GetNamedString("content");
                    String _mime = info.GetNamedString("mime");
                    long _timestamp = 0;
                    try
                    {
                        _timestamp = long.Parse(info.GetNamedString("timestamp"));
                    }
                    catch
                    {
                        _timestamp = (long)info.GetNamedNumber("timestamp");
                    }
                    String _type = info.GetNamedString("type");
                    ChatDetailInfo temp_info = new ChatDetailInfo(_id, _content, _mime, _timestamp, _type);
                    temp_list.Add(temp_info);
                }
                temp_list.Sort(cmp);
                if (temp_list.Count == 0)
                {
                    lock (o)
                    {
                        hasmore = false;
                    }
                }
                for(int i=0;i<temp_list.Count;i++)
                {
                    if (i != temp_list.Count - 1)
                        temp_list[i].ShowTime = JudgeShowTime(temp_list[i], temp_list[i + 1]);
                    this.Insert(0, temp_list[i]);
                }
            }
            catch
            {
                return;
            }
        }
    }
}
