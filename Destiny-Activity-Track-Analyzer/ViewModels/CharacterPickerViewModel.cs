using ReactiveUI;
using Tracker.Shared.Frontend;
using Tracker.Shared.Stores;

namespace Tracker.ViewModels
{
    public class CharacterPickerViewModel : ViewModelBase
    {
        private UserStore _userStore = null!;


        public UserStore UserStore
        {
            get => _userStore;
            set => this.RaiseAndSetIfChanged(ref _userStore, value);
        }
        

        public CharacterPickerViewModel(ViewRemote remote)
        {
            Remote = remote;
            UserStore = Remote.SharedStores.UserStore;
        }
    }
}
