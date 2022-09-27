using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Tracker.Shared.Static
{
    public static class SharedPlatformSpecificVariables
    {
        public static OSPlatform Platform { get; } = GetPlatform();
        public static string BaseDir { get; } = Path.Combine(GetLocalAppdataPath(), "DATA");
        public static string SettingsPath { get; } = Path.Combine(BaseDir, "Settings.json");
        public static string DefinitionsDir { get; } = Path.Combine(BaseDir, "json", "Definitions");
        public static string TempDir { get; } = Path.Combine(BaseDir, "temp");

        public static OSPlatform GetPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return OSPlatform.Windows;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return OSPlatform.Linux;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return OSPlatform.OSX;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
                return OSPlatform.FreeBSD;
            
            return OSPlatform.Create("Unknown");
        }

        public static string GetLocalAppdataPath()
        {
            try
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            }
            catch (PlatformNotSupportedException)
            {
                throw new PlatformNotSupportedException("DATA does not support this platform");
            }
        }
    }
}