using System.Collections.Generic;
using ReactiveUI;
using API.Enums;
using API.Entities.User;
using API.Entities.Characters;

namespace Tracker.Shared.Stores.Component
{
    public class UXSettings : ReactiveObject
    {
        private Dictionary<DestinyComponentType, bool> _componentVisibility = new();
        private bool _shouldEnquire = true;
        private int _tracksPerPage = 10;
        private int _activitiesPerPage = 10;

        // TODO: Replace bool with an Class containing Display name and state
        public Dictionary<DestinyComponentType, bool> ComponentVisibility
        {
            get => _componentVisibility;
            set => this.RaiseAndSetIfChanged(ref _componentVisibility, value);
        }

        public bool ShouldEnquire
        {
            get => _shouldEnquire;
            set => this.RaiseAndSetIfChanged(ref _shouldEnquire, value);
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
