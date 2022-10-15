using System.Threading.Tasks;
using ReactiveUI;
using Tracker.Shared.Frontend;
using Tracker.Shared.Stores;

namespace Tracker.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private SettingsStore _settingsStore = null!;
        private DefaultsStore _defaultsStore = null!;
        private UserStore _userStore = null!;


        public SettingsStore SettingsStore
        {
            get => _settingsStore;
            set => this.RaiseAndSetIfChanged(ref _settingsStore, value);
        }

        public DefaultsStore DefaultsStore
        {
            get => _defaultsStore;
            set => this.RaiseAndSetIfChanged(ref _defaultsStore, value);
        }

        public UserStore UserStore
        {
            get => _userStore;
            set => this.RaiseAndSetIfChanged(ref _userStore, value);
        }


        public SettingsViewModel(ViewRemote remote)
        {
            Remote = remote;
            SettingsStore = Remote.SharedStores.SettingsStore;
            DefaultsStore = Remote.SharedStores.DefaultsStore;
            UserStore = Remote.SharedStores.UserStore;
        }


        public void Save()
        {
            SettingsStore.Save();
            DefaultsStore.Save();
            UserStore.Save();
            _ = Task.Run(Remote.SharedStores.EmblemStore.Update);
        }
    }
}