using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using API.Entities.Definitions;
using Microsoft.Data.Sqlite;
using ReactiveUI;
using Tracker.Shared.Interfaces;
using Tracker.Shared.Static;

namespace Tracker.Shared.Stores.Component
{
    public interface IDefinition<out T> : IStore where T : DestinyDefinition 
    {
        public void Update(SqliteConnection db);
    }

    public class Definition<T> : ReactiveObject, IDefinition<T> where T : DestinyDefinition
    {
        private Dictionary<uint, T> _items = new();
        private string _name = "";
        private string _filePath = "";
        private bool _isEmpty = true;

        public event EventHandler<Dictionary<uint, T>> DefinitionLoaded = null!;
        public event EventHandler<Dictionary<uint, T>> DefinitionUpdated = null!;

        /// <Summary>
        ///   Dictionary Containing all activities as hash : <see cref="{DestinyDefinition}"/>
        /// </Summary>
        public Dictionary<uint, T> Items
        {
            get => _items;
            set => this.RaiseAndSetIfChanged(ref _items, value);
        }

        /// <Summary>
        ///   Name of the definition in question
        /// </Summary>
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        /// <Summary>
        ///   Path toward the file containing the definition
        /// </Summary>
        public string FilePath
        {
            get => _filePath;
            set => this.RaiseAndSetIfChanged(ref _filePath, value);
        }

        /// <Summary>
        ///   Property existing for the sole purpose of knowing if the definition is empty inside a control
        /// </Summary>
        public bool IsEmpty
        {
            get => _isEmpty;
            set => this.RaiseAndSetIfChanged(ref _isEmpty, value);
        }


        public Definition(string name)
        {
            this.Name = name;
            this.FilePath = Path.Combine(SharedPlatformSpecificVariables.DefinitionsDir, $"{Name}.json");
        }


        /// <Summary>
        ///   Method responsible from Initializing the Definition's event
        /// </Summary>
        public void Initialize()
        {
            DefinitionLoaded += OnDefinitionLoadComplete;
            DefinitionUpdated += OnDefinitionUpdateComplete;
        }

        /// <Summary>
        ///   Method responsible from loading the definition from <see cref="FilePath"/>
        /// </Summary>
        public void Load()
        {
            if (string.IsNullOrEmpty(FilePath))
                throw new Exception("FilePath is null or empty");

            if (!Directory.Exists(SharedPlatformSpecificVariables.DefinitionsDir))
                Directory.CreateDirectory(SharedPlatformSpecificVariables.DefinitionsDir);

            if (!File.Exists(FilePath))
                throw new FileNotFoundException("Definition file not found", FilePath);

            string serializedDefinitions = File.ReadAllText(FilePath);

            var definitions = JsonSerializer.Deserialize<Dictionary<uint, T>>(serializedDefinitions, SharedSerializerOptions.SerializerReadOptions);

            if (definitions == null)
                throw new InvalidDataException("Definition file is invalid");

            Items = definitions;

            DefinitionLoaded?.Invoke(this, Items);
        }

        /// <Summary>
        ///   Method responsible from saving the definition to <see cref="FilePath"/>
        /// </Summary>
        public void Save()
        {
            if (string.IsNullOrEmpty(FilePath))
                throw new Exception("FilePath is null or empty");

            if (!Directory.Exists(SharedPlatformSpecificVariables.DefinitionsDir))
                Directory.CreateDirectory(SharedPlatformSpecificVariables.DefinitionsDir);

            string serializedDefinitions = JsonSerializer.Serialize(Items, SharedSerializerOptions.SerializerWriteOptions);

            File.WriteAllText(FilePath, serializedDefinitions);
        }

        /// <Summary>
        ///   Method responsible from updating the definition from <see cref="SqliteConnection"/>
        /// </Summary>
        public void Update(SqliteConnection db)
        {
            if (string.IsNullOrWhiteSpace(Name))
                throw new System.Exception("Definition Name is empty");

            var getDefinition = db.CreateCommand();
            getDefinition.CommandText = $"SELECT id, CAST(json as TEXT) FROM {Name}";

            using (var reader = getDefinition.ExecuteReader())
            {
                while (reader.Read())
                {
                    // M$ seems to feed 4 bytes worth of garbage on an integer which is 4 bytes
                    uint hash = (uint)((long)reader["id"] & 0xFFFFFFFF);
                    string json = reader.GetString(1);

                    var definition = JsonSerializer.Deserialize<T>(json, SharedSerializerOptions.SerializerReadOptions);

                    // TODO: Elements will need to be filtered later on to improve performance
                    if (definition == null)
                        throw new InvalidDataException("Definition data is invalid");

                    Items[hash] = definition;
                }

                reader.Close();
            }

            getDefinition.Dispose();

            this.RaisePropertyChanged("Items");
            Save();

            DefinitionUpdated?.Invoke(this, Items);
        }

        /// <Summary>
        ///   Method triggered on <see cref="DefinitionLoaded"/> event
        /// </Summary>
        public void OnDefinitionLoadComplete(object? sender, Dictionary<uint, T> definitions)
        {
            Console.WriteLine($"Successfully loaded {definitions.Count} {Name}");
            IsEmpty = definitions.Count == 0;
        }

        /// <Summary>
        ///   Method triggered on <see cref="DefinitionUpdated"/> event
        /// </Summary>
        public void OnDefinitionUpdateComplete(object? sender, Dictionary<uint, T> definitions)
        {
            Console.WriteLine($"Successfully updated {definitions.Count} {Name}");
            IsEmpty = definitions.Count == 0;
        }
    }
}