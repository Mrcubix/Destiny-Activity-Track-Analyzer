using System;
using System.Collections.ObjectModel;
using ReactiveUI;

namespace Tracker.ViewModels
{
    public interface PagedViewModelBase
    {
        public int PageNumber { get; set; }
        public int Count { get; set; }
        public ObservableCollection<object> CurrentItemList { get; set; }
        public void NextPage();
        public void PreviousPage();
        public string GetPageName();
    }
}
