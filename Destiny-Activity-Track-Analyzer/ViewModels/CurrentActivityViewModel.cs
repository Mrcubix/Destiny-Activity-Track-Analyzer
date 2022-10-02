using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Endpoints;
using API.Entities.Characters;
using API.Entities.Definitions;
using ReactiveUI;
using Tracker.Shared.Frontend;

namespace Tracker.ViewModels
{
    public class CurrentActivityViewModel : ViewModelBase
    {
        public Destiny2 API { get; set; }

        private DestinyCharacterComponent _character = null!;
        public DestinyCharacterComponent Character
        {
            get => _character;
            set => this.RaiseAndSetIfChanged(ref _character, value);
        }

        private DestinyActivityDefinition _currentActivity = null!;
        public DestinyActivityDefinition CurrentActivity
        {
            get => _currentActivity;
            set => this.RaiseAndSetIfChanged(ref _currentActivity, value);
        }

        private Dictionary<uint, DestinyActivityDefinition> items => Remote.SharedStores.DefinitionsStore.ActivityDefinitions.Items;

        public CurrentActivityViewModel(Destiny2 api)
        {
            API = api;

            _ = Task.Run(StartTrackingCurrentActivity);
        }

        public CurrentActivityViewModel(ViewRemote remote)
        {
            API = new(remote.SharedStores.SettingsStore.Settings.APISettings);
            Remote = remote;

            _ = Task.Run(StartTrackingCurrentActivity);
        }

        public CurrentActivityViewModel(Destiny2 api, ViewRemote remote)
        {
            API = api;
            Remote = remote;

            _ = Task.Run(StartTrackingCurrentActivity);
        }

        public async Task StartTrackingCurrentActivity()
        {
            while(true)
            {
                await Task.Delay(1000);

                if (Character != null)
                {
                    var activities = await API.GetCurrentActivity(Character.MembershipType, Character.GetMembershipId(), Character.GetCharacterId());

                    if (activities.CurrentActivityHash != 0 && items.ContainsKey(activities.CurrentActivityHash))
                        CurrentActivity = items[activities.CurrentActivityHash];
                }
                else
                {
                    if (Remote.SharedStores.SettingsStore.Settings.UXSettings.Characters.Count != 0)
                        Character = Remote.SharedStores.SettingsStore.Settings.UXSettings.Characters.Values.ElementAt(0);
                }
            }
        }
    }
}
