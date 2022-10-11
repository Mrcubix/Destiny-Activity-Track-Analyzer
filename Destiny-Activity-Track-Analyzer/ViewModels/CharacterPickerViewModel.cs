using System.Collections.Generic;
using API.Entities.Characters;
using ReactiveUI;
using Tracker.Shared.Frontend;

namespace Tracker.ViewModels
{
    public class CharacterPickerViewModel : ViewModelBase
    {
        private Dictionary<long, DestinyCharacterComponent> _characters = null!;

        public CharacterPickerViewModel(ViewRemote remote)
        {
            Remote = remote;
            Characters = Remote.SharedStores.DefaultsStore.Defaults.Characters;
        }

        public Dictionary<long, DestinyCharacterComponent> Characters
        {
            get => _characters;
            set => this.RaiseAndSetIfChanged(ref _characters, value);
        }
    }
}
