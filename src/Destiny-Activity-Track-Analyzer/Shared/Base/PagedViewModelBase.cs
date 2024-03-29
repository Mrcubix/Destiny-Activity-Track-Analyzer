using System.Collections.ObjectModel;

namespace Tracker.Shared.Base
{
    public interface PagedViewModelBase
    {
        public int PageNumber { get; set; }
        public int Count { get; set; }
        public ObservableCollection<object> CurrentItemList { get; set; }
        public void NextPage();
        public void PreviousPage();
    }
}
