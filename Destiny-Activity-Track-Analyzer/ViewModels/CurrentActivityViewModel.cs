using System.Threading.Tasks;
using API.Endpoints;
using API.Entities.Characters;
using ReactiveUI;

namespace Tracker.ViewModels
{
    public class CurrentActivityViewModel : ViewModelBase
    {
        public Destiny2 API { get; }

        private DestinyCharacterComponent _character;
        public DestinyCharacterComponent Character
        {
            get => _character;
            set => this.RaiseAndSetIfChanged(ref _character, value);
        }

        private DestinyCharacterActivitiesComponent _activity;
        public DestinyCharacterActivitiesComponent CurrentActivity
        {
            get => _activity;
            set => this.RaiseAndSetIfChanged(ref _activity, value);
        }

        public CurrentActivityViewModel(Destiny2 api)
        {
            API = api;
        }

        public void GetActivityFromDefinition(long activityHash)
        {

        }

        public async Task StartTrackingCurrentActivity()
        {
            while(true)
            {
                await Task.Delay(1000);

                CurrentActivity = await API.GetCurrentActivity(Character.MembershipType, Character.GetMembershipId(), Character.GetCharacterId());
            }
        }
    }
}
