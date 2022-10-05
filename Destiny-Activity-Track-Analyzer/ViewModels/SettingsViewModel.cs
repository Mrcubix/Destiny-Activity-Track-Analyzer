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
            Settings = Remote.SharedStores.SettingsStore.Settings;

            Initialize();
        }

        public void Initialize()
        {
            // Note: The issue is not that it assign the value, but that the reference becomes absolete
            Remote.SharedStores.SettingsStore.SettingsUpdated += OnSettingsChange;
        }

        public void Save()
        {
            this.RaiseAndSetIfChanged(ref _settings, _settings);
            Remote.SharedStores.SettingsStore.Save();
        }

        public void OnSettingsChange(object? send, AppSettings settings)
        {
            this.RaiseAndSetIfChanged(ref _settings, settings);
        }
    }
}