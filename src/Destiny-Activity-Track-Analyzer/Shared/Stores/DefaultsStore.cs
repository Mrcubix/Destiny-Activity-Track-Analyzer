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
    public class DefaultsStore : ReactiveObject, IStore
    {
        private Defaults _defaults = new();
        private bool _hasLoaded = false;

        public event EventHandler<Defaults> DefaultsLoaded = null!;
        public event EventHandler<Defaults> DefaultsUpdated = null!;

        public Defaults Defaults
        {
            get => _defaults;
            set => this.RaiseAndSetIfChanged(ref _defaults, value);
        }

        public bool HasLoaded
        {
            get => _hasLoaded;
            set => this.RaiseAndSetIfChanged(ref _hasLoaded, value);
        }


        public DefaultsStore(ViewModelBase vm)
        {
            Defaults.DefaultViewModelName = vm.GetType().Name;
        }

        public DefaultsStore()
        {
        }


        public void Initialize()
        {
            DefaultsLoaded += OnDefaultsLoadComplete;
            DefaultsUpdated += OnDefaultsUpdateComplete;
        }

        public void Load()
        {
            bool shouldWrite = true;

            if (!Directory.Exists(SharedPlatformSpecificVariables.BaseDir))
                Directory.CreateDirectory(SharedPlatformSpecificVariables.BaseDir);

            if (File.Exists(SharedPlatformSpecificVariables.DefaultsPath))
            {
                var serializedSettings = File.ReadAllText(SharedPlatformSpecificVariables.DefaultsPath);
                
                try
                {
                    Defaults = JsonSerializer.Deserialize<Defaults>(serializedSettings, SharedSerializerOptions.SerializerReadOptions) ?? throw new JsonException("Failed to deserialize defaults");
                    shouldWrite = false;
                }
                catch(JsonException)
                {
                    File.Move(SharedPlatformSpecificVariables.DefaultsPath, Path.Combine(SharedPlatformSpecificVariables.DefaultsPath + ".bak"), true);
                    Console.WriteLine($"Defaults file may be corrupted or invalid, a backup was created in {Path.Combine(SharedPlatformSpecificVariables.BaseDir, "json")}");
                }
            }

            if (shouldWrite)
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
        public void Update()
        {
            DefaultsUpdated?.Invoke(this, Defaults);
        }

        /// <Summary>
        ///   Method triggered on <see cref="InformationLoaded"/> event
        /// </Summary>
        private void OnDefaultsLoadComplete(object? sender, Defaults info)
        {
            Console.WriteLine($"Successfully loaded Defaults");
        }

        /// <Summary>
        ///   Method triggered on <see cref="InformationUpdated"/> event
        /// </Summary>
        private void OnDefaultsUpdateComplete(object? sender, Defaults info)
        {
            Console.WriteLine($"Successfully updated Defaults");
        }
    }
}