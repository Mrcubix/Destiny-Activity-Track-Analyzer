using System;
using System.IO;
using System.Text.Json;
using Tracker.Shared.Backend;

namespace Tracker.Shared
{
    public static class SharedSettingsStore
    {
        public static AppSettings Settings { get; set; } = new();

        public static void LoadSettings()
        {
            if (!Directory.Exists(SharedPlatformSpecificVariables.BaseDir))
                Directory.CreateDirectory(SharedPlatformSpecificVariables.BaseDir);

            if (File.Exists(SharedPlatformSpecificVariables.SettingsPath))
            {
                var serializedSettings = File.ReadAllText(SharedPlatformSpecificVariables.SettingsPath);
                
                try
                {
                    Settings = JsonSerializer.Deserialize<AppSettings>(serializedSettings) ?? throw new JsonException("Failed to deserialize settings");
                    return;
                }
                catch(JsonException)
                {
                    File.Move(SharedPlatformSpecificVariables.SettingsPath, Path.Combine(SharedPlatformSpecificVariables.BaseDir, "Settings.json.bak"));
                    Console.WriteLine($"Settings file may be corrupted or invalid, a backup was created in {SharedPlatformSpecificVariables.BaseDir}");
                }
            }

            File.Create(SharedPlatformSpecificVariables.SettingsPath);
            Settings = new();
        }
    }
}