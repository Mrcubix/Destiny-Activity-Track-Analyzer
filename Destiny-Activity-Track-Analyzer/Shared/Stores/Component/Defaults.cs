using System.Collections.Generic;
using API.Entities.Characters;
using API.Entities.User;
using ReactiveUI;

namespace Tracker.Shared.Stores.Component
{
    public class Defaults : ReactiveObject
    {
        private string _defaultViewModelName = "";
        private DestinyCharacterComponent _defaultharacter = null!;


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
    }
}