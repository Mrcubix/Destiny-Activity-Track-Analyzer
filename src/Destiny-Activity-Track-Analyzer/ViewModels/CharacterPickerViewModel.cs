using System.Threading.Tasks;
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

        public override bool ShouldEnquire(AppSettings settings)
        {
            bool keySet = SettingsStore.IsKeySet;

            return UserStore.User.CurrentCharacter == null && keySet && settings.UXSettings.ShouldEnquire;
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
