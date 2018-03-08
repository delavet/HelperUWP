using HelperUWP.HoleRef;
using HelperUWP.Lib;
using HelperUWP.Lib.Web;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace HelperUWP.NCref
{
    class NoticeInfoList : ObservableCollection<NoticeInfo>, ISupportIncrementalLoading
    {
        public static bool ALL
        {
            get
            {
                return false;
            }
        }
        public static bool ONE
        {
            get
            {
                return true;
            }
        }
        public int sid = 0;
        public bool type;
        private bool busy = false;
        private bool has_more_items = false;
        private int current_page = 0;
        public event DataLoadingEventHandler DataLoading;
        public event DataLoadedEventHandler DataLoaded;

        public int TotalCount { get; set; }

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
        public NoticeInfoList(int _sid)
        {
            has_more_items = true;
            if (_sid != -1)
            {
                type = ONE;
                sid = _sid;
            }
            else
            {
                type = ALL;
                sid = -1;
            }
        }
        public void DoRefresh()
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
            List<NoticeInfo> temp_list = null;
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
                Constants.BoxPage.ShowMessage("加载通知失败");
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

        private async Task<List<NoticeInfo>> execute_load_more()
        {
            String url = Constants.domain + "/pkuhelper/nc/fetch.php?token=" + Constants.token;
            url = url + "&p=" + (current_page + 1).ToString() + "&platform=Android";
            if (type == ONE) url = url + "&sid=" + sid.ToString();
            Parameters param = await WebConnection.Connect(url, new List<Parameters>());
            if (param.name != "200") return null;
            else
            {
                try
                {
                    JsonObject jsonObject = JsonObject.Parse(param.value);
                    int code = (int)jsonObject.GetNamedNumber("code");
                    if (code == 1)
                    {
                        Constants.BoxPage.ShowMessage("您还没有设置订阅源！请设置订阅源");
                        return null;
                    }
                    JsonArray array = jsonObject.GetNamedArray("data");
                    List<NoticeInfo> more_infos = new List<NoticeInfo>();
                    foreach (var info in array)
                    {
                        JsonObject obj = info.GetObject();
                        String tTitle = obj.GetNamedString("title");
                        int tnid = int.Parse(obj.GetNamedString("nid"));
                        String turl = obj.GetNamedString("url");
                        int tsid = int.Parse(obj.GetNamedString("sid"));
                        String tsubscribe = obj.GetNamedString("subscribe");
                        long ttime = long.Parse(obj.GetNamedString("time"));
                        NoticeInfo temp_info = new NoticeInfo(tnid, tTitle, tsid, turl, tsubscribe, ttime);
                        if (NcSourceQuery.nc_sources != null)
                        {
                            try
                            {
                                NcSourceInfo temp_source;
                                if (NcSourceQuery.nc_sources.TryGetValue(tsid.ToString(), out temp_source))
                                {
                                    temp_info.SourceIcon = temp_source.Icon;
                                    temp_info.SourceName = temp_source.Name;
                                }
                                else
                                {
                                    temp_info.SourceIcon = null;
                                    temp_info.SourceName = "Unknown source name";
                                }
                            }
                            catch(Exception e)
                            {
                                Debug.WriteLine(e.StackTrace);
                            }
                        }
                        more_infos.Add(temp_info);
                    }
                    return more_infos;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.StackTrace);
                    return null;
                }
            }
        }
    }
}
