using System;
using System.IO;
using System.Text.Json;
using Tracker.Shared.Static;
using Tracker.Shared.Stores.Component;
using Tracker.ViewModels;

namespace Tracker.Shared.Stores
{
    public class SettingsStore
    {
        public AppSettings Settings { get; set; } = new();

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
                    File.Move(SharedPlatformSpecificVariables.SettingsPath, Path.Combine(SharedPlatformSpecificVariables.BaseDir, "Settings.json.bak"));
                    Console.WriteLine($"Settings file may be corrupted or invalid, a backup was created in {SharedPlatformSpecificVariables.BaseDir}");
                }
            }

            Settings = new();
            Settings.UXSettings.DefaultViewModelName = defaultViewModel.GetType().Name;
            Settings.APISettings.MaxRetries = 3;
            File.WriteAllText(SharedPlatformSpecificVariables.SettingsPath, JsonSerializer.Serialize(Settings, SharedSerializerOptions.SerializerWriteOptions));
        }
    }
}