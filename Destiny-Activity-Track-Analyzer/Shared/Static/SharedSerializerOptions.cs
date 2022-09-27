using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tracker.Shared.Static
{
    public static class SharedSerializerOptions
    {
        public static JsonSerializerOptions SerializerReadOptions { get; } = new()
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        public static JsonSerializerOptions SerializerWriteOptions { get; } = new()
        {
            WriteIndented = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };
    }
}