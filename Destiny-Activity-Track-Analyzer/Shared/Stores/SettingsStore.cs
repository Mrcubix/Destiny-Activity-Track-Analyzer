using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Authentication;
using System.Text.Json;
using System.Threading.Tasks;
using API.Endpoints;
using API.Entities.User;
using API.Enums;
using ReactiveUI;
using Tracker.Shared.Interfaces;
using Tracker.Shared.Static;
using Tracker.Shared.Stores.Component;
using Tracker.ViewModels;

namespace Tracker.Shared.Stores
{
    public class SettingsStore :  ReactiveObject, IStore
    {
        private Destiny2 api = null!;

        private bool _isKeySet;
        private AppSettings _settings = new();

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
        ///   Property containing all of the app's settings
        /// </Summary>
        public AppSettings Settings
        {
            get => _settings;
            set => this.RaiseAndSetIfChanged(ref _settings, value);
        }


        public SettingsStore()
        {
            this.api = new(Settings.APISettings);
        }

        public SettingsStore(ViewModelBase vm)
        {
            this.api = new(Settings.APISettings);
            Settings.UXSettings.DefaultViewModelName = vm.GetType().Name;
        }

        public SettingsStore(Destiny2 api)
        {
            this.api = api;
        }

        public SettingsStore(Destiny2 api, ViewModelBase vm)
        {
            this.api = api;
            Settings.UXSettings.DefaultViewModelName = vm.GetType().Name;
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
                    return;
                }
                catch(JsonException)
                {
                    File.Move(SharedPlatformSpecificVariables.SettingsPath, Path.Combine(SharedPlatformSpecificVariables.BaseDir, "Settings.json.bak"), true);
                    Console.WriteLine($"Settings file may be corrupted or invalid, a backup was created in {SharedPlatformSpecificVariables.BaseDir}");
                }
            }

            File.WriteAllText(SharedPlatformSpecificVariables.SettingsPath, JsonSerializer.Serialize(Settings, SharedSerializerOptions.SerializerWriteOptions));

            SettingsLoaded?.Invoke(this, Settings);
        }

        /// <Summary>
        ///   Method responsible for saving the settings to <see cref="SharedPlatformSpecificVariables.SettingsPath" />
        /// </Summary>
        public void Save()
        {
            if (File.Exists(SharedPlatformSpecificVariables.SettingsPath))
            {
                File.WriteAllText(SharedPlatformSpecificVariables.SettingsPath, JsonSerializer.Serialize(Settings, SharedSerializerOptions.SerializerWriteOptions));
                _ = Task.Run(Update);
            }

            SettingsUpdated?.Invoke(this, Settings);
        }

        // The content of this should be moved elsewhere
        /// <Summary>
        ///   Method responsible for refreshing the settings
        /// </Summary>
        public async Task Update()
        {
            SettingsUpdated?.Invoke(this, Settings);

            api = new(Settings.APISettings);

            if (!IsKeySet)
                return;

            List<UserInfoCard> users = new();
            
            try
            {
                users = await api.SearchDestinyPlayerByBungieName(Settings.APISettings.Username, Settings.APISettings.Tag);
            }
            catch (InvalidCredentialException E)
            {
                Console.WriteLine(E);
                return;
            }

            if (users.Count == 0)
                return;
            
            Settings.UXSettings.UserInfo = users[0];

            var user = users[0];

            Console.WriteLine($"Found {user.BungieGlobalDisplayName}");

            Settings.UXSettings.Characters = (await api.GetProfile(user.MembershipType, user.GetMembershipId(), DestinyComponentType.Characters)).Characters.Data;

            SettingsUpdated?.Invoke(this, Settings);
        }

        /// <Summary>
        ///   Method triggered on <see cref="SettingsLoaded" /> event
        /// </Summary>
        public void OnSettingsLoadComplete(object? sender, AppSettings settings)
        {
            var key = settings.APISettings.Key;

            IsKeySet = !string.IsNullOrEmpty(key) && key.Length == 32;

            Console.WriteLine($"Settings loaded, API Key is {(IsKeySet ? "set" : "not set")}");
        }

        /// <Summary>
        ///   Method triggered on <see cref="SettingsUpdated" /> event
        /// </Summary>
        public void OnSettingsUpdateComplete(object? sender, AppSettings settings)
        {
            var key = settings.APISettings.Key;

            IsKeySet = !string.IsNullOrEmpty(key) && key.Length == 32;
            
            Console.WriteLine($"Settings updated");
        }
    }
}