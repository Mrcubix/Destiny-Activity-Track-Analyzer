using System.Linq;
using DynamicData.Kernel;
using ReactiveUI;
using Tracker.Shared.Frontend;
using Tracker.Shared.Stores;
using Tracker.Shared.Stores.Component;

namespace Tracker.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private SettingsStore _settings = null!;
        private DefaultsStore _defaults = null!;


        public SettingsStore SettingsStore
        {
            get => _settings;
            set => this.RaiseAndSetIfChanged(ref _settings, value);
        }

        public DefaultsStore DefaultsStore
        {
            get => _defaults;
            set => this.RaiseAndSetIfChanged(ref _defaults, value);
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
            // Note: The issue is not that it assign the value, but that the reference becomes absolete
            //Remote.SharedStores.SettingsStore.SettingsUpdated += OnSettingsChange;
            //Remote.SharedStores.DefaultsStore.DefaultsUpdated += OnDefaultsChange;
        }

        public void Save()
        {
            this.RaiseAndSetIfChanged(ref _settings, _settings);
            this.RaiseAndSetIfChanged(ref _defaults, _defaults);

            SettingsStore.Save();
            //DefaultsStore.Save();
        }

        public void OnSettingsChange(object? sender, AppSettings settings)
        {
            //Settings = settings;
        }

        public void OnDefaultsChange(object? sender, Defaults defaults)
        {
            //Defaults = defaults;
        }
    }
}