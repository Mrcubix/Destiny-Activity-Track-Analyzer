using System.Collections.Generic;
using ReactiveUI;
using Tracker.ViewModels;
using API.Enums;
using API.Entities.User;
using API.Entities.Characters;

namespace Tracker.Shared.Stores.Component
{
    public class UXSettings : ReactiveObject
    {
        public UserInfoCard UserInfo { get; set; } = new();
        public DestinyCharacterComponent DefaultCharacter { get; set; } = new();
        public List<DestinyCharacterComponent> Characters { get; set; } = new();
        public string DefaultViewModelName { get; set; } = "";
        // TODO: Replace bool with an Class containing Display name and state
        public Dictionary<DestinyComponentType, bool> ComponentVisibility { get; set; } = new();
        public int TracksPerPage { get; set; } = 10;
        public int ActivitiesPerPage { get; set; } = 10;
    }
}
