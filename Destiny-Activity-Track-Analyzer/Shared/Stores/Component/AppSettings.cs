using API;
using ReactiveUI;

namespace Tracker.Shared.Stores.Component
{
    public class AppSettings : ReactiveObject
    {
        private APISettings _apiSettings = new();
        public APISettings APISettings
        {
            get => _apiSettings;
            set => this.RaiseAndSetIfChanged(ref _apiSettings, value);
        }

        private UXSettings _uxSettings = new();
        public UXSettings UXSettings
        {
            get => _uxSettings;
            set => this.RaiseAndSetIfChanged(ref _uxSettings, value);
        }
    }
}