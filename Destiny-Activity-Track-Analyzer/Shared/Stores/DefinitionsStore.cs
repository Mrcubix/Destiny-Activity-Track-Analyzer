using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using API.Endpoints;
using API.Entities.Definitions;
using Microsoft.Data.Sqlite;
using ReactiveUI;
using Tracker.Shared.Interfaces;
using Tracker.Shared.Static;
using Tracker.Shared.Stores.Component;

namespace Tracker.Shared.Stores
{
    public class DefinitionsStore : ReactiveObject, IStore
    {
        private event EventHandler<Dictionary<string, object>> DefinitionsLoaded = null!;
        private event EventHandler<Dictionary<string, object>> DefinitionsUpdated = null!;
        private SettingsStore settingsStore { get; set; } = null!;
        

        private bool _isUpdating = false;
        private Dictionary<string, object> _definitions = null!;
        private Definition<DestinyActivityDefinition> _activityDefinitions = new("DestinyActivityDefinition");
        private Definition<DestinyActivityModeDefinition> _activityModeDefinitions = new("DestinyActivityModeDefinition");
        private Definition<DestinyActivityTypeDefinition> _activityTypeDefinitions = new("DestinyActivityTypeDefinition");
        private Definition<DestinyClassDefinition> _classDefinitions = new("DestinyClassDefinition");


        /// <Summary>
        ///   State whether one of more Definition is getting updated
        /// </Summary>
        public bool IsUpdating
        {
            get => _isUpdating;
            set => this.RaiseAndSetIfChanged(ref _isUpdating, value);
        }

        /// <Summary>
        ///   A Dictionary containing every definitions in DefinitionStore as Definition Name : Definition
        /// </Summary>
        public Dictionary<string, object> Definitions
        {
            get => _definitions;
            set => this.RaiseAndSetIfChanged(ref _definitions, value);
        }
        
        /// <Summary>
        ///   A Definition containing every Activities in the game
        /// </Summary>
        public Definition<DestinyActivityDefinition> ActivityDefinitions 
        {
            get => _activityDefinitions;
            set => this.RaiseAndSetIfChanged(ref _activityDefinitions, value);
        }
        
        /// <Summary>
        ///   A Definition containing every Activity Modes in the game
        /// </Summary>
        public Definition<DestinyActivityModeDefinition> ActivityModeDefinitions 
        {
            get => _activityModeDefinitions;
            set => this.RaiseAndSetIfChanged(ref _activityModeDefinitions, value);
        }
        
        /// <Summary>
        ///   A Definition containing every Activity Types in the game
        /// </Summary>
        public Definition<DestinyActivityTypeDefinition> ActivityTypeDefinitions 
        {
            get => _activityTypeDefinitions;
            set => this.RaiseAndSetIfChanged(ref _activityTypeDefinitions, value);
        }
        
        /// <Summary>
        ///   A Definition containing every Classes in the game
        /// </Summary>
        public Definition<DestinyClassDefinition> ClassDefinitions 
        {
            get => _classDefinitions;
            set => this.RaiseAndSetIfChanged(ref _classDefinitions, value);
        }


        public DefinitionsStore(SettingsStore settings)
        {
            settingsStore = settings;
        }


        /// <Summary>
        ///   Initiliaze the dictionary of definitions
        /// </Summary>
        public void Initialize()
        {
            Definitions = new()
            {
                ["DestinyActivityDefinition"] = ActivityDefinitions,
                ["DestinyActivityModeDefinition"] = ActivityModeDefinitions,
                ["DestinyActivityTypeDefinition"] = ActivityTypeDefinitions,
                ["DestinyClassDefinition"] = ClassDefinitions
            };

            foreach(IStore definition in Definitions.Values)
                definition.Initialize();

            DefinitionsLoaded += OnLoadCompletion;
            DefinitionsUpdated += OnUpdateCompletion;
        }

        // TODO: This need a refactor
        // - make method void
        // - remove await, 
        // - when an update need to occur, check if IsUpdating is true, if so, continue

        // Done: Now Review?
        // There is still the potential issue of a Definition being loaded for nothing because it's going to get updated anyway
        
        /// <Summary>
        ///   Load Definitions from their respective files <see cref="Definition{T}.Load"/>
        /// </Summary>
        public void Load()
        {
            if (Definitions == null)
                return;

            foreach(var definition in Definitions)
            {
                try
                {
                    ((IDefinition<DestinyDefinition>)definition.Value).Load();
                }
                catch(InvalidDataException)
                {
                    if (!IsUpdating)
                        _ = Task.Run(Update);
                }
                catch(FileNotFoundException)
                {
                    if (!IsUpdating)
                        _ = Task.Run(Update);
                }
            }

            DefinitionsLoaded?.Invoke(this, Definitions);
        }

        public void Save() { }

        // Method is async because using the GetAwaiter() method cause the method call to never end;
        // Need to be called on the UI Thread but that's meh

        /// <Summary>
        ///   Method responsible for fetching, download and updating every Definitions <see cref="Definition{T}.Update"/>
        /// </Summary>
        public async Task Update()
        {
            if (Definitions == null)
                return;
                
            var api = new Destiny2(settingsStore.Settings.APISettings);

            IsUpdating = true;

            Console.WriteLine("Fetching manifests...");
            var manifest = await api.GetDestinyManifest();
            var definitionDBURL = manifest.MobileWorldContentPaths;

            Console.WriteLine("Now Downloading Manifest...");

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
                    ((IDefinition<DestinyDefinition>)definition.Value).Update(db);
                }

                await db.CloseAsync();
            }

            // This is here because M$ threw and don't close the file until the garbage collector do its job
            SqliteConnection.ClearAllPools();

            File.Delete(Path.Combine(SharedPlatformSpecificVariables.TempDir, "DestinyManifest.sqlite3"));
           
            DefinitionsUpdated?.Invoke(this, Definitions);
        }

        /// <Summary>
        ///   Method triggered on <see cref="DefinitionsLoaded"/> event
        /// </Summary>
        public void OnLoadCompletion(object? sender, Dictionary<string, object> definitions)
        {
            Console.WriteLine("Definition Load Complete");
        }

        /// <Summary>
        ///   Method triggered on <see cref="DefinitionsLoaded"/> event
        /// </Summary>
        public void OnUpdateCompletion(object? sender, Dictionary<string, object> definitions)
        {
            Console.WriteLine("Definition Update Complete");
            IsUpdating = false;
        }
    }
}