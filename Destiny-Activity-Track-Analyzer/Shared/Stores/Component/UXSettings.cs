using System.Collections.Generic;
using ReactiveUI;
using API.Enums;
using API.Entities.User;
using API.Entities.Characters;

namespace Tracker.Shared.Stores.Component
{
    public class UXSettings : ReactiveObject
    {
        private UserInfoCard _userInfo = new();
        private DestinyCharacterComponent _defaultCharacter = null!;
        private Dictionary<long, DestinyCharacterComponent> _characters = new();
        private string _defaultViewModelName = "";
        private Dictionary<DestinyComponentType, bool> _componentVisibility = new();
        private int _tracksPerPage = 10;
        private int _activitiesPerPage = 10;


        // TODO: Default values should be stored in a different file (defaults.json) and serialized under the DefaultValues class
        public UserInfoCard UserInfo
        {
            get => _userInfo;
            set => this.RaiseAndSetIfChanged(ref _userInfo, value);
        }
        
        public DestinyCharacterComponent DefaultCharacter
        {
            get => _defaultCharacter;
            set => this.RaiseAndSetIfChanged(ref _defaultCharacter, value);
        }

        // TODO: Default values should be stored in a different file (defaults.json) and serialized under the DefaultValues class
        public Dictionary<long, DestinyCharacterComponent> Characters
        {
            get => _characters;
            set => this.RaiseAndSetIfChanged(ref _characters, value);
        }

        // TODO: Default values should be stored in a different file (defaults.json) and serialized under the DefaultValues class
        public string DefaultViewModelName
        {
            get => _defaultViewModelName;
            set => this.RaiseAndSetIfChanged(ref _defaultViewModelName, value);
        }

        // TODO: Replace bool with an Class containing Display name and state
        public Dictionary<DestinyComponentType, bool> ComponentVisibility
        {
            get => _componentVisibility;
            set => this.RaiseAndSetIfChanged(ref _componentVisibility, value);
        }

        public int TracksPerPage
        {
            get => _tracksPerPage;
            set => this.RaiseAndSetIfChanged(ref _tracksPerPage, value);
        }

        public int ActivitiesPerPage
        {
            get => _activitiesPerPage;
            set => this.RaiseAndSetIfChanged(ref _activitiesPerPage, value);
        }
    }
}
