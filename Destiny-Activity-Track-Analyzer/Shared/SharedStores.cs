using Tracker.Shared.Stores;
using Tracker.ViewModels;

namespace Tracker.Shared
{
    public class SharedStores
    {
        public DefinitionsStore DefinitionsStore { get; set; } = null!;
        public SettingsStore SettingsStore { get; set; } = new();

        public SharedStores(ViewModelBase vm)
        {
            SettingsStore.LoadSettings(vm);
            DefinitionsStore = new(SettingsStore);
        }
    }
}