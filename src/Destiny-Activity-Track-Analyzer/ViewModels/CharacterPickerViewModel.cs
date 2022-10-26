using ReactiveUI;
using Tracker.Shared.Frontend;
using Tracker.Shared.Stores;

namespace Tracker.ViewModels
{
    public class CharacterPickerViewModel : ViewModelBase
    {
        private UserStore _userStore = null!;
        private DefaultsStore _defaultsStore = null!;

        public UserStore UserStore
        {
            get => _userStore;
            set => this.RaiseAndSetIfChanged(ref _userStore, value);
        }

        public DefaultsStore DefaultsStore
        {
            get => _defaultsStore;
            set => this.RaiseAndSetIfChanged(ref _defaultsStore, value);
        }

        public CharacterPickerViewModel(ViewRemote remote, string name = "") : base(remote, name)
        {
            UserStore = Remote.SharedStores.UserStore;
            DefaultsStore = Remote.SharedStores.DefaultsStore;
        }

        public void OnCharacterSelect(long id)
        {
            UserStore.SetCharacter(UserStore.User.Characters[id]);
            Remote.ShowView("CurrentActivityViewModel");
        }
    }
}
