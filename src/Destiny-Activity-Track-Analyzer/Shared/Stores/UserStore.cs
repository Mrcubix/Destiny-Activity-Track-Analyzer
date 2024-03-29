using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Authentication;
using System.Text.Json;
using System.Threading.Tasks;
using API;
using API.Endpoints;
using API.Entities.Characters;
using API.Entities.User;
using API.Enums;
using ReactiveUI;
using Tracker.Shared.Interfaces;
using Tracker.Shared.Static;
using Tracker.Shared.Stores.Component;
using Tracker.ViewModels;

namespace Tracker.Shared.Stores
{
    public class UserStore : ReactiveObject, IStore
    {
        private Destiny2 api = null!;
        private SettingsStore settings = null!;
        private DefaultsStore defaults = null!;

        private User _user = new();
        private bool _isUserSet = false;
        public bool _hasLoaded = false;
        private bool _isUpdating = false;


        public event EventHandler<User> UserLoaded = null!;
        public event EventHandler<User> UserUpdated = null!;
        public event EventHandler<DestinyCharacterComponent> CurrentCharacterChanged = null!;

        public User User
        {
            get => _user;
            set => this.RaiseAndSetIfChanged(ref _user, value);
        }

        public bool IsUserSet
        {
            get => _isUserSet;
            set => this.RaiseAndSetIfChanged(ref _isUserSet, value);
        }

        public bool HasLoaded
        {
            get => _hasLoaded;
            set => this.RaiseAndSetIfChanged(ref _hasLoaded, value);
        }

        public bool IsUpdating
        {
            get => _isUpdating;
            set => this.RaiseAndSetIfChanged(ref _isUpdating, value);
        }


        public UserStore(SettingsStore settings, DefaultsStore defaults)
        {
            this.settings = settings;
            this.defaults = defaults;
            api = new(settings.Settings.APISettings);
        }


        /// <Summary>
        ///   User is outdated when one of the following is true: <br/>
        ///     - BungieGlobalDisplayName or BungieGlobalDisplayNameCode doesn't match with settings.APISettings <br/>
        /// </Summary>
        /// <Returns>
        ///   True if user is outdated, false otherwise
        /// </Returns>
        public bool IsUserOutdated()
        {
            var settings = this.settings.Settings.APISettings;
            var info = User.UserInfo;
            
            return info.BungieGlobalDisplayName != settings.Username || info.BungieGlobalDisplayNameCode != settings.Tag;
        }

        /// <Summary>
        ///   Set the <see cref="User.CurrentCharacter"/> to the specified value and invoke the <see cref="CurrentCharacterChanged"/> event. <br/>
        ///   The real purpose of this method is to be able to detect character changes even when the reference of <see cref="User"/> is changed.
        /// </Summary>
        public void SetCharacter(DestinyCharacterComponent character)
        {
            User.CurrentCharacter = character;
            CurrentCharacterChanged?.Invoke(this, character);
        }

        public void Initialize()
        {
            UserLoaded += OnUserLoadComplete;
            UserUpdated += OnUserUpdateComplete;
        }

        public void Load()
        {
            bool shouldWrite = true;

            if (!Directory.Exists(SharedPlatformSpecificVariables.BaseDir))
                Directory.CreateDirectory(SharedPlatformSpecificVariables.BaseDir);

            if (File.Exists(SharedPlatformSpecificVariables.UserPath))
            {
                var serializedSettings = File.ReadAllText(SharedPlatformSpecificVariables.UserPath);
                
                try
                {
                    User = JsonSerializer.Deserialize<User>(serializedSettings, SharedSerializerOptions.SerializerReadOptions) ?? throw new JsonException("Failed to deserialize User");
                    shouldWrite = false;
                }
                catch(JsonException)
                {
                    File.Move(SharedPlatformSpecificVariables.UserPath, Path.Combine(SharedPlatformSpecificVariables.UserPath + ".bak"), true);
                    Console.WriteLine($"Defaults file may be corrupted or invalid, a backup was created in {Path.Combine(SharedPlatformSpecificVariables.BaseDir, "json")}");
                }
            }

            if (shouldWrite)
                File.WriteAllText(SharedPlatformSpecificVariables.UserPath, JsonSerializer.Serialize(User, SharedSerializerOptions.SerializerWriteOptions));

            SetCharacter(defaults.Defaults.DefaultCharacter);

            UserLoaded?.Invoke(this, User);
            UserUpdated?.Invoke(this, User);
        }

        public void Save()
        {
            if (File.Exists(SharedPlatformSpecificVariables.UserPath))
            {
                File.WriteAllText(SharedPlatformSpecificVariables.UserPath, JsonSerializer.Serialize(User, SharedSerializerOptions.SerializerWriteOptions));
            }

            _ = Task.Run(Update);
        }

        /// <Summary>
        ///   Method responsible for refreshing the settings
        /// </Summary>
        public async Task Update()
        {
            IsUpdating = true;

            var apiSettings = settings.Settings.APISettings;

            api = new(apiSettings);

            if (!settings.IsKeySet)
                return;

            if (!IsUserOutdated())
                return;

            // STEP 1: Check if the user is the same as current

            await UpdateUserInfo(apiSettings);

            // STEP 2: Fetch the characters for the user

            await UpdateCharacters(User.UserInfo);

            // STEP 3: Invoke future CharactersUpdated event
            // EmblemStore will then start downloading emblems (Don't forget to set the state)

            // TODO: EmblemStore

            // CharactersUpdated?.Invoke(this, User.Characters);

            UserUpdated?.Invoke(this, User);
        }

        public async Task<bool> UpdateUserInfo(APISettings settings)
        {
            List<UserInfoCard> users = new();
            
            try
            {
                users = await api.SearchDestinyPlayerByBungieName(settings.Username, settings.Tag);
            }
            catch (InvalidCredentialException E)
            {
                Console.WriteLine(E);
                IsUpdating = false;
                return false;
            }

            // SUBSTEP 1: Check the number of results using the specified tag
            // Count == 0 : No user found, just skip
            // Count == 1 : User found, check if the user is the same as current
            // Count > 1 : Multiple users found, prompt the user to select one (May be the same player, but different platform)

            if (users.Count == 0)
            {
                IsUpdating = false;
                return false;
            }

            // TODO: Handle cases where multiple users are found

            //if (users.Count > 1)
            //    throw new NotImplementedException("More than one user found, this isn't handled yet");
            

            // SUBSTEP 2: Check if the user is the same as current
            
            var user = users[0];

            // User fetch from the API and current user are the same, skip
            if (User.UserInfo != null && user.GetMembershipId() == User.UserInfo.GetMembershipId())
            {
                IsUpdating = false;
                return false;
            }

            User.UserInfo = users[0];

            Console.WriteLine($"Found {user.BungieGlobalDisplayName}");

            return true;
        }

        /// <Summary>
        ///   Since Something break the moment the reference of <see cref="User.Characters"/> is changed, <br/>
        ///   we need to instead replace the existing values, which is effectively slower
        /// </Summary>
        public async Task UpdateCharacters(UserInfoCard user)
        {
            // TODO: Assets related to the characters need to be downloaded
            // Emblem Background res: 395 x 80 px

            // Reference cannot be replaced or else Avalonia will throw
            User.Characters.Clear();
            var temp = (await api.GetProfile(user.MembershipType, user.GetMembershipId(), DestinyComponentType.Characters)).Characters.Data;

            foreach(var entry in temp)
                User.Characters.Add(entry.Key, entry.Value);
        }

        /// <Summary>
        ///   Method triggered on <see cref="UserLoaded"/> event
        /// </Summary>
        private void OnUserLoadComplete(object? sender, User info)
        {
            HasLoaded = true;

            var isoutdated = IsUserOutdated();

            IsUserSet = User.UserInfo != null && !isoutdated;

            Console.WriteLine($"Successfully loaded User");
        }

        /// <Summary>
        ///   Method triggered on <see cref="UserUpdated"/> event
        /// </Summary>
        private void OnUserUpdateComplete(object? sender, User info)
        {
            IsUpdating = false;
            Console.WriteLine($"Successfully updated User");
        }

        /// <Summary>
        ///   Method triggered on <see cref="UserUpdated"/> event
        /// </Summary>
        private void OnCharacterChange(object? sender, DestinyCharacterComponent character)
        {
            Console.WriteLine("Character changed");
        }
    }
}