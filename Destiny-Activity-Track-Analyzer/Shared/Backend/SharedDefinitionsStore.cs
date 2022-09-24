using System.Collections.Generic;
using API.Entities.Definitions;

namespace Tracker.Shared.Backend
{
    public static class SharedDefinitionsStore
    {
        public static Dictionary<long, DestinyActivityDefinition> ActivityDefinitions { get; set; } = new();
        //public static Dictionary<long, DestinyActivityModeDefinition> ActivityModeDefinitions { get; set; } = new();
        //public static Dictionary<long, DestinyActivityTypeDefinition> ActivityTypeDefinitions { get; set; } = new();

        public static void LoadActivityDefinitions()
        {

        }
    }
}