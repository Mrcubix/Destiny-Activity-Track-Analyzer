using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API;
using API.Endpoints;
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

        public static async Task UpdateDefinitions()
        {
            var api = new Destiny2(SharedSettingsStore.Settings.APISettings);
            var manifest = await api.GetDestinyManifest();
            var definitionDBURL = manifest.MobileWorldContentPaths;
            var serializedDefinitionDB = await api.SendRequest("GET", new Uri($"Bungie.net{definitionDBURL}"));
        }
    }
}