using System.Threading.Tasks;
using Tracker.Shared;
using Tracker.Shared.Frontend;
using Tracker.Shared.Stores;
using Tracker.Shared.Stores.Component;

namespace Tracker.ViewModels
{
    public class CharacterPickerViewModel : EnquiryViewModelBase
    {
        public CharacterPickerViewModel(ViewRemote remote, string name = "") : base(remote, name)
        {
        }

        public override bool ShouldEnquire(SharedStores stores)
        {
            bool keySet = SettingsStore.IsKeySet;


            // Has the settings loaded yet?
            if (!Remote.SharedStores.HasLoaded)
                return false;

            // Are the other properties set? (To avoid conflicts with other enquiries)
            if (!keySet || !UserStore.IsUserSet)
                return false;

            return UserStore.User.CurrentCharacter == null && SettingsStore.Settings.UXSettings.ShouldEnquire;
        }

        public override void Enquire()
        {
            Remote.ShowView("Character Picker");
        }

        public override async Task Save()
        {
            await Task.Delay(0);
        }

        public void Save(long id)
        {
            UserStore.SetCharacter(UserStore.User.Characters[id]);
            UserStore.Save();
            Remote.ShowView("Current Activity");
        }

        public override void Skip()
        {

        }
    }
}
