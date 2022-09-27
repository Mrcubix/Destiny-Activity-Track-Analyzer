using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using API.Entities.Definitions;
using Microsoft.Data.Sqlite;
using ReactiveUI;
using Tracker.Shared.Static;

namespace Tracker.Shared.Stores.Component
{
    public interface IDefinition<out T> where T : DestinyDefinition 
    {
        public void LoadDefinitions();
        public void UpdateDefinition(SqliteConnection db);
    }

    public class Definition<T> : ReactiveObject, IDefinition<T> where T : DestinyDefinition
    {
        private Dictionary<ulong, T> Items { get; set; } = new();
        public string Name { get; set; } = "";
        public string FilePath { get; set; } = "";

        public Definition(string name)
        {
            this.Name = name;
            this.FilePath = Path.Combine(SharedPlatformSpecificVariables.DefinitionsDir, $"{Name}.json");
        }

        public void LoadDefinitions()
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
        }

        public void UpdateDefinition(SqliteConnection db)
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

            if (!Directory.Exists(SharedPlatformSpecificVariables.DefinitionsDir))
                Directory.CreateDirectory(SharedPlatformSpecificVariables.DefinitionsDir);

            File.WriteAllText(FilePath, JsonSerializer.Serialize(Items, SharedSerializerOptions.SerializerWriteOptions));
        }
    }
}