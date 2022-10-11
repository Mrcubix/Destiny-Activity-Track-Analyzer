using ReactiveUI;
using Tracker.Shared.Frontend;
using Tracker.Shared.Stores;

namespace Tracker.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private SettingsStore _settingsStore = null!;
        private DefaultsStore _defaultsStore = null!;


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


        public SettingsViewModel(ViewRemote remote)
        {
            Remote = remote;
            SettingsStore = Remote.SharedStores.SettingsStore;
            DefaultsStore = Remote.SharedStores.DefaultsStore;

            Initialize();
        }


        public void Initialize()
        {
            // Note: The issue is not that it assign the value, but that the reference becoming absolete
            //Remote.SharedStores.SettingsStore.SettingsUpdated += OnSettingsChange;
            //Remote.SharedStores.DefaultsStore.DefaultsUpdated += OnDefaultsChange;
        }

        public void Save()
        {
            //this.RaiseAndSetIfChanged(ref _settings, _settings);
            //this.RaiseAndSetIfChanged(ref _defaults, _defaults);

            SettingsStore.Save();
            DefaultsStore.Save();
        }
    }
}