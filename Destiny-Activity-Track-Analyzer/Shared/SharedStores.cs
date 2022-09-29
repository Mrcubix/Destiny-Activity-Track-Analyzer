using Tracker.Shared.Stores;
using Tracker.ViewModels;

namespace Tracker.Shared
{
    public class SharedStores
    {
        public DefinitionsStore DefinitionsStore { get; set; }
        public SettingsStore SettingsStore { get; set; }

        public SharedStores(ViewModelBase vm)
        {
            SettingsStore = new();
            SettingsStore.LoadSettings(vm);
            
            DefinitionsStore = new(SettingsStore);
        }
    }
}