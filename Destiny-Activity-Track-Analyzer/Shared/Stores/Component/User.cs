using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using API.Entities.Characters;
using API.Entities.User;
using ReactiveUI;

namespace Tracker.Shared.Stores.Component
{
    public class User : ReactiveObject
    {
        private UserInfoCard _userInfo = null!;
        private Dictionary<long, DestinyCharacterComponent> _characters = new();
        private DestinyCharacterComponent _currentCharacter = null!;


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

        /// <Summary>
        ///   Currently Selected Character
        /// </Summary>
        [JsonIgnore]
        public DestinyCharacterComponent CurrentCharacter
        {
            get => _currentCharacter;
            set => this.RaiseAndSetIfChanged(ref _currentCharacter, value);
        }
    }
}