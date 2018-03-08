using HelperUWP.HoleRef;
using MailKit;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using static HelperUWP.MailRef.MailUtil;

namespace HelperUWP.MailRef
{
    class MailList : ObservableCollection<MailSummary>, ISupportIncrementalLoading
    {
        public FolderType type;
        private IMailFolder folder;
        private bool busy = false;
        private bool has_more_items = false;
        private int current_page = 0;
        private int _totalCount;
        public int TotalCount
        {
            get
            {
                return _totalCount;
            }
            private set
            {
                _totalCount = value;
            }
        }
        public event DataLoadingEventHandler DataLoading;
        public event DataLoadedEventHandler DataLoaded;
        public MailList(FolderType t = FolderType.Inbox)
        {
            type = t;
            folder = MailUtil.GetMailFolder(type);
            HasMoreItems = true;
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
        public void DoFresh()
        {
            current_page = 0;
            Clear();
            HasMoreItems = true;
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return InnerLoadMoreItemsAsync(count).AsAsyncOperation();
        }

        private async Task<LoadMoreItemsResult> InnerLoadMoreItemsAsync(uint count)
        {
            busy = true;
            var actualCount = 0;
            List<MailSummary> tempList = null;
            try
            {
                if (DataLoading != null)
                {
                    DataLoading();
                }
                tempList = await executeLoadMore();
            }
            catch
            {
                HasMoreItems = false;
            }
            if (tempList != null && tempList.Any())
            {
                actualCount = tempList.Count;
                TotalCount += actualCount;
                current_page++;
                HasMoreItems = true;
                tempList.ForEach((c) => { this.Add(c); });
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
            return new LoadMoreItemsResult { Count=(uint)actualCount};
            
        }

        private async Task<List<MailSummary>> executeLoadMore()
        {
            try
            {
                folder.Open(FolderAccess.ReadOnly);
                int lowBound, highBound;
                highBound = folder.Count - 20 * current_page-1;
                lowBound = highBound - 19;
                if (lowBound <= 0) lowBound = 0;
                List<MailSummary> temp = new List<MailSummary>();
                var summarys = await folder.FetchAsync(lowBound, highBound, MessageSummaryItems.Full | MessageSummaryItems.UniqueId);
                for (int i = summarys.Count-1; i >= 0; i--)
                {
                    temp.Add(new MailSummary(summarys[i].Index, summarys[i].NormalizedSubject, summarys[i].Date, summarys[i].Envelope.Sender));
                }
                return temp;
            }
            catch
            {
                return null;
            }
        }

        
    }
}
