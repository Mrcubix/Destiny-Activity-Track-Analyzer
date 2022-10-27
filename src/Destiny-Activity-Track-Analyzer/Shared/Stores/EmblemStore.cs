using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Authentication;
using System.Text.Json;
using System.Threading.Tasks;
using API;
using API.Endpoints;
using API.Entities.User;
using API.Enums;
using ReactiveUI;
using Tracker.Shared.Interfaces;
using Tracker.Shared.Static;
using Tracker.Shared.Stores.Component;

namespace Tracker.Shared.Stores
{
    public class EmblemStore : ReactiveObject, IStore
    {
        // TODO: All instances of API need to be moved elsewhere later on
        private Destiny2 api = null!;
        private SettingsStore settings = null!;
        private Emblem[] cachedEmblems = new Emblem[3];
        private bool hasLoaded = false;


        private UserStore _user = null!;
        private Dictionary<uint, Emblem> _emblems = new();
        private bool _isUpdating = false;


        public event EventHandler<Dictionary<uint, Emblem>> EmblemsLoaded = null!;
        public event EventHandler<Dictionary<uint, Emblem>> EmblemsUpdated = null!;


        public UserStore UserStore
        {
            get => _user;
            set => this.RaiseAndSetIfChanged(ref _user, value);
        }

        public Dictionary<uint, Emblem> Emblems
        {
            get => _emblems;
            set => this.RaiseAndSetIfChanged(ref _emblems, value);
        }

        public bool IsUpdating
        {
            get => _isUpdating;
            set => this.RaiseAndSetIfChanged(ref _isUpdating, value);
        }


        public EmblemStore(SettingsStore settings, UserStore user)
        {
            this.settings = settings;
            this.UserStore = user;
            api = new(settings.Settings.APISettings);
        }


        /// <summary>
        ///     Method responsible for initializing the EmblemStore events
        /// </summary>
        public void Initialize()
        {
            UserStore.UserUpdated += OnUserUpdateComplete;
            EmblemsLoaded += OnEmblemStoreLoadComplete;
            EmblemsUpdated += OnEmblemStoreUpdateComplete;
        }


        /// <summary>
        ///     Method responsible for loading the EmblemStore from <see cref="SharedPlatformSpecificVariables.EmblemPath" />
        /// </summary>
        public void Load()
        {
            bool shouldWrite = true;

            if (!Directory.Exists(SharedPlatformSpecificVariables.BaseDir))
                Directory.CreateDirectory(SharedPlatformSpecificVariables.BaseDir);

            if (File.Exists(SharedPlatformSpecificVariables.EmblemPath))
            {
                var serializedSettings = File.ReadAllText(SharedPlatformSpecificVariables.EmblemPath);
                
                try
                {
                    Emblems = JsonSerializer.Deserialize<Dictionary<uint, Emblem>>(serializedSettings, SharedSerializerOptions.SerializerReadOptions) ?? throw new JsonException("Failed to deserialize the EmblemStore");
                    shouldWrite = false;
                }
                catch(JsonException)
                {
                    File.Move(SharedPlatformSpecificVariables.EmblemPath, Path.Combine(SharedPlatformSpecificVariables.EmblemPath + ".bak"), true);
                    Console.WriteLine($"EmblemStore file may be corrupted or invalid, a backup was created in {Path.Combine(SharedPlatformSpecificVariables.BaseDir, "json")}");
                }
            }

            if (shouldWrite)
                File.WriteAllText(SharedPlatformSpecificVariables.EmblemPath, JsonSerializer.Serialize(Emblems, SharedSerializerOptions.SerializerWriteOptions));

            EmblemsLoaded?.Invoke(this, Emblems);
            EmblemsUpdated?.Invoke(this, Emblems);
        }

        /// <summary>
        ///   Method responsible for saving the content of the store to <see cref="SharedPlatformSpecificVariables.EmblemPath" />
        /// </summary>
        public void Save()
        {
            if (File.Exists(SharedPlatformSpecificVariables.EmblemPath))
            {
                File.WriteAllText(SharedPlatformSpecificVariables.EmblemPath, JsonSerializer.Serialize(Emblems, SharedSerializerOptions.SerializerWriteOptions));
            }
        }

        /// <Summary>
        ///   Method responsible for downloading and updating emblems
        /// </Summary>
        public async Task Update()
        {
            if(hasLoaded)
                return;

            IsUpdating = true;
            cachedEmblems = new Emblem[3];

            var shouldDownload = false;

            // STEP 3: iterate over characters and check if all emblemHash exist in UserStore.Characters

            var characters = UserStore.User.Characters;

            int i = 0;
            foreach(var entry in characters)
            {
                var character = entry.Value;
                Emblem emblem;

                if (!Emblems.ContainsKey(character.EmblemHash))
                {
                    emblem = new Emblem()
                    {
                        Hash = character.EmblemHash,
                        EmblemPath = character.EmblemPath,
                        EmblemBackgroundPath = character.EmblemBackgroundPath,
                    };

                    Emblems.Add(character.EmblemHash, emblem);
                    shouldDownload = true;
                }
                else
                {
                    emblem = Emblems[character.EmblemHash];
                }

                cachedEmblems[i] = emblem;
                i++;
            }

            // STEP 4: Invoke future CharactersUpdated event
            if (shouldDownload)
            {
                foreach(var emblem in cachedEmblems)
                {
                    if (emblem == null || emblem.Hash == 0)
                        continue;

                    await Download(emblem);
                }
            }

            Save();

            EmblemsUpdated?.Invoke(this, Emblems);
        }


        /// <Summary>
        ///   Method responsible for downloading emblems if they don't already exist
        /// </Summary>
        public async Task Download(Emblem emblem)
        {
            IsUpdating = true;

            if (!Directory.Exists(SharedPlatformSpecificVariables.EmblemDir))
                Directory.CreateDirectory(SharedPlatformSpecificVariables.EmblemDir);

            if (!Directory.Exists(emblem.Directory))
                Directory.CreateDirectory(emblem.Directory);

            // Check if the emblem already exist
            byte[] emblemBytes;
            byte[] emblemBackgroundBytes;
            byte[] emblemIconBytes;

            Console.WriteLine($"Now downloading missing emblem components for {emblem.Hash}");

            if (!string.IsNullOrEmpty(emblem.EmblemPath) && !File.Exists(emblem.EmblemFilePath))
            {
                emblemBytes = await api.SendRequest(new Uri($"https://bungie.net{emblem.EmblemPath}"));
                // save emblem to %localappdata%/DATA/Emblems/hash/emblem.png
                File.WriteAllBytes(emblem.EmblemFilePath, emblemBytes);
            }

            if (!string.IsNullOrEmpty(emblem.EmblemBackgroundPath) && !File.Exists(emblem.EmblemBackgroundFilePath))
            {
                emblemBackgroundBytes = await api.SendRequest(new Uri($"https://bungie.net{emblem.EmblemBackgroundPath}"));
                // save emblem to %localappdata%/DATA/Emblems/hash/emblem_background.png
                File.WriteAllBytes(emblem.EmblemBackgroundFilePath, emblemBackgroundBytes);
            }

            if (!string.IsNullOrEmpty(emblem.EmblemIconPath) && !File.Exists(emblem.EmblemIconFilePath))
            {
                emblemIconBytes = await api.SendRequest(new Uri($"https://bungie.net{emblem.EmblemIconPath}"));
                // save emblem to %localappdata%/DATA/Emblems/hash/emblem_icon.png
                File.WriteAllBytes(emblem.EmblemIconFilePath, emblemIconBytes);
            }

            IsUpdating = false;
            EmblemsUpdated?.Invoke(this, Emblems);
        }

        /// <Summary>
        ///   Method triggered on <see cref="UserStore.UserUpdated"/> event
        /// </Summary>
        public void OnUserUpdateComplete(object? sender, User user)
        {
            _ = Task.Run(Update);
        }

        /// <Summary>
        ///   Method triggered on <see cref="EmblemsLoaded"/> event
        /// </Summary>
        public void OnEmblemStoreLoadComplete(object? sender, Dictionary<uint, Emblem> info)
        {
            Console.WriteLine($"Successfully loaded EmblemStore");
            hasLoaded = true;
        }

        /// <Summary>
        ///   Method triggered on <see cref="EmblemsUpdated"/> event
        /// </Summary>
        public void OnEmblemStoreUpdateComplete(object? sender, Dictionary<uint, Emblem> info)
        {
            IsUpdating = false;
            Console.WriteLine($"Successfully updated EmblemStore");
        }
    }
}