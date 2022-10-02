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
using Tracker.Shared.Static;
using Tracker.Shared.Stores.Component;
using Tracker.ViewModels;

namespace Tracker.Shared.Stores
{
    public class SettingsStore : ReactiveObject
    {
        private Destiny2 _api = null!;

        public bool IsKeySet => !string.IsNullOrWhiteSpace(Settings.APISettings.Key);
        
        private AppSettings _settings = new();
        public AppSettings Settings
        {
            get => _settings;
            set => this.RaiseAndSetIfChanged(ref _settings, value);
        }

        public SettingsStore()
        {
            _api = new(Settings.APISettings);
        }

        public SettingsStore(Destiny2 api)
        {
            _api = api;
        }

        public void LoadSettings(ViewModelBase defaultViewModel)
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

            Settings = new();
            Settings.UXSettings.DefaultViewModelName = defaultViewModel.GetType().Name;
            Settings.APISettings.MaxRetries = 3;
            this.RaiseAndSetIfChanged(ref _settings, Settings);

            File.WriteAllText(SharedPlatformSpecificVariables.SettingsPath, JsonSerializer.Serialize(Settings, SharedSerializerOptions.SerializerWriteOptions));
        }

        public void SaveSettings()
        {
            if (File.Exists(SharedPlatformSpecificVariables.SettingsPath))
            {
                File.WriteAllText(SharedPlatformSpecificVariables.SettingsPath, JsonSerializer.Serialize(Settings, SharedSerializerOptions.SerializerWriteOptions));
                _ = Task.Run(Refresh);
            }
        }

        public async Task Refresh()
        {
            this.RaiseAndSetIfChanged(ref _settings, Settings);

            _api = new(Settings.APISettings);

            List<UserInfoCard> users = new();
            try
            {
                users = await _api.SearchDestinyPlayerByBungieName(Settings.APISettings.Username, Settings.APISettings.Tag);
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

            Settings.UXSettings.Characters = (await _api.GetProfile(user.MembershipType, user.GetMembershipId(), DestinyComponentType.Characters)).Characters.Data;

            Console.WriteLine("Settings refreshed");
        }
    }
}