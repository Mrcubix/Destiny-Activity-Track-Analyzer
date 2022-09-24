using System;
using API.Endpoints;
using Tracker.Shared;

namespace Tracker.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public string Greeting => "Welcome to Avalonia!";
        private Destiny2 API { get; }

        public MainViewModel()
        {
            Remote.AddVM(this);

            SharedSettingsStore.LoadSettings();
            API = new(SharedSettingsStore.Settings.APISettings);

            Remote.AddVM(new CurrentActivityViewModel(API));


            Remote.CurrentViewModel = this;
        }

        public void LoadAPISettings()
        {
            
        }

    }
}
