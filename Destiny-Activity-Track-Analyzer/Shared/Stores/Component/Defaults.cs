using System.Collections.Generic;
using API.Entities.Characters;
using API.Entities.User;
using ReactiveUI;

namespace Tracker.Shared.Stores.Component
{
    public class Defaults : ReactiveObject
    {
        private string _defaultViewModelName = "";
        private UserInfoCard _userInfo = null!;
        private DestinyCharacterComponent _defaultharacter = null!;
        private Dictionary<long, DestinyCharacterComponent> _characters = new();


        // TODO: Default values should be stored in a different file (defaults.json) and serialized under the DefaultValues class
        public string DefaultViewModelName
        {
            get => _defaultViewModelName;
            set => this.RaiseAndSetIfChanged(ref _defaultViewModelName, value);
        }

        /// <Summary>
        ///   Dictionary Containing all activities as hash : <see cref="{DestinyDefinition}"/>
        /// </Summary>
        public DestinyCharacterComponent DefaultCharacter
        {
            get => _defaultharacter;
            set => this.RaiseAndSetIfChanged(ref _defaultharacter, value);
        }

        /// <Summary>
        ///   Name of the definition in question
        /// </Summary>
        public UserInfoCard UserInfo
        {
            get => _userInfo;
            set => this.RaiseAndSetIfChanged(ref _userInfo, value);
        }

        /// <Summary>
        ///   Property existing for the sole purpose of knowing if the definition is empty inside a control
        /// </Summary>
        public Dictionary<long, DestinyCharacterComponent> Characters
        {
            get => _characters;
            set => this.RaiseAndSetIfChanged(ref _characters, value);
        }
    }
}