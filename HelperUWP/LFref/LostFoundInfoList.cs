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
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace HelperUWP.LFref
{
    public enum LFstatus
    {
        Lost,Found,My
    }
    class LostFoundInfoList : ObservableCollection<LostFoundInfo>, ISupportIncrementalLoading
    {

        private LFstatus type;
        public LFstatus Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
                do_fresh();
            }
        }

        private bool busy = false;
        private bool has_more_items = false;
        private int current_page = 0;
        public int TotalCount;
        public event DataLoadingEventHandler DataLoading;
        public event DataLoadedEventHandler DataLoaded;
        public bool HasMoreItems
        {
            get
            {
                if (busy) return false;
                else return has_more_items;
            }
            private set
            {
                has_more_items = value;
            }
        }

        public LostFoundInfoList(LFstatus _type=LFstatus.Lost)
        {
            type = _type;
            has_more_items = true;
        }

        public void do_fresh()
        {
            current_page = 0;
            Clear();
            HasMoreItems = true;
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return InnerLoadMoreItemsAsync(count).AsAsyncOperation();
        }



        private async Task<LoadMoreItemsResult> InnerLoadMoreItemsAsync(uint expected_count)
        {
            busy = true;
            var actualCount = 0;
            List<LostFoundInfo> temp_list = null;
            try
            {
                if (DataLoading != null)
                {
                    DataLoading();
                }
                temp_list = await execute_load_more();
            }
            catch (Exception)
            {
                HasMoreItems = false;
            }
            if (temp_list != null && temp_list.Any())
            {
                actualCount = temp_list.Count;
                TotalCount += actualCount;
                current_page++;
                HasMoreItems = true;
                temp_list.ForEach((c) => { this.Add(c); });
            }
            else
            {
                //await (new MessageDialog("加载失物招领失败!")).ShowAsync();
                HasMoreItems = false;
            }
            if (DataLoaded != null)
            {
                DataLoaded();
            }
            busy = false;

            return new LoadMoreItemsResult
            {
                Count = (uint)actualCount
            };
        }

        private async Task<List<LostFoundInfo>> execute_load_more()
        {

            String type_str = "", token = "";
            switch (type)
            {
                case LFstatus.Lost:
                    {
                        type_str = "lost";
                    }
                    break;
                case LFstatus.Found:
                    {
                        type_str = "found";
                    }
                    break;
                case LFstatus.My:
                    {
                        token = Constants.token;
                    }
                    break;
                default: break;
            }
            Parameters result = await WebConnection.Connect(Constants.domain + "/services/LFList.php?type=" + type_str + "&page=" + (current_page).ToString() + "&token=" + token, new List<Parameters>());
            if (result.name != "200")
            {
                return null;
            }
            JsonObject json;
            JsonArray data;
            try
            {
                json = JsonObject.Parse(result.value);
                data = json.GetNamedArray("data");
            }
            catch (Exception)
            {
                return null;
            }
            List<LostFoundInfo> more_infos = new List<LostFoundInfo>();

            if (type != LFstatus.My)
            {
                try
                {
                    Boolean bmore = json.GetNamedBoolean("more");
                    if (!bmore) has_more_items = false;
                }
                catch (Exception)
                {

                }
            }

            foreach (var info in data)
            {
                try
                {
                    JsonObject data_obj = info.GetObject();

                    int tid = int.Parse(data_obj.GetNamedString("id"));
                    if (type == LFstatus.My)
                    {
                        foreach (var temp in this)
                        {
                            if (temp.id == tid)
                            {
                                HasMoreItems = false;
                                return null;
                            }
                        }
                    }
                    String tname = data_obj.GetNamedString("name");
                    String tlost_or_found = data_obj.GetNamedString("lost_or_found");
                    String ttypr = data_obj.GetNamedString("type");
                    String tdetail = data_obj.GetNamedString("detail");
                    long tpost_time = long.Parse(data_obj.GetNamedString("post_time"));
                    long taction_time = long.Parse(data_obj.GetNamedString("action_time"));
                    String timage = data_obj.GetNamedString("image");
                    String tposter_uid = data_obj.GetNamedString("poster_uid");
                    String tposter_phone = data_obj.GetNamedString("poster_phone");
                    String tposter_name = data_obj.GetNamedString("poster_name");
                    String tposter_college = data_obj.GetNamedString("poster_college");
                    LostFoundInfo temp_info = new LostFoundInfo(tid, tname, tlost_or_found,
                        ttypr, tdetail, tpost_time, taction_time, timage, tposter_uid, tposter_phone
                        , tposter_name, tposter_college);
                    if (type == LFstatus.My) temp_info.mine = true;
                    more_infos.Add(temp_info);
                }
                catch
                {

                }
            }
            return more_infos;
        }
    }
}
