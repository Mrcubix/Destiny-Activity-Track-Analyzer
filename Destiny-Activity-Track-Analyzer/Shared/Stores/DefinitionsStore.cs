using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using API.Endpoints;
using API.Entities.Definitions;
using Microsoft.Data.Sqlite;
using ReactiveUI;
using Tracker.Shared.Static;
using Tracker.Shared.Stores.Component;

namespace Tracker.Shared.Stores
{
    public class DefinitionsStore : ReactiveObject
    {
        private SettingsStore settingsStore { get; set; } = null!;

        private bool _isUpdating = false;
        public bool IsUpdating
        {
            get => _isUpdating;
            set => this.RaiseAndSetIfChanged(ref _isUpdating, value);
        }

        private Dictionary<string, object> _definitions = new();
        public Dictionary<string, object> Definitions
        {
            get => _definitions;
            set => this.RaiseAndSetIfChanged(ref _definitions, value);
        }

        private Definition<DestinyActivityDefinition> _activityDefinitions = new("DestinyActivityDefinition");
        public Definition<DestinyActivityDefinition> ActivityDefinitions 
        {
            get => _activityDefinitions;
            set => this.RaiseAndSetIfChanged(ref _activityDefinitions, value);
        }
        //public static Dictionary<long, DestinyActivityModeDefinition> ActivityModeDefinitions { get; set; } = new();
        //public static Dictionary<long, DestinyActivityTypeDefinition> ActivityTypeDefinitions { get; set; } = new();

        public DefinitionsStore(SettingsStore settings)
        {
            settingsStore = settings;
        }

        public void Initialize()
        {
            Definitions = new()
            {
                ["DestinyActivityDefinition"] = ActivityDefinitions
            };
        }

        public void LoadDefinitions()
        {
            foreach(var definition in Definitions)
            {
                try
                {
                    ((IDefinition<DestinyDefinition>)definition.Value).LoadDefinitions();
                }
                catch(InvalidDataException)
                {
                    _ = Task.Run(UpdateDefinitions);
                }
                catch(FileNotFoundException)
                {
                    _ = Task.Run(UpdateDefinitions);
                }
            }
        }

        // Method is async because using the GetAwaiter() method cause the method call to never end;
        public async Task UpdateDefinitions()
        {
            var api = new Destiny2(settingsStore.Settings.APISettings);

            Console.WriteLine("Fetching manifests...");
            var manifest = await api.GetDestinyManifest();
            var definitionDBURL = manifest.MobileWorldContentPaths;

            Console.WriteLine("Now Downloading Manifest...");
            IsUpdating = true;

            MemoryStream ms;

            try
            {
                // TODO: Allow language changes later on
                byte[] bytes = await api.SendRequest(new Uri($"https://bungie.net{definitionDBURL["en"]}"));
                ms = new MemoryStream(bytes);
            }
            catch(Exception E)
            {
                Console.WriteLine(E.Message);
                return;
            }

            if (!Directory.Exists(SharedPlatformSpecificVariables.TempDir))
                Directory.CreateDirectory(SharedPlatformSpecificVariables.TempDir);

            using (var archive = new ZipArchive(ms))
            {
                var entry = archive.Entries[0];
                // TODO: handle UnauthorizedAccessException
                entry.ExtractToFile(Path.Combine(SharedPlatformSpecificVariables.TempDir, "DestinyManifest.sqlite3"), true);
            }

            using (var db = new SqliteConnection($"Data Source=\"{Path.Combine(SharedPlatformSpecificVariables.TempDir, "DestinyManifest.sqlite3")}\""))
            {
                await db.OpenAsync();

                foreach (var definition in Definitions)
                {
                    ((IDefinition<DestinyDefinition>)definition.Value).UpdateDefinition(db);
                }

                await db.CloseAsync();
            }

            // This is here because M$ threw and don't close the file until the garbage collector do its job
            SqliteConnection.ClearAllPools();

            File.Delete(Path.Combine(SharedPlatformSpecificVariables.TempDir, "DestinyManifest.sqlite3"));
            Console.WriteLine("Definition Update Complete");
            IsUpdating = false;
        }
    }
}