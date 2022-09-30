using System.Threading;
using System.Threading.Tasks;
using API.Endpoints;

namespace Tracker.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public string Greeting => "Welcome to Avalonia!";
        private Destiny2 API { get; }

        public MainViewModel()
        {
            Remote = new();
            Remote.SharedStores = new(this);
            Remote.AddVM(this);

            API = new(Remote.SharedStores.SettingsStore.Settings.APISettings);

            var sharedDefinitionsStore = Remote.SharedStores.DefinitionsStore;
            
            sharedDefinitionsStore.Initialize();
            sharedDefinitionsStore.LoadDefinitions();
            _ = Task.Run(Remote.SharedStores.SettingsStore.Refresh);

            Remote.AddVM(new CurrentActivityViewModel(API, Remote));
            Remote.AddVM(new SettingsViewModel(Remote));
        }
    }
}
