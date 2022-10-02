using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Authentication;
using System.Text.Json;
using System.Threading.Tasks;
using API.Endpoints;
using API.Entities.User;
using API.Enums;
using Avalonia;
using Avalonia.Platform;
using ReactiveUI;
using Tracker.Shared.Static;
using Tracker.Shared.Stores.Component;

namespace Tracker.Shared.Stores
{
    public class IconStore : ReactiveObject
    {
        Dictionary<uint, string> Icons { get; set; }

        public void LoadIcons()
        {
            if (!Directory.Exists(SharedPlatformSpecificVariables.BaseDir))
                Directory.CreateDirectory(SharedPlatformSpecificVariables.BaseDir);

            if (File.Exists(SharedPlatformSpecificVariables.SettingsPath))
            {
                var serializedSettings = File.ReadAllText(SharedPlatformSpecificVariables.SettingsPath);
                
                try
                {
                    Icons = JsonSerializer.Deserialize<Dictionary<uint, string>>(serializedSettings, SharedSerializerOptions.SerializerReadOptions) ?? throw new JsonException("Failed to deserialize settings");
                    return;
                }
                catch(JsonException)
                {
                    File.Move(SharedPlatformSpecificVariables.SettingsPath, Path.Combine(SharedPlatformSpecificVariables.BaseDir, "Settings.json.bak"), true);
                    Console.WriteLine($"Settings file may be corrupted or invalid, a backup was created in {SharedPlatformSpecificVariables.BaseDir}");
                }
            }
        }
    }
}