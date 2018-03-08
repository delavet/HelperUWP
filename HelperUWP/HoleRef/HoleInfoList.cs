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

namespace HelperUWP.HoleRef
{
    public enum HoleListType
    {
        normal,attention
    }
    class HoleInfoList : ObservableCollection<HoleInfo>
    {
        public static int WaterCol = 2;
        private bool busy = false;
        private bool has_more_items = false;
        private object o = new object();
        private int current_page = 0;
        private HoleListType type;

        public event DataLoadingEventHandler DataLoading;
        public event DataLoadedEventHandler DataLoaded;
        public int TotalCount
        {
            get; set;
        }
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
        public void SetHoleType(HoleListType t)
        {
            type = t;
            DoFresh();
        }

        public HoleInfoList(HoleListType _type=HoleListType.normal)
        {
            has_more_items = true;
            type = _type;
        }
        public async void DoFresh()
        {
            current_page = 0;
            Clear();
            HasMoreItems = true;
            await LoadMoreItemsAsync();
        }

       
        public async Task LoadMoreItemsAsync()
        {
            if (busy) return;
            lock (o)
            {
                busy = true;                
            }
            var actualCount = 0;
            List<HoleInfo> temp_list = null;
            try
            {
                if (DataLoading != null)
                {
                    DataLoading();
                }
                temp_list = await excute_load_more();
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
                HasMoreItems = false;
            }
            if (DataLoaded != null)
            {
                DataLoaded();
            }
            busy = false;

            return;
        }

        private async Task<List<HoleInfo>> excute_load_more()
        {
            String url = Constants.domain + "/services/pkuhole/api.php";
            //List<Parameters> list = new List<Parameters>();
            if (type == HoleListType.normal)
            {
                url = url + "?action=getlist&p=" + (current_page + 1)+"&?="+DateTime.Now.Millisecond;
            }
            else
            {
                url = url + "?action=getattention&p=" + (current_page + 1)+"&token="+Constants.token + "&?=" + DateTime.Now.Millisecond;
            }
            //list.Add(new Parameters("p", (current_page + 1).ToString()));
            Parameters param = await WebConnection.Connect(url, null);
            if (param.name != "200")
            {
                Util.DealWithDisconnect(param);
                return null;
            }
            else
            {
                try
                {
                    JsonObject jsonObject = JsonObject.Parse(param.value);
                    int code = (int)jsonObject.GetNamedNumber("code");
                    if (code != 0) return null;
                    JsonArray array = jsonObject.GetNamedArray("data");
                    List<HoleInfo> more_infos = new List<HoleInfo>();
                    foreach (var info in array)
                    {
                        JsonObject obj = info.GetObject();
                        int tpid = int.Parse(obj.GetNamedString("pid"));
                        String ttext = obj.GetNamedString("text");
                        long tstamp = long.Parse(obj.GetNamedString("timestamp"));
                        String ttype = obj.GetNamedString("type");
                        int treply = int.Parse(obj.GetNamedString("reply"));
                        int tlike = int.Parse(obj.GetNamedString("likenum"));
                        int textra = int.Parse(obj.GetNamedString("extra"));
                        String turl = obj.GetNamedString("url");
                        HoleInfo hole_info = new HoleInfo(tpid, ttext, tstamp
                            , ttype, treply, tlike, textra, turl);
                        more_infos.Add(hole_info);
                    }
                    return more_infos;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }
    }
}
