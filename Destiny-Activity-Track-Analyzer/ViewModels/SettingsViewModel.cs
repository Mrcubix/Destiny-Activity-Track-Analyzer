using ReactiveUI;
using Tracker.Shared.Frontend;
using Tracker.Shared.Stores.Component;

namespace Tracker.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private AppSettings _settings = null!;

        public AppSettings Settings
        {
            get => _settings;
            set => this.RaiseAndSetIfChanged(ref _settings, value);
        }

        public SettingsViewModel(ViewRemote remote)
        {
            Remote = remote;
            Settings = remote.SharedStores.SettingsStore.Settings;
        }
    }
}