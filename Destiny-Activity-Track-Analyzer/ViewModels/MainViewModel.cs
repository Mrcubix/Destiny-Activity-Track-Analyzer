using API.Endpoints;

namespace Tracker.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public string Greeting => "Welcome to Avalonia!";
        private Destiny2 API { get; }

        public MainViewModel()
        {
            Remote.AddVM(this);

            Remote.SharedStores = new(this);

            API = new(Remote.SharedStores.SettingsStore.Settings.APISettings);

            Remote.AddVM(new CurrentActivityViewModel(API));

            var sharedDefinitionsStore = Remote.SharedStores.DefinitionsStore;
            
            sharedDefinitionsStore.Initialize();
            sharedDefinitionsStore.LoadDefinitions();
        }
    }
}
