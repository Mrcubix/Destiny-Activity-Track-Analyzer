using System.Collections.Generic;
using API.Entities.Characters;
using ReactiveUI;
using Tracker.Shared.Frontend;
using Tracker.Shared.Stores;
using Tracker.Shared.Stores.Component;

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
