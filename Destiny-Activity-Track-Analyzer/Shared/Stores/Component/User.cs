using System.Collections.Generic;
using API.Entities.Characters;
using API.Entities.User;
using ReactiveUI;

namespace Tracker.Shared.Stores.Component
{
    public class User : ReactiveObject
    {
        private UserInfoCard _userInfo = null!;
        private Dictionary<long, DestinyCharacterComponent> _characters = new();


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