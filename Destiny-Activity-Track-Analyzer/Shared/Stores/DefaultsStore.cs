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
    public class DefaultsStore : ReactiveObject, IStore
    {
        private Destiny2 api = null!;
        private SettingsStore settings = null!;

        private Defaults _defaults = new();

        public event EventHandler<Defaults> DefaultsLoaded = null!;
        public event EventHandler<Defaults> DefaultsUpdated = null!;

        public Defaults Defaults
        {
            get => _defaults;
            set => this.RaiseAndSetIfChanged(ref _defaults, value);
        }

        public DefaultsStore(SettingsStore settings, ViewModelBase vm)
        {
            this.settings = settings;
            api = new(settings.Settings.APISettings);
            Defaults.DefaultViewModelName = vm.GetType().Name;
        }

        public DefaultsStore(SettingsStore settings)
        {
            this.settings = settings;
            this.api = new(settings.Settings.APISettings);
        }

        public void Initialize()
        {
            DefaultsLoaded += OnDefaultsLoadComplete;
            DefaultsUpdated += OnDefaultsUpdateComplete;
        }

        public void Load()
        {
            if (!Directory.Exists(SharedPlatformSpecificVariables.BaseDir))
                Directory.CreateDirectory(SharedPlatformSpecificVariables.BaseDir);

            if (File.Exists(SharedPlatformSpecificVariables.DefaultsPath))
            {
                var serializedSettings = File.ReadAllText(SharedPlatformSpecificVariables.DefaultsPath);
                
                try
                {
                    Defaults = JsonSerializer.Deserialize<Defaults>(serializedSettings, SharedSerializerOptions.SerializerReadOptions) ?? throw new JsonException("Failed to deserialize settings");
                    return;
                }
                catch(JsonException)
                {
                    File.Move(SharedPlatformSpecificVariables.SettingsPath, Path.Combine(SharedPlatformSpecificVariables.BaseDir, "Settings.json.bak"), true);
                    Console.WriteLine($"Settings file may be corrupted or invalid, a backup was created in {SharedPlatformSpecificVariables.BaseDir}");
                }
            }

            File.WriteAllText(SharedPlatformSpecificVariables.DefaultsPath, JsonSerializer.Serialize(Defaults, SharedSerializerOptions.SerializerWriteOptions));

            DefaultsLoaded?.Invoke(this, Defaults);
            DefaultsUpdated?.Invoke(this, Defaults);
        }

        public void Save()
        {
            if (File.Exists(SharedPlatformSpecificVariables.DefaultsPath))
            {
                File.WriteAllText(SharedPlatformSpecificVariables.DefaultsPath, JsonSerializer.Serialize(Defaults, SharedSerializerOptions.SerializerWriteOptions));
            }

            _ = Task.Run(Update);
        }

        /// <Summary>
        ///   Method responsible for refreshing the settings
        /// </Summary>
        public async Task Update()
        {
            var apiSettings = settings.Settings.APISettings;

            api = new(apiSettings);

            List<UserInfoCard> users = new();
            
            try
            {
                users = await api.SearchDestinyPlayerByBungieName(apiSettings.Username, apiSettings.Tag);
            }
            catch (InvalidCredentialException E)
            {
                Console.WriteLine(E);
                return;
            }

            if (users.Count == 0)
                return;

            //if (users.Count > 1)
            //    throw new NotImplementedException("More than one user found, this isn't handled yet");
            
            Defaults.UserInfo = users[0];

            var user = users[0];

            Console.WriteLine($"Found {user.BungieGlobalDisplayName}");

            Defaults.Characters = (await api.GetProfile(user.MembershipType, user.GetMembershipId(), DestinyComponentType.Characters)).Characters.Data;

            DefaultsUpdated?.Invoke(this, Defaults);
        }

        /// <Summary>
        ///   Method triggered on <see cref="InformationLoaded"/> event
        /// </Summary>
        public void OnDefaultsLoadComplete(object? sender, Defaults info)
        {
            Console.WriteLine($"Successfully loaded Defaults");
        }

        /// <Summary>
        ///   Method triggered on <see cref="InformationUpdated"/> event
        /// </Summary>
        public void OnDefaultsUpdateComplete(object? sender, Defaults info)
        {
            Console.WriteLine($"Successfully updated Defaults");
        }
    }
}