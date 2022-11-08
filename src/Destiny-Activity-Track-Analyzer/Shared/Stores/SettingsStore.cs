using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using ReactiveUI;
using Tracker.Shared.Interfaces;
using Tracker.Shared.Static;
using Tracker.Shared.Stores.Component;
using Tracker.ViewModels;

namespace Tracker.Shared.Stores
{
    public class SettingsStore : ReactiveObject, IStore
    {
        //private Destiny2 api = null!;

        private bool _isKeySet;
        private AppSettings _settings = new();
        private bool _hasLoaded = false;

        public event EventHandler<AppSettings> SettingsLoaded = null!;
        public event EventHandler<AppSettings> SettingsUpdated = null!;

        /// <Summary>
        ///   State whether the API Key is set properly or not
        /// </Summary>
        public bool IsKeySet
        {
            get => _isKeySet;
            set => this.RaiseAndSetIfChanged(ref _isKeySet, value);
        }

        /// <Summary>
        ///   State whether the settings have been loaded or not
        /// </Summary>
        public bool HasLoaded
        {
            get => _hasLoaded;
            set => this.RaiseAndSetIfChanged(ref _hasLoaded, value);
        }
        
        /// <Summary>
        ///   Property containing all of the app's settings
        /// </Summary>
        public AppSettings Settings
        {
            get => _settings;
            set => this.RaiseAndSetIfChanged(ref _settings, value);
        }


        /// <Summary>
        ///   Method responsible from Initializing the SettingsStore events
        /// </Summary>
        public void Initialize() 
        { 
            SettingsLoaded += OnSettingsLoadComplete;
            SettingsUpdated += OnSettingsUpdateComplete;
        }

        /// <Summary>
        ///   Method responsible for loading the settings from <see cref="SharedPlatformSpecificVariables.SettingsPath" />
        /// </Summary>
        public void Load()
        {
            if (!Directory.Exists(SharedPlatformSpecificVariables.BaseDir))
                Directory.CreateDirectory(SharedPlatformSpecificVariables.BaseDir);

            if (File.Exists(SharedPlatformSpecificVariables.SettingsPath))
            {
                var serializedSettings = File.ReadAllText(SharedPlatformSpecificVariables.SettingsPath);
                
                try
                {
                    Settings = JsonSerializer.Deserialize<AppSettings>(serializedSettings, SharedSerializerOptions.SerializerReadOptions) ?? throw new JsonException("Failed to deserialize settings");
                }
                catch(JsonException)
                {
                    File.Move(SharedPlatformSpecificVariables.SettingsPath, Path.Combine(SharedPlatformSpecificVariables.BaseDir, "Settings.json.bak"), true);
                    Console.WriteLine($"Settings file may be corrupted or invalid, a backup was created in {SharedPlatformSpecificVariables.BaseDir}");
                }
            }
            else
            {
                File.WriteAllText(SharedPlatformSpecificVariables.SettingsPath, JsonSerializer.Serialize(Settings, SharedSerializerOptions.SerializerWriteOptions));
            }

            SettingsLoaded?.Invoke(this, Settings);
            SettingsUpdated?.Invoke(this, Settings);
        }

        /// <Summary>
        ///   Method responsible for saving the settings to <see cref="SharedPlatformSpecificVariables.SettingsPath" />
        /// </Summary>
        public void Save()
        {
            if (File.Exists(SharedPlatformSpecificVariables.SettingsPath))
            {
                File.WriteAllText(SharedPlatformSpecificVariables.SettingsPath, JsonSerializer.Serialize(Settings, SharedSerializerOptions.SerializerWriteOptions));
            }

            SettingsUpdated?.Invoke(this, Settings);
        }

        /// <Summary>
        ///   Method responsible for triggering the <see cref="SettingsUpdated" /> event
        /// </Summary>
        public void Update()
        {
            SettingsUpdated?.Invoke(this, Settings);
        }

        /// <Summary>
        ///   Method triggered on <see cref="SettingsLoaded" /> event
        /// </Summary>
        private void OnSettingsLoadComplete(object? sender, AppSettings settings)
        {
            HasLoaded = true;

            var key = settings.APISettings.Key;

            IsKeySet = !string.IsNullOrEmpty(key) && key.Length == 32;

            Console.WriteLine($"Settings loaded, API Key is {(IsKeySet ? "set" : "not set")}");
        }

        /// <Summary>
        ///   Method triggered on <see cref="SettingsUpdated" /> event
        /// </Summary>
        private void OnSettingsUpdateComplete(object? sender, AppSettings settings)
        {
            var key = settings.APISettings.Key;

            IsKeySet = !string.IsNullOrEmpty(key) && key.Length == 32;
            
            Console.WriteLine($"Settings updated");
        }
    }
}